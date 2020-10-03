using ProductionReportWF.Models;
using ProductionReportWF.Services;
using System;
using System.Windows.Forms;

namespace ProductionReportWF
{
    public partial class SettingsForm : Form
    {
        private SystemSettings systemSettings { get; set; }
        public bool SavePressed { get; protected set; }
        private SystemSettingsService settingsService { get; set; }

        public SettingsForm()
        {
            InitializeComponent();
            SavePressed = false;
            settingsService = new SystemSettingsService(new CryptoService());
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            systemSettings = settingsService.GetSystemSettings();
            if (systemSettings != null)
            {
                textDatabaseAddress.Text = systemSettings.DatabaseAddress;
                textDatabaseName.Text = systemSettings.DatabaseName;
                textUserName.Text = systemSettings.UserName;
                textUserPassword.Text = systemSettings.UserPassword;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            SavePressed = false;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SavePressed = true;
            SaveSettings();
            Close();
        }

        private bool SaveSettings()
        {
            systemSettings = new SystemSettings
            {
                DatabaseAddress = textDatabaseAddress.Text,
                DatabaseName = textDatabaseName.Text,
                UserName = textUserName.Text,
                UserPassword = textUserPassword.Text
            };
            settingsService.SetSystemSettings(systemSettings);
            return !settingsService.Message.IsError;
        }
    }
}
