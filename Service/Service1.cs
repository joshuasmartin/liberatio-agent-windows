using Liberatio.Agent.Service.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace Liberatio.Agent.Service
{
    public partial class Service1 : ServiceBase
    {
        Timer t = new Timer();
        ServiceHost host;
        CommandManager commandManager = new CommandManager();

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
                var interval = LiberatioConfiguration.GetValue("inventoryInterval");
                if ((interval.Length == 0) || (int.Parse(interval) < 30))
                    throw new Exception("Invalid inventoryInterval. Verify that the interval is an integer.");
                t.Interval = int.Parse(interval) * 1000;

                t.Elapsed += new ElapsedEventHandler(TriggerInventory);
                t.Enabled = true;
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
                UpdateManager.Setup();

                // Check for needed updates every 60 minutes.
                t.Interval = 60 * 1000;

                t.Elapsed += new ElapsedEventHandler(TriggerUpdateManger);
                t.Enabled = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }

            // Start the TriggerUpdateCheck timer.
            try
            {
                // Check for updates every 60 minutes.
                t.Interval = 60 * 1000;

                t.Elapsed += new ElapsedEventHandler(TriggerUpdateCheck);
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
            commandManager.Start();
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
            commandManager.Stop();
        }

        private void TriggerInventory(object source, ElapsedEventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", "Performing inventory, and sending to Liberatio.com");
            Inventory i = new Inventory();
            i.Send();
        }

        private void TriggerUpdateManger(object source, ElapsedEventArgs e)
        {
            UpdateManager.GetInstalled();
            UpdateManager.GetNeeded();
        }

        private void TriggerUpdateCheck(object source, ElapsedEventArgs e)
        {
            LiberatioConfiguration.CheckForUpdates();
        }
    }
}
