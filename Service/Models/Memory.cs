using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberatio.Agent.Service.Models
{
    public class Memory
    {
        public string Capacity { get; set; }
        public string FormFactor { get; set; }
        public string Manufacturer { get; set; }
        public string MemoryType { get; set; }
        public string Speed { get; set; }

        public Memory(string capacity, string formFactor, string manufacturer, string memoryType, string speed)
        {
            this.Capacity = capacity;
            this.Manufacturer = manufacturer;
            this.Speed = speed;

            // Select text from given integer for the form factor.
            Dictionary<int, string> formFactors = new Dictionary<int, string>();
            formFactors.Add(0, "Unknown");
            formFactors.Add(1, "Other");
            formFactors.Add(2, "SIP");
            formFactors.Add(3, "DIP");
            formFactors.Add(4, "ZIP");
            formFactors.Add(5, "SOJ");
            formFactors.Add(6, "Proprietary");
            formFactors.Add(7, "SIMM");
            formFactors.Add(8, "DIMM");
            formFactors.Add(9, "TSOP");
            formFactors.Add(10, "PGA");
            formFactors.Add(11, "RIMM");
            formFactors.Add(12, "SODIMM");
            formFactors.Add(13, "SRIMM");
            formFactors.Add(14, "SMD");
            formFactors.Add(15, "SSMP");
            formFactors.Add(16, "QFP");
            formFactors.Add(17, "TQFP");
            formFactors.Add(18, "SOIC");
            formFactors.Add(19, "LCC");
            formFactors.Add(20, "PLCC");
            formFactors.Add(21, "BGA");
            formFactors.Add(22, "FPBGA");
            formFactors.Add(23, "LGA");

            int f = int.Parse(formFactor);
            this.FormFactor = formFactors[f];

            // Select text from given integer for the memory type.
            Dictionary<int, string> memoryTypes = new Dictionary<int, string>();
            memoryTypes.Add(0, "Unknown");
            memoryTypes.Add(1, "Other");
            memoryTypes.Add(2, "DRAM");
            memoryTypes.Add(3, "Synchronous DRAM");
            memoryTypes.Add(4, "Cache DRAM");
            memoryTypes.Add(5, "EDO");
            memoryTypes.Add(6, "EDRAM");
            memoryTypes.Add(7, "VRAM");
            memoryTypes.Add(8, "SRAM");
            memoryTypes.Add(9, "RAM");
            memoryTypes.Add(10, "ROM");
            memoryTypes.Add(11, "Flash");
            memoryTypes.Add(12, "EEPROM");
            memoryTypes.Add(13, "FEPROM");
            memoryTypes.Add(14, "EPROM");
            memoryTypes.Add(15, "CDRAM");
            memoryTypes.Add(16, "3DRAM");
            memoryTypes.Add(17, "SDRAM");
            memoryTypes.Add(18, "SGRAM");
            memoryTypes.Add(19, "RDRAM");
            memoryTypes.Add(20, "DDR");
            memoryTypes.Add(21, "DDR-2");

            int t = int.Parse(memoryType);
            this.MemoryType = memoryTypes[t];
        }
    }
}
