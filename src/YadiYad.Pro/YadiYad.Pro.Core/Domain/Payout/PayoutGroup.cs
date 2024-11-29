using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutGroup : BaseEntityExtension
    {
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public int Status { get; set; }
        public int PayoutTo { get; set; }
        public int PayoutBatchId { get; set; }
        public string Remarks { get; set; }
    }
}
