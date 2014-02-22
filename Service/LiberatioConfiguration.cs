using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
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
            // Exit the application if there is no code and no token.
            // The Agent will never be able to make contact with the
            // server without one or the other.
            if ((GetValue("registrationCode").Trim().Length == 0) &&
                (GetValue("communicationToken").Trim().Length == 0))
            {
                EventLog.WriteEntry("LiberatioAgent",
                    "No communcations token and no registration code. " +
                    "Provide the registration code for a token to be " +
                    "retrieved or a valid token.", EventLogEntryType.Error);
                Environment.Exit(1);
            }

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
                var content = response.Content;

                switch ((int)response.StatusCode)
                {
                    case 200:
                        var json = (JObject)JsonConvert.DeserializeObject(content);
                        string uuid = json["uuid"].ToString();
                        string token = json["token"].ToString();

                        if (uuid == GetValue("uuid"))
                        {
                            UpdateValue("communicationToken", token);
                            UpdateValue("registrationCode", "");
                        }

                        break;
                    case 422:
                        EventLog.WriteEntry("LiberatioAgent", "Failed to register " + content, EventLogEntryType.Error);
                        Environment.Exit(1);
                        break;
                    default:
                        EventLog.WriteEntry("LiberatioAgent", "Failed to register " + content, EventLogEntryType.Error);
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
        public static string GetValue(string key)
        {
            string value = "";

            try
            {
                value = ConfigurationManager.AppSettings[key].ToString().Trim();
            }
            catch (Exception) {}

            return value;
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
