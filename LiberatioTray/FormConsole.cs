using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace LiberatioTray
{
    [ServiceContract]
    public interface IConsoleService
    {
        [OperationContract]
        bool UpdateConfiguration(string key, string value);

        [OperationContract]
        string GetValueFromConfiguration(string key);

        [OperationContract]
        bool IsRegistered(string uuid);
    }

    public partial class FormConsole : Form
    {
        public FormConsole()
        {
            InitializeComponent();
        }

        private void FormConsole_Load(object sender, EventArgs e)
        {
            // wcf client to the service
            IConsoleService pipeProxy = OpenChannelToService();

            // load values from the wcf service
            txtUuid.Text = pipeProxy.GetValueFromConfiguration("uuid");
            txtLocation.Text = pipeProxy.GetValueFromConfiguration("location");
            cmbRole.Text = pipeProxy.GetValueFromConfiguration("role");

            ReportIsRegistered();
        }

        private void btnSaveAndRestart_Click(object sender, EventArgs e)
        {
            btnSaveAndRestart.Enabled = false;
            progressBar.Visible = true;

            // wcf client to the service
            IConsoleService pipeProxy = OpenChannelToService();

            // update the service configuration
            pipeProxy.UpdateConfiguration("uuid", txtUuid.Text);
            pipeProxy.UpdateConfiguration("location", txtLocation.Text);
            pipeProxy.UpdateConfiguration("role", cmbRole.Text);

            // restart the service to refresh the configuration
            RestartService();

            progressBar.Visible = false;
            btnSaveAndRestart.Enabled = true;

            MessageBox.Show("Settings have been saved, and the service has been restarted.", "Success",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information);
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

        private void ReportIsRegistered()
        {
            // wcf client to the service
            IConsoleService pipeProxy = OpenChannelToService();

            // determine if the uuid is registered
            if (pipeProxy.IsRegistered(txtUuid.Text))
            {
                lblConnectionStatusValue.Text = "Registered";
                lblConnectionStatusValue.ForeColor = Color.Green;
                btnConnect.Visible = false;
            }
            else
            {
                lblConnectionStatusValue.Text = "Unregistered";
                lblConnectionStatusValue.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Opens a named-pipe WCF client connection to
        /// the Liberatio Agent service.
        /// </summary>
        /// <returns></returns>
        private IConsoleService OpenChannelToService()
        {
            ChannelFactory<IConsoleService> pipeFactory =
                new ChannelFactory<IConsoleService>(
                                                    new NetNamedPipeBinding(),
                                                    new EndpointAddress("net.pipe://localhost/ConsoleService"));
            return pipeFactory.CreateChannel();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            new FormRegister().Show(this);
        }

        private void btnSaveAndRestart_Enter(object sender, EventArgs e)
        {
            ToolTip t = new ToolTip();
            t.SetToolTip(btnSaveAndRestart, "Saves your changes, and restarts the service.");
        }
    }
}
