using System;

namespace DockerForm
{
    public enum Constructor
    {
        Intel = 0,
        Nvidia = 1,
        AMD = 2
    }

    public enum Type
    {
        Internal = 0,
        Discrete = 1
    }

    public class VideoController
    {
        public string DeviceID;
        public string Name;
        public string Description;
        public uint ConfigManagerErrorCode;
        public DateTime lastCheck;
        public Constructor Constructor;
        public Type Type;

        internal void Initialize()
        {
            if (Name.ToLower().Contains("nvidia"))
                Constructor = Constructor.Nvidia;
            else if (Name.ToLower().Contains("amd"))
                Constructor = Constructor.AMD;
            else
                Constructor = Constructor.Intel;

            Type = Constructor == Constructor.Intel ? Type.Internal : Type.Discrete;
        }
    }
}
