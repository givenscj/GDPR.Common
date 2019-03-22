using GDPR.Common.Classes;
using GDPR.Common.Data;
using GDPR.Common.Messages;
using System.Collections.Generic;

namespace GDPR.Applications
{
    /// <summary>
    /// This class defines the very lowest level of implmentation of methods for an application.
    /// </summary>
    public abstract class GDPRApplicationCore
    {
        /// <summary>
        /// This method allows you save an entity property bag value to the backend data storage
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="isHidden">A hidden property will not display on the entities edit page</param>
        /// <param name="persist">This property will save the value back to the backend data storage.  If set to false it simply means to set the value in the instance property bag.</param>
        virtual public void SetProperty(string name, string value, bool isHidden, bool persist)
        {
            //to be done via platform or local application environment.
        }

        /// <summary>
        /// This method allows you retrieve an entity property bag value to the backend data storage
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        virtual public string GetProperty(string name)
        {
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        virtual public ExportInfo ExportData(List<Record> records)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<BaseApplicationPolicy> GetPolicies()
        {
            return new List<BaseApplicationPolicy>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void SendDiscoveryMessage(BaseGDPRMessage msg)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="status"></param>
        public void SaveApplicationRequest(BaseGDPRMessage msg, string status)
        {

        }
    }
}
