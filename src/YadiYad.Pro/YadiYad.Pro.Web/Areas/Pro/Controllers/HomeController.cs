using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Home;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Factories;
using YadiYad.Pro.Web.Models.Customer;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class HomeController : BaseController
    {
        #region Fields

        private readonly AccountContext _acountContext;
        private readonly IPermissionService _permissionService;
        private readonly CommonModelFactory _commonModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly ProHomeSettings _proHomeSettings;

        #endregion

        #region Ctor

        public HomeController(
            AccountContext accountContext,
            IPermissionService permissionService,
            CommonModelFactory commonModelFactory,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            IWorkContext workContext,
            ProWorkflowMessageService proWorkflowMessageService,
            ProHomeSettings proHomeSettings)
        {
            _permissionService = permissionService;
            _acountContext = accountContext;
            _commonModelFactory = commonModelFactory;
            _localizationService = localizationService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _proWorkflowMessageService = proWorkflowMessageService;
            _proHomeSettings = proHomeSettings;
        }

        #endregion

        #region Methods

        [HttpGet("/")]
        public IActionResult Landing([FromQuery] string utm_source, [FromQuery] string utm_medium, [FromQuery] string utm_campaign)
        {
            return RedirectToAction("Landing", "Home", new { area = AreaNames.Pro, utm_source = utm_source, utm_medium = utm_medium, utm_campaign = utm_campaign });
        }

        [HttpGet("/pro")]
        public IActionResult Index([FromQuery] string utm_source, [FromQuery] string utm_medium, [FromQuery] string utm_campaign)
        {
            if (_permissionService.Authorize(nameof(StandardPermissionProvider.ModeratorReview)))
            {
                return RedirectToAction("JobAdsReview", "Consultation", new { area = AreaNames.Pro });
            }
            if (_permissionService.Authorize(nameof(StandardPermissionProvider.IndividualProfile)))
            {
                return RedirectToAction("Index", "Individual", new { area = AreaNames.Pro });
            }
            if (_permissionService.Authorize(nameof(StandardPermissionProvider.OrganisationProfile)))
            {
                return RedirectToAction("Index", "Organization", new { area = AreaNames.Pro });
            }

            return RedirectToRoute("ProHomepage");
        }

        [HttpGet]
        [ActionName("Info")]
        public IActionResult Info()
        {
            var link = _proHomeSettings.InfoPrimaryVideoUrl;
            ViewData["PrimaryVideoUrl"] = link;
            return View();
        }
        [HttpGet]
        public IActionResult Individual()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Organization()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Segment()
        {
            var model = new SegmentContactUsModel();
            return View(model);
        }
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }
        //[HttpGet("/comingsoon")]
        //public IActionResult ComingSoon()
        //{
        //    return View();
        //}
        [HttpGet("/landing")]
        public IActionResult Landing()
        {
            return View();
        }

        [HttpGet("/splash")]
        public IActionResult Splash()
        {
            return View();
        }

        public IActionResult Vision()
        {
            return View();
        }

        public IActionResult HowItWork()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        public IActionResult ContactUs()
        {
            var model = new ContactUsProModel();
            model = _commonModelFactory.PrepareContactUsModel(model, false);
            ViewData["ContactUsSubjects"] = Enum.GetValues(typeof(ContactUsSubject))
               .Cast<ContactUsSubject>()
               .Select(x => new SelectListItem
               {
                   Value = ((int)x).ToString(),
                   Text = x.GetDescription()
               })
               .OrderBy(x => x.Text == "Others")
               .ThenBy(x => x.Text)
               .ToList();
            return View(model);
        }

        [HttpPost, ActionName("ContactUs")]
        public virtual IActionResult ContactUsSend(ContactUsProModel model)
        {
            model = _commonModelFactory.PrepareContactUsModel(model, true);

            if (ModelState.IsValid)
            {
                var subject = ((ContactUsSubject)model.SubjectId).GetDescription();

                if (subject == "Others")
                {
                    subject = model.SubjectOther;
                }
                var body = model.Enquiry;

                //_workflowMessageService.SendContactUsMessage(_workContext.WorkingLanguage.Id,
                //    model.Email.Trim(), model.FullName, subject, body);
                _proWorkflowMessageService.SendContactUsMessage(_workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.FullName, subject, body, model.SubjectId);
                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                return View(model);
            }

            return View(model);
        }

        [HttpPost, ActionName("Segment")]
        public virtual IActionResult Segment(SegmentContactUsModel model)
        {
            if (ModelState.IsValid)
            {
                var subject = ContactUsSubject.Suggestions.ToString();
                var subjectId = (int)ContactUsSubject.Suggestions;
                var body = "Hi I would like to enquire for new segment - " + model.Segment;

                //_workflowMessageService.SendContactUsMessage(_workContext.WorkingLanguage.Id,
                //    model.Email.Trim(), model.FullName, subject, body);
                _proWorkflowMessageService.SendContactUsMessage(_workContext.WorkingLanguage.Id,
                    model.Email.Trim(), model.Name, subject, body, subjectId);
                model.SuccessfullySent = true;
                model.Result = _localizationService.GetResource("ContactUs.YourEnquiryHasBeenSent");

                return View(model);
            }

            return View(model);
        }

        #endregion

    }
}
