using GDPR.Common.Classes;

namespace GDPR.Common.Messages
{
    public class BaseExportMessage : BaseApplicationMessage
    {
        public ExportInfo Info { get; set; }
        public override bool Process()
        {
            base.Process();

            return true;
        }
    }
}
