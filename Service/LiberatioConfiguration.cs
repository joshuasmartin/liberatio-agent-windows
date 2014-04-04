using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Liberatio.Agent.Service
{
    public static class LiberatioConfiguration
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

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
        /// Uses the Windows API to get the Internet connection state.
        /// </summary>
        /// <returns>Returns whether or not the computer is connected</returns>
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }

        /// <summary>
        /// Checks the configuration file to determine if
        /// the remote commands feature should be enabled.
        /// </summary>
        /// <returns>Returns whether or not to use
        /// the remote commands feature.</returns>
        public static bool UseRemoteCommands()
        {
            var value = GetValue("useRemoteCommands");
            return bool.Parse(value);
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

        /// <summary>
        /// Creates the "liberatio" Administrator account on the system if it
        /// does not exist already. If it does exist, it changes the password.
        /// The password is encrypted by Windows DPAPI and stored in a file
        /// called "auth.dat" whose contents can only be decrypted by the
        /// LocalSystem account.
        /// </summary>
        public static void CreateOrUpdateLiberatioUser()
        {
            string password = CreateSecureRandomString(20);

            try
            {
                PrincipalContext context = new PrincipalContext(ContextType.Machine, Environment.MachineName);
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, "liberatio");

                // Either create or change password. Make sure that the account
                // is disabled, cannot change the password, and has a password
                // that never expires.
                if (userPrincipal == null)
                {
                    userPrincipal = new UserPrincipal(context, "liberatio", password, false);
                    userPrincipal.UserCannotChangePassword = true;
                    userPrincipal.PasswordNeverExpires = false;
                    userPrincipal.Save();
                }
                else
                {
                    userPrincipal.UserCannotChangePassword = true;
                    userPrincipal.PasswordNeverExpires = false;
                    userPrincipal.Enabled = false;
                    userPrincipal.SetPassword(password);
                    userPrincipal.Save();
                }

                // Ensure user is in the Administrators group.
                GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(context, "Administrators");
                if (!groupPrincipal.Members.Contains(userPrincipal))
                {
                    groupPrincipal.Members.Add(userPrincipal);
                    groupPrincipal.Save();
                }
                groupPrincipal.Dispose();

                // Save the password to a file for using later. Will be
                // encrypted using DPAPI and will be accessible to the
                // LocalSystem user only.
                byte[] toEncrypt = UnicodeEncoding.ASCII.GetBytes(password);
                byte[] entropy = CreateRandomEntropy();

                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var pathToPassword = Path.Combine(Path.GetDirectoryName(location), "auth.dat");

                // Encrypt a copy of the data to the stream.
                FileStream stream = new FileStream(pathToPassword, FileMode.OpenOrCreate);
                //int bytesWritten = EncryptDataToStream(toEncrypt, entropy, DataProtectionScope.CurrentUser, stream);
                int bytesWritten = EncryptDataToStream(toEncrypt, DataProtectionScope.CurrentUser, stream);
                stream.Close();
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", "Failed to create or update the liberatio Windows account.",
                                    EventLogEntryType.Error);
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
                Environment.Exit(1);
            }
        }

        public static SecureString GetLiberatioUserPassword()
        {
            var securePassword = new SecureString();
            char[] insecurePassword;

            try
            {
                var location = System.Reflection.Assembly.GetEntryAssembly().Location;
                var path = Path.Combine(Path.GetDirectoryName(location), "auth.dat");

                using (FileStream fileStream = new FileStream(path, FileMode.Open))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Position = 0;
                    fileStream.CopyTo(memoryStream);
                    var bytesEncrypted = memoryStream.ToArray();

                    //byte[] bytesDecrypted = DecryptDataFromStream(entropy, DataProtectionScope.CurrentUser,
                    //                                                fileStream, bytesEncrypted);
                    byte[] bytesDecrypted = DecryptDataFromStream(DataProtectionScope.CurrentUser,
                                                                    fileStream, bytesEncrypted.Length);

                    insecurePassword = UnicodeEncoding.ASCII.GetString(bytesDecrypted).ToCharArray();
                    foreach (char c in insecurePassword) securePassword.AppendChar(c);
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Error);
            }

            // Ensure insecure password is deleted from memory.
            insecurePassword = null;
            GC.Collect();

            return securePassword;
        }

        /// <summary>
        /// Adds a DWORD value to the SpecialAccounts\UserList registry key
        /// in order to hide the "liberatio" Windows account from the Windows
        /// login screen.
        /// </summary>
        private static void HideUserInRegistry()
        {
            try
            {
                string accountsKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\WinLogon\SpecialAccounts";
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(accountsKey))
                {
                    // Create the SpecialAccounts key if it doesn't exist.
                    if (key == null)
                        Registry.LocalMachine.CreateSubKey(accountsKey);
                }

                string listKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\WinLogon\SpecialAccounts\UserList";
                using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(listKey))
                {
                    // Create the UserList key if it doesn't exist.
                    if (key == null)
                        Registry.LocalMachine.CreateSubKey(listKey);

                    // Always set the value of "liberatio" DWORD to 0
                    key.SetValue("liberatio", 0, RegistryValueKind.DWord);
                }
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent",
                                    @"Failed to add the 'liberatio' value to the SpecialAccounts\UserList registry key",
                                    EventLogEntryType.Warning);
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }
        }

        /// <summary>
        /// Used from http://stackoverflow.com/a/8996788
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string CreateSecureRandomString(int length)
        {
            char[] AvailableCharacters = {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
            };

            char[] identifier = new char[length];
            byte[] randomData = new byte[length];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            return new string(identifier);
        }

        private static byte[] CreateRandomEntropy()
        {
            // Create a byte array to hold the random value. 
            byte[] entropy = new byte[16];

            // Create a new instance of the RNGCryptoServiceProvider. 
            // Fill the array with a random value. 
            new RNGCryptoServiceProvider().GetBytes(entropy);

            // Return the array. 
            return entropy;
        }

        //private static int EncryptDataToStream(byte[] Buffer, byte[] Entropy, DataProtectionScope Scope, Stream S)
        private static int EncryptDataToStream(byte[] Buffer, DataProtectionScope Scope, Stream S)
        {
            if (Buffer.Length <= 0)
                throw new ArgumentException("Buffer");
            if (Buffer == null)
                throw new ArgumentNullException("Buffer");
            //if (Entropy.Length <= 0)
            //    throw new ArgumentException("Entropy");
            //if (Entropy == null)
            //    throw new ArgumentNullException("Entropy");
            if (S == null)
                throw new ArgumentNullException("S");

            int length = 0;

            // Encrypt the data in memory. The result is stored in the same same array as the original data.
            //byte[] encrptedData = ProtectedData.Protect(Buffer, Entropy, Scope);
            byte[] encrptedData = ProtectedData.Protect(Buffer, null, Scope);

            // Write the encrypted data to a stream. 
            if (S.CanWrite && encrptedData != null)
            {
                S.Write(encrptedData, 0, encrptedData.Length);

                length = encrptedData.Length;
            }

            // Return the length that was written to the stream.  
            return length;

        }

        //private static byte[] DecryptDataFromStream(byte[] Entropy, DataProtectionScope Scope, Stream S, int Length)
        private static byte[] DecryptDataFromStream(DataProtectionScope Scope, Stream S, int Length)
        {
            if (S == null)
                throw new ArgumentNullException("S");
            if (Length <= 0)
                throw new ArgumentException("Length");
            //if (Entropy == null)
            //    throw new ArgumentNullException("Entropy");
            //if (Entropy.Length <= 0)
            //    throw new ArgumentException("Entropy");



            byte[] inBuffer = new byte[Length];
            byte[] outBuffer;

            // Read the encrypted data from a stream. 
            if (S.CanRead)
            {
                S.Read(inBuffer, 0, Length);

                //outBuffer = ProtectedData.Unprotect(inBuffer, Entropy, Scope);
                outBuffer = ProtectedData.Unprotect(inBuffer, null, Scope);
            }
            else
            {
                throw new IOException("Could not read the stream.");
            }

            // Return the length that was written to the stream.  
            return outBuffer;
        }
    }
}
