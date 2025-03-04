using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobTransactionHistory : BaseEntityExtension
    {
        public int JobProfileId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public int Status { get; set; } // Enum: Pending, Completed, Failed, etc.
        public DateTime TransactionDate { get; set; }
        public string TransactionReference { get; set; }
    }
}
