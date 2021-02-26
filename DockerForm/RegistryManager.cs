using System.Diagnostics;

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
    }
}
