using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DockerForm
{
    public partial class Settings : Form
    {
        PowerProfile profile;
        bool Initialized;

        public Settings()
        {
            InitializeComponent();
            checkBoxMinimizeOnStartup.Checked = Properties.Settings.Default.MinimizeOnStartup;
            checkBoxMinimizeOnClosing.Checked = Properties.Settings.Default.MinimizeOnClosing;
            checkBoxBootOnStartup.Checked = Properties.Settings.Default.BootOnStartup;
            checkBoxToastNotifications.Checked = Properties.Settings.Default.ToastNotifications;
            checkBoxMonitorProcesses.Checked = Properties.Settings.Default.MonitorProcesses;
            checkBoxSaveOnExit.Checked = Properties.Settings.Default.SaveOnExit;
            checkBoxMonitorHardware.Checked = Properties.Settings.Default.MonitorHardware;
            checkBoxMonitorPowerProfiles.Checked = Properties.Settings.Default.MonitorProfiles;
            Initialized = true;
        }

        private void checkBoxPowerSpecific_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeOnStartup = checkBoxMinimizeOnStartup.Checked;
            SaveSettings();
        }

        private void checkBoxMinimizeOnClosing_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeOnClosing = checkBoxMinimizeOnClosing.Checked;
            SaveSettings();
        }

        private void checkBoxBootOnStartup_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.BootOnStartup = checkBoxBootOnStartup.Checked;
            SaveSettings();
        }

        private void checkBoxToastNotifications_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ToastNotifications = checkBoxToastNotifications.Checked;
            SaveSettings();
        }

        private void checkBoxMonitorProcesses_CheckedChanged(object sender, EventArgs e)
        {
            if(Initialized)
                MessageBox.Show("Restarting the application is required to update this setting.");

            Properties.Settings.Default.MonitorProcesses = checkBoxMonitorProcesses.Checked;
            SaveSettings();
        }

        private void checkBoxSaveOnExit_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SaveOnExit = checkBoxSaveOnExit.Checked;
            SaveSettings();
        }

        private void checkBoxMonitorHardware_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MonitorHardware = checkBoxMonitorHardware.Checked;
            SaveSettings();
        }

        private void checkBoxMonitorPowerProfiles_CheckedChanged(object sender, EventArgs e)
        {
            if(Initialized)
                MessageBox.Show("Restarting the application is required to update this setting.");

            Properties.Settings.Default.MonitorProfiles = checkBoxMonitorPowerProfiles.Checked;
            SaveSettings();
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.Save();
            Form1.UpdateSettings();
        }

        private void Settings_FormClosing(Object sender, FormClosingEventArgs e)
        {
            foreach (ListViewItem item in ProfilesList.SelectedItems)
            {
                string ProfileName = item.SubItems[0].Text;
                if (Form1.ProfileDB.ContainsKey(ProfileName))
                {
                    PowerProfile profile = Form1.ProfileDB[ProfileName];
                    if (profile.JustCreated)
                        profile.RunMe = Form1.CanRunProfile(profile, false);
                    profile.Serialize();
                }
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            foreach (CheckBox ctrl in groupBoxGeneral.Controls.Cast<Control>().Concat(groupBox1.Controls.Cast<Control>()).Concat(groupBox2.Controls.Cast<Control>()).Where(a => a.GetType() == typeof(CheckBox)))
            {
                if(toolTip1.GetToolTip(ctrl).Equals(""))
                    toolTip1.SetToolTip(ctrl, ctrl.Text);
            }

            foreach (PowerProfile profile in Form1.ProfileDB.Values)
            {
                bool isOnBattery = profile._ApplyMask.HasFlag(ProfileMask.OnBattery);
                bool isPluggedIn = profile._ApplyMask.HasFlag(ProfileMask.PluggedIn);
                bool isExtGPU = profile._ApplyMask.HasFlag(ProfileMask.ExternalGPU);
                bool isOnBoot = profile._ApplyMask.HasFlag(ProfileMask.OnStartup);
                bool isOnStatusChange = profile._ApplyMask.HasFlag(ProfileMask.OnStatusChange);
                bool isOnScreen = profile._ApplyMask.HasFlag(ProfileMask.ExternalScreen);
                bool isGameBounds = profile._ApplyMask.HasFlag(ProfileMask.GameBounds);

                ListViewItem newProfile = new ListViewItem(new string[] { profile.ProfileName }, profile.ProfileName);

                // skip default
                if (profile.ApplyPriority == -1)
                    continue;

                ProfilesList.Items.Add(newProfile);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            // reset before updating
            profile._ApplyMask = 0;

            if (listBoxTriggers.GetSelected(0))
                profile._ApplyMask |= ProfileMask.OnBattery;
            if (listBoxTriggers.GetSelected(1))
                profile._ApplyMask |= ProfileMask.PluggedIn;
            if (listBoxTriggers.GetSelected(2))
                profile._ApplyMask |= ProfileMask.ExternalGPU;
            if (listBoxTriggers.GetSelected(3))
                profile._ApplyMask |= ProfileMask.ExternalScreen;
            if (listBoxTriggers.GetSelected(4))
                profile._ApplyMask |= ProfileMask.OnStartup;
            if (listBoxTriggers.GetSelected(5))
                profile._ApplyMask |= ProfileMask.OnStatusChange;
            if (listBoxTriggers.GetSelected(6))
                profile._ApplyMask |= ProfileMask.GameBounds;
        }

        private void ProfilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            MenuItemRemoveSetting.Enabled = false;
            foreach (ListViewItem item in ProfilesList.SelectedItems)
            {
                string ProfileName = item.SubItems[0].Text;
                if (Form1.ProfileDB.ContainsKey(ProfileName))
                    MenuItemRemoveSetting.Enabled = true;
            }

            foreach (Control ctrl in groupBoxFIVR.Controls)
                if (ctrl.GetType() == typeof(TextBox))
                    ctrl.Text = "";

            foreach (Control ctrl in groupBoxPowerProfile.Controls)
                if (ctrl.GetType() == typeof(TextBox))
                    ctrl.Text = "";

            foreach (ListViewItem item in ProfilesList.SelectedItems)
            {
                string ProfileName = item.SubItems[0].Text;
                profile = Form1.ProfileDB[ProfileName];

                bool isOnBattery = profile._ApplyMask.HasFlag(ProfileMask.OnBattery);
                bool isPluggedIn = profile._ApplyMask.HasFlag(ProfileMask.PluggedIn);
                bool isExtGPU = profile._ApplyMask.HasFlag(ProfileMask.ExternalGPU);
                bool isOnScreen = profile._ApplyMask.HasFlag(ProfileMask.ExternalScreen);
                bool isOnBoot = profile._ApplyMask.HasFlag(ProfileMask.OnStartup);
                bool isOnStatusChange = profile._ApplyMask.HasFlag(ProfileMask.OnStatusChange);
                bool isGameBounds = profile._ApplyMask.HasFlag(ProfileMask.GameBounds);

                // Misc
                numericUpDown1.Value = profile.HasLongPowerMax() ? decimal.Parse(profile.TurboBoostLongPowerMax) : 0;
                numericUpDown2.Value = profile.HasShortPowerMax() ? decimal.Parse(profile.TurboBoostShortPowerMax) : 0;
                numericUpDown3.Value = profile.HasPowerBalanceCPU() ? decimal.Parse(profile.PowerBalanceCPU) : 9;
                numericUpDown4.Value = profile.HasPowerBalanceGPU() ? decimal.Parse(profile.PowerBalanceGPU) : 13;

                // FIVR
                numericUpDown5.Value = profile.HasCPUCore() ? decimal.Parse(profile.CPUCore) : 0;
                numericUpDown6.Value = profile.HasCPUCache() ? decimal.Parse(profile.CPUCache) : 0;
                numericUpDown7.Value = profile.HasSystemAgent() ? decimal.Parse(profile.SystemAgent) : 0;
                numericUpDown8.Value = profile.HasIntelGPU() ? decimal.Parse(profile.IntelGPU) : 0;

                // Triggers
                listBoxTriggers.ClearSelected();
                if (isOnBattery)
                    listBoxTriggers.SetSelected(0, true);
                if (isPluggedIn)
                    listBoxTriggers.SetSelected(1, true);
                if (isExtGPU)
                    listBoxTriggers.SetSelected(2, true);
                if (isOnScreen)
                    listBoxTriggers.SetSelected(3, true);
                if (isOnBoot)
                    listBoxTriggers.SetSelected(4, true);
                if (isOnStatusChange)
                    listBoxTriggers.SetSelected(5, true);
                if (isGameBounds)
                    listBoxTriggers.SetSelected(6, true);
                UpdateProfile();
            }
        }

        private void UpdateProfile()
        {
            Form1.ProfileDB[profile.ProfileName] = profile;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.TurboBoostLongPowerMax = numericUpDown1.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.TurboBoostShortPowerMax = numericUpDown2.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.PowerBalanceCPU = numericUpDown3.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.PowerBalanceGPU = numericUpDown4.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.CPUCore = numericUpDown5.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.CPUCache = numericUpDown6.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.SystemAgent = numericUpDown7.Value.ToString();
            UpdateProfile();
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.IntelGPU = numericUpDown8.Value.ToString();
            UpdateProfile();
        }

        private void MenuItemRemoveSetting_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in ProfilesList.SelectedItems)
            {
                string ProfileName = item.SubItems[0].Text;

                ProfilesList.Items.Remove(item);
                if (Form1.ProfileDB.ContainsKey(ProfileName))
                {
                    PowerProfile profile = Form1.ProfileDB[ProfileName];
                    profile.Remove();
                }
            }
        }

        private void MenuItemCreateSetting_Click(object sender, EventArgs e)
        {
            string ProfileName = Interaction.InputBox("Please make sure profile name is not already used.", "New Power Profile");

            if (ProfileName == "")
            {
                MessageBox.Show("Profile name can't be empty.");
                return;
            }
            else if (Form1.ProfileDB.ContainsKey(ProfileName))
            {
                MessageBox.Show("Profile name has to be unique.");
                return;
            }

            PowerProfile pP = new PowerProfile(ProfileName);
            Form1.ProfileDB[ProfileName] = pP;

            ListViewItem newProfile = new ListViewItem(new string[] { pP.ProfileName }, pP.ProfileName);
            ProfilesList.Items.Add(newProfile);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
            FileInfo fileinfo = new FileInfo(path);
            Process.Start(fileinfo.DirectoryName);
        }
    }
}
