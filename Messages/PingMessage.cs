using GDPR.Common.Core;

namespace GDPR.Common.Messages
{
    public class PingMessage : BaseGDPRMessage
    {
        public override bool Process()
        {
            PongMessage pm = new PongMessage();
            pm.Status = $"Hello from {this.ApplicationId}";
            pm.ApplicationId = this.ApplicationId;
            pm.QueueUri = this.QueueUri;
            GDPRCore.Current.SendMessage(pm, EncryptionContext.Default);
            return true;
        }
    }
}
