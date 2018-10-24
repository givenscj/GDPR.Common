using GDPR.Util;
using GDPR.Util.Classes;
using GDPR.Util.Data;
using GDPR.Util.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
