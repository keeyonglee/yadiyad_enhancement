using System;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Services.Subscription;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class PayToViewInviteJobsCampaign : ICampaignProcessor
    {
        private readonly ICustomerService _customerService;
        private readonly ServiceSubscriptionService _subscriptionService;
        public CampaignType CampaignType => CampaignType.PayToViewAndInvite;
        public CampaignProcessType ProcessType => CampaignProcessType.Process;
        public SubscriptionType SubscriptionType => SubscriptionType.ViewJobCandidateFulleProfile;

        public PayToViewInviteJobsCampaign(ICustomerService customerService, ServiceSubscriptionService subscriptionService)
        {
            _customerService = customerService;
            this._subscriptionService = subscriptionService;
        }
        
        public void Process(int customerId, decimal subscriptionDays, int refId = 0, Action<int> onSuccess = null)
        {
            if (customerId == default)
                return;

            if (subscriptionDays <= 0)
                return;

            if (refId == 0)
                return;
            
            var customer = _customerService.GetCustomerById(customerId);
            var isOrg = _customerService.IsInCustomerRole(customer, NopCustomerDefaults.OrganizationRoleName);

            if (!isOrg)
                return;
            
            var service = _subscriptionService.CreateServiceSubscription(customerId, customerId, SubscriptionType, (int)subscriptionDays, refId);
            
            if (onSuccess != null && service != null)
                onSuccess(service.Id);
        }
    }
}