using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq
{
    public class VendorAttributeValue_ShippingMethod_Mapping : BaseEntity
    {
        public int VendorAttributeValueId { get; set; }
        public int ShippingMethodId { get; set; }
    }
}
