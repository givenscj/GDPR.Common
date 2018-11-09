using System;

namespace GDPR.Util.Enums
{
    public class Authentication
    {
        public Guid AccessToken = Guid.Parse("5EF3DDD2-CE6C-4E92-99FD-05842570640A");
        public Guid UsernamePassword = Guid.Parse("8D54563C-0857-4FE6-AB66-255559D281F7");
        public Guid OAuth10 = Guid.Parse("D29DFDE7-2B92-45C1-BD75-D249225E08D1");
        public Guid OAuth20 = Guid.Parse("CD3F0695-972D-457F-B79D-3A68120C8177");
        public Guid Default = Guid.Parse("5A633B2B-6A3A-4F60-AD01-4076792A868E");
        public Guid WsFederation = Guid.Parse("1DDA04FB-C43D-4DEF-87C1-5D54FC62B302");
        public Guid None = Guid.Parse("C7B0FA66-43D2-4B7F-A501-745F2D0AA582");
        public Guid NTLM = Guid.Parse("C5E45FBD-439C-49A2-9F8D-8295C275E5CD");
    }
}
