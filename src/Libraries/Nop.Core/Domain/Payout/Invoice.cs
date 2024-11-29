using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Payout
{
    public class Invoice : BaseEntityExtension
    {
        public string InvoiceNumber { get; set; }

        public int InvoiceTo { get; set; }

    }
}
