using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Services;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Factories
{
    public class AppPushNotificationTemplateModelFactory : IPushNotificationTemplateModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IPushNotificationTemplateService _pushNotificationTemplateService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IPushNotificationTokenProvider _pushNotificationTokenProvider;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;

        #endregion

        #region Ctor

        public AppPushNotificationTemplateModelFactory(ILocalizationService localizationService,
            IPushNotificationTemplateService pushNotificationTemplateService,
            ILocalizedModelFactory localizedModelFactory,
            IPushNotificationTokenProvider pushNotificationTokenProvider,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory)
        {
            _localizationService = localizationService;
            _pushNotificationTemplateService = pushNotificationTemplateService;
            _localizedModelFactory = localizedModelFactory;
            _pushNotificationTokenProvider = pushNotificationTokenProvider;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
        }

        #endregion

        #region Methods

        public virtual AppPushNotificationTemplateSearchModel PreparePushNotificationTemplateSearchModel(AppPushNotificationTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            searchModel.AvailableActiveTypes.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.Common.All"),
                Value = "0"
            });
            searchModel.AvailableActiveTypes.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId.Active"),
                Value = "1"
            });
            searchModel.AvailableActiveTypes.Add(new SelectListItem()
            {
                Text = _localizationService.GetResource("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId.Inactive"),
                Value = "2"
            });

            return searchModel;
        }

        public virtual AppPushNotificationTemplateListModel PreparePushNotificationTemplateListModel(AppPushNotificationTemplateSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            bool? active = null;
            if (searchModel.SearchActiveId == 1)
                active = true;
            if (searchModel.SearchActiveId == 2)
                active = false;

            //get AppPushNotificationTemplates
            var pushNotificationTemplates = _pushNotificationTemplateService.GetAllPushNotificationTemplates(searchModel.SearchKeyword,
                active, 0, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new AppPushNotificationTemplateListModel().PrepareToGrid(searchModel, pushNotificationTemplates, () =>
            {
                return pushNotificationTemplates.Select(pushNotificationTemplate =>
                {
                    //fill in model values from the entity
                    return PreparePushNotificationTemplateModel(null, pushNotificationTemplate, true);
                });
            });

            return model;
        }

        public virtual AppPushNotificationTemplateModel PreparePushNotificationTemplateModel(AppPushNotificationTemplateModel model, 
            AppPushNotificationTemplate pushNotificationTemplate, bool excludeProperties = false)
        {
            Action<AppPushNotificationTemplateLocalizedModel, int> localizedModelConfiguration = null;

            if (pushNotificationTemplate != null)
            {
                if (model == null)
                {
                    model = pushNotificationTemplate.ToModel<AppPushNotificationTemplateModel>();
                    model.Name = pushNotificationTemplate.Name;

                    if (!excludeProperties)
                    {
                        localizedModelConfiguration = (locale, languageId) =>
                        {
                            locale.Title = _localizationService.GetLocalized(pushNotificationTemplate, entity => entity.Title, languageId, false, false);
                            locale.Body = _localizationService.GetLocalized(pushNotificationTemplate, entity => entity.Body, languageId, false, false);
                        };
                    }
                }
            }

            if (!excludeProperties)
            {
                var allowedTokens = string.Join(", ", _pushNotificationTokenProvider.GetTokenGroups(pushNotificationTemplate));
                model.AllowedTokens = $"{allowedTokens}{Environment.NewLine}{Environment.NewLine}" +
                    $"{_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Tokens.ConditionalStatement")}{Environment.NewLine}";
                
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);
                _storeMappingSupportedModelFactory.PrepareModelStores(model, pushNotificationTemplate, excludeProperties);
                model.AvailableActionTypes = NotificationActionType.None.ToSelectList().ToList();
            }

            return model;
        }

        #endregion
    }
}
