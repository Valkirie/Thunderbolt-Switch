using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;

namespace DockerForm
{
    public class GameSettings
    {
        public string Type;
        public bool IsEnabled;
        public string Uri;
        public bool IsRelative;

        public GameSettings()
        {
        }

        public GameSettings(string _type, string _uri, bool _enabled, bool _relative)
        {
            this.Type = _type;
            this.Uri = _uri;
            this.IsEnabled = _enabled;
            this.IsRelative = _relative;
        }
    }

    public class DockerGame
    {
        public string Executable = "";      // Executable Name
        public string Name = "";            // Description
        public string ProductName = "";     // Product Name
        public string Artwork = "";         // Artwork File Name
        public string Version = "";         // Product Version
        public string GUID = "";            // Product GUID (harcoded)
        public string Uri = "";             // File path
        public string Company = "";         // Product Company

        [XmlIgnore]
        public Image Image;
        [XmlIgnore]
        public bool JustCreated = true;

        public List<GameSettings> Settings = new List<GameSettings>();

        public DockerGame()
        {
        }

        public bool CanSerialize()
        {
            if (GUID != "" && ProductName != "" && Executable != "")
                return true;
            return false;
        }
    }
}
