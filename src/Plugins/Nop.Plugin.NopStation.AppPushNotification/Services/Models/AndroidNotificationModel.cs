using Newtonsoft.Json;

namespace Nop.Plugin.NopStation.AppPushNotification.Services.Models
{
    public class AndroidNotificationModel: NotificationBaseModel
    {
        public AndroidNotificationModel()
        {
            Data = new DataModel();
        }

        [JsonProperty("data")]
        public DataModel Data { get; set; }
        
        [JsonProperty("notification")]
        public NotificationModel Notification { get; set; }

        public partial class DataModel
        {
            [JsonProperty("click_action")]
            public string ClickAction { get; set; } = "FLUTTER_NOTIFICATION_CLICK";
            
            [JsonProperty("itemType")]
            public int ActionType { get; set; }

            [JsonProperty("itemId")]
            public string ActionValue { get; set; }

            [JsonProperty("bigPicture")]
            public string BigPicture { get; set; }
        }

        public partial class NotificationModel
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("body")]
            public string Body { get; set; }
        }

        [JsonProperty("priority")]
        public string Priority => FcmPriority.GetDescription();
    }
}
