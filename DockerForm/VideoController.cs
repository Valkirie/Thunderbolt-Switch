using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerForm
{
    public enum Constructor
    {
        Intel = 0,
        Nvidia = 1,
        AMD = 2
    }

    public class VideoController
    {
        public string DeviceID;
        public string Name;
        public string Description;
        public uint ConfigManagerErrorCode;
        public DateTime lastCheck;
        public Constructor Constructor;
        public bool IsExternal;

        internal void Initialize()
        {
            if (Name.ToLower().Contains("nvidia"))
                Constructor = Constructor.Nvidia;
            else if (Name.ToLower().Contains("amd"))
                Constructor = Constructor.AMD;
            else
                Constructor = Constructor.Intel;

            IsExternal = (Constructor != Constructor.Intel);
        }
    }
}
