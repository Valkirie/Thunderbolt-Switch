using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Task = Microsoft.Win32.TaskScheduler.Task;

namespace DockerForm
{
    public partial class Form1 : Form
    {
        // Global vars
        public static bool prevDockStatus = false;
        public static bool DockStatus = false;
        public static bool IsFirstBoot = true;
        public static bool IsHardwareNew = false;
        public static bool IsHardwarePending = true;
        public static bool IsPowerPending = true;
        public static bool IsScreenPending = true;
        public static VideoController CurrentController;
        public static bool prevPowerStatus;
        public static bool PowerStatus;
        public static bool IsPowerNew = false;
        public static int prevScreenCount;
        public static int ScreenCount;
        public static bool IsScreenNew = false;
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
        public static int MonitorThreadRefresh;
        public static bool MonitorProfiles = false;
        public static bool MonitorHardware = false;

        // Devices vars
        public static Dictionary<Type, VideoController> VideoControllers = new Dictionary<Type, VideoController>();
        public static string MCHBAR;

        // Folder vars
        public static string path_application, path_database, path_dependencies, path_profiles;
        public static string path_rw, path_devcon;

        // Form vars
        private static Form1 CurrentForm;
        private static CultureInfo CurrentCulture;

        // Threading vars
        private static Thread ThreadGPU, ThreadProfile;
        private static ManagementEventWatcher processStartWatcher, processStopWatcher;
        private static Dictionary<int, string> GameProcesses = new Dictionary<int, string>();

        // PowerProfile vars
        public static Dictionary<string, PowerProfile> ProfileDB = new Dictionary<string, PowerProfile>();
        public static PowerProfile CurrentProfile = new PowerProfile();

        // TaskManager vars
        private static TaskService CurrentTask;
        private const string taskName = "ThunderboltSwitch";

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            const int WM_DISPLAYCHANGE = 0x007e;
            const int WM_DEVICECHANGE = 0x0219;

            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                case WM_DISPLAYCHANGE:
                    IsHardwarePending = true;
                    break;
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

            IsPowerPending = true;
        }

        private static bool CanRunProfile(PowerProfile profile, bool IsFirstBoot)
        {
            bool isOnBattery = profile._ApplyMask.HasFlag(ProfileMask.OnBattery);
            bool isPluggedIn = profile._ApplyMask.HasFlag(ProfileMask.PluggedIn);
            bool isExtGPU = profile._ApplyMask.HasFlag(ProfileMask.ExternalGPU);
            bool isOnBoot = profile._ApplyMask.HasFlag(ProfileMask.OnStartup);
            bool isOnStatusChange = profile._ApplyMask.HasFlag(ProfileMask.OnStatusChange);
            bool isOnScreen = profile._ApplyMask.HasFlag(ProfileMask.ExternalScreen);

            if (IsFirstBoot && !isOnBoot)
                return false;
            if (!IsFirstBoot && !isOnStatusChange)
                return false;

            if (PowerStatus && isPluggedIn)
                return true;
            else if (!PowerStatus && isOnBattery)
                return true;
            else if (DockStatus && isExtGPU)
                return true;
            else if (ScreenCount > 1 && isOnScreen)
                return true;

            return false;
        }

        private static void ApplyPowerProfiles()
        {
            PowerProfile sum_profile = new PowerProfile();
            foreach (PowerProfile profile in ProfileDB.Values.Where(a => a.RunMe).OrderBy(a => a.ApplyPriority))
                sum_profile.DigestProfile(profile, true);

            SetPowerProfile(sum_profile);

            // update form
            UpdateFormProfiles();
        }

