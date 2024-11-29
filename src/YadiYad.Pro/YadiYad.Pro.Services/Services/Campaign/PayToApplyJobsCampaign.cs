using System;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Services.Subscription;

namespace YadiYad.Pro.Services.Services.Campaign
{
    public class PayToApplyJobsCampaign : ICampaignProcessor
    {
        private readonly ICustomerService _customerService;
        private readonly ServiceSubscriptionService _subscriptionService;
        public CampaignType CampaignType => CampaignType.PayToApplyJobs;
        public CampaignProcessType ProcessType => CampaignProcessType.Process;
        public SubscriptionType SubscriptionType => SubscriptionType.ApplyJob;

        public PayToApplyJobsCampaign(ICustomerService customerService, ServiceSubscriptionService subscriptionService)
        {
            _customerService = customerService;
            _subscriptionService = subscriptionService;
        }
        
        public void Process(int customerId, decimal subscriptionDays, int refId = 0, Action<int> onSuccess = null)
        {
            if (customerId <= 0)
                return;

            if (subscriptionDays <= 0)
                return;
            
            var customer = _customerService.GetCustomerById(customerId);
            var isIndividual = _customerService.IsInCustomerRole(customer, NopCustomerDefaults.IndividualRoleName);

            if (!isIndividual)
                return;

            var service = _subscriptionService.CreateServiceSubscription(customerId, customerId, SubscriptionType, (int)subscriptionDays);

            if (onSuccess != null && service != null)
                onSuccess(service.Id);
        }
    }
}