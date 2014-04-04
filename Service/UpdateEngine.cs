using Ionic.Zip;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;

namespace Liberatio.Agent.Service
{
    public static class UpdateEngine
    {
        private static string applicationFilePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        private static string applicationDirectory = Path.GetDirectoryName(applicationFilePath);
        private static string applicationCacheDirectory = Path.Combine(applicationDirectory, "cache");

        public static void Update()
        {
            try
            {
                // Make sure update cache directory exists before continuing.
                Directory.CreateDirectory(applicationCacheDirectory);

                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("/au/latest.json", Method.GET);

                // execute the request
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // The json response should resemble:
                    // { windows: { 'version': '1.0.0.0', 'sha2sum': 'theSum', 'url': 'theUrl' } }
                    string content = response.Content;
                    var json = (JObject)JsonConvert.DeserializeObject(content);
                    var windows = (JObject)json["windows"];
                    var updateSha2sum = windows["sha2sum"].ToString();
                    var updateUrl = windows["url"].ToString();
                    var updateVersion = windows["version"].ToString();

                    // Check if the latest version is greater than the installed version.
                    Version latestVersion = new Version(updateVersion);
                    Version installedVersion = GetInstalledVersion();

                    // Delete all files in the cache that do not
                    // match the given version number.
                    ClearCache(latestVersion);

                    // If the latest version is newer that the installed
                    // version, proceed with download and install.
                    if (latestVersion > installedVersion)
                    {
                        EventLog.WriteEntry("LiberatioAgent", "Liberatio is NOT up-to-date", EventLogEntryType.Information);

                        // If the update does not exist or is not valid, download it.
                        if (GetUpdateFilePath(latestVersion, updateSha2sum).Length == 0)
                        {
                            // Get the URL for the update file.
                            Uri uri = new Uri(updateUrl);
                            if (!uri.IsFile)
                                throw new Exception("URL is invalid");

                            // Download the update file and verify its integrity.
                            DownloadUpdate(uri, updateSha2sum);
                        }

                        // Extract the update file and perform install.
                        PerformInstall(latestVersion, updateSha2sum);
                    }
                    else
                    {
                        EventLog.WriteEntry("LiberatioAgent", "Liberatio is up-to-date", EventLogEntryType.Information);
                    }
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", "Failed to perform update", EventLogEntryType.Warning);
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }
        }

        /// <summary>
        /// Returns a Version object for the currently running assembly.
        /// </summary>
        /// <returns>The Version object representing this assembly's version</returns>
        public static Version GetInstalledVersion()
        {
            return new Version(FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).ProductVersion);
        }

        /// <summary>
        /// Downloads a given update file and verifies its integrity.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="sha2sum"></param>
        /// <returns>Whether or not the download and verification succeeded.</returns>
        private static bool DownloadUpdate(Uri uri, string sha2sum)
        {
            bool success = false;

            try
            {
                string path = Path.Combine(applicationCacheDirectory, Path.GetFileName(uri.LocalPath));

                // Download the file.
                using (WebClient web = new WebClient())
                {
                    web.DownloadFile(uri, path);
                }

                IsUpdateValid(path, sha2sum);

                success = true;
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }

            return success;
        }

        /// <summary>
        /// Returns the path to the update file in the cache that matches the
        /// given version number and sha2sum. Returns an empty string, otherwise.
        /// </summary>
        /// <param name="version">The version number of the update file to retrieve</param>
        /// <param name="sha2sum">The sha2sum of the update file to retrieve</param>
        /// <returns>Returns the path to the file with the given version and sha2sum</returns>
        private static string GetUpdateFilePath(Version version, string sha2sum)
        {
            string file = "";
            string[] filePaths = Directory.GetFiles(applicationCacheDirectory);

            foreach (string path in filePaths)
            {
                string filename = Path.GetFileNameWithoutExtension(path);

                if (new Version(filename) == version)
                    if (IsUpdateValid(path, sha2sum))
                        file = path;
            }

            return file;
        }

        /// <summary>
        /// Starts a new process that executes the update file. The process
        /// will not wait for the update to complete, because the service will
        /// be terminated by the update.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="sha2sum"></param>
        private static void PerformInstall(Version version, string sha2sum)
        {
            string zipPath = GetUpdateFilePath(version, sha2sum);
            string extractedFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string extractedSetupFilePath = Path.Combine(extractedFolderPath, "setup.exe");
            string extractedMsiFilePath = Path.Combine(extractedFolderPath, "setup.msi");

            // Extract the update file to a temporary directory.
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(extractedFolderPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            // Start the setup.exe process with the MSI file path as an argument.
            ProcessStartInfo info = new ProcessStartInfo(extractedSetupFilePath, extractedMsiFilePath);
            info.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = info;
            process.Start();
        }

        /// <summary>
        /// Deletes all the files in the cache directory that do not match
        /// the given version number. If no version number is given, all files
        /// are deleted, regardless of their version number.
        /// </summary>
        /// <param name="version">The version number of the files to leave.</param>
        private static void ClearCache(Version version = null)
        {
            string[] filePaths = Directory.GetFiles(applicationCacheDirectory);

            foreach (string path in filePaths)
            {
                // Always delete the file if version is irrelevant.
                if (version == null)
                {
                    File.Delete(path);
                }
                else
                {
                    // Delete the file if the file version is not the given version.
                    string filename = Path.GetFileNameWithoutExtension(path);
                    if (new Version(filename) != version)
                        File.Delete(path);
                }
            }
        }

        private static bool IsUpdateValid(string path, string sha2sum)
        {
            bool valid = false;

            using (var sha2 = SHA256Managed.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    byte[] hashValue = sha2.ComputeHash(stream);
                    string s = BitConverter.ToString(hashValue).Replace("-", String.Empty);

                    if (s.Equals(sha2))
                        valid = true;
                }
            }

            return valid;
        }
    }
}
