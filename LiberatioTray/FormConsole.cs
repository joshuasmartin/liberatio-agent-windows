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
        IConsoleService pipeProxy;

        public FormConsole()
        {
            InitializeComponent();
        }

        private void FormConsole_Load(object sender, EventArgs e)
        {
            // wcf client for the service
            ChannelFactory<IConsoleService> pipeFactory =
                new ChannelFactory<IConsoleService>(
                                                    new NetNamedPipeBinding(),
                                                    new EndpointAddress("net.pipe://localhost/ConsoleService"));

            pipeProxy = pipeFactory.CreateChannel();

            // load values from the wcf service
            txtUuid.Text = pipeProxy.GetValueFromConfiguration("uuid");
            txtLocation.Text = pipeProxy.GetValueFromConfiguration("location");
            cmbRole.Text = pipeProxy.GetValueFromConfiguration("role");

            // determine  if the uuid is registered
            if (pipeProxy.IsRegistered(txtUuid.Text))
            {
                lblRegistered.Text = "Registered";
                lblRegistered.ForeColor = Color.Green;
            }
            else
            {
                lblRegistered.Text = "Unregistered";
                lblRegistered.ForeColor = Color.Red;
            }
        }

        private void btnSaveAndRestart_Click(object sender, EventArgs e)
        {
            btnSaveAndRestart.Enabled = false;
            progressBar.Visible = true;

            // wcf client for the service
            ChannelFactory<IConsoleService> pipeFactory =
                new ChannelFactory<IConsoleService>(
                                                    new NetNamedPipeBinding(),
                                                    new EndpointAddress("net.pipe://localhost/ConsoleService"));

            pipeProxy = pipeFactory.CreateChannel();

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
    }
}
