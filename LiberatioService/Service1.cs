using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

            // attempt to discover role
            LiberatioConfiguration.DiscoverRole();

            // start the timer
            t.Interval = 10000;
            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            t.Enabled = true;

            // start the console service
            host = new ServiceHost(typeof(ConsoleService),
                                            new Uri[] { new Uri("net.pipe://localhost") });
            host.AddServiceEndpoint(typeof(IConsoleService), new NetNamedPipeBinding(), "ConsoleService");
            host.Open(); // start the named pipe WCF host
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

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Inventory i = new Inventory();

            try
            {
                EventLog.WriteEntry("LiberatioAgent", "Attempt to print JSON");
                //String json = JsonConvert.SerializeObject(i);
                //EventLog.WriteEntry("LiberatioAgent", json, EventLogEntryType.Information);
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
