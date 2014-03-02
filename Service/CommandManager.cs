using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PusherClient;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebSocket4Net;

namespace Liberatio.Agent.Service
{
    public class CommandManager
    {
        private Pusher _pusher = null;
        private Channel _cmdChannel = null;

        public CommandManager()
        {
            try
            {
                _pusher = new Pusher("b64eb9cae3befce08a2f");
                _pusher.Connected += pusher_Connected;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        public void Start()
        {
            _pusher.Connect();
        }

        public void Stop()
        {
            _pusher.Disconnect();
        }

        /// <summary>
        /// Subscribes to a private Pusher channel that enables remote
        /// desktop communication to this computer on the Website.
        /// </summary>
        /// <param name="sender"></param>
        private void pusher_Connected(object sender)
        {
            string uuid = LiberatioConfiguration.GetValue("uuid");

            _cmdChannel = _pusher.Subscribe(string.Format("cmd_{0}", uuid));
            _cmdChannel.Subscribed += _cmdChannel_Subscribed;
        }

        private void _cmdChannel_Subscribed(object sender)
        {
            _cmdChannel.Bind("commands.run", (dynamic data) =>
            {
                EventLog.WriteEntry("LiberatioAgent", "Data is " + data, EventLogEntryType.Information);
                JObject message = (JObject)JsonConvert.DeserializeObject(Convert.ToString(data));
                JArray commands = (JArray)message["commands"];

                foreach (JToken c in commands)
                {
                    string s = c.ToString();
                    EventLog.WriteEntry("LiberatioAgent", "command is " + s, EventLogEntryType.Information);
                    string path = Environment.ExpandEnvironmentVariables(@"%windir%\system32\shutdown.exe");

                    ProcessStartInfo i = null;
                    Process p = null;

                    switch (s)
                    {
                        case "reboot":
                            i = new ProcessStartInfo(path, "/r /c \"Liberatio has initiated a *reboot* of your computer - you have 1 minute.\" /t 60");
                            i.UseShellExecute = false;
                            i.RedirectStandardOutput = true;
                            i.RedirectStandardError = true;
                            p = new Process();
                            p.StartInfo = i;
                            p.OutputDataReceived += CaptureOutput;
                            p.ErrorDataReceived += CaptureError;
                            p.Start();
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();
                            p.WaitForExit();
                            break;
                        case "shutdown":
                            i = new ProcessStartInfo(path, "/s /c \"Liberatio has initiated a *shutdown* of your computer - you have 1 minute.\" /t 60");
                            i.UseShellExecute = false;
                            i.RedirectStandardOutput = true;
                            i.RedirectStandardError = true;
                            p = new Process();
                            p.StartInfo = i;
                            p.OutputDataReceived += CaptureOutput;
                            p.ErrorDataReceived += CaptureError;
                            p.Start();
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();
                            p.WaitForExit();
                            break;
                    }
                }
            });
        }

        private void CaptureOutput(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", e.Data, EventLogEntryType.Information);
        }

        private void CaptureError(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", e.Data, EventLogEntryType.Warning);
        }

        // var triggered = _cmdChannel.trigger('client-someeventname', { your: data });
    }
}
