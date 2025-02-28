﻿using GDPR.Common.Core;
using GDPR.Common.Enums;
using GDPR.Common.Exceptions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;
using PayPal.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace GDPR.Common
{
    public class Configuration
    {
        private static string _dbLogLevel = "Verbose";
        private static string _eventLogLevel = "Verbose";
        private static string _consoleLogLevel = "Verbose";

        static Hashtable _settings;
        static readonly KeyVaultClient kv;
        private static string _gdprSqlConnectionString;
        private static string _eventHubConnectionString;
        private static string _mode;
        private static string _eventHubConnectionStringWithPath;
        private static string _storageAccountKey;
        private static string _storageAccountSecret;
        private static string _storageService;
        private static string _searchService;
        private static string _searchServiceName;
        private static string _searchKey;
        private static bool _enableEncryption;
        private static string _storageRegion;

        private static bool _allowTrials;
        private static bool _notHosted;
        private static string _siteName;
        private static string _companyName;
        private static string _companyShortName;

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
        private static string _systemPin;
        private static Guid _systemId;
        private static string _externalDns;
        private static string _consumerSystemUrl;
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

        private static string _textAnalyticsServiceName;
        private static string _textAnalyticsApiUri;
        private static string _textAnalyticsApiKey;


        private static string _faceServiceName;
        private static string _faceApiUri;
        private static string _faceApiKey;

        private static string graphResourceId = "https://graph.microsoft.com";
        private static string graphUrl = "https://graph.microsoft.com";
        private static string graphVersion = "v1.0";

        private static string _systemKeyVersion;
        private static string _systemPinVersion;

        /*event hub processing*/
        private static string _eventHubNamespacePrefix;
        private static string _eventHubNamespace;
        private static string _eventHubName;
        private static string _eventErrorHubName;
        private static string _eventDiscoveryHubName;
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
        private static string _ivrUrl;
        private static Guid _coreApplicationId;
        private static string _tenantSystemUrl;

        /* captcha */
        private static string _recaptchaPublicKey;
        private static string _recaptchaPrivateKey;
        private static string _recaptchaApiVersion;

        /* maps apis */
        private static string _azureMapsApiKey;
        private static string _bingMapsApiKey;
        private static string _googleMapsApiKey;

        /* payment api */
        private static string _paypalClientId;
        private static string _paypalClientSecret;
        private static string _paypalMode;

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

        static Configuration()
        {
            _settings = new Hashtable();

            string mode = ConfigurationManager.AppSettings["Mode"];
            Configuration.LoadWithMode(mode);

            //Get an access token for the Key Vault to get the secret out...
            //https://azure.microsoft.com/en-us/resources/samples/app-service-msi-keyvault-dotnet/
            try
            {
                if (Configuration.IsManaged && !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")))
                {
                    AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                    //this is for managed service identities...
                    kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                }
                else
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
                GDPRCore.Current.Log(ex, LogLevel.Error);
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
            if (value != null)
            {
                var result = Task.Run(async () =>
                {
                    return await kv.SetSecretAsync(AzureKeyVaultUrl, name, value);
                }).Result;

                SetProperty(name, value);

                return result.Id.Replace(Configuration.AzureKeyVaultUrl + "/secrets/" + name + "/", "");
            }

            return "";
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

        public static void LoadWithMode(string mode, string filePath)
        {
            if (GDPRCore.Current == null)
                GDPRCore.Current = new GDPRCore();

            filePath = filePath + $"/configuration.{mode}.json";

            FileInfo fi = new FileInfo(filePath);

            if (!fi.Exists)
            {
                //try some manual paths...(function app)
                filePath = $"D:/home/site/wwwroot/configuration.{mode}.json";
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

        public static void LoadWithMode(string mode)
        {
            //may have loaded previously...
            if (GDPRCore.Current == null)
                GDPRCore.Current = new GDPRCore();

            string filePath = HostingEnvironment.MapPath($"~/");

            if (string.IsNullOrEmpty(filePath))
            {
                FileInfo fi = new FileInfo(filePath = Assembly.GetExecutingAssembly().Location);
                filePath = fi.Directory.FullName + $"\\";
            }

            LoadWithMode(mode, filePath);
        }

        public static object GetProperty(string name)
        {
            object val = null;

            PropertyInfo[] props = typeof(Configuration).GetProperties();

            foreach (PropertyInfo pi in props)
            {
                if (pi.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    val = pi.GetValue(null);
                }
            }

            if (val == null)
            {
                val = _settings[name.ToLower()];
            }

            return val;
        }

        public static string GetSetting(string name)
        {
            if (_settings.ContainsKey(name.ToLower()))
                return _settings[name.ToLower()].ToString();
            else
                return null;
        }

        private static void SetProperty(string name, string value)
        {
            PropertyInfo prop = typeof(Configuration).GetProperty(name);

            if (prop != null)
                SetProperty(prop, value);

            if (_settings.ContainsKey(name.ToLower()))
                _settings[name.ToLower()] = value;
            else
                _settings.Add(name.ToLower(), value);
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
                    case "System.Decimal":
                        if (!string.IsNullOrEmpty(inValue))
                        {
                            value = decimal.Parse(inValue);
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
                ht.Add(setting.Name.ToString().ToLower(), setting.Value);
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
                    Dictionary<string, string> config = new Dictionary<string, string>
                    {
                        { "clientId", PaypalClientId },
                        { "clientSecret", PaypalClientSecret },
                        { "mode", "sandbox" },
                        { "requestRetries", "1" },
                        { "connectionTimeout", "360000" }
                    };

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

        public static string SystemPinVersion
        {
            get
            {
                if (string.IsNullOrEmpty(_systemPinVersion))
                {
                    _systemPinVersion = LoadFromKeyVault("SystemPinVersion");
                }

                if (string.IsNullOrEmpty(_systemPinVersion))
                    _systemPinVersion = "1";

                return _systemPinVersion;
            }
            set { _systemPinVersion = value; }
        }

        public static string GDPRSQLConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_gdprSqlConnectionString))
                {
                    _gdprSqlConnectionString = LoadFromKeyVault("GDPRSQLConnectionString");

                    if (string.IsNullOrEmpty(_gdprSqlConnectionString))
                        throw new GDPRException("Database connection string is empty.  Check the configuration, keyvault firewall or azure credentials.");
                }

                return _gdprSqlConnectionString;
            }
            set
            {
                _gdprSqlConnectionString = value;
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

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

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

        public static string ConsumerSystemUrl
        {
            get { return _consumerSystemUrl; }
            set { _consumerSystemUrl = value; }
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
            get
            {
                if (string.IsNullOrEmpty(_visionApiUri))
                {
                    _visionApiUri = LoadFromKeyVault("VisionApiUri");
                }

                return _visionApiKey;
            }
            set { _visionApiUri = value; }
        }

        public static string VisionApiKey
        {
            get {
                if (string.IsNullOrEmpty(_visionApiKey))
                {
                    _visionApiKey = LoadFromKeyVault("VisionApiKey");
                }

                return _visionApiKey;
            }
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
            get {
                if (string.IsNullOrEmpty(_speechApiKey))
                {
                    _speechApiKey = LoadFromKeyVault("SpeechApiKey");
                }

                return _speechApiKey;
            }
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

        public static string EventDiscoveryHubName
        {
            get { return _eventDiscoveryHubName; }
            set { _eventDiscoveryHubName = value; }
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

        public static string IvrUrl
        {
            get { return _ivrUrl; }
            set { _ivrUrl = value; }
        }

        public static Guid CoreApplicationId
        {
            get { return _coreApplicationId; }
            set { _coreApplicationId = value; }
        }

        public static string DbLogLevel
        {
            get { return _dbLogLevel; }
            set { _dbLogLevel = value; }
        }

        public static string EventLogLevel
        {
            get { return _eventLogLevel; }
            set { _eventLogLevel = value; }
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

        
        public static string PaypalMode
        {
            get { return _paypalMode; }
            set { _paypalMode = value; }
        }

        public static string AzureMapsApiKey
        {
            get { return _azureMapsApiKey; }
            set { _azureMapsApiKey = value; }
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

        public static string SystemPin
        {
            get
            {
                if (string.IsNullOrEmpty(_systemPin))
                {
                    _systemPin = LoadFromKeyVault("SystemPin");
                }

                return _systemPin;
            }
            set
            {
                _systemPin = value;
            }
        }

        public static Guid SystemId
        {
            get
            {
                if (_systemId == Guid.Empty)
                {
                    try
                    {
                        _systemId = Guid.Parse(LoadFromKeyVault("SystemId"));
                    }
                    catch (Exception ex)
                    {
                        return Guid.Empty;
                    }
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
        //

        public static string TextAnalyticsServiceName
        {
            get { return _textAnalyticsServiceName; }
            set { _textAnalyticsServiceName = value; }
        }

        public static string TextAnalyticsApiUri
        {
            get { return _textAnalyticsApiUri; }
            set { _textAnalyticsApiUri = value; }
        }

        public static string TextAnalyticsApiKey
        {
            get
            {
                if (string.IsNullOrEmpty(_textAnalyticsApiKey))
                {
                    _textAnalyticsApiKey = LoadFromKeyVault("TextAnalyticsApiKey");
                }

                return _textAnalyticsApiKey;
            }
            set { _textAnalyticsApiKey = value; }
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
            get {
                if (string.IsNullOrEmpty(_faceApiKey))
                {
                    _faceApiKey = LoadFromKeyVault("FaceApiKey");
                }

                return _faceApiKey;
            }
            set { _faceApiKey = value; }
        }

        

        public static string SiteName
        {
            get { return _siteName; }
            set { _siteName = value; }
        }


        public static string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        public static string CompanyShortName
        {
            get { return _companyShortName; }
            set { _companyShortName = value; }
        }

        public static bool AllowTrials
        {
            get { return _allowTrials; }
            set { _allowTrials = value; }
        }

        public static bool NotHosted
        {
            get { return _notHosted; }
            set { _notHosted = value; }
        }

        static string _linkSecurity;

        public static string LinkSecurity
        {
            get { return _linkSecurity; }
            set { _linkSecurity = value; }
        }

        static string _linkSupport;

        public static string LinkSupport
        {
            get { return _linkSupport; }
            set { _linkSupport = value; }
        }

        static string _linkTerms;

        public static string LinkTerms
        {
            get { return _linkTerms; }
            set { _linkTerms = value; }
        }

        static string _linkEula;

        public static string LinkEula
        {
            get { return _linkEula; }
            set { _linkEula = value; }
        }

        static string _linkPrivacy;

        public static string LinkPrivacy
        {
            get { return _linkPrivacy; }
            set { _linkPrivacy = value; }
        }

        static string _linkFeedback;

        public static string LinkFeedback
        {
            get { return _linkFeedback; }
            set { _linkFeedback = value; }
        }

        static string _linkContact;

        public static string LinkContact
        {
            get { return _linkContact; }
            set { _linkContact = value; }
        }

        static string _linkPhone;

        public static string LinkPhone
        {
            get { return _linkPhone; }
            set { _linkPhone = value; }
        }

        static string _emailDomain;

        public static string EmailDomain
        {
            get { return _emailDomain; }
            set { _emailDomain = value; }
        }

        static string _appInsightsKey;

        public static string AppInsightsKey
        {
            get { return _appInsightsKey; }
            set { _appInsightsKey = value; }
        }

        static bool _isManaged;

        public static bool IsManaged
        {
            get { return _isManaged; }
            set { _isManaged = value; }
        }

        static bool _enablePersonalVerfication;

        public static bool EnablePersonalVerification
        {
            get { return _enablePersonalVerfication; }
            set { _enablePersonalVerfication = value; }
        }

        static bool _enableGeoLocation;

        public static bool EnableGeoLocation
        {
            get { return _enableGeoLocation; }
            set { _enableGeoLocation = value; }
        }

        static bool _enableUserPin;

        public static bool EnableUserPin
        {
            get { return _enableUserPin; }
            set { _enableUserPin = value; }
        }

        static string _surveyLink;

        public static string SurveyLink
        {
            get { return _surveyLink; }
            set { _surveyLink = value; }
        }

        static decimal _creditPrice;

        public static decimal CreditPrice
        {
            get { return _creditPrice; }
            set { _creditPrice = value; }
        }

        public static string AzureKeyVaultUrl
        { 
            get { return string.Format("https://{0}{1}.vault.azure.net", ResourcePrefix.ToLower(), KeyVaultName.ToLower()); }
        }


    }
}