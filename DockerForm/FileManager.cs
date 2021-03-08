using System.Drawing;
using System.IO;
using System.Net;

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

        public static Bitmap DownloadImage(string filename)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(filename);
            Bitmap bitmap; bitmap = new Bitmap(stream);
            stream.Flush();
            stream.Close();
            client.Dispose();
            return bitmap;
        }

        public static Bitmap GetImage(string filename)
        {
            Stream stream = File.OpenRead(filename);
            Bitmap bitmap; bitmap = new Bitmap(stream);
            stream.Flush();
            stream.Close();
            return bitmap;
        }
    }
}
