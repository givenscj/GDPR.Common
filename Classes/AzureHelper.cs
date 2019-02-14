using GDPR.Common.Core;
using Microsoft.Azure.Management.Eventhub.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;

namespace GDPR.Common.Classes
{
    public class AzureHelper
    {
        static public AzureCredentials AzureCredentials { get; set; }
        static public IAzure AzureInstance { get; set; }
        static public Region AzureRegion { get; set; }
        static public IResourceGroup AzureResourceGroup { get; set; }

        public static void Initialize()
        {
            AzureCredentials = MakeAzureCredentials(Configuration.SubscriptionId);

            //connect to azure...
            AzureInstance = Azure.Configure().Authenticate(AzureCredentials).WithDefaultSubscription();

            //create resource group...
            AzureRegion = Region.USCentral;

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
                return AzureInstance.EventHubNamespaces.Define(name).WithRegion(AzureRegion).WithExistingResourceGroup(AzureResourceGroup.Name).Create();
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
                return AzureInstance.EventHubs.Define(eventHubName).WithExistingNamespace(ns).Create();
            }
            catch (Exception ex)
            {
                GDPRCore.Current.Log(ex.Message);
            }

            return null;
        }

        
    }
}
