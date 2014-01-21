using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace LiberatioService
{
    public partial class Service1 : ServiceBase
    {
        Timer t = new Timer();
        ServiceHost host;

        public Service1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Establishes the event log source, starts the agent,
        /// and opens the ConsoleService named pipe
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // configure logging
            if (!EventLog.SourceExists("LiberatioAgent"))
            {
                EventLog.CreateEventSource("LiberatioAgent", "Application");
                return;
            }

            // verify that there is a valid uuid
            LiberatioConfiguration.CheckOrUpdateUuid();

            // start the timer
            t.Interval = 3000;
            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            t.Enabled = true;

            // start the console service
            using (host = new ServiceHost(  typeof(ConsoleService),
                                            new Uri[]{ new Uri("net.pipe://localhost") } ))
            {
                host.AddServiceEndpoint(typeof(IConsoleService),
                                        new NetNamedPipeBinding(),
                                        "ConsoleService");
                host.Open(); // start the named pipe WCF host
            }
        }

        /// <summary>
        /// Stops the agent, and closes the ConsoleService named pipe
        /// </summary>
        protected override void OnStop()
        {
            // stop the timer, and close the console service host
            t.Enabled = false;
            host.Close();
        }

        /// <summary>
        /// Starts the console in the system tray when a user logs on, only if
        /// the user is an Administrator
        /// </summary>
        /// <param name="changeDescription"></param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch(changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    // current user
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);

                    // if user is an administrator, start the gui notification icon
                    if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                    {
                        Process p = new Process();

                        ProcessStartInfo i = new ProcessStartInfo();
                        i.FileName = "LiberatioTray.exe";
                        i.UseShellExecute = false;
                        i.UserName = identity.Name;
                        p.StartInfo = i;

                        p.Start();
                    }

                    break;
            }

            base.OnSessionChange(changeDescription);
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Inventory i = new Inventory();

            try
            {
                EventLog.WriteEntry("LiberatioAgent", "Attempt to print JSON");
                String json = JsonConvert.SerializeObject(i);
                EventLog.WriteEntry("LiberatioAgent", json, EventLogEntryType.Information);
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
