using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class PaymentDetailsDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public int BaseInvoiceNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
    }
}
