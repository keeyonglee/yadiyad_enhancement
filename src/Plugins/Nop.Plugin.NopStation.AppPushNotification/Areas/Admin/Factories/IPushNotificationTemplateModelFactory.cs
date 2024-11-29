using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public interface IPushNotificationTemplateModelFactory
    {
        AppPushNotificationTemplateSearchModel PreparePushNotificationTemplateSearchModel(AppPushNotificationTemplateSearchModel searchModel);

        AppPushNotificationTemplateListModel PreparePushNotificationTemplateListModel(AppPushNotificationTemplateSearchModel searchModel);

        AppPushNotificationTemplateModel PreparePushNotificationTemplateModel(AppPushNotificationTemplateModel model, 
            AppPushNotificationTemplate pushNotificationTemplate, bool excludeProperties = false);
    }
}