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
        LiberatioCommandsClient commandsClient = new LiberatioCommandsClient();

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
            t.Interval = 10 * 1000;
            try
            {
                t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }
            t.Enabled = true;

            // start the console service
            host = new ServiceHost(typeof(ConsoleService),
                                            new Uri[] { new Uri("net.pipe://localhost") });
            host.AddServiceEndpoint(typeof(IConsoleService), new NetNamedPipeBinding(), "ConsoleService");
            host.Open(); // start the named pipe WCF host

            // start the Liberatio Commands web socket client
            //commandsClient.start();
        }

        /// <summary>
        /// Stops the agent, and closes the ConsoleService named pipe
        /// </summary>
        protected override void OnStop()
        {
            // stop the timer, and close the console service host
            t.Enabled = false;
            host.Close();

            // stop the Liberatio Commands web socket client
            //commandsClient.stop();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", "Performing inventory, and sending to Liberatio.com");
            Inventory i = new Inventory();

            try
            {
                String json = JsonConvert.SerializeObject(i, Formatting.Indented);

                EventLog.WriteEntry("LiberatioAgent", json);
            }
            catch(Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }
        }
    }
}
