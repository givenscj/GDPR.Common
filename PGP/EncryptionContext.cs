using System;
using GDPR.Common.Core;
using GDPR.Common.Messages;

namespace GDPR.Common
{
    public class EncryptionContext
    {
        public bool Encrypt { get; set; }
        public string Path { get; set; }
        public string Password { get; set; }
        public int Version { get; set; }
        public string Id { get; set; }
        public bool IsApplication { get; set; }

        public EncryptionContext()
        {

        }

        public EncryptionContext(GDPRMessageWrapper msg)
        {
            this.Encrypt = msg.IsEncrypted;
            this.Version = msg.KeyVersion;

            if (msg.IsSystem)
            {
                this.Id = msg.SystemId;
                this.Password = GDPRCore.Current.GetSystemKey(msg.SystemId, msg.KeyVersion);
                this.IsApplication = false;
            }
            else
            {
                this.Id = msg.ApplicationId;
                this.Password = GDPRCore.Current.GetApplicationKey(msg.SystemId, msg.KeyVersion);
                this.IsApplication = true;
            }

            this.Path = Configuration.CertKeyPath;
        }

        public static EncryptionContext Default
        {
            get
            {
                EncryptionContext ctx = new EncryptionContext();
                ctx.Encrypt = true;
                ctx.Path = Configuration.GetProperty("PrivateKeyPath").ToString();
                ctx.Id = Configuration.GetProperty("SystemId").ToString();
                ctx.Password = Configuration.GetProperty("PrivateKeyPassword").ToString();
                return ctx;
            }
        }

        public static EncryptionContext CreateForApplication(Guid applicationId)
        {
            EncryptionContext ctx = new EncryptionContext();
            ctx.Encrypt = true;
            ctx.Path = Configuration.GetProperty("PrivateKeyPath").ToString();
            ctx.Id = applicationId.ToString();
            ctx.Password = Configuration.GetProperty("PrivateKeyPassword").ToString();
            ctx.IsApplication = true;
            return ctx;
        }
    }

}