namespace GDPR.Common
{
    public class BaseAddress
    {
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Country { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual string Raw { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5}", this.Address1, this.Address2, this.City, this.State, this.Zip, this.Country);
        }
    }
}
