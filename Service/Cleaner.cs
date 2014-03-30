using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Diagnostics;

namespace Liberatio.Agent.Service
{
    public class Cleaner
    {
        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();

        public void Start()
        {
            // Empty Recycle Bin
            EmptyRecycleBin();

            // Delete Windows Update cache.
            DeleteWindowsUpdateCache();

            // Delete Prefetch.
            DeleteWindowsPrefetchFiles();

            // Flush DNS cache.
            FlushDnsCache();

            // Delete the System Archived Error Reporting Logs.
            DeleteSystemArchivedErrorReportingLogs();
        }

        /// <summary>
        ///  Not Finished
        /// </summary>
        private void DeleteTemporaryInternetFiles()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_UserAccount");
            ManagementObjectCollection users = searcher.Get();

            var localUsers = users.Cast<ManagementObject>().Where(
                u =>    (bool)u["Disabled"] == false &&
                        int.Parse(u["SIDType"].ToString()) == 1 &&
                        u["Name"].ToString() != "HomeGroupUser$");

            foreach (ManagementObject user in localUsers)
            {
            }
        }

        /// <summary>
        /// Delete the contents of the $Recycle.bin directory on each drive.
        /// </summary>
        private void EmptyRecycleBin()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo(drive.Name + @"\$Recycle.bin");

                    foreach (FileInfo file in info.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in info.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// Flushes the Windows DNS cache using P/Invoke.
        /// </summary>
        private void FlushDnsCache()
        {
            UInt32 result = DnsFlushResolverCache();
        }

        /// <summary>
        /// Deletes the files and directories contained within the Windows
        /// Prefetch directory. Prefetch is used by Windows to cache certain
        /// applications that are launched often. However, it may contain
        /// data for applications that have been uninstalled or used long ago.
        /// </summary>
        private void DeleteWindowsPrefetchFiles()
        {
            try
            {
                string path = Environment.GetEnvironmentVariable("SystemRoot") + @"\Prefetch";
                DirectoryInfo info = new DirectoryInfo(path);

                foreach (FileInfo file in info.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in info.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Deletes the files in the Windows Update download cache only after
        /// stopping both the Windows Update and Background Intelligent
        /// Transfer Service (BITS) services in order to prevent corruption.
        /// </summary>
        private void DeleteWindowsUpdateCache()
        {
            ServiceController serviceWindowsUpdate = new ServiceController("wuauserv");
            ServiceController serviceBits = new ServiceController("BITS");

            // Make sure Windows Update service is stopped.
            if (serviceWindowsUpdate.Status.Equals(ServiceControllerStatus.Running) ||
                serviceWindowsUpdate.Status.Equals(ServiceControllerStatus.StartPending))
            {
                serviceWindowsUpdate.Stop();
            }

            // Make sure Background Intelligent Transfer Service service is stopped.
            if (serviceBits.Status.Equals(ServiceControllerStatus.Running) ||
                serviceBits.Status.Equals(ServiceControllerStatus.StartPending))
            {
                serviceBits.Stop();
            }

            // Delete the files and directories in the Windows Update cache.
            try
            {
                string path = Environment.GetEnvironmentVariable("SystemRoot") + @"\SoftwareDistribution\Download";
                DirectoryInfo info = new DirectoryInfo(path);

                foreach (FileInfo file in info.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in info.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Deletes the files in the System Level Archived Error Reporting
        /// and Solutions logs.
        /// </summary>
        private void DeleteSystemArchivedErrorReportingLogs()
        {
            try
            {
                string path = Environment.GetEnvironmentVariable("AllUsersProfile") + @"\Microsoft\Windows\WER\ReportArchive";
                DirectoryInfo info = new DirectoryInfo(path);

                foreach (FileInfo file in info.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in info.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
