using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Orders
{
    public class VendorShipping
    {
        #region Ctor

        public VendorShipping()
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
        public decimal ShippingCost { get; set; }

        public bool NotInsideCoverage { get; set; }
        public bool NotInsideCoverageCar { get; set; }
        public bool NotInsideCoverageBike { get; set; }
        public bool ShipmentOverweight { get; set; }
        public decimal MaxShipmentWeight { get; set; }
        public bool FailComputeShipping { get; set; }
    }
}