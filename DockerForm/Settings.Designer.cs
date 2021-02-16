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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            this.SettingMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemRemoveSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemCreateSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxInfo = new System.Windows.Forms.GroupBox();
            this.labelArguments = new System.Windows.Forms.Label();
            this.field_Arguments = new System.Windows.Forms.TextBox();
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.SettingsList = new System.Windows.Forms.ListView();
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.tabPowerProfiles = new System.Windows.Forms.TabPage();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabSettingsDesc = new System.Windows.Forms.TabControl();
            this.File = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SettingMenuStrip.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameIcon)).BeginInit();
            this.ImageMenuStrip.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.SuspendLayout();
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
            this.groupBoxInfo.Controls.Add(this.labelArguments);
            this.groupBoxInfo.Controls.Add(this.field_Arguments);
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
            this.groupBoxInfo.Location = new System.Drawing.Point(12, 12);
            this.groupBoxInfo.Name = "groupBoxInfo";
            this.groupBoxInfo.Size = new System.Drawing.Size(282, 566);
            this.groupBoxInfo.TabIndex = 0;
            this.groupBoxInfo.TabStop = false;
            this.groupBoxInfo.Text = "Info";
            // 
            // labelArguments
            // 
            this.labelArguments.AutoSize = true;
            this.labelArguments.Location = new System.Drawing.Point(6, 532);
            this.labelArguments.Name = "labelArguments";
            this.labelArguments.Size = new System.Drawing.Size(66, 15);
            this.labelArguments.TabIndex = 13;
            this.labelArguments.Text = "Arguments";
            // 
            // field_Arguments
            // 
            this.field_Arguments.Location = new System.Drawing.Point(76, 529);
            this.field_Arguments.Name = "field_Arguments";
            this.field_Arguments.Size = new System.Drawing.Size(197, 23);
            this.field_Arguments.TabIndex = 12;
            this.field_Arguments.TextChanged += new System.EventHandler(this.field_arguments_TextChanged);
            // 
            // field_Filename
            // 
            this.field_Filename.Location = new System.Drawing.Point(76, 500);
            this.field_Filename.Name = "field_Filename";
            this.field_Filename.ReadOnly = true;
            this.field_Filename.Size = new System.Drawing.Size(153, 23);
            this.field_Filename.TabIndex = 5;
            // 
            // labelFilename
            // 
            this.labelFilename.AutoSize = true;
            this.labelFilename.Location = new System.Drawing.Point(6, 504);
            this.labelFilename.Name = "labelFilename";
            this.labelFilename.Size = new System.Drawing.Size(55, 15);
            this.labelFilename.TabIndex = 11;
            this.labelFilename.Text = "Filename";
            // 
            // buttonBrowseFile
            // 
            this.buttonBrowseFile.Location = new System.Drawing.Point(236, 500);
            this.buttonBrowseFile.Name = "buttonBrowseFile";
            this.buttonBrowseFile.Size = new System.Drawing.Size(37, 23);
            this.buttonBrowseFile.TabIndex = 6;
            this.buttonBrowseFile.Text = "...";
            this.buttonBrowseFile.UseVisualStyleBackColor = true;
            this.buttonBrowseFile.Click += new System.EventHandler(this.buttonBrowseFile_Click);
            // 
            // field_Version
            // 
            this.field_Version.Location = new System.Drawing.Point(76, 440);
            this.field_Version.Name = "field_Version";
            this.field_Version.ReadOnly = true;
            this.field_Version.Size = new System.Drawing.Size(197, 23);
            this.field_Version.TabIndex = 3;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(6, 443);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(45, 15);
            this.labelVersion.TabIndex = 7;
            this.labelVersion.Text = "Version";
            // 
            // field_Developer
            // 
            this.field_Developer.Location = new System.Drawing.Point(76, 410);
            this.field_Developer.Name = "field_Developer";
            this.field_Developer.Size = new System.Drawing.Size(197, 23);
            this.field_Developer.TabIndex = 2;
            this.field_Developer.TextChanged += new System.EventHandler(this.field_Developer_TextChanged);
            // 
            // labelDeveloper
            // 
            this.labelDeveloper.AutoSize = true;
            this.labelDeveloper.Location = new System.Drawing.Point(6, 413);
            this.labelDeveloper.Name = "labelDeveloper";
            this.labelDeveloper.Size = new System.Drawing.Size(60, 15);
            this.labelDeveloper.TabIndex = 5;
            this.labelDeveloper.Text = "Developer";
            // 
            // field_GUID
            // 
            this.field_GUID.Location = new System.Drawing.Point(76, 470);
            this.field_GUID.Name = "field_GUID";
            this.field_GUID.ReadOnly = true;
            this.field_GUID.Size = new System.Drawing.Size(197, 23);
            this.field_GUID.TabIndex = 4;
            // 
            // labelGUID
            // 
            this.labelGUID.AutoSize = true;
            this.labelGUID.Location = new System.Drawing.Point(6, 473);
            this.labelGUID.Name = "labelGUID";
            this.labelGUID.Size = new System.Drawing.Size(43, 15);
            this.labelGUID.TabIndex = 3;
            this.labelGUID.Text = "Title ID";
            // 
            // field_Name
            // 
            this.field_Name.Location = new System.Drawing.Point(76, 380);
            this.field_Name.Name = "field_Name";
            this.field_Name.Size = new System.Drawing.Size(197, 23);
            this.field_Name.TabIndex = 1;
            this.field_Name.TextChanged += new System.EventHandler(this.field_Name_TextChanged);
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(6, 383);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(39, 15);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // GameIcon
            // 
            this.GameIcon.BackgroundImage = global::DockerForm.Properties.Resources.DefaultBackgroundImage;
            this.GameIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GameIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GameIcon.ContextMenuStrip = this.ImageMenuStrip;
            this.GameIcon.Location = new System.Drawing.Point(9, 22);
            this.GameIcon.Name = "GameIcon";
            this.GameIcon.Size = new System.Drawing.Size(264, 352);
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSettings);
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabPowerProfiles);
            this.tabControl1.Location = new System.Drawing.Point(300, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(613, 566);
            this.tabControl1.TabIndex = 3;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabSettingsDesc);
            this.tabSettings.Controls.Add(this.SettingsList);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(605, 538);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // SettingsList
            // 
            this.SettingsList.CheckBoxes = true;
            this.SettingsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.File,
            this.Path,
            this.Type});
            this.SettingsList.ContextMenuStrip = this.SettingMenuStrip;
            this.SettingsList.Dock = System.Windows.Forms.DockStyle.Top;
            this.SettingsList.FullRowSelect = true;
            this.SettingsList.HideSelection = false;
            this.SettingsList.Location = new System.Drawing.Point(3, 3);
            this.SettingsList.Margin = new System.Windows.Forms.Padding(2);
            this.SettingsList.MultiSelect = false;
            this.SettingsList.Name = "SettingsList";
            this.SettingsList.Size = new System.Drawing.Size(599, 260);
            this.SettingsList.TabIndex = 8;
            this.SettingsList.UseCompatibleStateImageBehavior = false;
            this.SettingsList.View = System.Windows.Forms.View.Details;
            this.SettingsList.SelectedIndexChanged += new System.EventHandler(this.SettingsList_SelectedIndexChanged);
            // 
            // Path
            // 
            this.Path.Text = "Path";
            this.Path.Width = 350;
            // 
            // Type
            // 
            this.Type.Text = "Type";
            this.Type.Width = 100;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupBoxGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(605, 538);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxGeneral.Location = new System.Drawing.Point(0, 0);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(605, 538);
            this.groupBoxGeneral.TabIndex = 0;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // tabPowerProfiles
            // 
            this.tabPowerProfiles.Location = new System.Drawing.Point(4, 24);
            this.tabPowerProfiles.Name = "tabPowerProfiles";
            this.tabPowerProfiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPowerProfiles.Size = new System.Drawing.Size(605, 538);
            this.tabPowerProfiles.TabIndex = 1;
            this.tabPowerProfiles.Text = "Power Profiles";
            this.tabPowerProfiles.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(838, 580);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(757, 580);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // tabSettingsDesc
            // 
            this.tabSettingsDesc.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabSettingsDesc.Location = new System.Drawing.Point(3, 268);
            this.tabSettingsDesc.Name = "tabSettingsDesc";
            this.tabSettingsDesc.SelectedIndex = 0;
            this.tabSettingsDesc.Size = new System.Drawing.Size(599, 267);
            this.tabSettingsDesc.TabIndex = 9;
            // 
            // File
            // 
            this.File.Text = "File";
            this.File.Width = 150;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 611);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBoxInfo);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Properties";
            this.SettingMenuStrip.ResumeLayout(false);
            this.groupBoxInfo.ResumeLayout(false);
            this.groupBoxInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameIcon)).EndInit();
            this.ImageMenuStrip.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
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
        private Label labelArguments;
        private TextBox field_Arguments;
        private TabControl tabControl1;
        private TabPage tabSettings;
        private TabPage tabPowerProfiles;
        private ListView SettingsList;
        private ColumnHeader Path;
        private ColumnHeader Type;
        private TabPage tabGeneral;
        private GroupBox groupBoxGeneral;
        private Button buttonCancel;
        private Button buttonOK;
        private TabControl tabSettingsDesc;
        private ColumnHeader File;
    }
}