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
    public class CommandsClient
    {
        private Pusher _pusher = null;
        private Channel _cmdChannel = null;

        public CommandsClient()
        {
            _pusher = new Pusher("application key");
            _pusher.Connected += pusher_Connected;
        }

        public void start()
        {
            _pusher.Connect();
        }

        public void stop()
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

            _cmdChannel = _pusher.Subscribe(string.Format("private-cmd_{0}", uuid));
            _cmdChannel.Subscribed += _cmdChannel_Subscribed;
        }

        private void _cmdChannel_Subscribed(object sender)
        {
            _cmdChannel.Bind("commands.run", (dynamic data) =>
            {
                string command = data.message;
                Console.WriteLine("[" + data.name + "] " + data.message);
            });
        }

        // var triggered = _cmdChannel.trigger('client-someeventname', { your: data });
    }
}
