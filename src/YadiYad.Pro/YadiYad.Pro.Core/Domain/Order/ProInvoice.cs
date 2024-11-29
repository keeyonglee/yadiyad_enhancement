using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class ProInvoice : BaseEntityExtension
    {
        public int BaseInvoiceNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public int RefType { get; set; }
        public int InvoiceTo { get; set; }
        public int? InvoiceFrom { get; set; }
    }
}
