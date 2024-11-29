using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Checkout
{
    public partial class CustomCheckoutAttributeModel
    {
        public int VendorId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeValue { get; set; }

    }
}