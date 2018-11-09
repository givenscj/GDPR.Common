using GDPR.Common.Core;

namespace GDPR.Common.Classes
{
    public class OAuthContext
    {
        public OAuthContext(string clientId, string clientSecret, bool allowFallback, string type)
        {
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                this.ClientId = clientId;
                this.ClientSecret = clientSecret;
            }
            else
            {
                if (allowFallback)
                {
                    GDPRCore.Current.SetSystemOAuth(this, type);
                }
            }
        }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Resource { get; set; }
        public string Code { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string ResponseType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
