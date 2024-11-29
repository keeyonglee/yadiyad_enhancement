using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public interface IPushNotificationCampaignService
    {
        IPagedList<AppPushNotificationCampaign> GetAllPushNotificationCampaigns(string keyword = "",
            DateTime? searchFrom = null, DateTime? searchTo = null, bool? addedToQueueStatus = null,
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        AppPushNotificationCampaign InsertPushNotificationCampaign(AppPushNotificationCampaign campaign);

        AppPushNotificationCampaign GetPushNotificationCampaignById(int id);

        void UpdatePushNotificationCampaign(AppPushNotificationCampaign campaign);

        void UpdatePushNotificationCampaigns(IList<AppPushNotificationCampaign> campaigns);

        void DeletePushNotificationCampaign(AppPushNotificationCampaign campaign);

        IPagedList<ApiDevice> GetCampaignDevices(AppPushNotificationCampaign campaign, int pageIndex = 0);
    }
}
