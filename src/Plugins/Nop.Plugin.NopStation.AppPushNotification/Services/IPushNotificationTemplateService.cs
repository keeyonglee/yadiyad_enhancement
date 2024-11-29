using Nop.Core;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public interface IPushNotificationTemplateService
    {
        void DeletePushNotificationTemplate(AppPushNotificationTemplate pushNotificationTemplate);

        void InsertPushNotificationTemplate(AppPushNotificationTemplate pushNotificationTemplate);

        void UpdatePushNotificationTemplate(AppPushNotificationTemplate pushNotificationTemplate);

        AppPushNotificationTemplate GetPushNotificationTemplateById(int pushNotificationTemplateId);

        IPagedList<AppPushNotificationTemplate> GetAllPushNotificationTemplates(string keyword = null, bool? active = null, 
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        IList<AppPushNotificationTemplate> GetPushNotificationTemplatesByName(string messageTemplateName, int storeId = 0);

        IList<AppPushNotificationTemplate> GetTemplatesByIds(int[] templateIds);
    }
}