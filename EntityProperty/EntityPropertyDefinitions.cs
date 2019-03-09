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

            
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "AllowUnverifiedData", Name = "AllowUnverifiedData", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "BatchSize", Name = "BatchSize", DefaultValue = "100", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "DaysBetweenRequests", Name = "DaysBetweenRequests", DefaultValue = "90", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "DataIsSold", Name = "DataIsSold", DefaultValue = "true", Type = "checkbox", Category = "Compliance", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "DeleteRequiresApproval", Name = "DeleteRequiresApproval", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "ExportRequiresApproval", Name = "ExportRequiresApproval", Type = "checkbox", DefaultValue = "true", Category = "Compliance", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "MinNumberOfVerifiedTypes", Name = "MinNumberOfVerifiedTypes", Type = "textbox", DefaultValue = "1", Category = "Compliance", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "MaxQueryRequestsPerYear", Name = "MaxQueryRequestsPerYear", DefaultValue = "1", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false });
            types.Add(new EntityPropertyTypeBase() { EntityPropertyTypeId = Guid.Parse(""), DisplayName = "PerQueryRequestCost", Name = "PerQueryRequestCost", DefaultValue = "5.00", Type = "textbox", Category = "General", IsMasked = false, IsSecure = false });

            types.Add(new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse(""),
                DisplayName = "ClientId",
                    Name = "ClientId",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = false,
                    IsSecure = true
                });
            types.Add(new EntityPropertyTypeBase(){EntityPropertyTypeId = Guid.Parse(""),
                    DisplayName = "ClientSecret",
                    Name = "ClientSecret",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true
                });
            types.Add(new EntityPropertyTypeBase()
                {
                EntityPropertyTypeId = Guid.Parse(""),
                DisplayName = "ApiKey",
                    Name = "ApiKey",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = false,
                    IsSecure = true
                });
            types.Add(
                new EntityPropertyTypeBase()
                {
                    EntityPropertyTypeId = Guid.Parse(""),
                    DisplayName = "AccessToken",
                    Name = "AccessToken",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true
                });
            types.Add(
                new EntityPropertyTypeBase()
                {
                    EntityPropertyTypeId = Guid.Parse(""),
                    DisplayName = "Username",
                    Name = "Username",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true
                });
            types.Add(
                new EntityPropertyTypeBase()
                {
                    EntityPropertyTypeId = Guid.Parse(""),
                    DisplayName = "Password",
                    Name = "Password",
                    Category = "Security",
                    Type = "textbox",
                    DefaultValue = "",
                    IsMasked = true,
                    IsSecure = true
                });

            return types;
        }
    }

    
}
