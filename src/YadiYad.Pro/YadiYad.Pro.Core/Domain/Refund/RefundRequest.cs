using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Refund
{
    public class RefundRequest : BaseEntityExtension
    {
        public int OrderItemId { get; set; }
        public int BaseRefundNumber { get; set; }
        public string RefundNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceCharge { get; set; }
        public int RefundTo { get; set; }
        public int Status { get; set; }
    }
}
