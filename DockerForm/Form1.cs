using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Concurrent;

namespace DockerForm
{
    public partial class Form1 : Form
    {
        // Global vars
        public static bool IsRunning = true;
        public static bool prevDockStatus = false;
        public static bool DockStatus = false;
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
        public static string path_application, path_storage, path_artworks, path_database;

        // Form vars
        private static Form1 _instance;

        public static void NewToastNotification(string input)
        {
            if (!ToastNotifications)
                return;

            _instance.Invoke(new Action(delegate () {
                _instance.notifyIcon1.BalloonTipText = input;
                _instance.debugTextBox.Text = input;
                _instance.notifyIcon1.ShowBalloonTip(1000);
            }));
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
            if (prevDockStatus != DockStatus)
            {
                UpdateFormIcons();

                if (!IsFirstBoot)
                    DatabaseManager.UpdateFilesAndRegistries(DockStatus);
                else
                    DatabaseManager.SanityCheck();

                prevDockStatus = DockStatus;

                if (IsFirstBoot)
                    IsFirstBoot = false;
            }
        }

        public static void ProcessMonitor(object data)
        {
            while (IsRunning)
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
                            if (gamepath == procpath)
                                DatabaseManager.GameProcesses.AddOrUpdate(item.Process, game, (key, value) => game);
                        }
                    }
                }

                for(int i = 0; i < DatabaseManager.GameProcesses.Count; i++)
                {
                    KeyValuePair<Process, DockerGame> pair = DatabaseManager.GameProcesses.ElementAt(i);

                    Process proc = pair.Key;
                    DockerGame game = pair.Value;

                    if (proc.HasExited)
                    {
                        DatabaseManager.UpdateFilesAndRegistries(game, !DockStatus, true, false);
                        DatabaseManager.GameProcesses.TryRemove(proc, out game);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public static void VideoControllerMonitor(object data)
        {
            while (IsRunning)
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

                    if (vc.IsDisable())
                        continue;

                    if (!VideoControllers.ContainsKey(vc.DeviceID))
                        VideoControllers.Add(vc.DeviceID, vc);
                    else
                        VideoControllers[vc.DeviceID].lastCheck = currentCheck;
                }

                for (int i = 0; i < VideoControllers.Count; i++)
                {
                    KeyValuePair<string, VideoController> pair = VideoControllers.ElementAt(i);

                    String DeviceID = pair.Key;
                    VideoController vc = pair.Value;

                    if (vc.lastCheck < currentCheck)
                        VideoControllers.Remove(DeviceID);

                    if (vc.IsIntegrated())
                        iGPU = vc.Name;
                    else
                        eGPU = vc.Name;
                }

                DockStatus = (VideoControllers.Count != 1);

                Thread.Sleep(1000);
            }
        }

        public static void UpdateFormIcons()
        {
            _instance.Invoke(new Action(delegate () {
                _instance.menuStrip2.Items[0].Text = DockStatus ? "Docked" : "Undocked";
                _instance.notifyIcon1.Icon = DockStatus ? Properties.Resources.icon_plugged1 : Properties.Resources.icon_unplugged1;
                _instance.Icon = DockStatus ? Properties.Resources.icon_plugged1 : Properties.Resources.icon_unplugged1;
            }));
        }

        public void UpdateGameItem(DockerGame game)
        {
            exListBoxItem listgame = new exListBoxItem(game);

            if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
            {
                GameList.Items.Add(listgame);
                DatabaseManager.GameDB.AddOrUpdate(game.GUID, game, (key, value) => game);

                // We save iGPU/nextDockStatus profiles on game creation
                DatabaseManager.UpdateFilesAndRegistries(game, true, true, false);
                DatabaseManager.UpdateFilesAndRegistries(game, false, true, false);
            }
            else
            {
                for (int i = 0; i < GameList.Items.Count; i++)
                {
                    exListBoxItem item = (exListBoxItem)GameList.Items[i];
                    if (item.Guid == game.GUID)
                    {
                        GameList.Items[i] = listgame;
                        DatabaseManager.GameDB[game.GUID] = game;
                        break;
                    }
                }
            }

            GameList.Sort();
        }

        public void UpdateGameList()
        {
            // Read all the game files (xml)
            string[] fileEntries = Directory.GetFiles(path_database, "*.xml");
            foreach (string filename in fileEntries)
            {
                XmlSerializer xs = new XmlSerializer(typeof(DockerGame));
                using (Stream reader = new FileStream(filename, FileMode.Open))
                {
                    DockerGame game = (DockerGame)xs.Deserialize(reader);
                    game.Image = GetGameIcon(game.Artwork);

                    if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
                        DatabaseManager.GameDB.AddOrUpdate(game.GUID, game, (key, value) => game);

                    reader.Dispose();
                }
            }

            // Update the DockerGame database
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                exListBoxItem item = new exListBoxItem(game);
                GameList.Items.Add(item);
            }

            GameList.Sort();
        }

        public Image GetGameIcon(string Artwork)
        {
            string filename = Path.Combine(path_artworks, Artwork);
            Image GameIcon = Properties.Resources.DefaultBackgroundImage;

            if (File.Exists(filename))
            {
                using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    GameIcon = Image.FromStream(stream);
                    stream.Dispose();
                }
            }

            return GameIcon;
        }

        public void SaveGameIcon(Bitmap img, string filename)
        {
            string filepath = Path.Combine(path_artworks, filename);

            if (File.Exists(filepath))
                File.Delete(filepath);

            img.Save(filepath);
        }

        public int GetIGDBListLength()
        {
            return IGDBListLength;
        }

        public Form1()
        {
            InitializeComponent();
            _instance = this;

            // folder settings
            path_application = AppDomain.CurrentDomain.BaseDirectory;

            // path settings
            path_storage = Path.Combine(path_application, "storage");
            path_artworks = Path.Combine(path_application, "artworks");
            path_database = Path.Combine(path_application, "db");

            if (!Directory.Exists(path_storage))
                Directory.CreateDirectory(path_storage);

            if (!Directory.Exists(path_artworks))
                Directory.CreateDirectory(path_artworks);

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

            if (BootOnStartup) // dirty, we should check if entry already exists
                RegistryManager.AddApplicationToStartup();
            else
                RegistryManager.RemoveApplicationFromStartup();
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            // draw GameDB
            UpdateGameList();

            // thread settings
            Thread ThreadGPU = new Thread(VideoControllerMonitor);
            Thread ThreadDB = new Thread(StatusMonitor);

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
                IsRunning = false;
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
                NewToastNotification(Text + " is running in the background.");
            }
        }

        private void GameList_DoubleClick(object sender, System.EventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                DockerGame game = DatabaseManager.GameDB[item.Guid];

                string filename = Path.Combine(game.Uri, game.Executable);
                Process.Start(filename);
            }
        }

        private void GameList_MouseDown(object sender, MouseEventArgs e)
        {
            Point pt = new Point(e.X, e.Y);
            int index = GameList.IndexFromPoint(pt);
            switch (e.Button)
            {
                case MouseButtons.Right:
                    GameList.SelectedIndex = index;
                    if(GameList.SelectedItem != null)
                    {
                        exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                        DockerGame game = DatabaseManager.GameDB[item.Guid];

                        navigateToIGDBEntryToolStripMenuItem.Enabled = (game.IGDB_Url != "");
                    }                    
                    break;
            }
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

        private void OpenDataFolder(object sender, EventArgs e)
        {
            exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
            DockerGame game = DatabaseManager.GameDB[item.Guid];

            string folderPath = Path.Combine(path_storage, game.FolderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = folderPath,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
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

            if (File.Exists(filename)) // still fails to open %appdata% based app...
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
                    string filename = Path.Combine(path_database, game.FolderName + ".xml");
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                        DatabaseManager.GameDB.TryRemove(item.Guid, out game);
                        GameList.Items.Remove(item);
                    }
                }
            }
        }

        private void findAGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings p = new Settings(this);
            if(p.GetIsReady())
                p.Show();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                Settings p = new Settings(this, DatabaseManager.GameDB[item.Guid]);
                p.Show();
            }
        }
    }
}
