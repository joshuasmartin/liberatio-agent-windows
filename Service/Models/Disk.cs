using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberatio.Agent.Service.Models
{
    public class Disk
    {
        public string disk_type { get; set; }
        public string file_system { get; set; }
        public string free_bytes { get; set; }
        public string total_bytes { get; set; }
        public string volume_name { get; set; }

        public Disk(string disk_type, string file_system, string free_bytes, string total_bytes, string volume_name)
        {
            this.file_system = file_system;
            this.free_bytes = free_bytes;
            this.total_bytes = total_bytes;
            this.volume_name = volume_name;

            // Select text from given integer for the disk type.
            Dictionary<int, string> types = new Dictionary<int, string>();
            types.Add(0, "Unknown");
            types.Add(1, "No Root Directory");
            types.Add(2, "Removable Disk");
            types.Add(3, "Local Disk");
            types.Add(4, "Network Drive");
            types.Add(5, "Compact Disc");
            types.Add(6, "RAM Disk");

            int t = int.Parse(disk_type);
            this.disk_type = types[t];
        }
    }
}
