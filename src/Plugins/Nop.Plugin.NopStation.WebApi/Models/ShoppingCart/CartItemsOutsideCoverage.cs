using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Models.ShoppingCart
{
    public class CartItemsOutsideCoverage
    {
        public int Id { get; set; }
        public int? DeliveryModeId { get; set; } = (int)DeliveryMode.Bike;
        public int VendorId { get; set; }
    }
}
