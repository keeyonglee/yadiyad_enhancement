using System.ComponentModel;
using Newtonsoft.Json;

namespace Nop.Plugin.NopStation.AppPushNotification.Services.Models
{
    public class NotificationBaseModel
    {
        [JsonProperty("to")]
        public string SubscriptionId { get; set; }

        [JsonIgnore]
        public NotificationPriority FcmPriority { get; set; } = NotificationPriority.Normal;
    }

    public enum NotificationPriority
    {
        [Description("normal")]
        Normal = 5,
        [Description("high")]
        High = 10
    }
}
