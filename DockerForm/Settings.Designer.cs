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
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabSettingsDesc = new System.Windows.Forms.TabControl();
            this.SettingsList = new System.Windows.Forms.ListView();
            this.File = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.checkBoxPowerSpecific = new System.Windows.Forms.CheckBox();
            this.tabPowerProfiles = new System.Windows.Forms.TabPage();
            this.groupBoxFIVR = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.ProfilesList = new System.Windows.Forms.ListView();
            this.PowerProfileHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PowerProfileHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PowerProfileHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PowerProfileHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PowerProfileHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBoxPowerProfile = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SettingMenuStrip.SuspendLayout();
            this.groupBoxInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameIcon)).BeginInit();
            this.ImageMenuStrip.SuspendLayout();
            this.tabControlSettings.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupBoxGeneral.SuspendLayout();
            this.tabPowerProfiles.SuspendLayout();
            this.groupBoxFIVR.SuspendLayout();
            this.groupBoxPowerProfile.SuspendLayout();
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
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabSettings);
            this.tabControlSettings.Controls.Add(this.tabGeneral);
            this.tabControlSettings.Controls.Add(this.tabPowerProfiles);
            this.tabControlSettings.Location = new System.Drawing.Point(300, 12);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(532, 566);
            this.tabControlSettings.TabIndex = 3;
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.tabSettingsDesc);
            this.tabSettings.Controls.Add(this.SettingsList);
            this.tabSettings.Location = new System.Drawing.Point(4, 24);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Size = new System.Drawing.Size(524, 538);
            this.tabSettings.TabIndex = 0;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabSettingsDesc
            // 
            this.tabSettingsDesc.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabSettingsDesc.Location = new System.Drawing.Point(3, 268);
            this.tabSettingsDesc.Name = "tabSettingsDesc";
            this.tabSettingsDesc.SelectedIndex = 0;
            this.tabSettingsDesc.Size = new System.Drawing.Size(518, 267);
            this.tabSettingsDesc.TabIndex = 9;
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
            this.SettingsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SettingsList.HideSelection = false;
            this.SettingsList.Location = new System.Drawing.Point(3, 3);
            this.SettingsList.Margin = new System.Windows.Forms.Padding(2);
            this.SettingsList.MultiSelect = false;
            this.SettingsList.Name = "SettingsList";
            this.SettingsList.Size = new System.Drawing.Size(518, 260);
            this.SettingsList.TabIndex = 8;
            this.SettingsList.UseCompatibleStateImageBehavior = false;
            this.SettingsList.View = System.Windows.Forms.View.Details;
            this.SettingsList.SelectedIndexChanged += new System.EventHandler(this.SettingsList_SelectedIndexChanged);
            // 
            // File
            // 
            this.File.Text = "File";
            this.File.Width = 150;
            // 
            // Path
            // 
            this.Path.Text = "Path";
            this.Path.Width = 264;
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
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(524, 538);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Controls.Add(this.checkBoxPowerSpecific);
            this.groupBoxGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxGeneral.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(518, 532);
            this.groupBoxGeneral.TabIndex = 0;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "General";
            // 
            // checkBoxPowerSpecific
            // 
            this.checkBoxPowerSpecific.AutoSize = true;
            this.checkBoxPowerSpecific.Location = new System.Drawing.Point(6, 22);
            this.checkBoxPowerSpecific.Name = "checkBoxPowerSpecific";
            this.checkBoxPowerSpecific.Size = new System.Drawing.Size(170, 19);
            this.checkBoxPowerSpecific.TabIndex = 0;
            this.checkBoxPowerSpecific.Text = "Use power-specific settings";
            this.checkBoxPowerSpecific.UseVisualStyleBackColor = true;
            this.checkBoxPowerSpecific.CheckedChanged += new System.EventHandler(this.checkBoxPowerSpecific_CheckedChanged);
            // 
            // tabPowerProfiles
            // 
            this.tabPowerProfiles.Controls.Add(this.groupBoxFIVR);
            this.tabPowerProfiles.Controls.Add(this.ProfilesList);
            this.tabPowerProfiles.Controls.Add(this.groupBoxPowerProfile);
            this.tabPowerProfiles.Location = new System.Drawing.Point(4, 24);
            this.tabPowerProfiles.Name = "tabPowerProfiles";
            this.tabPowerProfiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPowerProfiles.Size = new System.Drawing.Size(524, 538);
            this.tabPowerProfiles.TabIndex = 1;
            this.tabPowerProfiles.Text = "Power Profiles";
            this.tabPowerProfiles.UseVisualStyleBackColor = true;
            // 
            // groupBoxFIVR
            // 
            this.groupBoxFIVR.Controls.Add(this.label8);
            this.groupBoxFIVR.Controls.Add(this.label7);
            this.groupBoxFIVR.Controls.Add(this.label6);
            this.groupBoxFIVR.Controls.Add(this.label5);
            this.groupBoxFIVR.Controls.Add(this.textBox8);
            this.groupBoxFIVR.Controls.Add(this.textBox5);
            this.groupBoxFIVR.Controls.Add(this.textBox7);
            this.groupBoxFIVR.Controls.Add(this.textBox6);
            this.groupBoxFIVR.Location = new System.Drawing.Point(265, 389);
            this.groupBoxFIVR.Name = "groupBoxFIVR";
            this.groupBoxFIVR.Size = new System.Drawing.Size(256, 146);
            this.groupBoxFIVR.TabIndex = 10;
            this.groupBoxFIVR.TabStop = false;
            this.groupBoxFIVR.Text = "FIVR Control";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(94, 15);
            this.label8.TabIndex = 14;
            this.label8.Text = "Intel GPU Offset:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "System Agent Offset:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "CPU Cache Offset:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "CPU Core Offset:";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(166, 109);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(84, 23);
            this.textBox8.TabIndex = 11;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(166, 22);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(84, 23);
            this.textBox5.TabIndex = 8;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(166, 80);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(84, 23);
            this.textBox7.TabIndex = 10;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(166, 51);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(84, 23);
            this.textBox6.TabIndex = 9;
            // 
            // ProfilesList
            // 
            this.ProfilesList.CheckBoxes = true;
            this.ProfilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PowerProfileHeader1,
            this.PowerProfileHeader2,
            this.PowerProfileHeader3,
            this.PowerProfileHeader4,
            this.PowerProfileHeader6});
            this.ProfilesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfilesList.FullRowSelect = true;
            this.ProfilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ProfilesList.HideSelection = false;
            this.ProfilesList.Location = new System.Drawing.Point(3, 3);
            this.ProfilesList.Margin = new System.Windows.Forms.Padding(2);
            this.ProfilesList.MultiSelect = false;
            this.ProfilesList.Name = "ProfilesList";
            this.ProfilesList.Size = new System.Drawing.Size(518, 381);
            this.ProfilesList.TabIndex = 9;
            this.ProfilesList.UseCompatibleStateImageBehavior = false;
            this.ProfilesList.View = System.Windows.Forms.View.Details;
            this.ProfilesList.SelectedIndexChanged += new System.EventHandler(this.ProfilesList_SelectedIndexChanged);
            // 
            // PowerProfileHeader1
            // 
            this.PowerProfileHeader1.Text = "Name";
            this.PowerProfileHeader1.Width = 214;
            // 
            // PowerProfileHeader2
            // 
            this.PowerProfileHeader2.Text = "OnBattery";
            this.PowerProfileHeader2.Width = 75;
            // 
            // PowerProfileHeader3
            // 
            this.PowerProfileHeader3.Text = "OnPlugged";
            this.PowerProfileHeader3.Width = 75;
            // 
            // PowerProfileHeader4
            // 
            this.PowerProfileHeader4.Text = "OnExtGPU";
            this.PowerProfileHeader4.Width = 75;
            // 
            // PowerProfileHeader6
            // 
            this.PowerProfileHeader6.Text = "OnScreen";
            this.PowerProfileHeader6.Width = 75;
            // 
            // groupBoxPowerProfile
            // 
            this.groupBoxPowerProfile.Controls.Add(this.label4);
            this.groupBoxPowerProfile.Controls.Add(this.label3);
            this.groupBoxPowerProfile.Controls.Add(this.textBox4);
            this.groupBoxPowerProfile.Controls.Add(this.textBox3);
            this.groupBoxPowerProfile.Controls.Add(this.textBox2);
            this.groupBoxPowerProfile.Controls.Add(this.textBox1);
            this.groupBoxPowerProfile.Controls.Add(this.label2);
            this.groupBoxPowerProfile.Controls.Add(this.label1);
            this.groupBoxPowerProfile.Location = new System.Drawing.Point(3, 389);
            this.groupBoxPowerProfile.Name = "groupBoxPowerProfile";
            this.groupBoxPowerProfile.Size = new System.Drawing.Size(256, 146);
            this.groupBoxPowerProfile.TabIndex = 1;
            this.groupBoxPowerProfile.TabStop = false;
            this.groupBoxPowerProfile.Text = "Power Profile";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Power Balance GPU:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Power Balance CPU:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(167, 109);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(84, 23);
            this.textBox4.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(167, 80);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(84, 23);
            this.textBox3.TabIndex = 4;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(167, 51);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(84, 23);
            this.textBox2.TabIndex = 3;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(167, 22);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(84, 23);
            this.textBox1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "TurboBoostShortPowerMax:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "TurboBoostLongPowerMax:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(757, 580);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(676, 580);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 611);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControlSettings);
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
            this.Load += new System.EventHandler(this.Settings_Load);
            this.SettingMenuStrip.ResumeLayout(false);
            this.groupBoxInfo.ResumeLayout(false);
            this.groupBoxInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.GameIcon)).EndInit();
            this.ImageMenuStrip.ResumeLayout(false);
            this.tabControlSettings.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.tabPowerProfiles.ResumeLayout(false);
            this.groupBoxFIVR.ResumeLayout(false);
            this.groupBoxFIVR.PerformLayout();
            this.groupBoxPowerProfile.ResumeLayout(false);
            this.groupBoxPowerProfile.PerformLayout();
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
        private TabControl tabControlSettings;
        private TabPage tabSettings;
        private TabPage tabPowerProfiles;
        private TabPage tabGeneral;
        private Button buttonCancel;
        private Button buttonOK;
        private TabControl tabSettingsDesc;
        private GroupBox groupBoxPowerProfile;
        private GroupBox groupBoxGeneral;
        private CheckBox checkBoxPowerSpecific;
        private ToolTip toolTip1;
        private ListView SettingsList;
        private ColumnHeader File;
        private ColumnHeader Path;
        private ColumnHeader Type;
        private ListView ProfilesList;
        private ColumnHeader PowerProfileHeader1;
        private ColumnHeader PowerProfileHeader2;
        private ColumnHeader PowerProfileHeader3;
        private ColumnHeader PowerProfileHeader4;
        private Label label2;
        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label4;
        private Label label3;
        private TextBox textBox4;
        private TextBox textBox3;
        private GroupBox groupBoxFIVR;
        private TextBox textBox8;
        private TextBox textBox5;
        private TextBox textBox7;
        private TextBox textBox6;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private ColumnHeader PowerProfileHeader6;
    }
}