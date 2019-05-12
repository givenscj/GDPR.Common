using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace GDPR.Common
{
    public class BasePhone
    {
        virtual public string CountryCode { get; set; }
        virtual public string NationalNumber { get; set; }
        virtual public string Raw { get; set; }

        //ideally all phone numbers should be in E.164 format in order for them to be searched
        public static BasePhone Parse(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                string parsedNumber = GetNumbers(phoneNumber);

                if (phoneNumber.Contains("+"))
                    parsedNumber = "+" + parsedNumber;

                libphonenumber.PhoneNumber pn = libphonenumber.PhoneNumberUtil.Instance.Parse(parsedNumber, "US");
                BasePhone p = new BasePhone();
                p.CountryCode = pn.CountryCode.ToString();
                p.NationalNumber = pn.NationalNumber.ToString();
                p.Raw = p.ToString();

                return p;
            }

            return null;
        }

        public static string Parse(string phoneNumber, bool useDots, bool includePlus, bool includeCountry, bool addParentheses, bool addSpace, bool addDashes)
        {
            StringBuilder sb = new StringBuilder();

            BasePhone bp = Parse(phoneNumber);

            string areaCode = bp.NationalNumber.Substring(0, 3);
            string prefix = bp.NationalNumber.Substring(3, 3);
            string code = bp.NationalNumber.Substring(6, 4);

            if (useDots && includeCountry)
            {
                sb.Append(bp.CountryCode);
                sb.Append(".");
                sb.Append(areaCode);
                sb.Append(".");
                sb.Append(prefix);
                sb.Append(".");
                sb.Append(code);
                return sb.ToString();
            }

            if (useDots && !includeCountry)
            {
                sb.Append(areaCode);
                sb.Append(".");
                sb.Append(prefix);
                sb.Append(".");
                sb.Append(code);
                return sb.ToString();
            }

            if (includePlus)
                sb.Append("+");

            if (includeCountry)
                sb.Append(bp.CountryCode);

            if (addSpace)
                sb.Append(" ");

            if (addParentheses)
            {
                sb.Append("(" + areaCode + ")");
            }
            else
                sb.Append(areaCode);

            if (addSpace)
                sb.Append(" ");

            if (addDashes)
            {
                sb.Append(prefix + "-" + code);
            }
            else
            {
                if (addSpace)
                    sb.Append(prefix + " " + code);
                else
                    sb.Append(prefix + code);
            }

            return sb.ToString();
        }

        static public string GetNumbers(string input)
        {
            if (input != null)
                return new string(input.Where(c => char.IsDigit(c)).ToArray());

            return input;
        }

        public override string ToString()
        {
            return this.CountryCode + this.NationalNumber;
        }

        public static Hashtable GetAllFormats(string raw)
        {
            Hashtable formats = new Hashtable();

            for (int a = 0; a <= 1; a++)
            {
                for (int b = 0; b <= 1; b++)
                {
                    for (int c = 0; c <= 1; c++)
                    {
                        for (int d = 0; d <= 1; d++)
                        {
                            for (int e = 0; e <= 1; e++)
                            {
                                for (int f = 0; f <= 1; f++)
                                {
                                    string newRaw = Parse(raw, a == 1, b == 1, c == 1, d == 1, e == 1, f==1).Trim();

                                    if (!formats.ContainsKey(newRaw))
                                        formats.Add(newRaw, newRaw);
                                }
                            }
                        }
                    }
                }
            }

            return formats;
        }
    }
}
