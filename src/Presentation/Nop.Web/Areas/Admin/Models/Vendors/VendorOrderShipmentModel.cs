using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    public partial class VendorOrderShipmentModel : BaseNopModel
    {
        public int OrderId { get; set; }
        public int ShipmentId { get; set; }
        public string OrderStatus { get; set; }
        public int OrderStatusId { get; set; }
        public string ShippingStatus { get; set; }
        public int ShippingStatusId { get; set; }
        public DateTime OrderTime { get; set; }
        public string CheckoutAttributeXML { get; set; }
        public DateTime DueDate { get; set; }
    }
}