using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Services.Attentions;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Web.Models.Attentions;

namespace YadiYad.Pro.Web.Components
{
    public class AccountMenuViewComponent: NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly IndividualAttentionService _individualAttentionService;
        private readonly OrganizationAttentionService _organizationAttentionService;

        public AccountMenuViewComponent(
            IWorkContext workContext,
            AccountContext accountContext,
            IndividualAttentionService individualAttentionService,
            OrganizationAttentionService organizationAttentionService)
        {
            _workContext = workContext;
            _accountContext = accountContext;
            _individualAttentionService = individualAttentionService;
            _organizationAttentionService = organizationAttentionService;
        }

        public IViewComponentResult Invoke()
        {
            var attentionVM = new MenuAttentionModel();

            switch(_accountContext.CurrentAccount.AccountType)
            {
                case AccountType.Individual:
                    attentionVM.IndividualAttentionDTO = _individualAttentionService
                        .GetIndividualUserAttention(_workContext.CurrentCustomer.Id);

                    break;
                case AccountType.Organization:
                    attentionVM.OrganizationAttnetionDTO = _organizationAttentionService
                        .GetOrganizationUserAttention(_workContext.CurrentCustomer.Id);
                    break;
            }

            return View("~/Areas/Pro/Views/Shared/Components/AccountMenu/Default.cshtml", attentionVM);
        }
    }
}
