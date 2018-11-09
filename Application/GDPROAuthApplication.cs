namespace GDPR.Applications
{
    public abstract class BaseGDPROAuthApplication : BaseGDPRApplication
    {
        public string AccessToken { get; set; }

        public bool OAuthFallbackEnabled { get; set; }

        public override void BuildProperties(bool overwrite)
        {
            base.BuildProperties(overwrite);

            CreateOAuthProperties(overwrite);
        }

        abstract public string GetAuthorizationUrl();
    }
}
