using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Caching;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Infrastructure.Cache;

namespace YadiYad.Pro.Web.FactoriesPro
{
    public class BaseAdminModelFactory 
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IPluginService _pluginService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreService _storeService;
        private readonly ITaxCategoryService _taxCategoryService;
        private readonly ITopicTemplateService _topicTemplateService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public BaseAdminModelFactory(ICacheKeyService cacheKeyService,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IEmailAccountService emailAccountService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPluginService pluginService,
            IProductTemplateService productTemplateService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            ITopicTemplateService topicTemplateService,
            IVendorService vendorService)
        {
            _cacheKeyService = cacheKeyService;
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _emailAccountService = emailAccountService;
            _languageService = languageService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _manufacturerTemplateService = manufacturerTemplateService;
            _pluginService = pluginService;
            _productTemplateService = productTemplateService;
            _shippingService = shippingService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _storeService = storeService;
            _taxCategoryService = taxCategoryService;
            _topicTemplateService = topicTemplateService;
            _vendorService = vendorService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare default item
        /// </summary>
        /// <param name="items">Available items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use "All" text</param>
        protected virtual void PrepareDefaultItem(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //at now we use "0" as the default value
            const string value = "0";

            //prepare item text
            defaultItemText ??= _localizationService.GetResource("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = value });
        }

        /// <summary>
        /// Get category list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Category list</returns>
        protected virtual List<SelectListItem> GetCategoryList(bool showHidden = true)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoriesListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var categories = _categoryService.GetAllCategories(showHidden: showHidden);
                return categories.Select(c => new SelectListItem
                {
                    Text = _categoryService.GetFormattedBreadCrumb(c, categories),
                    Value = c.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get manufacturer list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Manufacturer list</returns>
        protected virtual List<SelectListItem> GetManufacturerList(bool showHidden = true)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ManufacturersListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var manufacturers = _manufacturerService.GetAllManufacturers(showHidden: showHidden);
                return manufacturers.Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        /// <summary>
        /// Get vendor list
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Vendor list</returns>
        protected virtual List<SelectListItem> GetVendorList(bool showHidden = true)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.VendorsListKey, showHidden);
            var listItems = _staticCacheManager.Get(cacheKey, () =>
            {
                var vendors = _vendorService.GetAllVendors(showHidden: showHidden);
                return vendors.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString()
                });
            });

            var result = new List<SelectListItem>();
            //clone the list to ensure that "selected" property is not set
            foreach (var item in listItems)
            {
                result.Add(new SelectListItem
                {
                    Text = item.Text,
                    Value = item.Value
                });
            }

            return result;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Prepare available categories
        /// </summary>
        /// <param name="items">Category items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCategories(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available categories
            var availableCategoryItems = GetCategoryList();
            foreach (var categoryItem in availableCategoryItems)
            {
                items.Add(categoryItem);
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available category templates
        /// </summary>
        /// <param name="items">Category template items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCategoryTemplates(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available category templates
            var availableTemplates = _categoryTemplateService.GetAllCategoryTemplates();
            foreach (var template in availableTemplates)
            {
                items.Add(new SelectListItem { Value = template.Id.ToString(), Text = template.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }

        /// <summary>
        /// Prepare available countries
        /// </summary>
        /// <param name="items">Country items</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareCountries(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //prepare available countries
            var availableCountries = _countryService.GetAllCountries(showHidden: true);
            foreach (var country in availableCountries)
            {
                items.Add(new SelectListItem { Value = country.Id.ToString(), Text = country.Name });
            }

            //insert special item for the default value
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.SelectCountry"));
        }

        /// <summary>
        /// Prepare available states and provinces
        /// </summary>
        /// <param name="items">State and province items</param>
        /// <param name="countryId">Country identifier; pass null to don't load states and provinces</param>
        /// <param name="withSpecialDefaultItem">Whether to insert the first special item for the default value</param>
        /// <param name="defaultItemText">Default item text; pass null to use default value of the default item text</param>
        public virtual void PrepareStatesAndProvinces(IList<SelectListItem> items, int? countryId,
            bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            if (countryId.HasValue)
            {
                //prepare available states and provinces of the country
                var availableStates = _stateProvinceService.GetStateProvincesByCountryId(countryId.Value, showHidden: true);
                foreach (var state in availableStates)
                {
                    items.Add(new SelectListItem { Value = state.Id.ToString(), Text = state.Name });
                }

                //insert special item for the default value
                if (items.Any())
                    PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.SelectState"));
            }

            //insert special item for the default value
            if (!items.Any())
                PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText ?? _localizationService.GetResource("Admin.Address.Other"));
        }

        #endregion
    }
}
