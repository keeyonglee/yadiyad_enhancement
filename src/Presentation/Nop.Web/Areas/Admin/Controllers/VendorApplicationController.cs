using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.VendorApplications;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class VendorApplicationController : BaseAdminController
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorService _vendorService;
        private readonly IVendorApplicationService _vendorApplicationService;
        private readonly IVendorApplicationModelFactory _vendorApplicationModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorApplicationController(IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorService vendorService,
            IVendorApplicationService vendorApplicationService,
            IVendorApplicationModelFactory vendorApplicationModelFactory,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            VendorSettings vendorSettings)
        {
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _vendorService = vendorService;
            _vendorApplicationService = vendorApplicationService;
            _vendorApplicationModelFactory = vendorApplicationModelFactory;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _vendorSettings = vendorSettings;

        }

        #endregion

        #region Utilities

        protected virtual void UpdatePictureSeoNames(Vendor vendor)
        {
            var picture = _pictureService.GetPictureById(vendor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(vendor.Name));
        }

        #endregion

        #region Vendors

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorApplications))
                return AccessDeniedView();

            //prepare model
            var model = _vendorApplicationModelFactory.PrepareVendorApplicationSearchModel(new VendorApplicationSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(VendorApplicationSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorApplications))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _vendorApplicationModelFactory.PrepareVendorApplicationListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorApplications))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = _vendorApplicationService.GetVendorApplicationById(id);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _vendorApplicationModelFactory.PrepareVendorApplicationModel(null, vendor);
            model.AvailableEatCategories.Insert(0, new SelectListItem { Text = "", Value = "0" });
            model.AvailableMartCategories.Insert(0, new SelectListItem { Text = "", Value = "0" });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(VendorApplicationModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorApplications))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendorApplication = _vendorApplicationService.GetVendorApplicationById(model.Id);
            if (vendorApplication == null || vendorApplication.Deleted)
                return RedirectToAction("List");

            model.CustomerId = vendorApplication.CustomerId;
            model.CreatedOnUtc = vendorApplication.CreatedOnUtc;
            model.CreatedById = vendorApplication.CreatedById;
            var vendorAttributesXml = "";

            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();

            var attribute = vendorAttributes.Where(x => x.Name == NopVendorDefaults.VendorName).FirstOrDefault();
            vendorAttributesXml = _vendorAttributeParser.AddVendorAttribute(vendorAttributesXml, attribute, model.StoreName);

            var attribute2 = vendorAttributes.Where(x => x.Name == NopVendorDefaults.VendorBusinessNature).FirstOrDefault();

            var vendorNatureAttribute = (model.BusinessNatureCategoryId == _vendorSettings.ShuqEatsCategoryId 
                ? _vendorSettings.ShuqEatsBusinessNatureVendorAttributeId 
                : _vendorSettings.ShuqMartBusinessNatureVendorAttributeId);

            vendorAttributesXml = _vendorAttributeParser.AddVendorAttribute(vendorAttributesXml, attribute2, vendorNatureAttribute.ToString());

            var validateVendorAttributes = vendorAttributes.Where(x =>
                x.Name.Equals(NopVendorDefaults.VendorName, StringComparison.InvariantCultureIgnoreCase)
                || x.Name.Equals(NopVendorDefaults.VendorBusinessNature, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            _vendorAttributeParser.GetAttributeWarnings(vendorAttributesXml, validateVendorAttributes).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                if (model.BusinessNatureCategoryId == _vendorSettings.ShuqEatsCategoryId)
                {
                    model.CategoryId = model.EatCategoryId;
                }
                if (model.BusinessNatureCategoryId == _vendorSettings.ShuqMartCategoryId)
                {
                    model.CategoryId = model.MartCategoryId;
                }

                var customer = _customerService.GetCustomerById(vendorApplication.CustomerId);

                if(customer == null)
                {
                    throw new NopException("No customer found.");
                }

                var vendor = new Vendor
                {
                    Name = model.StoreName,
                    Email = customer.Email,
                    PageSize = _vendorSettings.DefaultVendorPageSize,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions,
                    Active = true,
                    Online = true,
                    CategoryId = model.BusinessNatureCategoryId
                };
                if (vendorApplication.IsApproved == null && model.IsApproved == true)
                {
                    //create vendor
                    _vendorService.InsertVendor(vendor);

                    var seName = _urlRecordService.ValidateSeName(vendor, "", vendor.Name, true);
                    _urlRecordService.SaveSlug(vendor, seName, 0);

                    customer.VendorId = vendor.Id;
                    _customerService.UpdateCustomer(customer);

                    _genericAttributeService.SaveAttribute(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                    //add new vendor role to customer 
                    //get vendor role.
                    var vendorRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.VendorRoleName);

                    if(vendorRole == null)
                    {
                        throw new NopException("Vendor role not found.");
                    }

                    _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = vendorRole.Id });

                    //send approve messsage
                    _workflowMessageService.SendNewVendorAccountApproveStoreOwnerNotification(customer, vendor, _localizationSettings.DefaultAdminLanguageId);
                }
                if (vendorApplication.IsApproved == null && model.IsApproved == false)
                {
                    //send reject messsage
                    _workflowMessageService.SendNewVendorAccountRejectStoreOwnerNotification(customer, vendor, model.AdminComment, _localizationSettings.DefaultAdminLanguageId);

                }
                vendorApplication = model.ToEntity(vendorApplication);
                vendorApplication.UpdatedById = customer.Id;
                vendorApplication.UpdatedOnUtc = DateTime.UtcNow;
                _vendorApplicationService.UpdateVendorApplication(vendorApplication);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = vendorApplication.Id });
            }

            //prepare model
            model = _vendorApplicationModelFactory.PrepareVendorApplicationModel(model, vendorApplication, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

    }
}