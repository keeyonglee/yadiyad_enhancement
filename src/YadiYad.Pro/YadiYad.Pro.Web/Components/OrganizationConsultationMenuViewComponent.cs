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
using YadiYad.Pro.Web.Models.Consultation;

namespace YadiYad.Pro.Web.Components
{
    public class OrganizationConsultationMenuViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly OrganizationAttentionService _organizationAttentionService;

        public OrganizationConsultationMenuViewComponent(
            IWorkContext workContext,
            IPermissionService permissionService,
            OrganizationAttentionService organizationAttentionService)
        {
            _permissionService = permissionService;
            _organizationAttentionService = organizationAttentionService;
            _workContext = workContext;

        }

        public IViewComponentResult Invoke(ConsultationMenuType consultationMenuType)
        {
            var organizationConsultationMenu = new OrganizationConsultationMenuModel();

            organizationConsultationMenu.OrganizationAttentionDTO = _organizationAttentionService
                .GetOrganizationUserAttention(_workContext.CurrentCustomer.Id);

            organizationConsultationMenu.ConsultationMenuType = consultationMenuType;

            return View(organizationConsultationMenu);
        }
    }
}
