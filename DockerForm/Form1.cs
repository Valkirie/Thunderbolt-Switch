using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Serialization;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Linq;
using System.Collections.Concurrent;

namespace DockerForm
{
    public partial class Form1 : Form
    {
        // Global vars
        static bool IsRunning = true;
        static bool LastPlugged = false;
        static bool IsPlugged = false;
        static bool IsFirstBoot = true;

        // Configurable vars
        static bool MinimizeOnStartup = false;
        static bool MinimizeOnClosing = false;
        static bool BootOnStartup = false;
        static bool ForceClose = false;
        static bool MonitorProcesses = false;
        static int IGDBListLength;

        // Devices vars
        static List<string> VideoControllers = new List<string>();

        // DockerGame vars
        static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();
        static ConcurrentDictionary<Process, DockerGame> GameProcesses = new ConcurrentDictionary<Process, DockerGame>();

        // Folder vars
        public static string path_application, path_storage, path_artworks, path_database;

        // Form vars
        private static Form1 _instance;

        private static void NewToastNotification(string input)
        {
            _instance.notifyIcon1.BalloonTipText = input;
            _instance.debugTextBox.Text = input;
            _instance.notifyIcon1.ShowBalloonTip(1000);
        }

        public static void CopyFolder(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyFile(FileInfo sourceFile, string targetDirectory)
        {
            DirectoryInfo target = new DirectoryInfo(targetDirectory);

            if (!Directory.Exists(target.FullName))
                Directory.CreateDirectory(target.FullName);

            if (File.Exists(sourceFile.FullName))
                sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name), true);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if(!Directory.Exists(target.FullName))
                Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }

