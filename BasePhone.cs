using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class BasePhone
    {
        public string CountryCode { get; set; }
        public string NationalNumber { get; set; }
        public string Raw { get; set; }

        public static BasePhone Parse(string phoneNumber)
        {
            string parsedNumber = GetNumbers(phoneNumber);

            if (phoneNumber.Contains("+"))
                parsedNumber = "+" + parsedNumber;

            libphonenumber.PhoneNumber pn = libphonenumber.PhoneNumberUtil.Instance.Parse(parsedNumber, "US");
            BasePhone p = new BasePhone();
            p.CountryCode = pn.CountryCode.ToString();
            p.NationalNumber = pn.NationalNumber.ToString();
            return p;
        }

        static public string GetNumbers(string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public override string ToString()
        {
            return this.CountryCode + this.NationalNumber;
        }
    }
}
