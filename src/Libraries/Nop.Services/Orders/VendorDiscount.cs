using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Orders
{
    public class VendorDiscount
    {
        #region Ctor

        public VendorDiscount()
        {
        }

        #endregion

        /// <summary>
        /// The vendor id
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// The shipping cost
        /// </summary>
        public decimal DiscountTotal { get; set; }
        public List<Discount> Discounts { get; set; }

        public decimal OrderTotalDiscount { get; set; }
        public decimal SubTotalDiscount { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal PlatformShippingDiscount { get; set; }
        public decimal PlatformSubTotalDiscount { get; set; }
        public decimal SubTotalWithoutDiscountBase { get; set; }
        public decimal ShippingEstimatedCost { get; set; }

    }
}