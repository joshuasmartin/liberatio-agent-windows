using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace LiberatioService
{
    public partial class Service1 : ServiceBase
    {
        Timer t = new Timer();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // configure logging
            if (!EventLog.SourceExists("LiberatioAgent"))
            {
                EventLog.CreateEventSource("LiberatioAgent", "Application");
                return;
            }

            // // generate a new uuid for the config if one does not exist
            if (ConfigurationManager.AppSettings["uuid"].Trim().Length == 0)
            {
                Guid g = Guid.NewGuid();
                EventLog.WriteEntry("Setting UUID to " + g.ToString());

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings.Remove("uuid");
                config.AppSettings.Settings.Add("uuid", g.ToString());

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }

            // start the timer
            t.Interval = 3000;

            t.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            t.Enabled = true;
        }

        protected override void OnStop()
        {
            t.Enabled = false;
        }

        // Specify what you want to happen when the Elapsed event is 
        // raised.
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
            Inventory i = new Inventory();
            i.populate();

            try
            {
                EventLog.WriteEntry("LiberatioAgent", "Attempt to print JSON");
                String json = JsonConvert.SerializeObject(i);
                EventLog.WriteEntry("LiberatioAgent", json, EventLogEntryType.Information);
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.Message, EventLogEntryType.Error);
            }
        }
    }
}
