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
using System.Threading.Tasks;
using System.Collections.Specialized;
using Microsoft.Win32;

namespace DockerForm
{
    public partial class Form1 : Form
    {
        // Global vars
        public static bool prevDockStatus = false;
        public static bool DockStatus = false;
        public static bool IsFirstBoot = true;
        public static bool IsHardwareNew = false;
        public static bool IsHardwarePending = false;
        public static VideoController CurrentController;
        public static bool prevPowerStatus;
        public static bool PowerStatus;
        public static bool IsPowerNew = false;
        public static bool IsRunning = true;

        // Configurable vars
        public static bool MinimizeOnStartup = false;
        public static bool MinimizeOnClosing = false;
        public static bool BootOnStartup = false;
        public static bool ForceClose = false;
        public static bool MonitorProcesses = false;
        public static bool ToastNotifications = false;
        public static bool SaveOnExit = false;
        public static int IGDBListLength;
        public static StringCollection Blacklist;

        // Devices vars
        public static Dictionary<Type, VideoController> VideoControllers = new Dictionary<Type, VideoController>();

        // Folder vars
        public static string path_application, path_database;

        // Form vars
        private static Form1 _instance;

        // Threading vars
        private static Thread ThreadGPU;
        private static ManagementEventWatcher processStartWatcher, processStopWatcher;
        private static Dictionary<int, string> GameProcesses = new Dictionary<int, string>();

