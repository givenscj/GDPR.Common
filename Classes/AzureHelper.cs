using GDPR.Common.Core;
using Microsoft.Azure.Management.AppService.Fluent;
using Microsoft.Azure.Management.Eventhub.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ResourceManager.Fluent.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace GDPR.Common.Classes
{
    public class AzureHelper
    {
        static public AzureCredentials AzureCredentials { get; set; }
        static public IAzure AzureInstance { get; set; }
        static public Region AzureRegion { get; set; }
        static public IResourceGroup AzureResourceGroup { get; set; }
        static public IEventHubNamespace AzureEventNamespace { get; set; }

        public static void Initialize()
        {
            AzureCredentials = MakeAzureCredentials(Configuration.SubscriptionId);

            var client = RestClient
            .Configure()
            .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
            .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
            .WithCredentials(AzureCredentials)
            .Build();

            //connect to azure...
            Azure.IConfigurable ic = Azure.Configure();
            Azure.IAuthenticated auth = ic.Authenticate(AzureCredentials);
            AzureInstance = auth.WithDefaultSubscription();

            //create resource group...
            AzureRegion = Region.Create(Configuration.Region);

            AzureResourceGroup = CreateResourceGroup();
        }

        static public AzureCredentials MakeAzureCredentials(string subscriptionId)
        {
            var appId = Configuration.AdminAzureClientId;
            var appSecret = Configuration.AdminAzureClientSecret;
            var tenantId = Configuration.TenantId;
            var environment = AzureEnvironment.AzureGlobalCloud;

            var credentials = new AzureCredentialsFactory()
                                    .FromServicePrincipal(appId, appSecret, tenantId, environment);            

            return credentials;
        }

        public static IResourceGroup CreateResourceGroup()
        {
            try
            {
                if (AzureInstance == null)
                    Initialize();

                AzureResourceGroup = AzureInstance.ResourceGroups.Define(Configuration.ResourceGroupName).WithRegion(AzureRegion).Create();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return AzureResourceGroup;
        }

        public static IEventHubNamespace CreateEventNamespace()
        {
            return CreateEventNamespace(Configuration.EventHubNamespace);
        }

        public static IEventHubNamespace CreateEventNamespace(string name)
        {
            //create namespace...
            try
            {
                if (AzureInstance == null)
                    Initialize();

                IEnumerable<IEventHubNamespace> list = AzureInstance.EventHubNamespaces.List();

                foreach(IEventHubNamespace ns in list)
                {
                    if (ns.Name == name)
                    {
                        AzureEventNamespace = ns;
                        return ns;
                    }
                }

                AzureEventNamespace = AzureInstance.EventHubNamespaces.Define(name).WithRegion(AzureRegion).WithExistingResourceGroup(AzureResourceGroup.Name).Create();
                return AzureEventNamespace;
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex.Message);
            }

            return null;
        }

        public static IEventHub CreateEventHub(IEventHubNamespace ns, string eventHubName)
        {
            //create event hub
            try
            {
                if (AzureInstance == null)
                    Initialize();

                return AzureInstance.EventHubs.Define(eventHubName).WithExistingNamespace(ns).Create();
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex.Message);
            }

            return null;
        }

        public static IWebApp GetWebApp(string v)
        {
            if (AzureInstance == null)
                Initialize();

            IEnumerable<IWebApp> apps = AzureInstance.WebApps.List();
            IEnumerator<IWebApp> e = apps.GetEnumerator();

            while(e.MoveNext())
            {
                if (e.Current.Name.ToLower() == v.ToLower() && e.Current.ResourceGroupName == AzureResourceGroup.Name)
                    return e.Current;
            }

            return null;
        }

        public static void UploadWebAppFile(IWebAppBase webApp, byte[] file, string path)
        {
            Console.WriteLine("Uploading Configuration");

            var profile = webApp.GetPublishingProfile();
            var base64Auth = Convert.ToBase64String(Encoding.Default.GetBytes($"{profile.GitUsername}:{profile.GitPassword}"));
            MemoryStream stream = new MemoryStream(file);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("If-Match", "*");
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64Auth);
                var baseUrl = new Uri($"https://" + webApp.Name + ".scm.azurewebsites.net/");
                var requestURl = baseUrl + path;
                var httpContent = new StreamContent(stream);
                var response = client.PutAsync(requestURl, httpContent).Result;
            }
        }

        public static bool AddEventHubIpAddress(string ipAddress)
        {
            if (AzureInstance == null)
                Initialize();

            return false;
        }

        public static void CreateDeployment(string jsonFile, string parametersFile, Hashtable tokens)
        {
            if (AzureInstance == null)
                Initialize();

            string json = "{}";
            string parameters = "{}";

            if (!string.IsNullOrEmpty(jsonFile))
                json = jsonFile;

            if (!string.IsNullOrEmpty(parametersFile))
                parameters = parametersFile;

            foreach (string key in tokens.Keys)
            {
                json = json.Replace("{" + key + "}", tokens[key].ToString());
                parameters = parameters.Replace("{" + key + "}", tokens[key].ToString());
            }

            CreateDeployment(json, parameters);
        }

        public static void CreateDeployment(FileInfo jsonFile, FileInfo paramatersFile, Hashtable tokens)
        {
            if (AzureInstance == null)
                Initialize();

            string json = "{}";
            string parameters = "{}";

            if (jsonFile != null)
                json = File.ReadAllText(jsonFile.FullName);

            if (paramatersFile != null)
                parameters = File.ReadAllText(paramatersFile.FullName);

            CreateDeployment(json, parameters, tokens);
        }

        public static void CreateDeployment(string templateJson, string parameters)
        {
            if (AzureInstance == null)
                Initialize();

            if (string.IsNullOrEmpty(parameters))
                parameters = "{}";

            Guid id = Guid.NewGuid();

            try
            {

                IDeployment deploy = AzureInstance.Deployments.Define(id.ToString())
                        .WithExistingResourceGroup(AzureResourceGroup.Name)
                        .WithTemplate(templateJson)
                        .WithParameters(parameters)
                        .WithMode(DeploymentMode.Incremental)
                        .Create();

                Console.WriteLine(deploy.ProvisioningState);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
    }
}
