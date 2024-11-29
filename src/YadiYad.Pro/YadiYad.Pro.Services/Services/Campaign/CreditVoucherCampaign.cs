using System;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class CreditVoucherCampaign : ICampaignProcessor
    {
        public CampaignType CampaignType => CampaignType.CreditVoucher;
        public CampaignProcessType ProcessType => CampaignProcessType.Process;
        
        public void Process(int customerId, decimal creditValue, int refId = 0, Action<int> onSuccess = null)
        {
            
        }
    }
}