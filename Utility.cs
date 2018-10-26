using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common
{
    public class Utility
    {
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
    }
}
