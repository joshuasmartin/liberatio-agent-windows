using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace LiberatioService
{
    public class Inventory
    {
        public String uuid { get; set; }
        public String name { get; set; }
        public String operating_system { get; set; }
        public String location { get; set; }
        public String role { get; set; }
        public String model_number { get; set; }
        public String serial_number { get; set; }
        public IList<Application> applications { get; set; }

        public Inventory()
        {
            uuid = ConfigurationManager.AppSettings["uuid"].Trim(); // from config
            name = System.Environment.MachineName;
            operating_system = getOperatingSystem();
            location = ConfigurationManager.AppSettings["location"].Trim(); // from config
            role = ConfigurationManager.AppSettings["role"].Trim(); // from config
            model_number = getModelNumber();
            applications = getApplications();
        }

        public void Send()
        {
            try
            {
                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("inventories", Method.POST);

                request.AddParameter("application/json", "data", ParameterType.RequestBody);

                // execute the request
                RestResponse response = (RestResponse)client.Execute(request);
                var content = response.Content; // raw content as string
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }
        }

        /// <summary>
        /// Acquires the friendly version of the operating system
        /// name from the local system using WMI.
        /// </summary>
        /// <returns>The operating system name (ex. Windows XP)</returns>
        private String getOperatingSystem()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["Caption"].ToString();
                break;
            }
            return result;
        }

        /// <summary>
        /// Acquires the computer's manufacturer and model number
        /// and concatenates it into a single string.
        /// </summary>
        /// <returns>The computer manufacturer and model number</returns>
        private String getModelNumber()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Manufacturer,Model FROM Win32_ComputerSystem");
            foreach (ManagementObject os in searcher.Get())
            {
                result = String.Format("{0} {1}", os["Manufacturer"], os["Model"]);
                break;
            }
            return result;
        }

        /// <summary>
        /// Parses the registry to determine installed applications. Looks under
        /// both the common HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall
        /// and the HKLM\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall
        /// </summary>
        /// <returns>A List of installed applications</returns>
        private List<Application> getApplications()
        {
            List<Application> list = new List<Application>();

            String key32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            String key64 = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

            try
            {
                // get installed applications on 32 bit machines
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(key32))
                {
                    foreach (String subkeyName in key.GetSubKeyNames())
                    {
                        string _name = key.OpenSubKey(subkeyName).GetValue("DisplayName", "").ToString();
                        string _version = key.OpenSubKey(subkeyName).GetValue("DisplayVersion", "").ToString();
                        string _publisher = key.OpenSubKey(subkeyName).GetValue("Publisher", "").ToString();
                        string _sComponent = key.OpenSubKey(subkeyName).GetValue("SystemComponent", "").ToString();

                        if ((_name.Length > 0) && (_version.Length > 0) && (_publisher.Length > 0))
                        {
                            if ((_sComponent.Length == 0) || (_sComponent == "0"))
                            {
                                list.Add(new Application(_name, _publisher, _version));
                            }
                        }
                    }
                }

                // get installed applications on 64 bit machines
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(key64))
                {
                    foreach (String subkeyName in key.GetSubKeyNames())
                    {
                        string _name = key.OpenSubKey(subkeyName).GetValue("DisplayName", "").ToString();
                        string _version = key.OpenSubKey(subkeyName).GetValue("DisplayVersion", "").ToString();
                        string _publisher = key.OpenSubKey(subkeyName).GetValue("Publisher", "").ToString();
                        string _sComponent = key.OpenSubKey(subkeyName).GetValue("SystemComponent", "").ToString();

                        if ((_name.Length > 0) && (_version.Length > 0) && (_publisher.Length > 0))
                        {
                            if ((_sComponent.Length == 0) || (_sComponent == "0"))
                            {
                                list.Add(new Application(_name, _publisher, _version));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            return list;
        }
    }
}
