using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class AppPushNotificationTemplateModel : BaseNopEntityModel, ILocalizedModel<AppPushNotificationTemplateLocalizedModel>, IStoreMappingSupportedModel
    {
        public AppPushNotificationTemplateModel()
        {
            AvailableActionTypes = new List<SelectListItem>();
            Locales = new List<AppPushNotificationTemplateLocalizedModel>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
            AvailableTemplates = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Body")]
        public string Body { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ImageId")]
        public int ImageId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.LimitedToStores")]
        public IList<int> SelectedStoreIds { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.DelayBeforeSend")]
        [UIHint("Int32Nullable")]
        public int? DelayBeforeSend { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ActionType")]
        public int ActionTypeId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.ActionValue")]
        public string ActionValue { get; set; }

        public int DelayPeriodId { get; set; }

        public IList<SelectListItem> AvailableActionTypes { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableTemplates { get; set; }

        public IList<AppPushNotificationTemplateLocalizedModel> Locales { get; set; }
    }

    public partial class AppPushNotificationTemplateLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }
        
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.Fields.Body")]
        public string Body { get; set; }
    }
}
