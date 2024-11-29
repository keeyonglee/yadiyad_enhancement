using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq
{
    public class UnableToLocateDriverEvent
    {
        public Order Order { get; private set; }

        public UnableToLocateDriverEvent(Order order)
        {
            Order = order;
        }
    }
}
