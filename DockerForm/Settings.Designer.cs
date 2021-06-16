
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
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxMonitorPowerProfiles = new System.Windows.Forms.CheckBox();
            this.checkBoxMonitorHardware = new System.Windows.Forms.CheckBox();
            this.checkBoxSaveOnExit = new System.Windows.Forms.CheckBox();
            this.checkBoxMonitorProcesses = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxToastNotifications = new System.Windows.Forms.CheckBox();
            this.checkBoxBootOnStartup = new System.Windows.Forms.CheckBox();
            this.groupBoxGeneral = new System.Windows.Forms.GroupBox();
            this.checkBoxMinimizeOnClosing = new System.Windows.Forms.CheckBox();
            this.checkBoxMinimizeOnStartup = new System.Windows.Forms.CheckBox();
            this.tabPowerProfiles = new System.Windows.Forms.TabPage();
            this.groupBoxTriggers = new System.Windows.Forms.GroupBox();
            this.listBoxTriggers = new System.Windows.Forms.ListBox();
            this.groupBoxFIVR = new System.Windows.Forms.GroupBox();
            this.numericUpDown8 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown7 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ProfilesList = new System.Windows.Forms.ListView();
            this.PowerProfileHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SettingMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuItemRemoveSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemCreateSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxPowerProfile = new System.Windows.Forms.GroupBox();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.tabControlSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxGeneral.SuspendLayout();
            this.tabPowerProfiles.SuspendLayout();
            this.groupBoxTriggers.SuspendLayout();
            this.groupBoxFIVR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            this.SettingMenuStrip.SuspendLayout();
            this.groupBoxPowerProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabGeneral);
            this.tabControlSettings.Controls.Add(this.tabPowerProfiles);
            this.tabControlSettings.Location = new System.Drawing.Point(12, 12);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(759, 501);
            this.tabControlSettings.TabIndex = 4;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupBox2);
            this.tabGeneral.Controls.Add(this.groupBox1);
            this.tabGeneral.Controls.Add(this.groupBoxGeneral);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(751, 475);
            this.tabGeneral.TabIndex = 2;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.checkBoxMonitorPowerProfiles);
            this.groupBox2.Controls.Add(this.checkBoxMonitorHardware);
            this.groupBox2.Controls.Add(this.checkBoxSaveOnExit);
            this.groupBox2.Controls.Add(this.checkBoxMonitorProcesses);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 143);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(745, 116);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Environment";
            // 
            // checkBoxMonitorPowerProfiles
            // 
            this.checkBoxMonitorPowerProfiles.AutoSize = true;
            this.checkBoxMonitorPowerProfiles.Location = new System.Drawing.Point(6, 45);
            this.checkBoxMonitorPowerProfiles.Name = "checkBoxMonitorPowerProfiles";
            this.checkBoxMonitorPowerProfiles.Size = new System.Drawing.Size(129, 17);
            this.checkBoxMonitorPowerProfiles.TabIndex = 3;
            this.checkBoxMonitorPowerProfiles.Text = "Monitor power profiles";
            this.toolTip1.SetToolTip(this.checkBoxMonitorPowerProfiles, "Monitor if a power profile has been altered and automatically update and apply ne" +
        "w values");
            this.checkBoxMonitorPowerProfiles.UseVisualStyleBackColor = true;
            this.checkBoxMonitorPowerProfiles.CheckedChanged += new System.EventHandler(this.checkBoxMonitorPowerProfiles_CheckedChanged);
            // 
            // checkBoxMonitorHardware
            // 
            this.checkBoxMonitorHardware.AutoSize = true;
            this.checkBoxMonitorHardware.Location = new System.Drawing.Point(6, 68);
            this.checkBoxMonitorHardware.Name = "checkBoxMonitorHardware";
            this.checkBoxMonitorHardware.Size = new System.Drawing.Size(215, 17);
            this.checkBoxMonitorHardware.TabIndex = 2;
            this.checkBoxMonitorHardware.Text = "Automatically switch internal GPU status";
            this.toolTip1.SetToolTip(this.checkBoxMonitorHardware, "Automatically enable or disable internal GPU when a discrete GPU is enabled or di" +
        "sabled");
            this.checkBoxMonitorHardware.UseVisualStyleBackColor = true;
            this.checkBoxMonitorHardware.CheckedChanged += new System.EventHandler(this.checkBoxMonitorHardware_CheckedChanged);
            // 
            // checkBoxSaveOnExit
            // 
            this.checkBoxSaveOnExit.AutoSize = true;
            this.checkBoxSaveOnExit.Location = new System.Drawing.Point(6, 91);
            this.checkBoxSaveOnExit.Name = "checkBoxSaveOnExit";
            this.checkBoxSaveOnExit.Size = new System.Drawing.Size(286, 17);
            this.checkBoxSaveOnExit.TabIndex = 1;
            this.checkBoxSaveOnExit.Text = "Save environment settings when I close the application";
            this.checkBoxSaveOnExit.UseVisualStyleBackColor = true;
            this.checkBoxSaveOnExit.CheckedChanged += new System.EventHandler(this.checkBoxSaveOnExit_CheckedChanged);
            // 
            // checkBoxMonitorProcesses
            // 
            this.checkBoxMonitorProcesses.AutoSize = true;
            this.checkBoxMonitorProcesses.Location = new System.Drawing.Point(6, 22);
            this.checkBoxMonitorProcesses.Name = "checkBoxMonitorProcesses";
            this.checkBoxMonitorProcesses.Size = new System.Drawing.Size(151, 17);
            this.checkBoxMonitorProcesses.TabIndex = 0;
            this.checkBoxMonitorProcesses.Text = "Monitor applications status";
            this.toolTip1.SetToolTip(this.checkBoxMonitorProcesses, "Monitor if an application has started or exited. Needed to apply app-specific pow" +
        "er profile");
            this.checkBoxMonitorProcesses.UseVisualStyleBackColor = true;
            this.checkBoxMonitorProcesses.CheckedChanged += new System.EventHandler(this.checkBoxMonitorProcesses_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxToastNotifications);
            this.groupBox1.Controls.Add(this.checkBoxBootOnStartup);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(3, 73);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(745, 70);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interface";
            // 
            // checkBoxToastNotifications
            // 
            this.checkBoxToastNotifications.AutoSize = true;
            this.checkBoxToastNotifications.Location = new System.Drawing.Point(6, 45);
            this.checkBoxToastNotifications.Name = "checkBoxToastNotifications";
            this.checkBoxToastNotifications.Size = new System.Drawing.Size(166, 17);
            this.checkBoxToastNotifications.TabIndex = 1;
            this.checkBoxToastNotifications.Text = "Show application notifications";
            this.checkBoxToastNotifications.UseVisualStyleBackColor = true;
            this.checkBoxToastNotifications.CheckedChanged += new System.EventHandler(this.checkBoxToastNotifications_CheckedChanged);
            // 
            // checkBoxBootOnStartup
            // 
            this.checkBoxBootOnStartup.AutoSize = true;
            this.checkBoxBootOnStartup.Location = new System.Drawing.Point(6, 22);
            this.checkBoxBootOnStartup.Name = "checkBoxBootOnStartup";
            this.checkBoxBootOnStartup.Size = new System.Drawing.Size(309, 17);
            this.checkBoxBootOnStartup.TabIndex = 0;
            this.checkBoxBootOnStartup.Text = "Automatically start the application when I log on to Windows";
            this.checkBoxBootOnStartup.UseVisualStyleBackColor = true;
            this.checkBoxBootOnStartup.CheckedChanged += new System.EventHandler(this.checkBoxBootOnStartup_CheckedChanged);
            // 
            // groupBoxGeneral
            // 
            this.groupBoxGeneral.Controls.Add(this.checkBoxMinimizeOnClosing);
            this.groupBoxGeneral.Controls.Add(this.checkBoxMinimizeOnStartup);
            this.groupBoxGeneral.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxGeneral.Location = new System.Drawing.Point(3, 3);
            this.groupBoxGeneral.Name = "groupBoxGeneral";
            this.groupBoxGeneral.Size = new System.Drawing.Size(745, 70);
            this.groupBoxGeneral.TabIndex = 0;
            this.groupBoxGeneral.TabStop = false;
            this.groupBoxGeneral.Text = "Taskbar";
            // 
            // checkBoxMinimizeOnClosing
            // 
            this.checkBoxMinimizeOnClosing.AutoSize = true;
            this.checkBoxMinimizeOnClosing.Location = new System.Drawing.Point(6, 45);
            this.checkBoxMinimizeOnClosing.Name = "checkBoxMinimizeOnClosing";
            this.checkBoxMinimizeOnClosing.Size = new System.Drawing.Size(309, 17);
            this.checkBoxMinimizeOnClosing.TabIndex = 1;
            this.checkBoxMinimizeOnClosing.Text = "Minimize to the notification area when I close the application";
            this.checkBoxMinimizeOnClosing.UseVisualStyleBackColor = true;
            this.checkBoxMinimizeOnClosing.CheckedChanged += new System.EventHandler(this.checkBoxMinimizeOnClosing_CheckedChanged);
            // 
            // checkBoxMinimizeOnStartup
            // 
            this.checkBoxMinimizeOnStartup.AutoSize = true;
            this.checkBoxMinimizeOnStartup.Location = new System.Drawing.Point(6, 22);
            this.checkBoxMinimizeOnStartup.Name = "checkBoxMinimizeOnStartup";
            this.checkBoxMinimizeOnStartup.Size = new System.Drawing.Size(300, 17);
            this.checkBoxMinimizeOnStartup.TabIndex = 0;
            this.checkBoxMinimizeOnStartup.Text = "Minimize to the notification area when I log on to Windows";
            this.checkBoxMinimizeOnStartup.UseVisualStyleBackColor = true;
            this.checkBoxMinimizeOnStartup.CheckedChanged += new System.EventHandler(this.checkBoxPowerSpecific_CheckedChanged);
            // 
            // tabPowerProfiles
            // 
            this.tabPowerProfiles.Controls.Add(this.groupBoxTriggers);
            this.tabPowerProfiles.Controls.Add(this.groupBoxFIVR);
            this.tabPowerProfiles.Controls.Add(this.ProfilesList);
            this.tabPowerProfiles.Controls.Add(this.groupBoxPowerProfile);
            this.tabPowerProfiles.Location = new System.Drawing.Point(4, 22);
            this.tabPowerProfiles.Name = "tabPowerProfiles";
            this.tabPowerProfiles.Padding = new System.Windows.Forms.Padding(3);
            this.tabPowerProfiles.Size = new System.Drawing.Size(751, 475);
            this.tabPowerProfiles.TabIndex = 1;
            this.tabPowerProfiles.Text = "Power Profiles";
            this.tabPowerProfiles.UseVisualStyleBackColor = true;
            // 
            // groupBoxTriggers
            // 
            this.groupBoxTriggers.Controls.Add(this.listBoxTriggers);
            this.groupBoxTriggers.Location = new System.Drawing.Point(530, 323);
            this.groupBoxTriggers.Name = "groupBoxTriggers";
            this.groupBoxTriggers.Size = new System.Drawing.Size(215, 146);
            this.groupBoxTriggers.TabIndex = 11;
            this.groupBoxTriggers.TabStop = false;
            this.groupBoxTriggers.Text = "Triggers";
            // 
            // listBoxTriggers
            // 
            this.listBoxTriggers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTriggers.FormattingEnabled = true;
            this.listBoxTriggers.Items.AddRange(new object[] {
            "When running on battery",
            "When running on current",
            "When running on external GPU",
            "When running on external screen",
            "When application starts",
            "When device status changes",
            "When a game is executed"});
            this.listBoxTriggers.Location = new System.Drawing.Point(3, 16);
            this.listBoxTriggers.Name = "listBoxTriggers";
            this.listBoxTriggers.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listBoxTriggers.Size = new System.Drawing.Size(209, 127);
            this.listBoxTriggers.TabIndex = 0;
            this.listBoxTriggers.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // groupBoxFIVR
            // 
            this.groupBoxFIVR.Controls.Add(this.numericUpDown8);
            this.groupBoxFIVR.Controls.Add(this.numericUpDown7);
            this.groupBoxFIVR.Controls.Add(this.numericUpDown6);
            this.groupBoxFIVR.Controls.Add(this.numericUpDown5);
            this.groupBoxFIVR.Controls.Add(this.label8);
            this.groupBoxFIVR.Controls.Add(this.label7);
            this.groupBoxFIVR.Controls.Add(this.label6);
            this.groupBoxFIVR.Controls.Add(this.label5);
            this.groupBoxFIVR.Location = new System.Drawing.Point(268, 323);
            this.groupBoxFIVR.Name = "groupBoxFIVR";
            this.groupBoxFIVR.Size = new System.Drawing.Size(256, 146);
            this.groupBoxFIVR.TabIndex = 10;
            this.groupBoxFIVR.TabStop = false;
            this.groupBoxFIVR.Text = "FIVR Control";
            // 
            // numericUpDown8
            // 
            this.numericUpDown8.Location = new System.Drawing.Point(166, 109);
            this.numericUpDown8.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown8.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.numericUpDown8.Name = "numericUpDown8";
            this.numericUpDown8.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown8.TabIndex = 8;
            this.numericUpDown8.ValueChanged += new System.EventHandler(this.numericUpDown8_ValueChanged);
            // 
            // numericUpDown7
            // 
            this.numericUpDown7.Location = new System.Drawing.Point(166, 80);
            this.numericUpDown7.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown7.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.numericUpDown7.Name = "numericUpDown7";
            this.numericUpDown7.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown7.TabIndex = 7;
            this.numericUpDown7.ValueChanged += new System.EventHandler(this.numericUpDown7_ValueChanged);
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.Location = new System.Drawing.Point(166, 51);
            this.numericUpDown6.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown6.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown6.TabIndex = 6;
            this.numericUpDown6.ValueChanged += new System.EventHandler(this.numericUpDown6_ValueChanged);
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.Location = new System.Drawing.Point(166, 22);
            this.numericUpDown5.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown5.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown5.TabIndex = 5;
            this.numericUpDown5.ValueChanged += new System.EventHandler(this.numericUpDown5_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Intel GPU Offset (mV)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(127, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "System Agent Offset (mV)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(118, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "CPU Cache Offset (mV)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "CPU Core Offset (mV)";
            // 
            // ProfilesList
            // 
            this.ProfilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PowerProfileHeader1});
            this.ProfilesList.ContextMenuStrip = this.SettingMenuStrip;
            this.ProfilesList.Dock = System.Windows.Forms.DockStyle.Top;
            this.ProfilesList.FullRowSelect = true;
            this.ProfilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ProfilesList.HideSelection = false;
            this.ProfilesList.Location = new System.Drawing.Point(3, 3);
            this.ProfilesList.Margin = new System.Windows.Forms.Padding(2);
            this.ProfilesList.MultiSelect = false;
            this.ProfilesList.Name = "ProfilesList";
            this.ProfilesList.Size = new System.Drawing.Size(745, 315);
            this.ProfilesList.TabIndex = 9;
            this.ProfilesList.UseCompatibleStateImageBehavior = false;
            this.ProfilesList.View = System.Windows.Forms.View.Details;
            this.ProfilesList.SelectedIndexChanged += new System.EventHandler(this.ProfilesList_SelectedIndexChanged);
            // 
            // PowerProfileHeader1
            // 
            this.PowerProfileHeader1.Text = "Name";
            this.PowerProfileHeader1.Width = 600;
            // 
            // SettingMenuStrip
            // 
            this.SettingMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemRemoveSetting,
            this.MenuItemCreateSetting});
            this.SettingMenuStrip.Name = "contextMenuStrip1";
            this.SettingMenuStrip.Size = new System.Drawing.Size(168, 48);
            // 
            // MenuItemRemoveSetting
            // 
            this.MenuItemRemoveSetting.Enabled = false;
            this.MenuItemRemoveSetting.Name = "MenuItemRemoveSetting";
            this.MenuItemRemoveSetting.Size = new System.Drawing.Size(167, 22);
            this.MenuItemRemoveSetting.Text = "Remove profile(s)";
            this.MenuItemRemoveSetting.ToolTipText = "Remove the targeted setting.";
            this.MenuItemRemoveSetting.Click += new System.EventHandler(this.MenuItemRemoveSetting_Click);
            // 
            // MenuItemCreateSetting
            // 
            this.MenuItemCreateSetting.Name = "MenuItemCreateSetting";
            this.MenuItemCreateSetting.Size = new System.Drawing.Size(167, 22);
            this.MenuItemCreateSetting.Text = "Create profile";
            this.MenuItemCreateSetting.ToolTipText = "Create a new setting.";
            this.MenuItemCreateSetting.Click += new System.EventHandler(this.MenuItemCreateSetting_Click);
            // 
            // groupBoxPowerProfile
            // 
            this.groupBoxPowerProfile.Controls.Add(this.numericUpDown4);
            this.groupBoxPowerProfile.Controls.Add(this.numericUpDown3);
            this.groupBoxPowerProfile.Controls.Add(this.numericUpDown2);
            this.groupBoxPowerProfile.Controls.Add(this.numericUpDown1);
            this.groupBoxPowerProfile.Controls.Add(this.label4);
            this.groupBoxPowerProfile.Controls.Add(this.label3);
            this.groupBoxPowerProfile.Controls.Add(this.label2);
            this.groupBoxPowerProfile.Controls.Add(this.label1);
            this.groupBoxPowerProfile.Location = new System.Drawing.Point(6, 323);
            this.groupBoxPowerProfile.Name = "groupBoxPowerProfile";
            this.groupBoxPowerProfile.Size = new System.Drawing.Size(256, 146);
            this.groupBoxPowerProfile.TabIndex = 1;
            this.groupBoxPowerProfile.TabStop = false;
            this.groupBoxPowerProfile.Text = "Power Profile";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Location = new System.Drawing.Point(167, 109);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown4.TabIndex = 4;
            this.numericUpDown4.Value = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.numericUpDown4.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(167, 80);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown3.TabIndex = 3;
            this.numericUpDown3.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(167, 51);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown2.TabIndex = 2;
            this.numericUpDown2.Value = new decimal(new int[] {
            28,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(167, 22);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(84, 20);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(105, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Power Balance GPU";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Power Balance CPU";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "TurboBoostShortPowerMax (W)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "TurboBoostLongPowerMax (W)";
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(667, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 97);
            this.button1.TabIndex = 4;
            this.button1.Text = "Open Settings Location";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 525);
            this.Controls.Add(this.tabControlSettings);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Settings_FormClosing);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.tabControlSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxGeneral.ResumeLayout(false);
            this.groupBoxGeneral.PerformLayout();
            this.tabPowerProfiles.ResumeLayout(false);
            this.groupBoxTriggers.ResumeLayout(false);
            this.groupBoxFIVR.ResumeLayout(false);
            this.groupBoxFIVR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            this.SettingMenuStrip.ResumeLayout(false);
            this.groupBoxPowerProfile.ResumeLayout(false);
            this.groupBoxPowerProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.GroupBox groupBoxGeneral;
        private System.Windows.Forms.CheckBox checkBoxMinimizeOnStartup;
        private System.Windows.Forms.TabPage tabPowerProfiles;
        private System.Windows.Forms.GroupBox groupBoxFIVR;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView ProfilesList;
        private System.Windows.Forms.ColumnHeader PowerProfileHeader1;
        private System.Windows.Forms.GroupBox groupBoxPowerProfile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.NumericUpDown numericUpDown8;
        private System.Windows.Forms.NumericUpDown numericUpDown7;
        private System.Windows.Forms.NumericUpDown numericUpDown6;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.CheckBox checkBoxMinimizeOnClosing;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxBootOnStartup;
        private System.Windows.Forms.CheckBox checkBoxToastNotifications;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxSaveOnExit;
        private System.Windows.Forms.CheckBox checkBoxMonitorProcesses;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBoxMonitorHardware;
        private System.Windows.Forms.GroupBox groupBoxTriggers;
        private System.Windows.Forms.ListBox listBoxTriggers;
        private System.Windows.Forms.CheckBox checkBoxMonitorPowerProfiles;
        private System.Windows.Forms.ContextMenuStrip SettingMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MenuItemRemoveSetting;
        private System.Windows.Forms.ToolStripMenuItem MenuItemCreateSetting;
        private System.Windows.Forms.Button button1;
    }
}