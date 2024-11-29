using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.ShippingShuq
{
    public interface IShipmentProcessor
    {
        void Ship(Shipment shipment);
        ShipmentCarrierCoverageChecking CheckProductCoverage(Vendor vendor, Customer customer, Address customerAddress);
    }
}