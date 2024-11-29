using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq
{
    public class NotInCoverageShippingException: Exception
    {
        public NotInCoverageShippingException(string message):base(message)
        {

        }
    }
}
