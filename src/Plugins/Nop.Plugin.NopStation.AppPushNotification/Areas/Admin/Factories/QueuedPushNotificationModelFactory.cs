using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using System;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System.Linq;
using Nop.Services.Customers;
using Nop.Services.Stores;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public class QueuedPushNotificationModelFactory : IQueuedPushNotificationModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IQueuedPushNotificationService _queuedPushNotificationService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public QueuedPushNotificationModelFactory(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IQueuedPushNotificationService queuedPushNotificationService,
            ICustomerService customerService,
            IStoreService storeService)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _queuedPushNotificationService = queuedPushNotificationService;
            _customerService = customerService;
            _storeService = storeService;
        }

        #endregion

        #region Methods

        public virtual AppQueuedPushNotificationSearchModel PrepareQueuedPushNotificationSearchModel(AppQueuedPushNotificationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual AppQueuedPushNotificationListModel PrepareQueuedPushNotificationListModel(AppQueuedPushNotificationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get queuedPushNotifications
            var queuedPushNotifications = _queuedPushNotificationService.GetAllQueuedPushNotifications(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AppQueuedPushNotificationListModel().PrepareToGrid(searchModel, queuedPushNotifications, () =>
            {
                return queuedPushNotifications.Select(queuedPushNotification =>
                {
                    return PrepareQueuedPushNotificationModel(null, queuedPushNotification, false);
                });
            });

            return model;
        }

        public virtual AppQueuedPushNotificationModel PrepareQueuedPushNotificationModel(AppQueuedPushNotificationModel model, 
            AppQueuedPushNotification queuedPushNotification, bool excludeProperties = false)
        {
            if (queuedPushNotification != null)
            {
                //fill in model values from the entity
                model = model ?? queuedPushNotification.ToModel<AppQueuedPushNotificationModel>();
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(queuedPushNotification.CreatedOnUtc, DateTimeKind.Utc);
                if(queuedPushNotification.SentOnUtc.HasValue)
                    model.SentOn = _dateTimeHelper.ConvertToUserTime(queuedPushNotification.SentOnUtc.Value, DateTimeKind.Utc);
                if (queuedPushNotification.DontSendBeforeDateUtc.HasValue)
                    model.DontSendBeforeDate = _dateTimeHelper.ConvertToUserTime(queuedPushNotification.DontSendBeforeDateUtc.Value, DateTimeKind.Utc);
                else 
                    model.SendImmediately = true;

                model.ActionTypeStr = _localizationService.GetLocalizedEnum(queuedPushNotification.ActionType);
                model.DeviceTypeStr = _localizationService.GetLocalizedEnum(queuedPushNotification.DeviceType);

                if (!string.IsNullOrWhiteSpace(model.Body))
                    model.Body = model.Body.Replace(Environment.NewLine, "<br />");

                var customer = _customerService.GetCustomerById(queuedPushNotification.CustomerId);
                model.CustomerName = customer?.Email ??
                    _localizationService.GetResource("Admin.NopStation.AppPushNotification.QueuedPushNotifications.Guest");

                var store = _storeService.GetStoreById(queuedPushNotification.StoreId);
                model.StoreName = store?.Name;
            }

            if (!excludeProperties)
            {

            }
            return model;
        }

        #endregion
    }
}
