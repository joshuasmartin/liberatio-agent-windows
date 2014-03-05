using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Liberatio.Agent.Service.Models
{
    public class Inventory
    {
        public string uuid { get; set; }
        public string token { get; set; }
        public string name { get; set; }
        public string operating_system { get; set; }
        public string location { get; set; }
        public string role { get; set; }
        public string model_number { get; set; }
        public string serial_number { get; set; }
        public IList<Application> applications { get; set; }
        public IList<Memory> memory { get; set; }
        public IList<Processor> processor { get; set; }
        public IList<Disk> disks { get; set; }
        public IList<Update> updates { get; set; }

        public Inventory()
        {
            uuid = ConfigurationManager.AppSettings["uuid"].Trim(); // from config
            token = ConfigurationManager.AppSettings["communicationToken"].Trim(); // from config
            name = System.Environment.MachineName;
            operating_system = getOperatingSystem();
            location = ConfigurationManager.AppSettings["location"].Trim(); // from config
            role = ConfigurationManager.AppSettings["role"].Trim(); // from config
            model_number = getModelNumber();
            serial_number = getSerialNumber();
            applications = getApplications();
            memory = getMemory();
            processor = getProcessor();
            disks = getDisks();

            List<Update> all = new List<Update>();
            all.AddRange(UpdateManager.Installed);
            all.AddRange(UpdateManager.Needed);
            updates = all;

            CheckAntivirus();
        }

        public void Send()
        {
            try
            {
                // client
                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("inventories.json", Method.POST);

                // payload
                String json = JsonConvert.SerializeObject(new { inventory = this }, Formatting.Indented);
                request.AddParameter("application/json", json, ParameterType.RequestBody);

                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.IO.File.WriteAllText(string.Format(@"{0}\inventory.txt", System.IO.Path.GetDirectoryName(location)), json);

                // execute the request
                RestResponse response = (RestResponse)client.Execute(request);
                var content = response.Content;

                switch ((int)response.StatusCode)
                {
                    case 200:
                        break;
                    case 422:
                        EventLog.WriteEntry("LiberatioAgent", "Failed to inventory " + content, EventLogEntryType.Error);
                        break;
                    default:
                        EventLog.WriteEntry("LiberatioAgent", "Failed to inventory " + content, EventLogEntryType.Error);
                        break;
                }
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
                result = os["Caption"].ToString().Replace("Microsoft ", "");
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
        /// Acquires the motherboard serial number via WMI.
        /// </summary>
        /// <returns>The motherboard serial number</returns>
        private String getSerialNumber()
        {
            string result = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
            foreach (ManagementObject os in searcher.Get())
            {
                result = os["SerialNumber"].ToString();
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

        /// <summary>
        /// Acquires the information about each memory module via WMI.
        /// </summary>
        /// <returns>A list of memory modules</returns>
        private List<Memory> getMemory()
        {
            List<Memory> list = new List<Memory>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity,FormFactor,Manufacturer,MemoryType,Speed FROM Win32_PhysicalMemory");
                foreach (ManagementObject os in searcher.Get())
                {
                    list.Add(new Memory(os["Capacity"].ToString(),
                                        os["FormFactor"].ToString(),
                                        os["Manufacturer"].ToString(),
                                        os["MemoryType"].ToString(),
                                        os["Speed"].ToString()));
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            return list;
        }

        /// <summary>
        /// Acquires the information about each processor via WMI.
        /// </summary>
        /// <returns>A list of processors</returns>
        private List<Processor> getProcessor()
        {
            List<Processor> list = new List<Processor>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Architecture,MaxClockSpeed,Name,NumberOfCores FROM Win32_Processor");
                foreach (ManagementObject os in searcher.Get())
                {
                    list.Add(new Processor(os["Name"].ToString(),
                                        os["NumberOfCores"].ToString(),
                                        os["MaxClockSpeed"].ToString(),
                                        os["Architecture"].ToString()));
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            return list;
        }

        /// <summary>
        /// Acquires the information about each disk via WMI.
        /// </summary>
        /// <returns>A list of disks</returns>
        private List<Disk> getDisks()
        {
            List<Disk> list = new List<Disk>();
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT DriveType,FileSystem,FreeSpace,Size,VolumeName FROM Win32_LogicalDisk");
                foreach (ManagementObject o in searcher.Get())
                {
                    string drive_type = GetValueFromKey(o["DriveType"]);
                    string file_system = GetValueFromKey(o["FileSystem"]);
                    string free_bytes = GetValueFromKey(o["FreeSpace"]);
                    string total_bytes = GetValueFromKey(o["Size"]);
                    string volume_name = GetValueFromKey(o["VolumeName"]);

                    list.Add(new Disk(  drive_type, file_system, free_bytes,
                                        total_bytes, volume_name));
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            return list;
        }

        private void CheckAntivirus()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SystemDriver");
                foreach (ManagementObject o in searcher.Get())
                {
                    try
                    {
                        FileInfo f = new FileInfo(GetValueFromKey(o["PathName"]));
                        string name = f.Name;

                        if (name.ToLower() == "mpfilter.sys")
                        {
                            EventLog.WriteEntry("LiberatioAgent", GetValueFromKey(o["PathName"]), EventLogEntryType.Information);

                            string concat = "";

                            concat += GetValueFromKey(o["DisplayName"]);
                            concat += " ";
                            concat += GetValueFromKey(o["PathName"]);

                            // Check the digital signature of the driver at the given path from WMI.
                            // The method used to check the signature will throw an exception if it
                            // cannot get the certificate for the signature, meaning for us, that the
                            // certificate is invalid or non-existant.
                            try
                            {
                                var certificate = X509Certificate.CreateFromSignedFile(f.FullName);
                                EventLog.WriteEntry("LiberatioAgent", certificate.Subject, EventLogEntryType.Information);
                                EventLog.WriteEntry("LiberatioAgent", certificate.Issuer, EventLogEntryType.Information);
                            }
                            catch (Exception)
                            {
                                EventLog.WriteEntry("LiberatioAgent", "Signature cannot be verified.", EventLogEntryType.Information);
                            }
                        }
                    }
                    catch (ArgumentException)
                    {

                    }
                    catch (Exception exception)
                    {
                        EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        private string GetValueFromKey(object o)
        {
            string value = "";

            if (o != null)
            {
                value = o.ToString();
            }

            return value;
        }
    }
}
