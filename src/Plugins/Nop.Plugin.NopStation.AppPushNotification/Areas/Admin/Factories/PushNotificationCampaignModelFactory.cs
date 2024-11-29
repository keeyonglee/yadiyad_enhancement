using System;
using System.Linq;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Services;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public class AppPushNotificationCampaignModelFactory : IPushNotificationCampaignModelFactory
    {
        #region Fields

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IPushNotificationCampaignService _pushNotificationCampaignService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IPushNotificationTokenProvider _pushNotificationTokenProvider;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;

        #endregion

        #region Ctor

        public AppPushNotificationCampaignModelFactory(IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IPushNotificationCampaignService pushNotificationCampaignService,
            ILocalizedModelFactory localizedModelFactory,
            IPushNotificationTokenProvider pushNotificationTokenProvider,
            IBaseAdminModelFactory baseAdminModelFactory)
        {
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _pushNotificationCampaignService = pushNotificationCampaignService;
            _localizedModelFactory = localizedModelFactory;
            _pushNotificationTokenProvider = pushNotificationTokenProvider;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Methods

        public virtual AppPushNotificationCampaignSearchModel PreparePushNotificationCampaignSearchModel(AppPushNotificationCampaignSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual AppPushNotificationCampaignListModel PreparePushNotificationCampaignListModel(AppPushNotificationCampaignSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var searchFrom = !searchModel.SearchSendStartFromDate.HasValue ? (DateTime?)null :
                _dateTimeHelper.ConvertToUtcTime(searchModel.SearchSendStartFromDate.Value);
            var searchTo = !searchModel.SearchSendStartToDate.HasValue ? (DateTime?)null :
                _dateTimeHelper.ConvertToUtcTime(searchModel.SearchSendStartToDate.Value);

            //get AppPushNotificationCampaigns
            var pushNotificationCampaigns = _pushNotificationCampaignService.GetAllPushNotificationCampaigns(
                keyword: searchModel.SearchKeyword,
                searchFrom: searchFrom,
                searchTo: searchTo,
                pageIndex: searchModel.Page - 1, 
                pageSize: searchModel.PageSize);

            //prepare list model
            var model = new AppPushNotificationCampaignListModel().PrepareToGrid(searchModel, pushNotificationCampaigns, () =>
            {
                return pushNotificationCampaigns.Select(pushNotificationCampaign =>
                {
                    return PreparePushNotificationCampaignModel(null, pushNotificationCampaign, true);
                });
            });

            return model;
        }

        public virtual AppPushNotificationCampaignModel PreparePushNotificationCampaignModel(AppPushNotificationCampaignModel model,
            AppPushNotificationCampaign pushNotificationCampaign, bool excludeProperties = false)
        {
            Action<AppPushNotificationCampaignLocalizedModel, int> localizedModelConfiguration = null;

            if (pushNotificationCampaign != null)
            {
                if (model == null)
                {
                    model = pushNotificationCampaign.ToModel<AppPushNotificationCampaignModel>();
                    model.SendingWillStartOn = _dateTimeHelper.ConvertToUserTime(pushNotificationCampaign.SendingWillStartOnUtc, DateTimeKind.Utc);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(pushNotificationCampaign.CreatedOnUtc, DateTimeKind.Utc);

                    if (!string.IsNullOrWhiteSpace(pushNotificationCampaign.CustomerRoles))
                        model.CustomerRoles = pushNotificationCampaign.CustomerRoles
                            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                    if (!string.IsNullOrWhiteSpace(pushNotificationCampaign.DeviceTypes))
                        model.DeviceTypes = pushNotificationCampaign.DeviceTypes
                            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

                    if (pushNotificationCampaign.AddedToQueueOnUtc.HasValue)
                        model.AddedToQueueOn = _dateTimeHelper.ConvertToUserTime(pushNotificationCampaign.AddedToQueueOnUtc.Value, DateTimeKind.Utc);

                    model.CopyPushNotificationCampaignModel.Id = pushNotificationCampaign.Id;
                    model.CopyPushNotificationCampaignModel.Name = $"{pushNotificationCampaign.Name} - Copy";

                    if (!excludeProperties)
                    {
                        localizedModelConfiguration = (locale, languageId) =>
                        {
                            locale.Title = _localizationService.GetLocalized(pushNotificationCampaign, entity => entity.Title, languageId, false, false);
                            locale.Body = _localizationService.GetLocalized(pushNotificationCampaign, entity => entity.Body, languageId, false, false);
                        };
                    }
                }
            }

            if (!excludeProperties)
            {
                var allowedTokens = string.Join(", ", _pushNotificationTokenProvider.GetListOfAllowedTokens(new[] { AppPushNotificationTokenGroupNames.StoreTokens, AppPushNotificationTokenGroupNames.CustomerTokens }));
                model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}" +
                    $"{_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";

                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                _baseAdminModelFactory.PrepareStores(model.AvailableStores);
                _baseAdminModelFactory.PrepareCustomerRoles(model.AvailableCustomerRoles, false);
                model.AvailableDeviceTypes = DeviceType.Android.ToSelectList().ToList();
                model.AvailableActionTypes = NotificationActionType.None.ToSelectList().ToList();
            }

            return model;
        }

        #endregion
    }
}
