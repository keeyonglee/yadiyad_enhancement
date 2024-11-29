using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public class WebApiModelFactory : IWebApiModelFactory
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IApiStringResourceService _apiStringResourceService;
        private readonly IWorkContext _workContext;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;

        #endregion

        #region Ctor

        public WebApiModelFactory(ISettingService settingService,
            IStoreContext storeContext,
            IApiStringResourceService apiStringResourceService,
            IWorkContext workContext,
            IBaseAdminModelFactory baseAdminModelFactory)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _apiStringResourceService = apiStringResourceService;
            _workContext = workContext;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Methods

        public ConfigurationModel PrepareConfigurationModel()
        {
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var webApiSettings = _settingService.LoadSetting<WebApiSettings>(storeScope);

            var model = webApiSettings.ToSettingsModel<ConfigurationModel>();
            model.ActiveStoreScopeConfiguration = storeScope;

            model.SearchLanguageId = _workContext.WorkingLanguage.Id;
            model.LocaleResourceSearchModel.LanguageId = _workContext.WorkingLanguage.Id;
            _baseAdminModelFactory.PrepareLanguages(model.AvailableLanguages, false);

            model.AvailableBarcodeScanKeys = BarcodeScanKeyType.Sku.ToSelectList().ToList();

            if (storeScope == 0)
                return model;

            model.CheckIat_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.CheckIat, storeScope);
            model.DefaultCategoryIcon_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.DefaultCategoryIcon, storeScope);
            model.IOSProductPriceTextSize_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.IOSProductPriceTextSize, storeScope);
            model.IonicProductPriceTextSize_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.IonicProductPriceTextSize, storeScope);
            model.AndroidProductPriceTextSize_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.AndroidProductPriceTextSize, storeScope);
            model.ShowHomepageSlider_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowHomepageSlider, storeScope);
            model.SliderAutoPlay_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.SliderAutoPlay, storeScope);
            model.SliderAutoPlayTimeout_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.SliderAutoPlayTimeout, storeScope);
            model.EnableCORS_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.EnableCORS, storeScope);
            model.EnableJwtSecurity_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.EnableJwtSecurity, storeScope);
            model.MaximumNumberOfHomePageSliders_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.MaximumNumberOfHomePageSliders, storeScope);
            model.NumberOfHomepageCategoryProducts_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.NumberOfHomepageCategoryProducts, storeScope);
            model.NumberOfManufacturers_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.NumberOfManufacturers, storeScope);
            model.ShowBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowBestsellersOnHomepage, storeScope);
            model.NumberOfBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.NumberOfBestsellersOnHomepage, storeScope);
            model.ShowFeaturedProducts_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowFeaturedProducts, storeScope);
            model.ShowHomepageCategoryProducts_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowHomepageCategoryProducts, storeScope);
            model.ShowManufacturers_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowManufacturers, storeScope);
            model.ShowSubCategoryProducts_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowSubCategoryProducts, storeScope);
            model.TokenKey_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.TokenKey, storeScope);
            model.TokenSecondsValid_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.TokenSecondsValid, storeScope);
            model.TokenSecret_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.TokenSecret, storeScope);
            model.AndroidVersion_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.AndroidVersion, storeScope);
            model.AndriodForceUpdate_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.AndriodForceUpdate, storeScope);
            model.PlayStoreUrl_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.PlayStoreUrl, storeScope);
            model.IOSVersion_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.IOSVersion, storeScope);
            model.IOSForceUpdate_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.IOSForceUpdate, storeScope);
            model.AppStoreUrl_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.AppStoreUrl, storeScope);
            model.LogoId_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.LogoId, storeScope);
            model.LogoSize_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.LogoSize, storeScope);
            model.ShowChangeBaseUrlPanel_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ShowChangeBaseUrlPanel, storeScope);
            model.PrimaryThemeColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.PrimaryThemeColor, storeScope);
            model.BottomBarActiveColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.BottomBarActiveColor, storeScope);
            model.BottomBarBackgroundColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.BottomBarBackgroundColor, storeScope);
            model.BottomBarInactiveColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.BottomBarInactiveColor, storeScope);
            model.TopBarBackgroundColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.TopBarBackgroundColor, storeScope);
            model.TopBarTextColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.TopBarTextColor, storeScope);
            model.GradientStartingColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.GradientStartingColor, storeScope);
            model.GradientMiddleColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.GradientMiddleColor, storeScope);
            model.GradientEndingColor_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.GradientEndingColor, storeScope);
            model.GradientEnabled_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.GradientEnabled, storeScope);
            model.ProductBarcodeScanKeyId_OverrideForStore = _settingService.SettingExists(webApiSettings, x => x.ProductBarcodeScanKeyId, storeScope);

            return model;
        }

        public LocaleResourceListModel PrepareLocaleResourceListModel(LocaleResourceSearchModel searchModel, Language language)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (language == null)
                throw new ArgumentNullException(nameof(language));

            //get locale resources
            var localeResources = _apiStringResourceService.GetAllResourceValues(language.Id)
                .OrderBy(localeResource => localeResource.Key).AsQueryable();

            //filter locale resources
            //TODO: move filter to language service
            if (!string.IsNullOrEmpty(searchModel.SearchResourceName))
                localeResources = localeResources.Where(l => l.Key.ToLowerInvariant().Contains(searchModel.SearchResourceName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(searchModel.SearchResourceValue))
                localeResources = localeResources.Where(l => l.Value.Value.ToLowerInvariant().Contains(searchModel.SearchResourceValue.ToLowerInvariant()));

            var pagedLocaleResources = new PagedList<KeyValuePair<string, KeyValuePair<int, string>>>(localeResources,
                searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new LocaleResourceListModel().PrepareToGrid(searchModel, pagedLocaleResources, () =>
            {
                //fill in model values from the entity
                return pagedLocaleResources.Select(localeResource => new LocaleResourceModel
                {
                    LanguageId = language.Id,
                    Id = localeResource.Value.Key,
                    ResourceName = localeResource.Key,
                    ResourceValue = localeResource.Value.Value
                });
            });

            return model;
        }

        #endregion
    }
}
