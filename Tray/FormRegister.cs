using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Liberatio.Agent.Tray
{
    public partial class FormRegister : Form
    {
        public FormRegister()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;

            // wcf client to the service
            IConsoleService pipeProxy = OpenChannelToService();

            pipeProxy.UpdateConfiguration("registrationCode", txtCode.Text);

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

            this.Dispose(); // hide and dispose
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
    }
}
