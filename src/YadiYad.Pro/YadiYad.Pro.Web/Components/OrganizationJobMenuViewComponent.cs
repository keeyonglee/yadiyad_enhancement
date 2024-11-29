using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Services.Attentions;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Web.Models.Job;

namespace YadiYad.Pro.Web.Components
{
    public class OrganizationJobMenuViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly OrganizationAttentionService _organizationAttentionService;

        public OrganizationJobMenuViewComponent(
            IWorkContext workContext,
            IPermissionService permissionService,
            OrganizationAttentionService organizationAttentionService)
        {
            _permissionService = permissionService;
            _organizationAttentionService = organizationAttentionService;
            _workContext = workContext;

        }

        public IViewComponentResult Invoke(JobAdsHeaderModel jobAdsHeaderModel)
        {
            jobAdsHeaderModel.OrganizationAttentionDTO = _organizationAttentionService
                .GetOrganizationUserAttention(_workContext.CurrentCustomer.Id);

            return View(jobAdsHeaderModel);
        }
    }
}
