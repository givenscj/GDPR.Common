using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using GDPR.Common;
using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Util.GDPRVerificationService;

namespace GDPR.Common
{
    public class HttpHelper
    {

        HttpWebRequest req;
        System.Net.WebProxy proxy;

        public String downloadLocation = "c:\\temp";
        public String certificateName = "";
        public String certificatePassword = "";
        public String fileName = "";
        public String acceptOverride = String.Empty;
        public String contentTypeOverride = "";
        public bool ignoreContentDisposition = false;
        public String methodOverride = "";
        public Hashtable headers = new Hashtable();
        public String Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";

        public String username = "";
        public String password = "";
        public String domain = "";

        public String host = "";

        public Hashtable urlCookies = new Hashtable();

        public bool allowRedirects = false;
        public String referer = "";
        public String language = "en-US";
        CookieCollection cookieC;

        public String cookies = "";
        public String location = "";
        public String statusCode = "";
        public int httpCode = -1;

        bool isAmazon = false;
        AwsV4SignatureHelper amazonHelper;

        public HttpHelper()
        {

        }

        public HttpHelper(AwsV4SignatureHelper h)
        {
            this.isAmazon = true;
            this.amazonHelper = h;

        }

        public void SetupHttpRequest(HttpWebRequest req)
        {
            ServicePointManager.Expect100Continue = false;
            req.ServicePoint.Expect100Continue = false;
            req.AllowAutoRedirect = allowRedirects;
            req.UseDefaultCredentials = true;
            req.UserAgent = Agent;
            req.Timeout = 200000;

            if (!String.IsNullOrEmpty(certificateName) && !String.IsNullOrEmpty(certificatePassword))
            {
                SecureString pass = ConvertToSecureString(certificatePassword);
                X509Certificate2 cert = new X509Certificate2(certificateName, pass);
                if (cert != null)
                {
                    req.ClientCertificates.Add(cert);
                    req.ServerCertificateValidationCallback += delegate { return true; };
                }
            }

            if (proxy != null) req.Proxy = proxy;

            if (!string.IsNullOrEmpty(referer))
            {
                req.Referer = referer;
                referer = "";
            }

            if (req.Method != "GET")
            {
                if (contentTypeOverride.Length > 0)
                {
                    req.ContentType = contentTypeOverride;
                    contentTypeOverride = "";
                }
                else req.ContentType = "application/x-www-form-urlencoded";
            }

            if (acceptOverride.Length > 0)
            {
                req.Accept = acceptOverride;
                acceptOverride = "";
            }
            else req.Accept = "*/*";

            foreach (String key in headers.Keys)
            {
                req.Headers.Add(key.ToString(), headers[key].ToString());
            }

            headers.Clear();

            req.Headers.Add("Accept-Language", language);
            req.Headers.Add("Accept-Encoding", "gzip, deflate");
        }

        public string DoPost(string url, string postData, string cookies)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] data = encoding.GetBytes(postData);

