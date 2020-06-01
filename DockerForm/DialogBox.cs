using System.Windows.Forms;

namespace DockerForm
{
    public partial class DialogBox : Form
    {
        public DialogBox()
        {
            InitializeComponent();
        }

        public void UpdateDialogBox(string Title, string Warning, string LastModified_DB, string LastModified_LOCAL)
        {
            this.Text = Title;
            this.label_WARNING.Text = Warning;
            this.label_LastModified_DB.Text = LastModified_DB;
            this.label_LastModified_LOCAL.Text = LastModified_LOCAL;
        }
    }
}