        public static void RestoreKey(string targetFile)
        {
            if(File.Exists(targetFile))
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "regedit.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc = Process.Start("regedit.exe", "/s " + targetFile);
            }
        }

        public static void ExportKey(string RegKey, string SavePath, string targetFile)
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            string path = "\"" + SavePath + "\\" + targetFile + "\"";
            string key = "\"" + RegKey + "\"";

            Process proc = new Process();
            proc.StartInfo.FileName = "regedit.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc = Process.Start("regedit.exe", "/e " + path + " " + key);
        }

        public static void SanityCheck()
        {
            foreach(DockerGame game in GameDB.Values)
            {
                foreach(GameSettings setting in game.Settings)
                {
                    string filename = setting.GetUri(game);
                    if (setting.Type == "File") // file
                    {
                        // get the current settings file
                        FileInfo file = new FileInfo(Environment.ExpandEnvironmentVariables(filename));
                        string fileBytes = File.ReadAllText(file.FullName); // dirty but ReadBytes was causing issues

                        // get the stored settings files
                        string path_db = Path.Combine(path_storage, game.ProductName, IsPlugged ? "eGPU" : "iGPU", file.Name);
                        FileInfo fileDB = new FileInfo(Environment.ExpandEnvironmentVariables(path_db));
                        string fileDBBytes = File.ReadAllText(fileDB.FullName); // dirty but ReadBytes was causing issues

                        if (file.LastWriteTime > game.LastCheck || fileBytes != fileDBBytes)
                        {
                            // string generation
                            string WarningStr = "Your local " + game.Name + " Main files conflict with the ones stored in our Database.";
                            string ModifiedDB = "Last modified: " + game.LastCheck + " - " + (file.LastWriteTime > game.LastCheck ? "OLDER" : "NEWER");
                            string ModifiedLOCAL = "Last modified: " + file.LastWriteTime + " - " + (file.LastWriteTime < game.LastCheck ? "OLDER" : "NEWER");

                            DialogBox dialogBox = new DockerForm.DialogBox();
                            dialogBox.UpdateDialogBox("Database Sync Conflict", WarningStr, ModifiedDB, ModifiedLOCAL);

                            DialogResult dialogResult = dialogBox.ShowDialog();
                            if(dialogResult == DialogResult.Yes) // Overwrite current settings
                            {
                                UpdateFilesAndRegistries(game, !IsPlugged, false, true);
                            }
                            else if(dialogResult == DialogResult.No) // Overwrite current database
                            {
                                UpdateFilesAndRegistries(game, !IsPlugged, true, false);
                            }

                            continue;
                        }
                    }
                }
            }
        }

        public static void UpdateGameDatabase()
        {
            if (LastPlugged != IsPlugged)
            {
                _instance.Invoke(new Action(delegate () {
                    _instance.menuStrip2.Items[0].Text = IsPlugged ? "Docked" : "Undocked";
                    _instance.notifyIcon1.Icon = IsPlugged ? Properties.Resources.icon_plugged1 : Properties.Resources.icon_unplugged1;
                    _instance.Icon = IsPlugged ? Properties.Resources.icon_plugged1 : Properties.Resources.icon_unplugged1;
                }));

                if(!IsFirstBoot)
                    UpdateFilesAndRegistries(GameDB, IsPlugged);
                else
                    SanityCheck();

                LastPlugged = IsPlugged;
            }

            if (IsFirstBoot)
                IsFirstBoot = false;
        }

        public static void UpdateFilesAndRegistries(DockerGame game, bool eGPU, bool overwriteDB = true, bool restoreSETTING = true)
        {
            string path_game = Path.Combine(path_storage, game.ProductName, "iGPU");
            string path_dest = Path.Combine(path_storage, game.ProductName, "eGPU");

            if (!eGPU)
            {
                path_game = Path.Combine(path_storage, game.ProductName, "eGPU");
                path_dest = Path.Combine(path_storage, game.ProductName, "iGPU");
            }

            foreach (GameSettings setting in game.Settings)
            {
                if (!setting.IsEnabled)
                    continue;

                string filename = setting.GetUri(game);
                if (setting.Type == "File") // file
                {
                    // 1. Save current settings
                    FileInfo file = new FileInfo(Environment.ExpandEnvironmentVariables(filename));
                    if(overwriteDB)
                        CopyFile(file, path_game);

                    // 2. Restore proper settings
                    string path_file = Path.Combine(path_dest, file.Name);
                    FileInfo storedfile = new FileInfo(Environment.ExpandEnvironmentVariables(path_file));
                    if(restoreSETTING)
                        CopyFile(storedfile, file.DirectoryName);
                }
                else // registry
                {
                    string[] temp = filename.Split('\\');
                    string file = "";
                    foreach (string f in temp)
                        file += f[0];
                    file += ".reg";

                    // 1. Save current settings
                    if (overwriteDB)
                        ExportKey(filename, path_game, file);

                    // 2. Restore proper settings
                    string path_file = Path.Combine(path_dest, file);
                    if(restoreSETTING)
                        RestoreKey(path_file);
                }
            }

            game.LastCheck = DateTime.Now;
            _instance.SerializeGame(game);
            NewToastNotification(game.Name + " settings have been updated.");
        }

        public static void UpdateFilesAndRegistries(ConcurrentDictionary<string, DockerGame> localDB, bool Plugged)
        {
            // Scroll the provided database
            foreach (DockerGame game in localDB.Values)
                UpdateFilesAndRegistries(game, Plugged);
        }

        public static void ProcessMonitor(object data)
        {
            while (IsRunning)
            {
                Process[] localAll = Process.GetProcesses();

                foreach(DockerGame game in GameDB.Values)
                    foreach(Process proc in localAll)
                        if ((game.ProductName == proc.MainWindowTitle) || (game.Executable.Replace(".exe", "") == proc.ProcessName))
                            if(!GameProcesses.Values.Contains(game))
                                GameProcesses.AddOrUpdate(proc, game, (key, value) => game);

                for(int i = 0; i < GameProcesses.Count; i++)
                {
                    KeyValuePair<Process, DockerGame> pair = GameProcesses.ElementAt(i);

                    Process proc = pair.Key;
                    DockerGame game = pair.Value;

                    if (proc.HasExited)
                    {
                        UpdateFilesAndRegistries(game, !IsPlugged, true, false); // !IsPlugged to force save on eGPU folder, dirty
                        GameProcesses.TryRemove(proc, out game);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public static void MainMonitor(object data)
        {
            while (IsRunning)
            {
                VideoControllers.Clear();

                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
                foreach (ManagementObject mo in searcher.Get())
                {
                    string description = (string)mo.Properties["Description"].Value;
                    uint status = (uint)mo.Properties["ConfigManagerErrorCode"].Value;

                    if ((string)description != null && status == 0)
                        VideoControllers.Add((string)description);
                }

                IsPlugged = VideoControllers.Count != 1;

                UpdateGameDatabase();

                Thread.Sleep(1000);
            }
        }

        public void UpdateGameItem(DockerGame game)
        {
            exListBoxItem newgame = new exListBoxItem(game);

            if (game.JustCreated && !GameDB.ContainsKey(game.GUID))
            {
                GameList.Items.Add(newgame);
                GameDB.AddOrUpdate(game.GUID, game, (key, value) => game);
                game.JustCreated = false;
            }
            else
            {
                for (int i = 0; i < GameList.Items.Count; i++)
                {
                    exListBoxItem item = (exListBoxItem)GameList.Items[i];
                    if (item.Guid == game.GUID)
                    {
                        GameList.Items[i] = newgame;
                        GameDB[game.GUID] = game;
                        break;
                    }
                }
            }

            // We save iGPU/eGPU profiles on game creation
            UpdateFilesAndRegistries(game, true, true, false);
            UpdateFilesAndRegistries(game, false, true, false);

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
                    game.JustCreated = false;

                    if (!GameDB.ContainsKey(game.GUID))
                        GameDB.AddOrUpdate(game.GUID, game, (key, value) => game);

                    reader.Dispose();
                }
            }

            // Update the DockerGame database
            foreach (DockerGame game in GameDB.Values)
            {
                exListBoxItem item = new exListBoxItem(game);
                GameList.Items.Add(item);
            }

            GameList.Sort();
        }

        public void SerializeGame(DockerGame game)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DockerGame));
            TextWriter txtWriter = new StreamWriter(path_database + "\\" + game.ProductName + ".xml");
            xs.Serialize(txtWriter, game);
            txtWriter.Close();
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

            if (MinimizeOnStartup)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }

            if (BootOnStartup) // dirty, we should check if entry already exists
                AddApplicationToStartup();
            else
                RemoveApplicationFromStartup();
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            // draw GameDB
            UpdateGameList();

            // thread settings
            Thread ThreadGPU = new Thread(MainMonitor);
            ThreadGPU.Start();

            if (MonitorProcesses)
            {
                Thread ThreadEXE = new Thread(ProcessMonitor);
                ThreadEXE.Start();
            }
        }

        public static void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                key.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
        }

        public static void RemoveApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                key.DeleteValue(Application.ProductName, false);
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
                DockerGame game = GameDB[item.Guid];

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
                        DockerGame game = GameDB[item.Guid];

                        navigateToIGDBEntryToolStripMenuItem.Enabled = (game.IGDB_Url != "");
                    }                    
                    break;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
        }

        private void undockedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DEBUG
            IsPlugged = !IsPlugged;
            UpdateGameDatabase();
        }

        private void OpenGameFolder(object sender, EventArgs e)
        {
            exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
            DockerGame game = GameDB[item.Guid];

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
            DockerGame game = GameDB[item.Guid];

            string folderPath = Path.Combine(path_storage, game.ProductName);
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
            DockerGame game = GameDB[item.Guid];
            Process.Start(game.IGDB_Url);
        }

        private void removeTheGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameList.SelectedItem != null)
            {
                exListBoxItem item = (exListBoxItem)GameList.SelectedItem;
                DockerGame game = GameDB[item.Guid];

                string filename = Path.Combine(path_database, game.ProductName + ".xml");
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    GameDB.TryRemove(item.Guid, out game);
                    GameList.Items.Remove(item);
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
                Settings p = new Settings(this, GameDB[item.Guid]);
                p.Show();
            }
        }
    }
}
