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
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Filters;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class IndividualController : BaseController
    {
        private readonly AccountContext _accountContext;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;

        public IndividualController(
           ILocalizationService localizationService,
            AccountContext accountContext,
           IWorkContext workContext)
        {
            _accountContext = accountContext;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        public IActionResult Index()
        {
            if(string.IsNullOrWhiteSpace(_accountContext.CurrentAccount.Name))
            {
                return RedirectToAction(nameof(ProfileEdit));
            }
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        public IActionResult Dashboard()
        {
            return View();
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        public IActionResult Details()
        {
            var model = new IndividualProfileDTO();
            model.Email = _workContext.CurrentCustomer.Email;
            return View(model);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualProfile))]
        public IActionResult ProfileEdit()
        {
            var model = new IndividualProfileDTO();
            model.Email = _workContext.CurrentCustomer.Email;
            return View(model);
        }
    }
}
