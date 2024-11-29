using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Tasks;
using System;
using Nop.Core;

namespace Nop.Plugin.NopStation.AppPushNotification
{
    public class AppPushNotificationCampaignSendTask : IScheduleTask
    {
        private readonly IPushNotificationCampaignService _pushNotificationCampaignService;
        private readonly IWorkflowNotificationService _workflowNotificationService;
        private readonly IStoreContext _storeContext;

        public AppPushNotificationCampaignSendTask(IPushNotificationCampaignService pushNotificationCampaignService,
            IWorkflowNotificationService workflowNotificationService,
            IStoreContext storeContext)
        {
            _pushNotificationCampaignService = pushNotificationCampaignService;
            _workflowNotificationService = workflowNotificationService;
            _storeContext = storeContext;
        }

        public void Execute()
        {
            var campaigns = _pushNotificationCampaignService.GetAllPushNotificationCampaigns(
                searchTo: DateTime.UtcNow,
                addedToQueueStatus: false,
                storeId: _storeContext.CurrentStore.Id);

            for (int i = 0; i < campaigns.Count; i++)
            {
                var campaign = campaigns[i];
                _workflowNotificationService.SendCampaignNotification(campaign);

                campaign.AddedToQueueOnUtc = DateTime.UtcNow;
            }
            _pushNotificationCampaignService.UpdatePushNotificationCampaigns(campaigns);
        }
    }
}
