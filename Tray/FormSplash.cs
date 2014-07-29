using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;

namespace Liberatio.Agent.Tray
{
    public partial class FormSplash : Form
    {
        private BackgroundWorker backgroundWorker = new BackgroundWorker();

        public FormSplash()
        {
            InitializeComponent();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        private void FormSplash_Shown(object sender, EventArgs e)
        {
            lblStatus.Text = "Connecting to Service";

            if (backgroundWorker.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (worker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                // Wait for the Liberatio service to become available.
                ServiceController sc = new ServiceController();
                sc.ServiceName = "LiberatioServiceExe";

                try
                {
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch (InvalidOperationException) { }
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                //resultLabel.Text = "Canceled!";
            }
            else if (e.Error != null)
            {
                //resultLabel.Text = "Error: " + e.Error.Message;
            }
            else
            {
                lblStatus.Text = "Connected";

                new FormConsole().Show();
                this.Dispose();
            }
        }
    }
}
