using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace LiberatioTray
{
    public partial class FormConsole : Form
    {
        private Configuration config;

        public FormConsole()
        {
            InitializeComponent();

            // load configuration
            config = ConfigurationManager.OpenExeConfiguration("LiberatioService.exe");
        }

        private void FormConsole_Load(object sender, EventArgs e)
        {
            txtUuid.Text = config.AppSettings["uuid"];
            txtLocation.Text = config.AppSettings["location"];
            cmbRole.SelectedText = config.AppSettings["role"];
        }

        private void btnSaveAndRestart_Click(object sender, EventArgs e)
        {
            bool shouldSave = false;

            if (config.AppSettings["uuid"] != txtUuid.Text)
            {
                config.AppSettings["uuid"] = txtUuid.Text;
                shouldSave = true;
            }

            if (config.AppSettings["location"] != txtLocation.Text)
            {
                config.AppSettings["location"] = txtLocation.Text;
                shouldSave = true;
            }

            if (config.AppSettings["role"] != cmbRole.SelectedText)
            {
                config.AppSettings["role"] = cmbRole.SelectedText;
                shouldSave = true;
            }

            // save the configuration and restart the service
            if (shouldSave)
            {
                config.Save();
                RestartService();
            }
        }

        /// <summary>
        /// Restarts the Liberatio Agent service
        /// </summary>
        private void RestartService()
        {
            ServiceController sc = new ServiceController();
            sc.ServiceName = "Liberatio Agent";

            // stop the service, wait 2 seconds, then start it
            try
            {
                sc.Stop();
                var timeout = new TimeSpan(0, 0, 2); // 2 seconds
                sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                sc.Start();
            }
            catch (InvalidOperationException)
            {

            }
        }
    }
}
