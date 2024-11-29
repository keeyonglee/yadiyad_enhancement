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
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Web.Filters;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class ServiceApplicationController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public ServiceApplicationController(
           ILocalizationService localizationService,
           IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult Receives()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult Hires()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult Requests()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult Confirms()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpGet("pro/[controller]/repropose")]
        public IActionResult Repropose()
        {
            return PartialView("_Repropose");
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpGet("pro/[controller]/request")]
        public IActionResult Request()
        {
            return PartialView("_Request");
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
