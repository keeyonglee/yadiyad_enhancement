using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class AppPushNotificationCampaignModel : BaseNopEntityModel, ILocalizedModel<AppPushNotificationCampaignLocalizedModel>
    {
        public AppPushNotificationCampaignModel()
        {
            AvailableActionTypes = new List<SelectListItem>();
            AvailableSmartGroups = new List<SelectListItem>();
            AvailableStores = new List<SelectListItem>();
            AvailableCustomerRoles = new List<SelectListItem>();
            AvailableDeviceTypes = new List<SelectListItem>();
            CustomerRoles = new List<int>();
            DeviceTypes = new List<int>();
            CopyPushNotificationCampaignModel = new CopyPushNotificationCampaignModel();
            Locales = new List<AppPushNotificationCampaignLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Body")]
        public string Body { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ImageId")]
        public int ImageId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.AddedToQueueOn")]
        public DateTime? AddedToQueueOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.AllowedTokens")]
        public string AllowedTokens { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.SendingWillStartOn")]
        [UIHint("DateTime")]
        public DateTime SendingWillStartOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.CustomerRoles")]
        public IList<int> CustomerRoles { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.DeviceTypes")]
        public IList<int> DeviceTypes { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ActionType")]
        public int ActionTypeId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.ActionValue")]
        public string ActionValue { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.LimitedToStoreId")]
        public int LimitedToStoreId { get; set; }

        public IList<SelectListItem> AvailableActionTypes { get; set; }
        public IList<SelectListItem> AvailableSmartGroups { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public IList<SelectListItem> AvailableCustomerRoles { get; set; }
        public IList<SelectListItem> AvailableDeviceTypes { get; set; }

        public CopyPushNotificationCampaignModel CopyPushNotificationCampaignModel { get; set; }

        public IList<AppPushNotificationCampaignLocalizedModel> Locales { get; set; }
    }

    public partial class AppPushNotificationCampaignLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Fields.Body")]
        public string Body { get; set; }
    }
}
