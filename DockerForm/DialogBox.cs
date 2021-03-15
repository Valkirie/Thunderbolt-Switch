using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DockerForm
{
    public partial class DialogBox : Form
    {
        private string warningStr;
        private DateTime modifiedDB;
        private DateTime modifiedLOCAL;

        public DialogBox()
        {
            InitializeComponent();
        }

        public void UpdateDialogBox(string Title, string Name, DateTime LastCheck, FileInfo file)
        {
            warningStr = "The database settings data for " + Name + " [" + file.Name + "] does not match the data stored on this computer.";
            modifiedDB = LastCheck;
            modifiedLOCAL = file.LastWriteTime;

            label_WARNING.Text = warningStr;
            label_WARNING.Select(label_WARNING.Text.IndexOf(Name, 0), Name.Length);
            label_WARNING.SelectionFont = new Font(label_WARNING.Font, FontStyle.Bold);

            string DatabaseMonth = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetMonthName(modifiedDB.Month);
            string LocalMonth = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.GetMonthName(modifiedLOCAL.Month);

            this.label_LastModified_DB.Text = "MOST RECENT SAVE: " + DatabaseMonth + " " + modifiedDB.Day + ", " + modifiedDB.Year + " " + modifiedDB.Hour + ":" + modifiedDB.Minute;
            this.label_LastModified_LOCAL.Text = "MOST RECENT SAVE: " + LocalMonth + " " + modifiedLOCAL.Day + ", " + modifiedLOCAL.Year + " " + modifiedLOCAL.Hour + ":" + modifiedLOCAL.Minute;
        }
    }
}
