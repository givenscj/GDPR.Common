using GDPR.Common.Classes;
using System;

namespace GDPR.Common.Messages
{
    public class BaseExportMessage : BaseApplicationMessage
    {
        public ExportInfo Info { get; set; }
        public override bool Process()
        {
            throw new NotImplementedException();
        }
    }
}
