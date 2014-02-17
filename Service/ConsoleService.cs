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
        bool IsRegistered();
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

        public bool IsRegistered()
        {
            return LiberatioConfiguration.IsRegistered();
        }
    }
}
