using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace LiberatioService
{
    class Inventory
    {
        public String name { get; set; }
        public String operating_system { get; set; }

        public void populate()
        {
            name = System.Environment.MachineName;
            operating_system = getOperatingSystem();
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
    }
}
