using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace Liberatio.Agent.Tray
{
    public delegate void LoadConfigurationDelegate();
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
            //WindowsIdentity identity = WindowsIdentity.GetCurrent();
            //WindowsPrincipal principal = new WindowsPrincipal(identity);

            //if (principal.IsInRole(WindowsBuiltInRole.Administrator))
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
            d.BeginInvoke(null, null);
        }

        void loadConfiguration()
        {
            try
            {
                // Connect to the running service using WCF.
                IConsoleService pipeProxy = OpenChannelToService();

                // Load configuration values via the endpoint.
                string uuid = pipeProxy.GetValueFromConfiguration("uuid");
                string location = pipeProxy.GetValueFromConfiguration("location");
                string role = pipeProxy.GetValueFromConfiguration("role");

                string status = "";
                if (pipeProxy.IsRegistered())
                    status = "Registered";

                // Populate the user interface with the collected values.
                this.Invoke(new PopulateConfigurationDelegate(populateConfiguration), new object[] { uuid, location, role, status });
            }
            catch (EndpointNotFoundException exception)
            {
                var sc = new ServiceController("Liberatio Agent");
                var caption = "";
                var text = "";

                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    text = "Please start the service in the Control Panel or restart the computer.";
                    caption = "Liberatio Service is Not Running";
                }
                else
                {
                    EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);

                    text = exception.Message;
                    caption = "Could Not Connect to the Liberatio Service";
                }

                // Do not allow the user to continue without the service.
                if (MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    this.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Could Not Load Configuration",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        void populateConfiguration(string uuid, string location, string role, string status)
        {
            txtUuid.Text = uuid;
            txtLocation.Text = location;
            cmbRole.Text = role;

            if (status.Equals("Registered"))
            {
                lblConnectionStatus.Text = "Registered";
                lblConnectionStatus.ForeColor = Color.Green;
                pictureStatus.Image = Properties.Resources.connected;
                btnConnect.Visible = false;
            }
            else
            {
                lblConnectionStatus.Text = "Not Registered";
                lblConnectionStatus.ForeColor = Color.Firebrick;
                pictureStatus.Image = Properties.Resources.disconnected;
            }

            pictureWaiting.Visible = false;
            lblStatus.Text = "Done";
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
