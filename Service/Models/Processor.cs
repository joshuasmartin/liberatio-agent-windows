using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberatio.Agent.Service.Models
{
    public class Processor
    {
        public string architecture { get; set; }
        public string name { get; set; }
        public string cores_count { get; set; }
        public string speed { get; set; }

        public Processor (string name, string cores_count, string speed, string architecture)
        {
            this.name = name;
            this.cores_count = cores_count;
            this.speed = speed;

            switch (int.Parse(architecture))
            {
                case 0:
                    this.architecture = "x86";
                    break;
                case 1:
                    this.architecture = "MIPS";
                    break;
                case 2:
                    this.architecture = "Alpha";
                    break;
                case 3:
                    this.architecture = "PowerPC";
                    break;
                case 5:
                    this.architecture = "ARM";
                    break;
                case 6:
                    this.architecture = "Itanium";
                    break;
                case 9:
                    this.architecture = "x64";
                    break;
            }
        }
    }
}
