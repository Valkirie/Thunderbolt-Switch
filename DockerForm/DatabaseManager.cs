using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DockerForm
{
    class DatabaseManager
    {
        // DockerGame vars
        public static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();
        public static Dictionary<DockerGame, Process> GameProcesses = new Dictionary<DockerGame, Process>();

        public static void UpdateFilesAndRegistries(DockerGame game, string path_dest, string path_game, bool updateDB = true, bool updateFILE = true, bool pushToast = true)
        {
            foreach (GameSettings setting in game.Settings.Values.Where(a => a.IsEnabled))
            {
                string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));

                if (!File.Exists(filename))
                {
                    setting.IsEnabled = false;
                    continue;
                }

                if (setting.Type == SettingsType.File)
                {
                    // 1. Save current settings
                    if (updateDB)
                        setting.data[path_game] = File.ReadAllBytes(filename);

                    // 2. Restore proper settings
                    if (updateFILE && setting.data.ContainsKey(path_dest))
                    {
                        File.WriteAllBytes(filename, setting.data[path_dest]);
                        File.SetLastWriteTime(filename, game.LastCheck);
                    }
                }
                else if(setting.Type == SettingsType.Registry)
                {
                    // We generate a temporary reg file
                    string tempfile = Path.Combine(Form1.path_application, "temp.reg");

                    // 1. Save current settings
                    if (updateDB)
                    {
                        RegistryManager.ExportKey(filename, tempfile);
                        setting.data[path_game] = File.ReadAllBytes(tempfile);
                    }

                    // 2. Restore proper settings
                    if (updateFILE && setting.data.ContainsKey(path_dest))
                    {
                        File.WriteAllBytes(tempfile, setting.data[path_dest]);
                        RegistryManager.RestoreKey(tempfile);
                    }

                    // Delete the temporary reg file
                    File.Delete(tempfile);
                }
            }

            game.Serialize();

            Form1.SendNotification(game.Name + " settings have been updated for (" + path_dest + ")", pushToast);
        }

        public static void UpdateFilesAndRegistries(bool DockStatus)
        {
            string path_dest = DockStatus ? Form1.eGPU : Form1.iGPU;
            string path_game = DockStatus ? Form1.iGPU : Form1.eGPU;

            // Scroll the provided database
            foreach (DockerGame game in GameDB.Values)
                UpdateFilesAndRegistries(game, path_dest, path_game, true, true, false);

            Form1.SendNotification("All settings have been updated for (" + path_dest + ")", true);
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

        public static string GetCrc(string path_crc, string path_db)
        {
            string crc_value = File.Exists(path_crc) ? File.ReadAllText(path_crc) : path_db;
            return crc_value;
        }

        public static void SetCrc(string path_crc, string path_db)
        {
            if (File.Exists(path_crc))
                File.Delete(path_crc);

            File.WriteAllText(path_crc, path_db);
            File.SetAttributes(path_crc, FileAttributes.Hidden);
        }

        public static void SanityCheck()
        {
            string path_db = Form1.DockStatus ? Form1.eGPU : Form1.iGPU;

            foreach (DockerGame game in GameDB.Values)
            {
                if(game.ErrorCode != ErrorCode.None)
                {
                    switch(game.ErrorCode)
                    {
                        case ErrorCode.MissingExecutable: Form1.SendNotification(game.Name + " has an unreachable executable.", true); break;
                        case ErrorCode.MissingFolder: Form1.SendNotification(game.Name + " has an unreachable folder.", true); break;
                        case ErrorCode.MissingSettings: Form1.SendNotification(game.Name + " has no settings defined.", true); break;
                    }

                    continue;
                }

                string path_crc = Path.Combine(game.Uri, "donotdelete");
                string crc_value = GetCrc(path_crc, path_db);
                SetCrc(path_crc, path_db);

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
                        if(setting.data.ContainsKey((string)path_db))
                            fileDBBytes = setting.data[(string)path_db];
                    }
                    else if (setting.Type == SettingsType.Registry)
                    {
                        // We generate a temporary reg file
                        string tempfile = Path.Combine(Form1.path_application, "temp.reg");
                        
                        RegistryManager.ExportKey(filename, tempfile);
                        file = new FileInfo(tempfile);

                        fileBytes = File.ReadAllBytes(tempfile);
                        if (setting.data.ContainsKey((string)path_db))
                            fileDBBytes = setting.data[(string)path_db];

                        File.Delete(tempfile);
                    }

                    if (fileBytes == null || fileDBBytes == null)
                        return;

                    if (path_db != crc_value)
                    {
                        // Overwrite current database
                        UpdateFilesAndRegistries(game, crc_value, crc_value, true, false);

                        // Restore last known settings
                        UpdateFilesAndRegistries(game, path_db, path_db, false, true);

                        continue;
                    }
                    else if (file.LastWriteTime > game.LastCheck || !Equality(fileBytes,fileDBBytes))
                    {
                        // string generation
                        string WarningStr = "Your local " + game.Name + " Main files conflict with the ones stored in our Database.";
                        string ModifiedDB = "Last modified: " + game.LastCheck + " - " + (file.LastWriteTime > game.LastCheck ? "OLDER" : "NEWER");
                        string ModifiedLOCAL = "Last modified: " + file.LastWriteTime + " - " + (file.LastWriteTime < game.LastCheck ? "OLDER" : "NEWER");

                        DialogBox dialogBox = new DockerForm.DialogBox();
                        dialogBox.UpdateDialogBox("Database Sync Conflict", WarningStr, ModifiedDB, ModifiedLOCAL);

                        DialogResult dialogResult = dialogBox.ShowDialog();

                        if (dialogResult == DialogResult.Yes) // Overwrite current settings
                            UpdateFilesAndRegistries(game, path_db, path_db, false, true);
                        else if (dialogResult == DialogResult.No) // Overwrite current database
                            UpdateFilesAndRegistries(game, path_db, path_db, true, false);

                        continue;
                    }
                }
            }
        }

        public static List<string> SearchBattleNet()
        {
            List<string> listofBattleNetGames = new List<string>();

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
                                listofBattleNetGames.Add(filePath);
                        }
                    }
                }
            }
            
            return listofBattleNetGames;
        }

        public static List<string> SearchSteam()
        {
            List<string> listofSteamGames = new List<string>();

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
                                listofSteamGames.Add(filePath);
                        }
                    }
                }
            }

            return listofSteamGames;
        }
    }
}
