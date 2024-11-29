using System;
using System.Collections.Generic;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Orders
{
    public class ItemToBeDelivered
    {
        #region Ctor

        public ItemToBeDelivered()
        {
        }

        #endregion

        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime DeliveryDate { get; set; }
    }
}