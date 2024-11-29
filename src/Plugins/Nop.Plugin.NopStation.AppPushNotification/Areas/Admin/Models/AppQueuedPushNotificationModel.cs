using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class AppQueuedPushNotificationModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Customer")]
        public int? CustomerId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Customer")]
        public string CustomerName { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Store")]
        public int StoreId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Store")]
        public string StoreName { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.Body")]
        public string Body { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ImageUrl")]
        public string ImageUrl { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DeviceType")]
        public string DeviceTypeStr { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DeviceType")]
        public int DeviceTypeId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentOn")]
        public DateTime? SentOn { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.DontSendBeforeDate")]
        [UIHint("DateTimeNullable")]
        public DateTime? DontSendBeforeDate { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SendImmediately")]
        public bool SendImmediately { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ErrorLog")]
        public string ErrorLog { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SentTries")]
        public int SentTries { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.AppDeviceId")]
        public int AppDeviceId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionType")]
        public int ActionTypeId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionType")]
        public string ActionTypeStr { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.ActionValue")]
        public string ActionValue { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Fields.SubscriptionId")]
        public string SubscriptionId { get; set; }
    }
}
