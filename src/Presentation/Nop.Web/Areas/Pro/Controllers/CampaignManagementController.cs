using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.CampaignManagement;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Services.Services.Campaign;

namespace Nop.Web.Areas.Pro.Controllers
{
    public class CampaignManagementController : BaseAdminController
    {
        #region Fields

        private readonly CampaignManagementModelFactory _campaignManagementModelFactory;
        private readonly CampaignManagementService _campaignManagementService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public CampaignManagementController(
            CampaignManagementModelFactory campaignManagementModelFactory,
            CampaignManagementService campaignManagementService,
            IWorkContext workContext)
        {
            _campaignManagementModelFactory = campaignManagementModelFactory;
            _campaignManagementService = campaignManagementService;
            _workContext = workContext;
        }
        #endregion

        #region Methods

        #region List

        public IActionResult Index()
        {
            return RedirectToAction("List");

        }

        public virtual IActionResult List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {

            //prepare model
            var model = _campaignManagementModelFactory.PrepareCampaignManagementSearchModel(new CampaignManagementSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CampaignManagementList(CampaignManagementSearchModel searchModel)
        {

            //prepare model
            var model = _campaignManagementModelFactory.PrepareCampaignManagementListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create

        public virtual IActionResult Create()
        {
            var model = _campaignManagementModelFactory.PrepareCampaignManagementModel(new CampaignManagementModel(), null);

            ViewData["ValidShuqRewardsIndividual"] = _campaignManagementService.GetValidShuqRewardsIndividual();
            ViewData["ValidIndividualRewardTypes"] = _campaignManagementService.GetValidRewardsIndividual();
            ViewData["ValidOrganizationRewardTypes"] = _campaignManagementService.GetValidRewardsOrganization();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CampaignManagementModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var campaign = model.ToEntity<CampaignManagement>();
                campaign.CreatedOnUTC = DateTime.UtcNow;
                campaign.CreatedById = _workContext.CurrentCustomer.Id;
                _campaignManagementService.Insert(campaign);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = campaign.Id });
            }

            model = _campaignManagementModelFactory.PrepareCampaignManagementModel(model, null, true);
            ViewData["ValidShuqRewardsIndividual"] = _campaignManagementService.GetValidShuqRewardsIndividual();
            ViewData["ValidIndividualRewardTypes"] = _campaignManagementService.GetValidRewardsIndividual();
            ViewData["ValidOrganizationRewardTypes"] = _campaignManagementService.GetValidRewardsOrganization();
            return View(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult Edit(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            //try to get a category with the specified id
            var campaign = _campaignManagementService.GetById(id);
            if (campaign == null)
                return RedirectToAction("List");

            //prepare model
            var model = _campaignManagementModelFactory.PrepareCampaignManagementModel(null, campaign);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CampaignManagementModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            var campaign = _campaignManagementService.GetById(model.Id);
            if (campaign == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                campaign.From = model.From.Value;
                campaign.Until = model.Until;
                campaign.UpdatedOnUTC = DateTime.UtcNow;
                campaign.UpdatedById = _workContext.CurrentCustomer.Id;
                _campaignManagementService.Update(campaign);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = campaign.Id });
            }

            //prepare model
            model = _campaignManagementModelFactory.PrepareCampaignManagementModel(model, campaign, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #endregion

    }
}
