using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public class Question
    {
        public Question()
        {
            this.Options = new List<AnswerOption>();
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Text { get; set; }

        public string Type { get; set; }

        public List<AnswerOption> Options { get; set; }

        public AnswerOption Answer { get; set; }
    }
}
