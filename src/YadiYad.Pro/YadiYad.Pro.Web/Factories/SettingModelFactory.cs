using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Services.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;
using YadiYad.Pro.Web.Models.Setting;

namespace YadiYad.Pro.Web.FactoriesPro
{
    public class SettingModelFactory
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IAddressService _addressService;
        private readonly ICurrencyService _currencyService;
        private readonly INopDataProvider _dataProvider;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IFulltextService _fulltextService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IThemeProvider _themeProvider;
        private readonly IWorkContext _workContext;
        private readonly BaseAdminModelFactory _baseAdminModelFactory;

        #endregion

        #region Ctor

        public SettingModelFactory(CurrencySettings currencySettings,
            IAddressService addressService,
            ICurrencyService currencyService,
            INopDataProvider dataProvider,
            IDateTimeHelper dateTimeHelper,
            IFulltextService fulltextService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IThemeProvider themeProvider,
            IWorkContext workContext,
            BaseAdminModelFactory baseAdminModelFactory)
        {
            _currencySettings = currencySettings;
            _addressService = addressService;
            _currencyService = currencyService;
            _dataProvider = dataProvider;
            _dateTimeHelper = dateTimeHelper;
            _fulltextService = fulltextService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _themeProvider = themeProvider;
            _workContext = workContext;
            _baseAdminModelFactory = baseAdminModelFactory;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare address model
        /// </summary>
        /// <param name="model">Address model</param>
        /// <param name="address">Address</param>
        protected virtual void PrepareAddressModel(AddressModel model, Address address)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //set some of address fields as enabled and required
            model.CountryEnabled = true;
            model.StateProvinceEnabled = true;
            model.CountyEnabled = true;
            model.CityEnabled = true;
            model.StreetAddressEnabled = true;
            model.ZipPostalCodeEnabled = true;
            model.ZipPostalCodeRequired = true;

            //prepare available countries
            _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);

            //prepare available states
            _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId);
        }

        /// <summary>
        /// Prepare store theme models
        /// </summary>
        /// <param name="models">List of store theme models</param>
        protected virtual void PrepareStoreThemeModels(IList<StoreInformationSettingsModel.ThemeModel> models)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //load settings for a chosen store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeId);

            //get available themes
            var availableThemes = _themeProvider.GetThemes();
            foreach (var theme in availableThemes)
            {
                models.Add(new StoreInformationSettingsModel.ThemeModel
                {
                    FriendlyName = theme.FriendlyName,
                    SystemName = theme.SystemName,
                    PreviewImageUrl = theme.PreviewImageUrl,
                    PreviewText = theme.PreviewText,
                    SupportRtl = theme.SupportRtl,
                    Selected = theme.SystemName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                });
            }
        }

        /// <summary>
        /// Prepare sort option search model
        /// </summary>
        /// <param name="searchModel">Sort option search model</param>
        /// <returns>Sort option search model</returns>
        protected virtual SortOptionSearchModel PrepareSortOptionSearchModel(SortOptionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }



        #endregion

        #region Methods

        /// <summary>
        /// Prepare setting mode model
        /// </summary>
        /// <param name="modeName">Mode name</param>
        /// <returns>Setting mode model</returns>
        public virtual SettingModeModel PrepareSettingModeModel(string modeName)
        {
            var model = new SettingModeModel
            {
                ModeName = modeName,
                Enabled = _genericAttributeService.GetAttribute<bool>(_workContext.CurrentCustomer, modeName)
            };

            return model;
        }

        #endregion
    }
}
