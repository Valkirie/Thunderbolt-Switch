using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DockerForm
{
    [Flags]
    public enum ProfileMask
    {
        OnBattery = 0x01, // 0000 0000 0000 0001
        PluggedIn = 0x02, // 0000 0000 0000 0010
        ExternalGPU    = 0x04, // 0000 0000 0000 0100
        OnStartup    = 0x08, // 0000 0000 0000 1000
        ExternalScreen  = 0x16, // 0000 0000 0001 0000
    }

    public static class StringExtension
    {
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }

    [Serializable]
    public class PowerProfile : IDisposable
    {
        // private
        private string TurboBoostLongPowerMaxHex, TurboBoostShortPowerMaxHex;
        private string CPUCoreHex, IntelGPUHex, CPUCacheHex, SystemAgentHex;
        private string PowerBalanceCPUHex, PowerBalanceGPUHex;

        // public
        public string TurboBoostLongPowerMax, TurboBoostShortPowerMax;
        public string CPUCore, IntelGPU, CPUCache, SystemAgent;
        public string PowerBalanceCPU, PowerBalanceGPU;
        public string ProfileName = "";
        public byte ApplyMask = 0;
        public int ApplyPriority = 0;

        [NonSerialized()] public bool RunMe;
        [NonSerialized()] public string GameBounds;

        public string Serialize()
        {
            string filename = Path.Combine(Form1.path_profiles, ProfileName) + ".xml";
            using (FileStream writer = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }

            return "";
        }

        private string TDPToHex(int decValue)
        {
            decValue *= 8;
            return "0" + decValue.ToString("X");
        }

        private string VoltageToHex(int decValue)
        {
            // https://github.com/mihic/linux-intel-undervolt
            double temp = decValue * 1.024;
            decValue = (int)Math.Round(temp) << 21;
            return decValue.ToString("X");
        }

        public string GetName()
        {
            return ProfileName;
        }

        public string GetLongPowerMax()
        {
            return TurboBoostLongPowerMaxHex;
        }

        public string GetShortPowerMax()
        {
            return TurboBoostShortPowerMaxHex;
        }

        public string GetVoltageCPU()
        {
            return CPUCoreHex;
        }

        public string GetVoltageGPU()
        {
            return IntelGPUHex;
        }

        public string GetVoltageCache()
        {
            return CPUCacheHex;
        }

        public string GetVoltageSA()
        {
            return SystemAgentHex;
        }

        public string GetPowerBalanceCPU()
        {
            return PowerBalanceCPUHex;
        }

        public string GetPowerBalanceGPU()
        {
            return PowerBalanceGPUHex;
        }

        public PowerProfile()
        {
        }

        public bool HasLongPowerMax()
        {
            return TurboBoostLongPowerMax != null;
        }

        public bool HasShortPowerMax()
        {
            return TurboBoostShortPowerMax != null;
        }

        public bool HasSystemAgent()
        {
            return SystemAgent != null;
        }

        public bool HasCPUCore()
        {
            return CPUCore != null;
        }

        public bool HasIntelGPU()
        {
            return IntelGPU != null;
        }

        public bool HasCPUCache()
        {
            return CPUCache != null;
        }

        public bool HasPowerBalanceCPU()
        {
            return PowerBalanceCPU != null;
        }

        public bool HasPowerBalanceGPU()
        {
            return PowerBalanceGPU != null;
        }

        public void ComputeHex()
        {
            if (HasLongPowerMax())
                TurboBoostLongPowerMaxHex = TDPToHex(int.Parse(TurboBoostLongPowerMax));
            if (HasShortPowerMax())
                TurboBoostShortPowerMaxHex = TDPToHex(int.Parse(TurboBoostShortPowerMax));

            if (HasCPUCore())
                CPUCoreHex = VoltageToHex(int.Parse(CPUCore));
            if (HasIntelGPU())
                IntelGPUHex = VoltageToHex(int.Parse(IntelGPU));
            if (HasCPUCache())
                CPUCacheHex = VoltageToHex(int.Parse(CPUCache));
            if(HasSystemAgent())
                SystemAgentHex = VoltageToHex(int.Parse(SystemAgent));

            if (HasPowerBalanceCPU())
            {
                string hex = int.Parse(PowerBalanceCPU).ToString("X").GetLast(2);
                hex = hex.Length < 2 ? "0" + hex : hex;
                PowerBalanceCPUHex = hex;
            }
            if (HasPowerBalanceGPU())
            {
                string hex = int.Parse(PowerBalanceCPU).ToString("X").GetLast(2);
                hex = hex.Length < 2 ? "0" + hex : hex;
                PowerBalanceGPUHex = hex;
            }
        }

        public void DigestProfile(PowerProfile profile, bool Merging)
        {
            if (profile.HasLongPowerMax())
                TurboBoostLongPowerMax = profile.TurboBoostLongPowerMax;
            if (profile.HasShortPowerMax())
                TurboBoostShortPowerMax = profile.TurboBoostShortPowerMax;

            if (profile.HasCPUCore())
                CPUCore = profile.CPUCore;
            if (profile.HasIntelGPU())
                IntelGPU = profile.IntelGPU;
            if (profile.HasCPUCache())
                CPUCache = profile.CPUCache;
            if (profile.HasSystemAgent())
                SystemAgent = profile.SystemAgent;

            if (profile.HasPowerBalanceCPU())
                PowerBalanceCPU = profile.PowerBalanceCPU;
            if (profile.HasPowerBalanceGPU())
                PowerBalanceGPU = profile.PowerBalanceGPU;

            if (Merging)
                ProfileName += (ProfileName.Equals("") ? "" : ",") + profile.ProfileName;
            else
            {
                ProfileName = profile.ProfileName;
                ApplyMask = profile.ApplyMask;
                ApplyPriority = profile.ApplyPriority;
            }

            ComputeHex();
        }

        public override string ToString()
        {
            List<string> output = new List<string>();

            if (HasLongPowerMax())
                output.Add("TurboBoost LongPowerMax: " + TurboBoostLongPowerMax + "W");
            if (HasShortPowerMax())
                output.Add("TurboBoost ShortPowerMax: " + TurboBoostShortPowerMax + "W");

            if (HasCPUCore())
                output.Add("CPU Core: " + CPUCore + "mV");
            if (HasCPUCache())
                output.Add("CPU Cache: " + CPUCache + "mV");
            if (HasIntelGPU())
                output.Add("GPU Core: " + IntelGPU + "mV");
            if (HasSystemAgent())
                output.Add("System Agent: " + SystemAgent + "mV");

            if (HasPowerBalanceCPU())
                output.Add("PowerBalance CPU: " + PowerBalanceCPU);
            if (HasPowerBalanceGPU())
                output.Add("PowerBalance GPU: " + PowerBalanceGPU);

            if (output.Count != 0)
            {
                string myOutput = "";
                string latest = output.Last();
                foreach (string value in output)
                    myOutput += value + (value == latest ? "" : "\n");
                return myOutput;
            }

            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
