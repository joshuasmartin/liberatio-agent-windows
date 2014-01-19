using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LiberatioTray
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

            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream file =
                thisExe.GetManifestResourceStream("LiberatioTray.gear_16xLG.png");

            //Image image = Image.FromFile("gear_16xLG.png");
            //Bitmap bitmap = new Bitmap(image);

            Bitmap bitmap = new Bitmap(Image.FromStream(file));
            Icon icon = Icon.FromHandle(bitmap.GetHicon());
            notify.Icon = icon;

            // build the menu
            ContextMenu menu = new ContextMenu();
            notify.ContextMenu = menu;

            // show console menu item
            MenuItem itemShowConsole = new MenuItem("Show Console");
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
