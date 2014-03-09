using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;

namespace Liberatio.Agent.Service
{
    public class Cleaner
    {
        [DllImport("Srclient.dll")]
        public static extern int SRRemoveRestorePoint(int index);

        public void Start()
        {
            int SeqNum = 335;
            //int intReturn = SRRemoveRestorePoint(SeqNum);
        }

        private void EnumRestorePoints()
        {
            System.Management.ManagementClass objClass = new System.Management.ManagementClass("\\\\.\\root\\default", "systemrestore", new System.Management.ObjectGetOptions());
            System.Management.ManagementObjectCollection objCol = objClass.GetInstances();

            StringBuilder Results = new StringBuilder();
            foreach (System.Management.ManagementObject objItem in objCol)
            {
                Results.AppendLine((string)objItem["description"] + Convert.ToChar(9) + ((uint)objItem["sequencenumber"]).ToString());
            }

            //MessageBox.Show(Results.ToString());
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
                DirectoryInfo info = new DirectoryInfo(Environment.GetEnvironmentVariable("SystemRoot") + @"\Prefetch");

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
    }
}
