using System;
using System.Collections.Generic;

namespace GDPR.Common
{
    public class GDPRSubject
    {
        public List<GDPRSubjectPhone> Phones { get; set; }
        public List<GDPRSubjectAddress> Addresses { get; set; }
        public List<GDPRSubjectIdentity> Identities { get; set; }
        public List<GDPRSubjectBiometric> Biometrics { get; set; }
        public List<GDPRSubjectDna> Dnas { get; set; }
        public List<GDPRSubjectEmail> EmailAddresses { get; set; }
        public List<GDPRSubjectSocialIdentity> SocialIdentities { get; set; }
        public List<GDPRSubjectIpAddress> IpAddresses { get; set; }
        public string ApplicationSubjectId { get; set; }
        public string ProcessorId { get; set; }
        public bool IsLocked { get; set; }
        public DateTime LockedDate { get; set; }
        public Guid SubjectId { get; set; }
        public bool IsMinor { get; set; }
        public bool IsEmployee { get; set; }
        public DateTime BirthDate { get; set; }
        public string TimeZone { get; set; }
        public string Gender { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool ConsentGiven { get; set; }
        public DateTime ConsentDate { get; set; }
        public string ConsentMessage { get; set; }
        public string DefaultEmail { get; set; }
        public string DefaultPhone { get; set; }
        public System.Collections.Hashtable Attributes { get; set; }
        
        public GDPRSubject()
        {
            this.Addresses = new List<GDPRSubjectAddress>();
            this.EmailAddresses = new List<GDPRSubjectEmail>();
            this.Phones = new List<GDPRSubjectPhone>();
            this.Identities = new List<GDPRSubjectIdentity>();
            this.SocialIdentities = new List<GDPRSubjectSocialIdentity>();
            this.IpAddresses = new List<GDPRSubjectIpAddress>();
            this.Dnas = new List<GDPRSubjectDna>();
            this.Biometrics = new List<GDPRSubjectBiometric>();
        }

        public void AddEmail(string email)
        {
            if (Utility.IsValidEmail(email))
                this.EmailAddresses.Add(new GDPRSubjectEmail { EmailAddress = email });
        }

        public GDPRSubject(GDPRSubject subject)
        {
            this.SubjectId = subject.SubjectId;
            this.FirstName = subject.FirstName;
            this.LastName = subject.LastName;
            this.EmailAddresses = subject.EmailAddresses;
            this.Phones = subject.Phones;
            this.Identities = subject.Identities;
            this.Addresses = subject.Addresses;
            this.SocialIdentities = subject.SocialIdentities;
            this.Dnas = subject.Dnas;
            this.Biometrics = subject.Biometrics;

        }

        public void AddPhone(string phone)
        {
            this.Phones.Add(new GDPRSubjectPhone { Raw = phone });
        }
    }
}
