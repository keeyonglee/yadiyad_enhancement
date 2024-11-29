using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Catalog
{
    public class ProductSetupSettings : ISettings
    {
        public bool InventoryEnabled { get; set; }
        public bool AllowGroupedProductOnly { get; set; }
        public bool AllowChangeProductType { get; set; }
        public bool DefaultAllowCustomerReviews { get; set; }
    }
}
