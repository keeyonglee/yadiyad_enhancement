using System;
using System.Collections.Generic;
using System.Text;

using Nop.Core.Domain.Orders;

namespace Nop.Services.Orders
{
    public partial interface IOrderProcessingService_Pro
    {
        void ProcessOrder(ICustomOrderEntity customOrder);
    }
}
