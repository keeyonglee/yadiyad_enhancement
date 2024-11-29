using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
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
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using YadiYad.Pro.Web.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class VendorController : BaseAdminController
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
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly VendorSettings _vendorSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public VendorController(
            ILogger logger,
            IAddressService addressService,
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
            IVendorModelFactory vendorModelFactory,
            IVendorService vendorService,
            IWorkContext workContext,
            VendorSettings vendorSettings)
        {
            _logger = logger;
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
            _vendorModelFactory = vendorModelFactory;
            _vendorService = vendorService;
            _workContext = workContext;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        protected virtual void UpdatePictureSeoNames(Vendor vendor)
        {
            var picture = _pictureService.GetPictureById(vendor.PictureId);
            //if (picture != null)
                //_pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(vendor.Name));
        }

        protected virtual void UpdateLocales(Vendor vendor, VendorModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(vendor,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = _urlRecordService.ValidateSeName(vendor, localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(vendor, seName, localized.LanguageId);
            }
        }

        protected virtual string ParseVendorAttributesXML(IFormCollection form,
            List<Nop.Web.Areas.Admin.Models.Vendors.VendorModel.VendorAttributeModel> readonlyVendorAttributes = null)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();
            foreach (var attribute in vendorAttributes)
            {
                var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
                var readonlyVendorAttribute = readonlyVendorAttributes?.Where(x => x.Id == attribute.Id).FirstOrDefault();
                StringValues ctrlAttributes;
                ctrlAttributes = form[controlId];

                if (readonlyVendorAttribute != null)
                {
                    var strValues = readonlyVendorAttribute.Values
                        .Where(x => x.IsPreSelected == true)
                        .Select(x => x.Id.ToString())
                        .ToList();
                    ctrlAttributes = new StringValues(strValues.ToArray());
                }

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId];

                        if (readonlyVendorAttribute != null)
                        {
                            var strValues = readonlyVendorAttribute.Values
                                .Where(x => x.IsPreSelected == true)
                                .Select(x => x.Name)
                                .ToList();
                            cblAttributes = new StringValues(string.Join(',',strValues.ToArray()));
                        }
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        var attributeValues = _vendorAttributeService.GetVendorAttributeValues(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                            .Where(v => v.IsPreSelected)
                            .Select(v => v.Id)
                            .ToList())
                        {
                            attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                attribute, enteredText);
                        }

                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported vendor attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected virtual List<VendorModel.VendorAttributeModel> ParseVendorAttributes(IFormCollection form,
            List<VendorModel.VendorAttributeModel> readonlyVendorAttributes = null)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            List<VendorModel.VendorAttributeModel> vendorAttributeModels = new List<VendorModel.VendorAttributeModel>();
            var vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();
            foreach (var attribute in vendorAttributes)
            {
                var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
                var readonlyVendorAttribute = readonlyVendorAttributes?.Where(x => x.Id == attribute.Id).FirstOrDefault();
                StringValues ctrlAttributes;
                ctrlAttributes = form[controlId];

                var vendorAttributeModel = new VendorModel.VendorAttributeModel
                {
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    IsRequiredManageVendorsPermission = attribute.IsRequiredManageVendorsPermission,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (readonlyVendorAttribute != null)
                {
                    var strValues = readonlyVendorAttribute.Values
                        .Where(x => x.IsPreSelected == true)
                        .Select(x => x.Id.ToString())
                        .ToList();
                    ctrlAttributes = new StringValues(strValues.ToArray());
                }

                var attributeValues = _vendorAttributeService.GetVendorAttributeValues(attribute.Id);

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                            {
                                foreach (var attributeValue in attributeValues)
                                {
                                    var attributeValueModel = new VendorModel.VendorAttributeValueModel
                                    {
                                        Id = attributeValue.Id,
                                        Name = attributeValue.Name,
                                        IsPreSelected = attributeValue.Id == selectedAttributeId
                                    };
                                    vendorAttributeModel.Values.Add(attributeValueModel);
                                }
                            }
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId];

                        if (readonlyVendorAttribute != null)
                        {
                            var strValues = readonlyVendorAttribute.Values
                                .Where(x => x.IsPreSelected == true)
                                .Select(x => x.Name)
                                .ToList();
                            cblAttributes = new StringValues(string.Join(',', strValues.ToArray()));
                        }
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                {
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        var attributeValueModel = new VendorModel.VendorAttributeValueModel
                                        {
                                            Id = attributeValue.Id,
                                            Name = attributeValue.Name,
                                            IsPreSelected = attributeValue.Id == selectedAttributeId
                                        };
                                        vendorAttributeModel.Values.Add(attributeValueModel);
                                    }
                                }
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        //load read-only (already server-side selected) values
                        foreach (var attributeValue in attributeValues
                            .Where(v => v.IsPreSelected)
                            .ToList())
                        {
                            var attributeValueModel = new VendorModel.VendorAttributeValueModel
                            {
                                Id = attributeValue.Id,
                                Name = attributeValue.Name,
                                IsPreSelected = attributeValue.IsPreSelected
                            };
                            vendorAttributeModel.Values.Add(attributeValueModel);
                        }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            var attributeValueModel = new VendorModel.VendorAttributeValueModel
                            {
                                Name = enteredText,
                                IsPreSelected = true
                            };
                            vendorAttributeModel.Values.Add(attributeValueModel);
                        }

                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported vendor attributes
                    default:
                        break;
                }

                vendorAttributeModels.Add(vendorAttributeModel);
            }

            return vendorAttributeModels;
        }

        #endregion

        #region Vendors

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = _vendorModelFactory.PrepareVendorSearchModel(new VendorSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(VendorSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _vendorModelFactory.PrepareVendorListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //prepare model
            var model = _vendorModelFactory.PrepareVendorModel(new VendorModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Create(VendorModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //parse vendor attributes
            var vendorAttributesXml = ParseVendorAttributesXML(form);
            _vendorAttributeParser.GetAttributeWarnings(vendorAttributesXml).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));

            if (ModelState.IsValid)
            {
                var vendor = model.ToEntity<Vendor>();
                _vendorService.InsertVendor(vendor);

                //activity log
                _customerActivityService.InsertActivity("AddNewVendor",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewVendor"), vendor.Id), vendor);

                //search engine name
                model.SeName = _urlRecordService.ValidateSeName(vendor, model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);

                //address
                var address = model.Address.ToEntity<Address>();
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                _addressService.InsertAddress(address);
                vendor.AddressId = address.Id;
                _vendorService.UpdateVendor(vendor);

                //vendor attributes
                _genericAttributeService.SaveAttribute(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //locales
                UpdateLocales(vendor, model);

                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Vendors.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = vendor.Id });
            }

            //prepare model
            model = _vendorModelFactory.PrepareVendorModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ManageVendors),
            nameof(StandardPermissionProvider.ManageShop)
            )]
        public virtual IActionResult Edit(int? id)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.ManageShop))
            {
                if (id.HasValue == false)
                {
                    id = _workContext.CurrentVendor.Id;
                }
                else
                {
                    return AccessDeniedView();
                }
            } 
            else if (id.HasValue == false)
            {
                return AccessDeniedView();
            }

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(id.Value);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //prepare model
            var model = _vendorModelFactory.PrepareVendorModel(null, vendor);
            var businessNature = _vendorService.GetVendorAttributeValue(vendor, NopVendorDefaults.VendorBusinessNature).Name;
            if (businessNature == "Shuq Mart")
            {
                foreach (var item in model.VendorAttributes)
                {
                    if (item.Name != "Delivery Date" && item.Name != "Delivery Time Slot")
                    {
                        model.VendorAttributesModified.Add(item);
                    }
                }
            }
            else
            {
                model.VendorAttributesModified = model.VendorAttributes;
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ManageVendors),
            nameof(StandardPermissionProvider.ManageShop)
            )]
        public virtual IActionResult Edit(VendorModel model, bool continueEditing, IFormCollection form)
        {
            var canManageVendors = _permissionService.Authorize(StandardPermissionProvider.ManageVendors);

            if(!canManageVendors)
            {
                //a vendor should have access only to his vendors
                if (_workContext.CurrentVendor != null && !_workContext.IsAdmin)
                {
                    return AccessDeniedView();
                }
                else
                {
                    model.Id = _workContext.CurrentVendor.Id;
                }
            }

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(model.Id);
            if (vendor == null || vendor.Deleted)
                return RedirectToAction("List");

            //parse vendor attributes
            List<Nop.Web.Areas.Admin.Models.Vendors.VendorModel.VendorAttributeModel> readonlyVendorAttributes = null;
            if (!canManageVendors)
            {
                var vendorModel = _vendorModelFactory.PrepareVendorModel(null, vendor);

                readonlyVendorAttributes = vendorModel.VendorAttributes.Where(x => x.IsRequiredManageVendorsPermission == true).ToList();
            }

            var vendorAttributesChecking = _vendorAttributeService.GetAllVendorAttributes();
            var businessNature = _vendorService.GetVendorAttributeValue(vendor, NopVendorDefaults.VendorBusinessNature).Name;
            var vendorAttributesXml = ParseVendorAttributesXML(form, readonlyVendorAttributes);
            if (businessNature == "Shuq Mart")
            {
                var validateVendorAttributes = vendorAttributesChecking.Where(x =>
                x.Name.Equals(NopVendorDefaults.VendorName, StringComparison.InvariantCultureIgnoreCase)
                || x.Name.Equals(NopVendorDefaults.VendorBusinessNature, StringComparison.InvariantCultureIgnoreCase)
                || x.Name.Equals(NopVendorDefaults.VendorAboutUs, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

                _vendorAttributeParser.GetAttributeWarnings(vendorAttributesXml, validateVendorAttributes).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));
            }
            else
            {
                _vendorAttributeParser.GetAttributeWarnings(vendorAttributesXml).ToList()
                .ForEach(warning => ModelState.AddModelError(string.Empty, warning));
            }

            if (ModelState.IsValid)
            {
                var prevPictureId = vendor.PictureId;

                //update vendor category id according business nature attribute
                if(canManageVendors)
                {
                    var vendorAttributes = ParseVendorAttributes(form, readonlyVendorAttributes);

                    var businessNatureVendorAttribute = vendorAttributes
                        .Where(x => x.Name.Equals(NopVendorDefaults.VendorBusinessNature, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

                    if(businessNatureVendorAttribute == null)
                    {
                        throw new NopException("Missing business nature vendor attribute.");
                    }

                    var selectedBusinessNatureVendorAttribute = businessNatureVendorAttribute.Values?
                        .Where(x => x.IsPreSelected == true)
                        .FirstOrDefault();

                    if(selectedBusinessNatureVendorAttribute == null)
                    {
                        throw new NopException("Missing business nature vendor attribute value not selected.");
                    }

                    vendor.CategoryId = selectedBusinessNatureVendorAttribute.Id == _vendorSettings.ShuqEatsBusinessNatureVendorAttributeId
                        ? (int?)_vendorSettings.ShuqEatsCategoryId
                        : selectedBusinessNatureVendorAttribute.Id == _vendorSettings.ShuqMartBusinessNatureVendorAttributeId
                        ? (int?)_vendorSettings.ShuqMartCategoryId
                        : null;
                }

                vendor = model.ToEntity(vendor);
                if (model.Online)
                {
                    vendor.OfflineUntil = null;
                }
                _vendorService.UpdateVendor(vendor);

                //vendor attributes
                _genericAttributeService.SaveAttribute(vendor, NopVendorDefaults.VendorAttributes, vendorAttributesXml);

                //activity log
                _customerActivityService.InsertActivity("EditVendor",
                    string.Format(_localizationService.GetResource("ActivityLog.EditVendor"), vendor.Id), vendor);

                //search engine name
                if(string.IsNullOrWhiteSpace(model.SeName) == false)
                {
                    model.SeName = _urlRecordService.GetSeName(vendor);
                }

                model.SeName = _urlRecordService.ValidateSeName(vendor, model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);

                //address
                var address = _addressService.GetAddressById(vendor.AddressId);
                if (address == null)
                {
                    address = model.Address.ToEntity<Address>();
                    address.CreatedOnUtc = DateTime.UtcNow;

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    _addressService.InsertAddress(address);
                    vendor.AddressId = address.Id;
                    _vendorService.UpdateVendor(vendor);
                }
                else
                {
                    address = model.Address.ToEntity(address);

                    //some validation
                    if (address.CountryId == 0)
                        address.CountryId = null;
                    if (address.StateProvinceId == 0)
                        address.StateProvinceId = null;

                    _addressService.UpdateAddress(address);
                }

                //locales
                UpdateLocales(vendor, model);

                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }

                if (canManageVendors)
                {
                    //update picture seo file name
                    UpdatePictureSeoNames(vendor);
                }

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Vendors.Updated"));

                if (!continueEditing && canManageVendors)
                    return RedirectToAction("List");

                if (canManageVendors)
                {
                    return RedirectToAction("Edit", new { id = vendor.Id });
                }
                else
                {
                    return RedirectToAction("Edit");
                }
            }

            //prepare model
            model = _vendorModelFactory.PrepareVendorModel(model, vendor, true);

            if (businessNature == "Shuq Mart")
            {
                foreach (var item in model.VendorAttributes)
                {
                    if (item.Name != "Delivery Date" && item.Name != "Delivery Time Slot")
                    {
                        model.VendorAttributesModified.Add(item);
                    }
                }
            }
            else
            {
                model.VendorAttributesModified = model.VendorAttributes;
            }

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
                return RedirectToAction("List");

            //clear associated customer references
            var associatedCustomers = _customerService.GetAllCustomers(vendorId: vendor.Id);
            foreach (var customer in associatedCustomers)
            {
                customer.VendorId = 0;
                _customerService.UpdateCustomer(customer);
            }

            //delete a vendor
            _vendorService.DeleteVendor(vendor);

            //activity log
            _customerActivityService.InsertActivity("DeleteVendor",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteVendor"), vendor.Id), vendor);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Vendors.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Vendor notes

        [HttpPost]
        public virtual IActionResult VendorNotesSelect(VendorNoteSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedDataTablesJson();

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(searchModel.VendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            //prepare model
            var model = _vendorModelFactory.PrepareVendorNoteListModel(searchModel, vendor);

            return Json(model);
        }

        public virtual IActionResult VendorNoteAdd(int vendorId, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(message))
                return ErrorJson(_localizationService.GetResource("Admin.Vendors.VendorNotes.Fields.Note.Validation"));

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return ErrorJson("Vendor cannot be loaded");

            _vendorService.InsertVendorNote(new VendorNote
            {
                Note = message,
                CreatedOnUtc = DateTime.UtcNow,
                VendorId = vendor.Id
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult VendorNoteDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            //try to get a vendor note with the specified id
            var vendorNote = _vendorService.GetVendorNoteById(id)
                ?? throw new ArgumentException("No vendor note found with the specified id", nameof(id));

            _vendorService.DeleteVendorNote(vendorNote);

            return new NullJsonResult();
        }

        #endregion

        #region Vendor pictures

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ManageVendors),
            nameof(StandardPermissionProvider.ManageShop)
            )]
        public virtual IActionResult VendorPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, int vendorPictureType, int vendorId)
        {
            if (pictureId == 0)
                throw new ArgumentException();

            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(vendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            if (!Enum.IsDefined(typeof(VendorPictureType), vendorPictureType))
            {
                throw new ArgumentException("Invalid vendorPictureType");
            }

            //a vendor should have access only to his vendors
            if (_workContext.CurrentVendor != null && (!_workContext.IsAdmin || vendor.Id != _workContext.CurrentVendor.Id))
                return RedirectToAction("List");

            if (_vendorService.GetVendorPicturesByVendorId(vendorId).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");


            if (vendorPictureType == (int)VendorPictureType.Default)
            {
                var pictureList = _vendorModelFactory.PrepareVendorPictureListModel(new VendorPictureSearchModel
                {
                    VendorId = vendorId
                }, vendor);

                var defaultPicture = pictureList.Data
                    .Where(x => x.VendorPictureType == (int)VendorPictureType.Default)
                    .FirstOrDefault();

                if(defaultPicture != null)
                {
                    ModelState.AddModelError("AddPictureModel.VendorPictureType", "only allow one default picture");
                    var modelErrors = new Dictionary<string, string>();

                    foreach(var stateKey in ModelState.Keys)
                    {
                        foreach(var error in ModelState[stateKey].Errors)
                        {
                            modelErrors[stateKey] = error.ErrorMessage;
                        }
                    }

                    return Json(new { Result = false, Data = modelErrors });
                }
            }

            if (ModelState.IsValid) 
            {
                _pictureService.UpdatePicture(picture.Id,
                    _pictureService.LoadPictureBinary(picture),
                    picture.MimeType,
                    picture.SeoFilename,
                    overrideAltAttribute,
                    overrideTitleAttribute);

                _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName($"{vendor.Name} pic-{picture.Id}"));

                _vendorService.InsertVendorPicture(new VendorPicture
                {
                    PictureId = pictureId,
                    VendorId = vendorId,
                    DisplayOrder = displayOrder,
                    VendorPictureType = vendorPictureType,
                });
            }

            return Json(new { Result = true });
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ManageVendors),
            nameof(StandardPermissionProvider.ManageShop)
            )]
        [HttpPost]
        public virtual IActionResult VendorPictureList(VendorPictureSearchModel searchModel)
        {
            //try to get a vendor with the specified id
            var vendor = _vendorService.GetVendorById(searchModel.VendorId)
                ?? throw new ArgumentException("No vendor found with the specified id");

            //a vendor should have access only to his vendors
            if (!_workContext.IsAdmin || (_workContext.CurrentVendor != null && vendor.Id != _workContext.CurrentVendor.Id))
                return Content("This is not your vendor profile");

            //prepare model
            var model = _vendorModelFactory.PrepareVendorPictureListModel(searchModel, vendor);

            return Json(model);
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ManageVendors),
            nameof(StandardPermissionProvider.ManageShop)
            )]
        [HttpPost]
        public virtual IActionResult VendorPictureUpdate(VendorPictureModel model)
        {
            //try to get a vendor picture with the specified id
            var vendorPicture = _vendorService.GetVendorPictureById(model.Id)
                ?? throw new ArgumentException("No vendor picture found with the specified id");

            //if (!Enum.IsDefined(typeof(VendorPictureType), model.VendorPictureType))
            //{
            //    throw new ArgumentException("Invalid vendorPictureType");
            //}

            //a vendor should have access only to his vendors
            if (_workContext.CurrentVendor != null)
            {
                var vendor = _vendorService.GetVendorById(vendorPicture.VendorId);
                if (vendor != null && (!_workContext.IsAdmin || vendor.Id != _workContext.CurrentVendor.Id))
                    return Content("This is not your vendor profile");
            }

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(vendorPicture.PictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            vendorPicture.DisplayOrder = model.DisplayOrder;
            //vendorPicture.VendorPictureType = model.VendorPictureType;
            _vendorService.UpdateVendorPicture(vendorPicture);

            return new NullJsonResult();
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ManageVendors),
            nameof(StandardPermissionProvider.ManageShop)
            )]
        [HttpPost]
        public virtual IActionResult VendorPictureDelete(int id)
        {
            //try to get a vendor picture with the specified id
            var vendorPicture = _vendorService.GetVendorPictureById(id)
                ?? throw new ArgumentException("No vendor picture found with the specified id");

            //a vendor should have access only to his vendors
            if (_workContext.CurrentVendor != null)
            {
                var vendor = _vendorService.GetVendorById(vendorPicture.VendorId);
                if (vendor != null && (!_workContext.IsAdmin || vendor.Id != _workContext.CurrentVendor.Id))
                    return Content("This is not your vendor profile");
            }

            var pictureId = vendorPicture.PictureId;
            _vendorService.DeleteVendorPicture(vendorPicture);

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            try
            {
                _pictureService.DeletePicture(picture);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return new NullJsonResult();
        }

        #endregion
    }
}