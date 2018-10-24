namespace GDPR.Util.Data
{
    using System;
    using System.Collections.Generic;

    public partial class BaseEntityProperty
    {
        public System.Guid EntityPropertyId { get; set; }
        public System.Guid EntityId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public System.DateTime ModifyDate { get; set; }
        public System.DateTime CreateDate { get; set; }
        public bool IsSecure { get; set; }
        public bool IsMasked { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
    }
}
