using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;

namespace Nop.Core.Domain.Catalog
{
    public class LowStockProductAttributeEvent
    {
        public ProductAttributeCombination Combination { get; private set; }
        public Vendor Vendor { get; private set; }

        public LowStockProductAttributeEvent(ProductAttributeCombination combination, Vendor vendor)
        {
            Combination = combination;
            Vendor = vendor;
        }
    }
}