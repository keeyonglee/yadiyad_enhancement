using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public interface IPushNotificationCampaignModelFactory
    {
        AppPushNotificationCampaignSearchModel PreparePushNotificationCampaignSearchModel(AppPushNotificationCampaignSearchModel searchModel);

        AppPushNotificationCampaignListModel PreparePushNotificationCampaignListModel(AppPushNotificationCampaignSearchModel searchModel);

        AppPushNotificationCampaignModel PreparePushNotificationCampaignModel(AppPushNotificationCampaignModel model,
            AppPushNotificationCampaign pushNotificationCampaign, bool excludeProperties = false);
    }
}
