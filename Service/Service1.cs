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
        private Timer _timerInventory = new Timer();
        private Timer _timerWindowsUpdates = new Timer();
        private Timer _timerUpdateEngine = new Timer();
        private Timer _timerSystemCleanup = new Timer();
        private ServiceHost _host;
        private CommandManager _commandManager = new CommandManager();

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
            // Create Windows Event Source if it doesn't exist.
            if (!EventLog.SourceExists("LiberatioAgent"))
            {
                EventLog.CreateEventSource("LiberatioAgent", "Application");
                return;
            }

            LiberatioConfiguration.CreateOrUpdateLiberatioUser();

            // Verify that there is a valid uuid.
            LiberatioConfiguration.CheckOrUpdateUuid();

            // Attempt to discover role.
            LiberatioConfiguration.DiscoverRole();

            // Check for a registration code.
            LiberatioConfiguration.RegisterIfNecessary();

            // Start the TriggerInventory timer.
            try
            {
                // Perform inventory according to the interval specified.
                var interval = LiberatioConfiguration.GetValue("inventoryIntervalInSeconds");
                if ((interval.Length == 0) || (int.Parse(interval) < 30))
                    throw new Exception("Invalid inventoryInterval. Verify that the interval is an integer.");
                _timerInventory.Interval = int.Parse(interval) * 1000;

                _timerInventory.Elapsed += new ElapsedEventHandler(TriggerInventory);
                _timerInventory.Enabled = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }

            // Start the TriggerUpdateManger timer.
            try
            {
                // Perform any setup before using UpdateManager.
                WindowsUpdater.Setup();

                // Check for needed updates every 60 seconds.
                _timerWindowsUpdates.Interval = 60 * 1000;

                _timerWindowsUpdates.Elapsed += new ElapsedEventHandler(TriggerUpdateManger);
                _timerWindowsUpdates.Enabled = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }

            // Start the TriggerUpdateCheck timer.
            try
            {
                // Check for updates every 60 seconds.
                _timerUpdateEngine.Interval = 60 * 1000;

                _timerUpdateEngine.Elapsed += new ElapsedEventHandler(TriggerUpdateCheck);
                _timerUpdateEngine.Enabled = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }

            // Start the TriggerSystemCleanup timer.
            try
            {
                // Perform a system cleanup every 12 hours.
                _timerSystemCleanup.Interval = 12 * 60 * 60 * 1000;

                _timerSystemCleanup.Elapsed += new ElapsedEventHandler(TriggerSystemCleanup);
                _timerSystemCleanup.Enabled = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }

            // Start the ConsoleService WCF host.
            _host = new ServiceHost(typeof(ConsoleService),
                                            new Uri[] { new Uri("net.pipe://localhost") });
            _host.AddServiceEndpoint(typeof(IConsoleService), new NetNamedPipeBinding(), "ConsoleService");
            _host.Open();

            // Start listening for commands to execute.
            _commandManager.Start();
        }

        /// <summary>
        /// Stops the agent, and closes the ConsoleService named pipe
        /// </summary>
        protected override void OnStop()
        {
            // Stop the timers and close the WCF host.
            _timerInventory.Enabled = false;
            _timerWindowsUpdates.Enabled = false;
            _timerUpdateEngine.Enabled = false;

            _host.Close();

            // Stop the Remote Commands Manager.
            _commandManager.Stop();
        }

        private void TriggerInventory(object source, ElapsedEventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", "Performing inventory, and sending to Liberatio.com");
            Inventory i = new Inventory();
            i.Send();
        }

        private void TriggerUpdateManger(object source, ElapsedEventArgs e)
        {
            WindowsUpdater.GetInstalled();
            WindowsUpdater.GetNeeded();
        }

        private void TriggerUpdateCheck(object source, ElapsedEventArgs e)
        {
            UpdateEngine.Update();
        }

        private void TriggerSystemCleanup(object source, ElapsedEventArgs e)
        {
            var cleaner = new Cleaner();
            cleaner.Start();
        }
    }
}
