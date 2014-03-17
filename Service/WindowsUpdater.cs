using Liberatio.Agent.Service.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WUApiLib;

namespace Liberatio.Agent.Service
{
    public static class WindowsUpdater
    {
        public static List<Update> Installed = new List<Update>();
        public static List<Update> Needed = new List<Update>();
        private static UpdateSession session = new UpdateSession();

        public static void Setup()
        {
            UpdateServiceManager manager = session.CreateUpdateServiceManager();
            manager.AddService2("7971f918-a847-4430-9279-4a52d1efe18d", 7, "");
        }

        /// <summary>
        /// Returns a list of updates that are installed on this computer
        /// according to the Windows Update Agent API.
        /// </summary>
        /// <returns></returns>
        public static List<Update> GetInstalled()
        {
            List<Update> list = new List<Update>();
            IUpdateSearcher searcher = session.CreateUpdateSearcher();
            searcher.Online = false;

            try
            {
                ISearchResult result = searcher.Search("IsInstalled=1");
                foreach (IUpdate update in result.Updates)
                {
                    list.Add(new Update(title: update.Title,
                                        severity: update.MsrcSeverity,
                                        support_url: update.SupportUrl,
                                        is_installed: true));
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            Installed = list;
            return list;
        }

        /// <summary>
        /// Returns a list of updates that are needed on this computer
        /// according to the Windows Update Agent API.
        /// </summary>
        /// <returns></returns>
        public static List<Update> GetNeeded()
        {
            List<Update> list = new List<Update>();
            IUpdateSearcher searcher = session.CreateUpdateSearcher();

            try
            {
                ISearchResult result = searcher.Search("IsInstalled=0 And IsHidden=0");
                foreach (IUpdate update in result.Updates)
                {
                    list.Add(new Update(title: update.Title,
                                        severity: update.MsrcSeverity,
                                        support_url: update.SupportUrl,
                                        is_installed: false));
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            Needed = list;
            return list;
        }

        public static string GetHistory()
        {
            string history = "";

            IUpdateSearcher searcher = session.CreateUpdateSearcher();
            searcher.Online = false;

            try
            {
                int count = searcher.GetTotalHistoryCount();

                IUpdateHistoryEntryCollection query = searcher.QueryHistory(0, count);
                for (int i = 0; i < count; ++i)
                {
                    history += string.Format("title: {0} date: {1} description: {2} resultcode: {3}\r\n",
                        query[i].Title, query[i].Date, query[i].Description, query[i].ResultCode);
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            return history;
        }

        /// <summary>
        /// Downloads and installs updates using the Windows Update Agent
        /// from a list of given titles received from the site.
        /// </summary>
        /// <param name="titles"></param>
        public static void Install(List<string> titles)
        {
            UpdateCollection collection = new UpdateCollection();
            IUpdateSearcher searcher = session.CreateUpdateSearcher();

            try
            {
                // Build a collection from the titles received.
                ISearchResult result = searcher.Search("IsInstalled=0 And IsHidden=0");
                foreach (IUpdate update in result.Updates)
                {
                    // Go through each title and see if this update matches
                    // one of the titles. If it does, add it to the collection.
                    foreach (string title in titles)
                        if (update.Title.ToUpper() == title.ToUpper())
                            collection.Add(update);
                }

                // Download the updates.
                UpdateDownloader downloader = session.CreateUpdateDownloader();
                downloader.Updates = collection;
                downloader.Download();

                // Install the updates.
                IUpdateInstaller installer = session.CreateUpdateInstaller();
                installer.Updates = collection;
                IInstallationResult installResults = installer.Install();
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
