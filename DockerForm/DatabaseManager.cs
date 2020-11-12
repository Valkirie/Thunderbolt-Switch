using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace DockerForm
{
    class DatabaseManager
    {
        // DockerGame vars
        public static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();
        private static bool b_Updating = false;

        public static Dictionary<string, string> GetAppProperties(string filePath1)
        {
            Dictionary<string, string> AppProperties = new Dictionary<string, string>();

            var shellFile = Microsoft.WindowsAPICodePack.Shell.ShellObject.FromParsingName(filePath1);
            foreach (var property in typeof(ShellProperties.PropertySystem).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var shellProperty = property.GetValue(shellFile.Properties.System, null) as IShellProperty;
                if (shellProperty?.ValueAsObject == null) continue;
                var shellPropertyValues = shellProperty.ValueAsObject as object[];
                if (shellPropertyValues != null && shellPropertyValues.Length > 0)
                {
                    foreach (var shellPropertyValue in shellPropertyValues)
                        AppProperties.Add(property.Name, "" + shellPropertyValue);
                }
                else
                    AppProperties.Add(property.Name, "" + shellProperty.ValueAsObject);
            }

            return AppProperties;
        }

        [DllImport("Kernel32.dll")]
        static extern uint QueryFullProcessImageName(IntPtr hProcess, uint flags, StringBuilder text, out uint size);

        public static string GetPathToApp(Process proc)
        {
            string pathToExe = string.Empty;

            try
            {
                if (null != proc)
                {
                    uint nChars = 256;
                    StringBuilder Buff = new StringBuilder((int)nChars);

                    uint success = QueryFullProcessImageName(proc.Handle, 0, Buff, out nChars);

                    if (0 != success)
                    {
                        pathToExe = Buff.ToString();
                    }
                    else
                    {
                        int error = Marshal.GetLastWin32Error();
                        pathToExe = ("Error = " + error + " when calling GetProcessImageFileName");
                    }
                }
            }
            catch (Exception) { }

            return pathToExe;
        }

        public static void UpdateFilesAndRegistries(DockerGame game, string path_dest, string path_game, bool updateDB, bool updateFILE, bool pushToast, string crc_value)
        {
            string path_crc = game.GUID + ".crc";
            foreach (GameSettings setting in game.Settings.Values.Where(a => a.IsEnabled))
            {
                string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));
                string file = Path.GetFileName(filename);

                if (!File.Exists(filename))
                {
                    setting.IsEnabled = false;
                    continue;
                }

                if (setting.Type == SettingsType.File)
                {
                    // We store the data
                    byte[] s_file = File.ReadAllBytes(filename);

                    // 1. Save current settings
                    if (updateDB)
                    {
                        setting.data[path_game] = s_file;
                        LogManager.UpdateLog("[" + game.Name + "]" + " database data were updated for file [" + file + "]");
                    }

                    // 2. Restore proper settings
                    if (updateFILE)
                    {
                        if (setting.data.ContainsKey(path_dest))
                        {
                            File.WriteAllBytes(filename, setting.data[path_dest]);
                            File.SetLastWriteTime(filename, game.LastCheck);
                            LogManager.UpdateLog("[" + game.Name + "]" + " local data were restored for file [" + file + "]");
                        }
                        else
                        {
                            LogManager.UpdateLog("[" + game.Name + "]" + " local data restore skipped for file [" + file + "] - no database data available");
                        }
                    }
                }
                else if(setting.Type == SettingsType.Registry)
                {
                    // We generate a temporary reg file
                    string tempfile = Path.Combine(Form1.path_application, "temp.reg");
                    RegistryManager.ExportKey(filename, tempfile);

                    // We store the data
                    byte[] s_file = File.ReadAllBytes(tempfile);

                    // 1. Save current settings
                    if (updateDB)
                    {
                        setting.data[path_game] = s_file;
                        LogManager.UpdateLog("[" + game.Name + "]" + " database registry data were updated for file [" + file + "]");
                    }

                    // 2. Restore proper settings
                    if (updateFILE)
                    {
                        if(setting.data.ContainsKey(path_dest))
                        {
                            File.WriteAllBytes(tempfile, setting.data[path_dest]);
                            RegistryManager.RestoreKey(tempfile);
                            LogManager.UpdateLog("[" + game.Name + "]" + " local registry data were restored for file [" + file + "]");
                        }
                        else
                        {
                            LogManager.UpdateLog("[" + game.Name + "]" + " local registry data restore skipped for file [" + file + "] - no database data available");
                        }
                    }

                    // Delete the temporary reg file
                    File.Delete(tempfile);
                }
            }

            game.SetCrc(crc_value);
            game.Serialize();

            Form1.SendNotification(game.Name + " settings have been updated for (" + path_dest + ")", pushToast);
        }

        public static bool IsUpdating()
        {
            return b_Updating;
        }

        public static void UpdateFilesAndRegistries(bool DockStatus, bool updateFILE = false, bool updateDB = false)
        {
            b_Updating = true;

            string path_db = DockStatus ? Form1.VideoControllers[Type.Discrete].Name : Form1.VideoControllers[Type.Internal].Name;

            Form1.SendNotification("Updating (" + path_db + ") database with docking status", true, true);

            foreach (DockerGame game in GameDB.Values)
                UpdateFilesAndRegistries(game, path_db, game.GetCrc(), updateDB, updateFILE, false, path_db);

            b_Updating = false;
        }

        public static bool Equality(byte[] a1, byte[] b1)
        {
            if (a1 == null || b1 == null)
                return false;

            // If not same length, done
            if (a1.Length != b1.Length)
            {
                return false;
            }

            // If they are the same object, done
            if (object.ReferenceEquals(a1, b1))
            {
                return true;
            }

            // Loop all values and compare
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != b1[i])
                {
                    return false;
                }
            }

            // If we got here, equal
            return true;
        }

        public static void SanityCheck()
        {
            string path_db = Form1.DockStatus ? Form1.VideoControllers[Type.Discrete].Name : Form1.VideoControllers[Type.Internal].Name;

            foreach (DockerGame game in GameDB.Values)
            {
                if(game.ErrorCode != ErrorCode.None)
                {
                    switch(game.ErrorCode)
                    {
                        case ErrorCode.MissingExecutable: LogManager.UpdateLog("[" + game.Name + "]" + " has an unreachable executable", true); break;
                        case ErrorCode.MissingFolder: LogManager.UpdateLog("[" + game.Name + "]" + " has an unreachable folder", true); break;
                        case ErrorCode.MissingSettings: LogManager.UpdateLog("[" + game.Name + "]" + " has no settings defined", true); break;
                    }

                    continue;
                }

                string path_crc = game.GUID + ".crc";
                string crc_value = game.GetCrc();

                foreach (GameSettings setting in game.Settings.Values.Where(a => a.IsEnabled))
                {
                    FileInfo file = null;
                    byte[] fileBytes = null, fileDBBytes = null;

                    string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));

                    if (!File.Exists(filename))
                    {
                        setting.IsEnabled = false;
                        continue;
                    }

                    if (setting.Type == SettingsType.File)
                    {
                        file = new FileInfo(filename);

                        fileBytes = File.ReadAllBytes(file.FullName);
                        if(setting.data.ContainsKey(path_db))
                            fileDBBytes = setting.data[path_db];
                    }
                    else if (setting.Type == SettingsType.Registry)
                    {
                        // We generate a temporary reg file
                        string tempfile = Path.Combine(Form1.path_application, "temp.reg");
                        
                        RegistryManager.ExportKey(filename, tempfile);
                        file = new FileInfo(tempfile);

                        fileBytes = File.ReadAllBytes(tempfile);
                        if (setting.data.ContainsKey(path_db))
                            fileDBBytes = setting.data[path_db];

                        File.Delete(tempfile);
                    }

                    if (fileBytes == null || fileDBBytes == null)
                        return;

                    if (path_db != crc_value)
                    {
                        Form1.SendNotification("CRC missmatch detected for " + game.Name + ". Settings will be restored. (CRC: " + crc_value + ", Current: " + path_db + ")", true, true);
                        
                        // Overwrite current database and restore last known settings
                        UpdateFilesAndRegistries(game, crc_value, path_db, true, true, false, path_db);

                        continue;
                    }
                    else if (!Equality(fileBytes,fileDBBytes))
                    {
                        Form1.SendNotification("Database sync conflict detected for " + game.Name, true, true);
                        
                        DialogBox dialogBox = new DialogBox();
                        dialogBox.UpdateDialogBox("Database Sync Conflict", game.Name, game.LastCheck, file.LastWriteTime);
                        DialogResult dialogResult = dialogBox.ShowDialog();

                        bool result = (dialogResult == DialogResult.Yes);
                        UpdateFilesAndRegistries(game, path_db, path_db, !result, result, true, path_db);

                        continue;
                    }
                }
            }

            Form1.IsFirstBoot = false;
        }

        public static List<DockerGame> SearchMicrosoftStore()
        {
            List<DockerGame> listofGames = new List<DockerGame>();
            string foldername = "C:\\Program Files\\WindowsApps";

            foreach(string folder in Directory.GetDirectories(foldername))
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    FileInfo myFile = new FileInfo(file);
                    if (myFile.Name.Equals("MicrosoftGame.config"))
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(file);

                        string TitleId = doc.DocumentElement["TitleId"].InnerText;
                        string Executable = doc.DocumentElement["ExecutableList"]["Executable"].Attributes[0].InnerText;
                        string DisplayName = doc.DocumentElement["ShellVisuals"].Attributes[0].InnerText;
                        string PublisherDisplayName = doc.DocumentElement["ShellVisuals"].Attributes[1].InnerText;
                        string StoreLogo = doc.DocumentElement["ShellVisuals"].Attributes[2].InnerText;

                        string filePath = Path.Combine(folder, Executable);
                        if (File.Exists(filePath))
                        {
                            DockerGame thisGame = new DockerGame(filePath);
                            thisGame.Platform = PlatformCode.Microsoft;
                            thisGame.Name = DisplayName;
                            thisGame.Company = PublisherDisplayName;
                            thisGame.SanityCheck();

                            string filename = Path.Combine(folder, StoreLogo);
                            thisGame.Image = FileManager.GetImage(filename);
                            listofGames.Add(thisGame);
                        }
                    }
                }
            }

            return listofGames;
        }

        public static List<DockerGame> SearchBattleNet()
        {
            List<DockerGame> listofGames = new List<DockerGame>();

            // HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall
            string regkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(regkey);

            foreach (string ksubKey in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(ksubKey))
                {
                    Dictionary<string, string> subKeys = new Dictionary<string, string>();

                    foreach (string subkeyname in subKey.GetValueNames())
                        subKeys.Add(subkeyname, subKey.GetValue(subkeyname).ToString());

                    if (subKeys.ContainsKey("UninstallString"))
                    {
                        string UninstallString = subKeys["UninstallString"];

                        if (UninstallString.Contains("Battle.net"))
                        {
                            string filePath = subKeys["DisplayIcon"];
                            if(File.Exists(filePath))
                            {
                                DockerGame thisGame = new DockerGame(filePath);
                                thisGame.Platform = PlatformCode.BattleNet;
                                thisGame.SanityCheck();
                                listofGames.Add(thisGame);
                            }
                        }
                    }
                }
            }
            
            return listofGames;
        }

        public static List<DockerGame> SearchSteam()
        {
            List<DockerGame> listofGames = new List<DockerGame>();

            // HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall
            string regkey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(regkey);

            foreach (string ksubKey in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(ksubKey))
                {
                    Dictionary<string, string> subKeys = new Dictionary<string, string>();

                    foreach (string subkeyname in subKey.GetValueNames())
                        subKeys.Add(subkeyname, subKey.GetValue(subkeyname).ToString());

                    if (subKeys.ContainsKey("UninstallString"))
                    {
                        string UninstallString = subKeys["UninstallString"];

                        if (UninstallString.Contains("steam.exe"))
                        {
                            string filePath = subKeys["DisplayIcon"];
                            if (File.Exists(filePath))
                            {
                                DockerGame thisGame = new DockerGame(filePath);
                                thisGame.Platform = PlatformCode.Steam;
                                thisGame.SanityCheck();
                                listofGames.Add(thisGame);
                            }
                        }
                    }
                }
            }

            return listofGames;
        }
    }
}
