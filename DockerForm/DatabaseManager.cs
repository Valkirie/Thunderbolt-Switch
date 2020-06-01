using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace DockerForm
{
    class DatabaseManager
    {
        // DockerGame vars
        public static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();
        public static ConcurrentDictionary<Process, DockerGame> GameProcesses = new ConcurrentDictionary<Process, DockerGame>();

        public static void UpdateFilesAndRegistries(DockerGame game, bool nextDockStatus, bool overwriteDB = true, bool restoreSETTING = true)
        {
            string path_game = Path.Combine(Form1.path_storage, game.FolderName, nextDockStatus ? Form1.iGPU : Form1.eGPU);
            string path_dest = Path.Combine(Form1.path_storage, game.FolderName, nextDockStatus ? Form1.eGPU : Form1.iGPU);

            if (!Directory.Exists(path_game))
                Directory.CreateDirectory(path_game);

            if (!Directory.Exists(path_dest))
                Directory.CreateDirectory(path_dest);

            foreach (GameSettings setting in game.Settings)
            {
                if (!setting.IsEnabled)
                    continue;

                string filename = setting.GetUri(game);
                if (setting.Type == "File") // file
                {
                    // 1. Save current settings
                    FileInfo file = new FileInfo(Environment.ExpandEnvironmentVariables(filename));
                    if (overwriteDB)
                        FileManager.CopyFile(file, path_game);

                    // 2. Restore proper settings
                    string path_file = Path.Combine(path_dest, file.Name);
                    FileInfo storedfile = new FileInfo(Environment.ExpandEnvironmentVariables(path_file));
                    if (restoreSETTING)
                        FileManager.CopyFile(storedfile, file.DirectoryName);
                }
                else // registry
                {
                    string registry = RegistryManager.GetRegistryFile(filename); ;

                    // 1. Save current settings
                    if (overwriteDB)
                        RegistryManager.ExportKey(filename, path_game, registry);

                    // 2. Restore proper settings
                    string path_file = Path.Combine(path_dest, registry);
                    if (restoreSETTING)
                        RegistryManager.RestoreKey(path_file);
                }
            }

            game.LastCheck = DateTime.Now;
            SerializeGame(game);
            Form1.NewToastNotification(game.Name + " settings have been updated.");
        }

        public static void UpdateFilesAndRegistries(bool Plugged)
        {
            // Scroll the provided database
            foreach (DockerGame game in GameDB.Values)
                UpdateFilesAndRegistries(game, Plugged);
        }

        public static void SanityCheck()
        {
            foreach (DockerGame game in GameDB.Values)
            {
                foreach (GameSettings setting in game.Settings)
                {
                    FileInfo file = null, fileDB = null;
                    string filename = setting.GetUri(game);

                    if (setting.Type == "File") // file
                    {
                        file = new FileInfo(Environment.ExpandEnvironmentVariables(filename));

                        string path_db = Path.Combine(Form1.path_storage, game.FolderName, Form1.DockStatus ? Form1.eGPU : Form1.iGPU, file.Name);
                        fileDB = new FileInfo(Environment.ExpandEnvironmentVariables(path_db));
                    }
                    else // registry
                    {
                        string registry = RegistryManager.GetRegistryFile(filename);
                        RegistryManager.ExportKey(filename, game.Uri, registry);

                        filename = Path.Combine(game.Uri, registry);
                        file = new FileInfo(Environment.ExpandEnvironmentVariables(filename));

                        string path_db = Path.Combine(Form1.path_storage, game.FolderName, Form1.DockStatus ? Form1.eGPU : Form1.iGPU, file.Name);
                        fileDB = new FileInfo(Environment.ExpandEnvironmentVariables(path_db));
                    }

                    if (!File.Exists(file.FullName) || !File.Exists(fileDB.FullName))
                        return;

                    string fileBytes = File.ReadAllText(file.FullName); // dirty but ReadBytes was causing issues
                    string fileDBBytes = File.ReadAllText(fileDB.FullName); // dirty but ReadBytes was causing issues
                    if (/* file.LastWriteTime > game.LastCheck || */ fileBytes != fileDBBytes)
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

        public static void SerializeGame(DockerGame game)
        {
            string filename = Path.Combine(Form1.path_database, game.FolderName) + ".dat";
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, game);
            fs.Close();
        }
    }
}
