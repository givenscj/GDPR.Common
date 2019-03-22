namespace GDPR.Applications
{
    public abstract class GDPROAuthApplicationBase : GDPRApplicationBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool OAuthFallbackEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="overwrite"></param>
        public override void BuildProperties(bool overwrite)
        {
            base.BuildProperties(overwrite);

            CreateOAuthProperties(overwrite);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        abstract public string GetAuthorizationUrl();
    }
}
