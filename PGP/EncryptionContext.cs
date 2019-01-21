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
                this.Password = GDPRCore.Current.GetApplicationKey(msg.ApplicationId, msg.KeyVersion);
                this.IsApplication = true;
            }

            this.Path = Utility.GetPath();
        }

        public static EncryptionContext Default
        {
            get
            {
                string systemId = GDPRCore.Current.GetSystemId().ToString();

                EncryptionContext ctx = new EncryptionContext();
                ctx.Encrypt = Configuration.EnableEncryption;
                ctx.Id = systemId;
                int version = GDPRCore.Current.GetSystemKeyVersion(Guid.Parse(systemId));
                ctx.Version = version;
                ctx.Password = GDPRCore.Current.GetSystemKey(systemId, version);
                ctx.IsApplication = false;
                ctx.Path = Utility.GetPath();
                
                return ctx;
            }
        }

        public static EncryptionContext CreateForApplication(Guid applicationId)
        {
            //get the lastest version...
            int version = GDPRCore.Current.GetApplicationKeyVersion(applicationId);
            return CreateForApplication(applicationId, version);

        }

        public static EncryptionContext CreateForApplication(Guid applicationId, int version)
        {
            EncryptionContext ctx = new EncryptionContext();
            ctx.IsApplication = true;
            ctx.Id = applicationId.ToString();
            ctx.Encrypt = Configuration.EnableEncryption;
            ctx.Path = Utility.GetPath();
            ctx.Version = version;
            ctx.Password = GDPRCore.Current.GetApplicationKey(applicationId.ToString(), version);
            return ctx;
        }
    }

}