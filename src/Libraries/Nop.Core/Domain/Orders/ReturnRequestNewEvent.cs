using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Orders
{
    public class ReturnRequestNewEvent
    {
        public Order Order { get; private set; }

        public ReturnRequestNewEvent(Order order)
        {
            Order = order;
        }
    }
}
