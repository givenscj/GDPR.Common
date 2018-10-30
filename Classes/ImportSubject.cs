using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.Classes
{
    public class ImportSubject
    {
        //personal info
        public string SubjectId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Birthdate { get; set; }
        public string Weight { get; set; }
        public string Height { get; set; }
        public string Gender { get; set; }

        //only valid emails will import
        public string EmailAddress1 { get; set; }
        public string EmailAddress2 { get; set; }
        public string EmailAddress3 { get; set; }

        //the full address, will be geocoded on import
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        //ensure you place a "+" in front of international numbers
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Phone3 { get; set; }

        //valid values {Twitter-@givenscj, Instagram-@givenscj, Facebook-@12345678}
        public string SocialIdentity1 { get; set; }
        public string SocialIdentity2 { get; set; }
        public string SocialIdentity3 { get; set; }

        //valid values {DriverLicense-12345678, Passport-12345678, EmployeeId-12345678}
        public string Identity1 { get; set; }
        public string Identity2 { get; set; }
        public string Identity3 { get; set; }
    }
}
