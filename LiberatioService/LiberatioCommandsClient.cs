using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WebSocket4Net;

namespace LiberatioService
{
    class LiberatioCommandsClient
    {
        WebSocket websocket = new WebSocket("ws://liberatio.herokuapp.com/");

        public LiberatioCommandsClient()
        {
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
            websocket.Closed += new EventHandler(websocket_Closed);
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
        }

        public void start()
        {
            websocket.Open();
        }

        public void stop()
        {
            websocket.Close();
        }

        private void websocket_Opened(object sender, EventArgs e)
        {
            EventLog.WriteEntry("LiberatioAgent", "Web Socket Opened");
        }

        private void websocket_Error(object sender, ErrorEventArgs e)
        {
            // do something
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            // do something
        }

        private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // do something
            string message = e.Message;
            EventLog.WriteEntry("LiberatioAgent", message);
        }
    }
}
