using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DockerForm
{
    public class ShellEx
    {
        private const int SHGFI_SMALLICON = 0x1;
        private const int SHGFI_LARGEICON = 0x0;
        private const int SHIL_JUMBO = 0x4;
        private const int SHIL_EXTRALARGE = 0x2;
        private const int WM_CLOSE = 0x0010;

        public enum IconSizeEnum
        {
            SmallIcon16 = SHGFI_SMALLICON,
            MediumIcon32 = SHGFI_LARGEICON,
            LargeIcon48 = SHIL_EXTRALARGE,
            ExtraLargeIcon = SHIL_JUMBO
        }

        [DllImport("user32")]
        private static extern
            IntPtr SendMessage(
            IntPtr handle,
            int Msg,
            IntPtr wParam,
            IntPtr lParam);


        [DllImport("shell32.dll")]
        private static extern int SHGetImageList(
            int iImageList,
            ref Guid riid,
            out Shell.IImageList ppv);

        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(
            string pszPath,
            int dwFileAttributes,
            ref Shell.SHFILEINFO psfi,
            int cbFileInfo,
            uint uFlags);

        [DllImport("user32")]
        public static extern int DestroyIcon(
            IntPtr hIcon);

        public static System.Drawing.Bitmap GetBitmapFromFolderPath(
            string filepath, IconSizeEnum iconsize)
        {
            IntPtr hIcon = GetIconHandleFromFolderPath(filepath, iconsize);
            return getBitmapFromIconHandle(hIcon);
        }

        public static System.Drawing.Bitmap GetBitmapFromFilePath(
            string filepath, IconSizeEnum iconsize)
        {
            IntPtr hIcon = GetIconHandleFromFilePath(filepath, iconsize);
            return getBitmapFromIconHandle(hIcon);
        }

        public static System.Drawing.Bitmap GetBitmapFromPath(
            string filepath, IconSizeEnum iconsize)
        {
            IntPtr hIcon = IntPtr.Zero;
            if (Directory.Exists(filepath))
            {
                hIcon = GetIconHandleFromFolderPath(filepath, iconsize);
            }
            else
            {
                if (File.Exists(filepath))
                {
                    hIcon = GetIconHandleFromFilePath(filepath, iconsize);
                }
            }
            return getBitmapFromIconHandle(hIcon);
        }

        private static System.Drawing.Bitmap getBitmapFromIconHandle(IntPtr hIcon)
        {
            if (hIcon == IntPtr.Zero) throw new System.IO.FileNotFoundException();
            var myIcon = System.Drawing.Icon.FromHandle(hIcon);
            var bitmap = myIcon.ToBitmap();
            myIcon.Dispose();
            DestroyIcon(hIcon);
            SendMessage(hIcon, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            return bitmap;
        }

        private static IntPtr GetIconHandleFromFilePath(string filepath, IconSizeEnum iconsize)
        {
            var shinfo = new Shell.SHFILEINFO();
            const uint SHGFI_SYSICONINDEX = 0x4000;
            const int FILE_ATTRIBUTE_NORMAL = 0x80;
            uint flags = SHGFI_SYSICONINDEX;
            return getIconHandleFromFilePathWithFlags(filepath, iconsize, ref shinfo, FILE_ATTRIBUTE_NORMAL, flags);
        }

        private static IntPtr GetIconHandleFromFolderPath(string folderpath, IconSizeEnum iconsize)
        {
            var shinfo = new Shell.SHFILEINFO();

            const uint SHGFI_ICON = 0x000000100;
            const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            const int FILE_ATTRIBUTE_DIRECTORY = 0x00000010;
            uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
            return getIconHandleFromFilePathWithFlags(folderpath, iconsize, ref shinfo, FILE_ATTRIBUTE_DIRECTORY, flags);
        }

        private static IntPtr getIconHandleFromFilePathWithFlags(
            string filepath, IconSizeEnum iconsize,
            ref Shell.SHFILEINFO shinfo, int fileAttributeFlag, uint flags)
        {
            const int ILD_TRANSPARENT = 1;
            var retval = SHGetFileInfo(filepath, fileAttributeFlag, ref shinfo, Marshal.SizeOf(shinfo), flags);
            if (retval == 0) throw (new System.IO.FileNotFoundException());
            var iconIndex = shinfo.iIcon;
            var iImageListGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            Shell.IImageList iml;
            var hres = SHGetImageList((int)iconsize, ref iImageListGuid, out iml);
            var hIcon = IntPtr.Zero;
            hres = iml.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
            return hIcon;
        }

    }
}
