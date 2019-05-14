using System;
using System.Collections.Generic;

namespace GDPR.Common
{
    public class GDPRSubjectAlias
    {
        public Guid SubjectId { get; set; }
        public DateTime BirthDate { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public GDPRSubjectAlias()
        { }

        public GDPRSubjectAlias(GDPRSubject subject)
        {
            this.SubjectId = subject.SubjectId;
            this.FirstName = subject.FirstName;
            this.LastName = subject.LastName;            
        }        
    }
}
