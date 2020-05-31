using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DockerForm
{
    class DatabaseManager
    {
        // DockerGame vars
        public static ConcurrentDictionary<string, DockerGame> GameDB = new ConcurrentDictionary<string, DockerGame>();
        public static ConcurrentDictionary<Process, DockerGame> GameProcesses = new ConcurrentDictionary<Process, DockerGame>();
        static Form1 _instance;

        public DatabaseManager(Form1 form)
        {
            _instance = form;
        }

        /// <summary>
        /// This function is making sure there is no difference between the database file and the current game settings file.
        /// TODO: Check registry.
        /// </summary>
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

                        string path_db = Path.Combine(Form1.path_storage, game.MakeValidFileName(), Form1.DockStatus ? "eGPU" : "iGPU", file.Name);
                        fileDB = new FileInfo(Environment.ExpandEnvironmentVariables(path_db));
                    }
                    else // registry
                    {
                        string registry = RegistryManager.GetRegistryFile(filename);
                        RegistryManager.ExportKey(filename, game.Uri, registry);

                        filename = Path.Combine(game.Uri, registry);
                        file = new FileInfo(Environment.ExpandEnvironmentVariables(filename));

                        string path_db = Path.Combine(Form1.path_storage, game.MakeValidFileName(), Form1.DockStatus ? "eGPU" : "iGPU", file.Name);
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
                            Form1.UpdateFilesAndRegistries(game, !Form1.DockStatus, false, true);
                        else if (dialogResult == DialogResult.No) // Overwrite current database
                            Form1.UpdateFilesAndRegistries(game, !Form1.DockStatus, true, false);

                        continue;
                    }
                }
            }
        }
    }
}
