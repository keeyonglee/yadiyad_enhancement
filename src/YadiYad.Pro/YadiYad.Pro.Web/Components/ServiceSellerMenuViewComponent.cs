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
using YadiYad.Pro.Web.Models.Attentions;
using YadiYad.Pro.Web.Models.Services;

namespace YadiYad.Pro.Web.Components
{
    public class ServiceSellerMenuViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IndividualAttentionService _individualAttentionService;

        public ServiceSellerMenuViewComponent(
            IWorkContext workContext,
            IPermissionService permissionService,
            IndividualAttentionService individualAttentionService)
        {
            _permissionService = permissionService;
            _individualAttentionService = individualAttentionService;
            _workContext = workContext;

        }

        public IViewComponentResult Invoke(ServiceSellerMenuType serviceSellerMenuType)
        {
            var serviceSellerMenuModel = new ServiceSellerMenuModel();

            serviceSellerMenuModel.IndividualAttentionDTO = _individualAttentionService
                .GetIndividualUserAttention(_workContext.CurrentCustomer.Id);

            serviceSellerMenuModel.ServiceSellerMenuType = serviceSellerMenuType;

            return View(serviceSellerMenuModel);
        }
    }
}
