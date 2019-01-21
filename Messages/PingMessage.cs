using GDPR.Common.Core;

namespace GDPR.Common.Messages
{
    public class PingMessage : BaseGDPRMessage
    {
        public override bool Process()
        {
            PongMessage pm = new PongMessage();
            pm.ApplicationId = this.ApplicationId;
            GDPRCore.Current.SendMessage(pm, EncryptionContext.Default);
            return true;
        }
    }
}
