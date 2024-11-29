using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class WebApiSliderController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISliderModelFactory _sliderModelFactory;
        private readonly IApiSliderService _sliderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IVendorService _vendorService;
        private readonly ITopicService _topicService;

        #endregion

        #region Ctor

        public WebApiSliderController(ILocalizationService localizationService,
            INotificationService notificationService,
            ISliderModelFactory sliderModelFactory,
            IApiSliderService sliderService,
            IDateTimeHelper dateTimeHelper,
            IPermissionService permissionService,
            ICategoryService categoryService,
            IProductService productService,
            IManufacturerService manufacturerService,
            IVendorService vendorService,
            ITopicService topicService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _sliderModelFactory = sliderModelFactory;
            _sliderService = sliderService;
            _dateTimeHelper = dateTimeHelper;
            _permissionService = permissionService;
            _categoryService = categoryService;
            _productService = productService;
            _manufacturerService = manufacturerService;
            _vendorService = vendorService;
            _topicService = topicService;
        }

        #endregion

        #region Utilities

        public void ValidateSlider(SliderModel model)
        {
            if (model.SliderTypeId == (int)SliderType.Category)
            {
                var category = _categoryService.GetCategoryById(model.EntityId);
                if (category == null || category.Deleted)
                    ModelState.AddModelError("EntityId", _localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.EntityId.InvalidCategory"));
            }
            else if (model.SliderTypeId == (int)SliderType.Manufacturer)
            {
                var manufacterer = _manufacturerService.GetManufacturerById(model.EntityId);
                if (manufacterer == null || manufacterer.Deleted)
                    ModelState.AddModelError("EntityId", _localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.EntityId.InvalidManufacturer"));
            }
            else if (model.SliderTypeId == (int)SliderType.Product)
            {
                var product = _productService.GetProductById(model.EntityId);
                if (product == null || product.Deleted)
                    ModelState.AddModelError("EntityId", _localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.EntityId.InvalidProduct"));
            }
            else if (model.SliderTypeId == (int)SliderType.Vendor)
            {
                var vendor = _vendorService.GetVendorById(model.EntityId);
                if (vendor == null || vendor.Deleted)
                    ModelState.AddModelError("EntityId", _localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.EntityId.InvalidVendor"));
            }
            else if (model.SliderTypeId == (int)SliderType.Topic)
            {
                var topic = _topicService.GetTopicById(model.EntityId);
                if (topic == null)
                    ModelState.AddModelError("EntityId", _localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Fields.EntityId.InvalidTopic"));
            }
        }

        #endregion

        #region Methods        

        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            var searchModel = _sliderModelFactory.PrepareSliderSearchModel(new SliderSearchModel());
            return View(searchModel);
        }

        public virtual IActionResult GetList(SliderSearchModel searchModel)
        {
            var model = _sliderModelFactory.PrepareSliderListModel(searchModel);
            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            var model = _sliderModelFactory.PrepareSliderModel(new SliderModel(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(SliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            ValidateSlider(model);

            if (ModelState.IsValid)
            {
                var slider = model.ToEntity<ApiSlider>();

                if (model.ActiveEndDate.HasValue)
                    slider.ActiveEndDateUtc = _dateTimeHelper.ConvertToUtcTime(model.ActiveEndDate.Value, TimeZoneInfo.Local);
                if (model.ActiveStartDate.HasValue)
                    slider.ActiveStartDateUtc = _dateTimeHelper.ConvertToUtcTime(model.ActiveStartDate.Value, TimeZoneInfo.Local);
                slider.CreatedOnUtc = DateTime.UtcNow;
                _sliderService.InsertApiSlider(slider);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Created"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = slider.Id });
            }

            model = _sliderModelFactory.PrepareSliderModel(model, null);

            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            var slider = _sliderService.GetApiSliderById(id);
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            var model = _sliderModelFactory.PrepareSliderModel(null, slider);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(SliderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            ValidateSlider(model);

            var slider = _sliderService.GetApiSliderById(model.Id);
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            if (ModelState.IsValid)
            {
                slider = model.ToEntity(slider);

                if (model.ActiveEndDate.HasValue)
                    slider.ActiveEndDateUtc = _dateTimeHelper.ConvertToUtcTime(model.ActiveEndDate.Value, TimeZoneInfo.Local);
                if (model.ActiveStartDate.HasValue)
                    slider.ActiveStartDateUtc = _dateTimeHelper.ConvertToUtcTime(model.ActiveStartDate.Value, TimeZoneInfo.Local);

                _sliderService.UpdateApiSlider(slider);
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = slider.Id });
            }

            model = _sliderModelFactory.PrepareSliderModel(model, slider);
            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            var slider = _sliderService.GetApiSliderById(id);
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            _sliderService.DeleteApiSlider(slider);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.Sliders.Deleted"));

            return RedirectToAction("List");
        }

        public IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageSlider))
                return AccessDeniedView();

            if (selectedIds != null)
                _sliderService.DeleteApiSliders(_sliderService.GetApiSliderByIds(selectedIds.ToArray()).ToList());

            return Json(new { Result = true });
        }

        #endregion
    }
}
