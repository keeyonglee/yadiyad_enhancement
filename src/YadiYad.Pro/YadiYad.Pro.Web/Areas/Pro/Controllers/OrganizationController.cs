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
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Web.Filters;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class OrganizationController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public OrganizationController(
           ILocalizationService localizationService,
           IWorkContext workContext)
        {
            _localizationService = localizationService;
            _workContext = workContext;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationProfile))]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationProfile))]
        public IActionResult Dashboard()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationProfile))]
        public IActionResult Details()
        {
            var model = new OrganizationProfileDTO();
            model.ContactPersonEmail = _workContext.CurrentCustomer.Email;
            return View(model);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationProfile))]
        public IActionResult ProfileEdit()
        {
            var model = new OrganizationProfileDTO();
            model.ContactPersonEmail = _workContext.CurrentCustomer.Email;
            return View(model);
        }
    }
}
