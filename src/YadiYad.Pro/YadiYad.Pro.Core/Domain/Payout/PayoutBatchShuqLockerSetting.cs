using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutBatchShuqLockerSetting : ISettings
    {
        public DateTime? LastGeneratingUTCDateTime { get; set; }
        public DateTime? LastGeneratedUTCDateTime { get; set; }
    }
}
