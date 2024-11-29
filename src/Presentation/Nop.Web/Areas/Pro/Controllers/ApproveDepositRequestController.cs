using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.ApproveDepositRequest;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Services.Messages;

namespace Nop.Web.Areas.Pro.Controllers
{
    public class ApproveDepositRequestController : BaseAdminController
    {
        #region Fields

        private readonly ApproveDepositRequestModelFactory _approveDepositRequestModelFactory;
        private readonly DepositRequestService _depositRequestService;
        private readonly IWorkContext _workContext;
        private readonly OrderProcessingService _orderProcessingService;
        private readonly OrderService _orderService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        #endregion

        #region Ctor
        public ApproveDepositRequestController(
            ApproveDepositRequestModelFactory approveDepositRequestModelFactory,
            DepositRequestService depositRequestService,
            IWorkContext workContext,
            OrderProcessingService orderProcessingService,
            OrderService orderService,
            ProWorkflowMessageService proWorkflowMessageService)
        {
            _approveDepositRequestModelFactory = approveDepositRequestModelFactory;
            _depositRequestService = depositRequestService;
            _workContext = workContext;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _proWorkflowMessageService = proWorkflowMessageService;

        }
        #endregion

        #region Methods

        #region List

        public IActionResult Index()
        {
            return View();
        }

        public virtual IActionResult List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {

            //prepare model
            var model = _approveDepositRequestModelFactory.PrepareSearchModel(new ApproveDepositRequestSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ApproveDepositRequestList(ApproveDepositRequestSearchModel searchModel)
        {
            var model = _approveDepositRequestModelFactory.PrepareListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult Edit(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            //try to get a category with the specified id
            var deposit = _depositRequestService.GetApprovedDepositRequestById(id);
            if (deposit == null)
                return RedirectToAction("List");

            //prepare model
            var model = _approveDepositRequestModelFactory.PrepareModel(null, deposit);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ApproveDepositRequestModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            var deposit = _depositRequestService.GetApprovedDepositRequestById(model.Id);

            if (deposit == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                if (model.Validity == true)
                {
                    var dto = new SubmitOrderDTO 
                    {
                        RefId = deposit.RefId,
                        ProductTypeId = deposit.ProductTypeId,
                        DepositRequestId = model.Id
                    };
                    var order = _orderProcessingService.CreateOrder(dto);
                    var proOrder = _orderService.GetCustomOrder(order.Id);

                    proOrder.OrderStatus = OrderStatus.Complete;
                    proOrder.PaymentStatus = PaymentStatus.Paid;

                    _orderProcessingService.ProcessOrder(proOrder);

                    deposit.OrderItemId = order.OrderItems.Where(x => x.ProductTypeId == deposit.ProductTypeId).Select(x => x.Id).FirstOrDefault();
                    deposit.Status = (int)DepositRequestStatus.Paid;
                    deposit.UpdatedOnUTC = DateTime.UtcNow;
                    deposit.UpdatedById = _workContext.CurrentCustomer.Id;
                    _depositRequestService.UpdateApprovedDepositRequest(deposit);
                    _proWorkflowMessageService.SendDepositApproved(_workContext.WorkingLanguage.Id, deposit.ProductTypeId, deposit.RefId, model.ApproveRemarks, true);
                }
                else
                {
                    if (model.ApproveRemarks != "")
                    {
                        //var newApproveRemarks = "<p>" + DateTime.UtcNow.ToString() + "-" + model.ApproveRemarks + "</p>";
                        var newApproveRemarks = $"<p>[{DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm")}] - {model.ApproveRemarks}</p>";
                        deposit.ApproveRemarks = newApproveRemarks + deposit.ApproveRemarks;
                    }
                    deposit.Status = (int)DepositRequestStatus.Invalid;
                    deposit.UpdatedOnUTC = DateTime.UtcNow;
                    deposit.UpdatedById = _workContext.CurrentCustomer.Id;
                    _depositRequestService.UpdateApprovedDepositRequest(deposit);
                    _proWorkflowMessageService.SendDepositApproved(_workContext.WorkingLanguage.Id, deposit.ProductTypeId, deposit.RefId, model.ApproveRemarks, false);
                }

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = deposit.Id });
            }

            //prepare model
            model = _approveDepositRequestModelFactory.PrepareModel(model, deposit, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #endregion

    }
}
