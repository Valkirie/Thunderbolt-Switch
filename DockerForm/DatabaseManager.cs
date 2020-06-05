using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace DockerForm
{
    class DatabaseManager
    {
        // DockerGame vars
        public static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();
        public static Dictionary<DockerGame, Process> GameProcesses = new Dictionary<DockerGame, Process>();

        public static void UpdateFilesAndRegistries(DockerGame game, bool nextDockStatus, bool updateDB = true, bool updateFILE = true, bool ignoreToast = false)
        {
            string path_game = nextDockStatus ? Form1.iGPU : Form1.eGPU;
            string path_dest = nextDockStatus ? Form1.eGPU : Form1.iGPU;

            if (!updateDB || !updateFILE) // dirty
                path_dest = path_game;

            foreach (GameSettings setting in game.Settings.Values)
            {
                if (!setting.IsEnabled)
                    continue;

                string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));
                if (setting.Type == "File") // file
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
                else // registry
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

            if(!ignoreToast)
                Form1.NewToastNotification(game.Name + " settings have been updated.");
        }

        public static void UpdateFilesAndRegistries(bool Plugged)
        {
            // Scroll the provided database
            foreach (DockerGame game in GameDB.Values)
                UpdateFilesAndRegistries(game, Plugged, true, true, true);
        }

        public static bool Equality(byte[] a1, byte[] b1)
        {
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
            foreach (DockerGame game in GameDB.Values)
            {
                foreach (GameSettings setting in game.Settings.Values)
                {
                    FileInfo file = null;
                    byte[] fileBytes = null, fileDBBytes = null;

                    string filename = Environment.ExpandEnvironmentVariables(setting.GetUri(game));
                    string path_db = Form1.DockStatus ? Form1.eGPU : Form1.iGPU;

                    if (setting.Type == "File") // file
                    {
                        file = new FileInfo(filename);

                        fileBytes = File.ReadAllBytes(file.FullName);
                        fileDBBytes = setting.data[path_db];
                    }
                    else // registry
                    {
                        // We generate a temporary reg file
                        string tempfile = Path.Combine(Form1.path_application, "temp.reg");

                        RegistryManager.ExportKey(filename, tempfile);
                        file = new FileInfo(tempfile);

                        fileBytes = File.ReadAllBytes(tempfile);
                        fileDBBytes = setting.data[path_db];

                        File.Delete(tempfile);
                    }

                    if (file.LastWriteTime > game.LastCheck || !Equality(fileBytes,fileDBBytes))
                    {
                        // string generation
                        string WarningStr = "Your local " + game.Name + " Main files conflict with the ones stored in our Database.";
                        string ModifiedDB = "Last modified: " + game.LastCheck + " - " + (file.LastWriteTime > game.LastCheck ? "OLDER" : "NEWER");
                        string ModifiedLOCAL = "Last modified: " + file.LastWriteTime + " - " + (file.LastWriteTime < game.LastCheck ? "OLDER" : "NEWER");

                        DialogBox dialogBox = new DockerForm.DialogBox();
                        dialogBox.UpdateDialogBox("Database Sync Conflict", WarningStr, ModifiedDB, ModifiedLOCAL);

                        DialogResult dialogResult = dialogBox.ShowDialog();

                        if (dialogResult == DialogResult.Yes) // Overwrite current settings
                            UpdateFilesAndRegistries(game, !Form1.DockStatus, false, true);
                        else if (dialogResult == DialogResult.No) // Overwrite current database
                            UpdateFilesAndRegistries(game, !Form1.DockStatus, true, false);

                        continue;
                    }
                }
            }
        }
    }
}
