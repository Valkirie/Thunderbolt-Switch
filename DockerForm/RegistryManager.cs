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
            if (File.Exists(targetFile))
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "regedit.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc = Process.Start("regedit.exe", "/s " + targetFile);
                proc.WaitForExit();
            }
        }

        public static void ExportKey(string RegKey, string SavePath, string targetFile)
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            string path = "\"" + SavePath + "\\" + targetFile + "\"";
            string key = "\"" + RegKey + "\"";

            Process proc = new Process();
            proc.StartInfo.FileName = "regedit.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc = Process.Start("regedit.exe", "/e " + path + " " + key);
            proc.WaitForExit();
        }

        public static string GetRegistryFile(string keypath)
        {
            string[] temp = keypath.Split('\\');
            string registry = "";
            foreach (string f in temp)
                registry += f[0];
            registry += ".reg";

            return registry;
        }

        public static void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                key.SetValue(Application.ProductName, "\"" + Application.ExecutablePath + "\"");
        }

        public static void RemoveApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                key.DeleteValue(Application.ProductName, false);
        }
    }
}
