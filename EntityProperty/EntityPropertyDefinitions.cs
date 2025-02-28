﻿using System;
using System.Collections.Generic;

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
        public string Options { get; set; }
        public bool IsMasked { get; set; }
        public bool IsSecure { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }

        public static List<EntityPropertyTypeBase> LoadEntityPropertyTypes()
        {
            List<EntityPropertyTypeBase> types = new List<EntityPropertyTypeBase>();

            //temp

            //default application properties
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("5a45b15f-7d44-47de-96f8-b4baa94c3d71"), DisplayName = "Allow Unverified Data", Name = "AllowUnverifiedData", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "Specifies if you should allow unverified data to be submitted to your tenant/application." });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("3af231bd-a7ec-4922-8ad7-78b234705223"), DisplayName = "Batch Size", Name = "BatchSize", DefaultValue = "100", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "The size of the items that should be retrieved on each API call" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("a7c1041c-9a67-4ff8-8b70-be3a7771aa3b"), DisplayName = "Days Between Requests", Name = "DaysBetweenRequests", DefaultValue = "365", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "The minimum number of days between the same type of request for a data subject (default '365')" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("3dfd0327-3b5c-4e67-aa9e-694bd5cee6b4"), DisplayName = "Data Is Sold", Name = "DataIsSold", DefaultValue = "true", Type = "checkbox", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "A flag that specifies if you sell use data" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("c3125b19-5937-4c3d-a323-57f0c085c4b0"), DisplayName = "Delete Requires Approval", Name = "DeleteRequiresApproval", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "If every delete request must be reviewed and approved" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("19561589-41d8-40a4-8f4d-ee48e993583a"), DisplayName = "Export Requires Approval", Name = "ExportRequiresApproval", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "If every export request must be reviewed and approved" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("2559e2b8-1ae3-4c7a-8129-1ba0ce55df62"), DisplayName = "Min Number Of Verified Types", Name = "MinNumberOfVerifiedTypes", Type = "textbox", DefaultValue = "1", Category = "Compliance", IsMasked = false, IsSecure = false, Description = "If you require more than just one verified identity in order to submit a request" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("e6507d74-e3d8-4f3d-bbab-9eb7b391f55f"), DisplayName = "Max Query Requests Per Year", Name = "MaxQueryRequestsPerYear", DefaultValue = "1", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "Max query requests per year" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("ae099068-29fa-4c13-979a-563c6411ef49"), DisplayName = "Per Query Request Cost", Name = "PerQueryRequestCost", DefaultValue = "5.00", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "The cost for executing a query request to your application" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("e920b5e7-b130-4bbc-b255-ad299cf86d56"), DisplayName = "Export Timeout", Name = "ExportTimeout", DefaultValue = "", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false, Description = "The number of seconds before an export request will timeout and perform a polling action" });

            //paypal
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("5ac728e9-8639-4ec7-8494-d3f268115e10"), Category = "Payment", Type = "textbox", DisplayName = "Paypal Client Id", Name = "PaypalClientId", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "The paypal client id for your payment account" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("e8984fc7-dda7-46cf-8a22-911266dc7af7"), Category = "Payment", Type = "textbox", DisplayName = "Paypal Client Secret", Name = "PaypalClientSecret", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "The paypal client secret for your payment account" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("11e88b0d-b308-47b9-953c-bdffdb018aa5"), Category = "Payment", Type = "textbox", DisplayName = "Paypal Mode",  Name = "PaypalMode", DefaultValue = "sandbox", IsMasked = true, IsSecure = true, Description = "The payment gateway mode {Development or Production}" });

            //hidden
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("00000000-0000-0000-0000-000000000000"), DisplayName = "Hidden", Name = "Hidden", DefaultValue = "", Type = "textbox", Category = "Hidden", IsMasked = false, IsSecure = true, Description = "These never show up in the UI" });

            //core system props
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("1a4cf053-2c3c-4832-a1ec-e6a467971496"), DisplayName = "Phone Number", Name = "PhoneNumber", DefaultValue = "", Type = "textbox", Category = "IVR", IsMasked = false, IsSecure = false, Description = "This is the phone number that the IVR web will response and call with" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("2b8162aa-ef4d-4c08-a863-88b7f4c5598f"), DisplayName = "Registration Password", Name = "RegistrationPassword", DefaultValue = "", Type = "textbox", Category = "Configuration", IsMasked = false, IsSecure = false, Description = "When registering your system with other remote systems, this is the password required to pair" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("4af73175-8408-465b-8f82-3972cfeaa366"), DisplayName = "System Password", Name = "SystemPassword", DefaultValue = "", Type = "textbox", Category = "Configuration", IsMasked = false, IsSecure = false, Description = "This is the default system password and private key for the PGP certification" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("09683fed-dcae-4bb1-aa9e-48f01d95061c"), DisplayName = "License Key", Name = "LicenseKey", DefaultValue = "", Type = "textbox", Category = "Configuration", IsMasked = false, IsSecure = false, Description = "For on-premises or hosted systems, this is your license key" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("d64b5ee7-013c-4409-8023-7e7c8af33562"), DisplayName = "External Dns", Name = "ExternalDns", DefaultValue = "", Type = "textbox", Category = "Configuration", IsMasked = false, IsSecure = false, Description = "This is the external internet DNS name for the consumer web" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("332ea56f-c6a9-4011-aedb-888bb4457cdb"), DisplayName = "Notify On Discovery", Name = "NotifyOnDiscovery", DefaultValue = "0", Type = "checkbox", Category = "Configuration", IsMasked = false, IsSecure = false, Description = "If a user is discovered in an application discovery, they will receive a notification of such" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("62c17e22-d907-49bd-81ba-5bbb323a8808"), DisplayName = "Allow Trials", Category = "Configuration", Type = "checkbox", Name = "AllowTrials", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "This is a setting for the system on whether it will allow trial tenants to be created" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("38b10751-7d4e-4e8c-a802-95886c7e1a40"), DisplayName = "Remove Minors", Category = "Configuration", Type = "checkbox", Name = "RemoveMinors", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Whether you will allow the system to remove known minors from your applications" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("1a7eb7de-dc79-42bb-89ff-d55b673f2bf4"), DisplayName = "Enable Auto Upgrade", Category = "Configuration", Type = "checkbox", Name = "EnableAutoUpgrade", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "This setting will ensure that users are able to submit requests if you go over your data subject limit" });

            //OAuth properties
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("039a78ba-802a-4155-9126-b43931197afd"),DisplayName = "Client Id",Name = "ClientId",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = false,IsSecure = true, Description = "The application's client id for OAuth" });
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("7bc7d139-bf37-44a1-ae51-c0f989dfb634"),DisplayName = "Client Secret",Name = "ClientSecret",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = true,IsSecure = true, Description = "The application's client secret for OAuth" });
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("7ead7711-918b-473b-b742-f498e9e12443"),DisplayName = "Api Key",Name = "ApiKey",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = false,IsSecure = true,Description = "The account API key to make API calls with (typically in absense of OAuth)"});
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("4408690c-8e81-40fe-9de2-d3a8c85704b4"),DisplayName = "Access Token",Name = "AccessToken",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = true,IsSecure = true,Description = "The access token that is retrieved during the OAuth process"});
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("2c24c170-2330-4224-9b64-cf347806ec63"),DisplayName = "Username",Name = "Username",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = true,IsSecure = true,Description = "The username used for auth events"});
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("89340c3a-80f3-4913-abe0-5d3a76e9815c"),DisplayName = "Password",Name = "Password",Category = "Security",Type = "textbox",DefaultValue = "",IsMasked = true,IsSecure = true, Description = "The password used for auth events"});
            types.Add(new EntityPropertyTypeBase() {EntityPropertyTypeId = Guid.Parse("0ec6bb9b-8bcf-4935-af63-7bb01fa80dc4"), DisplayName = "Mfa Secret", Name = "MfaSecret", Category = "Security", Type = "textbox", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "The TOTP MFA Secret for MFA logins" });

            //security
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("b3e06ea6-0c40-4e65-8ed3-d311f802dbcd"), DisplayName = "Hash Support", Name = "HashSupport", DefaultValue = "", Type = "checkbox", Category = "Security", IsMasked = false, IsSecure = false, Description = "If your application supports hashing subject data" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("7d6d21dc-a3b0-4c20-8791-4124d42fdf1b"), DisplayName = "Hash Type", Name = "HashType", DefaultValue = "", Type = "singleselect", Options = "MD5|RIPEMD160|SHA1|SHA256|SHA384|SHA512", Category = "Security", IsMasked = false, IsSecure = false, Description = "The Hash algorithm used" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("5b0e96bf-3ee3-4215-8a20-67225febc4f7"), DisplayName = "Hash Salt", Name = "HashSalt", DefaultValue = "", Type = "textbox", Category = "Security", IsMasked = true, IsSecure = true, Description = "The salt for your hash algo" });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse("a9f7ce9d-1960-431d-80f9-06154f8909e3"), DisplayName = "Email Domain", Name = "EmailDomain", DefaultValue = "", Type = "textbox", Category = "Security", IsMasked = false, IsSecure = false, Description = "The domain subject requests are restricted too" });

            //maps type...
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("783962fd-238b-46ef-a01d-ec83c1e2524e"), Category = "Maps", Type = "textbox", DisplayName = "Bing Maps ApiKey", Name = "BingMapsApiKey",DefaultValue = "", IsMasked = true, IsSecure = true, Description = "Your Bing maps API key for geolocation lookups" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("3b2967c1-eb7d-43df-8794-67cf274a6a0a"), Category = "Maps", Type = "textbox", DisplayName = "Google Maps ApiKey", Name = "GoogleMapsApiKey",DefaultValue = "", IsMasked = true, IsSecure = true, Description = "Your Google maps API key for geolocation lookups" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("bcf5c431-74ad-4e8b-887b-ba3326558ee0"), Category = "Maps", Type = "textbox", DisplayName = "Azure Maps ApiKey", Name = "AzureMapsApiKey", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "Your Azure maps API key for geolocation lookups" });

            //system/tenant level configuration props
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("01206917-63ae-420f-babf-99b9bbe6b178"), Category = "Supported Data", Type = "checkbox", Name = "EnableAddress", DisplayName= "Enable Address", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on address input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("bc7d8fb1-4909-40d9-81fc-4dee4cd4243a"), Category = "Supported Data", Type = "checkbox", Name = "EnableBiometrics", DisplayName = "Enable Biometrics", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on biometric input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("1532bca6-c5a8-45e1-ad84-d008074d2b92"), Category = "Supported Data", Type = "checkbox", Name = "EnableDevices", DisplayName = "Enable Devices", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on device input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("ae747daf-de52-4346-b71c-3fa7f3d4696f"), Category = "Supported Data", Type = "checkbox", Name = "EnableDNA", DisplayName = "Enable DNA", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on DNA input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("9477aa30-6a4d-4eea-a806-4475c9c8e326"), Category = "Supported Data", Type = "checkbox", Name = "EnableEmails", DisplayName = "Enable Emails", DefaultValue = "1", IsMasked = false, IsSecure = false, Description = "Turns on email input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("429610e9-5fc4-481e-943f-a21830415682"), Category = "Supported Data", Type = "checkbox", Name = "EnableFido2", DisplayName = "Enable Fido2", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on FIDO2 device support" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("97388fc7-effb-4386-82b9-f454239d6dd4"), Category = "Supported Data", Type = "checkbox", Name = "EnableIdentity", DisplayName = "Enable Identity", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on identity input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("4f8fc982-3d99-4b6f-9aec-65e64eecde74"), Category = "Supported Data", Type = "checkbox", Name = "EnableIpAddress", DisplayName = "Enable Ip Address", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on IP Address input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("dbc217ad-7cfc-45bd-b361-215b71201be1"), Category = "Supported Data", Type = "checkbox", Name = "EnablePersonal", DisplayName = "Enable Personal", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on personal info input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("d077718a-5858-4a70-8c7b-df7f7dd09fce"), Category = "Supported Data", Type = "checkbox", Name = "EnablePhone", DisplayName = "Enable Phone", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on phone input and submission" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("45183a98-e682-4219-9691-231a60900217"), Category = "Supported Data", Type = "checkbox", Name = "EnableSocialIdentity", DisplayName = "Enable Social Identity", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on social identity input and submission" });

            //search
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("0783ebc1-a12d-4a41-bcab-c7436e045c45"), Category = "Search", Type = "checkbox", Name = "EnablePhoneFormatsSearch", DisplayName = "Enable Phone Format Search", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Turns on the phone number permutation search" });

            //notifications
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("90f05064-4102-49e8-b1d4-2d3ee2b37a61"), Category = "Notifications", Type = "checkbox", Name = "NotifyOnDiscovery", DisplayName = "Notify On Discovery", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Should subjects be notified on discovery?" });

            //ipaddresses
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("c27f82b3-80c8-4bf2-a77d-aea11960e830"), Category = "IpAddress", Type = "checkbox", Name = "EnableIpAddressResolution", DisplayName = "Enable IpAddress Resolution", DefaultValue = "0", IsMasked = false, IsSecure = false, Description = "Try to resolve IP Addresses to geo addresses?" });

            //mail
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("4b77e942-6e70-4b6d-9a50-a2021b530b8d"), Category = "Mail", Type = "textbox", Name = "MailServer", DisplayName = "Mail Server", DefaultValue = "", IsMasked = false, IsSecure = false, Description = "SMTP Mail server" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("dd2dcb81-31aa-490a-8cc7-705151cd0fc3"), Category = "Mail", Type = "textbox", Name = "MailPort", DisplayName = "Mail Port", DefaultValue = "", IsMasked = false, IsSecure = false, Description = "SMTP Mail Port" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("dd98811a-f03c-42d0-b117-e70307ab6a9a"), Category = "Mail", Type = "textbox", Name = "MailUsername", DisplayName = "Mail Username", DefaultValue = "", IsMasked = false, IsSecure = false, Description = "SMTP Mail username" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("a9e768d5-e926-4765-a82e-f02098abdb9f"), Category = "Mail", Type = "textbox", Name = "MailPassword", DisplayName = "Mail Password", DefaultValue = "", IsMasked = true, IsSecure = true, Description = "SMTP Mail Password" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("26f28152-af40-4b8a-bcb2-d830d1120f37"), Category = "Mail", Type = "checkbox", Name = "UseSecurity", DisplayName = "Use Security", DefaultValue = "1", IsMasked = false, IsSecure = false, Description = "Flag for username/password authentication" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("5d4164f4-9b77-4d88-b1e3-351efdcde547"), Category = "Mail", Type = "textbox", Name = "MailFrom", DisplayName = "Mail From", DefaultValue = "", IsMasked = false, IsSecure = false, Description = "The FROM address for outgoing emails" });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("1aaf343c-cff4-44a1-8085-4962c26bacd9"), Category = "Mail", Type = "textbox", Name = "MailReplyTo", DisplayName = "Mail Reply To", DefaultValue = "", IsMasked = false, IsSecure = false, Description = "The REPLY address for outgoing emails" });

            //azure storage
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("315448a3-b834-4890-aeb9-5bfffe3328c3"), Category = "Azure", Type = "checkbox", Name = "SubscriptionId", DisplayName="Subscription Id", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("b5bbbf26-8e78-4d01-9ffd-acc5ca7eea29"), Category = "Azure", Type = "checkbox", Name = "ResourceGroup", DisplayName="Resource Group", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("b97e9b6e-40d3-4556-8293-3b9c712244b6"), Category = "Azure", Type = "checkbox", Name = "StorageAccountName", DisplayName="Storage Account Name", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("fcf5ffe6-a532-4e03-b8a6-c747a642a995"), Category = "Azure", Type = "checkbox", Name = "StorageAccountKey", DisplayName="Storage Account Key", DefaultValue = "", IsMasked = false, IsSecure = false });

            //search
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("fcf5ffe6-a532-4e03-b8a6-c747a642a995"), Category = "Search", Type = "checkbox", Name = "PreQuery", DisplayName="Pre Query", DefaultValue = "", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase { EntityPropertyTypeId = Guid.Parse("fcf5ffe6-a532-4e03-b8a6-c747a642a995"), Category = "Search", Type = "checkbox", Name = "PostQuery", DisplayName="Post Query", DefaultValue = "", IsMasked = false, IsSecure = false });

            return types;
        }
    }
}
