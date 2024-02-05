using Microsoft.VisualBasic;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
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
        // DllImports
        [DllImport("kernel32.dll")]
        private static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags, StringBuilder lpExeName, out int size);
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(Int32 dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        // DockerGame vars
        public static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();

        public static Dictionary<string, string> GetAppProperties(string filePath1)
        {
            Dictionary<string, string> AppProperties = new Dictionary<string, string>();

            ShellObject shellFile = ShellObject.FromParsingName(filePath1);
            foreach (var property in typeof(ShellProperties.PropertySystem).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                IShellProperty shellProperty = property.GetValue(shellFile.Properties.System, null) as IShellProperty;
                if (shellProperty?.ValueAsObject == null) continue;
                if (AppProperties.ContainsKey(property.Name)) continue;

                string[] shellPropertyValues = shellProperty.ValueAsObject as string[];

                if (shellPropertyValues != null && shellPropertyValues.Length > 0)
                {
                    foreach (string shellPropertyValue in shellPropertyValues)
                        AppProperties[property.Name] = shellPropertyValue.ToString();
                }
                else
                    AppProperties[property.Name] = shellProperty.ValueAsObject.ToString();
            }

            return AppProperties;
        }

        public static string GetPathToApp(Process proc)
        {
            var buffer = new StringBuilder(1024);
            IntPtr hprocess = OpenProcess(0x00001000,false, proc.Id);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    CloseHandle(hprocess);
                }
            }

            return string.Empty;
        }

        public static void UpdateFileAndRegistry(DockerGame game, string path_dest, string path_game, bool updateDB, bool updateFILE, bool pushToast, string crc_value, GameSettings setting)
        {
            string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));
            string file = Path.GetFileName(filename);
            string gamefile = game.Uri + '\\' + game.Executable;
            if (setting.Type == SettingsType.File)
            {
                if (!File.Exists(filename) && !File.Exists(gamefile)) //If Game No Longer Exist 
                {
                    setting.IsEnabled = false;
                    string content = string.Format(MainForm.CurrentResource.GetString("DatabaseSettingsDisabled"), game.Name, file, path_dest, "file");
                    LogManager.UpdateLog(content);
                    return;
                }

                // 1. Save current settings
                if (updateDB && File.Exists(filename))
                {
                    setting.data[path_game] = File.ReadAllBytes(filename);
                    string content = string.Format(MainForm.CurrentResource.GetString("DatabaseFileUpdate"), game.Name, file, path_game, "file");
                    LogManager.UpdateLog(content);
                }

                // 2. Restore proper settings
                if (updateFILE)
                {
                    if (setting.removeunused != null && setting.removeunused.ContainsKey(path_dest) && setting.removeunused[path_dest] == true) File.Delete(filename);
                    else
                    {
                        if (setting.data.ContainsKey(path_dest))
                        {
                            File.WriteAllBytes(filename, setting.data[path_dest]);
                            File.SetLastWriteTime(filename, game.LastCheck);
                            string content = string.Format(MainForm.CurrentResource.GetString("DatabaseSettingsUpdate"), game.Name, file, path_dest, "file");
                            LogManager.UpdateLog(content);
                        }
                        else
                        {
                            string content = string.Format(MainForm.CurrentResource.GetString("DatabaseSettingsSkipped"), game.Name, file, path_dest, "file");
                            LogManager.UpdateLog(content);
                        }
                    }
                }
            }
            else if (setting.Type == SettingsType.Registry)
            {
                // We generate a temporary reg file
                string tempfile = Path.Combine(MainForm.path_application, "temp.reg");
                RegistryManager.ExportKey(filename, tempfile);

                if (!File.Exists(tempfile) && !File.Exists(gamefile))
                {
                    setting.IsEnabled = false;
                    string content = string.Format(MainForm.CurrentResource.GetString("DatabaseSettingsDisabled"), game.Name, filename, path_dest, "registry entry");
                    LogManager.UpdateLog(content);
                    return;
                }

                // 1. Save current settings
                if (updateDB)
                {
                    setting.data[path_game] = File.ReadAllBytes(tempfile);
                    string content = string.Format(MainForm.CurrentResource.GetString("DatabaseFileUpdate"), game.Name, filename, path_game, "registry entry");
                    LogManager.UpdateLog(content);
                }

                // 2. Restore proper settings
                if (updateFILE)
                {
                    if (setting.data.ContainsKey(path_dest))
                    {
                        File.WriteAllBytes(tempfile, setting.data[path_dest]);
                        RegistryManager.RestoreKey(tempfile);
                        string content = string.Format(MainForm.CurrentResource.GetString("DatabaseSettingsUpdate"), game.Name, filename, path_dest, "registry entry");
                        LogManager.UpdateLog(content);
                    }
                    else
                    {
                        string content = string.Format(MainForm.CurrentResource.GetString("DatabaseSettingsSkipped"), game.Name, filename, path_dest, "registry entry");
                        LogManager.UpdateLog(content);
                    }
                }

                // Delete the temporary reg file
                File.Delete(tempfile);
            }
        }

        public static void UpdateFilesAndRegistries(DockerGame game, string path_dest, string path_game, bool updateDB, bool updateFILE, bool pushToast, string crc_value)
        {
            foreach (GameSettings setting in game.Settings.Values.Where(a => a.IsEnabled))
                UpdateFileAndRegistry(game, path_dest, path_game, updateDB, updateFILE, pushToast, crc_value, setting);

            game.SetCrc(crc_value);
            game.Serialize();

            string content = string.Format(MainForm.CurrentResource.GetString("DatabaseUpdate"), game.Name);
            MainForm.SendNotification(content, pushToast);
        }

        public static void UpdateFilesAndRegistries(bool updateFILE, bool updateDB)
        {
            foreach (DockerGame game in GameDB.Values)
            {
                string path_db = MainForm.GetCurrentState(game);
                UpdateFilesAndRegistries(game, path_db, game.GetCrc(), updateDB, updateFILE, false, path_db);
            }
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

        public static bool SanityCheck()
        {
            foreach (DockerGame game in GameDB.Values)
            {
                string path_db = MainForm.GetCurrentState(game);

                if (game.ErrorCode != ErrorCode.None)
                {
                    string content = string.Empty;
                    switch (game.ErrorCode)
                    {
                        case ErrorCode.MissingExecutable:
                            content = string.Format(MainForm.CurrentResource.GetString("ErrorUnreachableExe"), game.Name);
                            break;
                        case ErrorCode.MissingFolder:
                            content = string.Format(MainForm.CurrentResource.GetString("ErrorUnreachableFolder"), game.Name);
                            break;
                        case ErrorCode.MissingSettings:
                            content = string.Format(MainForm.CurrentResource.GetString("ErrorNoSettings"), game.Name);
                            break;
                    }
                    LogManager.UpdateLog(content, true);

                    continue;
                }

                string path_crc = game.GUID + ".crc";
                string crc_value = game.GetCrc();

                foreach (GameSettings setting in game.Settings.Values.Where(a => a.IsEnabled))
                {
                    FileInfo file = null;
                    byte[] fileBytes = null, fileDBBytes = null;

                    string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));
                    string gamefile = game.Uri + '\\' + game.Executable;

                    if (!File.Exists(filename) && !File.Exists(gamefile))
                    {
                        setting.IsEnabled = false;
                        continue;
                    }

                    if (setting.Type == SettingsType.File && File.Exists(filename))
                    {
                        file = new FileInfo(filename);

                        fileBytes = File.ReadAllBytes(file.FullName);
                        if (setting.data.ContainsKey(path_db))
                            fileDBBytes = setting.data[path_db];
                    }
                    else if (setting.Type == SettingsType.Registry)
                    {
                        // We generate a temporary reg file
                        string tempfile = Path.Combine(MainForm.path_application, "temp.reg");

                        RegistryManager.ExportKey(filename, tempfile);
                        file = new FileInfo(tempfile);

                        fileBytes = File.ReadAllBytes(tempfile);
                        if (setting.data.ContainsKey(path_db))
                            fileDBBytes = setting.data[path_db];

                        File.Delete(tempfile);
                    }

                    if (fileBytes == null || fileDBBytes == null)
                        return true;

                    if (path_db != crc_value)
                    {
                        string content = string.Format(MainForm.CurrentResource.GetString("DatabaseMissmatch"), game.Name, file.Name);
                        MainForm.SendNotification(content, true, true, true);
                        UpdateFileAndRegistry(game, path_db, crc_value, true, true, false, path_db, setting);
                    }
                    else if (!Equality(fileBytes, fileDBBytes))
                    {
                        string title = string.Format(MainForm.CurrentResource.GetString("DatabasConflictTitle"));
                        string content = string.Format(MainForm.CurrentResource.GetString("DatabaseConflict"), game.Name, file.Name);
                        MainForm.SendNotification(content, true, true, true);

                        DialogBox dialogBox = new DialogBox();
                        dialogBox.UpdateDialogBox(title, game.Name, game.LastCheck, file);
                        DialogResult dialogResult = dialogBox.ShowDialog();

                        bool result = (dialogResult == DialogResult.Yes);
                        UpdateFileAndRegistry(game, path_db, path_db, !result, result, true, path_db, setting);
                    }
                }

                game.SetCrc(path_db);
                game.Serialize();
            }

            return true;
        }

        public static List<DockerGame> SearchMicrosoftStore()
        {
            List<DockerGame> listofGames = new List<DockerGame>();
            string foldername = "C:\\Program Files\\WindowsApps";

            foreach (string folder in Directory.GetDirectories(foldername).Where(a => a.Contains("x86") || a.Contains("x64")))
            {
                foreach (string file in Directory.GetFiles(folder))
                {
                    FileInfo myFile = new FileInfo(file);
                    if (myFile.Name.Equals("AppxManifest.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        XmlDocument doc = new XmlDocument();
                        // prevent crash if file is being read/write by Microsoft Store
                        try
                        {
                            doc.Load(file);
                        }
                        catch (Exception e)
                        {
                            LogManager.UpdateLog($"SearchMicrosoftStore(): {e.Message}", true);
                        }

                        string IdentityName = "";
                        string IdentityVersion = "";
                        string DisplayName = "";
                        string PublisherDisplayName = "";
                        string Executable = "";
                        string StoreLogo = "";
                        string StoreLogoScale = "";

                        XmlNodeList Identity = doc.GetElementsByTagName("Identity");
                        foreach (XmlNode node in Identity)
                        {
                            if (node.Attributes != null)
                            {
                                foreach (XmlAttribute attribute in node.Attributes)
                                {
                                    switch (attribute.Name)
                                    {
                                        case "Name":
                                            IdentityName = attribute.InnerText;
                                            break;
                                        case "Version":
                                            IdentityVersion = attribute.InnerText;
                                            break;
                                    }
                                }
                            }
                        }

                        Identity = doc.GetElementsByTagName("Properties");
                        foreach (XmlNode node in Identity)
                        {
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                switch (child.Name)
                                {
                                    case "DisplayName":
                                        DisplayName = child.InnerText;
                                        break;
                                    case "PublisherDisplayName":
                                        PublisherDisplayName = child.InnerText;
                                        break;
                                }
                            }
                        }

                        Identity = doc.GetElementsByTagName("Resources");
                        foreach (XmlNode node in Identity)
                        {
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.Attributes != null)
                                {
                                    foreach (XmlAttribute attribute in child.Attributes)
                                    {
                                        if (attribute.Name.Equals("uap:Scale"))
                                        {
                                            StoreLogoScale = attribute.InnerText;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        Identity = doc.GetElementsByTagName("Applications");
                        foreach (XmlNode node in Identity)
                        {
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.Name.Equals("Application"))
                                {
                                    if (child.Attributes != null)
                                    {
                                        foreach (XmlAttribute attribute in child.Attributes)
                                        {
                                            switch (attribute.Name)
                                            {
                                                case "Executable":
                                                    Executable = attribute.InnerText;
                                                    break;
                                            }
                                            continue;
                                        }
                                    }

                                    foreach (XmlNode subchild in child.ChildNodes)
                                    {
                                        if (subchild.Attributes != null)
                                        {
                                            foreach (XmlAttribute attribute in subchild.Attributes)
                                            {
                                                if (attribute.Name.Contains("Logo"))
                                                {
                                                    if (attribute.Name.Contains("Square"))
                                                    {
                                                        StoreLogo = attribute.InnerText;
                                                        break;
                                                    }
                                                    else if (attribute.Name.Contains("Wide"))
                                                    {
                                                        StoreLogo = attribute.InnerText;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (!StoreLogoScale.Equals(""))
                            StoreLogo = StoreLogo.Replace(".", ".scale-" + StoreLogoScale + ".");

                        string filePath = Path.Combine(folder, Executable);
                        if (File.Exists(filePath))
                        {
                            DockerGame thisGame = new DockerGame(filePath);
                            thisGame.Platform = PlatformCode.Microsoft;
                            thisGame.Name = DisplayName.Contains("ms-resource") ? IdentityName : DisplayName;
                            thisGame.Company = PublisherDisplayName;

                            string filename = Path.Combine(folder, StoreLogo);
                            if (File.Exists(filename))
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
                            if (File.Exists(filePath))
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

            // HKEY_CURRENT_USER\System\GameConfigStore\Children
            string regkey = "System\\GameConfigStore\\Children";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(regkey);

            foreach (string ksubKey in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(ksubKey))
                {
                    Dictionary<string, string> subKeys = new Dictionary<string, string>();

                    foreach (string subkeyname in subKey.GetValueNames())
                        subKeys.Add(subkeyname, subKey.GetValue(subkeyname).ToString());

                    if (subKeys.ContainsKey("MatchedExeFullPath"))
                    {
                        string filePath = subKeys["MatchedExeFullPath"];

                        if (filePath.Contains("steamapps"))
                        {
                            if (File.Exists(filePath))
                            {
                                DockerGame thisGame = new DockerGame(filePath);
                                thisGame.Platform = PlatformCode.Default;
                                thisGame.SanityCheck();
                                listofGames.Add(thisGame);
                            }
                        }
                    }
                }
            }
            
            return listofGames;
        }

        public static List<DockerGame> SearchUniversal()
        {
            List<DockerGame> listofGames = new List<DockerGame>();

            // HKEY_CURRENT_USER\System\GameConfigStore\Children
            string regkey = "System\\GameConfigStore\\Children";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(regkey);

            // HKEY_CURRENT_USER\System\GameConfigStore\Children
            regkey = "System\\GameConfigStore\\Children";
            key = Registry.CurrentUser.OpenSubKey(regkey);

            foreach (string ksubKey in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(ksubKey))
                {
                    Dictionary<string, string> subKeys = new Dictionary<string, string>();

                    foreach (string subkeyname in subKey.GetValueNames())
                        subKeys.Add(subkeyname, subKey.GetValue(subkeyname).ToString());

                    if (subKeys.ContainsKey("MatchedExeFullPath"))
                    {
                        string filePath = subKeys["MatchedExeFullPath"];

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

            return listofGames;
        }
    }
}
