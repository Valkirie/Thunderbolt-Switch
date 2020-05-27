using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerForm
{
    class FileManager
    {
        public static void CopyFolder(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyFile(FileInfo sourceFile, string targetDirectory)
        {
            DirectoryInfo target = new DirectoryInfo(targetDirectory);

            if (!Directory.Exists(target.FullName))
                Directory.CreateDirectory(target.FullName);

            if (File.Exists(sourceFile.FullName))
                sourceFile.CopyTo(Path.Combine(target.FullName, sourceFile.Name), true);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            if (!Directory.Exists(target.FullName))
                Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }
    }
}
