using System;

namespace GDPR.Common.Models
{
    public class OAuthResult
    {
        public string AccessToken { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public DateTime Expires { get; set; }
    }
}
