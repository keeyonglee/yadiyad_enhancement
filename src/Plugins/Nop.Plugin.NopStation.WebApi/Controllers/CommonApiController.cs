using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Factories;
using Nop.Plugin.NopStation.WebApi.Filters;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Common;
using YadiYad.Pro.Core.Domain.Home;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/common")]
    public class CommonApiController : BaseApiController
    {
        #region Fields

        private readonly CommonSettings _commonSettings;
        private readonly ICommonModelFactory _commonModelFactory;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly VendorSettings _vendorSettings;
        private readonly ICommonApiModelFactory _commonApiModelFactory;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public CommonApiController(CommonSettings commonSettings,
            ICommonModelFactory commonModelFactory,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IVendorService vendorService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            VendorSettings vendorSettings,
            ICommonApiModelFactory commonApiModelFactory,
            ICustomerService customerService)
        {
            _commonSettings = commonSettings;
            _commonModelFactory = commonModelFactory;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _languageService = languageService;
            _localizationService = localizationService;
            _vendorService = vendorService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _vendorSettings = vendorSettings;
            _commonApiModelFactory = commonApiModelFactory;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        [CheckAccessPublicStore(true)]
        [HttpGet("setlanguage/{langid}")]
        public virtual IActionResult SetLanguage(int langid)
        {
            var language = _languageService.GetLanguageById(langid);
            if (!language?.Published ?? false)
                language = _workContext.WorkingLanguage;

            _workContext.WorkingLanguage = language;
            return Ok(_localizationService.GetResource("NopStation.WebApi.Response.LanguageChanged"));
        }

        [CheckAccessPublicStore(true)]
        [HttpGet("setcurrency/{customerCurrency}")]
        public virtual IActionResult SetCurrency(int customerCurrency)
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            return Ok(_localizationService.GetResource("NopStation.WebApi.Response.CurrencyChanged"));
        }

        [CheckAccessPublicStore(true)]
        [HttpGet("settaxtype/{customerTaxType}")]
        public virtual IActionResult SetTaxType(int customerTaxType)
        {
            var taxDisplayType = (TaxDisplayType)Enum.ToObject(typeof(TaxDisplayType), customerTaxType);
            _workContext.TaxDisplayType = taxDisplayType;

            return Ok(_localizationService.GetResource("NopStation.WebApi.Response.TaxTypeChanged"));
        }

        [HttpGet("contactus")]
        public virtual IActionResult ContactUs()
        {
            var response = new GenericResponseModel<ContactUsModel>();
            response.Data = _commonModelFactory.PrepareContactUsModel(new ContactUsModel(), false);
            return Ok(response);
        }

        [HttpPost("contactus")]
        public virtual IActionResult ContactUs([FromBody]BaseQueryModel<ContactUsModel> queryModel)
        {
            var model = queryModel.Data;
            var response = new GenericResponseModel<ContactUsModel>();
            model = _commonModelFactory.PrepareContactUsModel(model, true);

            if (ModelState.IsValid)
            {
                //var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;

                var subject = ((ContactUsSubject)model.SubjectId).GetDescription();

                if (subject == "Others")
                {
                    subject = model.SubjectOther;
                }

                var body = Nop.Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactUsMessage(_workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                response.Data = model;
                response.Message = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                //activity log
                _customerActivityService.InsertActivity("PublicStore.ContactUs",
                    _localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

                return Ok(response);
            }

            response.Data = model;
            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            return BadRequest(response);
        }

        [HttpGet("contactvendor/{vendorId}")]
        public virtual IActionResult ContactVendor(int vendorId)
        {
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return Unauthorized();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return NotFound();

            var model = new ContactVendorModel();
            model = _commonModelFactory.PrepareContactVendorModel(model, vendor, false);

            var response = new GenericResponseModel<ContactVendorModel>();
            response.Data = model;
            return Ok(response);
        }

        [HttpPost("contactvendor")]
        public virtual IActionResult ContactVendor([FromBody]BaseQueryModel<ContactVendorModel> queryModel)
        {
            var model = queryModel.Data;
            if (!_vendorSettings.AllowCustomersToContactVendors)
                return Unauthorized();

            var vendor = _vendorService.GetVendorById(model.VendorId);
            if (vendor == null || !vendor.Active || vendor.Deleted)
                return NotFound();

            var response = new GenericResponseModel<ContactVendorModel>();
            response.Data = _commonModelFactory.PrepareContactVendorModel(model, vendor, true);

            if (ModelState.IsValid)
            {
                var subject = _commonSettings.SubjectFieldOnContactUsForm ? model.Subject : null;
                var body = Nop.Core.Html.HtmlHelper.FormatText(model.Enquiry, false, true, false, false, false, false);

                _workflowMessageService.SendContactVendorMessage(vendor, _workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body);

                model.SuccessfullySent = true;
                response.Message = _localizationService.GetResource("ContactVendor.YourEnquiryHasBeenSent");

                return Ok(response);
            }

            foreach (var modelState in ModelState.Values)
                foreach (var error in modelState.Errors)
                    response.ErrorList.Add(error.ErrorMessage);

            return BadRequest(response);
        }

        [HttpGet("getstringresources/{languageId?}")]
        public virtual IActionResult GetStringResources(int? languageId)
        {
            var response = new GenericResponseModel<List<KeyValueApi>>();
            response.Data = _commonApiModelFactory.GetStringRsources(languageId).ToList();

            return Ok(response);
        }

        [HttpPost("setofflineuntil")]
        public virtual IActionResult SetOfflineUntil([FromBody] BaseQueryModel<string> queryModel)
        {
            if (!_customerService.IsVendor(_workContext.CurrentCustomer))
                return Unauthorized();

            var form = queryModel.FormValues.ToNameValueCollection();
            var checkDate = DateTime.TryParse(form["offline_until"], out var offlineUntilDateTime);
            var checkToggle = bool.TryParse(form["going_offline"], out var goingOffline);
            var vendor = _workContext.CurrentVendor;

            if (checkToggle)
            {
                if (goingOffline)
                {
                    if (checkDate)
                    {
                        vendor.OfflineUntil = offlineUntilDateTime;
                        _vendorService.UpdateVendor(vendor);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest("Invalid Date for going offline");
                    }
                }
                else
                {
                    vendor.OfflineUntil = null;
                    _vendorService.UpdateVendor(vendor);
                    return Ok();
                }
            }
            else
            {
                return BadRequest("Please select going online or offline");
            }
        }

        #endregion
    }
}
