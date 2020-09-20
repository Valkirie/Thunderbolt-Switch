using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DockerForm
{
    class LogManager
    {
        private static CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        private static string filename = "DockerForm.log";

        public static void InitializeLog()
        {
            // Change current culture
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (File.Exists(filename))
                File.Delete(filename);
            else
                File.CreateText(filename).Close();
        }

        public static void UpdateLog(string input, bool IsError = false)
        {
            try
            {
                string type = IsError ? "ERR" : "LOG";
                string str = DateTime.Now.ToString("d", culture) + "\t" + type + "\t\t" + input + "\n";
                File.AppendAllText(filename, str);
            }
            catch(Exception)
            {
                Thread.Sleep(1000);
                UpdateLog(input, IsError);
            }
        }
    }
}
