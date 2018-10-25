using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Storage
{
    public abstract class Storage
    {
        public abstract string UploadBlob(Guid applicationId, string fileName);
    }
}
