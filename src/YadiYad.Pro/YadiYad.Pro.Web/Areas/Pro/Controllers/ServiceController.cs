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
    public class ServiceController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public ServiceController(
           ILocalizationService localizationService,
           IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult Details()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpGet("pro/[controller]/{id}")]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult Search()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        public IActionResult List()
        {
            return View();
        }
    }
}
