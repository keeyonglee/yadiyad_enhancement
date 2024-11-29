using Newtonsoft.Json;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Media;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public class AppPushNotificationSender : IPushNotificationSender
    {
        private readonly IPictureService _pictureService;
        private readonly AppPushNotificationSettings _appPushNotificationSettings;

        public AppPushNotificationSender(IPictureService pictureService,
            AppPushNotificationSettings appPushNotificationSettings)
        {
            _pictureService = pictureService;
            _appPushNotificationSettings = appPushNotificationSettings;
        }

        private NotificationBaseModel PrepareNotificationModel(DeviceType deviceType, string title,
            string body, string subscriptionId, int actionTypeId = 0, string actionValue = "", string imageUrl = "")
        {
            if (_appPushNotificationSettings.ApplicationTypeId == (int)ApplicationType.Native)
            {
                if (deviceType == DeviceType.IPhone)
                {
                    var model = new IosNotificationModel()
                    {
                        SubscriptionId = subscriptionId,
                        Data = new IosNotificationModel.DataModel()
                        {
                            ActionType = actionTypeId,
                            ActionValue = actionValue,
                            ImageUrl = imageUrl
                        },
                        Notification = new IosNotificationModel.NotificationModel()
                        {
                            Body = body,
                            Title = title,
                            MutableContent = true
                        }
                    };

                    return model;
                }
                else if (deviceType == DeviceType.Android)
                {
                    var model = new AndroidNotificationModel()
                    {
                        SubscriptionId = subscriptionId,
                        Data = new AndroidNotificationModel.DataModel()
                        {
                            ActionType = actionTypeId,
                            ActionValue = actionValue,
                            BigPicture = imageUrl,
                        },
                        Notification = new AndroidNotificationModel.NotificationModel
                        {
                            Body = body,
                            Title = title
                        }
                    };

                    return model;
                }
            }
            else
            {
                var model = new IonicNotificationModel()
                {
                    SubscriptionId = subscriptionId,
                    Data = new IonicNotificationModel.DataModel()
                    {
                        ActionType = actionTypeId,
                        ActionValue = actionValue,
                        ImageUrl = imageUrl,
                        NotificationForeground = "true",
                        Body = body,
                        Title = title
                    },
                    Notification = new IonicNotificationModel.NotificationModel()
                    {
                        Body = body,
                        Title = title,
                        Image = imageUrl
                    }
                };

                return model;
            }

            throw new NotImplementedException();
        }

        public void SendNotification(AppQueuedPushNotification notification)
        {
            SendNotification(notification.DeviceType, notification.Title, notification.Body,
                notification.SubscriptionId, notification.AppTypeId, notification.ActionTypeId, notification.ActionValue,
                notification.ImageUrl);
        }

        public void SendNotification(DeviceType deviceType, string title, string body, string subscriptionId, int appTypeId,
            int actionTypeId = 0, string actionValue = "", string imageUrl = "")
        {
            var fcmModel = PrepareNotificationModel(deviceType, title, body, subscriptionId, actionTypeId, actionValue, imageUrl);

            var data = JsonConvert.SerializeObject(fcmModel);

            var request = (HttpWebRequest)WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            request.ContentType = "application/json";

            var accessKey = appTypeId == (int)AppType.BuyerApp ? _appPushNotificationSettings.GoogleConsoleApiAccessKey : _appPushNotificationSettings.VendorAppApiAccessKey;
            request.Headers.Add("Authorization", "key=" + accessKey);

            var bytes = Encoding.UTF8.GetBytes(data);
            request.Method = "POST";
            request.ContentLength = bytes.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            var response = (HttpWebResponse)request.GetResponse();
        }
    }
}
