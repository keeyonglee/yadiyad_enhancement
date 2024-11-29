using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public interface IQueuedPushNotificationModelFactory
    {
        AppQueuedPushNotificationSearchModel PrepareQueuedPushNotificationSearchModel(AppQueuedPushNotificationSearchModel searchModel);

        AppQueuedPushNotificationListModel PrepareQueuedPushNotificationListModel(AppQueuedPushNotificationSearchModel searchModel);

        AppQueuedPushNotificationModel PrepareQueuedPushNotificationModel(AppQueuedPushNotificationModel model, 
            AppQueuedPushNotification queuedPushNotification, bool excludeProperties = false);
    }
}