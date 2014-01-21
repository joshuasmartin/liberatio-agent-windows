using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

namespace LiberatioTray
{
    public partial class FormConsole : Form
    {

        public FormConsole()
        {
            InitializeComponent();
        }

        private void FormConsole_Load(object sender, EventArgs e)
        {
            // load the service configuration file
            XmlDocument doc = new XmlDocument();
            doc.Load("LiberatioService.exe.config");
            XmlNode root = doc.DocumentElement;

            XmlNode nodeUuid = root.SelectSingleNode("appSettings/add[@key='uuid']");
            txtUuid.Text = nodeUuid.Attributes["value"].Value;

            XmlNode nodeLocation = root.SelectSingleNode("appSettings/add[@key='location']");
            txtLocation.Text = nodeLocation.Attributes["value"].Value;

            XmlNode nodeRole = root.SelectSingleNode("appSettings/add[@key='role']");
            cmbRole.Text = nodeRole.Attributes["value"].Value;

            // determine  if the uuid is registered
            if (IsRegistered(txtUuid.Text))
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

            // update the service configuration file
            XmlDocument doc = new XmlDocument();
            doc.Load("LiberatioService.exe.config");
            XmlNode root = doc.DocumentElement;

            XmlNode nodeUuid = root.SelectSingleNode("appSettings/add[@key='uuid']");
            nodeUuid.Attributes["value"].Value = txtUuid.Text;

            XmlNode nodeLocation = root.SelectSingleNode("appSettings/add[@key='location']");
            nodeLocation.Attributes["value"].Value = txtLocation.Text;

            XmlNode nodeRole = root.SelectSingleNode("appSettings/add[@key='role']");
            nodeRole.Attributes["value"].Value = cmbRole.Text;

            doc.Save("LiberatioService.exe.config");

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

        /// <summary>
        /// Connects to the Liberatio Website to determine if
        /// the given uuid is registered
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        private bool IsRegistered(String uuid)
        {
            var client = new RestClient("http://liberatio.herokuapp.com");
            var request = new RestRequest("nodes/{id}", Method.GET);

            // execute the request
            RestResponse response = (RestResponse)client.Execute(request);
            var content = response.Content; // raw content as string

            return true;
        }
    }
}
