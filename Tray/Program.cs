﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Liberatio.Agent.Tray
{
    static class Program
    {

        private static NotifyIcon notify = new NotifyIcon();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                return;  

            // Create Windows Event Source if it doesn't exist.
            if (!EventLog.SourceExists("LiberatioAgent"))
            {
                EventLog.CreateEventSource("LiberatioAgent", "Application");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += new EventHandler(OnApplicationExit);

            // the notify icon
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var pathToIcon = Path.Combine(Path.GetDirectoryName(location), "logo.ico");

            notify.Text = "Liberatio Agent";
            notify.Icon = new Icon(pathToIcon);
            notify.MouseClick += new MouseEventHandler(notify_MouseClick);

            // build the menu
            ContextMenu menu = new ContextMenu();
            notify.ContextMenu = menu;

            // show console menu item
            MenuItem itemShowConsole = new MenuItem("Open");
            itemShowConsole.Click += new EventHandler(itemShowConsole_Click);
            menu.MenuItems.Add(itemShowConsole);

            // separator
            menu.MenuItems.Add(new MenuItem("-"));

            // close menu item
            MenuItem itemClose = new MenuItem("Close");
            itemClose.Click += new EventHandler(itemClose_Click);
            menu.MenuItems.Add(itemClose);

            // finally make the icon visible
            notify.Visible = true;

            bool initial = false;
            string[] arguments = Environment.GetCommandLineArgs();
            foreach (string arg in arguments)
            {
                if (arg == "/initial")
                    initial = true;
            }

            if (initial)
                new FormSplash().Show();

            Application.Run();
        }

        private static void notify_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                new FormSplash().Show();
            }
        }

        private static void itemClose_Click(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void itemShowConsole_Click(object Sender, EventArgs e)
        {
            new FormSplash().Show();
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            try
            {
                notify.Dispose();
            }
            catch { }
        }
    }
}
