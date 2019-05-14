using System.Collections.Generic;

namespace GDPR.Common.Classes
{
    public class RequestInfo
    {
        public RequestInfo()
        {
            this.Urls = new List<string>();
        }
        public string SearchUrl { get; set; }
        public string SearchId { get; set; }
        public string SearchResponse { get; set; }
        public List<string> Urls { get; set; }
        public string FileType { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
