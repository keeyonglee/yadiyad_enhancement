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
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Models.Job;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class JobController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly PermissionService _permissionService;
        private readonly JobSeekerProfileService _jobSeekerProfilerService;

        public JobController(
           ILocalizationService localizationService,
           IWorkContext workContext,
           PermissionService permissionService,
           JobSeekerProfileService jobSeekerProfilerService)
        {
            _localizationService = localizationService;
            _workContext = workContext;
            _permissionService = permissionService;
            _jobSeekerProfilerService = jobSeekerProfilerService;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpGet("pro/[controller]/{id}")]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult Details()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpGet("pro/[controller]/search")]
        [HttpPost("pro/[controller]/search")]
        public IActionResult Search()
        {
            if (_jobSeekerProfilerService.HasJobSeekerProfile(_workContext.CurrentCustomer.Id) == false)
            {
                return RedirectToAction("Details", "JobSeeker", new { area = AreaNames.Pro });
            }

            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpGet("pro/[controller]/{id}/candidate")]
        [HttpPost("pro/[controller]/{id}/candidate")]
        public IActionResult Candidate([FromRoute] int id)
        {
            var model = new JobAdsHeaderModel
            {
                MenuType = YadiYad.Pro.Web.Enums.JobAdsMenuType.SearchCandidates,
                JobAdsId = id
            };
            return View(model);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult Summary()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult List()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Applications()
        {
            return View();
        }
    }
}