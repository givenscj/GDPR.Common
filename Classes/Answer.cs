using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public class AnswerOption
    {
        public AnswerOption()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public bool IsAnswer { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }        
    }
}
