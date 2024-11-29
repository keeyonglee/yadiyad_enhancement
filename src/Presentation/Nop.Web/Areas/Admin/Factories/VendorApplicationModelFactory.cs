using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.VendorApplications;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the vendor model factory implementation
    /// </summary>
    public partial class VendorApplicationModelFactory : IVendorApplicationModelFactory
    {
        #region Fields

        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAddressService _addressService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorService _vendorService;
        private readonly IVendorApplicationService _vendorApplicationService;
        private readonly ICategoryService _categoryService;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorApplicationModelFactory(IAddressAttributeModelFactory addressAttributeModelFactory,
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorService vendorService,
            IVendorApplicationService vendorApplicationService,
            ICategoryService categoryService,
            VendorSettings vendorSettings)
        {
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _addressService = addressService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _urlRecordService = urlRecordService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _vendorService = vendorService;
            _vendorApplicationService = vendorApplicationService;
            _categoryService = categoryService;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare vendor search model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>Vendor search model</returns>
        public virtual VendorApplicationSearchModel PrepareVendorApplicationSearchModel(VendorApplicationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged vendor list model
        /// </summary>
        /// <param name="searchModel">Vendor search model</param>
        /// <returns>Vendor list model</returns>
        public virtual VendorApplicationListModel PrepareVendorApplicationListModel(VendorApplicationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get vendors
            var vendors = _vendorApplicationService.GetAllVendorApplications(null, searchModel.SearchName,
                searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new VendorApplicationListModel().PrepareToGrid(searchModel, vendors, () =>
            {
                //fill in model values from the entity
                return vendors.Select(vendor =>
            {
                var vendorModel = vendor.ToModel<VendorApplicationModel>();
                vendorModel.Status = vendorModel.IsApproved == null ? "Pending" : vendorModel.IsApproved.Value == true ? "Approved" : "Rejected";
                vendorModel.BusinessNatureCategory = _categoryService.GetCategoryById(vendorModel.BusinessNatureCategoryId)?.Name;
                vendorModel.Category = _categoryService.GetCategoryById(vendorModel.CategoryId)?.Name;
                return vendorModel;
            });
            });

            return model;
        }

        /// <summary>
        /// Prepare vendor model
        /// </summary>
        /// <param name="model">Vendor model</param>
        /// <param name="vendor">Vendor</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Vendor model</returns>
        public virtual VendorApplicationModel PrepareVendorApplicationModel(VendorApplicationModel model, VendorApplication vendor, bool excludeProperties = false)
        {
            if (vendor != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = vendor.ToModel<VendorApplicationModel>();
                }
                if(model.BusinessNatureCategoryId == _vendorSettings.ShuqEatsCategoryId)
                {
                    model.EatCategoryId = model.CategoryId;
                }
                if (model.BusinessNatureCategoryId == _vendorSettings.ShuqMartCategoryId)
                {
                    model.MartCategoryId = model.CategoryId;
                }
                _baseAdminModelFactory.PrepareEatCategories(model.AvailableEatCategories);
                _baseAdminModelFactory.PrepareMartCategories(model.AvailableMartCategories);
                model.SampleProductPictureIds = _vendorApplicationService.GetSampleProductPicturesByVendorApplicationId(model.Id);
            }

            return model;
        }

        #endregion
    }
}