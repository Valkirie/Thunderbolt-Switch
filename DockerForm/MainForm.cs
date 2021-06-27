using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Task = Microsoft.Win32.TaskScheduler.Task;

namespace DockerForm
{
    public partial class MainForm : Form
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
        public static bool PlaySound = false;
        public static bool SpeechSynthesizer = false;

        // Devices vars
        public static Dictionary<Type, VideoController> VideoControllers;
        public static CPU CurrentCPU;
        private static GlobalKeyboardHook _globalKeyboardHook;
        private static SpeechSynthesizer CurrentSynthesizer;

        // Folder vars
        public static string path_application, path_database, path_dependencies, path_profiles;
        public static string path_rw, path_devcon, path_ryz;

        // Form vars
        private static MainForm CurrentForm;
        private static CultureInfo CurrentCulture;
        public static ResourceManager CurrentResource;

        // Threading vars
        private static Thread ThreadGPU, ThreadProfile;
        private static ManagementEventWatcher startWatcher;
        private static ManagementEventWatcher stopWatcher;
        private static ConcurrentDictionary<int, string> GameProcesses;

        // PowerProfile vars
        public static ConcurrentDictionary<Guid, PowerProfile> ProfileDB;
        public static PowerProfile CurrentProfile;

        // TaskManager vars
        private static Task CurrentTask;
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
                case PowerModes.Suspend:
                    break;
            }

            IsPowerPending = true;
        }

        public static bool CanRunProfile(PowerProfile profile, bool IsFirstBoot)
        {
            bool isOnBattery = profile._ApplyMask.HasFlag(ProfileMask.OnBattery);
            bool isPluggedIn = profile._ApplyMask.HasFlag(ProfileMask.PluggedIn);
            bool isExtGPU = profile._ApplyMask.HasFlag(ProfileMask.ExternalGPU);
            bool isOnBoot = profile._ApplyMask.HasFlag(ProfileMask.OnStartup);
            bool isOnStatusChange = profile._ApplyMask.HasFlag(ProfileMask.OnStatusChange);
            bool isOnScreen = profile._ApplyMask.HasFlag(ProfileMask.ExternalScreen);
            bool isGameBounds = profile._ApplyMask.HasFlag(ProfileMask.GameBounds);

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

        private void ApplyPowerProfiles()
        {
            PowerProfile sum_profile = new PowerProfile();
            foreach (PowerProfile profile in ProfileDB.Values.Where(a => a.RunMe).OrderBy(a => a.ApplyPriority))
                sum_profile.DigestProfile(profile, true);

            CurrentCPU.SetPowerProfile(sum_profile);

            // update form
            UpdateFormProfiles();
        }

        public static List<string> notifications = new List<string>();
        public static void SendNotification(string input, bool pushToast = false, bool pushLog = false, bool IsError = false)
        {
            if (pushLog)
                LogManager.UpdateLog(input, IsError);

            if (pushToast && ToastNotifications)
            {
                CurrentForm.notifyIcon1.BalloonTipText = input;
                CurrentForm.notifyIcon1.ShowBalloonTip(1000);
            }

            // avoid speeches duplication
            if(!notifications.Contains(input))
                notifications.Add(input);

            // avoid clashes of speeches
            CurrentSynthesizer.SpeakAsyncCancelAll();
        }

        private static void myTimer_Tick(object sender, EventArgs e)
        {
            // skip if empty
            if (notifications.Count == 0)
                return;

            // read text
            if (SpeechSynthesizer)
                CurrentSynthesizer.SpeakAsync(String.Join("\n", notifications));

            notifications.Clear();
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

                    foreach (PowerProfile profile in game.PowerProfiles.Values)
                        ProfileDB[profile.ProfileGuid].RunMe = CanRunProfile(profile, false);

                    game.IsRunning = true;

                    if (CurrentForm.InvokeRequired)
                        CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                        {
                            CurrentForm.UpdateGameDetails(game);
                            CurrentForm.ApplyPowerProfiles();
                        });
                    else
                    {
                        CurrentForm.UpdateGameDetails(game);
                        CurrentForm.ApplyPowerProfiles();
                    }
                }

            }
            catch (Exception ex)
            {
                LogManager.UpdateLog($"startWatch_EventArrived(): {ex.Message}", true);
            }
        }

        static void startWatch_Stopped(object sender, StoppedEventArgs e)
        {
            string content = string.Format(CurrentResource.GetString("WatcherStop"), "startWatcher");
            LogManager.UpdateLog(content);
            ((ManagementEventWatcher)sender).Dispose();
        }

        static void startWatch_Disposed(object sender, EventArgs e)
        {
            string content = string.Format(CurrentResource.GetString("WatcherDispose"), "startWatcher");
            LogManager.UpdateLog(content);
            if(MonitorProcesses)
                StartMonitoringProcessCreation();
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

                    GameProcesses.TryRemove(ProcessID, out PathToApp);

                    string path_db = GetCurrentState(game);
                    DatabaseManager.UpdateFilesAndRegistries(game, path_db, path_db, true, false, true, path_db);

                    foreach (PowerProfile profile in game.PowerProfiles.Values)
                        ProfileDB[profile.ProfileGuid].RunMe = false;

                    game.IsRunning = false;

                    if (CurrentForm.InvokeRequired)
                        CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                        {
                            CurrentForm.UpdateGameDetails(game);
                            CurrentForm.ApplyPowerProfiles();
                        });
                    else
                    {
                        CurrentForm.UpdateGameDetails(game);
                        CurrentForm.ApplyPowerProfiles();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.UpdateLog($"stopWatch_EventArrived(): {ex.Message}", true);
            }
        }

        static void stopWatch_Stopped(object sender, StoppedEventArgs e)
        {
            string content = string.Format(CurrentResource.GetString("WatcherStop"), "stopWatcher");
            LogManager.UpdateLog(content);
            ((ManagementEventWatcher)sender).Dispose();
        }

        static void stopWatch_Disposed(object sender, EventArgs e)
        {
            string content = string.Format(CurrentResource.GetString("WatcherDispose"), "stopWatcher");
            LogManager.UpdateLog(content);
            if(MonitorProcesses)
                StartMonitoringProcessTermination();
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
                string content = string.Empty;

                if (VideoControllers.ContainsKey(Type.Discrete))
                    content = string.Format(CurrentResource.GetString("VideoControllerDetected"), VideoControllers[Type.Discrete].Name);
                else if (VideoControllers.ContainsKey(Type.Internal))
                    content = string.Format(CurrentResource.GetString("VideoControllerDetected"), VideoControllers[Type.Internal].Name);

                SendNotification(content, true, true);

                DatabaseManager.UpdateFilesAndRegistries(false, true);
                DatabaseManager.UpdateFilesAndRegistries(true, false);
            }

            // update status
            prevDockStatus = DockStatus;
            IsHardwarePending = false;

            // update form
            if (CurrentForm.InvokeRequired)
                CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                {
                    UpdateFormConstructor();
                });
            else
                UpdateFormConstructor();
        }

        public static void UpdateMonitorPower()
        {
            PowerStatus = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
            IsPowerNew = prevPowerStatus != PowerStatus;

            // Power Status has changed
            if (IsPowerNew && !IsFirstBoot)
            {
                string content = string.Format(CurrentResource.GetString("PowerStatus"), GetCurrentPower());
                SendNotification(content, true, true);
            }

            // update status
            prevPowerStatus = PowerStatus;
            IsPowerPending = false;

            // update form
            if (CurrentForm.InvokeRequired)
                CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                {
                    UpdateFormConstructor();
                });
            else
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
                        ProfileDB[profile.ProfileGuid].RunMe = CanRunProfile(profile, IsFirstBoot);

                    if (CurrentForm.InvokeRequired)
                        CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                        {
                            CurrentForm.ApplyPowerProfiles();
                        });
                    else
                        CurrentForm.ApplyPowerProfiles();

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

        public void UpdateFormProfiles()
        {
            // profiles
            toolStripMenuItem2.DropDownItems.Clear();

            ToolStripMenuItem currentItem = new ToolStripMenuItem()
            {
                Text = "Current profile",
                ToolTipText = CurrentProfile.ToString(),
                Enabled = false
            };
            toolStripMenuItem2.DropDownItems.Add(currentItem);
            toolStripMenuItem2.DropDownItems.Add(new ToolStripSeparator());

            // do not display the default profile
            foreach (PowerProfile profile in ProfileDB.Values.Where(a => a.ApplyPriority != -1))
            {
                ToolStripMenuItem newItem = new ToolStripMenuItem()
                {
                    Text = profile.ProfileName,
                    Tag = profile.ProfileGuid,
                    Checked = profile.RunMe,
                    ToolTipText = profile.ToString()
                };
                newItem.Click += new EventHandler(PowerMenuClickHandler);
                toolStripMenuItem2.DropDownItems.Add(newItem);
            }
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
            CurrentForm.menuStrip2.Items[0].Text = ConstructorName + " (" + GetCurrentPower() + ")";
            CurrentForm.undockedToolStripMenuItem.Image = ConstructorLogo;
            CurrentForm.notifyIcon1.Icon = myIcon;
            CurrentForm.Icon = myIcon;
        }

        public int GetListViewIndex(string GUID)
        {
            foreach (ListViewItem item in GameListView.Items)
                if ((string)item.Tag == GUID)
                    return GameListView.Items.IndexOf(item);
            return -1;
        }

        public void UpdateGameDetails(DockerGame game)
        {
            // update game details
            game.LastCheck = DateTime.Now;

            // update LastCheck on application start
            int idx = GetListViewIndex(game.GUID);
            if (idx != -1)
                GameListView.Items[idx].SubItems[3].Text = game.LastCheck.ToString(CurrentCulture);
        }

        public void InsertOrUpdateGameItem(DockerGame game, bool auto)
        {
            ListViewItem newgame = new ListViewItem(new string[] { "", game.Company, game.Version, game.LastCheck.ToString(CurrentCulture), game.GetSettingsList(), game.GetProfilesList() }, game.GUID);
            newgame.Tag = game.GUID;
            newgame.Name = game.Name;
            newgame.Text = game.Name;

            if (!DatabaseManager.GameDB.ContainsKey(game.GUID))
            {
                // upate imageList
                imageList1.Images.Add(game.GUID, game.Image);
                imageList2.Images.Add(game.GUID, game.Image);

                GameListView.Items.Add(newgame);
                DatabaseManager.GameDB[game.GUID] = game;

                string content = string.Format(CurrentResource.GetString("DatabaseCreate"), game.Name);
                LogManager.UpdateLog(content);
            }
            else
            {
                // upate imageList
                if (imageList1.Images.ContainsKey(game.GUID))
                {
                    imageList1.Images.RemoveByKey(game.GUID);
                    imageList1.Images.Add(game.GUID, game.Image);
                }
                if (imageList2.Images.ContainsKey(game.GUID))
                {
                    imageList2.Images.RemoveByKey(game.GUID);
                    imageList2.Images.Add(game.GUID, game.Image);
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
                string content = string.Format(CurrentResource.GetString("DatabaseUpdate"), game.Name);
                LogManager.UpdateLog(content);
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
                            DatabaseManager.GameDB.AddOrUpdate(thisGame.GUID, thisGame, (key, value) => thisGame);

                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    LogManager.UpdateLog($"UpdateGameList(): {ex.Message}", true);
                }
            }

            // Update the listview images
            imageList1.Images.Clear();
            imageList2.Images.Clear();
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                imageList1.Images.Add(game.GUID, game.Image);
                imageList2.Images.Add(game.GUID, game.Image);
            }

            // Update the DockerGame database
            GameListView.BeginUpdate();
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                ListViewItem newgame = new ListViewItem(new string[] { "", game.Company, game.Version, game.LastCheck.ToString(CurrentCulture), game.GetSettingsList(), game.GetProfilesList() }, game.GUID);
                newgame.Tag = game.GUID;
                newgame.Name = game.Name;
                newgame.Text = game.Name;
                GameListView.Items.Add(newgame);
                // item.Enabled = game.Enabled;
            }
            GameListView.EndUpdate();
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(bmp, 0, 0, width, height);
            return result;
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

            Dictionary<Guid, PowerProfile> profileList = new Dictionary<Guid, PowerProfile>();
            List<Guid> removeList = new List<Guid>();
            List<string> deletelist = new List<string>();
            foreach (string filename in fileEntries)
            {
                try
                {
                    using (Stream reader = new FileStream(filename, FileMode.Open))
                    {
                        XmlSerializer formatter = new XmlSerializer(typeof(PowerProfile));
                        PowerProfile profile = (PowerProfile)formatter.Deserialize(reader);

                        // ensure backward compatibility with 1.05
                        if (profile.ProfileGuid == Guid.Empty)
                        {
                            profile.ProfileGuid = Guid.NewGuid();
                            profile.Serialize(false);
                            deletelist.Add(filename);
                        }
                        else
                        {
                            profileList.Add(profile.ProfileGuid, profile);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.UpdateLog($"UpdateProfiles(): {ex.Message}", true);
                }
            }

            // ensure backward compatibility with 1.05
            foreach (string filename in deletelist)
                File.Delete(filename);

            // add or update profiles
            foreach (PowerProfile profile in profileList.Values)
            {
                string ProfileName = profile.ProfileName;
                Guid ProfileGuid = profile.ProfileGuid;

                bool RunMe = ProfileDB.ContainsKey(ProfileGuid) ? ProfileDB[ProfileGuid].RunMe : false;

                ProfileDB[ProfileGuid] = profile;
                ProfileDB[ProfileGuid].RunMe = RunMe;

                ProfileDB[ProfileGuid].ComputeHex();
            }

            // insert all removed profiles
            foreach (Guid ProfileGuid in ProfileDB.Keys.Where(a => !profileList.ContainsKey(a)))
                removeList.Add(ProfileGuid);

            // remove obsolete profiles
            PowerProfile temp;
            foreach (Guid ProfileGuid in removeList)
                ProfileDB.TryRemove(ProfileGuid, out temp);

            // update games
            foreach (DockerGame game in DatabaseManager.GameDB.Values)
            {
                // remove outdated profiles
                foreach (Guid ProfileGuid in removeList)
                    game.PowerProfiles.Remove(ProfileGuid);

                // update associated profiles
                foreach (PowerProfile profile in ProfileDB.Values)
                    if (game.PowerProfiles.ContainsKey(profile.ProfileGuid))
                        game.PowerProfiles[profile.ProfileGuid] = profile;
            }

            // re-apply values
            if (CurrentForm.InvokeRequired)
                CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                {
                    CurrentForm.ApplyPowerProfiles();
                });
            else
                CurrentForm.ApplyPowerProfiles();

            // update var
            prevFileInfos = fileInfos;
        }

        private static void PowerMenuClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string ProfileName = item.Text;
            Guid ProfileGuid = (Guid)item.Tag;

            item.Checked = !item.Checked;
            ProfileDB[ProfileGuid].RunMe = item.Checked;

            if (CurrentForm.InvokeRequired)
                CurrentForm.BeginInvoke((MethodInvoker)delegate ()
                {
                    CurrentForm.ApplyPowerProfiles();
                });
            else
                CurrentForm.ApplyPowerProfiles();
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
            PlaySound = Properties.Settings.Default.PlaySound;
            SpeechSynthesizer = Properties.Settings.Default.SpeechSynthesizer;

            if (Properties.Settings.Default.MonitorProcesses)
            {
                StartMonitoringProcessCreation();
                StartMonitoringProcessTermination();
            }
            else
            {
                StopMonitoringProcessCreation();
                StopMonitoringProcessTermination();
            }

            if (Properties.Settings.Default.MonitorProfiles)
                StartMonitoringProfils();
            else
                StopMonitoringProfils();

            UpdateTask();
        }

        private List<Keys> KeyboardHookTriggers;
        private List<Keys> KeyboardHookListener;
        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            Keys loggedKey = e.KeyboardData.Key;
            int loggedVkCode = e.KeyboardData.VirtualCode;

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                // start listening keyboard
                if (KeyboardHookTriggers.Contains(loggedKey))
                    KeyboardHookListener.Add(loggedKey);
            }
            else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)
            {
                // stop listening keyboard
                if (KeyboardHookTriggers.Contains(loggedKey))
                {
                    foreach (PowerProfile profile in ProfileDB.Values)
                    {

                    }
                }
            }
        }

        public MainForm()
        {
            InitializeComponent();

            // initialize vars
            CurrentForm = this;
            CurrentCulture = CultureInfo.CurrentCulture;
            CurrentCPU = new CPU();
            CurrentSynthesizer = new SpeechSynthesizer();
            CurrentSynthesizer.SetOutputToDefaultAudioDevice();
            
            GameProcesses = new ConcurrentDictionary<int, string>();
            ProfileDB = new ConcurrentDictionary<Guid, PowerProfile>();
            CurrentProfile = new PowerProfile();
            VideoControllers = new Dictionary<Type, VideoController>();

            ThreadGPU = new Thread(MonitorThread);

            CurrentForm.CurrentTimer.Tick += new EventHandler(myTimer_Tick);

            KeyboardHookTriggers = new List<Keys>() { Keys.LControlKey, Keys.RControlKey };
            KeyboardHookListener = new List<Keys>();
            _globalKeyboardHook = new GlobalKeyboardHook(new Keys[] { Keys.LControlKey, Keys.RControlKey, Keys.A, Keys.B });
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;

            // folder settings
            path_application = AppDomain.CurrentDomain.BaseDirectory;
            LogManager.InitializeLog(path_application);

            // path settings
            path_database = Path.Combine(path_application, "database");
            path_dependencies = Path.Combine(path_application, "dependencies");
            path_profiles = Path.Combine(path_application, "profiles");
            path_rw = Path.Combine(path_dependencies, "Rw.exe");
            path_ryz = Path.Combine(path_dependencies, "ryzenadj.exe");
            path_devcon = Path.Combine(path_dependencies, Environment.Is64BitOperatingSystem ? "x64" : "x86", "DevManView.exe");

            if (!Directory.Exists(path_database))
                Directory.CreateDirectory(path_database);

            if (!Directory.Exists(path_dependencies))
                Directory.CreateDirectory(path_dependencies);

            if (!Directory.Exists(path_profiles))
                Directory.CreateDirectory(path_profiles);

            // language settings
            switch(CurrentCulture.Name)
            {
                case "fr-FR":
                    CurrentResource = new ResourceManager($"DockerForm.Resources.{CurrentCulture.Name}", Assembly.GetExecutingAssembly());
                    break;

                default:
                case "en-US":
                    CurrentResource = new ResourceManager($"DockerForm.Resources.en-US", Assembly.GetExecutingAssembly());
                    break;
            }

            // configurable settings
            settingsToolStripMenuItem.ToolTipText = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            settingsToolStripMenuItem.AutoToolTip = true;

            // read processor details
            CurrentCPU.Initialise();
            string content = string.Format(CurrentResource.GetString("DetectionCPU"), CurrentCPU.Name, CurrentCPU.Manuf);
            LogManager.UpdateLog(content);

            // update Settings
            DefineTask();
            UpdateSettings();

            // update Database
            UpdateGameList();

            // update Position and Size
            imageList1.ImageSize = new Size(Properties.Settings.Default.ImageWidth, Properties.Settings.Default.ImageHeight);
            imageList2.ImageSize = new Size(Properties.Settings.Default.SmallImageWidth, Properties.Settings.Default.SmallImageHeight);
            Location = new Point(Properties.Settings.Default.MainWindowX, Properties.Settings.Default.MainWindowY);
            Size = new Size(Properties.Settings.Default.MainWindowWidth, Properties.Settings.Default.MainWindowHeight);
            columnName.Width = Properties.Settings.Default.ColumnNameWidth;
            columnDev.Width = Properties.Settings.Default.ColumnDevWidth;
            columnVersion.Width = Properties.Settings.Default.ColumnVersionWidth;
            columnPlayed.Width = Properties.Settings.Default.ColumnPlayedWidth;
            columnSettings.Width = Properties.Settings.Default.ColumnSettingsWidth;

            switch (Properties.Settings.Default.GameListStyle)
            {
                case "List":
                    GameListView.View = View.List; break;
                case "LargeIcon":
                    GameListView.View = View.LargeIcon; break;
                case "SmallIcon":
                    GameListView.View = View.SmallIcon; break;
                case "Details":
                    GameListView.View = View.Details; break;
                default:
                case "Tile":
                    GameListView.View = View.Tile; break;
            }

            if (Properties.Settings.Default.MinimizeOnStartup)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        private static void DefineTask()
        {
            TaskService TaskServ = new TaskService();
            CurrentTask = TaskServ.FindTask(taskName);

            TaskDefinition td = TaskService.Instance.NewTask();
            td.Principal.RunLevel = TaskRunLevel.Highest;
            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Settings.DisallowStartIfOnBatteries = false;
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.ExecutionTimeLimit = TimeSpan.Zero;
            td.Settings.Enabled = false;
            td.Triggers.Add(new LogonTrigger());
            td.Actions.Add(new ExecAction(Path.Combine(path_application, "DockerForm.exe")));
            CurrentTask = TaskService.Instance.RootFolder.RegisterTaskDefinition(taskName, td);

            LogManager.UpdateLog(string.Format(CurrentResource.GetString("SchedulerCreate"), taskName));
        }

        public static void UpdateTask()
        {
            if (CurrentTask == null)
                return;

            if (BootOnStartup && !CurrentTask.Enabled)
            {
                CurrentTask.Enabled = true;
                LogManager.UpdateLog(string.Format(CurrentResource.GetString("SchedulerEnable"), taskName));
            }
            else if (!BootOnStartup && CurrentTask.Enabled)
            {
                CurrentTask.Enabled = false;
                LogManager.UpdateLog(string.Format(CurrentResource.GetString("SchedulerDisable"), taskName));
            }
        }

        private static void StartMonitoringProcessCreation()
        {
            startWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
            startWatcher.EventArrived += new EventArrivedEventHandler(startWatch_EventArrived);
            startWatcher.Stopped += new StoppedEventHandler(startWatch_Stopped);
            startWatcher.Disposed += new EventHandler(startWatch_Disposed);
            startWatcher.Start();

            string content = string.Format(CurrentResource.GetString("WatcherStart"), "startWatcher");
            LogManager.UpdateLog(content);
        }

        private static void StartMonitoringProcessTermination()
        {
            stopWatcher = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
            stopWatcher.EventArrived += new EventArrivedEventHandler(stopWatch_EventArrived);
            stopWatcher.Stopped += new StoppedEventHandler(stopWatch_Stopped);
            stopWatcher.Disposed += new EventHandler(stopWatch_Disposed);
            stopWatcher.Start();

            string content = string.Format(CurrentResource.GetString("WatcherStart"), "stopWatcher");
            LogManager.UpdateLog(content);
        }

        private static void StopMonitoringProcessCreation()
        {
            if (startWatcher != null)
                startWatcher.Stop();
        }

        private static void StopMonitoringProcessTermination()
        {
            if(stopWatcher != null)
                stopWatcher.Stop();
        }

        public static void StartMonitoringProfils()
        {
            if (ThreadProfile != null)
                return;

            ThreadProfile = new Thread(ProfilesMonitorThread);
            ThreadProfile.Start();
        }

        public static void StopMonitoringProfils()
        {
            if (ThreadProfile == null)
                ThreadProfile.Abort();
        }

        private void Form1_Shown(object sender, System.EventArgs e)
        {
            // update ProfileDB
            UpdateProfiles();

            // Monitor power profiles
            if (MonitorProfiles)
                StartMonitoringProfils();

            // search for GPUs
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

            if (MinimizeOnClosing && e.CloseReason == CloseReason.UserClosing && !ForceClose)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                Properties.Settings.Default.ColumnNameWidth = columnName.Width;
                Properties.Settings.Default.ColumnDevWidth = columnDev.Width;
                Properties.Settings.Default.ColumnVersionWidth = columnVersion.Width;
                Properties.Settings.Default.ColumnPlayedWidth = columnPlayed.Width;
                Properties.Settings.Default.ColumnSettingsWidth = columnSettings.Width;
                Properties.Settings.Default.Save();

                if (SaveOnExit)
                    DatabaseManager.UpdateFilesAndRegistries(false, true);
                IsRunning = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ForceClose = true;
            Close();
        }

        FormWindowState CurrentWindowState;
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (CurrentWindowState == WindowState)
                return;

            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;

                string content = CurrentResource.GetString("BackgroundRun");
                SendNotification(content, !IsFirstBoot);
            }

            CurrentWindowState = WindowState;
        }

        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            string folderPath = item.ToolTipText;
            if (Directory.Exists(folderPath))
                Process.Start(folderPath);
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
                Process.Start(folderPath);
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

        private ColumnHeader SortingColumn = null;
        private void GameListView_HeaderClicked(object sender, ColumnClickEventArgs e)
        {
            // Get the new sorting column.
            ColumnHeader new_sorting_column = GameListView.Columns[e.Column];
            // Figure out the new sorting order.
            System.Windows.Forms.SortOrder sort_order;

            if (SortingColumn == null)
                sort_order = SortOrder.Ascending;
            else
            {
                // See if this is the same column.
                if (new_sorting_column == SortingColumn)
                {
                    if (SortingColumn.Text.StartsWith("> "))
                        sort_order = SortOrder.Descending;
                    else
                        sort_order = SortOrder.Ascending;
                }
                else
                    sort_order = SortOrder.Ascending;

                // Remove the old sort indicator.
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
                SortingColumn.Text = "> " + SortingColumn.Text;
            else
                SortingColumn.Text = "< " + SortingColumn.Text;

            GameListView.ListViewItemSorter = new ListViewComparer(e.Column, sort_order);
            GameListView.Sort();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings mainSettings = new Settings();
            mainSettings.ShowDialog();
        }

        private void GameListView_DoubleClick(object sender, MouseEventArgs e)
        {
            // skip if not left click
            if (e.Button != MouseButtons.Left)
                return;

            // start the game on double click
            toolStripStartItem_Click(sender, e);
        }

        private void GameListView_Clicked(object sender, MouseEventArgs e)
        {
            // skip if not right click
            if (e.Button != MouseButtons.Right)
                return;

            if (GameListView.SelectedItems.Count == 0)
            {
                GameListView.ContextMenuStrip = contextMenuStrip3;
            }
            else
            {
                // a game is selected
                GameListView.ContextMenuStrip = contextMenuStrip1;
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
        }

        private void contextMenuStrip3_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (ToolStripMenuItem item in contextMenuStrip3.Items)
                item.Checked = false;

            switch (GameListView.View)
            {
                case View.List:
                    ((ToolStripMenuItem)contextMenuStrip3.Items[0]).Checked = true;
                    break;
                case View.LargeIcon:
                    ((ToolStripMenuItem)contextMenuStrip3.Items[1]).Checked = true;
                    break;
                case View.SmallIcon:
                    ((ToolStripMenuItem)contextMenuStrip3.Items[2]).Checked = true;
                    break;
                case View.Details:
                    ((ToolStripMenuItem)contextMenuStrip3.Items[3]).Checked = true;
                    break;
                case View.Tile:
                    ((ToolStripMenuItem)contextMenuStrip3.Items[4]).Checked = true;
                    break;
            }
        }

        private void styleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameListView.View = View.List;
            Properties.Settings.Default.GameListStyle = "List";
        }

        private void syleIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameListView.View = View.LargeIcon;
            Properties.Settings.Default.GameListStyle = "LargeIcon";
        }

        private void styleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GameListView.View = View.SmallIcon;
            Properties.Settings.Default.GameListStyle = "SmallIcon";
        }

        private void styleDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameListView.View = View.Details;
            Properties.Settings.Default.GameListStyle = "Details";
        }

        private void styleTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GameListView.View = View.Tile;
            Properties.Settings.Default.GameListStyle = "Tile";
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
            UpdateGameDetails(game);

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

                if (imageList1.Images.ContainsKey(game.GUID))
                    imageList1.Images.RemoveByKey(item.ImageKey);
                if (imageList2.Images.ContainsKey(game.GUID))
                    imageList2.Images.RemoveByKey(item.ImageKey);

                string content = string.Format(CurrentResource.GetString("DatabaseRemove"), game.Name);
                LogManager.UpdateLog(content);
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
                {
                    string content = string.Format(CurrentResource.GetString("DatabaseAlreadyExist"), game.Name);
                    SendNotification(content, true);
                }
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
