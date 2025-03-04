using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class Payment : BaseEntityExtension
    {
        public int OrderItemId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int PayerId { get; set; }
        public int PayeeId { get; set; }
        public int Status { get; set; } 

        public decimal Amount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal ServiceChargeRate { get; set; }
        public int ServiceChargeType { get; set; }

        public string PaymentMethod { get; set; } 
        public string TransactionId { get; set; } 
        public string PaymentNote { get; set; }

    }
}
