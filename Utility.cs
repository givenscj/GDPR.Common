using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Common.Messages;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class Utility
    {
        public static List<string> GetInternetIp()
        {
            List<string> items = new List<string>();

            HttpHelper hh = new HttpHelper();
            string html = hh.DoGet("https://www.google.com/search?q=what+is+my+ip", "");
            string temp = Utility.ParseValue(html, "w-answer-desktop>", "/div>");
            string ipAddress = Utility.ParseValue(temp, ">", "<");

            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                items.Add(ip.MapToIPv6().ToString());
                items.Add(ip.MapToIPv4().ToString());
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex, GDPR.Common.Enums.LogLevel.Error);
            }

            html = hh.DoGet("http://whatismyip.host/ ", "");
            ipAddress = Utility.ParseValue(html, "<p class=\"ipaddress\">", "<");

            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                items.Add(ip.MapToIPv6().ToString());
                items.Add(ip.MapToIPv4().ToString());
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex, GDPR.Common.Enums.LogLevel.Error);
            }

            items.Add("127.0.0.1");

            return items;
        }
        public static JToken RemoveEmptyChildren(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                JObject copy = new JObject();
                foreach (JProperty prop in token.Children<JProperty>())
                {
                    JToken child = prop.Value;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }
                    if (!IsEmpty(child))
                    {
                        copy.Add(prop.Name, child);
                    }
                }
                return copy;
            }
            else if (token.Type == JTokenType.Array)
            {
                JArray copy = new JArray();
                foreach (JToken item in token.Children())
                {
                    JToken child = item;
                    if (child.HasValues)
                    {
                        child = RemoveEmptyChildren(child);
                    }
                    if (!IsEmpty(child))
                    {
                        copy.Add(child);
                    }
                }
                return copy;
            }
            return token;
        }

        internal static Type LoadType(string applicationClass)
        {
            GDPRCore.Current.Log($"Loading application class : {applicationClass}");

            Type t = System.Type.GetType(applicationClass);
            
            if (t == null)
            {
                Assembly[] asses = AppDomain.CurrentDomain.GetAssemblies();

                foreach(Assembly a in asses)
                {
                    t = a.GetType(applicationClass);
                    if (t != null)
                        return t;
                }
            }

            GDPRCore.Current.Log($"Application class not found : {applicationClass}");

            return null;
        }

        public static void LoadAssemblies()
        {
            string appStubPath = Utility.GetPath() + GDPR.Common.Configuration.AppStubPath;
            LoadAssemblies(appStubPath);
        }

        public static void LoadAssemblies(string dirPath)
        {
            GDPRCore.Current.Log($"Loading application stubs : {dirPath}");

            if (!string.IsNullOrEmpty(dirPath))
            {
                DirectoryInfo di = new DirectoryInfo(dirPath);

                foreach (FileInfo fi in di.GetFiles("*.dll"))
                {
                    try
                    {
                        GDPRCore.Current.Log($"Loading : {fi.FullName}");
                        Assembly.LoadFrom(fi.FullName);
                    }
                    catch (Exception ex)
                    {
                        GDPRCore.Current.Log(ex, GDPR.Common.Enums.LogLevel.Error);
                    }
                }
            }
        }

        public static Guid GetCharacterGuid()
        {
            Guid g = Guid.NewGuid();

            while (!Char.IsLetter(g.ToString().ToCharArray()[0]))
            {
                g = Guid.NewGuid();

            }

            return g;
        }

        public async Task<dynamic> DoAzureCall(string url, string token)
        {
            var uri = new Uri(url);
            var content = new StringContent(string.Empty, Encoding.UTF8, "text/html");

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization
                    = new AuthenticationHeaderValue("Bearer", token);

                using (var response = await httpClient.PostAsync(uri, content))
                {
                    var responseText = await response.Content.ReadAsStringAsync();

                    dynamic json = JsonConvert.DeserializeObject(responseText);
                    return json;
                }
            }

            return null;
        }

        public static bool IsEmpty(JToken token)
        {
            return (token.Type == JTokenType.Null);
        }

        public static async Task<string> GetMSIToken(string authority, string resource, string scope)
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            string token = await azureServiceTokenProvider.GetAccessTokenAsync(resource);
            
            if (token == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return token;
        }

        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(Configuration.AdminAzureClientId, Configuration.AdminAzureClientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain the JWT token");

            return result.AccessToken;
        }

        public static string GetToken(string authority, string clientId, string appKey, string resource)
        {
            var httpClient = new HttpClient();
            var authContext = new AuthenticationContext(authority);
            var clientCredential = new ClientCredential(clientId, appKey);
            var result = authContext.AcquireTokenAsync(resource, clientCredential);
            return result.Result.AccessToken;
        }

        public static string GetConfigurationValue(string v)
        {
            string value = "";

           object val = Configuration.GetProperty(v);

            if (val == null)
                value = ConfigurationManager.AppSettings[v];
            else
                value = val.ToString();

            return value;
        }

        private const string ValidUrlCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string UrlEncode(string data)
        {
            StringBuilder encoded = new StringBuilder();
            foreach (char symbol in Encoding.UTF8.GetBytes(data))
            {
                if (ValidUrlCharacters.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
                }
            }
            return encoded.ToString();
        }

        public static string UnicodeToAscii(string stuff)
        {
            string unicodestring = stuff;

            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;
            //Encoding Utf8 = Encoding.UTF8;

            // // Convert the string into a byte array.
            byte[] unicodeBytes = unicode.GetBytes(unicodestring);

            // // Perform the conversion from one encoding to the other.
            byte[] ascibytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // // Convert the new byte[] into a char[] and then into a string.
            char[] asciiChars = new char[ascii.GetCharCount(ascibytes, 0, ascibytes.Length)];
            ascii.GetChars(ascibytes, 0, ascibytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);
            return asciiString;
        }

        public static byte[] Hash(string value)
        {

            return new SHA256CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(value));
        }


        public static byte[] GetKeyedHash(string key, string value)
        {
            return GetKeyedHash(Encoding.UTF8.GetBytes(key), value);
        }

        public static byte[] GetKeyedHash(byte[] key, string value)
        {
            KeyedHashAlgorithm mac = new HMACSHA256(key);
            mac.Initialize();
            return mac.ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        public static string ToHex(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }
        static public String ParseValue(String line, String startToken, String endToken)
        {
            if (startToken == null)
            {
                return "";
            }

            try
            {
                if (startToken == "")
                {
                    return line.Substring(0, line.IndexOf(endToken));
                }
                else
                {

                    String rtn = line.Substring(line.IndexOf(startToken));

                    if (endToken == "")
                        return line.Substring(line.IndexOf(startToken) + startToken.Length);
                    else
                        return rtn.Substring(startToken.Length, rtn.IndexOf(endToken, startToken.Length) - startToken.Length).Replace("\n", "").Replace("\t", "");
                }
            }
            catch (Exception)
            {
                //Logger.LogGeneralMessage("Tried to find [" + startToken + "] in [" + line + "]");
            }
            finally
            {
            }

            return "";
        }

        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static string SerializeObject(object o, int depth)
        {
            //trim the request down...
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MaxDepth = depth;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args) 
            {
                GDPRCore.Current.Log(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
        }

        public static byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];
            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }
            return bytes;
        }

        public static string BytesToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] StringToBytes(string str)
        {
            return ASCIIEncoding.UTF8.GetBytes(str);
        }


        public static string StreamToString(Stream stream)
        {
            stream.Position = 0;
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s.ToCharArray());
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        internal static T GetProcessor<T>(object p)
        {
            throw new NotImplementedException();
        }

        public static T TrimObject<T>(T o, int depth)
        {
            //trim the request down...
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MaxDepth = depth;
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;
            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(o, settings);
            return JsonConvert.DeserializeObject<T>(msg);
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static void Log(string v, Enums.LogLevel information)
        {
            return;
        }

        public static MemoryStream CreateToMemoryStream(System.IO.Stream memStreamIn, string zipEntryName, string password)
        {

            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);

            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            zipStream.Password = password;

            ZipEntry newEntry = new ZipEntry(zipEntryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;    // False stops the Close also Closing the underlying stream.
            zipStream.Close();          // Must finish the ZipOutputStream before using outputMemStream.

            outputMemStream.Position = 0;
            return outputMemStream;

            // Alternative outputs:
            // ToArray is the cleaner and easiest to use correctly with the penalty of duplicating allocated memory.

            /*
            byte[] byteArrayOut = outputMemStream.ToArray();

            // GetBuffer returns a raw buffer raw and so you need to account for the true length yourself.
            byte[] byteArrayOut = outputMemStream.GetBuffer();
            long len = outputMemStream.Length;
            */
        }

        public static string GetCertPath()
        {
            string path = GetPath();

            if (System.Web.HttpContext.Current != null)
                path += Configuration.CertKeyPath;
            else
                path = Configuration.CertKeyDirectory + "\\" + Configuration.CertKeyPath;

            return path;
        }


        public static string GetPath()
        {
            string path = "";

            if (System.Web.HttpContext.Current != null)
            {
                try
                {
                    path = System.Web.HttpContext.Current.Server.MapPath("../");
                }
                catch (Exception ex)
                {
                    path = System.Web.HttpContext.Current.Server.MapPath("");
                }
            }
            else
                path = Assembly.GetExecutingAssembly().Location;

            return path;
        }

        public static string GenerateCode()
        {
            CaptchaImage ci = new CaptchaImage();
            return ci.Text;
        }

        public static string GenerateCode(int chars, bool numbersOnly)
        {
            string code = "";
            Random r = new Random();

            if (numbersOnly)
            {
                while (code.Length < chars)
                {
                    code += r.Next(9).ToString();
                }
            }
            else
            {
                code = GenerateCode();
            }

            return code;
        }

        public static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    //msi.CopyTo(gs);
                    CopyTo(msi, gs);
                }

                return mso.ToArray();
            }
        }

        public static bool ParseBoolean(string inVal)
        {
            switch(inVal)
            {
                case "1":
                    return true;
                case "0":
                    return false;
            }

            bool bAllowTrials = false;
            bool.TryParse(inVal, out bAllowTrials);

            return bAllowTrials;
        }

        public static string Unzip(byte[] bytes)
        {
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    //gs.CopyTo(mso);
                    CopyTo(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

    }
}
