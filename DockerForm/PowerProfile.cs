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
        ExtGPU    = 0x04, // 0000 0000 0000 0100
        OnBoot    = 0x08, // 0000 0000 0000 1000
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
        public int TurboBoostLongPowerMax, TurboBoostShortPowerMax;
        public int CPUCore, IntelGPU, CPUCache, SystemAgent;
        public int PowerBalanceCPU, PowerBalanceGPU;
        public string ProfileName = "";
        public byte ApplyMask;

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
            return TurboBoostLongPowerMax != 0;
        }

        public bool HasShortPowerMax()
        {
            return TurboBoostShortPowerMax != 0;
        }

        public bool HasSystemAgent()
        {
            return SystemAgent != 0;
        }

        public bool HasCPUCore()
        {
            return CPUCore != 0;
        }

        public bool HasIntelGPU()
        {
            return IntelGPU != 0;
        }

        public bool HasCPUCache()
        {
            return CPUCache != 0;
        }

        public bool HasPowerBalanceCPU()
        {
            return PowerBalanceCPU != 0;
        }

        public bool HasPowerBalanceGPU()
        {
            return PowerBalanceGPU != 0;
        }

        public void ComputeHex()
        {
            if (HasLongPowerMax())
                TurboBoostLongPowerMaxHex = TDPToHex(TurboBoostLongPowerMax);
            if (HasShortPowerMax())
                TurboBoostShortPowerMaxHex = TDPToHex(TurboBoostShortPowerMax);

            if (HasCPUCore())
                CPUCoreHex = VoltageToHex(CPUCore);
            if (HasIntelGPU())
                IntelGPUHex = VoltageToHex(IntelGPU);
            if (HasCPUCache())
                CPUCacheHex = VoltageToHex(CPUCache);
            if(HasSystemAgent())
                SystemAgentHex = VoltageToHex(SystemAgent);

            if (HasPowerBalanceCPU())
            {
                string hex = PowerBalanceCPU.ToString("X").GetLast(2);
                hex = hex.Length < 2 ? "0" + hex : hex;
                PowerBalanceCPUHex = hex;
            }
            if (HasPowerBalanceGPU())
            {
                string hex = PowerBalanceCPU.ToString("X").GetLast(2);
                hex = hex.Length < 2 ? "0" + hex : hex;
                PowerBalanceGPUHex = hex;
            }
        }

        public void DigestProfile(PowerProfile profile)
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

            ComputeHex();
        }

        public override string ToString()
        {
            return  "\n\t\t\t\t\t\t\t\tPL1:" + TurboBoostLongPowerMax + "W" +
                    "\t\t\t\tPL2:" + TurboBoostShortPowerMax + "W" +
                    "\n\t\t\t\t\t\t\t\tCPUCore:" + CPUCore + "mv" +
                    "\t\tCPUCache:" + CPUCache + "mv" +
                    "\n\t\t\t\t\t\t\t\tGPU:" + IntelGPU + "mv" +
                    "\t\t\tSystemAgent:" + SystemAgent + "mv" +
                    "\n\t\t\t\t\t\t\t\tPowerBalanceCPU:" + PowerBalanceCPU +
                    "\tPowerBalanceGPU:" + PowerBalanceGPU;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
