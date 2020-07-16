using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Globalization;

namespace DockerForm
{
    public partial class Form1 : Form
    {
        // Global vars
        public static bool IsRunning = true;
        public static bool prevDockStatus = false;
        public static bool DockStatus = false;
        public static bool IsNvidia = false;
        public static bool IsAmd = false;
        public static int prevGPUCount = 0;
        public static int GPUCount = 0;
        public static bool IsFirstBoot = true;
        public static string iGPU = "iGPU";
        public static string eGPU = "eGPU";

        // Configurable vars
        public static bool MinimizeOnStartup = false;
        public static bool MinimizeOnClosing = false;
        public static bool BootOnStartup = false;
        public static bool ForceClose = false;
        public static bool MonitorProcesses = false;
        public static bool ToastNotifications = false;
        public static int IGDBListLength;

        // Devices vars
        public static Dictionary<string, VideoController> VideoControllers = new Dictionary<string, VideoController>();

        // Folder vars
        public static string path_application, path_database;

        // Form vars
        private static Form1 _instance;
        private static Thread ThreadGPU, ThreadDB;

        public static void SendNotification(string input, bool pushToast)
        {
            if (!ToastNotifications)
                return;

            _instance.BeginInvoke(new Action(() => _instance.debugTextBox.Text = input));

            if (pushToast)
            {
                _instance.BeginInvoke(new Action(() => _instance.notifyIcon1.BalloonTipText = input));
                _instance.BeginInvoke(new Action(() => _instance.notifyIcon1.ShowBalloonTip(1000)));
            }
        }

        public static void StatusMonitor(object data)
        {
            while (IsRunning)
            {
                UpdateGameDatabase();
                Thread.Sleep(1000);
            }
        }

        public static void UpdateGameDatabase()
        {
            if (prevDockStatus != DockStatus || prevGPUCount != GPUCount)
            {
                try { _instance.Invoke(new Action(delegate () { UpdateFormIcons(); })); } catch (Exception) { }

                if (!IsFirstBoot)
                    DatabaseManager.UpdateFilesAndRegistries(DockStatus);
                else
                    DatabaseManager.SanityCheck();

                prevDockStatus = DockStatus;
                prevGPUCount = GPUCount;

                if (IsFirstBoot)
                    IsFirstBoot = false;
            }
        }

        public static void ProcessMonitor(object data)
        {
            while (IsRunning)
            {
                try
                {
                    var wmiQueryString = "SELECT ProcessId, ExecutablePath FROM Win32_Process";
                    using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                    using (var results = searcher.Get())
                    {
                        var query = from p in Process.GetProcesses()
                                    join mo in results.Cast<ManagementObject>()
                                    on p.Id equals (int)(uint)mo["ProcessId"]
                                    select new
                                    {
                                        Process = p,
                                        Path = (string)mo["ExecutablePath"],
                                    };
                        foreach (var item in query)
                        {
                            foreach (DockerGame game in DatabaseManager.GameDB.Values)
                            {
                                string gamepath = Path.Combine(game.Uri, game.Executable).ToLower();
                                string procpath = item.Path != null ? item.Path.ToLower() : "";

                                if (gamepath != procpath)
                                    continue;

                                if (!DatabaseManager.GameProcesses.ContainsKey(game))
                                    DatabaseManager.GameProcesses.Add(game, item.Process);
                            }
                        }
                    }

                    for (int i = 0; i < DatabaseManager.GameProcesses.Count; i++)
                    {
                        KeyValuePair<DockerGame, Process> pair = DatabaseManager.GameProcesses.ElementAt(i);

                        Process proc = pair.Value;
                        DockerGame game = pair.Key;

                        if (proc.HasExited)
                        {
                            // Update current title
                            string path_game = DockStatus ? eGPU : iGPU;
                            DatabaseManager.UpdateFilesAndRegistries(game, path_game, path_game, true, false);

                            DatabaseManager.GameProcesses.Remove(game);
                        }
                    }
                }
                catch (Exception) { }
                Thread.Sleep(1000);
            }
        }

