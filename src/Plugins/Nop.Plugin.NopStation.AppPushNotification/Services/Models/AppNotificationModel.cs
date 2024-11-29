using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;

namespace Nop.Plugin.NopStation.AppPushNotification.Services.Models
{
    public class AppNotificationModel 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int ItemType { get; set; }
        public string ItemId { get; set; }
        public string BigPictureUrl { get; set; }
        public int ApptypeId { get; set; }
    }

    public class CustomerNotificationModel: BaseNopModel
    {
        public CustomerNotificationModel()
        {
            Notifications = new List<AppNotificationModel>();
        }
        public PagerModel PagerModel { get; set; }
        public IList<AppNotificationModel> Notifications { get; set; }
    }
}