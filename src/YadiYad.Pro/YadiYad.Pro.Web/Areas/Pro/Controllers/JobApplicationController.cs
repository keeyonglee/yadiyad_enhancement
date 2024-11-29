using Microsoft.AspNetCore.Mvc;
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
    public class JobApplicationController : BaseController
    {
        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        public IActionResult Apply()
        {
            return PartialView("_Apply");
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet("pro/[controller]/{id}/applicants")]
        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult Applicants(int id)
        {
            var model = new JobAdsHeaderModel
            {
                MenuType = YadiYad.Pro.Web.Enums.JobAdsMenuType.Applicants,
                JobAdsId = id
            };
            return View(model);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult Hired()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult Review()
        {
            return PartialView("_Review");
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        public IActionResult HireJobSeeker()
        {
            return PartialView("_HireJobSeeker");
        }

        public IActionResult Terminate()
        {
            return PartialView("_Terminate");
        }

        public IActionResult Cancel()
        {
            return PartialView("_Cancel");
        }
    }
}
