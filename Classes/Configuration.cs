using GDPR.Common.Core;
using GDPR.Common.Enums;
using GDPR.Common.Exceptions;
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
            kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(Utility.GetToken));
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

                        if (val != null || val.ToString() != "")
                        {
                            var result = Task.Run(async () =>
                            {
                                return await kv.SetSecretAsync(AzureKeyVaultUrl, pi.Name, val.ToString());
                            }).Result;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        static public string LoadFromKeyVault(string name)
        {
            try
            {
                string uri = AzureKeyVaultUrl + "/secrets/" + name;
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

        public static void Load()
        {
            GDPRCore.Current = new GDPRCore();

            string filePath = HostingEnvironment.MapPath("~/configuration.json");

            if (string.IsNullOrEmpty(filePath))
            {
                FileInfo fi = new FileInfo(filePath = Assembly.GetExecutingAssembly().Location);
                filePath = fi.Directory.FullName + "\\configuration.json";
            }

            //load configuartion
            _settings = LoadConfiguration(filePath);
            LoadConfiguration(_settings);

            //SaveToKeyVault();
            //LoadFromKeyVault();
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

        public static void LoadConfiguration(Hashtable ht)
        {
            PropertyInfo[] props = typeof(Configuration).GetProperties();

            foreach(string key in ht.Keys)
            {
                foreach(PropertyInfo pi in props)
                {
                    if (pi.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        object value;

                        switch (pi.PropertyType.FullName)
                        {
                            case "System.Int32":
                                value = int.Parse(ht[key].ToString());
                                pi.SetValue(null, value);
                                break;
                            case "System.Guid":
                                value = Guid.Parse(ht[key].ToString());
                                pi.SetValue(null, value);
                                break;
                            case "System.String":
                                value = ht[key].ToString();
                                pi.SetValue(null, value);
                                break;
                        }
                        
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
                    // Use OAuthTokenCredential to request an access token from PayPal
                    var accessToken = new OAuthTokenCredential(config).GetAccessToken();
                    _apiContext = new APIContext(accessToken);
                }

                return _apiContext;
            }
            set
            {
                _apiContext = value;
            }
        }

        public static string GDPRSQLConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_gdprSqlConnectionString))
                {
                    string uri = KeyVaultGdprsqlConnectionStringUri;
                    var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    _gdprSqlConnectionString = result.Value;
                }

                return _gdprSqlConnectionString;
            }
        }

        public static string TranslateSQLConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_translateSqlConnectionString))
                {
                    string uri = KeyVaultTranslateSqlConnectionStringUri;
                    var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    _translateSqlConnectionString = result.Value;
                }

                return _translateSqlConnectionString;
            }
            set
            {
                _translateSqlConnectionString = value;
            }
        }

        public static string AesKey
        {
            get
            {
                if (string.IsNullOrEmpty(_aesKey))
                {
                    string uri = AesKeyUri;
                    var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    _aesKey = result.Value;
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
                    string uri = KeyVaultStorageAccountKeyUri;
                    var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    _storageAccountKey = result.Value;

                }
                return _storageAccountKey;
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
                    string uri = KeyVaultEventHubConnectionStringUri;
                    string eventHubName = EventHubName;
                    var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    _eventHubConnectionStringWithPath = result.Value + ";EntityPath=" + eventHubName;
                }

                return _eventHubConnectionStringWithPath;
            }
        }

        public static string EventHubConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_eventHubConnectionString))
                {
                    string uri = KeyVaultEventHubConnectionStringUri;
                    var result = Task.Run(async () => { return await kv.GetSecretAsync(uri); }).Result;
                    _eventHubConnectionString = result.Value;
                }

                return _eventHubConnectionString;
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

        public static string KeyVaultGdprsqlConnectionStringUri
        {
            get { return keyVaultGDPRSQLConnectionStringUri; }
            set { keyVaultGDPRSQLConnectionStringUri = value; }
        }

        public static string KeyVaultTranslateSqlConnectionStringUri
        {
            get { return keyVaultTranslateSQLConnectionStringUri; }
            set { keyVaultTranslateSQLConnectionStringUri = value; }
        }

        public static string KeyVaultEventHubConnectionStringUri
        {
            get { return keyVaultEventHubConnectionStringUri; }
            set { keyVaultEventHubConnectionStringUri = value; }
        }

        public static string KeyVaultStorageAccountKeyUri
        {
            get { return keyVaultStorageAccountKeyUri; }
            set { keyVaultStorageAccountKeyUri = value; }
        }

        public static string AesKeyUri
        {
            get { return aesKeyUri; }
            set { aesKeyUri = value; }
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

        public static Guid SystemId
        {
            get { return _systemId; }
            set { _systemId = value; }
        }

        public static string ApplicationId
        {
            get { return _applicationId; }
            set { _applicationId = value; }
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

        public static int SystemKeyVersion
        {
            get { return _systemKeyVersion; }
            set { _systemKeyVersion = value; }
        }


        

        public static string EventLogLevel
        {
            get { return _eventLogLevel; }
            set { _eventLogLevel = value; }
        }

        public static string AzureKeyVaultUrl
        {
            get { return _azureKeyVaultUrl; }
            set { _azureKeyVaultUrl = value; }
        }

        private static string _dbLogLevel = "Verbose";

        static Hashtable _settings;
        static readonly KeyVaultClient kv;
        private static string _gdprSqlConnectionString;
        private static string _crmSqlConnectionString;
        private static string _translateSqlConnectionString;
        private static string _eventHubConnectionString;
        private static string _mode;
        private static string _aesKey;
        private static string _eventHubConnectionStringWithPath;
        private static string _storageAccountKey;
        private static string _storageService;
        private static string _storageRegion;
        private static string _azureKeyVaultUrl;

        private static string _eventLogLevel;
        private static string _consoleLogLevel;
        
        private static string _applicationId;

        private static string _azureClientId;
        private static string _azureClientSecret;

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
        private static string _databaseName;

        private static string _keyVaultName;

        private static string _azureUsername;
        private static string _azurePassword;

        private static string _webType;

        private static string _resourcePrefix;

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

        private static string keyVaultGDPRSQLConnectionStringUri;
        private static string keyVaultTranslateSQLConnectionStringUri;
        private static string keyVaultEventHubConnectionStringUri;
        private static string keyVaultStorageAccountKeyUri;
        private static string aesKeyUri;

        private static int _systemKeyVersion = 1;

        /*event hub processing*/
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

        private static string liveClientId;
        private static string liveClientSecret;

        private static string twitterClientId;
        private static string twitterClientSecret;

        private static string facebookClientId;
        private static string facebookClientSecret;

        private static string googleClientId;
        private static string googleClientSecret;

        private static string githubClientId;
        private static string githubClientSecret;

        private static string instagramClientId;
        private static string instagramClientSecret;

        private static string amazonClientId;
        private static string amazonClientSecretId;

        private static string linkedInClientId;
        private static string linkedInClientSecret;

        private static string salesForceClientId;
        private static string salesForceClientSecret;
        private static string salesForceSecurityToken;

        private static string auth0ClientId = "";
        private static string auth0ClientSecret = "";
        
    }
}