using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;

namespace Nop.Core.Domain.Catalog
{
    public class LowStockProductEvent
    {
        public Product Product { get; private set; }
        public Vendor Vendor { get; private set; }

        public LowStockProductEvent(Product product, Vendor vendor)
        {
            Product = product;
            Vendor = vendor;
        }
    }
}