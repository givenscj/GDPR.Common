using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public class Question
    {
        public string Text { get; set; }

        public string Type { get; set; }

        public KeyValuePair<string, string> Answers { get; set; }
    }
}
