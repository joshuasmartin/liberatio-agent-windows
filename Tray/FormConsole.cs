using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace Liberatio.Agent.Tray
{
    public delegate void LoadConfigurationDelegate(int value);
    public delegate void PopulateConfigurationDelegate(string uuid, string location, string role, string status);

    [ServiceContract]
    public interface IConsoleService
    {
        [OperationContract]
        bool UpdateConfiguration(string key, string value);

        [OperationContract]
        string GetValueFromConfiguration(string key);

        [OperationContract]
        bool IsRegistered();
    }

    public partial class FormConsole : Form
    {
        public FormConsole()
        {
            InitializeComponent();
        }

        private void FormConsole_Load(object sender, EventArgs e)
        {
            // Set the version number label text.
            string applicationFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            Version version = new Version(FileVersionInfo.GetVersionInfo(applicationFilePath).ProductVersion);
            lblVersionNumber.Text = version.ToString();

            // Load configuration on another thread.
            pictureWaiting.Visible = true;
            LoadConfigurationDelegate d = loadConfiguration;
            d.BeginInvoke(15, null, null);
        }

        void loadConfiguration(int value)
        {
            try
            {
                // wcf client to the service
                IConsoleService pipeProxy = OpenChannelToService();

                // load values from the wcf service
                string uuid = pipeProxy.GetValueFromConfiguration("uuid");
                string location = pipeProxy.GetValueFromConfiguration("location");
                string role = pipeProxy.GetValueFromConfiguration("role");

                // determine if the uuid is registered
                string status = "";
                if (pipeProxy.IsRegistered())
                    status = "Registered";

                this.Invoke(new PopulateConfigurationDelegate(populateConfiguration), new object[] { uuid, location, role, status });
            }
            catch
            {

            }
        }

        void populateConfiguration(string uuid, string location, string role, string status)
        {
            txtUuid.Text = uuid;
            txtLocation.Text = location;
            cmbRole.Text = role;

            if (status.Equals("Registered"))
            {
                lblConnectionStatusValue.Text = "Registered";
                pnlStatus.BackColor = Color.Green;
                btnConnect.Visible = false;
            }
            else
            {
                lblConnectionStatusValue.Text = "Not Registered";
                pnlStatus.BackColor = Color.Firebrick;
            }

            pictureWaiting.Visible = false;
        }

        private void btnSaveAndRestart_Click(object sender, EventArgs e)
        {
            btnSaveAndRestart.Enabled = false;

            // wcf client to the service
            IConsoleService pipeProxy = OpenChannelToService();

            // update the service configuration
            pipeProxy.UpdateConfiguration("uuid", txtUuid.Text);
            pipeProxy.UpdateConfiguration("location", txtLocation.Text);
            pipeProxy.UpdateConfiguration("role", cmbRole.Text);

            // restart the service to refresh the configuration
            RestartService();

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
