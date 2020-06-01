using System;
using System.Windows.Forms;

namespace DockerForm
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SettingsList = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SettingMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemRemoveSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemCreateSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.field_Filename = new System.Windows.Forms.TextBox();
            this.labelFilename = new System.Windows.Forms.Label();
            this.buttonBrowseFile = new System.Windows.Forms.Button();
            this.field_Version = new System.Windows.Forms.TextBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.field_Developer = new System.Windows.Forms.TextBox();
            this.labelDeveloper = new System.Windows.Forms.Label();
            this.field_GUID = new System.Windows.Forms.TextBox();
            this.labelGUID = new System.Windows.Forms.Label();
            this.field_Name = new System.Windows.Forms.TextBox();
            this.labelName = new System.Windows.Forms.Label();
            this.GameIcon = new System.Windows.Forms.PictureBox();
            this.ImageMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.IGDBList = new System.Windows.Forms.ToolStripMenuItem();
            this.searchOnPCGamingWikiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.SettingMenuStrip.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameIcon)).BeginInit();
            this.ImageMenuStrip.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsList
            // 
            this.SettingsList.CheckBoxes = true;
            this.SettingsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5});
            this.SettingsList.ContextMenuStrip = this.SettingMenuStrip;
            this.SettingsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SettingsList.FullRowSelect = true;
            this.SettingsList.HideSelection = false;
            this.SettingsList.LabelEdit = true;
            this.SettingsList.Location = new System.Drawing.Point(3, 16);
            this.SettingsList.Margin = new System.Windows.Forms.Padding(2);
            this.SettingsList.Name = "SettingsList";
            this.SettingsList.Size = new System.Drawing.Size(388, 242);
            this.SettingsList.TabIndex = 7;
            this.SettingsList.UseCompatibleStateImageBehavior = false;
            this.SettingsList.View = System.Windows.Forms.View.Details;
            this.SettingsList.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.SettingsList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SettingsList_MouseDown);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Path";
            this.columnHeader4.Width = 314;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Type";
            this.columnHeader5.Width = 70;
            // 
            // SettingMenuStrip
            // 
            this.SettingMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemRemoveSetting,
            this.MenuItemCreateSetting});
            this.SettingMenuStrip.Name = "contextMenuStrip1";
            this.SettingMenuStrip.Size = new System.Drawing.Size(170, 48);
            // 
            // MenuItemRemoveSetting
            // 
            this.MenuItemRemoveSetting.Enabled = false;
            this.MenuItemRemoveSetting.Name = "MenuItemRemoveSetting";
            this.MenuItemRemoveSetting.Size = new System.Drawing.Size(169, 22);
            this.MenuItemRemoveSetting.Text = "Remove setting(s)";
            this.MenuItemRemoveSetting.ToolTipText = "Remove the targeted setting.";
            this.MenuItemRemoveSetting.Click += new System.EventHandler(this.MenuItemRemoveSetting_Click);
            // 
            // MenuItemCreateSetting
            // 
            this.MenuItemCreateSetting.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.registryToolStripMenuItem});
            this.MenuItemCreateSetting.Name = "MenuItemCreateSetting";
            this.MenuItemCreateSetting.Size = new System.Drawing.Size(169, 22);
            this.MenuItemCreateSetting.Text = "Create setting";
            this.MenuItemCreateSetting.ToolTipText = "Create a new setting.";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // registryToolStripMenuItem
            // 
            this.registryToolStripMenuItem.Name = "registryToolStripMenuItem";
            this.registryToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.registryToolStripMenuItem.Text = "Registry";
            this.registryToolStripMenuItem.Click += new System.EventHandler(this.registryToolStripMenuItem_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Column1";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Column2";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // groupBoxInfo
            // 
            this.groupBoxInfo.Controls.Add(this.field_Filename);
            this.groupBoxInfo.Controls.Add(this.labelFilename);
            this.groupBoxInfo.Controls.Add(this.buttonBrowseFile);
            this.groupBoxInfo.Controls.Add(this.field_Version);
            this.groupBoxInfo.Controls.Add(this.labelVersion);
            this.groupBoxInfo.Controls.Add(this.field_Developer);
            this.groupBoxInfo.Controls.Add(this.labelDeveloper);
            this.groupBoxInfo.Controls.Add(this.field_GUID);
            this.groupBoxInfo.Controls.Add(this.labelGUID);
            this.groupBoxInfo.Controls.Add(this.field_Name);
            this.groupBoxInfo.Controls.Add(this.labelName);
            this.groupBoxInfo.Controls.Add(this.GameIcon);
            this.groupBoxInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxInfo.Location = new System.Drawing.Point(10, 10);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(394, 170);
            this.groupBoxInfo.TabIndex = 0;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Info";
            // 
            // field_Filename
            // 
            this.field_Filename.Location = new System.Drawing.Point(80, 123);
            this.field_Filename.Name = "field_Filename";
            this.field_Filename.ReadOnly = true;
            this.field_Filename.Size = new System.Drawing.Size(159, 20);
            this.field_Filename.TabIndex = 5;
            this.field_Filename.TextChanged += new System.EventHandler(this.field_Filename_TextChanged);
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.Location = new System.Drawing.Point(6, 123);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(49, 13);
            this.labelFilename.TabIndex = 11;
            this.labelFilename.Text = "Filename";
            // 
            // buttonBrowseFile
            // 
            this.buttonBrowseFile.Location = new System.Drawing.Point(245, 123);
            this.buttonBrowseFile.Name = "buttonBrowseFile";
            this.buttonBrowseFile.Size = new System.Drawing.Size(32, 20);
            this.buttonBrowseFile.TabIndex = 6;
            this.buttonBrowseFile.Text = "...";
            this.buttonBrowseFile.UseVisualStyleBackColor = true;
            this.buttonBrowseFile.Click += new System.EventHandler(this.buttonBrowseFile_Click);
            // 
            // field_Version
            // 
            this.field_Version.Location = new System.Drawing.Point(80, 71);
            this.field_Version.Name = "field_Version";
            this.field_Version.ReadOnly = true;
            this.field_Version.Size = new System.Drawing.Size(197, 20);
            this.field_Version.TabIndex = 3;
            this.field_Version.TextChanged += new System.EventHandler(this.field_Version_TextChanged);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(6, 71);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(42, 13);
            this.labelVersion.TabIndex = 7;
            this.labelVersion.Text = "Version";
            // 
            // field_Developer
            // 
            this.field_Developer.Location = new System.Drawing.Point(80, 45);
            this.field_Developer.Name = "field_Developer";
            this.field_Developer.Size = new System.Drawing.Size(197, 20);
            this.field_Developer.TabIndex = 2;
            this.field_Developer.TextChanged += new System.EventHandler(this.field_Developer_TextChanged);
            // 
            // labelDeveloper
            // 
            this.labelDeveloper.AutoSize = true;
            this.labelDeveloper.Location = new System.Drawing.Point(6, 45);
            this.labelDeveloper.Name = "labelDeveloper";
            this.labelDeveloper.Size = new System.Drawing.Size(56, 13);
            this.labelDeveloper.TabIndex = 5;
            this.labelDeveloper.Text = "Developer";
            // 
            // field_GUID
            // 
            this.field_GUID.Location = new System.Drawing.Point(80, 97);
            this.field_GUID.Name = "field_GUID";
            this.field_GUID.ReadOnly = true;
            this.field_GUID.Size = new System.Drawing.Size(197, 20);
            this.field_GUID.TabIndex = 4;
            this.field_GUID.TextChanged += new System.EventHandler(this.field_GUID_TextChanged);
            // 
            // labelGUID
            // 
            this.labelGUID.AutoSize = true;
            this.labelGUID.Location = new System.Drawing.Point(6, 97);
            this.labelGUID.Name = "labelGUID";
            this.labelGUID.Size = new System.Drawing.Size(41, 13);
            this.labelGUID.TabIndex = 3;
            this.labelGUID.Text = "Title ID";
            // 
            // field_Name
            // 
            this.field_Name.Location = new System.Drawing.Point(80, 19);
            this.field_Name.Name = "field_Name";
            this.field_Name.Size = new System.Drawing.Size(197, 20);
            this.field_Name.TabIndex = 1;
            this.field_Name.TextChanged += new System.EventHandler(this.field_Name_TextChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(6, 19);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(35, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // GameIcon
            // 
            this.GameIcon.BackgroundImage = global::DockerForm.Properties.Resources.DefaultBackgroundImage;
            this.GameIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GameIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GameIcon.ContextMenuStrip = this.ImageMenuStrip;
            this.GameIcon.Location = new System.Drawing.Point(283, 19);
            this.GameIcon.Name = "GameIcon";
            this.GameIcon.Size = new System.Drawing.Size(105, 140);
            this.GameIcon.TabIndex = 0;
            this.GameIcon.TabStop = false;
            this.GameIcon.BackgroundImageChanged += new System.EventHandler(this.GameIcon_Click);
            // 
            // ImageMenuStrip
            // 
            this.ImageMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.IGDBList,
            this.searchOnPCGamingWikiToolStripMenuItem});
            this.ImageMenuStrip.Name = "ImageMenuStrip";
            this.ImageMenuStrip.Size = new System.Drawing.Size(213, 48);
            // 
            // IGDBList
            // 
            this.IGDBList.Name = "IGDBList";
            this.IGDBList.Size = new System.Drawing.Size(212, 22);
            this.IGDBList.Text = "Download from IGDB";
            this.IGDBList.Click += new System.EventHandler(this.downloadFromIGDBToolStripMenuItem_Click);
            // 
            // searchOnPCGamingWikiToolStripMenuItem
            // 
            this.searchOnPCGamingWikiToolStripMenuItem.Name = "searchOnPCGamingWikiToolStripMenuItem";
            this.searchOnPCGamingWikiToolStripMenuItem.Size = new System.Drawing.Size(212, 22);
            this.searchOnPCGamingWikiToolStripMenuItem.Text = "Search on PCGaming Wiki";
            this.searchOnPCGamingWikiToolStripMenuItem.Click += new System.EventHandler(this.searchOnPCGamingWikiToolStripMenuItem_Click);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.SettingsList);
            this.groupBoxSettings.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBoxSettings.Location = new System.Drawing.Point(10, 186);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(394, 261);
            this.groupBoxSettings.TabIndex = 1;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 457);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxInfo);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_Closing);
            this.SettingMenuStrip.ResumeLayout(false);
            this.groupBoxInfo.ResumeLayout(false);
            this.groupBoxInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameIcon)).EndInit();
            this.ImageMenuStrip.ResumeLayout(false);
            this.groupBoxSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxInfo;
        private System.Windows.Forms.TextBox field_Name;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.PictureBox GameIcon;
        private System.Windows.Forms.TextBox field_GUID;
        private System.Windows.Forms.Label labelGUID;
        private System.Windows.Forms.TextBox field_Developer;
        private System.Windows.Forms.Label labelDeveloper;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.ListView SettingsList;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private TextBox field_Version;
        private Label labelVersion;
        private Button buttonBrowseFile;
        private ContextMenuStrip SettingMenuStrip;
        private ToolStripMenuItem MenuItemRemoveSetting;
        private ToolStripMenuItem MenuItemCreateSetting;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem registryToolStripMenuItem;
        private ContextMenuStrip ImageMenuStrip;
        private ToolStripMenuItem IGDBList;
        private TextBox field_Filename;
        private Label labelFilename;
        private ToolStripMenuItem searchOnPCGamingWikiToolStripMenuItem;
    }
}