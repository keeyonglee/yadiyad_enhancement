using System;
using YadiYad.Pro.Core.Domain.Campaign;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class CreditVoucherReferralCampaign : ICampaignProcessor
    {
        public CampaignType CampaignType => CampaignType.CreditVoucherReferral;
        public CampaignProcessType ProcessType => CampaignProcessType.Process;
        public bool NeedReferral => true;

        public void Process(int customerId, decimal value1, int refId = 0, Action<int> onSuccess = null)
        {
            throw new NotImplementedException();
        }
    }
}