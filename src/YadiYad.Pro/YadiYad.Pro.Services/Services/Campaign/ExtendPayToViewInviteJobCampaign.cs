using System;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Services.Services.Subscription;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class ExtendPayToViewInviteJobCampaign : ICampaignProcessor
    {
        private readonly ServiceSubscriptionService _serviceSubscriptionService;
        private readonly ICustomerService _customerService;
        public CampaignType CampaignType => CampaignType.ExtendPayToViewAndInvite;
        public CampaignProcessType ProcessType => CampaignProcessType.Process;

        public ExtendPayToViewInviteJobCampaign(ServiceSubscriptionService serviceSubscriptionService, ICustomerService customerService)
        {
            _serviceSubscriptionService = serviceSubscriptionService;
            _customerService = customerService;
        }
        public void Process(int customerId, decimal campaignValue, int refId = 0, Action<int> onSuccess = null)
        {
            if (customerId <= 0)
                return;

            if (campaignValue <= 0)
                return;

            if (refId == 0)
                return;
            
            var customer = _customerService.GetCustomerById(customerId);
            var isOrg = _customerService.IsInCustomerRole(customer, NopCustomerDefaults.OrganizationRoleName);

            if (!isOrg)
                return;

            _serviceSubscriptionService.ExtendSubscription(customerId, (int)campaignValue, refId);

            if (onSuccess != null)
                onSuccess(refId);
        }

    }
}