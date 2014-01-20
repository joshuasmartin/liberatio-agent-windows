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
        /// Updates the configuration file for the LiberatioService
        /// </summary>
        /// <param name="pathToValue"></param>
        /// <param name="newValue"></param>
        private void UpdateConfiguration(String pathToValue, String newValue)
        {
            using (TransactionScope transactionScope = new TransactionScope())
            {
                XmlDocument configFile = new XmlDocument();

                configFile.Load("LiberatioService.exe.config");

                XPathNavigator fileNavigator = configFile.CreateNavigator();

                // User recursive function to get to the correct node and set the value
                WriteValueToConfigFile(fileNavigator, pathToValue, newValue);

                configFile.Save("LiberatioService.exe.config");

                // Commit transaction
                transactionScope.Complete();
            }
        }

        private void WriteValueToConfigFile(XPathNavigator fileNavigator, string remainingPath, string newValue)
        {
            string[] splittedXPath = remainingPath.Split(new[] { '/' }, 2);
            if (splittedXPath.Length == 0 || String.IsNullOrEmpty(remainingPath))
            {
                throw new Exception("Path incorrect.");
            }

            string xPathPart = splittedXPath[0];
            XPathNavigator nodeNavigator = fileNavigator.SelectSingleNode(xPathPart);

            if (splittedXPath.Length > 1)
            {
                // Recursion
                WriteValueToConfigFile(nodeNavigator, splittedXPath[1], newValue);
            }
            else
            {
                nodeNavigator.SetValue(newValue ?? String.Empty);
            }
        }
    }
}
