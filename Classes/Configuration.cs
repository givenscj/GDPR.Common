using GDPR.Common.Core;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using PayPal.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace GDPR.Common
{
    public class Configuration
    {
        static Configuration()
        {
            //Get an access token for the Key Vault to get the secret out...
            try
            {
                kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Utility.GetToken));
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex, Enums.LogLevel.Error);
            }
        }

        static public void SaveToKeyVault()
        {
            PropertyInfo[] props = typeof(Configuration).GetProperties();

            foreach (PropertyInfo pi in props)
            {
                if (pi.PropertyType.FullName == "System.String")
                {
                    try
                    {
                        object val = pi.GetValue(null);

                        if (val != null && val.ToString() != "")
                        {
                            var result = Task.Run(async () =>
                            {
                                return await kv.SetSecretAsync(AzureKeyVaultUrl, pi.Name, val.ToString());
                            }).Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        GDPRCore.Current.Log(ex.Message);                        
                    }
                }
            }
        }

        static public string LoadFromKeyVault(string name)
        {
            try
            {
                string vaultUrl = AzureKeyVaultUrl;
                if (vaultUrl.EndsWith("/"))
                    vaultUrl = vaultUrl.Substring(0, vaultUrl.Length - 1);

                string uri = vaultUrl + "/secrets/" + name;
                var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                string keyValue = result.Value;
                return keyValue;
            }
            catch (Exception ex)
            {
                //GDPRCore.Current.Log(new GDPRException($"{name} was not found in {AzureKeyVaultUrl}"), LogLevel.Error);
                return null;
            }
        }

        static public string LoadFromKeyVault(string name, string version)
        {
            try
            {
                string uri = AzureKeyVaultUrl + "/secrets/" + name + "/" + version;
                var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                string keyValue = result.Value;
                return keyValue;
            }
            catch (Exception e)
            {
                //GDPRCore.Current.Log(new GDPRException($"{name} was not found in {AzureKeyVaultUrl}"), LogLevel.Error);
                return null;
            }
            
        }

        static public string SaveToKeyVault(string name, string value)
        {
            var result = Task.Run(async () =>
            {
                return await kv.SetSecretAsync(AzureKeyVaultUrl, name, value);
            }).Result;

            SetProperty(name, value);

            return result.Id.Replace(Configuration.AzureKeyVaultUrl + "/secrets/" + name + "/", "");
        }

        static public void LoadFromKeyVault()
        {
            PropertyInfo[] props = typeof(Configuration).GetProperties();

            foreach (PropertyInfo pi in props)
            {
                if (pi.PropertyType.FullName == "System.String")
                {
                    try
                    {
                        object val = pi.GetValue(null);

                        if (val == null || val.ToString() == "")
                        {
                            string keyValue = LoadFromKeyVault(pi.Name);
                            pi.SetValue(null, keyValue);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        public static void LoadWithMode(string mode)
        {
            GDPRCore.Current = new GDPRCore();

            string filePath = HostingEnvironment.MapPath($"~/configuration.{mode}.json");

            if (string.IsNullOrEmpty(filePath))
            {
                FileInfo fi = new FileInfo(filePath = Assembly.GetExecutingAssembly().Location);
                filePath = fi.Directory.FullName + $"\\configuration.{mode}.json";
            }

            //load configuartion
            _settings = LoadConfiguration(filePath);
            LoadConfiguration(_settings);

            //SaveToKeyVault();
            //LoadFromKeyVault();

            //Load application map...
            Configuration.ApplicationMap = new Hashtable();

            if (!string.IsNullOrEmpty(Configuration.Applications))
            {
                string[] apps = Configuration.Applications.Split('|');

                foreach (string app in apps)
                {
                    string[] data = app.Split('_');
                    Configuration.ApplicationMap.Add(data[0].ToLower(), data[1]);
                }
            }
        }

        public static object GetProperty(string name)
        {
            PropertyInfo[] props = typeof(Configuration).GetProperties();

            foreach (PropertyInfo pi in props)
            {
                if (pi.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return pi.GetValue(null);
                }
            }

            return null;
        }

        private static void SetProperty(string name, string value)
        {
            PropertyInfo prop = typeof(Configuration).GetProperty(name);

            if (prop != null)
                SetProperty(prop, value);
        }

        private static void SetProperty(PropertyInfo pi, string inValue)
        {
            try
            {
                object value;

                switch (pi.PropertyType.FullName)
                {
                    case "System.Int32":
                        if (!string.IsNullOrEmpty(inValue))
                        {
                            value = int.Parse(inValue);
                            pi.SetValue(null, value);
                        }
                        break;
                    case "System.Guid":
                        if (!string.IsNullOrEmpty(inValue))
                        {
                            value = Guid.Parse(inValue);
                            pi.SetValue(null, value);
                        }
                        break;
                    case "System.String":
                        value = inValue;
                        pi.SetValue(null, value);
                        break;
                    case "System.Boolean":
                        pi.SetValue(null, Boolean.Parse(inValue));
                        break;
                    default:
                        GDPRCore.Current.Log($"Configuration property type not supported {pi.PropertyType.FullName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex.Message);
            }
        }

        public static void LoadConfiguration(Hashtable ht)
        {
            PropertyInfo[] props = typeof(Configuration).GetProperties();

            foreach(string key in ht.Keys)
            {
                foreach(PropertyInfo pi in props)
                {
                    if (pi.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        SetProperty(pi, ht[key].ToString());
                    }
                }
            }
        }

        public static Hashtable LoadConfiguration(string filename)
        {
            return Load(File.ReadAllText(filename));
        }

        public static void Load(FileInfo fi)
        {
            LoadConfiguration(fi.FullName);
        }

        public static Hashtable Load(string configJson)
        {
            dynamic json = JsonConvert.DeserializeObject(configJson);

            Hashtable ht = new Hashtable();

            foreach (dynamic setting in json)
            {
                ht.Add(setting.Name, setting.Value);
            }

            return ht;
        }

        static public Hashtable ApplicationMap { get; set; }

        private static APIContext _apiContext;

        public static APIContext ApiContext
        {
            get
            {
                if (_apiContext == null)
                {
                    Dictionary<string, string> config = new Dictionary<string, string>();
                    config.Add("clientId", PaypalClientId);
                    config.Add("clientSecret", PaypalClientSecret);
                    config.Add("mode", "sandbox");
                    config.Add("requestRetries", "1");
                    config.Add("connectionTimeout", "360000");
                    ConfigManager.GetConfigWithDefaults(config);

                    try
                    {
                        // Use OAuthTokenCredential to request an access token from PayPal
                        var accessToken = new OAuthTokenCredential(config).GetAccessToken();
                        _apiContext = new APIContext(accessToken);
                    }
                    catch (Exception ex)
                    {
                        GDPRCore.Current.Log(ex.Message);
                    }
                }

                return _apiContext;
            }
            set
            {
                _apiContext = value;
            }
        }

        public static string SystemKeyVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_systemKeyVersion))
                {
                    _systemKeyVersion = LoadFromKeyVault("SystemKeyVersion");
                }

                return _systemKeyVersion;
            }
            set { _systemKeyVersion = value; }
        }

        public static string GDPRSQLConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_gdprSqlConnectionString))
                {
                    _gdprSqlConnectionString = LoadFromKeyVault("GDPRSQLConnectionString");
                }

                return _gdprSqlConnectionString;
            }
            set
            {
                _gdprSqlConnectionString = value;
            }
        }

        public static string AesKey
        {
            get
            {
                if (string.IsNullOrEmpty(_aesKey))
                {
                    _aesKey = LoadFromKeyVault("AesKey");
                }

                return _aesKey;
            }
            set
            {
                _aesKey = value;
            }
        }

        public static string StorageAccountKey
        {
            get
            {
                if (string.IsNullOrEmpty(_storageAccountKey))
                {
                    _storageAccountKey = LoadFromKeyVault("StorageAccountKey");
                }
                return _storageAccountKey;
            }
            set
            {
                _storageAccountKey = value;
            }
        }

        public static string StorageAccountSecret
        {
            get
            {
                if (string.IsNullOrEmpty(_storageAccountSecret))
                {
                    _storageAccountSecret = LoadFromKeyVault("StorageAccountSecret");
                }
                return _storageAccountSecret;
            }
            set
            {
                _storageAccountSecret = value;
            }
        }

        public static bool EnableEncryption
        {
            get
            {
                return _enableEncryption;
            }
            set
            {
                _enableEncryption = value;
            }
        }

        public static string SearchServiceName
        {
            get
            {
                return _searchServiceName;
            }
            set
            {
                _searchServiceName = value;
            }
        }

        public static string SearchService
        {
            get
            {
                return _searchService;
            }
            set
            {
                _searchService = value;
            }
        }

        public static string SearchKey
        {
            get
            {
                if (string.IsNullOrEmpty(_searchKey))
                {
                    _searchKey = LoadFromKeyVault("SearchKey");
                }

                return _searchKey;
            }
            set
            {
                _searchKey = value;
            }
        }

        public static string StorageService
        {
            get
            {
                return _storageService;
            }
            set
            {
                _storageService = value;
            }
        }

        public static string StorageRegion
        {
            get
            {
                return _storageRegion;
            }
            set
            {
                _storageRegion = value;
            }
        }

        public static string EventHubConnectionStringWithPath
        {
            get
            {
                if (string.IsNullOrEmpty(_eventHubConnectionStringWithPath))
                {
                    string eventHubName = EventHubName;
                    _eventHubConnectionStringWithPath = LoadFromKeyVault("EventHubConnectionStringWithPath") + ";EntityPath=" + eventHubName; ;
                }

                return _eventHubConnectionStringWithPath;
            }
        }

        public static void Export(string filePath, Hashtable ht)
        {
            FileInfo fi = new FileInfo(filePath);

            if (fi.Exists)
                fi.Delete();

            Configuration c = new Configuration();
            PropertyInfo[] props = c.GetType().GetProperties();

            var obj = new Dictionary<string, object>();

            foreach (PropertyInfo pi in props)
            {
                try
                {
                    object val = pi.GetValue(null);
                    string name = pi.Name;

                    if (ht.ContainsKey(name))
                        val = ht[name];
                    
                    obj.Add(pi.Name, val);
                }
                catch (Exception ex)
                {

                }
            }

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Formatting = Formatting.Indented;
            string json = JsonConvert.SerializeObject(obj, settings);
            File.AppendAllText(filePath, json);
        }

        public static string EventHubConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_eventHubConnectionString))
                {
                    _eventHubConnectionString = LoadFromKeyVault("EventHubConnectionString");
                }

                return _eventHubConnectionString;
            }
            set
            {
                _eventHubConnectionString = value;
            }
        }

        public static string Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public static string DbLogLevel
        {
            get { return _dbLogLevel; }
            set { _dbLogLevel = value; }
        }


        public string GetSetting(string name)
        {
            if (_settings.ContainsKey(name))
                return _settings[name].ToString();
            else
                return null;
        }

        public static string AdminAzureClientId
        {
            get { return _adminAzureClientId; }
            set { _adminAzureClientId = value; }
        }

        public static string AdminAzureClientSecret
        {
            get { return _adminAzureClientSecret; }
            set { _adminAzureClientSecret = value; }
        }

        public static string AzureClientId
        {
            get { return _azureClientId;}
            set { _azureClientId = value; }
        }

        public static string AzureClientSecret
        {
            get { return _azureClientSecret; }
            set { _azureClientSecret = value; }
        }

        public static string DynamicsClientId
        {
            get { return _dynamicsClientId; }
            set { _dynamicsClientId = value; }
        }

        public static string DynamicsClientSecret
        {
            get { return _dynamicsClientSecret; }
            set { _dynamicsClientSecret = value; }
        }

        public static string ExternalDns
        {
            get { return _externalDns; }
            set { _externalDns = value; }
        }

        public static string HashSalt
        {
            get { return _hashSalt; }
            set { _hashSalt = value; }
        }

        public static int HashVersion
        {
            get { return _hashVersion; }
            set { _hashVersion = value; }
        }

        public static string CertKeyPath
        {
            get { return _certKeyPath; }
            set { _certKeyPath = value; }
        }

        public static string CertKeyDirectory
        {
            get { return _certKeyDirectory; }
            set { _certKeyDirectory = value; }
        }

        public static string TenantId
        {
            get { return _tenantId; }
            set { _tenantId = value; }
        }

        public static string SubscriptionId
        {
            get { return _subscriptionId; }
            set { _subscriptionId = value; }
        }

        public static string SqlServerName
        {
            get { return _sqlServerName; }
            set { _sqlServerName = value; }
        }

        public static string SqlUsername
        {
            get { return _sqlUsername; }
            set { _sqlUsername = value; }
        }

        public static string RegistrationPassword
        {
            get { return _registrationPassword; }
            set { _registrationPassword = value; }
        }

        public static string ResourceGroupName
        {
            get { return _resourceGroupName; }
            set { _resourceGroupName = value; }
        }

        public static string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public static string ApplicationPassword
        {
            get { return _applicationPassword; }
            set { _applicationPassword = value; }
        }

        public static string SqlPassword
        {
            get { return _sqlPassword; }
            set { _sqlPassword = value; }
        }

        public static string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }

        public static string GitHubUrl { get => _gitHubUrl; set => _gitHubUrl = value; }

        public static string KeyVaultName
        {
            get { return _keyVaultName; }
            set { _keyVaultName = value; }
        }

        public static string WebType
        {
            get { return _webType; }
            set { _webType = value; }
        }

        public static string ResourcePrefix
        {
            get { return _resourcePrefix; }
            set { _resourcePrefix = value; }
        }

        public static string AppServicePlan
        {
            get { return _appServicePlan; }
            set { _appServicePlan = value; }
        }

        public static string AdminWebName
        {
            get { return _adminWebName; }
            set { _adminWebName = value; }
        }

        public static string SubjectWebName
        {
            get { return _subjectWebName; }
            set { _subjectWebName = value; }
        }

        public static string IvrWebName
        {
            get { return _ivrWebName; }
            set { _ivrWebName = value; }
        }

        public static string VisionServiceName
        {
            get { return _visionServiceName; }
            set { _visionServiceName = value; }
        }

        public static string VisionApiUri
        {
            get { return _visionApiUri; }
            set { _visionApiUri = value; }
        }

        public static string VisionApiKey
        {
            get { return _visionApiKey; }
            set { _visionApiKey = value; }
        }

        public static string SpeechServiceName
        {
            get { return _speechServiceName; }
            set { _speechServiceName = value; }
        }

        public static string SpeechApiUri
        {
            get { return _speechApiUri; }
            set { _speechApiUri = value; }
        }

        public static string SpeechApiKey
        {
            get { return _speechApiKey; }
            set { _speechApiKey = value; }
        }

        public static string EventHubNamespacePrefix
        {
            get { return _eventHubNamespacePrefix; }
            set { _eventHubNamespacePrefix = value; }
        }

        public static string EventHubNamespace
        {
            get { return _eventHubNamespace; }
            set { _eventHubNamespace = value; }
        }

        public static string EventHubName
        {
            get { return _eventHubName; }
            set { _eventHubName = value; }
        }

        public static string EventErrorHubName
        {
            get { return _eventErrorHubName; }
            set { _eventErrorHubName = value; }
        }

        public static string EventNotificationHubName
        {
            get { return _eventNotificationHubName; }
            set { _eventNotificationHubName = value; }
        }

        public static string StorageAccountName
        {
            get { return _storageAccountName; }
            set { _storageAccountName = value; }
        }

        public static string TwilioAccountSid
        {
            get { return _twilioAccountSid; }
            set { _twilioAccountSid = value; }
        }

        public static string TwilioAuthToken
        {
            get { return _twilioAuthToken; }
            set { _twilioAuthToken = value; }
        }

        public static string TwilioNumber
        {
            get { return _twilioNumber; }
            set { _twilioNumber = value; }
        }

        public static string EmailTemplatesDirectory
        {
            get { return _emailTemplatesDirectory; }
            set { _emailTemplatesDirectory = value; }
        }

        public static string MailServer
        {
            get { return _mailServer; }
            set { _mailServer = value; }
        }

        public static string MailServerPort
        {
            get { return _mailServerPort; }
            set { _mailServerPort = value; }
        }

        public static string UseSecurity
        {
            get { return _useSecurity; }
            set { _useSecurity = value; }
        }

        public static string MailServerUserName
        {
            get { return _mailServerUserName; }
            set { _mailServerUserName = value; }
        }

        public static string MailServerPassword
        {
            get { return _mailServerPassword; }
            set { _mailServerPassword = value; }
        }

        public static string NoReplyEmailAddress
        {
            get { return _noReplyEmailAddress; }
            set { _noReplyEmailAddress = value; }
        }

        public static string CoreSystemUrl
        {
            get { return _coreSystemUrl; }
            set { _coreSystemUrl = value; }
        }

        public static Guid CoreApplicationId
        {
            get { return _coreApplicationId; }
            set { _coreApplicationId = value; }
        }

        public static string ConsoleLogLevel
        {
            get { return _consoleLogLevel; }
            set { _consoleLogLevel = value; }
        }

        public static string AppStubPath
        {
            get { return _appStubPath; }
            set { _appStubPath = value; }
        }

        public static string GraphResourceId
        {
            get { return graphResourceId; }
            set { graphResourceId = value; }
        }

        public static string GraphUrl
        {
            get { return graphUrl; }
            set { graphUrl = value; }
        }

        public static string GraphVersion
        {
            get { return graphVersion; }
            set { graphVersion = value; }
        }
        
        public static string TenantSystemUrl
        {
            get { return _tenantSystemUrl; }
            set { _tenantSystemUrl = value; }
        }

        public static string RecaptchaPublicKey
        {
            get { return _recaptchaPublicKey; }
            set { _recaptchaPublicKey = value; }
        }

        public static string RecaptchaPrivateKey
        {
            get { return _recaptchaPrivateKey; }
            set { _recaptchaPrivateKey = value; }
        }

        public static string RecaptchaApiVersion
        {
            get { return _recaptchaApiVersion; }
            set { _recaptchaApiVersion = value; }
        }

        public static string FacebookRedirect
        {
            get { return facebookRedirect; }
            set { facebookRedirect = value; }
        }

        public static string TwitterRedirect
        {
            get { return twitterRedirect; }
            set { twitterRedirect = value; }
        }

        public static string LinkedInRedirect
        {
            get { return linkedInRedirect; }
            set { linkedInRedirect = value; }
        }

        public static string AzureRedirect
        {
            get { return azureRedirect; }
            set { azureRedirect = value; }
        }

        public static string AmazonRedirect
        {
            get { return amazonRedirect; }
            set { amazonRedirect = value; }
        }

        public static string GoogleRedirect
        {
            get { return googleRedirect; }
            set { googleRedirect = value; }
        }

        public static string PaypalClientId
        {
            get { return _paypalClientId; }
            set { _paypalClientId = value; }
        }

        public static string PaypalClientSecret
        {
            get { return _paypalClientSecret; }
            set { _paypalClientSecret = value; }
        }

        public static string DripClientId
        {
            get { return _dripClientId; }
            set { _dripClientId = value; }
        }

        public static string LionDeskClientId
        {
            get { return _lionDeskClientId; }
            set { _lionDeskClientId = value; }
        }

        public static string LionDeskClientSecret
        {
            get { return _lionDeskClientSecret; }
            set { _lionDeskClientSecret = value; }
        }

        public static string LionDeskRedirect
        {
            get { return _lionDeskRedirect; }
            set { _lionDeskRedirect = value; }
        }

        public static string PaypalMode
        {
            get { return _paypalMode; }
            set { _paypalMode = value; }
        }

        public static string BingMapsApiKey
        {
            get { return _bingMapsApiKey; }
            set { _bingMapsApiKey = value; }
        }

        public static string GoogleMapsApiKey
        {
            get { return _googleMapsApiKey; }
            set { _googleMapsApiKey = value; }
        }

        public static string LiveRedirect
        {
            get { return liveRedirect; }
            set { liveRedirect = value; }
        }

        public static string GithubRedirect
        {
            get { return githubRedirect; }
            set { githubRedirect = value; }
        }

        public static string InstagramRedirect
        {
            get { return instagramRedirect; }
            set { instagramRedirect = value; }
        }

        public static string SalesForceRedirect
        {
            get { return salesForceRedirect; }
            set { salesForceRedirect = value; }
        }

        public static string LiveClientId
        {
            get { return liveClientId; }
            set { liveClientId = value; }
        }

        public static string LiveClientSecret
        {
            get { return liveClientSecret; }
            set { liveClientSecret = value; }
        }

        public static string TwitterClientId
        {
            get { return twitterClientId; }
            set { twitterClientId = value; }
        }

        public static string TwitterClientSecret
        {
            get { return twitterClientSecret; }
            set { twitterClientSecret = value; }
        }

        public static string ConstantContactClientId
        {
            get { return constantContactClientId; }
            set { constantContactClientId = value; }
        }

        public static string ConstantContactClientSecret
        {
            get { return constantContactClientSecret; }
            set { constantContactClientSecret = value; }
        }

        public static string ConstantContactRedirect
        {
            get { return _constantContactRedirect; }
            set { _constantContactRedirect = value; }
        }

        public static string DropboxClientId
        {
            get { return dropboxClientId; }
            set { dropboxClientId = value; }
        }

        public static string DropboxClientSecret
        {
            get { return dropboxClientSecret; }
            set { dropboxClientSecret = value; }
        }

        public static string DropboxRedirect
        {
            get { return _dropboxRedirect; }
            set { _dropboxRedirect = value; }
        }

        public static string DocusignClientId
        {
            get { return docusignClientId; }
            set { docusignClientId = value; }
        }

        public static string DocusignClientSecret
        {
            get { return docusignClientSecret; }
            set { docusignClientSecret = value; }
        }

        public static string DocusignRedirect
        {
            get { return _docusignRedirect; }
            set { _docusignRedirect = value; }
        }

        public static string FacebookClientId
        {
            get { return facebookClientId; }
            set { facebookClientId = value; }
        }

        public static string FacebookClientSecret
        {
            get { return facebookClientSecret; }
            set { facebookClientSecret = value; }
        }

        public static string GoogleClientId
        {
            get { return googleClientId; }
            set { googleClientId = value; }
        }

        public static string GoogleClientSecret
        {
            get { return googleClientSecret; }
            set { googleClientSecret = value; }
        }

        public static string GithubClientId
        {
            get { return githubClientId; }
            set { githubClientId = value; }
        }

        public static string GithubClientSecret
        {
            get { return githubClientSecret; }
            set { githubClientSecret = value; }
        }

        public static string InstagramClientId
        {
            get { return instagramClientId; }
            set { instagramClientId = value; }
        }

        public static string InstagramClientSecret
        {
            get { return instagramClientSecret; }
            set { instagramClientSecret = value; }
        }

        public static string AmazonClientId
        {
            get { return amazonClientId; }
            set { amazonClientId = value; }
        }

        public static string AmazonClientSecretId
        {
            get { return amazonClientSecretId; }
            set { amazonClientSecretId = value; }
        }

        public static string LinkedInClientId
        {
            get { return linkedInClientId; }
            set { linkedInClientId = value; }
        }

        public static string LinkedInClientSecret
        {
            get { return linkedInClientSecret; }
            set { linkedInClientSecret = value; }
        }

        public static string SalesForceClientId
        {
            get { return salesForceClientId; }
            set { salesForceClientId = value; }
        }

        public static string SalesForceClientSecret
        {
            get { return salesForceClientSecret; }
            set { salesForceClientSecret = value; }
        }

        public static string Auth0ClientId
        {
            get { return auth0ClientId; }
            set { auth0ClientId = value; }
        }

        public static string Auth0ClientSecret
        {
            get { return auth0ClientSecret; }
            set { auth0ClientSecret = value; }
        }

        public static string YammerClientId
        {
            get { return yammerClientId; }
            set { yammerClientId = value; }
        }

        public static string YammerClientSecret
        {
            get { return yammerClientSecret; }
            set { yammerClientSecret = value; }
        }

        public static string YammerRedirect
        {
            get { return _yammerRedirect; }
            set { _yammerRedirect = value; }
        }

        public static Guid SystemId
        {
            get
            {
                if (_systemId == Guid.Empty)
                {
                    _systemId = Guid.Parse(LoadFromKeyVault("SystemId"));
                }

                return _systemId;
            }

            set { _systemId = value; }
        }

        public static string ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
        }

        public static string Applications
        {
            get { return _applications; }
            set { _applications = value; }
        }

        public static string AzureUsername
        {
            get { return _azureUsername; }
            set { _azureUsername = value; }
        }

        public static string AzurePassword
        {
            get { return _azurePassword; }
            set { _azurePassword = value; }
        }

        public static string FaceServiceName
        {
            get { return _faceServiceName; }
            set { _faceServiceName = value; }
        }

        public static string FaceApiUri
        {
            get { return _faceApiUri; }
            set { _faceApiUri = value; }
        }

        public static string FaceApiKey
        {
            get { return _faceApiKey; }
            set { _faceApiKey = value; }
        }

        public static string EventLogLevel
        {
            get { return _eventLogLevel; }
            set { _eventLogLevel = value; }
        }

        public static string AzureKeyVaultUrl
        { 
            get { return string.Format("https://{0}{1}.vault.azure.net", ResourcePrefix.ToLower(), KeyVaultName.ToLower()); }
        }

        private static string _dbLogLevel = "Verbose";

        static Hashtable _settings;
        static readonly KeyVaultClient kv;
        private static string _gdprSqlConnectionString;
        private static string _eventHubConnectionString;
        private static string _mode;
        private static string _aesKey;
        private static string _eventHubConnectionStringWithPath;
        private static string _storageAccountKey;
        private static string _storageAccountSecret;
        private static string _storageService;
        private static string _searchService;
        private static string _searchServiceName;
        private static string _searchKey;
        private static bool _enableEncryption;
        private static string _storageRegion;
        
        private static string _eventLogLevel;
        private static string _consoleLogLevel;
        private static string _appStubPath;

        private static string _applicationId;
        private static string _applications;

        private static string _adminAzureClientId;
        private static string _adminAzureClientSecret;

        private static string _azureClientId;
        private static string _azureClientSecret;

        private static string _dynamicsClientId;
        private static string _dynamicsClientSecret;

        private static string _gitHubUrl;
        private static Guid _systemId;
        private static string _externalDns;
        private static string _hashSalt;
        private static int _hashVersion;
        private static string _certKeyPath;
        private static string _certKeyDirectory;
        private static string _registrationPassword;

        private static string _tenantId;
        private static string _subscriptionId;
        private static string _resourceGroupName;
        private static string _region;
        private static string _sqlServerName;
        private static string _sqlUsername;
        private static string _sqlPassword;
        private static string _applicationPassword;
        private static string _databaseName;

        private static string _keyVaultName;

        private static string _azureUsername;
        private static string _azurePassword;

        private static string _webType;

        private static string _resourcePrefix;

        private static string _appServicePlan;
        private static string _adminWebName;
        private static string _subjectWebName;
        private static string _ivrWebName;

        private static string _visionServiceName;
        private static string _visionApiUri;
        private static string _visionApiKey;

        private static string _speechServiceName;
        private static string _speechApiUri;
        private static string _speechApiKey;

        private static string _faceServiceName;
        private static string _faceApiUri;
        private static string _faceApiKey;

        private static string graphResourceId = "https://graph.microsoft.com";
        private static string graphUrl = "https://graph.microsoft.com";
        private static string graphVersion = "v1.0";

        private static string _systemKeyVersion;

        /*event hub processing*/
        private static string _eventHubNamespacePrefix;
        private static string _eventHubNamespace;
        private static string _eventHubName;
        private static string _eventErrorHubName;
        private static string _eventNotificationHubName;

        /* blog storage account */
        private static string _storageAccountName;

        /* IVR settings */
        private static string _twilioAccountSid;
        private static string _twilioAuthToken;
        private static string _twilioNumber;

        /* mail settings */
        private static string _emailTemplatesDirectory;
        private static string _mailServer;
        private static string _mailServerPort;
        private static string _useSecurity;
        private static string _mailServerUserName;
        private static string _mailServerPassword;
        private static string _noReplyEmailAddress;

        private static string _coreSystemUrl;
        private static Guid _coreApplicationId;
        private static string _tenantSystemUrl;

        /* captcha */
        private static string _recaptchaPublicKey;
        private static string _recaptchaPrivateKey;
        private static string _recaptchaApiVersion;

        /* maps apis */
        private static string _bingMapsApiKey;
        private static string _googleMapsApiKey;

        /* payment api */
        private static string _paypalClientId;
        private static string _paypalClientSecret;
        private static string _paypalMode;
        
        /* social settings */
        private static string facebookRedirect = CoreSystemUrl + "/Home/FacebookAuthorize";
        private static string twitterRedirect = CoreSystemUrl + "/Home/TwitterAuthorize";
        private static string linkedInRedirect = CoreSystemUrl + "/Home/LinkedInAuthorize";
        private static string azureRedirect = CoreSystemUrl + "/Home/AzureAuthorize";
        private static string amazonRedirect = CoreSystemUrl + "/Home/AmazonAuthorize";
        private static string googleRedirect = CoreSystemUrl + "/Home/GoogleAuthorize";
        private static string liveRedirect = CoreSystemUrl + "/Home/LiveAuthorize";
        private static string githubRedirect = CoreSystemUrl + "/Home/GitHubAuthorize";
        private static string instagramRedirect = CoreSystemUrl + "/Home/InstagramAuthorize";
        private static string salesForceRedirect = CoreSystemUrl + "/Home/SalesForceAuthorize";
        private static string _constantContactRedirect = CoreSystemUrl + "/Home/ConstantContactAuthorize";
        private static string _dropboxRedirect = CoreSystemUrl + "/Home/DropboxAuthorize";
        private static string _docusignRedirect = CoreSystemUrl + "/Home/DocusignAuthorize";
        private static string _yammerRedirect = CoreSystemUrl + "/Home/YammerAuthorize";
        private static string _lionDeskRedirect = CoreSystemUrl + "/Home/LionDeskAuthorize";

        private static string liveClientId;
        private static string liveClientSecret;

        private static string twitterClientId;
        private static string twitterClientSecret;

        private static string dropboxClientId;
        private static string dropboxClientSecret;

        private static string docusignClientId;
        private static string docusignClientSecret;

        private static string constantContactClientId;
        private static string constantContactClientSecret;

        private static string facebookClientId;
        private static string facebookClientSecret;

        private static string googleClientId;
        private static string googleClientSecret;

        private static string githubClientId;
        private static string githubClientSecret;

        private static string instagramClientId;
        private static string instagramClientSecret;

        private static string _dripClientId;

        private static string _lionDeskClientId;
        private static string _lionDeskClientSecret;

        private static string amazonClientId;
        private static string amazonClientSecretId;

        private static string linkedInClientId;
        private static string linkedInClientSecret;

        private static string salesForceClientId;
        private static string salesForceClientSecret;
        private static string salesForceSecurityToken;

        private static string auth0ClientId = "";
        private static string auth0ClientSecret = "";

        private static string yammerClientId = "";
        private static string yammerClientSecret = "";

    }
}