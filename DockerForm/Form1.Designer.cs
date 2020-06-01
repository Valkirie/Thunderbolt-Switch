using DockerForm.Properties;

namespace DockerForm
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findAGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticDetectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.undockedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripStartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opentoolStripStartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeTheGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.navigateToIGDBEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exittoolStripStartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.GameList = new DockerForm.exListBox();
            this.menuStrip1.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.databaseToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
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
            this.findAGameToolStripMenuItem.Text = "Find a Game";
            this.findAGameToolStripMenuItem.Click += new System.EventHandler(this.findAGameToolStripMenuItem_Click);
            // 
            // automaticDetectionToolStripMenuItem
            // 
            this.automaticDetectionToolStripMenuItem.Enabled = false;
            this.automaticDetectionToolStripMenuItem.Name = "automaticDetectionToolStripMenuItem";
            this.automaticDetectionToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.automaticDetectionToolStripMenuItem.Text = "Automatic Detection";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // menuStrip2
            // 
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.menuStrip2.Enabled = false;
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undockedToolStripMenuItem,
            this.debugTextBox});
            this.menuStrip2.Location = new System.Drawing.Point(0, 537);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip2.TabIndex = 2;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // undockedToolStripMenuItem
            // 
            this.undockedToolStripMenuItem.Image = global::DockerForm.Properties.Resources.image_plugged;
            this.undockedToolStripMenuItem.Name = "undockedToolStripMenuItem";
            this.undockedToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.undockedToolStripMenuItem.Text = "Reading...";
            // 
            // debugTextBox
            // 
            this.debugTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.debugTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.debugTextBox.Name = "debugTextBox";
            this.debugTextBox.Size = new System.Drawing.Size(400, 20);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStartItem,
            this.toolStripSeparator4,
            this.openToolStripMenuItem,
            this.opentoolStripStartItem,
            this.toolStripSeparator1,
            this.removeTheGameToolStripMenuItem,
            this.propertiesToolStripMenuItem,
            this.toolStripSeparator2,
            this.navigateToIGDBEntryToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(195, 154);
            // 
            // toolStripStartItem
            // 
            this.toolStripStartItem.Name = "toolStripStartItem";
            this.toolStripStartItem.Size = new System.Drawing.Size(194, 22);
            this.toolStripStartItem.Text = "Start";
            this.toolStripStartItem.Click += new System.EventHandler(this.toolStripStartItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(191, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.openToolStripMenuItem.Text = "Open Game location";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenGameFolder);
            // 
            // opentoolStripStartItem
            // 
            this.opentoolStripStartItem.Name = "opentoolStripStartItem";
            this.opentoolStripStartItem.Size = new System.Drawing.Size(194, 22);
            this.opentoolStripStartItem.Text = "Open Data location";
            this.opentoolStripStartItem.Click += new System.EventHandler(this.OpenDataFolder);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(191, 6);
            // 
            // removeTheGameToolStripMenuItem
            // 
            this.removeTheGameToolStripMenuItem.Name = "removeTheGameToolStripMenuItem";
            this.removeTheGameToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.removeTheGameToolStripMenuItem.Text = "Remove Title";
            this.removeTheGameToolStripMenuItem.Click += new System.EventHandler(this.removeTheGameToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(191, 6);
            // 
            // navigateToIGDBEntryToolStripMenuItem
            // 
            this.navigateToIGDBEntryToolStripMenuItem.Name = "navigateToIGDBEntryToolStripMenuItem";
            this.navigateToIGDBEntryToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
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
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showToolStripMenuItem,
            this.toolStripSeparator3,
            this.exittoolStripStartItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(104, 54);
            // 
            // showToolStripMenuItem
            // 
            this.showToolStripMenuItem.Name = "showToolStripMenuItem";
            this.showToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new System.EventHandler(this.showToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(100, 6);
            // 
            // exittoolStripStartItem
            // 
            this.exittoolStripStartItem.Name = "exittoolStripStartItem";
            this.exittoolStripStartItem.Size = new System.Drawing.Size(103, 22);
            this.exittoolStripStartItem.Text = "Exit";
            this.exittoolStripStartItem.Click += new System.EventHandler(this.exittoolStripStartItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // GameList
            // 
            this.GameList.BackColor = System.Drawing.SystemColors.Control;
            this.GameList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.GameList.ContextMenuStrip = this.contextMenuStrip1;
            this.GameList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GameList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.GameList.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.GameList.FormattingEnabled = true;
            this.GameList.Location = new System.Drawing.Point(0, 24);
            this.GameList.Name = "GameList";
            this.GameList.Size = new System.Drawing.Size(1008, 513);
            this.GameList.Sorted = true;
            this.GameList.TabIndex = 0;
            this.GameList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GameList_MouseDown);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.GameList);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.menuStrip2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private DockerForm.exListBox GameList;
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
        private System.Windows.Forms.ToolStripMenuItem opentoolStripStartItem;
        private System.Windows.Forms.ToolStripTextBox debugTextBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem showToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exittoolStripStartItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripStartItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

