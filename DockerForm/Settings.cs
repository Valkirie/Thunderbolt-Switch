using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

                ListViewItem newProfile = new ListViewItem(new string[] { profile.ProfileName, isOnBattery.ToString(), isPluggedIn.ToString(), isExtGPU.ToString(), isOnScreen.ToString() }, profile.ProfileName);

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

            profile.Serialize();
        }

        private void ProfilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in groupBoxFIVR.Controls)
                if (ctrl.GetType() == typeof(TextBox))
                    ctrl.Text = "";

            foreach (Control ctrl in groupBoxPowerProfile.Controls)
                if (ctrl.GetType() == typeof(TextBox))
                    ctrl.Text = "";

            listBoxTriggers.SelectedIndex = -1;

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

                // Misc
                if (profile.HasLongPowerMax())
                    numericUpDown1.Value = decimal.Parse(profile.TurboBoostLongPowerMax);
                if (profile.HasShortPowerMax())
                    numericUpDown2.Value = decimal.Parse(profile.TurboBoostShortPowerMax);
                if (profile.HasPowerBalanceCPU())
                    numericUpDown3.Value = decimal.Parse(profile.PowerBalanceCPU);
                if (profile.HasPowerBalanceGPU())
                    numericUpDown4.Value = decimal.Parse(profile.PowerBalanceGPU);

                // FIVR
                if (profile.HasCPUCore())
                    numericUpDown5.Value = decimal.Parse(profile.CPUCore);
                if (profile.HasCPUCache())
                    numericUpDown6.Value = decimal.Parse(profile.CPUCache);
                if (profile.HasSystemAgent())
                    numericUpDown7.Value = decimal.Parse(profile.SystemAgent);
                if (profile.HasIntelGPU())
                    numericUpDown8.Value = decimal.Parse(profile.IntelGPU);

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
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.TurboBoostLongPowerMax = numericUpDown1.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.TurboBoostShortPowerMax = numericUpDown2.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.PowerBalanceCPU = numericUpDown3.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.PowerBalanceGPU = numericUpDown4.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.CPUCore = numericUpDown5.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.CPUCache = numericUpDown6.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.SystemAgent = numericUpDown7.Value.ToString();
            profile.Serialize();
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            if (profile == null)
                return;

            profile.IntelGPU = numericUpDown8.Value.ToString();
            profile.Serialize();
        }
    }
}
