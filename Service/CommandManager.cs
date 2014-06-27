using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PusherClient;
using RestSharp.Contrib;
using System;
using System.Diagnostics;

namespace Liberatio.Agent.Service
{
    public class CommandManager
    {
        private Pusher _pusher = null;
        private Channel _cmdChannel = null;
        private PresenceChannel _presenceChannel = null;

        public CommandManager()
        {
            if (!LiberatioConfiguration.IsConnectedToInternet() || !LiberatioConfiguration.IsRegistered())
                return;

            try
            {
                string _authParams = string.Format("token={0}", LiberatioConfiguration.GetValue("communicationToken"));
                string _authUrl = "http://liberatio.herokuapp.com/pusher/auth?token=" + LiberatioConfiguration.GetValue("communicationToken"); //HttpUtility.UrlEncode(_authParams);

                EventLog.WriteEntry("LiberatioAgent", "Attempting to connect", EventLogEntryType.Information);
                _pusher = new Pusher("b64eb9cae3befce08a2f", new PusherOptions()
                {
                    Authorizer = new HttpAuthorizer(_authUrl)
                });
                _pusher.Connected += pusher_Connected;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        public void Start()
        {
            if (_pusher != null)
                _pusher.Connect();
        }

        public void Stop()
        {
            if (_pusher != null)
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

            try
            {
                // Setup private commands channel if we are told to do so.
                if (LiberatioConfiguration.UseRemoteCommands())
                {
                    EventLog.WriteEntry("LiberatioAgent", "Attempting to subscribe to private channel", EventLogEntryType.Information);
                    _cmdChannel = _pusher.Subscribe(string.Format("private-cmd_{0}", uuid));
                    _cmdChannel.Subscribed += _cmdChannel_Subscribed;
                }

                // Setup presence channel always.
                EventLog.WriteEntry("LiberatioAgent", "Attempting to subscribe to presence channel", EventLogEntryType.Information);
                _presenceChannel = (PresenceChannel)_pusher.Subscribe(string.Format("presence-cmd_{0}", uuid));
                _presenceChannel.Subscribed += _presenceChannel_Subscribed;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", "Failed to subscribe", EventLogEntryType.Error);
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Called by the presence channel when the channel connection is made.
        /// </summary>
        /// <param name="sender"></param>
        void _presenceChannel_Subscribed(object sender)
        {
            EventLog.WriteEntry("LiberatioAgent", "Subscribed to presence channel", EventLogEntryType.Information);
        }

        /// <summary>
        /// Called by the private channel when the channel connection is made.
        /// </summary>
        /// <param name="sender"></param>
        private void _cmdChannel_Subscribed(object sender)
        {
            EventLog.WriteEntry("LiberatioAgent", "Subscribed to private channel", EventLogEntryType.Information);

            _cmdChannel.Bind("commands.run", (dynamic data) =>
            {
                try
                {
                    EventLog.WriteEntry("LiberatioAgent", "Data is " + data, EventLogEntryType.Information);
                    JObject message = (JObject)JsonConvert.DeserializeObject(Convert.ToString(data));
                    JArray commands = (JArray)message["commands"];

                    foreach (JToken c in commands)
                    {
                        string s = c.ToString();

                        ProcessStartInfo i;
                        Process p;
                        string shutdownExecutable = Environment.ExpandEnvironmentVariables(@"%windir%\system32\shutdown.exe");
                        
                        string paexecExecutable = string.Format(@"{0}\paexec.exe", LiberatioConfiguration.GetApplicationDirectory());
                        string paexecArguments = string.Format("-u liberatio -p \"{0}\"", LiberatioConfiguration.GetLiberatioUserPassword());
                        string commandWithArguments = "";

                        // Determine if this is a custom command or a builtin command.
                        // Custom commands do not use the "name" property.
                        if (c["name"] != null)
                        {
                            switch (c["name"].ToString())
                            {
                                // Builtin - reboot the computer.
                                case "reboot":
                                    EventLog.WriteEntry("LiberatioAgent", "Reboot command received from Liberatio", EventLogEntryType.Information);

                                    commandWithArguments = string.Format("{0} /r /c \"Liberatio has initiated a *reboot* of your computer - you have 1 minute.\" /t 60", shutdownExecutable);

                                    i = new ProcessStartInfo(paexecExecutable, string.Format("{0} {1}", paexecArguments, commandWithArguments));
                                    i.UseShellExecute = false;
                                    i.RedirectStandardOutput = true;
                                    i.RedirectStandardError = true;

                                    p = new Process();
                                    p.StartInfo = i;
                                    p.OutputDataReceived += new DataReceivedEventHandler(CaptureOutput);
                                    p.ErrorDataReceived += new DataReceivedEventHandler(CaptureError);
                                    p.Start();
                                    p.BeginOutputReadLine();
                                    p.BeginErrorReadLine();
                                    break;

                                // Builtin - shutdown the computer.
                                case "shutdown":
                                    EventLog.WriteEntry("LiberatioAgent", "Shutdown command received from Liberatio", EventLogEntryType.Information);

                                    commandWithArguments = string.Format("{0} /s /c \"Liberatio has initiated a *shutdown* of your computer - you have 1 minute.\" /t 60", shutdownExecutable);

                                    i = new ProcessStartInfo(paexecExecutable, string.Format("{0} {1}", paexecArguments, commandWithArguments));
                                    i.UseShellExecute = false;
                                    i.RedirectStandardOutput = true;
                                    i.RedirectStandardError = true;

                                    p = new Process();
                                    p.StartInfo = i;
                                    p.OutputDataReceived += new DataReceivedEventHandler(CaptureOutput);
                                    p.ErrorDataReceived += new DataReceivedEventHandler(CaptureError);
                                    p.Start();
                                    p.BeginOutputReadLine();
                                    p.BeginErrorReadLine();
                                    break;
                            }
                        }
                        else
                        {
                            EventLog.WriteEntry("LiberatioAgent", "Custom command received from Liberatio", EventLogEntryType.Information);

                            i = new ProcessStartInfo(paexecExecutable, string.Format("{0} {1} {2}", paexecArguments, c["executable"].ToString(), c["arguments"].ToString()));
                            i.UseShellExecute = false;
                            i.RedirectStandardOutput = true;
                            i.RedirectStandardError = true;

                            p = new Process();
                            p.StartInfo = i;
                            p.OutputDataReceived += new DataReceivedEventHandler(CaptureOutput);
                            p.ErrorDataReceived += new DataReceivedEventHandler(CaptureError);
                            p.Start();
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();
                        }
                    }
                }
                catch (Exception exception)
                {
                    EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Information);
                }
            });
        }

        private void CaptureOutput(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
                EventLog.WriteEntry("LiberatioAgent", e.Data, EventLogEntryType.Information);
        }

        private void CaptureError(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
                EventLog.WriteEntry("LiberatioAgent", e.Data, EventLogEntryType.Warning);
        }

        // var triggered = _cmdChannel.trigger('client-someeventname', { your: data });
    }
}
