using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Vendors;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;

namespace Nop.Web.Controllers
{
    [CamelCaseResponseFormatter]
    [Route("api/shuq/[controller]")]
    public partial class VendorApplicationController : BasePublicController
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly ICustomerService _customerService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly IVendorApplicationService _vendorApplicationService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public VendorApplicationController(CaptchaSettings captchaSettings,
            ICustomerService customerService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IUrlRecordService urlRecordService,
            IVendorAttributeParser vendorAttributeParser,
            IVendorAttributeService vendorAttributeService,
            IVendorModelFactory vendorModelFactory,
            IVendorApplicationService vendorApplicationService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            VendorSettings vendorSettings)
        {
            _captchaSettings = captchaSettings;
            _customerService = customerService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _urlRecordService = urlRecordService;
            _vendorAttributeParser = vendorAttributeParser;
            _vendorAttributeService = vendorAttributeService;
            _vendorModelFactory = vendorModelFactory;
            _vendorApplicationService = vendorApplicationService;
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

        protected virtual string ParseVendorAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = _vendorAttributeService.GetAllVendorAttributes();
            foreach (var attribute in attributes)
            {
                var controlId = $"{NopVendorDefaults.VendorAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                )
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
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
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _vendorAttributeParser.AddVendorAttribute(attributesXml,
                                    attribute, enteredText);
                            }
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

        #endregion

        #region Methods



        //[HttpsRequirement]
        //public virtual IActionResult ApplyVendor()
        //{
        //    if (!_vendorSettings.AllowCustomersToApplyForVendorAccount)
        //        return RedirectToRoute("Homepage");

        //    if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
        //        return Challenge();

        //    var model = new ApplyVendorModel();

        //    var vendorApplication = _vendorApplicationService.GetVendorApplicationByCustomerId(_workContext.CurrentCustomer.Id);
        //    if (vendorApplication != null)
        //    {
        //        model.Id = vendorApplication.Id;
        //        model.Name = vendorApplication.StoreName;
        //        model.BusinessNatureCategoryId = vendorApplication.BusinessNatureCategoryId;
        //        model.CategoryId = vendorApplication.CategoryId;
        //        model.ProposedCategory = vendorApplication.ProposedCategory;
        //        model.SampleProductPictureIds = _vendorApplicationService.GetSampleProductPicturesByVendorApplicationId(vendorApplication.Id).Select(picture => picture.Id).ToList();
        //    }

        //    model = _vendorModelFactory.PrepareApplyVendorModel(model, true, false);
        //    return View(model);
        //}

        [HttpPost("create")]
        public IActionResult ApplyVendorSubmit([FromBody] ApplyVendorModel model)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
                return Ok(response);
            }

            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                throw new Exception("User is not registered");

            if (_vendorApplicationService.GetVendorApplicationByCustomerId(_workContext.CurrentCustomer.Id) != null)
                throw new Exception("Vendor application already exist");

            if(model.SampleProductPictureIds == null
                || model.SampleProductPictureIds.Count <= 0)
            {
                throw new Exception("Vendor application already exist");
            }


            //insert to vendor application
            var vendorApplication = new VendorApplication
            {
                CustomerId = _workContext.CurrentCustomer.Id,
                StoreName = model.Name,
                BusinessNatureCategoryId = model.BusinessNatureCategoryId,
                CategoryId = model.CategoryId,
                ProposedCategory = model.ProposedCategory,
                CreatedById = _workContext.CurrentCustomer.Id,
                CreatedOnUtc = DateTime.UtcNow,
            };
            _vendorApplicationService.InsertVendorApplication(vendorApplication);

            model.SampleProductPictureIds.ForEach(pictureId =>
            {
                _vendorApplicationService.InsertSampleProductPicture(new VendorApplicationSampleProductPicture
                {
                    VendorApplicationId = vendorApplication.Id,
                    PictureId = pictureId,
                });
            });

            //var vendorApplication = new VendorApplication
            //{
            //    Id = model.Id,
            //    CustomerId = _workContext.CurrentCustomer.Id,
            //    StoreName = model.Name,
            //    BusinessNatureCategoryId = model.BusinessNatureCategoryId,
            //    CategoryId = model.CategoryId,
            //    ProposedCategory = model.ProposedCategory,
            //    CreatedById = _workContext.CurrentCustomer.Id,
            //    CreatedOnUtc = DateTime.UtcNow,
            //};
            //_vendorApplicationService.UpdateVendorApplication(vendorApplication);

            //var existingPictures = _vendorApplicationService.GetSampleProductPicturesByVendorApplicationId(model.Id);
            //model.PictureIds
            //    .Where(pictureId => !existingPictures.Any(picture => picture.PictureId == pictureId))
            //    .ToList()
            //    .ForEach(pictureId =>
            //    {
            //        _vendorApplicationService.InsertSampleProductPicture(new VendorApplicationSampleProductPicture
            //        {
            //            VendorApplicationId = vendorApplication.Id,
            //            PictureId = pictureId,
            //        });
            //    });
            //existingPictures
            //    .Where(picture => !model.PictureIds.Any(pictureId => picture.PictureId == pictureId))
            //    .ToList()
            //    .ForEach(picture =>
            //    {
            //        _vendorApplicationService.DeleteProductPicture(picture);
            //    });

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        #endregion
    }
}