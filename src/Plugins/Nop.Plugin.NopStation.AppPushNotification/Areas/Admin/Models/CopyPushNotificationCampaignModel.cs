using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class CopyPushNotificationCampaignModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationCampaigns.Copy.SendingWillStartOn")]
        [UIHint("DateTime")]
        public DateTime SendingWillStartOnUtc { get; set; }
    }
}
