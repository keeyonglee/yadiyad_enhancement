using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutRequestSettings :ISettings
    {
        public int AutoApprovalDays { get; set; }
        public int CsvMaxRecord { get; set; }
        public decimal CsvMaxTotalAmount { get; set; }
        public int MaxPayoutGenerationSeconds { get; set; } = 5 * 60;
    }
}
