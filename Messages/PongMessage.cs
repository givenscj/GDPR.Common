using GDPR.Common.Core;

namespace GDPR.Common.Messages
{
    public class PongMessage : BaseGDPRMessage
    {
        public override bool Process()
        {
            if (string.IsNullOrEmpty(this.Status))
            {
                this.Status = "Empty response received";
            }

            //update the status...
            GDPRCore.Current.UpdateApplicationStatus(this.ApplicationId, this.Status);

            return true;
        }
    }
}
