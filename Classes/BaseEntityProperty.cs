namespace GDPR.Common.Data
{
    using System;
    using System.Collections.Generic;

    public partial class BaseEntityProperty
    {
        virtual public System.Guid EntityPropertyId { get; set; }
        virtual public System.Guid EntityPropertyTypeId { get; set; }
        virtual public System.Guid EntityId { get; set; }
        virtual public string Name { get; set; }
        virtual public string Value { get; set; }
        virtual public System.DateTime ModifyDate { get; set; }
        virtual public System.DateTime CreateDate { get; set; }
        virtual public bool IsSecure { get; set; }
        virtual public bool IsEncrypted { get; set; }
        virtual public bool IsMasked { get; set; }
        virtual public bool IsReadOnly { get; set; }
        virtual public string Description { get; set; }
        virtual public string DisplayName { get; set; }
        virtual public string Type { get; set; }
        virtual public int SystemPinVersion { get; set; }
        virtual public string Category { get; set; }

        
    }
}
