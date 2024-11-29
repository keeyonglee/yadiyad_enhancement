using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq
{
    public class ComputeShippingException: Exception
    {
        public ComputeShippingException(string message) : base(message)
        {

        }
    }
}
