using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerForm
{
    public class VideoController
    {
        public string DeviceID;
        public string Name;
        public string Description;
        public uint ConfigManagerErrorCode;
        public DateTime lastCheck;

        public bool IsDisable()
        {
            return (ConfigManagerErrorCode == 22);
        }

        public bool IsEnable()
        {
            return (ConfigManagerErrorCode == 0);
        }

        public bool IsIntegrated()
        {
            return (DeviceID == "VideoController1");
        }
    }
}