        private static void SetPowerProfile(PowerProfile profile)
        {
            // skip if unsupported platform
            if (MCHBAR == null || !MCHBAR.Contains("0x"))
                return;

            // skip check on empty profile
            if (profile.ProfileName == "")
                return;

            // skip update on similar profiles
            if (CurrentProfile.Equals(profile))
                return;

            string command = "/Min /Nologo /Stdout /command=\"Delay 1000;";

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
                command += "wrmsr 0x150 0x80000111 0x" + profile.GetVoltageGPU() + ";";
            if (profile.HasCPUCache())
                command += "wrmsr 0x150 0x80000211 0x" + profile.GetVoltageCache() + ";";
            if (profile.HasSystemAgent())
                command += "wrmsr 0x150 0x80000411 0x" + profile.GetVoltageSA() + ";";

            if (profile.HasPowerBalanceCPU())
                command += "wrmsr 0x642 0x00000000 0x000000" + profile.GetPowerBalanceCPU() + ";";
            if (profile.HasPowerBalanceGPU())
                command += "wrmsr 0x63a 0x00000000 0x000000" + profile.GetPowerBalanceGPU() + ";";

            // command += "w " + MCHBAR + "94 0xFF;";
            command += "Delay 1000;rwexit\"";

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

            // update current profile
            CurrentProfile = profile;

            SendNotification("Power Profile [" + profile.GetName() + "] applied.", true, true);
        }

        public static void SendNotification(string input, bool pushToast = false, bool pushLog = false, bool IsError = false)
        {
            if (pushLog)
                LogManager.UpdateLog(input, IsError);

            if (pushToast && ToastNotifications)
            {
                CurrentForm.notifyIcon1.BalloonTipText = input;
                CurrentForm.notifyIcon1.ShowBalloonTip(1000);
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

                    GameProcesses[ProcessID] = PathToApp;

                    foreach (PowerProfile profile in game.Profiles.Values)
                    {
                        ProfileDB[profile.ProfileName].RunMe = CanRunProfile(profile, false);
                        ProfileDB[profile.ProfileName].GameBounds = game.Name;
                    }

                    game.IsRunning = true;

                    ApplyPowerProfiles();
                }

            }
            catch (Exception ex) { }
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

                    GameProcesses.Remove(ProcessID);

                    string path_db = GetCurrentState(game);
                    DatabaseManager.UpdateFilesAndRegistries(game, path_db, path_db, true, false, true, path_db);

                    foreach (PowerProfile profile in game.Profiles.Values.Where(a => a.GameBounds != null))
                    {
                        ProfileDB[profile.ProfileName].GameBounds = null;
                        ProfileDB[profile.ProfileName].RunMe = false;
                    }

                    game.IsRunning = false;

