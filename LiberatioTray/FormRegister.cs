using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace LiberatioTray
{
    public partial class FormRegister : Form
    {
        public FormRegister()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            // wcf client to the service
            IConsoleService pipeProxy = OpenChannelToService();

            pipeProxy.UpdateConfiguration("registrationCode", txtCode.Text);
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
