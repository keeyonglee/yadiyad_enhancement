using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Models.Moderator;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class ConsultationController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public ConsultationController(
           ILocalizationService localizationService,
           IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        public IActionResult List()
        {
            return View();
        }
        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpGet("pro/[controller]")]
        [HttpGet("pro/[controller]/{id}")]
        public IActionResult Details()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpGet("pro/[controller]/{id}/candidates")]
        public IActionResult Candidates()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpGet("pro/[controller]/invited")]
        public IActionResult Invited()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpGet("pro/[controller]/applicant")]
        public IActionResult Applicant()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpGet("pro/[controller]/confirmed")]
        public IActionResult ConfirmedOrder()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpGet("pro/[controller]/advs/review")]
        public IActionResult JobAdsReview()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpGet("pro/[controller]/reply/review")]
        public IActionResult JobReplyReview()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpGet("pro/[controller]/facilitating")]
        public IActionResult SessionFacilitating()
        {
            return View();
        }

        [HttpGet("pro/[controller]/complete")]
        public IActionResult Complete(int id)
        {
            var model = new ModeratorConsultationModel();
            model.Id = id;
            return PartialView("_Complete", model);
        }

        [HttpGet("pro/[controller]/reschedule")]
        public IActionResult Reschedule(int id)
        {
            var model = new ModeratorConsultantRescheduleModel();
            model.Id = id;
            return PartialView("_Reschedule", model);
        }

        [HttpGet("pro/[controller]/cancel")]
        public IActionResult Cancel(int id)
        {
            var model = new ModeratorConsultationCancelModel();
            model.Id = id;
            return PartialView("_Cancel", model);
        }
    }
}
