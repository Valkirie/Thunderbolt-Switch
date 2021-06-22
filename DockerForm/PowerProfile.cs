using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DockerForm
{
    [Flags]
    public enum ProfileMask
    {
        None = 0,
        OnBattery = 1,
        PluggedIn = 2,
        ExternalGPU = 4,
        OnStartup = 8,
        ExternalScreen = 16,
        OnStatusChange = 32,
        GameBounds = 64,
        All = OnBattery | PluggedIn | ExternalGPU | OnStartup | ExternalScreen | OnStatusChange | GameBounds
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
        [XmlIgnore]
        public ProfileMask _ApplyMask = ProfileMask.None;
        public int ApplyMask
        {
            get { return (int)_ApplyMask; }
            set { _ApplyMask = (ProfileMask)value; }
        }
        public int ApplyPriority = 0;

        [XmlIgnore] public bool RunMe;
        [XmlIgnore] public bool JustCreated;
        [XmlIgnore] public bool JustUpdated;
        [XmlIgnore] public int Merged = 0;
        public Guid ProfileGuid;
        public string ProfileVersion;

        public void Serialize(bool UpdateProfiles = true)
        {
            // update values
            ComputeHex();

            string filename = Path.Combine(MainForm.path_profiles, ProfileGuid.ToString()) + ".xml";
            using (FileStream writer = new FileStream(filename, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }

            if(UpdateProfiles)
                MainForm.UpdateProfiles();
        }

        public void Remove()
        {
            string filename = Path.Combine(MainForm.path_profiles, ProfileGuid.ToString()) + ".xml";
            if (File.Exists(filename))
                File.Delete(filename);

            MainForm.UpdateProfiles();
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
            return TurboBoostLongPowerMaxHex.GetLast(3);
        }

        public string GetShortPowerMax()
        {
            return TurboBoostShortPowerMaxHex.GetLast(3);
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

        public PowerProfile(string profileName)
        {
            ProfileName = profileName;
            ProfileGuid = Guid.NewGuid();
            JustCreated = true;
        }

        public PowerProfile()
        {
        }

        public bool HasLongPowerMax()
        {
            return (TurboBoostLongPowerMax != null && TurboBoostLongPowerMax != "0");
        }

        public bool HasShortPowerMax()
        {
            return (TurboBoostShortPowerMax != null && TurboBoostShortPowerMax != "0");
        }

        public bool HasSystemAgent()
        {
            return (SystemAgent != null && SystemAgent != "0");
        }

        public bool HasCPUCore()
        {
            return (CPUCore != null && CPUCore != "0");
        }

        public bool HasIntelGPU()
        {
            return (IntelGPU != null && IntelGPU != "0");
        }

        public bool HasCPUCache()
        {
            return (CPUCache != null && CPUCache != "0");
        }

        public bool HasPowerBalanceCPU()
        {
            return (PowerBalanceCPU != null);
        }

        public bool HasPowerBalanceGPU()
        {
            return (PowerBalanceGPU != null);
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
            if (HasSystemAgent())
                SystemAgentHex = VoltageToHex(int.Parse(SystemAgent));

            if (HasPowerBalanceCPU())
            {
                string hex = int.Parse(PowerBalanceCPU).ToString("X").GetLast(2);
                hex = hex.Length < 2 ? "0" + hex : hex;
                PowerBalanceCPUHex = hex;
            }
            if (HasPowerBalanceGPU())
            {
                string hex = int.Parse(PowerBalanceGPU).ToString("X").GetLast(2);
                hex = hex.Length < 2 ? "0" + hex : hex;
                PowerBalanceGPUHex = hex;
            }
        }

        public void DigestProfile(PowerProfile profile, bool isMerged)
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

            if (isMerged)
            {
                if (profile.ApplyPriority != -1)
                {
                    ProfileName += (ProfileName.Equals("") ? "" : " & ") + profile.ProfileName;
                    Merged++;
                }
            }
            else
            {
                ProfileName = profile.ProfileName;
                ApplyMask = profile.ApplyMask;
                ApplyPriority = profile.ApplyPriority;
            }

            ComputeHex();
        }

        public bool Equals(PowerProfile compare)
        {
            if (TurboBoostLongPowerMax != compare.TurboBoostLongPowerMax ||
                TurboBoostShortPowerMax != compare.TurboBoostShortPowerMax ||
                CPUCore != compare.CPUCore || CPUCache != compare.CPUCache ||
                IntelGPU != compare.IntelGPU || SystemAgent != compare.SystemAgent ||
                PowerBalanceCPU != compare.PowerBalanceCPU || PowerBalanceGPU != compare.PowerBalanceGPU)
                return false;

            return true;
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
            // do something
        }
    }
}
