using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiberatioService
{
    public class Application
    {
        public String name { get; set; }
        public String publisher { get; set; }
        public String version { get; set; }

        public Application (String name, String publisher, String version)
        {
            this.name = name;
            this.publisher = publisher;
            this.version = version;
        }
    }
}