        private const int WM_DEVICECHANGE = 0x0219;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                IsHardwarePending = true;
            }
            base.WndProc(ref m);
        }

        private static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            IsHardwarePending = true;
        }

        public static void SendNotification(string input, bool pushToast = false, bool pushLog = false, bool IsError = false)
        {
            if (pushLog)
                LogManager.UpdateLog(input, IsError);

            if (pushToast && ToastNotifications)
            {
                _instance.notifyIcon1.BalloonTipText = input;
                _instance.notifyIcon1.ShowBalloonTip(1000);
            }
        }

        static private bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }

        static void startWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                int ProcessID = Int32.Parse(e.NewEvent["ProcessID"].ToString());

                if (!ProcessExists(ProcessID))
                    return;

                Process Proc = Process.GetProcessById(ProcessID);
                string PathToApp = DatabaseManager.GetPathToApp(Proc);

                if (PathToApp == string.Empty)
                    return;

                if (GameProcesses.ContainsKey(ProcessID))
                    GameProcesses.Remove(ProcessID);
                GameProcesses.Add(ProcessID, PathToApp);

            }catch(Exception ex) { }
        }

        static void stopWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                int ProcessID = Int32.Parse(e.NewEvent["ProcessID"].ToString());

                if (!GameProcesses.ContainsKey(ProcessID))
                    return;

                string PathToApp = GameProcesses[ProcessID];
                if (PathToApp == string.Empty)
                    return;

                foreach (DockerGame game in DatabaseManager.GameDB.Values)
                {
                    string game_exe = game.Executable.ToLower();
                    string game_uri = game.Uri.ToLower();

                    FileInfo info = new FileInfo(PathToApp);
                    string info_exe = info.Name.ToLower();
                    string info_uri = info.FullName.ToLower();

                    // Update current title
                    if (game_exe == info_exe || game_uri == info_uri)
                        DatabaseManager.UpdateFilesAndRegistries(game, GetCurrentState(), GetCurrentState(), true, false, true, GetCurrentState());
                }
            }
            catch (Exception ex) { }
        }

        public static string GetCurrentState()
        {
            string state = CurrentController.Name;

            /*            
             *            NVIDIA GeForce RTX 2070
             *            Intel(R) Iris(R) Plus Graphics
             *            Intel(R) Iris(R) Plus Graphics:False
            */
            if (!DockStatus && !PowerStatus)
                state += ":" + PowerStatus;

            return state;
        }

        public static string GetCurrentPower()
        {
            return PowerStatus ? "plugged in" : "on battery";
        }

        public static void VideoControllerMonitor(object data)
        {
            while(IsRunning)
            {
                if (IsHardwarePending || IsFirstBoot)
                {
                    DateTime currentCheck = DateTime.Now;
                    VideoControllers.Clear();

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

                        // skip if not enabled
                        if (vc.ConfigManagerErrorCode != 0)
                            continue;

                        // initialize VideoController
                        vc.Initialize();

                        // update VideoController
                        VideoControllers[vc.Type] = vc;
                    }

                    // update all status
                    DockStatus = VideoControllers.ContainsKey(Type.Discrete);
                    CurrentController = DockStatus ? VideoControllers[Type.Discrete] : VideoControllers[Type.Internal];
                    PowerStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;

                    // monitor hardware changes
                    IsHardwareNew = IsFirstBoot ? false : (prevDockStatus != DockStatus);
                    IsPowerNew = IsFirstBoot ? false : (prevPowerStatus != PowerStatus);

                    if (IsHardwareNew || IsPowerNew || IsFirstBoot)
                    {
                        if (IsHardwareNew)
                        {
                            if (VideoControllers.ContainsKey(Type.Discrete))
                                LogManager.UpdateLog("eGPU: " + VideoControllers[Type.Discrete].Name);
                            else if (VideoControllers.ContainsKey(Type.Internal))
                                LogManager.UpdateLog("iGPU: " + VideoControllers[Type.Internal].Name);
                        }
                        
                        if (IsPowerNew)
                            LogManager.UpdateLog("Power Status: " + GetCurrentPower());

                        if (IsFirstBoot)
                        {
                            IsFirstBoot = false;
                            DatabaseManager.SanityCheck();
                        }
                        else
                        {
                            DatabaseManager.UpdateFilesAndRegistries(false, true);
                            DatabaseManager.UpdateFilesAndRegistries(true, false);
                        }
                    }

                    // update status
                    prevDockStatus = DockStatus;
                    prevPowerStatus = PowerStatus;

                    // update form
                    UpdateFormIcons();
                    IsHardwarePending = false;
                }

                Thread.Sleep(500);
            }
        }

        public static void UpdateFormIcons()
        {
            try
            {
                // taskbar icon
                Image ConstructorLogo = Properties.Resources.intel;
                switch(CurrentController.Constructor)
                {
                    case Constructor.AMD: ConstructorLogo = Properties.Resources.amd; break;
                    case Constructor.Nvidia: ConstructorLogo = Properties.Resources.nvidia; break;
                }
                // main application icon
                Icon myIcon = DockStatus ? Properties.Resources.tb3_on : Properties.Resources.tb3_off;

                // drawing
                _instance.BeginInvoke((MethodInvoker)delegate ()
                {
                    _instance.menuStrip2.Items[0].Text = CurrentController.Name + " (" + GetCurrentPower() + ")";
                    _instance.undockedToolStripMenuItem.Image = ConstructorLogo;
                    _instance.notifyIcon1.Icon = myIcon;
                    _instance.Icon = myIcon;
                });
            }
            catch (Exception ex) { LogManager.UpdateLog("UpdateFormIcons: " + ex.Message, true); }
        }

        public void InsertOrUpdateGameItem(DockerGame game, bool Update = true)
        {
            exListBoxItem newitem = new exListBoxItem(game);

            if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
            {
                GameList.Items.Add(newitem);
                DatabaseManager.GameDB[game.GUID] = game;
                LogManager.UpdateLog("[" + game.Name + "] profile has been added to the database");
            }
            else if (Update)
            {
                for (int i = 0; i < GameList.Items.Count; i++)
                {
                    exListBoxItem item = (exListBoxItem)GameList.Items[i];
                    if (item.Guid == game.GUID)
                    {
                        GameList.Items[i] = newitem;
                        DatabaseManager.GameDB[game.GUID] = game;
                        LogManager.UpdateLog("[" + game.Name + "] profile has been updated");
                        break;
                    }
                }
            }

            // Update current title
            DockerGame output = DatabaseManager.GameDB[game.GUID];
            DatabaseManager.UpdateFilesAndRegistries(output, GetCurrentState(), GetCurrentState(), true, false, true, GetCurrentState());

            GameList.Sort();
        }

        public void UpdateGameList()
        {
            // Read all the game files (xml)
            string[] fileEntries = Directory.GetFiles(path_database, "*.dat");
            foreach (string filename in fileEntries)
            {
                try
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
                catch (Exception ex) { LogManager.UpdateLog("UpdateGameList: " + ex.Message, true); }
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

            // initialize vars
            _instance = this;

            // folder settings
            path_application = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.InitializeLog(path_application);

            // path settings
            path_database = Path.Combine(path_application, "database");

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
            SaveOnExit = Properties.Settings.Default.SaveOnExit;
            Blacklist = Properties.Settings.Default.Blacklist;

            if (MinimizeOnStartup)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            // draw GameDB
            UpdateGameList();

            // search for GPUs
            ThreadGPU = new Thread(VideoControllerMonitor);
            ThreadGPU.Start();

            // Monitor processes
            if (MonitorProcesses)
            {
                processStartWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                processStartWatcher.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
                processStartWatcher.Start();

                processStopWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                processStopWatcher.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
                processStopWatcher.Start();

                LogManager.UpdateLog("Process monitor has started");
            }

            // Monitor Power Status
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
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
                if(SaveOnExit)
                    DatabaseManager.UpdateFilesAndRegistries(false, true);

                notifyIcon1.Dispose();
                processStartWatcher.Dispose();
                processStopWatcher.Dispose();

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
                    string filename = Path.Combine(path_database, game.GUID + ".dat");

                    if (File.Exists(filename))
                        File.Delete(filename);

                    DatabaseManager.GameDB.TryRemove(item.Guid, out game);
                    GameList.Items.Remove(item);
                    LogManager.UpdateLog("[" + game.Name + "] has been removed from the database");
                }
            }
        }

        private void automaticDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DockerGame> DetectedGames = new List<DockerGame>();
            DetectedGames.AddRange(DatabaseManager.SearchBattleNet());
            DetectedGames.AddRange(DatabaseManager.SearchMicrosoftStore());

            foreach (DockerGame game in DetectedGames.Where(a => !Blacklist.Contains(a.Name)))
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to add [" + game.Name + "] to your Database ? ", "(Beta) Automatic Detection", MessageBoxButtons.YesNoCancel);
                if (dialogResult == DialogResult.Yes)
                    InsertOrUpdateGameItem(game, false);
                else if (dialogResult == DialogResult.No)
                {
                    // Properties.Settings.Default.Blacklist.Add(game.Name);
                    // Properties.Settings.Default.Save();
                }
                else if (dialogResult == DialogResult.Cancel)
                    break;
            }
        }

        private void findAGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings currentSettings = new Settings(this);

            // only display the settings window when an executable has been picked.
            if (currentSettings.GetIsReady())
            {
                DockerGame game = currentSettings.GetGame();
                if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
                    currentSettings.Show();
                else
                    SendNotification(game.Name + " already exists in your current database", true);
            }
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