        public static void VideoControllerMonitor(object data)
        {
            while (IsRunning)
            {
                try
                {
                    DateTime currentCheck = DateTime.Now;

                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        VideoController vc = new VideoController
                        {
                            DeviceID = (string)mo.Properties["DeviceID"].Value,
                            Name = (string)mo.Properties["Name"].Value,
                            Description = (string)mo.Properties["Description"].Value,
                            ConfigManagerErrorCode = (uint)mo.Properties["ConfigManagerErrorCode"].Value,
                            lastCheck = currentCheck
                        };

                        if (!vc.IsEnable())
                            continue;

                        if (!VideoControllers.ContainsKey(vc.DeviceID))
                            VideoControllers.Add(vc.DeviceID, vc);
                        else
                            VideoControllers[vc.DeviceID].lastCheck = currentCheck;
                    }

                    for (int i = 0; i < VideoControllers.Count; i++)
                    {
                        KeyValuePair<string, VideoController> pair = VideoControllers.ElementAt(i);

                        string DeviceID = pair.Key;
                        VideoController vc = pair.Value;

                        if (vc.lastCheck < currentCheck)
                            VideoControllers.Remove(DeviceID);

                        if (vc.IsIntegrated())
                            iGPU = vc.Name;
                        else
                            eGPU = vc.Name;
                    }

                    DockStatus = (VideoControllers.Count != 1);

                    if(DockStatus)
                    {
                        IsNvidia = eGPU.ToLower().Contains("nvidia");
                        IsAmd = eGPU.ToLower().Contains("amd");
                    }

                    GPUCount = VideoControllers.Count;
                }
                catch (Exception) { }
                Thread.Sleep(1000);
            }
        }

        public static void UpdateFormIcons()
        {
            // taskbar text
            _instance.menuStrip2.Items[0].Text = DockStatus ? eGPU : iGPU;

            // taskbar icon
            _instance.undockedToolStripMenuItem.Image = DockStatus ? (IsNvidia ? Properties.Resources.nvidia : Properties.Resources.amd) : Properties.Resources.intel;

            // main application icon
            Bitmap bmp = DockStatus ? Properties.Resources.thunderbolt : Properties.Resources.thunderbolt_off;
            IntPtr Hicon = bmp.GetHicon();
            Icon myIcon = Icon.FromHandle(Hicon);
            _instance.notifyIcon1.Icon = myIcon;
            _instance.Icon = myIcon;
        }

        public void UpdateGameItem(DockerGame game, bool ForceUpdate = false)
        {
            exListBoxItem newitem = new exListBoxItem(game);

            if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
            {
                GameList.Items.Add(newitem);
                DatabaseManager.GameDB[game.GUID] = game;
            }
            else if (ForceUpdate)
            {
                for (int i = 0; i < GameList.Items.Count; i++)
                {
                    exListBoxItem item = (exListBoxItem)GameList.Items[i];
                    if (item.Guid == game.GUID)
                    {
                        GameList.Items[i] = newitem;
                        DatabaseManager.GameDB[game.GUID] = game;
                        break;
                    }
                }
            }

            // Update current title
            string path_game = DockStatus ? eGPU : iGPU;
            DockerGame output = DatabaseManager.GameDB[game.GUID];
            DatabaseManager.UpdateFilesAndRegistries(output, path_game, path_game, true, false);

            GameList.Sort();
        }

        public void UpdateGameList()
        {
            // Read all the game files (xml)
            string[] fileEntries = Directory.GetFiles(path_database, "*.dat");
            foreach (string filename in fileEntries)
            {
                using (Stream reader = new FileStream(filename, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    DockerGame game = (DockerGame)formatter.Deserialize(reader);
                    game.SanityCheck();

                    if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
                        DatabaseManager.GameDB.AddOrUpdate(game.GUID, game, (key, value) => game);

                    reader.Dispose();
                }
            }

            // Update the DockerGame database
            GameList.BeginUpdate();
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                exListBoxItem item = new exListBoxItem(game);
                GameList.Items.Add(item);
                item.Enabled = game.Enabled;
            }
            GameList.EndUpdate();

            GameList.Sort();
        }

        public int GetIGDBListLength()
        {
            return IGDBListLength;
        }

        public Form1()
        {
            InitializeComponent();

            // Change current culture
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            _instance = this;

            // folder settings
            path_application = AppDomain.CurrentDomain.BaseDirectory;

            // path settings
            path_database = Path.Combine(path_application, "db");

            if (!Directory.Exists(path_database))
                Directory.CreateDirectory(path_database);

            // configurable settings
            GameList.SetSize(Math.Max(32, Properties.Settings.Default.ImageHeight), Math.Max(32, Properties.Settings.Default.ImageWidth));
            MinimizeOnStartup = Properties.Settings.Default.MinimizeOnStartup;
            MinimizeOnClosing = Properties.Settings.Default.MinimizeOnClosing;
            BootOnStartup = Properties.Settings.Default.BootOnStartup;
            MonitorProcesses = Properties.Settings.Default.MonitorProcesses;
            IGDBListLength = Properties.Settings.Default.IGDBListLength;
            ToastNotifications = Properties.Settings.Default.ToastNotifications;

            if (MinimizeOnStartup)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }

