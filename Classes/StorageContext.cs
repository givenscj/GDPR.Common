using GDPR.Common.Core;
using GDPR.Common.Services;
using System;
using System.Collections;

namespace GDPR.Common.Classes
{
    public class StorageContext
    {
        public static IStorageService Current {get; set;}
        public string Type { get; set; }
        public string Url { get; set; }
        public string Key { get; set; }

        static StorageContext()
        {
            string storageType = Configuration.StorageService;

            //instansiate service...
            Type pType = System.Type.GetType(storageType);
            Current = (IStorageService)Activator.CreateInstance(pType);

            //setup settings...
            Current.Url = Configuration.StorageAccountKey;
            Current.Key = Configuration.StorageAccountKey;
        }

        public StorageContext(Guid entityId)
        {
            Hashtable props = GDPRCore.Current.LoadProperties(entityId);

            bool propSet = false;

            /*
            if (props.ContainsKey("MailServer"))
            {
                EntityProperty ep = ((EntityProperty) props["MailServer"]);
                this.Server = ep.Value;
                propSet = true;
            }
            */


        }
    }
}
