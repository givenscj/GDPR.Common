namespace GDPR.Common.Messages
{
    public class PongMessage : BaseGDPRMessage
    {
        public override bool Process()
        {
            return true;
        }
    }
}
