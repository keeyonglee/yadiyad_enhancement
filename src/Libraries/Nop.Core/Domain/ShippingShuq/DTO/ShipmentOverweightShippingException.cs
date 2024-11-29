using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq.DTO
{
    public class ShipmentOverweightShippingException : Exception
    {
        public ShipmentOverweightShippingException(string message) : base(message)
        {

        }
    }
}
