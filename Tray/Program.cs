using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Liberatio.Agent.Tray
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // the notify icon
            NotifyIcon notify = new NotifyIcon();
            notify.Text = "Liberatio Agent";
            notify.Icon = new Icon("logo.ico");
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

            Application.Run();

            notify.Visible = false;
        }

        private static void notify_MouseClick(object sender, MouseEventArgs e)
        {
            new FormConsole().Show();
        }

        private static void itemClose_Click(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void itemShowConsole_Click(object Sender, EventArgs e)
        {
            new FormConsole().Show();
        }
    }
}
