﻿using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace Liberatio.Agent.Service
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
        /// Checks the configuration file to see if there is a value in
        /// "registrationCode" - if so, it registers the device with the
        /// organization that the code belongs to.
        /// </summary>
        public static void RegisterIfNecessary()
        {
            // Return if there is no registration code.
            if (GetValue("registrationCode").Trim().Length == 0)
                return;

            // POST the UUID and the registration code to the server, if
            // the request is successful, it will return the token which
            // will be sent in all communique to authenticate the Agent.
            try
            {
                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("nodes/register.json", Method.POST);

                request.AddParameter("uuid", GetValue("uuid"), ParameterType.QueryString);
                request.AddParameter("registration_code", GetValue("registrationCode"), ParameterType.QueryString);

                // execute the request
                RestResponse response = (RestResponse)client.Execute(request);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        var content = response.Content;
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        string error = @"Registration Failed. Check the
                                         registration code you provided and try again.";
                        EventLog.WriteEntry("LiberatioAgent", error, EventLogEntryType.Error);
                        Environment.Exit(1);
                        break;
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }
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

        public static bool IsRegistered()
        {
            bool found = false;

            try
            {
                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("nodes/registered.json", Method.GET);

                request.AddParameter("uuid", GetValue("uuid"), ParameterType.QueryString);

                // execute the request
                RestResponse response = (RestResponse)client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    found = true;
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }

            return found;
        }
    }
}
