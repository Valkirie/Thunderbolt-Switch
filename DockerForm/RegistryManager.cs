using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DockerForm
{
    class RegistryManager
    {
        public static void RestoreKey(string targetFile)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "regedit.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc = Process.Start("regedit.exe", "/s " + targetFile);
            proc.WaitForExit();
        }

        public static void ExportKey(string RegKey, string path)
        {
            string key = "\"" + RegKey + "\"";

            Process proc = new Process();
            proc.StartInfo.FileName = "regedit.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc = Process.Start("regedit.exe", "/e " + path + " " + key);
            proc.WaitForExit();
        }

        public static void StartupManager(bool add)
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if(add)
                    key.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
                else
                    key.DeleteValue(Application.ProductName, false);
            }
        }
    }
}