            return DoPost(url, data, cookies);
        }

        public string DoPost(string url, byte[] data, string cookies)
        {
            return DoPostWork(url, data, "POST", cookies);
        }

        public string DoDelete(string url, string postData, string cookies)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            return DoDelete(url, data, cookies);
        }

        public string DoDelete(string url, byte[] data, string cookies)
        {
            return DoPostWork(url, data, "DELETE", cookies);
        }

        public string DoPostWork(string url, byte[] data, string type, string cookies)
        {
            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(url);
            httpWReq.UseDefaultCredentials = true;

            if (this.methodOverride.Length > 0)
            {
                httpWReq.Method = this.methodOverride;
                this.methodOverride = "";
            }
            else
                httpWReq.Method = type;

            SetupHttpRequest(httpWReq);

            String cookie = GetCookies(url);

            if (!String.IsNullOrEmpty(cookie))
                httpWReq.Headers.Add(cookie);

            httpWReq.ContentLength = data.Length;

            if (url == "https://login.microsoftonline.com/common/SAS/ProcessAuth")
            {
                //httpWReq.Connection = "keep-alive";
            }

            try
            {
                if (data.Length > 0)
                {
                    using (Stream stream = httpWReq.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }
            catch (Exception) { }

            String responseString = ProcessResponse(httpWReq);

            //TODO Save response...

            return responseString;
        }

        public string DoPut(string url, string postData, string cookies)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            return DoPut(url, data, cookies);
        }

        public string DoPatch(string url, string postData, string cookies)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(postData);

            return DoPatch(url, data, cookies);
        }

        public string DoPatch(string url, byte[] data, string cookies)
        {
            return DoPostWork(url, data, "PATCH", cookies);
        }

        public string DoPut(string url, byte[] data, string cookies)
        {
            return DoPostWork(url, data, "PUT", cookies);
        }

        public string DoGet(string url, string cookies)
        {
            String response = "";

            try
            {
                req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "GET";

                if (!string.IsNullOrEmpty(host))
                    req.Host = this.host;

                SetupHttpRequest(req);

                String cookie = GetCookies(url);

                if (!String.IsNullOrEmpty(cookie) && cookie != "Cookie: ")
                    req.Headers.Add(cookie);

                response = ProcessResponse(req);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return response;
        }

        public static SecureString ConvertToSecureString(String password)
        {
            if (password == null)
                throw new ArgumentNullException("password");

            var securePassword = new SecureString();

            foreach (char c in password)
                securePassword.AppendChar(c);

            securePassword.MakeReadOnly();
            return securePassword;
        }


        static public Hashtable ParseCookies(String html)
        {
            Hashtable ht = new Hashtable();

            if (string.IsNullOrEmpty(html))
                return ht;

            String[] cookies = html.Split(';');

            foreach (String c in cookies)
            {
                String c1 = c.Replace("HttpOnly,", "").Replace("httponly,", "").Replace("HTTPOnly,", ""); ;

                c1 = c1.Replace("SameSite=Lax,", "");
                c1 = c1.Replace("secure,", "");
                c1 = c1.Replace("Secure,", "");

                if (c1.Trim().Contains("HttpOnly"))
                    continue;

                string expireText = Utility.ParseValue(c1, "expires=", "GMT,");
                c1 = c1.Replace("expires=" + expireText + "GMT,", "");

                if (c1.Contains("expires="))
                    continue;

                if (c1.Contains("Expires="))
                    continue;

                if (c1.Contains("domain="))
                    continue;

                if (c1.Contains("Domain="))
                    continue;

                if (c1.Contains("path="))
                    continue;

                if (c1.Contains("Path="))
                    continue;

                if (c1.Trim() == "secure")
                    continue;

                if (c1.Contains("="))
                {
                    String value = c1.Substring(c1.IndexOf("=") + 1);
                    String name = c1.Substring(0, c1.IndexOf("="));

                    if (ht.ContainsKey(name.Trim()))
                        ht[name.Trim()] = value;
                    else
                        ht.Add(name.Trim(), value);
                }
            }

            return ht;
        }

        public void SetCookies(String url, String cookies)
        {
            SetCookies(url, ParseCookies(cookies));
        }

        public void SetCookies(String url, Hashtable incoming)
        {
            Uri u = new Uri(url);

            if (urlCookies.Contains(u.Host))
            {
                //get current cookies...
                Hashtable current = (Hashtable)urlCookies[u.Host];

                foreach (String key in incoming.Keys)
                {
                    if (current.ContainsKey(key))
                    {
                        current[key] = incoming[key];
                    }
                    else
                    {
                        current.Add(key, incoming[key]);
                    }
                }
            }
            else
            {
                urlCookies.Add(u.Host, incoming);
            }

            return;
        }

        public String GetCookies(String url)
        {
            Uri u = new Uri(url);

            if (urlCookies.Contains(u.Host))
            {
                return "Cookie: " + FormatCookie((Hashtable)urlCookies[u.Host]);
            }

            return "Cookie: ";
        }

        static public String FormatCookie(Hashtable ht)
        {
            String cookie = "";

            foreach (String key in ht.Keys)
            {
                cookie += key.Trim() + "=" + ht[key] + "; ";
            }

            return cookie;
        }

        public String ProcessResponse(HttpWebRequest req)
        {
            this.fileName = "";

            try
            {
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                return ProcessResponse(res);
            }
            catch (WebException ex)
            {
                HttpWebResponse res = (HttpWebResponse)ex.Response;
                return ProcessResponse(res);
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex.Message);
            }

            return "";
        }

        public String ProcessResponse(HttpWebResponse res)
        {
            String response = "";

            try
            {
                statusCode = res.StatusCode.ToString();
                httpCode = (int)res.StatusCode;
                cookieC = res.Cookies;

                try
                {
                    location = res.Headers["Location"].ToString();
                }
                catch (Exception) { location = null; }

                try
                {
                    cookies = res.Headers["set-cookie"].ToString();

                    SetCookies(res.ResponseUri.ToString(), cookies);
                }
                catch (Exception) { }

                long length = 0;

                try
                {
                    fileName = res.Headers["Content-Disposition"].ToString();

                    if (fileName.Contains("inline"))
                        fileName = "";

                    fileName = fileName.Replace("attachment; filename=", "");

                    if (fileName.Contains("\""))
                        fileName = Utility.ParseValue(fileName, "\"", "\"");

                    length = res.ContentLength;
                }
                catch (Exception) { }

                if (fileName == "json.txt")
                {
                    fileName = "";
                }

                if (fileName.Length > 0 && !ignoreContentDisposition)
                {
                    // Define buffer and buffer size
                    int bufferSize = 2048;
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead = 0;

                    // Read from response and write to file
                    Stream strm = res.GetResponseStream();
                    FileStream fileStream = System.IO.File.Create($@"{downloadLocation}\{fileName}");
                    while ((bytesRead = strm.Read(buffer, 0, bufferSize)) != 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    } // end while

                    fileStream.Close();
                    strm.Close();
                }
                else
                {
                    Stream responseStream = res.GetResponseStream();

                    if (res.ContentEncoding.ToLower().Contains("gzip"))
                        responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

                    else if (res.ContentEncoding.ToLower().Contains("deflate"))
                        responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    response = reader.ReadToEnd();

                    res.Close();
                    responseStream.Close();

                    req = null;
                    proxy = null;
                }

            }
            catch (WebException ex)
            {
                HttpWebResponse exRes = (HttpWebResponse)ex.Response;

                Stream responseStream = exRes.GetResponseStream();

                if (exRes.ContentEncoding.ToLower().Contains("gzip"))
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);

                else if (exRes.ContentEncoding.ToLower().Contains("deflate"))
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                response = reader.ReadToEnd();

                if (response.Contains("\"error\""))
                {
                    //CheckForError(response);
                }
            }
            catch (Exception) { }

            return response;
        }

        public void SaveCacheValue(string html, HtmlCacheSetting cs)
        {
            string fileName = GetCacheFileName(cs);
            System.IO.File.AppendAllText(fileName, html);
        }

        public string DoGetCache(string url, string strCookies, HtmlCacheSetting cs)
        {
            string fileName = GetCacheFileName(cs);

            if (System.IO.File.Exists(fileName) && !cs.Overwrite)
            {
                headers.Clear();
                return System.IO.File.ReadAllText(fileName);
            }
            else
            {
                string html = DoGet(url, strCookies);

                if (httpCode == 200)
                {
                    try
                    {
                        System.IO.File.AppendAllText(fileName, html);
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                    return html;
                }
            }

            return null;
        }

        public string DoPostCache(string url, string post, string strCookies, HtmlCacheSetting cs)
        {
            string fileName = GetCacheFileName(cs);

            if (System.IO.File.Exists(fileName) && !cs.Overwrite)
            {
                this.headers.Clear();
                return System.IO.File.ReadAllText(fileName);
            }
            else
            {
                string html = DoPost(url, post, strCookies);

                if (httpCode == 200)
                {
                    FileInfo fi = new FileInfo(fileName);
                    System.IO.Directory.CreateDirectory(fi.Directory.FullName);
                    System.IO.File.AppendAllText(fileName, html);
                    return html;
                }
            }

            return null;
        }

        public string GetCacheFileName(HtmlCacheSetting cs)
        {
            string cachePath = $"c:\\temp\\HttpCache\\{cs.Category}\\";

            try
            {
                System.IO.Directory.CreateDirectory(cachePath);
            }
            catch { }

            string fileName = "";
            string prefix = "";

            DateTime check = DateTime.Now;

            if (cs.DateOverride != DateTime.MinValue)
            {
                check = cs.DateOverride;
            }

            switch (cs.Frequency.ToString())
            {
                case "Daily":
                    {
                        prefix = "D" + check.ToString("MM-dd-yyyy");
                        break;
                    }
                case "Hourly":
                    {
                        prefix = "H" + check.ToString("hh-MM-dd-yyyy");
                        break;
                    }
                case "Monthly":
                    {
                        prefix = "M" + check.ToString("MM-yyyy");
                        break;
                    }
                case "Weekly":
                    {
                        prefix = "W" + check.ToString("w");
                        break;
                    }
                case "Yearly":
                    {
                        prefix = "Y" + check.ToString("yyyy");
                        break;
                    }
            }

            string regexSearch = new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            cachePath = r.Replace(cachePath, "");

            fileName = prefix + "_" + cs.Category + "_" + cs.Id + ".html";

            //parse bad characters...
            regexSearch = new string(Path.GetInvalidFileNameChars());
            r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            fileName = r.Replace(fileName, "");

            fileName = cachePath + fileName;

            return fileName;
        }
    }
}