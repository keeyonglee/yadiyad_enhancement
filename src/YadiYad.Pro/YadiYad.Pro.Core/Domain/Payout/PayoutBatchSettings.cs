using Nop.Core.Configuration;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutBatchSettings : ISettings
    {
        public int EndDateCutoff { get; set; }
    }
}