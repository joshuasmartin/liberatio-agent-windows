using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace LiberatioService
{
    public static class LiberatioConfiguration
    {
        /// <summary>
        /// Verifies that the configuration file contains a UUID,
        /// and creates one if there isn't one present.
        /// </summary>
        public static void CheckOrUpdateUuid()
        {
            if (ConfigurationManager.AppSettings["uuid"].Trim().Length == 0)
            {
                String guid = Guid.NewGuid().ToString();
                EventLog.WriteEntry("LiberatioAgent", string.Format("Setting a new UUID {0}", guid));

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings.Remove("uuid");
                config.AppSettings.Settings.Add("uuid", guid);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        /// <summary>
        /// Uses WMI to determine the operating system type. It will be a
        /// Server, Workstation, or Domain Controller. The result is
        /// saved to the configuration file in the role attribute.
        /// </summary>
        public static void DiscoverRole()
        {
            // use wmi to determine the ProductType
            string result = string.Empty;
            ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("SELECT ProductType FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                switch (os["ProductType"].ToString())
                {
                    case "1":
                        result = "Workstation";
                        break;
                    case "2":
                        result = "Domain Controller";
                        break;
                    case "3":
                        result = "Server";
                        break;
                }
                break;
            }

            // set the result in the configuration
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings.Remove("role");
            config.AppSettings.Settings.Add("role", result);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// Returns the value in the configuration file for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static String GetValue(String key)
        {
            return ConfigurationManager.AppSettings[key].ToString().Trim();
        }

        /// <summary>
        /// Updates the value in the configuration file for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static bool UpdateValue(String key, String value)
        {
            bool success = false;

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings.Remove(key);
                config.AppSettings.Settings.Add(key, value);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                success = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            return success;
        }
    }
}
