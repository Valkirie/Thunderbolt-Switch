using DockerForm.Properties;

namespace DockerForm
{
    partial class MainForm
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findAGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UniversalMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.microsoftStoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.battleNetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.steamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.undockedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripStartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeTheGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.navigateToIGDBEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exittoolStripStartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.GameListView = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDev = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlayed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSettings = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProfiles = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.styleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syleIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.styleDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.styleTileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.CurrentTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(924, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findAGameToolStripMenuItem,
            this.automaticDetectionToolStripMenuItem});
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.databaseToolStripMenuItem.Text = "Database";
            // 
            // findAGameToolStripMenuItem
            // 
            this.findAGameToolStripMenuItem.Name = "findAGameToolStripMenuItem";
            this.findAGameToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.findAGameToolStripMenuItem.Text = "Select Manually";
            this.findAGameToolStripMenuItem.ToolTipText = "Select a game to add to your library.";
            this.findAGameToolStripMenuItem.Click += new System.EventHandler(this.findAGameToolStripMenuItem_Click);
            // 
            // automaticDetectionToolStripMenuItem
            // 
            this.automaticDetectionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UniversalMenuItem3,
            this.toolStripSeparator5,
            this.microsoftStoreToolStripMenuItem,
            this.battleNetToolStripMenuItem,
            this.steamToolStripMenuItem});
            this.automaticDetectionToolStripMenuItem.Name = "automaticDetectionToolStripMenuItem";
            this.automaticDetectionToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.automaticDetectionToolStripMenuItem.Text = "Automatic Detection";
            this.automaticDetectionToolStripMenuItem.ToolTipText = "We will search all supported games and allow you to select two that you wish to a" +
    "dd.";
            // 
            // UniversalMenuItem3
            // 
            this.UniversalMenuItem3.Name = "UniversalMenuItem3";
            this.UniversalMenuItem3.Size = new System.Drawing.Size(155, 22);
            this.UniversalMenuItem3.Text = "Universal";
            this.UniversalMenuItem3.Click += new System.EventHandler(this.UniversalMenuItem3_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(152, 6);
            // 
            // microsoftStoreToolStripMenuItem
            // 
            this.microsoftStoreToolStripMenuItem.Name = "microsoftStoreToolStripMenuItem";
            this.microsoftStoreToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.microsoftStoreToolStripMenuItem.Text = "Microsoft Store";
            this.microsoftStoreToolStripMenuItem.Click += new System.EventHandler(this.microsoftStoreToolStripMenuItem_Click);
            // 
            // battleNetToolStripMenuItem
            // 
            this.battleNetToolStripMenuItem.Name = "battleNetToolStripMenuItem";
            this.battleNetToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.battleNetToolStripMenuItem.Text = "Battle.net";
            this.battleNetToolStripMenuItem.Click += new System.EventHandler(this.battleNetToolStripMenuItem_Click);
            // 
            // steamToolStripMenuItem
            // 
            this.steamToolStripMenuItem.Name = "steamToolStripMenuItem";
            this.steamToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.steamToolStripMenuItem.Text = "Steam";
            this.steamToolStripMenuItem.Click += new System.EventHandler(this.steamToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // menuStrip2
            // 
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undockedToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(0, 559);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(924, 32);
            this.menuStrip2.TabIndex = 2;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // undockedToolStripMenuItem
            // 
            this.undockedToolStripMenuItem.Image = global::DockerForm.Properties.Resources.intel;
            this.undockedToolStripMenuItem.Name = "undockedToolStripMenuItem";
            this.undockedToolStripMenuItem.Size = new System.Drawing.Size(95, 28);
            this.undockedToolStripMenuItem.Text = "Reading...";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStartItem,
            this.toolStripSeparator4,
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.removeTheGameToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.toolStripSeparator2,
            this.navigateToIGDBEntryToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(198, 154);
            // 
            // toolStripStartItem
            // 
            this.toolStripStartItem.Name = "toolStripStartItem";
            this.toolStripStartItem.Size = new System.Drawing.Size(197, 22);
            this.toolStripStartItem.Text = "Start";
            this.toolStripStartItem.Click += new System.EventHandler(this.toolStripStartItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(194, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.openToolStripMenuItem.Text = "Open Game location";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenGameFolder);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 22);
            this.toolStripMenuItem1.Text = "Open Settings Location";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // removeTheGameToolStripMenuItem
            // 
            this.removeTheGameToolStripMenuItem.Name = "removeTheGameToolStripMenuItem";
            this.removeTheGameToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.removeTheGameToolStripMenuItem.Text = "Remove Title";
            this.removeTheGameToolStripMenuItem.Click += new System.EventHandler(this.removeTheGameToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(194, 6);
            // 
            // navigateToIGDBEntryToolStripMenuItem
            // 
            this.navigateToIGDBEntryToolStripMenuItem.Name = "navigateToIGDBEntryToolStripMenuItem";
            this.navigateToIGDBEntryToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.navigateToIGDBEntryToolStripMenuItem.Text = "Navigate to IGDB entry";
            this.navigateToIGDBEntryToolStripMenuItem.Click += new System.EventHandler(this.navigateToIGDBEntryToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipTitle = "Thunderbolt Switch";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip2;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Thunderbolt Switch";
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripSeparator3,
            this.exittoolStripStartItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(145, 54);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(144, 22);
            this.toolStripMenuItem2.Text = "Power Profile";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
            // 
            // exittoolStripStartItem
            // 
            this.exittoolStripStartItem.Name = "exittoolStripStartItem";
            this.exittoolStripStartItem.Size = new System.Drawing.Size(144, 22);
            this.exittoolStripStartItem.Text = "Exit";
            this.exittoolStripStartItem.Click += new System.EventHandler(this.exittoolStripStartItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // GameListView
            // 
            this.GameListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.GameListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnDev,
            this.columnVersion,
            this.columnPlayed,
            this.columnSettings,
            this.columnProfiles});
            this.GameListView.ContextMenuStrip = this.contextMenuStrip3;
            this.GameListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameListView.FullRowSelect = true;
            this.GameListView.HideSelection = false;
            this.GameListView.LargeImageList = this.imageList1;
            this.GameListView.Location = new System.Drawing.Point(0, 24);
            this.GameListView.Margin = new System.Windows.Forms.Padding(2);
            this.GameListView.MultiSelect = false;
            this.GameListView.Name = "GameListView";
            this.GameListView.Size = new System.Drawing.Size(924, 535);
            this.GameListView.SmallImageList = this.imageList2;
            this.GameListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.GameListView.TabIndex = 9;
            this.GameListView.UseCompatibleStateImageBehavior = false;
            this.GameListView.View = System.Windows.Forms.View.Tile;
            this.GameListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.GameListView_HeaderClicked);
            this.GameListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GameListView_DoubleClick);
            this.GameListView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GameListView_Clicked);
            // 
            // columnName
            // 
            this.columnName.Text = "";
            this.columnName.Width = 100;
            // 
            // columnDev
            // 
            this.columnDev.Text = "Developer";
            this.columnDev.Width = 200;
            // 
            // columnVersion
            // 
            this.columnVersion.Text = "Version";
            this.columnVersion.Width = 100;
            // 
            // columnPlayed
            // 
            this.columnPlayed.Text = "Last played";
            this.columnPlayed.Width = 140;
            // 
            // columnSettings
            // 
            this.columnSettings.Text = "Settings";
            this.columnSettings.Width = 150;
            // 
            // columnProfiles
            // 
            this.columnProfiles.Text = "Profiles";
            this.columnProfiles.Width = 150;
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.styleToolStripMenuItem,
            this.syleIconsToolStripMenuItem,
            this.styleToolStripMenuItem1,
            this.styleDetailsToolStripMenuItem,
            this.styleTileToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(166, 114);
            this.contextMenuStrip3.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip3_Opening);
            // 
            // styleToolStripMenuItem
            // 
            this.styleToolStripMenuItem.Name = "styleToolStripMenuItem";
            this.styleToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.styleToolStripMenuItem.Text = "Style: List";
            this.styleToolStripMenuItem.Click += new System.EventHandler(this.styleToolStripMenuItem_Click);
            // 
            // syleIconsToolStripMenuItem
            // 
            this.syleIconsToolStripMenuItem.Name = "syleIconsToolStripMenuItem";
            this.syleIconsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.syleIconsToolStripMenuItem.Text = "Style: Large Icons";
            this.syleIconsToolStripMenuItem.Click += new System.EventHandler(this.syleIconsToolStripMenuItem_Click);
            // 
            // styleToolStripMenuItem1
            // 
            this.styleToolStripMenuItem1.Name = "styleToolStripMenuItem1";
            this.styleToolStripMenuItem1.Size = new System.Drawing.Size(165, 22);
            this.styleToolStripMenuItem1.Text = "Style: Small Icons";
            this.styleToolStripMenuItem1.Click += new System.EventHandler(this.styleToolStripMenuItem1_Click);
            // 
            // styleDetailsToolStripMenuItem
            // 
            this.styleDetailsToolStripMenuItem.Name = "styleDetailsToolStripMenuItem";
            this.styleDetailsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.styleDetailsToolStripMenuItem.Text = "Style: Details";
            this.styleDetailsToolStripMenuItem.Click += new System.EventHandler(this.styleDetailsToolStripMenuItem_Click);
            // 
            // styleTileToolStripMenuItem
            // 
            this.styleTileToolStripMenuItem.Name = "styleTileToolStripMenuItem";
            this.styleTileToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.styleTileToolStripMenuItem.Text = "Style: Tile";
            this.styleTileToolStripMenuItem.Click += new System.EventHandler(this.styleTileToolStripMenuItem_Click);
            // 
            // imageList2
            // 
            this.imageList2.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // CurrentTimer
            // 
            this.CurrentTimer.Enabled = true;
            this.CurrentTimer.Interval = 1000;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(924, 591);
            this.Controls.Add(this.GameListView);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.menuStrip2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Thunderbolt Switch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem undockedToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeTheGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automaticDetectionToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem navigateToIGDBEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exittoolStripStartItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripStartItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem microsoftStoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem battleNetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ListView GameListView;
        private System.Windows.Forms.ColumnHeader columnDev;
        private System.Windows.Forms.ColumnHeader columnVersion;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnPlayed;
        private System.Windows.Forms.ColumnHeader columnSettings;
        private System.Windows.Forms.ToolStripMenuItem steamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UniversalMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syleIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem styleToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem styleDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem styleTileToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnProfiles;
        public System.Windows.Forms.Timer CurrentTimer;
    }
}

