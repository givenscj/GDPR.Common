using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class EncryptionContext
    {
        public bool Encrypt { get; set; }
        public string Path { get; set; }
        public string Password { get; set; }
        public string Id { get; set; }
    }
}
