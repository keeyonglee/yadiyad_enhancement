using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payout;
using Nop.Services.Security;
using Nop.Services.ShuqOrders;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class DisputeController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDisputeModelFactory _disputeModelFactory;
        private readonly IVendorService _vendorService;
        private readonly IPictureService _pictureService;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly IWorkContext _workContext;
        private readonly OrderPayoutService _orderPayoutService;
        private readonly IEventPublisher _eventPublisher;

        #endregion Fields

        #region Ctor

        public DisputeController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IReturnRequestModelFactory returnRequestModelFactory,
            IReturnRequestService returnRequestService,
            IDisputeModelFactory disputeModelFactory,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            IPictureService pictureService,
            IShuqOrderProcessingService orderProcessingService,
            IWorkContext workContext,
            OrderPayoutService orderPayoutService,
            IEventPublisher eventPublisher)
        {
            _customerActivityService = customerActivityService;
            _disputeModelFactory = disputeModelFactory;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _returnRequestService = returnRequestService;
            _workflowMessageService = workflowMessageService;
            _vendorService = vendorService;
            _pictureService = pictureService;
            _orderProcessingService = orderProcessingService;
            _workContext = workContext;
            _orderPayoutService = orderPayoutService;
            _eventPublisher = eventPublisher;
        }

        #endregion
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDisputes))
                return AccessDeniedView();

            //prepare model
            var model = _disputeModelFactory.PrepareDisputeSearchModel(new DisputeSearchModel());

            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DisputeSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDisputes))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _disputeModelFactory.PrepareDisputeListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult EditDispute(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var model = _returnRequestModelFactory.PrepareDisputeSubmitModel(null, id);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult EditDispute(DisputeSubmitModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var actorId = _workContext.CurrentCustomer.Id;

            var dispute = new Dispute();

            var disputeList = _returnRequestService.GetDisputeById(model.Id);
            var order = _orderService.GetOrderById(disputeList.OrderId);
            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(disputeList.GroupReturnRequestId);
            if (returnRequest == null)
                return RedirectToAction("List");

            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestById(disputeList.GroupReturnRequestId);
            var returnRequestList = _returnRequestService.GetReturnRequestByGroupReturnRequestId(disputeList.GroupReturnRequestId);
            var orderItem = _orderService.GetOrderItemById(returnRequest[0].OrderItemId);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var existingModel = _returnRequestModelFactory.PrepareDisputeSubmitModel(null, disputeList.Id);
            model.TotalReturnAmount = existingModel.TotalReturnAmount;

            ModelState.Clear();
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                dispute.Id = disputeList.Id;
                dispute.DisputeReasonId = model.DisputeReasonId;
                dispute.DisputeAction = model.DisputeAction;
                dispute.GroupReturnRequestId = disputeList.GroupReturnRequestId;
                dispute.CreatedOnUtc = disputeList.CreatedOnUtc;
                dispute.DisputeDetail = model.DisputeDetail;
                dispute.OrderId = disputeList.OrderId;
                dispute.PartialAmount = model.PartialAmount;
                dispute.RaiseClaim = model.RaiseClaim;
                dispute.VendorId = disputeList.VendorId;
                dispute.IsReturnDispute = disputeList.IsReturnDispute;

                dispute.UpdatedOnUtc = DateTime.UtcNow;

                if (dispute.IsReturnDispute == true)
                    groupReturnRequest.NeedReturnShipping = false;
                else
                    groupReturnRequest.NeedReturnShipping = model.NeedReturnShipping;

                //if (dispute.DisputeAction == (int)DisputeActionEnum.FullRefundFromBuyer || dispute.DisputeAction == (int)DisputeActionEnum.PartialRefund)
                //{
                //    _orderProcessingService.ApproveReturnRequest(groupReturnRequest, true);
                //    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(dispute));
                //} 
                //else if (dispute.DisputeAction == (int)DisputeActionEnum.NoRefund)
                //{
                //    groupReturnRequest.ApproveStatusId = (int)ApproveStatusEnum.NotApproved;
                //    _returnRequestService.UpdateGroupReturnRequest(groupReturnRequest);
                //}

                if (dispute.DisputeAction == (int)DisputeActionEnum.FullRefundFromBuyer)
                {
                    _orderProcessingService.ApproveReturnRequest(groupReturnRequest, true);
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(dispute, groupReturnRequest));
                    if (groupReturnRequest.NeedReturnShipping)
                    {
                        _workflowMessageService.SendOrderDisputeOutcomeFullRefundAndReturnCustomerNotification(order, dispute);
                        _workflowMessageService.SendOrderDisputeOutcomeFullRefundAndReturnVendorNotification(order, vendor, dispute);
                    }
                    else
                    {
                        _workflowMessageService.SendOrderDisputeOutcomeFullRefundCustomerNotification(order, dispute);
                        _workflowMessageService.SendOrderDisputeOutcomeFullRefundVendorNotification(order, vendor, dispute);
                    }
                }
                else if (dispute.DisputeAction == (int)DisputeActionEnum.PartialRefund)
                {
                    _orderProcessingService.ApproveReturnRequest(groupReturnRequest, true);
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(dispute, groupReturnRequest));
                    if (groupReturnRequest.NeedReturnShipping)
                    {
                        _workflowMessageService.SendOrderDisputeOutcomePartialRefundAndReturnCustomerNotification(order, dispute);
                        _workflowMessageService.SendOrderDisputeOutcomePartialRefundAndReturnVendorNotification(order, vendor, dispute);
                    }
                    else
                    {
                        _workflowMessageService.SendOrderDisputeOutcomePartialRefundCustomerNotification(order, dispute);
                        _workflowMessageService.SendOrderDisputeOutcomePartialRefundVendorNotification(order, vendor, dispute);
                    }
                }
                else if (dispute.DisputeAction == (int)DisputeActionEnum.NoRefund)
                {
                    groupReturnRequest.ApproveStatusId = (int)ApproveStatusEnum.NotApproved;
                    _returnRequestService.UpdateGroupReturnRequest(groupReturnRequest);
                    _workflowMessageService.SendOrderDisputeOutcomeRejectedCustomerNotification(order, dispute);
                    _workflowMessageService.SendOrderDisputeOutcomeRejectedVendorNotification(order, vendor, dispute);
                    _eventPublisher.Publish(new OrderDisputeSettlementOutcomeEvent(dispute, groupReturnRequest));
                }

                _returnRequestService.UpdateDispute(dispute);

                _orderPayoutService.GenerateOrderPayoutRequest(actorId, DateTime.UtcNow, orderItem.OrderId);

                return continueEditing ? RedirectToAction("EditDispute", new { id = model.Id }) : RedirectToAction("List");
            }

            model = _returnRequestModelFactory.PrepareDisputeSubmitModel(null, disputeList.Id);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult SellerDisputePictureList(SellerDisputePictureSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedDataTablesJson();

            var model = _disputeModelFactory.PrepareSellerDisputePictureListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult SellerDisputePictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute, int groupId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            if (_returnRequestService.GetSellerDisputePictureByGroupReturnRequestId(groupId).Any(p => p.PictureId == pictureId))
                return Json(new { Result = false });

            //try to get a picture with the specified id
            var picture = _pictureService.GetPictureById(pictureId)
                ?? throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _returnRequestService.InsertSellerDisputePicture(new SellerDisputePicture
            {
                PictureId = pictureId,
                GroupReturnRequestId = groupId,
                DisplayOrder = displayOrder
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult SellerDisputePictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product picture with the specified id
            var disputePicture = _returnRequestService.GetSellerDisputePictureById(id)
                ?? throw new ArgumentException("No picture found with the specified id");

            var pictureId = disputePicture.PictureId;
            _returnRequestService.DeleteSellerDisputePicture(disputePicture);

            //try to get a picture with the specified id
            //var picture = _pictureService.GetPictureById(pictureId)
            //    ?? throw new ArgumentException("No picture found with the specified id");

            //_pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }
    }
}