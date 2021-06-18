using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace DockerForm
{
    public class CPU
    {
        public string Name, Manufacturer, MCHBAR;
        private string GetProcessorDetails(string value)
        {
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
                return managObj.Properties[value].Value.ToString();

            return "";
        }

        public CPU()
        {
            Name = GetProcessorDetails("Name");
            Manufacturer = GetProcessorDetails("Manufacturer");
        }

        public void Initialise()
        {
            if (Manufacturer == "GenuineIntel")
            {
                string command = "/Min /Nologo /Stdout /command=\"Delay 1000;rpci32 0 0 0 0x48;Delay 1000;rwexit\"";
                using (var ProcessOutput = Process.Start(new ProcessStartInfo(Form1.path_rw, command)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                }))
                {
                    while (!ProcessOutput.StandardOutput.EndOfStream)
                    {
                        string line = ProcessOutput.StandardOutput.ReadLine();

                        if (!line.Contains("0x"))
                            continue;

                        MCHBAR = line.GetLast(10);
                        MCHBAR = MCHBAR.Substring(0, 6) + "59";
                        break;
                    }
                };
            }
            else
            {
                // AMD !?
            }
        }

        public void SetPowerProfile(PowerProfile profile)
        {
            // skip check on empty profile
            if (profile.ProfileName == "")
                return;

            // skip update on similar profiles
            if (Form1.CurrentProfile.Equals(profile))
                return;

            if (Manufacturer == "GenuineIntel")
            {
                // skip if unsupported platform
                if (MCHBAR == null || !MCHBAR.Contains("0x"))
                    return;

                string command = "/Min /Nologo /Stdout /command=\"Delay 1000;";

                if (profile.HasLongPowerMax())
                {
                    command += "w16 " + MCHBAR + "a0 0x8" + profile.GetLongPowerMax().Substring(0, 1) + profile.GetLongPowerMax().Substring(1) + ";";
                    command += "wrmsr 0x610 0x0 0x00dd8" + profile.GetLongPowerMax() + ";";
                }

                if (profile.HasShortPowerMax())
                {
                    command += "w16 " + MCHBAR + "a4 0x8" + profile.GetShortPowerMax().Substring(0, 1) + profile.GetShortPowerMax().Substring(1) + ";";
                    command += "wrmsr 0x610 0x0 0x00438" + profile.GetShortPowerMax() + ";";
                }

                if (profile.HasCPUCore())
                    command += "wrmsr 0x150 0x80000011 0x" + profile.GetVoltageCPU() + ";";
                if (profile.HasIntelGPU())
                    command += "wrmsr 0x150 0x80000111 0x" + profile.GetVoltageGPU() + ";";
                if (profile.HasCPUCache())
                    command += "wrmsr 0x150 0x80000211 0x" + profile.GetVoltageCache() + ";";
                if (profile.HasSystemAgent())
                    command += "wrmsr 0x150 0x80000411 0x" + profile.GetVoltageSA() + ";";

                if (profile.HasPowerBalanceCPU())
                    command += "wrmsr 0x642 0x00000000 0x000000" + profile.GetPowerBalanceCPU() + ";";
                if (profile.HasPowerBalanceGPU())
                    command += "wrmsr 0x63a 0x00000000 0x000000" + profile.GetPowerBalanceGPU() + ";";

                // command += "w " + MCHBAR + "94 0xFF;";
                command += "Delay 1000;rwexit\"";

                ProcessStartInfo RWInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Arguments = command,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = Form1.path_rw,
                    Verb = "runas"
                };
                Process.Start(RWInfo);
            }
            else
            {
                // AMD !?
            }

            // update current profile
            Form1.CurrentProfile = profile;
            Form1.SendNotification("Power Profile [" + profile.GetName() + "] applied.", true, true);
        }
    }
}
