using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public enum CacheFrequency
    {
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public class HtmlCacheSetting
    {
        public HtmlCacheSetting()
        {
            this.Frequency = CacheFrequency.Hourly;
        }

        public string Id { get; set; }
        public bool Overwrite { get; set; }
        public string Category { get; set; }
        public CacheFrequency Frequency { get; set; }
        public string Header { get; set; }
        public System.DateTime DateOverride { get; set; }
    }
}
