using Be.Windows.Forms;
using DockerForm.Properties;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace DockerForm
{
    public partial class GameProperties : Form
    {
        // Form vars
        static MainForm gForm;
        GameProperties gProperties;
        bool gIsReady;

        // Game vars
        DockerGame gGame;
        
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public bool GetIsReady()
        {
            return gIsReady;
        }

        public DockerGame GetGame()
        {
            return gGame;
        }

        public void SetStartPos()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Left = gForm.Location.X + (gForm.Size.Width - this.Size.Width) / 2;
            this.Top = gForm.Location.Y + (gForm.Size.Height - this.Size.Height) / 2;
        }

        private void InitializeForm()
        {
            tabSettingsDesc.HandleCreated += new System.EventHandler(TabControl_HandleCreated);
            tabSettingsDesc.MouseDoubleClick += tabSettingsDesc_DoubleClick;
        }

        void TabControl_HandleCreated(object sender, System.EventArgs e)
        {
            SendMessage((sender as TabControl).Handle, 0x1300 + 49, IntPtr.Zero, (IntPtr)4);
        }
        
        public GameProperties(MainForm form)
        {
            InitializeComponent();
            InitializeForm();

            // instances
            gForm = form;
            gProperties = this;
            SetStartPos();

            gIsReady = PickAGame();
        }

        public GameProperties(MainForm form, DockerGame game)
        {
            InitializeComponent();
            InitializeForm();

            // instances
            gGame = new DockerGame(game);
            gForm = form;
            gProperties = this;
            SetStartPos();

            field_Name.Text = gGame.Name;
            field_GUID.Text = gGame.GUID;
            field_Filename.Text = gGame.Executable;
            field_Version.Text = gGame.Version;
            field_Developer.Text = gGame.Company;
            field_Arguments.Text = gGame.Arguments;

            GameIcon.BackgroundImage = gGame.Image;

            // Settings tab
            foreach (GameSettings setting in gGame.Settings.Values)
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
            checkBoxPowerSpecific.Checked = gGame.PowerSpecific;
            checkBoxPowerProfileSpecific.Checked = gGame.PowerProfileSpecific;

            // Power Profiles tab
            groupBoxPowerProfile.Enabled = MainForm.MonitorProcesses;

            foreach (PowerProfile profile in MainForm.ProfileDB.Values)
            {
				bool isOnBattery = profile._ApplyMask.HasFlag(ProfileMask.OnBattery);
				bool isPluggedIn = profile._ApplyMask.HasFlag(ProfileMask.PluggedIn);
				bool isExtGPU = profile._ApplyMask.HasFlag(ProfileMask.ExternalGPU);
				bool isOnBoot = profile._ApplyMask.HasFlag(ProfileMask.OnStartup);
				bool isOnStatusChange = profile._ApplyMask.HasFlag(ProfileMask.OnStatusChange);
				bool isOnScreen = profile._ApplyMask.HasFlag(ProfileMask.ExternalScreen);
                bool isGameBounds = profile._ApplyMask.HasFlag(ProfileMask.GameBounds);

                ListViewItem newProfile = new ListViewItem(new string[] { profile.ProfileName, isOnBattery.ToString(), isPluggedIn.ToString(), isExtGPU.ToString(), isOnScreen.ToString() }, profile.ProfileName);
                newProfile.Tag = profile.ProfileGuid;

                // skip default
                if (profile.ApplyPriority == -1)
                    continue;

                if (!isGameBounds)
                    continue;

                if (gGame.PowerProfiles.ContainsKey(profile.ProfileGuid))
                    newProfile.Checked = true;

                ProfilesList.Items.Add(newProfile);
            }

            gIsReady = true;
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
                openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(gGame != null ? gGame.Uri : "");
                openFileDialog.Filter = "exe files (*.exe)|*.exe";
                openFileDialog.FilterIndex = 2;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    DockerGame newGame = new DockerGame(filePath);

                    // are we picking a new executable ?
                    if (gGame != null)
                    {
                        newGame.Settings = gGame.Settings;
                        newGame.PowerProfiles = gGame.PowerProfiles;
                    }

                    field_Name.Text = newGame.ProductName;
                    field_Version.Text = newGame.Version;
                    field_Filename.Text = newGame.Executable;
                    field_Developer.Text = newGame.Company;
                    field_GUID.Text = newGame.GUID;
                    GameIcon.BackgroundImage = newGame.Image;

                    gGame = new DockerGame(newGame);

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
            if (gGame == null)
                return;

            gGame.Company = field_Developer.Text;
        }

        private void field_Name_TextChanged(object sender, EventArgs e)
        {
            if (gGame == null)
                return;

            gGame.Name = field_Name.Text;
        }

        private void field_arguments_TextChanged(object sender, EventArgs e)
        {
            if (gGame == null)
                return;

            gGame.Arguments = field_Arguments.Text;
        }

        private void MenuItemRemoveSetting_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.Text;

                SettingsList.Items.Remove(item);
                if (gGame.Settings.ContainsKey(FileName))
                    gGame.Settings.Remove(FileName);
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
                openFileDialog.InitialDirectory = Environment.ExpandEnvironmentVariables(gGame.Uri);
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (String file in openFileDialog.FileNames)
                    {
                        bool IsRelative = false;
                        string FilePath = ContractEnvironmentVariables(file, ref IsRelative, gGame);
                        string FileName = System.IO.Path.GetFileName(FilePath);

                        ListViewItem listViewItem1 = new ListViewItem(new string[] { FileName, FilePath, "File" }, -1);
                        listViewItem1.Checked = true;
                        listViewItem1.Tag = IsRelative;
                        SettingsList.Items.Add(listViewItem1);

                        byte[] s_file = System.IO.File.ReadAllBytes(file);
                        GameSettings newSetting = new GameSettings(FileName, SettingsType.File, FilePath, true, IsRelative);
                        newSetting.data[MainForm.GetCurrentState(gGame)] = s_file;
                        //newSetting.removeunused[MainForm.GetCurrentState(gGame)] = false;
                        gGame.Settings[FileName] = newSetting;
                    }
                }
            }
        }

        private void registryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FileName = Interaction.InputBox("", gGame.Name + " - Registry Key", @"HKEY_CURRENT_USER\SOFTWARE\");

            if (FileName == "")
                return;

            ListViewItem listViewItem1 = new ListViewItem(new string[] { FileName, FileName, "Registry" }, -1);
            listViewItem1.Tag = false;

            GameSettings newSetting = new GameSettings(FileName, SettingsType.Registry, FileName, false, false);

            string FileTemp = System.IO.Path.Combine(MainForm.path_application, "temp.reg");
            RegistryManager.ExportKey(FileName, FileTemp);

            if (System.IO.File.Exists(FileTemp))
            {
                byte[] s_file = System.IO.File.ReadAllBytes(FileTemp);
                newSetting.data[MainForm.GetCurrentState(gGame)] = s_file;
                //newSetting.removeunused[MainForm.GetCurrentState(gGame)] = false;
                newSetting.IsEnabled = true;
                listViewItem1.Checked = true;
            }

            gGame.Settings[FileName] = newSetting;
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
            gProperties.Enabled = !status;
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
                string HTML = client.DownloadString("https://www.igdb.com/search?type=1&q=" + gGame.Name);
                string gamelist = Between(ref HTML, "mar-md-bottom text-muted", "hidden-sm hidden-xs");

                while (Between(ref gamelist, "id", "release_year") != "" && IGDBList.DropDownItems.Count < gForm.GetIGDBListLength())
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
                string igdb_name = Regex.Replace(gGame.Name, "[^a-zA-Z0-9]", "-").ToLower();
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

                gGame.IGDB_Url = "https://www.igdb.com/" + hidden_url;

                if (ImageUri != "")
                {
                    string filename = "https://images.igdb.com/igdb/Image/upload/t_cover_big/" + ImageUri + ".jpg";
                    Bitmap BackgroundImage = FileManager.DownloadImage(filename);

                    gGame.Image = BackgroundImage;
                    GameIcon.BackgroundImage = gGame.Image;
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
            Process.Start("https://www.pcgamingwiki.com/wiki/" + gGame.Name.Replace(" ", "_") + "#Game_data");
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (!gGame.CanSerialize())
                return;

            // Settings tab
            foreach (ListViewItem item in SettingsList.Items)
            {
                string FileName = item.Text;
                if (gGame.Settings.ContainsKey(FileName))
                    gGame.Settings[FileName].IsEnabled = item.Checked;
            }

            // Power Profiles tab
            gGame.PowerProfiles.Clear();
            foreach (ListViewItem item in ProfilesList.Items)
            {
                PowerProfile profile = MainForm.ProfileDB[(Guid)item.Tag];
                if (item.Checked)
                    gGame.PowerProfiles.Add(profile.ProfileGuid, profile);
            }

            gGame.SanityCheck();
            gForm.InsertOrUpdateGameItem(gGame, false);
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
            tabSettingsDesc.ShowToolTips = true;
            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.Text;
                if (gGame.Settings.ContainsKey(FileName))
                {
                    MenuItemRemoveSetting.Enabled = true;

                    foreach (KeyValuePair<string, byte[]> data in gGame.Settings[FileName].data)
                    {
                        TabPage myPage = new TabPage();
                        myPage.Text = data.Key;
                        myPage.Name = data.Key;
                        myPage.ToolTipText = "Double click to change GPU name";

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
                        if (gGame.Settings[FileName].Type == SettingsType.File)
                        {
                            CheckBox UnusedCheckbox = new CheckBox()
                            {
                                Name = data.Key,
                                Text = "Remove file on this GPU Profile",
                                Dock = DockStyle.Top,
                                Checked = gGame.Settings[FileName].removeunused != null && gGame.Settings[FileName].removeunused.ContainsKey(data.Key) ? gGame.Settings[FileName].removeunused[data.Key] : false
                            };
                            UnusedCheckbox.CheckedChanged += UnusedCheckbox_CheckedChanged;
                            myPage.Controls.Add(UnusedCheckbox);
                        }
                        tabSettingsDesc.TabPages.Add(myPage);
                    }
                }
                break;
            }
        }

        private void GPUname_MouseDoubleClick(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = MainForm.CurrentController.Name;
        }

        private void ByteProvider_Changed(object sender, EventArgs e)
        {
            DynamicByteProvider myViewer = (DynamicByteProvider)sender;
            myViewer.ApplyChanges();

            TabPage myPage = tabSettingsDesc.TabPages[tabSettingsDesc.SelectedIndex];

            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.Text;
                gGame.Settings[FileName].data[myPage.Name] = myViewer.Bytes.ToArray();

                break;
            }
        }

        private void tabSettingsDesc_DoubleClick(object sender, EventArgs e)
        {
            TabControl tabControl = (TabControl)sender;
            string currentGPUname = tabControl.TabPages[tabControl.SelectedIndex].Text;
            string newGPUname = Interaction.InputBox("Input new GPU name to change", "Change from '" + currentGPUname + "'", MainForm.CurrentController.Name);
            
            if (newGPUname == "")
                return;

            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.Text;

                Dictionary<string, byte[]> data = gGame.Settings[FileName].data;

                byte[] value;
                if (data.TryGetValue(currentGPUname, out value))
                {
                    gGame.Settings[FileName].data.Remove(currentGPUname);
                    if (data.ContainsKey(newGPUname)) gGame.Settings[FileName].data.Remove(newGPUname); // Delete existed GPU name and replaced with current edit one
                    gGame.Settings[FileName].data.Add(newGPUname, value);
                }

                break;
            }

            SettingsList.SelectedItems.Clear();
        }
        
        private void MyViewer_TextChanged(object sender, EventArgs e)
        {
            RichTextBox myViewer = (RichTextBox)sender;

            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.Text;
                gGame.Settings[FileName].data[myViewer.Name] = Encoding.ASCII.GetBytes(myViewer.Text);

                break;
            }
        }
        private void UnusedCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox UnusedCheckbox = (CheckBox)sender;
            foreach (ListViewItem item in SettingsList.SelectedItems)
            {
                string FileName = item.Text;
                gGame.Settings[FileName].removeunused[UnusedCheckbox.Name] = UnusedCheckbox.Checked;

                break;
            }
        }

        private void checkBoxPowerSpecific_CheckedChanged(object sender, EventArgs e)
        {
            gGame.PowerSpecific = checkBoxPowerSpecific.Checked;
        }

        private void checkBoxPowerProfileSpecific_CheckedChanged(object sender, EventArgs e)
        {
            gGame.PowerProfileSpecific = checkBoxPowerProfileSpecific.Checked;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(checkBoxPowerSpecific, "Use specific game settings based on device power status (on battery, plugged in)");
            toolTip1.SetToolTip(checkBoxPowerProfileSpecific, "Use specific game settings based on device power profile");
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
                string ProfileName = item.Text;
                Guid ProfileGuid = (Guid)item.Tag;
                PowerProfile profile = MainForm.ProfileDB[ProfileGuid];

                // Misc
                if (profile.HasLongPowerMax())
                    textBox1.Text = profile.TurboBoostLongPowerMax;
                if (profile.HasShortPowerMax())
                    textBox2.Text = profile.TurboBoostShortPowerMax;
                if (profile.HasPowerBalanceCPU())
                    textBox3.Text = profile.PowerBalanceCPU;
                if (profile.HasPowerBalanceGPU())
                    textBox4.Text = profile.PowerBalanceGPU;

                // FIVR
                if (profile.HasCPUCore())
                    textBox5.Text = profile.CPUCore;
                if (profile.HasCPUCache())
                    textBox6.Text = profile.CPUCache;
                if (profile.HasSystemAgent())
                    textBox7.Text = profile.SystemAgent;
                if (profile.HasIntelGPU())
                    textBox8.Text = profile.IntelGPU;
            }
        }
    }
}
