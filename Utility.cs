using GDPR.Util.Enums;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GDPR.Common
{
    public class Utility
    {
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

        public static void Log(string v, LogLevel information)
        {
            return;
        }
    }
}
