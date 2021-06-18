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
        public enum Manufacturer
        {
            Intel = 0,
            AMD = 1
        }

        public string Name, MCHBAR;
        public Manufacturer Constructor;
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

            switch(GetProcessorDetails("Manufacturer"))
            {
                case "GenuineIntel":
                    Constructor = Manufacturer.Intel;
                    break;
            }
        }

        public void Initialise()
        {
            if (Constructor == Manufacturer.Intel)
            {
                string command = "/Min /Nologo /Stdout /command=\"Delay 1000;rpci32 0 0 0 0x48;Delay 1000;rwexit\"";
                using (var ProcessOutput = Process.Start(new ProcessStartInfo(MainForm.path_rw, command)
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
                // do something
            }
        }

        public void SetPowerProfile(PowerProfile profile)
        {
            // skip check on empty profile
            if (profile.ProfileName == "")
                return;

            // skip update on similar profiles
            if (MainForm.CurrentProfile.Equals(profile))
                return;

            string command = null;
            string tool = null;

            if (Constructor == Manufacturer.Intel)
            {
                // skip if unsupported platform
                if (MCHBAR == null || !MCHBAR.Contains("0x"))
                    return;

                tool = MainForm.path_rw;
                command = "/Min /Nologo /Stdout /command=\"Delay 1000;";

                if (profile.HasLongPowerMax())
                {
                    command += $"w16 {MCHBAR}a0 0x8{profile.GetLongPowerMax().Substring(0, 1)}{profile.GetLongPowerMax().Substring(1)};";
                    command += $"wrmsr 0x610 0x0 0x00dd8{profile.GetLongPowerMax()};";
                }

                if (profile.HasShortPowerMax())
                {
                    command += $"w16 {MCHBAR}a4 0x8{profile.GetShortPowerMax().Substring(0, 1)}{profile.GetShortPowerMax().Substring(1)};";
                    command += $"wrmsr 0x610 0x0 0x00438{profile.GetShortPowerMax()};";
                }

                if (profile.HasCPUCore())
                    command += $"wrmsr 0x150 0x80000011 0x{profile.GetVoltageCPU()};";
                if (profile.HasIntelGPU())
                    command += $"wrmsr 0x150 0x80000111 0x{profile.GetVoltageGPU()};";
                if (profile.HasCPUCache())
                    command += $"wrmsr 0x150 0x80000211 0x{profile.GetVoltageCache()};";
                if (profile.HasSystemAgent())
                    command += $"wrmsr 0x150 0x80000411 0x{profile.GetVoltageSA()};";

                if (profile.HasPowerBalanceCPU())
                    command += $"wrmsr 0x642 0x00000000 0x000000{profile.GetPowerBalanceCPU()};";
                if (profile.HasPowerBalanceGPU())
                    command += $"wrmsr 0x63a 0x00000000 0x000000{profile.GetPowerBalanceGPU()};";

                command += "Delay 1000;rwexit\"";
            }
            else
            {
                tool = MainForm.path_ryz;
                command = "";

                if (profile.HasLongPowerMax())
                    command += $"--slow-limit={profile.TurboBoostLongPowerMax}000 --stapm-limit={profile.TurboBoostLongPowerMax}000 ";
                if (profile.HasShortPowerMax())
                    command += $"--fast-limit={profile.TurboBoostLongPowerMax}000 ";
            }

            // execute command
            if (command == null || tool == null)
                return;

            ProcessStartInfo PowerProcess = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = command,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = tool,
                Verb = "runas"
            };
            Process.Start(PowerProcess);

            // update current profile
            MainForm.CurrentProfile = profile;
            MainForm.SendNotification($"Power Profile [{profile.GetName()}] applied.", true, true);
        }
    }
}
