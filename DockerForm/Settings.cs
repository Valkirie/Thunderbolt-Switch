using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.VisualBasic;
using System.Windows.Input;

namespace DockerForm
{
    public partial class Settings : Form
    {
        // Form vars
        static Form1 thisForm;
        static Settings thisSetting;
        static bool IsReady;

        // Game vars
        static DockerGame thisGame;

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

        public Settings(Form1 form)
        {
            InitializeComponent();

            // instances
            thisForm = form;
            thisSetting = this;
            SetStartPos();

            IsReady = PickAGame();
        }

        public Settings(Form1 form, DockerGame game)
        {
            InitializeComponent();

            // instances
            thisGame = game;
            thisForm                = form;
            thisSetting             = this;
            SetStartPos();

            field_Name.Text         = game.Name;
            field_GUID.Text         = game.GUID;
            field_Filename.Text     = game.Executable;
            field_Version.Text      = game.Version;
            field_Developer.Text    = game.Company;

            GameIcon.BackgroundImage = game.Image;

            foreach (GameSettings setting in game.Settings.Values)
            {
                ListViewItem newSetting = new ListViewItem(new string[] { setting.Uri, Enum.GetName(typeof(SettingsType), setting.Type) }, setting.GUID );
                newSetting.Checked = setting.IsEnabled;
                newSetting.Tag = setting.IsRelative;
                SettingsList.Items.Add(newSetting);
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

        private void Settings_Closing(object sender, FormClosingEventArgs e)
        {
            if (!thisGame.CanSerialize())
                return;

            // Clear and update current settings
            List<int> GUIDs = new List<int>();
            foreach(ListViewItem item in SettingsList.Items)
            {
                string uri = item.SubItems[0].Text;
                SettingsType type = (SettingsType)Enum.Parse(typeof(SettingsType), item.SubItems[1].Text);
                int guid = Math.Abs((uri).GetHashCode());

                GameSettings newSetting = new GameSettings(guid, type, uri, item.Checked, (bool)item.Tag);
                if (!thisGame.Settings.ContainsKey(guid))
                    thisGame.Settings.Add(guid, newSetting);
                else
                    thisGame.Settings[guid] = newSetting;

                GUIDs.Add(guid);
            }

            int[] keys = thisGame.Settings.Keys.ToArray();
            foreach(int key in keys)
            {
                if (!GUIDs.Contains(key))
                    thisGame.Settings.Remove(key);
            }

            thisGame.SanityCheck();
            thisForm.InsertOrUpdateGameItem(thisGame);
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
                    thisGame = new DockerGame(filePath);

                    field_Name.Text = thisGame.ProductName;
                    field_Version.Text = thisGame.Version;
                    field_Filename.Text = thisGame.Executable;
                    field_Developer.Text = thisGame.Company;
                    field_GUID.Text = thisGame.GUID;
                    GameIcon.BackgroundImage = thisGame.Image;

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
            thisGame.Company = field_Developer.Text;
        }

        private void field_Name_TextChanged(object sender, EventArgs e)
        {
            thisGame.Name = field_Name.Text;
        }

        private void MenuItemRemoveSetting_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in SettingsList.SelectedItems)
                SettingsList.Items.Remove(item);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SettingsList.SelectedItems.Count != 0)
                SettingMenuStrip.Items[0].Enabled = true;
            else
                SettingMenuStrip.Items[0].Enabled = false;
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

        public static string ContractEnvironmentVariables(string path, ref bool IsRelative)
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
                        string filename = ContractEnvironmentVariables(file, ref IsRelative);
                        ListViewItem listViewItem1 = new ListViewItem(new string[] { filename, "File" }, -1);
                        listViewItem1.Checked = true;
                        listViewItem1.Tag = IsRelative;
                        SettingsList.Items.Add(listViewItem1);
                    }
                }
            }
        }

        private void registryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string UserAnswer = Interaction.InputBox("", thisGame.Name + " - Registry Key", @"HKEY_CURRENT_USER\SOFTWARE\");

            if (UserAnswer == "")
                return;

            ListViewItem listViewItem1 = new ListViewItem(new string[] { UserAnswer, "Registry" }, -1);
            listViewItem1.Checked = true;
            listViewItem1.Tag = false;
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
            }catch(Exception) { }

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
                string hidden_url       = IGDB["url"];

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
    }
}
