using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Vendor
{
    public class VendorAttributesDTO
    {
        public List<VendorAttribute> Attributes { get; set; } = new List<VendorAttribute>();
    }
    public class VendorAttribute
    {
        public int Id { get; set; }
        public VendorAttributeValue Value { get; set; } = new VendorAttributeValue();
    }
    public class VendorAttributeValue
    {
        public string Value { get; set; }
    }
}
