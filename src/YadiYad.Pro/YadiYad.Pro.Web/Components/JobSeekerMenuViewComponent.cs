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
using YadiYad.Pro.Web.Models.Job;

namespace YadiYad.Pro.Web.Components
{
    public class JobSeekerMenuViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IndividualAttentionService _individualAttentionService;

        public JobSeekerMenuViewComponent(
            IWorkContext workContext,
            IPermissionService permissionService,
            IndividualAttentionService individualAttentionService)
        {
            _permissionService = permissionService;
            _individualAttentionService = individualAttentionService;
            _workContext = workContext;

        }

        public IViewComponentResult Invoke(JobSeekerMenuType jobSeekerMenuType)
        {
            var jobSeekerMenuModel = new JobSeekerMenuModel();

            jobSeekerMenuModel.IndividualAttentionDTO = _individualAttentionService
                .GetIndividualUserAttention(_workContext.CurrentCustomer.Id);

            jobSeekerMenuModel.JobSeekerMenuType = jobSeekerMenuType;

            return View(jobSeekerMenuModel);
        }
    }
}
