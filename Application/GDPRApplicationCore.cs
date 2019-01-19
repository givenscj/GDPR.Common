using GDPR.Common.Classes;
using GDPR.Common.Data;
using GDPR.Common.Messages;
using System.Collections.Generic;

namespace GDPR.Applications
{
    public abstract class GDPRApplicationCore
    {
        virtual public void SetProperty(string name, string value, bool isHidden)
        {
            //to be done via platform...
        }

        virtual public string GetProperty(string name)
        {
            return "";
        }

        virtual public ExportInfo ExportData(List<Record> records)
        {
            return null;
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
