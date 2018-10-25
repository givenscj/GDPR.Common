using GDPR.Common.Classes;
using GDPR.Common.Messages;
using GDPR.Utililty;
using GDPR.Utililty.Classes;
using GDPR.Utililty.Data;
using GDPR.Utililty.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Applications
{
    public abstract class GDPRApplicationCore
    {
        virtual public void SetProperty(string name, string value)
        {
            //to be done via platform...
        }

        virtual public string GetProperty(string name)
        {
            return "";
        }

        virtual public string ExportData(List<Record> records)
        {
            return "";
        }

        public List<BaseApplicationPolicy> GetPolicies()
        {
            return new List<BaseApplicationPolicy>();
        }

        public void SendDiscoveryMessage(BaseGDPRMessage msg)
        {

        }

        public void SaveApplicationRequest(BaseGDPRMessage msg, string status)
        {

        }
    }
}
