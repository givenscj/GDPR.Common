using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class BaseAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Raw { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", this.Address1, this.Address2, this.City, this.State, this.Zip, this.Country);
        }
    }
}
