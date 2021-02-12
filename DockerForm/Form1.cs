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
using System.Xml.Serialization;
using Microsoft.Win32.TaskScheduler;
using Task = Microsoft.Win32.TaskScheduler.Task;
using System.Runtime.InteropServices;

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
        public static string MCHBAR = null;

        // Folder vars
        public static string path_application, path_database, path_dependencies, path_profiles;
        public static string path_rw;

        // Form vars
        private static Form1 _instance;

        // Threading vars
        private static Thread ThreadGPU;
        private static ManagementEventWatcher processStartWatcher, processStopWatcher;
        private static Dictionary<int, string> GameProcesses = new Dictionary<int, string>();

        // PowerProfile vars
        public static Dictionary<string, PowerProfile> ProfileDB = new Dictionary<string, PowerProfile>();
        public static PowerProfile CurrentProfile;

        // TaskManager vars
        private static TaskService ts;
        private const string taskName = "ThunderboltSwitch";

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int WM_DEVICECHANGE = 0x0219;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                IsHardwarePending = true;
            }
            base.WndProc(ref m);
        }

        private void OnPowerModeChanged(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    break;
                case PowerModes.Suspend:
                    break;
            }

            IsHardwarePending = true;
        }

        private static void CheckPowerProfiles()
        {
            PowerProfile sum_profile = new PowerProfile();
            foreach (PowerProfile profile in ProfileDB.Values.OrderBy(a => a.ApplyMask))
            {
                bool isOnBattery = (profile.ApplyMask & (byte)ProfileMask.OnBattery) != 0;
                bool isPluggedIn = (profile.ApplyMask & (byte)ProfileMask.PluggedIn) != 0;
                bool isExtGPU = (profile.ApplyMask & (byte)ProfileMask.ExtGPU) != 0;

                // if device is running on battery
                if (!PowerStatus && isOnBattery || PowerStatus && isPluggedIn || DockStatus && isExtGPU)
                {
                    sum_profile.DigestProfile(profile);
                    sum_profile.ProfileName += profile.ProfileName + ",";
                }
            }
            sum_profile.ProfileName = sum_profile.ProfileName.TrimEnd(',');

            SetPowerProfile(sum_profile);
        }

        private static void SetPowerProfile(PowerProfile profile, DockerGame game = null)
        {
            // skip if unsupported platform
            if (MCHBAR == null)
                return;

            // skip if call isn't needed
            if (profile == CurrentProfile)
                return;

            string command = "/Min /Nologo /Stdout /command=\"";

            if (profile.HasLongPowerMax())
            {
                command += "w16 " + MCHBAR + "a0 0x8" + profile.GetLongPowerMax().Substring(0, 1) + profile.GetLongPowerMax().Substring(1) + ";";
                command += "wrmsr 0x610 0x0 0x00dd8" + profile.GetLongPowerMax() + ";";
            }

            if (profile.HasShortPowerMax())
            {
                command += "w16 " + MCHBAR + "a4 0x8" + profile.GetShortPowerMax().Substring(0, 1) + profile.GetShortPowerMax().Substring(1) + ";";
                command += "wrmsr 0x610 0x0 0x00438" + profile.GetShortPowerMax() + ";";
            }

            if (profile.HasCPUCore())
                command += "wrmsr 0x150 0x80000011 0x" + profile.GetVoltageCPU() + ";";
            if (profile.HasIntelGPU())
            {
                command += "wrmsr 0x150 0x80000111 0x" + profile.GetVoltageGPU() + ";";
                command += "wrmsr 0x150 0x80000311 0x" + profile.GetVoltageGPU() + ";"; //unslice (obsolete ?)
            }
            if (profile.HasCPUCache())
                command += "wrmsr 0x150 0x80000211 0x" + profile.GetVoltageCache() + ";";
            if (profile.HasSystemAgent())
                command += "wrmsr 0x150 0x80000411 0x" + profile.GetVoltageSA() + ";";

            if (profile.HasPowerBalanceCPU())
                command += "wrmsr 0x642 0x00000000 0x000000" + profile.GetPowerBalanceCPU() + ";";
            if (profile.HasPowerBalanceGPU())
                command += "wrmsr 0x63a 0x00000000 0x000000" + profile.GetPowerBalanceGPU() + ";";

            command += "rwexit\"";

            // get current handle
            IntPtr curHnd = GetForegroundWindow();

            ProcessStartInfo RWInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = command,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = path_rw,
                Verb = "runas"
            };
            Process.Start(RWInfo);

            // restore handle
            SetForegroundWindow(curHnd);

            // update current profile
            CurrentProfile = profile;

            SendNotification("Power Profile [" + profile.GetName() + "] applied.", true);
            LogManager.UpdateLog("Power Profile [" + profile.GetName() + "] applied." + profile.ToString());
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

        static DockerGame GetGameFromPath(string PathToApp)
        {
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                string game_exe = game.Executable.ToLower();
                string game_uri = game.Uri.ToLower();

                FileInfo info = new FileInfo(PathToApp);
                string info_exe = info.Name.ToLower();
                string info_uri = info.FullName.ToLower();

                // Update current title
                if (game_exe == info_exe || game_uri == info_uri)
                    return game;
            }

            return null;
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

                // Apply application specific profile
                using (DockerGame game = GetGameFromPath(PathToApp))
                {
                    if (game == null)
                        return;

                    if (GameProcesses.ContainsKey(ProcessID))
                        GameProcesses.Remove(ProcessID);
                    GameProcesses.Add(ProcessID, PathToApp);

                    using (PowerProfile profile = game.Profile)
                    {
                        if (profile == null)
                            return;

                        SetPowerProfile(profile, game);
                    }
                }

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

                using (DockerGame game = GetGameFromPath(PathToApp))
                {
                    if (game == null)
                        return;

                    if (GameProcesses.ContainsKey(ProcessID))
                        GameProcesses.Remove(ProcessID);

                    DatabaseManager.UpdateFilesAndRegistries(game, GetCurrentState(), GetCurrentState(), true, false, true, GetCurrentState());

                    CheckPowerProfiles();
                }
            }
            catch (Exception ex) { }
        }

        public static string GetCurrentState()
        {
            string state = CurrentController.Name;

            /*            
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

        public static void MonitorThread(object data)
        {
            while(IsRunning)
            {
                bool IsGameRunning = GameProcesses.Count != 0;
                
                if ((IsHardwarePending || IsFirstBoot) && !IsGameRunning)
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
                        // Video Controllers has changed
                        if (IsHardwareNew)
                        {
                            if (VideoControllers.ContainsKey(Type.Discrete))
                                SendNotification(VideoControllers[Type.Discrete].Name + " detected.", true, true);
                            else if (VideoControllers.ContainsKey(Type.Internal))
                                SendNotification(VideoControllers[Type.Internal].Name + " detected.", true, true);
                        }

                        // Power Status has changed
                        if (IsPowerNew)
                        {
                            LogManager.UpdateLog("Power Status: " + GetCurrentPower());
                            SendNotification("Device is " + GetCurrentPower() + ".", true);
                        }

                        // Software is initializing 
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

                        CheckPowerProfiles();
                    }

                    // update status
                    prevDockStatus = DockStatus;
                    prevPowerStatus = PowerStatus;

                    // update form
                    UpdateFormIcons();
                    IsHardwarePending = false;
                }

                Thread.Sleep(MonitorThreadRefresh);
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

        public void InsertOrUpdateGameItem(DockerGame game, bool auto)
        {
            exListBoxItem newitem = new exListBoxItem(game);
            if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
            {
                GameList.Items.Add(newitem);
                DatabaseManager.GameDB[game.GUID] = game;
                LogManager.UpdateLog("[" + game.Name + "] profile has been added to the database");
            }
            else
            {
                int idx = GameList.GetIndexFromGuid(game.GUID);
                if (idx == -1)
                    return;

                DockerGame list_game = DatabaseManager.GameDB[game.GUID];
                if (auto)
                {
                    // automatic detection
                    list_game.Uri = game.Uri;
                    list_game.Version = game.Version;
                    list_game.LastCheck = DateTime.Now;
                    list_game.SanityCheck();
                }
                else
                {
                    // manually updated game
                    list_game = game;
                    GameList.Items[idx] = newitem;
                }

                ((exListBoxItem)GameList.Items[idx]).Enabled = list_game.Enabled;
                LogManager.UpdateLog("[" + list_game.Name + "] profile has been updated");
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
                        DockerGame thisGame = (DockerGame)formatter.Deserialize(reader);
                        thisGame.SanityCheck();

                        if (!DatabaseManager.GameDB.ContainsKey(thisGame.GUID))
                            DatabaseManager.GameDB.AddOrUpdate(thisGame.GUID, thisGame, (key, value) => thisGame);

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

        public static void UpdateProfiles()
        {
            // Clear array before update
            ProfileDB.Clear();

            // Read all the game files (xml)
            string[] fileEntries = Directory.GetFiles(path_profiles, "*.xml");
            foreach (string filename in fileEntries)
            {
                try
                {
                    using (Stream reader = new FileStream(filename, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(PowerProfile));
                        PowerProfile profile = (PowerProfile)formatter.Deserialize(reader);
                        profile.ComputeHex();

                        if (!ProfileDB.ContainsKey(profile.ProfileName))
                            ProfileDB.Add(profile.ProfileName, profile);

                        reader.Dispose();
                    }
                }
                catch (Exception ex) { LogManager.UpdateLog("UpdateProfiles: " + ex.Message, true); }
            }
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
            ts = new TaskService();

            // folder settings
            path_application = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.InitializeLog(path_application);

            // path settings
            path_database = Path.Combine(path_application, "database");
            path_dependencies = Path.Combine(path_application, "dependencies");
            path_profiles = Path.Combine(path_application, "profiles");
            path_rw = Path.Combine(path_dependencies, "Rw.exe");

            if (!Directory.Exists(path_database))
                Directory.CreateDirectory(path_database);

            if (!Directory.Exists(path_dependencies))
                Directory.CreateDirectory(path_dependencies);

            if (!Directory.Exists(path_profiles))
                Directory.CreateDirectory(path_profiles);

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

            Task myTask = ts.FindTask(taskName);
            if (myTask == null)
            {
                TaskDefinition td = TaskService.Instance.NewTask();
                td.Principal.RunLevel = TaskRunLevel.Highest;
                td.Principal.LogonType = TaskLogonType.InteractiveToken;
                td.Settings.DisallowStartIfOnBatteries = false;
                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
                td.Settings.Enabled = false;
                td.Triggers.Add(new LogonTrigger());
                td.Actions.Add(new ExecAction(Path.Combine(path_application, "DockerForm.exe")));
                myTask = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);
                LogManager.UpdateLog("Task Scheduler: " + taskName + " was successfully created");
            }

            if (myTask != null)
            {
                if (BootOnStartup && !myTask.Enabled)
                {
                    myTask.Enabled = true;
                    LogManager.UpdateLog("Task Scheduler: " + taskName + " was enabled");
                }
                else if (!BootOnStartup && myTask.Enabled)
                {
                    myTask.Enabled = false;
                    LogManager.UpdateLog("Task Scheduler: " + taskName + " was disabled");
                }
            }

            // update MCHBAR
            string ProcessorID = GetProcessorID();
            switch(ProcessorID.Substring(ProcessorID.Length-5))
            {
                case "206A7": // SandyBridge
                case "306A9": // IvyBridge
                case "40651": // Haswell
                case "306D4": // Broadwell
                case "406E3": // Skylake
                case "906ED": // CoffeeLake
                case "806E9": // AmberLake
                case "706E5": // IceLake
                    MCHBAR = "0xFED159";
                    break;
                case "806C1": // TigerLake
                    MCHBAR = "0xFEDC59";
                    break;
            }

            // update Database
            UpdateGameList();

            // update Profiles
            UpdateProfiles();
        }

        private string GetProcessorID()
        {
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
                return managObj.Properties["processorID"].Value.ToString();

            return "";
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
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

                LogManager.UpdateLog("Process Monitor: started");
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

                            // Sanity checks
                            openToolStripMenuItem.Enabled = game.HasReachableFolder();
                            toolStripStartItem.Enabled = game.HasReachableExe();
                            toolStripMenuItem1.Enabled = game.HasFileSettings();
                            navigateToIGDBEntryToolStripMenuItem.Enabled = game.HasIGDB();

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

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = game.Arguments,
                FileName = filename
            };

            Process.Start(startInfo);
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
                    InsertOrUpdateGameItem(game, true);
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
