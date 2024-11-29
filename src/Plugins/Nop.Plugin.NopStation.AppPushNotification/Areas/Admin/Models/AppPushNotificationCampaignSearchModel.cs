using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class AppPushNotificationCampaignSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchKeyword")]
        public string SearchKeyword { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchSendStartFromDate")]
        public DateTime? SearchSendStartFromDate { get; set; }

        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.List.SearchSendStartToDate")]
        public DateTime? SearchSendStartToDate { get; set; }
    }
}