                    ApplyPowerProfiles();
                }
            }
            catch (Exception ex) { }
        }

        public static string GetCurrentState(DockerGame game)
        {
            string state = CurrentController.Name;

            // Intel(R) Iris(R) Plus Graphics
            // Intel(R) Iris(R) Plus Graphics (on battery)
            if (!DockStatus && !PowerStatus && game.PowerSpecific)
                state += " (" + GetCurrentPower() + ")";

            return state;
        }

        public static string GetCurrentPower()
        {
            return PowerStatus ? "plugged in" : "on battery";
        }

        public static void ProfilesMonitorThread(object data)
        {
            while (IsRunning)
            {
                UpdateProfiles();
                Thread.Sleep(MonitorThreadRefresh);
            }
        }

        public static void UpdateMonitorHardware()
        {
            VideoControllers.Clear();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject mo in searcher.Get())
            {
                VideoController vc = new VideoController
                {
                    PNPDeviceID = (string)mo.Properties["PNPDeviceID"].Value,
                    Name = (string)mo.Properties["Name"].Value,
                    Description = (string)mo.Properties["Description"].Value,
                    ErrorCode = (uint)mo.Properties["ConfigManagerErrorCode"].Value
                };

                // initialize VideoController
                vc.Initialize();
                VideoControllers[vc.Type] = vc;
            }

            // update all status
            if (VideoControllers.ContainsKey(Type.Discrete))
            {
                if (VideoControllers[Type.Discrete].IsEnabled())
                {
                    CurrentController = VideoControllers[Type.Discrete];
                    DockStatus = true;
                }

                // disable iGPU when dGPU is available
                if (VideoControllers.ContainsKey(Type.Internal) && MonitorHardware)
                    VideoControllers[Type.Internal].DisableDevice(path_devcon);
            }
            else if (VideoControllers.ContainsKey(Type.Internal))
            {
                if (VideoControllers[Type.Internal].IsEnabled())
                {
                    CurrentController = VideoControllers[Type.Internal];
                    DockStatus = false;
                }

                // enable iGPU when no dGPU is available
                if (MonitorHardware)
                    VideoControllers[Type.Internal].EnableDevice(path_devcon);
            }
            else
            {
                // should not happen !
                CurrentController = new VideoController()
                {
                    Name = "Unknown",
                    Constructor = Constructor.Intel,
                    Type = Type.Internal
                };
            }

            // monitor hardware changes
            IsHardwareNew = prevDockStatus != DockStatus;

            // hardware has changed
            if (IsHardwareNew && !IsFirstBoot)
            {
                if (VideoControllers.ContainsKey(Type.Discrete))
                    SendNotification(VideoControllers[Type.Discrete].Name + " detected.", true, true);
                else if (VideoControllers.ContainsKey(Type.Internal))
                    SendNotification(VideoControllers[Type.Internal].Name + " detected.", true, true);

                DatabaseManager.UpdateFilesAndRegistries(false, true);
                DatabaseManager.UpdateFilesAndRegistries(true, false);
            }

            // update status
            prevDockStatus = DockStatus;
            IsHardwarePending = false;

            // update form
            UpdateFormConstructor();
        }

        public static void UpdateMonitorPower()
        {
            PowerStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
            IsPowerNew = prevPowerStatus != PowerStatus;

            // Power Status has changed
            if (IsPowerNew && !IsFirstBoot)
            {
                LogManager.UpdateLog("Power Status: " + GetCurrentPower());
                SendNotification("Device is " + GetCurrentPower() + ".", true);
            }

            // update status
            prevPowerStatus = PowerStatus;
            IsPowerPending = false;

            // update form
            UpdateFormConstructor();
        }

        public static void UpdateMonitorScreen()
        {
            ScreenCount = Screen.AllScreens.Length;
            IsScreenNew = prevScreenCount != ScreenCount;

            // update status
            prevScreenCount = ScreenCount;
            IsScreenPending = false;
        }

        public static void MonitorThread(object data)
        {
            while (IsRunning)
            {
                bool IsGameRunning = GameProcesses.Count != 0;
                if (IsHardwarePending && !IsGameRunning)
                    UpdateMonitorHardware();

                if (IsPowerPending)
                    UpdateMonitorPower();

                if (IsScreenPending)
                    UpdateMonitorScreen();

                if (IsFirstBoot || IsPowerNew || IsHardwareNew || IsScreenNew)
                {
                    foreach (PowerProfile profile in ProfileDB.Values)
                        ProfileDB[profile.ProfileName].RunMe = CanRunProfile(profile, IsFirstBoot);

                    ApplyPowerProfiles();
                    if (IsPowerNew) IsPowerNew = false;
                    if (IsHardwareNew) IsHardwareNew = false;
                    if (IsScreenNew) IsScreenNew = false;
                }

                if (IsFirstBoot)
                {
                    DatabaseManager.SanityCheck();
                    IsFirstBoot = false;
                }

                Thread.Sleep(MonitorThreadRefresh);
            }
        }

        public static void UpdateFormProfiles()
        {
            // profiles
            CurrentForm.BeginInvoke((MethodInvoker)delegate ()
            {
                CurrentForm.toolStripMenuItem2.DropDownItems.Clear();

                ToolStripMenuItem currentItem = new ToolStripMenuItem()
                {
                    Text = "Current profile",
                    ToolTipText = CurrentProfile.ToString(),
                    Enabled = false
                };
                CurrentForm.toolStripMenuItem2.DropDownItems.Add(currentItem);
                CurrentForm.toolStripMenuItem2.DropDownItems.Add(new ToolStripSeparator());

                // do not display the default profile
                foreach (PowerProfile profile in ProfileDB.Values.Where(a => a.ApplyPriority != -1))
                {
                    ToolStripMenuItem newItem = new ToolStripMenuItem()
                    {
                        Text = profile.ProfileName,
                        Checked = profile.RunMe,
                        ToolTipText = profile.ToString()
                    };
                    newItem.Click += new EventHandler(PowerMenuClickHandler);
                    CurrentForm.toolStripMenuItem2.DropDownItems.Add(newItem);
                }
            });
        }

        public static void UpdateFormConstructor()
        {
            // taskbar icon
            Image ConstructorLogo;
            string ConstructorName = CurrentController.Name;
            switch (CurrentController.Constructor)
            {
                case Constructor.AMD:
                    ConstructorLogo = Properties.Resources.amd;
                    break;
                case Constructor.Nvidia:
                    ConstructorLogo = Properties.Resources.nvidia;
                    break;
                case Constructor.Intel:
                default:
                    ConstructorLogo = Properties.Resources.intel;
                    break;
            }

            // main application icon
            Icon myIcon = DockStatus ? Properties.Resources.tb3_on : Properties.Resources.tb3_off;

            // drawing
            CurrentForm.BeginInvoke((MethodInvoker)delegate ()
            {
                CurrentForm.menuStrip2.Items[0].Text = ConstructorName + " (" + GetCurrentPower() + ")";
                CurrentForm.undockedToolStripMenuItem.Image = ConstructorLogo;
                CurrentForm.notifyIcon1.Icon = myIcon;
                CurrentForm.Icon = myIcon;
            });
        }

        public int GetListViewIndex(string GUID)
        {
            foreach (ListViewItem item in GameListView.Items)
                if ((string)item.Tag == GUID)
                    return GameListView.Items.IndexOf(item);
            return -1;
        }

        public void InsertOrUpdateGameItem(DockerGame game, bool auto)
        {
            ListViewItem newgame = new ListViewItem(new string[] { "", game.GetNameAndGUID(), game.Company, game.Version, game.LastCheck.ToString(CurrentCulture), game.GetSettingsList() }, game.GUID);
            newgame.Tag = newgame.ImageKey;
            
            if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
            {
                // upate imageList
                imageList1.Images.Add(game.GUID, game.Image);

                GameListView.Items.Add(newgame);
                DatabaseManager.GameDB[game.GUID] = game;
                LogManager.UpdateLog("[" + game.Name + "] profile has been added to the database");
            }
            else
            {
                // upate imageList
                if (imageList1.Images.ContainsKey(game.GUID))
                {
                    imageList1.Images.RemoveByKey(game.GUID);
                    imageList1.Images.Add(game.GUID, game.Image);
                }

                int idx = GetListViewIndex(game.GUID);
                if (idx == -1)
                    return;

                if (auto)
                {
                    // automatic detection
                    DatabaseManager.GameDB[game.GUID].Uri = game.Uri;
                    DatabaseManager.GameDB[game.GUID].Version = game.Version;
                    DatabaseManager.GameDB[game.GUID].SanityCheck();
                }
                else
                {
                    // manually updated game
                    DatabaseManager.GameDB[game.GUID] = new DockerGame(game);
                    GameListView.Items[idx] = newgame;
                }

                // update the current database
                DockerGame list_game = DatabaseManager.GameDB[game.GUID];
                // ((exListBoxItem)GameList.Items[idx]).Enabled = list_game.Enabled;
                LogManager.UpdateLog("[" + list_game.Name + "] profile has been updated");
            }

            // update current title
            DatabaseManager.GameDB[game.GUID].LastCheck = DateTime.Now;
            string path_db = GetCurrentState(game);
            DatabaseManager.UpdateFilesAndRegistries(DatabaseManager.GameDB[game.GUID], path_db, path_db, false, true, true, path_db);
            DatabaseManager.UpdateFilesAndRegistries(DatabaseManager.GameDB[game.GUID], path_db, path_db, true, false, true, path_db);
        }

        public void UpdateGameList()
        {
            // Read all the game files (dat)
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
                        {
                            DatabaseManager.GameDB.AddOrUpdate(thisGame.GUID, thisGame, (key, value) => thisGame);
                            imageList1.Images.Add(thisGame.GUID, thisGame.Image);
                        }

                        reader.Dispose();
                    }
                }
                catch (Exception ex) { LogManager.UpdateLog("UpdateGameList: " + ex.Message, true); }
            }

            // Update the DockerGame database
            GameListView.BeginUpdate();
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                ListViewItem newgame = new ListViewItem(new string[] { "", game.GetNameAndGUID(), game.Company, game.Version, game.LastCheck.ToString(CurrentCulture), game.GetSettingsList() }, game.GUID);
                newgame.Tag = newgame.ImageKey;
                GameListView.Items.Add(newgame);
                // item.Enabled = game.Enabled;
            }
            GameListView.EndUpdate();
        }

        public static Dictionary<string, DateTime> prevFileInfos = new Dictionary<string, DateTime>();
        public static void UpdateProfiles()
        {
            // read all the profiles files (xml)
            string[] fileEntries = Directory.GetFiles(path_profiles, "*.xml");
            Dictionary<string, DateTime> fileInfos = new Dictionary<string, DateTime>();

            foreach (string filename in fileEntries)
            {
                FileInfo info = new FileInfo(filename);
                fileInfos[filename] = info.LastWriteTime;
            }

            // skip if no changes on profiles files
            if (prevFileInfos.ContentEquals(fileInfos))
                return;

            Dictionary<string, PowerProfile> profileList = new Dictionary<string, PowerProfile>();
            List<string> removeList = new List<string>();
            foreach (string filename in fileEntries)
            {
                try
                {
                    using (Stream reader = new FileStream(filename, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(PowerProfile));
                        PowerProfile profile = (PowerProfile)formatter.Deserialize(reader);
                        profileList.Add(profile.ProfileName, profile);
                        reader.Dispose();
                    }
                }
                catch (Exception ex) { LogManager.UpdateLog("UpdateProfiles: " + ex.Message, true); }
            }

            // add or update profiles
            foreach (PowerProfile profile in profileList.Values)
            {
                string ProfileName = profile.ProfileName;

                bool RunMe = ProfileDB.ContainsKey(ProfileName) ? ProfileDB[ProfileName].RunMe : false;
                string GameBounds = ProfileDB.ContainsKey(ProfileName) ? ProfileDB[ProfileName].GameBounds : "";

                ProfileDB[ProfileName] = profile;
                ProfileDB[ProfileName].RunMe = RunMe;
                ProfileDB[ProfileName].GameBounds = GameBounds;

                ProfileDB[ProfileName].ComputeHex();
            }

            // insert all removed profiles
            foreach (string ProfileName in ProfileDB.Keys.Where(a => !profileList.ContainsKey(a)))
                removeList.Add(ProfileName);

            // remove obsolete profiles
            foreach (string ProfileName in removeList)
                ProfileDB.Remove(ProfileName);

            // update games
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                // remove outdated profiles
                foreach (string ProfileName in removeList)
                    game.Profiles.Remove(ProfileName);

                // update associated profiles
                foreach (PowerProfile profile in ProfileDB.Values)
                    if (game.Profiles.ContainsKey(profile.ProfileName))
                        game.Profiles[profile.ProfileName] = profile;
            }

            // re-apply values
            ApplyPowerProfiles();

            // update var
            prevFileInfos = fileInfos;
        }

        private static void PowerMenuClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string ProfileName = item.Text;

            item.Checked = !item.Checked;
            ProfileDB[ProfileName].RunMe = item.Checked;

            ApplyPowerProfiles();
        }

        public int GetIGDBListLength()
        {
            return IGDBListLength;
        }

        public static void UpdateSettings()
        {
            MinimizeOnStartup = Properties.Settings.Default.MinimizeOnStartup;
            MinimizeOnClosing = Properties.Settings.Default.MinimizeOnClosing;
            BootOnStartup = Properties.Settings.Default.BootOnStartup;
            MonitorProcesses = Properties.Settings.Default.MonitorProcesses;
            IGDBListLength = Properties.Settings.Default.IGDBListLength;
            ToastNotifications = Properties.Settings.Default.ToastNotifications;
            SaveOnExit = Properties.Settings.Default.SaveOnExit;
            Blacklist = Properties.Settings.Default.Blacklist;
            MonitorThreadRefresh = Properties.Settings.Default.MonitorThreadRefresh;
            MonitorProfiles = Properties.Settings.Default.MonitorProfiles;
            MonitorHardware = Properties.Settings.Default.MonitorHardware;
        }

        public Form1()
        {
            InitializeComponent();

            // initialize vars
            CurrentForm = this;
            CurrentCulture = CultureInfo.CurrentCulture;
            CurrentTask = new TaskService();

            // folder settings
            path_application = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.InitializeLog(path_application);

            // path settings
            path_database = Path.Combine(path_application, "database");
            path_dependencies = Path.Combine(path_application, "dependencies");
            path_profiles = Path.Combine(path_application, "profiles");
            path_rw = Path.Combine(path_dependencies, "Rw.exe");
            path_devcon = Path.Combine(path_dependencies, Environment.Is64BitOperatingSystem ? "x64" : "x86", "DevManView.exe");

            if (!Directory.Exists(path_database))
                Directory.CreateDirectory(path_database);

            if (!Directory.Exists(path_dependencies))
                Directory.CreateDirectory(path_dependencies);

            if (!Directory.Exists(path_profiles))
                Directory.CreateDirectory(path_profiles);

            // configurable settings
            settingsToolStripMenuItem.ToolTipText = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            settingsToolStripMenuItem.AutoToolTip = true;
            UpdateSettings();

            // position and size settings
            imageList1.ImageSize = new Size(Properties.Settings.Default.ImageWidth, Properties.Settings.Default.ImageHeight);
            Location = new Point(Properties.Settings.Default.MainWindowX, Properties.Settings.Default.MainWindowY);
            Size = new Size(Properties.Settings.Default.MainWindowWidth, Properties.Settings.Default.MainWindowHeight);
            columnName.Width = Properties.Settings.Default.ColumnNameWidth;
            columnDev.Width = Properties.Settings.Default.ColumnDevWidth;
            columnVersion.Width = Properties.Settings.Default.ColumnVersionWidth;
            columnPlayed.Width = Properties.Settings.Default.ColumnPlayedWidth;
            columnSettings.Width = Properties.Settings.Default.ColumnSettingsWidth;
            columnImage.Width = Properties.Settings.Default.ImageWidth;

            if (MinimizeOnStartup)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }

            Task myTask = CurrentTask.FindTask(taskName);
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
            string command = "/Min /Nologo /Stdout /command=\"Delay 1000;rpci32 0 0 0 0x48;Delay 1000;rwexit\"";
            using (var ProcessOutput = Process.Start(new ProcessStartInfo(path_rw, command)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                Verb = "runas"
            }))
            {
                while (!ProcessOutput.StandardOutput.EndOfStream)
                {
                    string line = ProcessOutput.StandardOutput.ReadLine();

                    if (!line.Contains("0x"))
                        continue;

                    MCHBAR = line.GetLast(10);
                    MCHBAR = MCHBAR.Substring(0, 6) + "59";
                    break;
                }
            };

            // update Database
            UpdateGameList();
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

            // update ProfileDB
            UpdateProfiles();

            // Monitor power profiles
            if (MonitorProfiles)
            {
                ThreadProfile = new Thread(ProfilesMonitorThread);
                ThreadProfile.Start();
            }

            // search for GPUs
            ThreadGPU = new Thread(MonitorThread);
            ThreadGPU.Start();

            // Monitor Power Status
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            // position and size settings
            if (CurrentForm.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.MainWindowX = CurrentForm.Location.X;
                Properties.Settings.Default.MainWindowY = CurrentForm.Location.Y;
                Properties.Settings.Default.MainWindowWidth = CurrentForm.Size.Width;
                Properties.Settings.Default.MainWindowHeight = CurrentForm.Size.Height;
            }

            Properties.Settings.Default.ColumnNameWidth = columnName.Width;
            Properties.Settings.Default.ColumnDevWidth = columnDev.Width;
            Properties.Settings.Default.ColumnVersionWidth = columnVersion.Width;
            Properties.Settings.Default.ColumnPlayedWidth = columnPlayed.Width;
            Properties.Settings.Default.ColumnSettingsWidth = columnSettings.Width;
            Properties.Settings.Default.Save();

            if (MinimizeOnClosing && e.CloseReason == CloseReason.UserClosing && !ForceClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                if (SaveOnExit)
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

        private void DisplayForm()
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            ShowInTaskbar = true;
        }

        private void OpenGameFolder(object sender, EventArgs e)
        {
            if (GameListView.SelectedItems.Count == 0)
                return;

            ListViewItem item = GameListView.SelectedItems[0];
            DockerGame game = DatabaseManager.GameDB[item.ImageKey];

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
            if (GameListView.SelectedItems.Count == 0)
                return;

            ListViewItem item = GameListView.SelectedItems[0];
            DockerGame game = DatabaseManager.GameDB[item.ImageKey];
            Process.Start(game.IGDB_Url);
        }

        private void exittoolStripStartItem_Click(object sender, EventArgs e)
        {
            ForceClose = true;
            Close();
        }

        private void microsoftStoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // check if Microsoft Store is running
            List<Process> procList = Process.GetProcessesByName("WinStore.App").ToList();
            if (procList.Count != 0)
            {
                Process WinStore = procList[0];
                if (WinStore.Responding)
                {
                    MessageBox.Show("Please close the Microsoft Store before starting the automatic detection.", "Automatic Detection");
                    return;
                }
            }

            List<DockerGame> DetectedGames = new List<DockerGame>();
            DetectedGames.AddRange(DatabaseManager.SearchMicrosoftStore());
            AutomaticDetection(DetectedGames, "Microsoft Store");
        }

        private void battleNetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DockerGame> DetectedGames = new List<DockerGame>();
            DetectedGames.AddRange(DatabaseManager.SearchBattleNet());
            AutomaticDetection(DetectedGames, "Battle.net");
        }

        private void steamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<DockerGame> DetectedGames = new List<DockerGame>();
            DetectedGames.AddRange(DatabaseManager.SearchSteam());
            AutomaticDetection(DetectedGames, "Steam");
        }

        private void UniversalMenuItem3_Click(object sender, EventArgs e)
        {
            List<DockerGame> DetectedGames = new List<DockerGame>();
            DetectedGames.AddRange(DatabaseManager.SearchUniversal());
            AutomaticDetection(DetectedGames, "Universal");
        }

        // The column we are currently using for sorting.
        private ColumnHeader SortingColumn = null;
        // Sort on this column.

        private void GameListView_HeaderClicked(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = GameListView.Columns[e.Column];

            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;
            if (SortingColumn == null)
            {
                // New column. Sort ascending.
                sort_order = SortOrder.Ascending;
            }
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    // Same column. Switch the sort order.
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    // New column. Sort ascending.
                    sort_order = SortOrder.Ascending;
                }

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            GameListView.ListViewItemSorter = new ListViewComparer(e.Column, sort_order);

            // Sort.
            GameListView.Sort();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings mainSettings = new Settings();
            mainSettings.ShowDialog();
        }

        private void GameListView_Clicked(object sender, MouseEventArgs e)
        {
            if (GameListView.SelectedItems.Count == 0)
                return;

            if (e.Button != MouseButtons.Right)
                return;

            contextMenuStrip1.Show(Cursor.Position);
            ListViewItem item = GameListView.SelectedItems[0];
            DockerGame game = DatabaseManager.GameDB[item.ImageKey];

            // disable all modifications on running app
            contextMenuStrip1.Enabled = !game.IsRunning;

            // disable options based on app properties
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

        private void AutomaticDetection(List<DockerGame> DetectedGames, string Platform)
        {
            foreach (DockerGame game in DetectedGames.Where(a => !Blacklist.Contains(a.Name)))
            {
                DialogResult dialogResult = CustomMessageBox.Show("Do you want to add [" + game.Name + "] to your Database ? ", Platform + " Automatic Detection", CustomMessageBox.eDialogButtons.YesNoCancel, game.Image);
                if (dialogResult == DialogResult.Yes)
                    InsertOrUpdateGameItem(game, true);
                else if (dialogResult == DialogResult.Cancel)
                    break;
            }
        }

        private void toolStripStartItem_Click(object sender, EventArgs e)
        {
            if (GameListView.SelectedItems.Count == 0)
                return;

            ListViewItem item = GameListView.SelectedItems[0];
            DockerGame game = DatabaseManager.GameDB[item.ImageKey];

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                Arguments = game.Arguments,
                WorkingDirectory = game.Uri,
                FileName = game.Executable
            };

            Process.Start(startInfo);
        }

        private void removeTheGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameListView.SelectedItems.Count == 0)
                return;

            ListViewItem item = GameListView.SelectedItems[0];
            DockerGame game = DatabaseManager.GameDB[item.ImageKey];

            if (game == null)
                return;

            DialogResult dialogResult = MessageBox.Show("This will remove " + game.Name + " from this database.", "Remove Title ?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string filename = Path.Combine(path_database, game.GUID + ".dat");

                if (File.Exists(filename))
                    File.Delete(filename);

                DatabaseManager.GameDB.TryRemove(game.GUID, out game);
                GameListView.Items.Remove(item);
                imageList1.Images.RemoveByKey(item.ImageKey);
                LogManager.UpdateLog("[" + game.Name + "] has been removed from the database");
            }
        }

        private void findAGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameProperties currentSettings = new GameProperties(this);

            // only display the settings window when an executable has been picked.
            if (currentSettings.GetIsReady())
            {
                DockerGame game = currentSettings.GetGame();
                if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
                    currentSettings.ShowDialog();
                else
                    SendNotification(game.Name + " already exists in your current database", true);
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (GameListView.SelectedItems.Count == 0)
                return;
            
            ListViewItem item = GameListView.SelectedItems[0];
            GameProperties currentSettings = new GameProperties(this, DatabaseManager.GameDB[item.ImageKey]);
            currentSettings.ShowDialog();
        }
    }

    public static class DictionaryExtensionMethods
    {
        public static bool ContentEquals<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> otherDictionary)
        {
            return (otherDictionary ?? new Dictionary<TKey, TValue>())
                .OrderBy(kvp => kvp.Key)
                .SequenceEqual((dictionary ?? new Dictionary<TKey, TValue>())
                                   .OrderBy(kvp => kvp.Key));
        }
    }
}
