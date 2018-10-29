using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GDPR.Common;

namespace GDPR.Util.GDPRVerificationService
{    
    public class AwsV4SignatureHelper
    {
        public const string Iso8601DateTimeFormat = "yyyyMMddTHHmmssZ";
        public const string Iso8601DateFormat = "yyyyMMdd";

        private readonly string _awsSecretKey;
        private readonly string _service;
        private readonly string _region;

        public AwsV4SignatureHelper(string awsSecretKey, string service, string region = null)
        {
            _awsSecretKey = awsSecretKey;
            _service = service;
            _region = region ?? "us-east-1";
        }

        /// <summary>
        /// Calculates request signature string using Signature Version 4.
        /// http://docs.aws.amazon.com/general/latest/gr/sigv4_signing.html
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="signedHeaders">Canonical headers that are a part of a signing process</param>
        /// <param name="requestDate">Date and time when request takes place</param>
        /// <returns>Signature</returns>        
        public string CalculateSignature(HttpRequestMessage req, string[] signedHeaders, DateTime requestDate, string post)
        {
            signedHeaders = signedHeaders.Select(x => x.Trim().ToLowerInvariant()).OrderBy(x => x).ToArray();

            var canonicalRequest = GetCanonicalRequest(req, signedHeaders, post);
            var stringToSign = GetStringToSign(requestDate, canonicalRequest);
            return GetSignature(requestDate, stringToSign);
        }

        // http://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html        
        private static string GetCanonicalRequest(HttpRequestMessage request, string[] signedHeaders, string post)
        {
            var canonicalRequest = new StringBuilder();
            canonicalRequest.AppendFormat("{0}\n", request.Method);
            canonicalRequest.AppendFormat("{0}\n", request.RequestUri.AbsolutePath);
            canonicalRequest.AppendFormat("{0}\n", GetCanonicalQueryParameters(request.RequestUri.ParseQueryString()));
            canonicalRequest.AppendFormat("{0}\n", GetCanonicalHeaders(request, signedHeaders));
            canonicalRequest.AppendFormat("{0}\n", String.Join(";", signedHeaders));
            canonicalRequest.Append(GetPayloadHash(post));
            return canonicalRequest.ToString();
        }

        private static string GetCanonicalQueryParameters(NameValueCollection queryParameters)
        {
            StringBuilder canonicalQueryParameters = new StringBuilder();
            foreach (string key in queryParameters)
            {
                canonicalQueryParameters.AppendFormat("{0}={1}&", Utility.UrlEncode(key),
                    Utility.UrlEncode(queryParameters[key]));
            }

            // remove trailing '&'
            if (canonicalQueryParameters.Length > 0)
                canonicalQueryParameters.Remove(canonicalQueryParameters.Length - 1, 1);

            return canonicalQueryParameters.ToString();
        }

        private static string GetCanonicalHeaders(HttpRequestMessage request, IEnumerable<string> signedHeaders)
        {                        
            var headers = request.Headers.ToDictionary(x => x.Key.Trim().ToLowerInvariant(),
                                                       x => String.Join(" ", x.Value).Trim());            

            var sortedHeaders = new SortedDictionary<string, string>(headers);

            StringBuilder canonicalHeaders = new StringBuilder();
            foreach (var header in sortedHeaders.Where(header => signedHeaders.Contains(header.Key)))
            {
                canonicalHeaders.AppendFormat("{0}:{1}\n", header.Key, header.Value);
            }
            return canonicalHeaders.ToString();
        }

        private static string GetPayloadHash(string post)
        {            
            var payload = post != null ? post : "";
            return Utility.ToHex(Utility.Hash(payload));
        }

        // http://docs.aws.amazon.com/general/latest/gr/sigv4-create-string-to-sign.html
        private string GetStringToSign(DateTime requestDate, string canonicalRequest)
        {
            var dateStamp = requestDate.ToString(Iso8601DateFormat, CultureInfo.InvariantCulture);
            var scope = string.Format("{0}/{1}/{2}/{3}", dateStamp, _region, _service, "aws4_request");

            var stringToSign = new StringBuilder();
            stringToSign.AppendFormat("AWS4-HMAC-SHA256\n{0}\n{1}\n",
                                      requestDate.ToString(Iso8601DateTimeFormat, CultureInfo.InvariantCulture),
                                      scope);
            stringToSign.Append(Utility.ToHex(Utility.Hash(canonicalRequest)));
            return stringToSign.ToString();
        }

        // http://docs.aws.amazon.com/general/latest/gr/sigv4-calculate-signature.html
        private string GetSignature(DateTime requestDate, string stringToSign)
        {
            var kSigning = GetSigningKey(requestDate);
            return Utility.ToHex(Utility.GetKeyedHash(kSigning, stringToSign));
        }

        private byte[] GetSigningKey(DateTime requestDate)
        {
            var dateStamp = requestDate.ToString(Iso8601DateFormat, CultureInfo.InvariantCulture);
            var kDate = Utility.GetKeyedHash("AWS4" + _awsSecretKey, dateStamp);
            var kRegion = Utility.GetKeyedHash(kDate, _region);
            var kService = Utility.GetKeyedHash(kRegion, _service);
            return Utility.GetKeyedHash(kService, "aws4_request");
        }        
    }
}
