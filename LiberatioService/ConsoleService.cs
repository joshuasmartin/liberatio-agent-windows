using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace LiberatioService
{
    [ServiceContract]
    public interface IConsoleService
    {
        [OperationContract]
        bool UpdateConfiguration(string key, string value);

        [OperationContract]
        string GetValueFromConfiguration(string key);

        [OperationContract]
        bool IsRegistered(string uuid);
    }

    public class ConsoleService : IConsoleService
    {
        public bool UpdateConfiguration(string key, string value)
        {
            return LiberatioConfiguration.UpdateValue(key, value);
        }

        public string GetValueFromConfiguration(string key)
        {
            return LiberatioConfiguration.GetValue(key);
        }

        public bool IsRegistered(string uuid)
        {
            try
            {
                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("nodes/{id}", Method.GET);

                // execute the request
                RestResponse response = (RestResponse)client.Execute(request);
                var content = response.Content; // raw content as string
            }
            catch (Exception exception)
            {
                EventLog.WriteEntry("LiberatioAgent", exception.ToString(), EventLogEntryType.Warning);
            }

            return true;
        }
    }
}
