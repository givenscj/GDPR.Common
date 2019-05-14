namespace GDPR.Common.Models
{
    public class PgpKeySet
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public int Version { get; set; }
    }
}
