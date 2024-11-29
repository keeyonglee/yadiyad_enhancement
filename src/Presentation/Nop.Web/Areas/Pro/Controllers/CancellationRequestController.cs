using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.CancellationRequest;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.DTO.Moderator;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Services.Moderator;

namespace Nop.Web.Areas.Pro.Controllers
{
    public class CancellationRequestController : BaseAdminController
    {
        #region Fields

        private readonly CancellationRequestModelFactory _cancellationRequestModelFactory;
        private readonly ModeratorCancellationRequestService _moderatorCancellationRequestService;
        private readonly BlockCustomerService _blockCustomerService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly IWorkContext _workContext;
        private readonly EngagementCancellationManager _engagementCancellationManager;
        #endregion

        #region Ctor

        public CancellationRequestController(
            CancellationRequestModelFactory cancellationRequestModelFactory,
            ModeratorCancellationRequestService moderatorCancellationRequestService,
            BlockCustomerService blockCustomerService,
            ProWorkflowMessageService proWorkflowMessageService,
            IWorkContext workContext,
            EngagementCancellationManager engagementCancellationManager)
        {
            _cancellationRequestModelFactory = cancellationRequestModelFactory;
            _moderatorCancellationRequestService = moderatorCancellationRequestService;
            _blockCustomerService = blockCustomerService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _workContext = workContext;
            _engagementCancellationManager = engagementCancellationManager;

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
            var model = _cancellationRequestModelFactory.PrepareCancellationRequestSearchModel(new CancellationRequestSearchModel());
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CancellationRequestList(CancellationRequestSearchModel searchModel)
        {
            var model = _cancellationRequestModelFactory.PrepareCancellationRequestListModel(searchModel);
            return Json(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult Edit(int type, int id)
        {
            //Change type to int
            var engagement = _moderatorCancellationRequestService.GetEngagementEdit(id, (EngagementType)type);
            if (engagement == null)
                return RedirectToAction("List");
            var model = _cancellationRequestModelFactory.PrepareCancellationRequestEditModel(null, engagement);
            var blockStatus = _blockCustomerService.GetBlockStatus(model.SellerId);
            model.EngagementTypeId = type;
            model.EngagementType = ((EngagementType)type).GetDescription();
            model.BlockReasonList = _moderatorCancellationRequestService.GetBlockReasonSeller();
            model.BlockLast90Days = blockStatus.WasBlockLast90Days == true ? "Yes" : "No";
            model.BlockCurrently = blockStatus.IsCurrentlyBlock == true ? true : false;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CancellationRequestEditModel model, bool continueEditing)
        {
            var actorId = _workContext.CurrentCustomer.Id;
            if (model.BlockSeller == true)
            {
                _engagementCancellationManager.CloseCancellationRequest(model.EngagementId, (EngagementType)model.EngagementTypeId, actorId, adminRemarks: model.Remarks,
                    _workContext.CurrentCustomer.Id,
               attachmentId: model.ReasonAttachmentDownloadId);
                _proWorkflowMessageService.SendBlockCustomer(_workContext.WorkingLanguage.Id, model.SellerId, model.Remarks);
            }
            else
            {
                if (model.ReasonAttachmentDownloadId != 0)
                {
                    _engagementCancellationManager.UpdateCancellationRequest(model.EngagementId, (EngagementType)model.EngagementTypeId, actorId,
               attachmentId: model.ReasonAttachmentDownloadId);
                }
            }

            if (!continueEditing)
                return RedirectToAction("List");
            return RedirectToAction("Edit", new { id = model.EngagementId});
            //model = _cancellationRequestModelFactory.PrepareCancellationRequestModel(model, consultation, true);
        }

        #endregion

        #endregion

    }
}