            RegistryManager.StartupManager(BootOnStartup);
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            // draw GameDB
            UpdateGameList();

            // thread settings
            ThreadGPU = new Thread(VideoControllerMonitor);
            ThreadDB = new Thread(StatusMonitor);

            ThreadGPU.Start();
            ThreadDB.Start();

            if (MonitorProcesses)
            {
                Thread ThreadEXE = new Thread(ProcessMonitor);
                ThreadEXE.Start();
            }
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (MinimizeOnClosing && e.CloseReason == CloseReason.UserClosing && !ForceClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                notifyIcon1.Dispose();
                IsRunning = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForceClose = true;
            Close();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                SendNotification(Text + " is running in the background.", !IsFirstBoot);
            }
        }

        private void GameList_MouseDown(object sender, MouseEventArgs e)
        {
            if (GameList.Items.Count == 0)
            {
                contextMenuStrip1.Enabled = false;
            }
            else
            {
                contextMenuStrip1.Enabled = true;

                Point pt = new Point(e.X, e.Y);
                int index = GameList.IndexFromPoint(pt);
                switch (e.Button)
                {
                    case MouseButtons.Right:
                        GameList.SelectedIndex = index;
                        if (GameList.SelectedItem != null)
                        {
                            exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                            DockerGame game = DatabaseManager.GameDB[item.Guid];

                            navigateToIGDBEntryToolStripMenuItem.Enabled = (game.IGDB_Url != "");

                            // Sanity checks
                            openToolStripMenuItem.Enabled = game.HasReachableFolder();
                            toolStripStartItem.Enabled = game.HasReachableExe();
                            toolStripMenuItem1.Enabled = game.HasFileSettings();

                            toolStripMenuItem1.DropDownItems.Clear();
                            foreach (GameSettings setting in game.Settings.Values.Where(a => a.IsFile()))
                            {
                                string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));
                                FileInfo fileinfo = new FileInfo(filename);

                                ToolStripMenuItem newItem = new ToolStripMenuItem();
                                newItem.Text = fileinfo.Name;
                                newItem.ToolTipText = fileinfo.DirectoryName;
                                newItem.Click += new EventHandler(MenuItemClickHandler);
                                toolStripMenuItem1.DropDownItems.Add(newItem);
                            }
                        }
                        break;
                }
            }
        }

        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string folderPath = item.ToolTipText;
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            };
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DisplayForm();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayForm();
        }

        private void DisplayForm()
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
        }

        private void OpenGameFolder(object sender, EventArgs e)
        {
            exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
            DockerGame game = DatabaseManager.GameDB[item.Guid];

            string folderPath = game.Uri;
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            };
        }

        private void navigateToIGDBEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
            DockerGame game = DatabaseManager.GameDB[item.Guid];
            Process.Start(game.IGDB_Url);
        }

        private void exittoolStripStartItem_Click(object sender, EventArgs e)
        {
            ForceClose = true;
            Close();
        }

        private void toolStripStartItem_Click(object sender, EventArgs e)
        {
            exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
            DockerGame game = DatabaseManager.GameDB[item.Guid];
            string filename = Path.Combine(game.Uri, game.Executable);

            Process.Start(filename);
        }

        private void removeTheGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                DockerGame game = DatabaseManager.GameDB[item.Guid];

                DialogResult dialogResult = MessageBox.Show("This will remove " + game.Name + " from this database.", "Remove Title ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string filename = Path.Combine(path_database, game.FolderName + ".dat");
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                        DatabaseManager.GameDB.TryRemove(item.Guid, out game);
                        GameList.Items.Remove(item);
                    }
                }
            }
        }

        private void automaticDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> DetectedGames = new List<string>();
            DetectedGames.AddRange(DatabaseManager.SearchBattleNet());

            foreach(string uri in DetectedGames)
            {
                DockerGame thisGame = new DockerGame(uri);
                thisGame.SanityCheck();
                UpdateGameItem(thisGame);
            }
        }

        private void findAGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings currentSettings = new Settings(this);

            // only display the settings window when an executable has been picked.
            if (currentSettings.GetIsReady())
                currentSettings.Show();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                Settings currentSettings = new Settings(this, DatabaseManager.GameDB[item.Guid]);
                currentSettings.Show();
            }
        }
    }
}
