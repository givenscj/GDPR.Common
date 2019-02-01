using GDPR.Common.Classes;
using GDPR.Common.Core;
using GDPR.Common.Messages;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class Utility
    {
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

        public static bool IsEmpty(JToken token)
        {
            return (token.Type == JTokenType.Null);
        }
        public static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(Configuration.AzureClientId, Configuration.AzureClientSecret);
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
                path = System.Web.HttpContext.Current.Server.MapPath("");
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

        static public bool SendMessage(GDPRMessageWrapper message)
        {
            GDPRCore.Current.Log($"Sending message wrapper : {message.Type}");

            string eventHubName = Configuration.EventHubName;

            if (message.IsError)
                eventHubName = Configuration.EventErrorHubName;

            if (!message.IsSystem)
                eventHubName = message.ApplicationId;

            string connectionStringBuilder = Configuration.EventHubConnectionString + ";EntityPath=" + eventHubName;

            //pick a different queue based on the message type..
            switch (message.Type)
            {
                case "DiscoverResponsesMessage":
                    break;
                case "DiscoverMessage":
                    break;
                case "PreApprovalMessage":
                    break;
                case "PostApprovalMessage":
                    break;
            }

            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(msg)));
            //eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(msg)));
            //eventHubClient.CloseAsync();

            return true;
        }
    }
}
