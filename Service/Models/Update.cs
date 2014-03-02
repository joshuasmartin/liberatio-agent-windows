using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Liberatio.Agent.Service.Models
{
    public class Update : IComparable<Update>
    {
        public string title { get; set; }
        public int severity { get; set; }
        public string support_url { get; set; }
        public bool is_installed { get; set; }

        public Update(string title, string severity, string support_url, bool is_installed)
        {
            this.title = title;
            this.is_installed = is_installed;
            this.support_url = support_url;

            switch (severity)
            {
                case "Critical":
                    this.severity = 40;
                    break;
                case "Important":
                    this.severity = 30;
                    break;
                case "Low":
                    this.severity = 20;
                    break;
                case "Moderate":
                    this.severity = 10;
                    break;
                case "Unspecified":
                    this.severity = 0;
                    break;
            }
        }

        public int CompareTo(Update b)
        {
            // Alphabetic sort name[A to Z]
            return this.title.CompareTo(b.title);
        }
    }
}
