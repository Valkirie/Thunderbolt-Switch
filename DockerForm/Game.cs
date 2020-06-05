using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DockerForm
{
    [Serializable]
    public class GameSettings
    {
        public int GUID;
        public string Type;
        public bool IsEnabled;
        public string Uri;
        public bool IsRelative;
        public Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();

        public GameSettings(int _guid, string _type, string _uri, bool _enabled, bool _relative)
        {
            this.GUID = _guid;
            this.Type = _type;
            this.Uri = _uri;
            this.IsEnabled = _enabled;
            this.IsRelative = _relative;
        }

        public string GetUri(DockerGame thisGame)
        {
            string filename = this.Uri;
            if (this.IsRelative)
                filename = Path.Combine(thisGame.Uri, filename);

            return filename;
        }

        public bool IsFile()
        {
            return Type == "File";
        }
    }

    [Serializable]
    public class DockerGame
    {
        public string Executable = "";      // Executable Name
        public string Name = "";            // Description
        public string ProductName = "";     // Product Name
        public string FolderName = "";      // Folder Name
        public string Version = "";         // Product Version
        public string GUID = "";            // Product GUID (harcoded)
        public string IGDB_Url = "";        // IGDB GUID
        public string Uri = "";             // File path
        public string Company = "";         // Product Company
        public DateTime LastCheck;          // Last time the game settings were saved
        public Bitmap Image = Properties.Resources.DefaultBackgroundImage;

        public Dictionary<int, GameSettings> Settings = new Dictionary<int, GameSettings>();

        public bool CanSerialize()
        {
            if (GUID != "" && ProductName != "" && Executable != "")
                return true;
            return false;
        }

        public void Serialize()
        {
            // Store the last time this game was updated
            LastCheck = DateTime.Now;

            string filename = Path.Combine(Form1.path_database, FolderName) + ".dat";
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, this);
            fs.Close();
        }

        public void SetFolderName()
        {
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            FolderName = System.Text.RegularExpressions.Regex.Replace(ProductName, invalidRegStr, "_");
            FolderName = FolderName.Replace(" ", "");
        }

        public bool HasFileSettings()
        {
            foreach (GameSettings setting in Settings.Values)
                if (setting.IsFile())
                    return true;
            return false;
        }
    }
}
