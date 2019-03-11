using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDPR.Common.EntityProperty
{
    public class EntityPropertyTypeBase
    {
        public System.Guid EntityPropertyTypeId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool IsMasked { get; set; }
        public bool IsSecure { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }

        public static List<EntityPropertyTypeBase> LoadEntityPropertyTypes()
        {
            List<EntityPropertyTypeBase> types = new List<EntityPropertyTypeBase>();

            //temp
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("00cde8c3-4c98-45be-b2b4-cea75146b93e"), Category = "Link", Name = "RedirectUrl", DefaultValue = "", Type = "textbox", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("65090127-f010-450c-9681-3218268ad671"), Category = "General", Name = "TenantId", DefaultValue = "", Type = "textbox", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("d161faca-aef2-48a6-b246-cd152a8b35ed"), Category = "General", Name = "TenantDomain", DefaultValue = "", Type = "textbox", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("9e326753-1adb-400c-8722-458baf5a0f15"), Category = "General", DisplayName="Office365Domain", Name = "Office365Domain", DefaultValue = "", Type = "textbox", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("99c3dfde-fc38-4102-a0e8-a4a7f90d0563"), Category = "Security", DisplayName = "AccountId", Name = "AccountId", DefaultValue = "", Type = "textbox", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("4eeff7db-e55b-4f2f-bf29-83f3a121910e"), Category = "General", DisplayName = "Domain", Name = "Domain", DefaultValue = "", Type = "textbox", IsMasked = false, IsSecure = false });


            //default application properties
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("5a45b15f-7d44-47de-96f8-b4baa94c3d71"), DisplayName = "AllowUnverifiedData", Name = "AllowUnverifiedData", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("3af231bd-a7ec-4922-8ad7-78b234705223"), DisplayName = "BatchSize", Name = "BatchSize", DefaultValue = "100", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("a7c1041c-9a67-4ff8-8b70-be3a7771aa3b"), DisplayName = "DaysBetweenRequests", Name = "DaysBetweenRequests", DefaultValue = "90", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("3dfd0327-3b5c-4e67-aa9e-694bd5cee6b4"), DisplayName = "DataIsSold", Name = "DataIsSold", DefaultValue = "true", Type = "checkbox", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("c3125b19-5937-4c3d-a323-57f0c085c4b0"), DisplayName = "DeleteRequiresApproval", Name = "DeleteRequiresApproval", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("19561589-41d8-40a4-8f4d-ee48e993583a"), DisplayName = "ExportRequiresApproval", Name = "ExportRequiresApproval", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("2559e2b8-1ae3-4c7a-8129-1ba0ce55df62"), DisplayName = "MinNumberOfVerifiedTypes", Name = "MinNumberOfVerifiedTypes", Type = "textbox", DefaultValue = "1", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("e6507d74-e3d8-4f3d-bbab-9eb7b391f55f"), DisplayName = "MaxQueryRequestsPerYear", Name = "MaxQueryRequestsPerYear", DefaultValue = "1", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("ae099068-29fa-4c13-979a-563c6411ef49"), DisplayName = "PerQueryRequestCost", Name = "PerQueryRequestCost", DefaultValue = "5.00", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });

            //paypal
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("5ac728e9-8639-4ec7-8494-d3f268115e10"), Category = "Payment", Type = "textbox", Name = "PaypalClientId", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("e8984fc7-dda7-46cf-8a22-911266dc7af7"), Category = "Payment", Type = "textbox", Name = "PaypalClientSecret", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("11e88b0d-b308-47b9-953c-bdffdb018aa5"), Category = "Payment", Type = "textbox", Name = "PaypalMode", DefaultValue = "sandbox", IsMasked = true, IsSecure = true, Description = "" });

            //hidden
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("c47a6ca9-4f8d-4b54-bd3b-62996efc1d96"), DisplayName = "Hidden", Name = "Hidden", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = true, Description = "" });

            //core system props
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("1a4cf053-2c3c-4832-a1ec-e6a467971496"), DisplayName = "PhoneNumber", Name = "PhoneNumber", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("2b8162aa-ef4d-4c08-a863-88b7f4c5598f"), DisplayName = "RegistrationPassword", Name = "RegistrationPassword", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("4af73175-8408-465b-8f82-3972cfeaa366"), DisplayName = "SystemPassword", Name = "SystemPassword", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("09683fed-dcae-4bb1-aa9e-48f01d95061c"), DisplayName = "LicenseKey", Name = "LicenseKey", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("d64b5ee7-013c-4409-8023-7e7c8af33562"), DisplayName = "ExternalDns", Name = "ExternalDns", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("cc394ae0-2a0a-4f37-8eb8-5ad974d3a433"), DisplayName = "EnableIpAddressResolution", Name = "EnableIpAddressResolution", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("332ea56f-c6a9-4011-aedb-888bb4457cdb"), DisplayName = "NotifyOnDiscovery", Name = "NotifyOnDiscovery", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("4369045b-c6c3-4e18-8b3d-bb86866513a0"), DisplayName = "ExportTimeout", Name = "ExportTimeout", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "" });

            //OAuth properties
            types.Add(new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse("039a78ba-802a-4155-9126-b43931197afd"),DisplayName = "ClientId",Name = "ClientId",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = false,IsSecure = true, Description = "" });
            types.Add(new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse("7bc7d139-bf37-44a1-ae51-c0f989dfb634"),DisplayName = "ClientSecret",Name = "ClientSecret",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = true,IsSecure = true, Description = "" });
            types.Add(new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse("7ead7711-918b-473b-b742-f498e9e12443"),
                DisplayName = "ApiKey",
                    Name = "ApiKey",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = false,
                    IsSecure = true,
                Description = ""
            });
            types.Add(
                new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse("4408690c-8e81-40fe-9de2-d3a8c85704b4"),
                    DisplayName = "AccessToken",
                    Name = "AccessToken",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true,
                    Description = ""
                });
            types.Add(
                new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse("2c24c170-2330-4224-9b64-cf347806ec63"),
                    DisplayName = "Username",
                    Name = "Username",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true,
                    Description = ""
                });
            types.Add(
                new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse("89340c3a-80f3-4913-abe0-5d3a76e9815c"),
                    DisplayName = "Password",
                    Name = "Password",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true,
                    Description = ""
                });

            //maps type...
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("783962fd-238b-46ef-a01d-ec83c1e2524e"), Category = "Maps", Type = "textbox", Name = "BingMapsApiKey",DefaultValue = "", IsMasked = true, IsSecure = true, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("3b2967c1-eb7d-43df-8794-67cf274a6a0a"), Category = "Maps", Type = "textbox", Name = "GoogleMapsApiKey",DefaultValue = "", IsMasked = true, IsSecure = true, Description = "" });

            //system/tenant level configuration props
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("01206917-63ae-420f-babf-99b9bbe6b178"), Category = "Supported Data", Type = "checkbox", Name = "EnableAddress",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("bc7d8fb1-4909-40d9-81fc-4dee4cd4243a"), Category = "Supported Data", Type = "checkbox", Name = "EnableBiometrics",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("1532bca6-c5a8-45e1-ad84-d008074d2b92"), Category = "Supported Data", Type = "checkbox", Name = "EnableDevices",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("ae747daf-de52-4346-b71c-3fa7f3d4696f"), Category = "Supported Data", Type = "checkbox", Name = "EnableDNA",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("9477aa30-6a4d-4eea-a806-4475c9c8e326"), Category = "Supported Data", Type = "checkbox", Name = "EnableEmails",DefaultValue = "1", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("429610e9-5fc4-481e-943f-a21830415682"), Category = "Supported Data", Type = "checkbox", Name = "EnableFido",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("97388fc7-effb-4386-82b9-f454239d6dd4"), Category = "Supported Data", Type = "checkbox", Name = "EnableIdentity",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("4f8fc982-3d99-4b6f-9aec-65e64eecde74"), Category = "Supported Data", Type = "checkbox", Name = "EnableIpAddress",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("d077718a-5858-4a70-8c7b-df7f7dd09fce"), Category = "Supported Data", Type = "checkbox", Name = "EnablePhone",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("45183a98-e682-4219-9691-231a60900217"), Category = "Supported Data", Type = "checkbox", Name = "EnableSocialIdentity",DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "" });

            //mail
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("4b77e942-6e70-4b6d-9a50-a2021b530b8d"), Category = "Mail", Type = "textbox", Name = "MailServer", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("dd2dcb81-31aa-490a-8cc7-705151cd0fc3"), Category = "Mail", Type = "textbox", Name = "MailPort", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("dd98811a-f03c-42d0-b117-e70307ab6a9a"), Category = "Mail", Type = "textbox", Name = "MailUsername", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("a9e768d5-e926-4765-a82e-f02098abdb9f"), Category = "Mail", Type = "textbox", Name = "MailPassword", DefaultValue = "", IsMasked = true, IsSecure = true });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("26f28152-af40-4b8a-bcb2-d830d1120f37"), Category = "Mail", Type = "checkbox", Name = "UseSecurity", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("5d4164f4-9b77-4d88-b1e3-351efdcde547"), Category = "Mail", Type = "textbox", Name = "MailFrom", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("1aaf343c-cff4-44a1-8085-4962c26bacd9"), Category = "Mail", Type = "textbox", Name = "MailReplyTo", DefaultValue = "", IsMasked = false, IsSecure = false });

            return types;
        }
    }

    
}
