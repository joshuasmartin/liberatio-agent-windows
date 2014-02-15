using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Liberatio.Agent.Service
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
            bool found = false;

            try
            {
                var client = new RestClient("http://liberatio.herokuapp.com");
                var request = new RestRequest("nodes/registered.json", Method.GET);

                request.AddParameter("uuid", uuid, ParameterType.QueryString);

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
