using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Nop.Core.Domain.Orders
{
    public enum OrderType
    {
        Pro = 1,
        Shuq = 2
    }

    public enum OrderSortBy
    {
        [Description("Purchase Order")]
        PurchaseOrder = 1,
        [Description("Delivery Slot")]
        DeliverySlot = 2
    }
}
