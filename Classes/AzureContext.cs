namespace GDPR.Common.Classes
{
    public class AzureContext
    {
        public AzureContext(string clientId, string clientSecret)
        {
            if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(clientSecret))
            {
                this.ClientId = clientId;
                this.ClientSecret = clientSecret;
            }
            else
            {
                //fall back to the system application...
                this.ClientId = Configuration.AzureClientId;
                this.ClientSecret = Configuration.AzureClientSecret;
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
