using Be.Windows.Forms;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DockerForm
{
    public partial class Settings : Form
    {
        // Form vars
        static Form1 thisForm;
        Settings thisSetting;
        bool IsReady;

        // Game vars
        DockerGame thisGame;

        public bool GetIsReady()
        {
            return IsReady;
        }

        public DockerGame GetGame()
        {
            return thisGame;
        }

        public void SetStartPos()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Left = thisForm.Location.X + (thisForm.Size.Width - this.Size.Width) / 2;
            this.Top = thisForm.Location.Y + (thisForm.Size.Height - this.Size.Height) / 2;
        }

        private void InitializeForm()
        {
            tabSettingsDesc.HandleCreated += new System.EventHandler(TabControl_HandleCreated);
        }

        void TabControl_HandleCreated(object sender, System.EventArgs e)
        {
            // Send TCM_SETMINTABWIDTH
            SendMessage((sender as TabControl).Handle, 0x1300 + 49, IntPtr.Zero, (IntPtr)4);
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public Settings(Form1 form)
        {
            InitializeComponent();
            InitializeForm();

            // instances
            thisForm = form;
            thisSetting = this;
            SetStartPos();

            IsReady = PickAGame();
        }

        public Settings(Form1 form, DockerGame game)
        {
            InitializeComponent();
            InitializeForm();

            // instances
            thisGame = new DockerGame(game);
            thisForm = form;
            thisSetting = this;
            SetStartPos();

            field_Name.Text = thisGame.Name;
            field_GUID.Text = thisGame.GUID;
            field_Filename.Text = thisGame.Executable;
            field_Version.Text = thisGame.Version;
            field_Developer.Text = thisGame.Company;
            field_Arguments.Text = thisGame.Arguments;

            GameIcon.BackgroundImage = thisGame.Image;

            // Settings tab
            foreach (GameSettings setting in thisGame.Settings.Values)
            {
                string FileName = System.IO.Path.GetFileName(setting.Uri);

                if (setting.Type == SettingsType.Registry)
                    FileName = setting.Uri;

                ListViewItem newSetting = new ListViewItem(new string[] { FileName, setting.Uri, Enum.GetName(typeof(SettingsType), setting.Type) }, setting.FileName);
                newSetting.Checked = setting.IsEnabled;
                newSetting.Tag = setting.IsRelative;
                SettingsList.Items.Add(newSetting);
            }

            // General tab
            checkBoxPowerSpecific.Checked = thisGame.PowerSpecific;

            // Power Profiles tab
            groupBoxPowerProfile.Enabled = Form1.MonitorProcesses;

            foreach (PowerProfile profile in Form1.ProfileDB.Values)
            {
                bool isOnBattery = (profile.ApplyMask & (byte)ProfileMask.OnBattery) == (byte)ProfileMask.OnBattery;
                bool isPluggedIn = (profile.ApplyMask & (byte)ProfileMask.PluggedIn) == (byte)ProfileMask.PluggedIn;
                bool isExtGPU = (profile.ApplyMask & (byte)ProfileMask.ExternalGPU) == (byte)ProfileMask.ExternalGPU;
                bool isOnScreen = (profile.ApplyMask & (byte)ProfileMask.ExternalScreen) == (byte)ProfileMask.ExternalScreen;
                ListViewItem newProfile = new ListViewItem(new string[] { profile.ProfileName, isOnBattery.ToString(), isPluggedIn.ToString(), isExtGPU.ToString(), isOnScreen.ToString() }, profile.ProfileName);

                // skip default
                if (profile.ApplyPriority == -1)
                    continue;

                if (thisGame.Profiles.ContainsKey(profile.ProfileName))
                    newProfile.Checked = true;

                ProfilesList.Items.Add(newProfile);
            }

            IsReady = true;
        }

        private void SettingsList_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right: break;
            }
        }

        private bool PickAGame()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(thisGame != null ? thisGame.Uri : "");
                openFileDialog.Filter = "exe files (*.exe)|*.exe";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    DockerGame newGame = new DockerGame(filePath);

                    // are we picking a new executable ?
                    if (thisGame != null)
                    {
                        newGame.Settings = thisGame.Settings;
                        newGame.Profiles = thisGame.Profiles;
                    }

                    field_Name.Text = newGame.ProductName;
                    field_Version.Text = newGame.Version;
                    field_Filename.Text = newGame.Executable;
                    field_Developer.Text = newGame.Company;
                    field_GUID.Text = newGame.GUID;
                    GameIcon.BackgroundImage = newGame.Image;

                    thisGame = new DockerGame(newGame);

                    return true;
                }
            }
            return false;
        }

        private void buttonBrowseFile_Click(object sender, EventArgs e)
        {
            PickAGame();
        }

        private void field_Developer_TextChanged(object sender, EventArgs e)
        {
            if (thisGame == null)
                return;

            thisGame.Company = field_Developer.Text;
        }

        private void field_Name_TextChanged(object sender, EventArgs e)
        {
            if (thisGame == null)
                return;

            thisGame.Name = field_Name.Text;
        }

        private void field_arguments_TextChanged(object sender, EventArgs e)
        {
            if (thisGame == null)
                return;

            thisGame.Arguments = field_Arguments.Text;
        }

        private void MenuItemRemoveSetting_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.SubItems[0].Text;

                SettingsList.Items.Remove(item);
                if (thisGame.Settings.ContainsKey(FileName))
                    thisGame.Settings.Remove(FileName);
            }
        }

        public static string GetRelativePath(string fullPath, string basePath)
        {
            // Require trailing backslash for path
            if (!basePath.EndsWith("\\"))
                basePath += "\\";

            Uri baseUri = new Uri(basePath);
            Uri fullUri = new Uri(fullPath);

            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            // Uri's use forward slashes so convert back to backward slashes
            return relativeUri.ToString().Replace("/", "\\");
        }

        public static string ContractEnvironmentVariables(string path, ref bool IsRelative, DockerGame thisGame)
        {
            string filename = path.ToLower();

            Dictionary<string, string> sortedDict = new Dictionary<string, string>();

            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                string key = (string)de.Key.ToString().ToLower();
                string value = (string)de.Value.ToString().ToLower();
                sortedDict.Add("%" + key + "%" + @"\", value + @"\");
            }

            foreach (KeyValuePair<string, string> item in sortedDict.OrderByDescending(key => key.Value))
            {
                if (filename.Contains(item.Value))
                {
                    filename = filename.Replace(item.Value, item.Key);
                    break;
                }
            }

            if (!filename.Contains("%"))
            {
                string pathrelative = GetRelativePath(filename, thisGame.Uri);
                if (!path.Equals(pathrelative))
                {
                    filename = pathrelative;
                    IsRelative = true;
                }
            }

            return filename;
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(thisGame.Uri);
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in openFileDialog.FileNames)
                    {
                        bool IsRelative = false;
                        string FilePath = ContractEnvironmentVariables(file, ref IsRelative, thisGame);
                        string FileName = System.IO.Path.GetFileName(FilePath);

                        ListViewItem listViewItem1 = new ListViewItem(new string[] { FileName, FilePath, "File" }, -1);
                        listViewItem1.Checked = true;
                        listViewItem1.Tag = IsRelative;
                        SettingsList.Items.Add(listViewItem1);

                        byte[] s_file = System.IO.File.ReadAllBytes(file);
                        GameSettings newSetting = new GameSettings(FileName, SettingsType.File, FilePath, true, IsRelative);
                        newSetting.data[Form1.GetCurrentState(thisGame)] = s_file;
                        thisGame.Settings[FileName] = newSetting;
                    }
                }
            }
        }

        private void registryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FileName = Interaction.InputBox("", thisGame.Name + " - Registry Key", @"HKEY_CURRENT_USER\SOFTWARE\");

            if (FileName == "")
                return;

            ListViewItem listViewItem1 = new ListViewItem(new string[] { FileName, FileName, "Registry" }, -1);
            listViewItem1.Tag = false;

            GameSettings newSetting = new GameSettings(FileName, SettingsType.Registry, FileName, false, false);

            string FileTemp = System.IO.Path.Combine(Form1.path_application, "temp.reg");
            RegistryManager.ExportKey(FileName, FileTemp);

            if (System.IO.File.Exists(FileTemp))
            {
                byte[] s_file = System.IO.File.ReadAllBytes(FileTemp);
                newSetting.data[Form1.GetCurrentState(thisGame)] = s_file;
                newSetting.IsEnabled = true;
                listViewItem1.Checked = true;
            }

            thisGame.Settings[FileName] = newSetting;
            SettingsList.Items.Add(listViewItem1);
        }

        public static string Between(ref string src, string start, string ended, bool del = false)
        {
            string ret = "";
            int idxStart = src.IndexOf(start);
            if (idxStart != -1)
            {
                idxStart = idxStart + start.Length;
                int idxEnd = src.IndexOf(ended, idxStart);
                if (idxEnd != -1)
                {
                    ret = src.Substring(idxStart, idxEnd - idxStart);
                    if (del == true)
                        src = src.Replace(start + ret + ended, "");
                }
            }
            return ret;
        }

        private void SuspendWindow(bool status)
        {
            thisSetting.Enabled = !status;
            Application.DoEvents();
        }

        private void downloadFromIGDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IGDBList.DropDownItems.Count != 0)
                return;

            SuspendWindow(true);

            try
            {
                WebClient client = new WebClient();
                string HTML = client.DownloadString("https://www.igdb.com/search?type=1&q=" + thisGame.Name);
                string gamelist = Between(ref HTML, "mar-md-bottom text-muted", "hidden-sm hidden-xs");

                while (Between(ref gamelist, "id", "release_year") != "" && IGDBList.DropDownItems.Count < thisForm.GetIGDBListLength())
                {
                    string gamedetails = "id&quot" + Between(ref gamelist, "id&quot", "release_year", true);
                    string gamename = System.Web.HttpUtility.HtmlDecode(Between(ref gamedetails, "name&quot;:&quot;", "&quot;"));
                    string gameurl = Between(ref gamedetails, "url&quot;:&quot;", "&quot;");
                    string gameid = Between(ref gamedetails, "id&quot;:", ",&quot;");

                    Dictionary<string, string> IGDB = new Dictionary<string, string>();
                    IGDB.Add("name", gamename);
                    IGDB.Add("url", gameurl);
                    IGDB.Add("id", gameid);

                    ToolStripMenuItem item = new ToolStripMenuItem();
                    item.Text = gamename;
                    item.Tag = IGDB;
                    item.Click += new EventHandler(MenuItemClickHandler);
                    IGDBList.DropDownItems.Add(item);
                }
            }
            catch (Exception) { }

            if (IGDBList.DropDownItems.Count == 0)
                MessageBox.Show("No result from IGDB. Please try another game name.");
            else
            {
                ImageMenuStrip.Show();
                IGDBList.ShowDropDown();
            }

            SuspendWindow(false);
        }

        private void MenuItemClickHandler(object sender, EventArgs e)
        {
            SuspendWindow(true);

            try
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;

                // gather datas
                Dictionary<string, string> IGDB = (Dictionary<string, string>)item.Tag;
                string hidden_url = IGDB["url"];

                WebClient client = new WebClient();
                string igdb_name = Regex.Replace(thisGame.Name, "[^a-zA-Z0-9]", "-").ToLower();
                string HTML = client.DownloadString("https://www.igdb.com/" + hidden_url);
                string ImageUri = Between(ref HTML, "t_cover_big/", ".jpg");
                string gamecompany = Between(ref HTML, "developers&quot;:[[&quot;", "&quot;");

                // update game details
                if (field_Developer.Text != gamecompany)
                {
                    DialogResult dialogResult = MessageBox.Show("Would you like to overwrite Game Company ?\n" + field_Developer.Text + "=>" + gamecompany, "IGDB", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        field_Developer.Text = gamecompany;
                }

                if (field_Name.Text != IGDB["name"])
                {
                    DialogResult dialogResult = MessageBox.Show("Would you like to overwrite Game Name ?\n" + field_Name.Text + "=>" + IGDB["name"], "IGDB", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        field_Name.Text = IGDB["name"];
                }

                thisGame.IGDB_Url = "https://www.igdb.com/" + hidden_url;

                if (ImageUri != "")
                {
                    string filename = "https://images.igdb.com/igdb/Image/upload/t_cover_big/" + ImageUri + ".jpg";
                    Bitmap BackgroundImage = FileManager.DownloadImage(filename);

                    thisGame.Image = BackgroundImage;
                    GameIcon.BackgroundImage = thisGame.Image;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "MenuItemClickHandler"); }

            SuspendWindow(false);
        }

        private void GameIcon_Click(object sender, EventArgs e)
        {
            if (GameIcon.BackgroundImage.Size.Width < 100)
                GameIcon.BackgroundImageLayout = ImageLayout.Center;
            else
                GameIcon.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void searchOnPCGamingWikiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.pcgamingwiki.com/wiki/" + thisGame.Name.Replace(" ", "_") + "#Game_data");
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!thisGame.CanSerialize())
                return;

            // Settings tab
            foreach (ListViewItem item in SettingsList.Items)
            {
                string FileName = item.SubItems[0].Text;
                if (thisGame.Settings.ContainsKey(FileName))
                    thisGame.Settings[FileName].IsEnabled = item.Checked;
            }

            // Power Profiles tab
            thisGame.Profiles.Clear();
            foreach (ListViewItem item in ProfilesList.Items)
            {
                PowerProfile profile = Form1.ProfileDB[item.Text];
                if (item.Checked)
                    thisGame.Profiles.Add(profile.ProfileName, profile);
            }

            thisGame.SanityCheck();
            thisForm.InsertOrUpdateGameItem(thisGame, false);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GetLanguage(string FileName)
        {
            string FileExtension = System.IO.Path.GetExtension(FileName);
            switch (FileExtension)
            {
                case ".ini":
                case ".txt":
                case ".xml":
                    return FileExtension;
                default:
                    return null;
            }
        }

        private void SettingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MenuItemRemoveSetting.Enabled = false;
            tabSettingsDesc.TabPages.Clear();

            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.SubItems[0].Text;

                if (thisGame.Settings.ContainsKey(FileName))
                {
                    MenuItemRemoveSetting.Enabled = true;

                    foreach (KeyValuePair<string, byte[]> data in thisGame.Settings[FileName].data)
                    {
                        TabPage myPage = new TabPage();
                        myPage.Text = data.Key;
                        myPage.Name = data.Key;

                        string myLanguage = GetLanguage(FileName);
                        if (myLanguage != null)
                        {
                            string myString = System.Text.Encoding.UTF8.GetString(data.Value);

                            RichTextBox myViewer = new RichTextBox()
                            {
                                Name = data.Key,
                                Text = myString,
                                Dock = DockStyle.Fill
                            };
                            myViewer.TextChanged += MyViewer_TextChanged;
                            myPage.Controls.Add(myViewer);
                        }
                        else
                        {
                            HexBox myViewer = new HexBox()
                            {
                                ByteProvider = new DynamicByteProvider(data.Value),
                                Dock = DockStyle.Fill,
                                Visible = true,
                                UseFixedBytesPerLine = true,
                                BytesPerLine = 12,
                                ColumnInfoVisible = true,
                                LineInfoVisible = true,
                                StringViewVisible = true,
                                VScrollBarVisible = true
                            };
                            myViewer.ByteProvider.Changed += ByteProvider_Changed;
                            myPage.Controls.Add(myViewer);
                        }

                        tabSettingsDesc.TabPages.Add(myPage);
                    }
                }
                break;
            }
        }

        private void ByteProvider_Changed(object sender, EventArgs e)
        {
            DynamicByteProvider myViewer = (DynamicByteProvider)sender;
            myViewer.ApplyChanges();

            TabPage myPage = tabSettingsDesc.TabPages[tabSettingsDesc.SelectedIndex];

            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.SubItems[0].Text;
                thisGame.Settings[FileName].data[myPage.Name] = myViewer.Bytes.ToArray();

                break;
            }
        }

        private void MyViewer_TextChanged(object sender, EventArgs e)
        {
            RichTextBox myViewer = (RichTextBox)sender;

            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.SubItems[0].Text;
                thisGame.Settings[FileName].data[myViewer.Name] = Encoding.ASCII.GetBytes(myViewer.Text);

                break;
            }
        }

        private void checkBoxPowerSpecific_CheckedChanged(object sender, EventArgs e)
        {
            thisGame.PowerSpecific = checkBoxPowerSpecific.Checked;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(checkBoxPowerSpecific, "Use specific settings when device power status changes (on battery, plugged in)");
        }

        private void ProfilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in groupBoxFIVR.Controls)
                if (ctrl.GetType() == typeof(TextBox))
                    ctrl.Text = "";

            foreach (Control ctrl in groupBoxPowerProfile.Controls)
                if (ctrl.GetType() == typeof(TextBox))
                    ctrl.Text = "";

            foreach (ListViewItem item in ProfilesList.SelectedItems)
            {
                string ProfileName = item.SubItems[0].Text;
                PowerProfile profile = Form1.ProfileDB[ProfileName];

                // Misc
                if (profile.HasLongPowerMax())
                    textBox1.Text = profile.TurboBoostLongPowerMax + "W";
                if (profile.HasShortPowerMax())
                    textBox2.Text = profile.TurboBoostShortPowerMax + "W";
                if (profile.HasPowerBalanceCPU())
                    textBox3.Text = profile.PowerBalanceCPU.ToString();
                if (profile.HasPowerBalanceGPU())
                    textBox4.Text = profile.PowerBalanceGPU.ToString();

                // FIVR
                if (profile.HasCPUCore())
                    textBox5.Text = profile.CPUCore + "mV";
                if (profile.HasCPUCache())
                    textBox6.Text = profile.CPUCache + "mV";
                if (profile.HasSystemAgent())
                    textBox7.Text = profile.SystemAgent + "mV";
                if (profile.HasIntelGPU())
                    textBox8.Text = profile.IntelGPU + "mV";
            }
        }
    }
}
