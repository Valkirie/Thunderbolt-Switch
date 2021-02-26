using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DockerForm
{
    public enum SettingsType
    {
        File = 0,
        Registry = 1
    }

    [Serializable]
    public class GameSettings
    {
        public SettingsType Type;
        public bool IsEnabled;
        public string FileName;
        public string Uri;
        public bool IsRelative;
        public Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();

        public GameSettings(string _filename, SettingsType _type, string _uri, bool _enabled, bool _relative)
        {
            this.FileName = _filename;
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
            return Type == SettingsType.File;
        }
    }

    public enum ErrorCode
    {
        None = 0,
        MissingFolder = 1,
        MissingExecutable = 2,
        MissingSettings = 3
    }

    public enum PlatformCode
    {
        Default = 0,
        Microsoft = 1,
        Steam = 2,
        BattleNet = 3
    }

    [Serializable]
    public class DockerGame : IDisposable
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
        public string crc_value = "";       // Crc
        public string Arguments = "";       // Executable arguments
        public bool Enabled = true;         // IsEnabled
        public DateTime LastCheck;          // Last time the game settings were saved
        public Bitmap Image = Properties.Resources.DefaultBackgroundImage;
        public PlatformCode Platform = PlatformCode.Default;

        public Dictionary<string, GameSettings> Settings = new Dictionary<string, GameSettings>();
        public Dictionary<string, PowerProfile> Profiles = new Dictionary<string, PowerProfile>();

        public bool PowerSpecific = false;  // Use power-specific settings (on battery, plugged in)

        [NonSerialized()] public ErrorCode ErrorCode = ErrorCode.None;
        [NonSerialized()] public bool IsRunning = false;
        public void SanityCheck()
        {
            ErrorCode = ErrorCode.None;

            if (!HasReachableFolder())
                ErrorCode = ErrorCode.MissingFolder;
            if (!HasReachableExe())
                ErrorCode = Platform != PlatformCode.Microsoft ? ErrorCode.MissingExecutable : ErrorCode.None;
            if (!HasFileSettings())
                ErrorCode = ErrorCode.MissingSettings;

            Enabled = ErrorCode == ErrorCode.None;
        }

        public string GetCrc()
        {
            return crc_value;
        }

        public void SetCrc(string _crc)
        {
            crc_value = _crc;
        }

        public DockerGame(DockerGame other)
        {
            this.Arguments = other.Arguments;
            this.Company = other.Company;
            this.crc_value = other.crc_value;
            this.Enabled = other.Enabled;
            this.ErrorCode = other.ErrorCode;
            this.Executable = other.Executable;
            this.FolderName = other.FolderName;
            this.GUID = other.GUID;
            this.IGDB_Url = other.IGDB_Url;
            this.Image = other.Image;
            this.LastCheck = other.LastCheck;
            this.Name = other.Name;
            this.Platform = other.Platform;
            this.ProductName = other.ProductName;
            this.Uri = other.Uri;
            this.Version = other.Version;

            this.Profiles = new Dictionary<string, PowerProfile>();
            if (other.Profiles != null)
                foreach (PowerProfile profile in other.Profiles.Values)
                    this.Profiles.Add(profile.ProfileName, profile);

            this.Settings = new Dictionary<string, GameSettings>();
            if (other.Settings != null)
                foreach (GameSettings settings in other.Settings.Values)
                    this.Settings.Add(settings.FileName, settings);

            this.PowerSpecific = other.PowerSpecific;

            this.SanityCheck();
        }

        public DockerGame(string filePath)
        {
            try
            {
                Dictionary<string, string> AppProperties = DatabaseManager.GetAppProperties(filePath);

                Executable = AppProperties["FileName"];
                ProductName = AppProperties.ContainsKey("FileDescription") ? AppProperties["FileDescription"] : AppProperties["ItemFolderNameDisplay"];
                Version = AppProperties.ContainsKey("FileVersion") ? AppProperties["FileVersion"] : "1.0.0.0";
                Company = AppProperties.ContainsKey("Company") ? AppProperties["Company"] : AppProperties.ContainsKey("Copyright") ? AppProperties["Copyright"] : "Unknown";
                Name = ProductName;
                GUID = "0x" + Math.Abs((Executable + ProductName).GetHashCode()).ToString();

                FileInfo fileInfo = new FileInfo(filePath);
                Uri = fileInfo.DirectoryName.ToLower();

                // cleanup before using as FolderName
                string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
                string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
                FolderName = System.Text.RegularExpressions.Regex.Replace(ProductName, invalidRegStr, "_").Replace(" ", "");
            }
            catch (Exception)
            { }

            try { Image = ShellEx.GetBitmapFromFilePath(filePath, ShellEx.IconSizeEnum.LargeIcon48); } catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

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

            string tempname = Path.Combine(Form1.path_database, GUID) + ".tmp";
            string filename = Path.Combine(Form1.path_database, GUID) + ".dat";
            using (FileStream fs = new FileStream(tempname, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, this);
                fs.Close();
            }

            if (File.Exists(filename))
                File.Delete(filename);

            // avoid database corruption on device crash
            File.Move(tempname, filename);
        }

        public bool HasReachableFolder()
        {
            return Directory.Exists(Uri);
        }

        public bool HasReadableExe(string filename)
        {
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    if (fs.CanRead)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception) { }

            return false;
        }

        public bool HasReachableExe()
        {
            string filename = Path.Combine(Uri, Executable);

            if (!File.Exists(filename))
                return false;

            if (!HasReadableExe(filename))
                return false;

            return true;
        }

        public bool HasFileSettings()
        {
            foreach (GameSettings setting in Settings.Values)
                if (setting.IsFile())
                    return true;
            return false;
        }

        public bool HasIGDB()
        {
            return IGDB_Url != "";
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
