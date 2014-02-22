using Liberatio.Agent.Service.Models;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;

namespace Liberatio.Agent.Service
{
    public partial class Service1 : ServiceBase
    {
        Timer t = new Timer();
        ServiceHost host;
        CommandsClient commandsClient = new CommandsClient();

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

            // check for registration code
            LiberatioConfiguration.RegisterIfNecessary();

            // start the timer
            string interval = LiberatioConfiguration.GetValue("inventoryInterval");
            try
            {
                if (interval.Length == 0)
                    throw new Exception("Invalid inventoryInterval. Verify that the interval is an integer.");
                t.Interval = int.Parse(interval) * 1000;

                t.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                t.Enabled = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }

            // start the console service
            host = new ServiceHost(typeof(ConsoleService),
                                            new Uri[] { new Uri("net.pipe://localhost") });
            host.AddServiceEndpoint(typeof(IConsoleService), new NetNamedPipeBinding(), "ConsoleService");
            host.Open(); // start the named pipe WCF host

            // Start listening for commands to execute.
            commandsClient.Start();
        }

        /// <summary>
        /// Stops the agent, and closes the ConsoleService named pipe
        /// </summary>
        protected override void OnStop()
        {
            // stop the timer, and close the console service host
            t.Enabled = false;
            host.Close();

            // Stop listening.
            commandsClient.Stop();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", "Performing inventory, and sending to Liberatio.com");
            Inventory i = new Inventory();
            i.Send();
        }
    }
}
