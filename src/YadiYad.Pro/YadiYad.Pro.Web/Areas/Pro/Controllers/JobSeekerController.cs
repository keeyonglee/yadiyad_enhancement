using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.JobSeeker;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class JobSeekerController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly JobSeekerProfileService _jobSeekerProfilerService;

        public JobSeekerController(
           ILocalizationService localizationService,
           JobSeekerProfileService jobSeekerProfilerService,
           IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
            _jobSeekerProfilerService = jobSeekerProfilerService;
        }

        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Index()
        {
            //if (_jobSeekerProfilerService.HasJobSeekerProfile(_workContext.CurrentCustomer.Id) == false)
            //{
            //    return RedirectToAction("Details", "JobSeeker", new { area = AreaNames.Pro });
            //}
            return View();
        }
    }
}
