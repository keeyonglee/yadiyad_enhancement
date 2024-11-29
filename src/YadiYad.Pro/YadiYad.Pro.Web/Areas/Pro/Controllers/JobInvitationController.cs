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
using YadiYad.Pro.Web.Models.Job;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class JobInvitationController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public JobInvitationController(
           ILocalizationService localizationService,
           IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        public IActionResult Invites()
        {
            return View();
        }

        //[AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        //public IActionResult Invited()
        //{
        //    return View();
        //}
        [HttpGet("pro/[controller]/{id}/Invited")]
        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult Invited(int id)
        {
            var model = new JobAdsHeaderModel
            {
                MenuType = YadiYad.Pro.Web.Enums.JobAdsMenuType.Invited,
                JobAdsId = id
            };
            return View(model);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        public IActionResult Accept()
        {
            return PartialView("_Accept");
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        public IActionResult Decline()
        {
            return PartialView("_Decline");
        }
    }
}