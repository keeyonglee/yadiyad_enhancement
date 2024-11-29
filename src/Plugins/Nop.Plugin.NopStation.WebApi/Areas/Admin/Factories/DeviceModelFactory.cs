using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public class DeviceModelFactory : IDeviceModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IApiDeviceService _deviceService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public DeviceModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IApiDeviceService deviceService,
            ICustomerService customerService,
            IStoreService storeService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _deviceService = deviceService;
            _customerService = customerService;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected void PrepareDeviceTypes(IList<SelectListItem> items, bool excludeDefaultItem = false)
        {
            var selectList = DeviceType.Android.ToSelectList(false);
            foreach (var item in selectList)
                items.Add(item);

            if (!excludeDefaultItem)
                items.Insert(0, new SelectListItem()
                {
                    Text = _localizationService.GetResource("Admin.Common.All"),
                    Value = "0"
                });
        }

        #endregion

        #region Methods

        public virtual DeviceSearchModel PrepareDeviceSearchModel(DeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            PrepareDeviceTypes(searchModel.AvailableDeviceTypes);
            searchModel.SelectedDeviceTypes.Add(0);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual DeviceListModel PrepareDeviceListModel(DeviceSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var selectedTypes = (searchModel.SelectedDeviceTypes?.Contains(0) ?? true) ? null : searchModel.SelectedDeviceTypes.ToList();

            //get devices
            var devices = _deviceService.SearchApiDevices(0, 0, selectedTypes, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new DeviceListModel().PrepareToGrid(searchModel, devices, () =>
            {
                return devices.Select(device =>
                {
                    return PrepareDeviceModel(null, device);
                });
            });

            return model;
        }

        public virtual DeviceModel PrepareDeviceModel(DeviceModel model, ApiDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(ApiSlider));

            if (model == null)
            {
                model = device.ToModel<DeviceModel>();
                model.DeviceTypeStr = _localizationService.GetLocalizedEnum(device.DeviceType);
            }

            var customer = _customerService.GetCustomerById(device.CustomerId);
            model.CustomerName = customer?.Email ?? _localizationService.GetResource("Admin.Customers.Guest");

            model.StoreName = _storeService.GetStoreById(model.StoreId)?.Name ?? 
                _localizationService.GetResource("Admin.NopStation.WebApi.Common.Unknown");
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(device.CreatedOnUtc, DateTimeKind.Utc);
            model.UpdatedOn = _dateTimeHelper.ConvertToUserTime(device.UpdatedOnUtc, DateTimeKind.Utc);

            return model;
        }

        #endregion
    }
}
