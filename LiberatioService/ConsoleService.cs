using System;
using System.Collections.Generic;
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
        bool IsRegistered(string uuid);
    }

    public class ConsoleService : IConsoleService
    {
        public bool UpdateConfiguration(string key, string value)
        {
            return LiberatioConfiguration.UpdateValue(key, value);
        }

        public bool IsRegistered(string uuid)
        {
            return true;
        }
    }
}
