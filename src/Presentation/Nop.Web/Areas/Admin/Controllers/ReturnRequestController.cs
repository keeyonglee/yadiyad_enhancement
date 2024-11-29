using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Vendors;
using Nop.Services.ShippingShuq;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.ShuqOrders;
using Nop.Services.Directory;
using Nop.Services.Payout;
using Nop.Core;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ReturnRequestController : BaseAdminController
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
        private readonly IVendorService _vendorService;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly IProductService _productService;
        private readonly IAddressService _addressService;
        private readonly ShipmentCarrierResolver _shipmentCarrierResolver;
        private readonly IShipmentService _shipmentService;
        private readonly ShippingMethodService _shippingMethodService;
        private readonly Web.Factories.IVendorModelFactory _vendorModelFactory;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly OrderPayoutService _orderPayoutService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public ReturnRequestController(ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IReturnRequestModelFactory returnRequestModelFactory,
            IReturnRequestService returnRequestService,
            IProductService productService,
            IAddressService addressService,
            IWorkflowMessageService workflowMessageService,
            ShipmentCarrierResolver shipmentCarrierResolver,
            IShipmentService shipmentService,
            ShippingMethodService shippingMethodService,
            Web.Factories.IVendorModelFactory vendorModelFactory,
            IVendorService vendorService,
            IShuqOrderProcessingService orderProcessingService,
            IStateProvinceService stateProvinceService,
            OrderPayoutService orderPayoutService,
            IWorkContext workContext)
        {
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _returnRequestService = returnRequestService;
            _productService = productService;
            _addressService = addressService;
            _workflowMessageService = workflowMessageService;
            _vendorService = vendorService;
            _orderProcessingService = orderProcessingService;
            _shipmentCarrierResolver = shipmentCarrierResolver;
            _shipmentService = shipmentService;
            _shippingMethodService = shippingMethodService;
            _vendorModelFactory = vendorModelFactory;
            _stateProvinceService = stateProvinceService;
            _orderPayoutService = orderPayoutService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        protected virtual void UpdateLocales(ReturnRequestReason rrr, ReturnRequestReasonModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(rrr,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        protected virtual void UpdateLocales(ReturnRequestAction rra, ReturnRequestActionModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(rra,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestSearchModel(new ReturnRequestSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ReturnRequestSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _returnRequestModelFactory.PrepareGroupReturnRequestListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ReturnOrderList(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnOrderModel(null, id);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult EditReturnOrder(ReturnOrderModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var rOrder = _returnRequestModelFactory.PrepareReturnOrderModel(null, model.Id);

            var order = _orderService.GetOrderById(rOrder.ReturnList[0].OrderId);
            var orderItems = _orderService.GetOrderItems(order.Id, isShipEnabled: true);
            var returnOrder = _returnRequestService.GetReturnOrderByGroupReturnRequestId(model.Id);

            if (returnOrder == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                if (model.IsShipped)
                {
                    var shipment = new Shipment
                    {
                        OrderId = order.Id,
                        ReturnOrderId = rOrder.Id,
                        CreatedOnUtc = DateTime.Now
                    };

                    var shipmentItems = new List<ShipmentItem>();

                    decimal totalWeight = 0;
                    decimal totalCost = 0;
                    int totalQuantity = 0;

                    foreach (var orderItem in orderItems)
                    {
                        var product = _productService.GetProductById(orderItem.ProductId);
                        //ensure that this product can be shipped (have at least one item to ship)
                        var maxQtyToAdd = _orderService.GetTotalNumberOfItemsCanBeAddedToShipment(orderItem);

                        var qtyToAdd = orderItem.Quantity;

                        var orderItemTotalWeight = product.Weight / 1000 * qtyToAdd;

                        totalWeight += orderItemTotalWeight;
                        totalCost += orderItem.PriceInclTax * qtyToAdd;
                        totalQuantity += qtyToAdd;
                        //create a shipment item
                        shipmentItems.Add(new ShipmentItem
                        {
                            OrderItemId = orderItem.Id,
                            Quantity = qtyToAdd
                        });
                    }

                    var shippingAddress = new Address();
                    if (order.ShippingAddressId != null)
                    {
                        shippingAddress = _addressService.GetAddressById(order.ShippingAddressId.Value)
                        ?? throw new ArgumentException("No address found with the buyer address id", nameof(order.ShippingAddressId.Value));
                    }
                    else
                    {
                        throw new ArgumentException("Buyer address id is null", nameof(order.ShippingAddressId.Value));
                    }
                    var shippingState = "";
                    if (shippingAddress != null && shippingAddress.StateProvinceId != null)
                    {
                        shippingState = _stateProvinceService.GetStateProvinceById(shippingAddress.StateProvinceId.Value).Name;
                    }

                    var shippingProduct = _productService.GetProductById(orderItems[0].ProductId);
                    var vendor = _vendorService.GetVendorById(shippingProduct.VendorId);
                    var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
                    var vendorState = "";
                    if (vendorAddress != null && vendorAddress.StateProvinceId != null)
                    {
                        vendorState = _stateProvinceService.GetStateProvinceById(vendorAddress.StateProvinceId.Value).Name;
                    }

                    var shipmentCarrier = new ShipmentCarrierDTO
                    {
                        OrderId = order.Id,
                        SenderName = $"{shippingAddress.FirstName} {shippingAddress.LastName}",
                        SenderAddress = $"{shippingAddress.Address1} {shippingAddress.Address2}",
                        SenderPhone = shippingAddress.PhoneNumber,
                        SenderZip = shippingAddress.ZipPostalCode,
                        SenderAddress1 = shippingAddress.Address1,
                        SenderAddress2 = shippingAddress.Address2,
                        SenderCity = shippingAddress.City,
                        SenderState = shippingState,
                        SenderCompanyName = string.Empty,
                        ReceiverName = $"{vendorAddress.FirstName} {vendorAddress.LastName}",
                        ReceiverAddress = $"{vendorAddress.Address1} {vendorAddress.Address2}",
                        ReceiverZip = vendorAddress.ZipPostalCode,
                        ReceiverAddress1 = vendorAddress.Address1,
                        ReceiverAddress2 = vendorAddress.Address2,
                        ReceiverCity = vendorAddress.City,
                        ReceiverState = vendorState,
                        ReceiverPhone = vendorAddress.PhoneNumber,
                        TotalWeight = totalWeight,
                        ProductName = shippingProduct.Name,
                        Remarks = string.Empty,
                        TotalValue = totalCost,
                        RequireInsurance = false,
                        Quantity = totalQuantity
                    };

                    //Ship via jnt
                    var shippingCarrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
                    if (shippingCarrier == null)
                    {
                        throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
                    }
                    var shippingCarrierReceipt = shippingCarrier.Ship(shipmentCarrier);

                    if (shippingCarrierReceipt != null)
                    {
                        shipment.TrackingNumber = shippingCarrierReceipt.TrackingNumber;
                        shipment.Insurance = shippingCarrierReceipt.InsuranceFeeRoundUp;
                        shipment.Type = 2;
                        shipment.TotalWeight = totalWeight;
                        shipment.ShippedDateUtc = DateTime.Now;
                        shipment.ShippingMethodId = _shippingMethodService.GetShippingMethodByName(shippingCarrierReceipt.ShippingMethod).Id;
                        var existingShipment = _shipmentService.GetShipmentsByReturnOrderId(rOrder.Id);
                        if (existingShipment.Count == 0)
                        {
                            _shipmentService.InsertShipment(shipment);

                            foreach (var shipmentItem in shipmentItems)
                            {
                                shipmentItem.ShipmentId = shipment.Id;
                                _shipmentService.InsertShipmentItem(shipmentItem);
                            }
                        }
                    }
                }

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareReturnOrderModel(model, model.Id);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GetReturnRequestDetails(ReturnRequestItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a group return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(searchModel.GroupReturnRequestId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult EditGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestModel(null, returnRequest[0]);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult EditGroup(GroupReturnRequestModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var actorId = _workContext.CurrentCustomer.Id;

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetGroupReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            var returnRequestList = _returnRequestService.GetReturnRequestByGroupReturnRequestId(model.Id);
            var order = _orderService.GetOrderByOrderItem(returnRequestList[0].OrderItemId);
            if (ModelState.IsValid)
            {
                returnRequest.IsInsuranceClaim = model.IsInsuranceClaim;
                returnRequest.NeedReturnShipping = model.NeedReturnShipping;

                if (returnRequest.ApproveStatusId == (int)ApproveStatusEnum.Pending)
                {
                    if (model.ApproveStatusId == (int)ApproveStatusEnum.Approved)
                    {
                        _orderProcessingService.ApproveReturnRequest(returnRequest);
                    }
                    else if (model.ApproveStatusId == (int)ApproveStatusEnum.Pending)
                    {
                        returnRequest.ApproveStatusId = model.ApproveStatusId;
                        _returnRequestService.UpdateGroupReturnRequest(returnRequest);
                    }
                    else
                    {
                        _returnRequestService.UpdateGroupReturnRequest(returnRequest);
                        _returnRequestService.RaiseDisputeOnDefect(returnRequest.Id, order.Id, DisputeTypeEnum.RejectRefund);
                    }
                }


                if (model.ReturnConditionId == (int)ReturnConditionEnum.Mint)
                {
                    //returnRequest.NeedReturnShipping = true;
                    returnRequest.ReturnConditionId = model.ReturnConditionId;
                    _workflowMessageService.SendOrderRefundApproveCustomerNotification(order);

                    _returnRequestService.ProcessRefund(returnRequest);
                }
                else if (model.ReturnConditionId == (int)ReturnConditionEnum.Defect)
                {
                    //returnRequest.NeedReturnShipping = true;
                    returnRequest.ReturnConditionId = model.ReturnConditionId;
                    _returnRequestService.UpdateGroupReturnRequest(returnRequest);
                    _returnRequestService.RaiseDisputeOnDefect(returnRequest.Id, order.Id, DisputeTypeEnum.DefectReturn);

                }
                    

                //activity log
                _customerActivityService.InsertActivity("EditReturnRequest",
                    string.Format(_localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id), returnRequest);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));

                _orderPayoutService.GenerateOrderPayoutRequest(actorId, DateTime.UtcNow, order.Id);

                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareReturnRequestModel(model, returnRequestList[0], true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult EditDispute(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            //prepare model
            var model = _returnRequestModelFactory.PrepareSellerDisputeModel(null, returnRequest[0]);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult EditDispute(SellerDisputeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestById(model.Id);
            var dispute = new Dispute();
            var disputeList = _returnRequestService.GetDisputeByGroupReturnRequestId(model.Id);
            var order = _orderService.GetOrderByOrderItem(returnRequest[0].OrderItemId);
            var orderItem = _orderService.GetOrderItemById(returnRequest[0].OrderItemId);
            var vendor = _vendorService.GetVendorByProductId(orderItem.ProductId);

            if (ModelState.IsValid)
            {
                if (disputeList.Count > 0)
                {
                    dispute.Id = disputeList[0].Id;
                    dispute.CreatedOnUtc = disputeList[0].CreatedOnUtc;
                }

                dispute.GroupReturnRequestId = model.Id;
                dispute.DisputeReasonId = model.DisputeReasonId;
                dispute.DisputeDetail = model.DisputeDetail;
                dispute.DisputeAction = (int)DisputeActionEnum.Pending;
                dispute.OrderId = order.Id;
                dispute.VendorId = vendor.Id;
                dispute.CreatedOnUtc = DateTime.UtcNow;
                dispute.UpdatedOnUtc = DateTime.UtcNow;

                //reset moderator dispute action for 2nd dispute by seller
                if (groupReturnRequest.ReturnConditionId == (int)ReturnConditionEnum.Mint)
                    _returnRequestService.ResetDisputeAction(dispute);

                if (disputeList.Count > 0)
                {
                    _returnRequestService.UpdateDispute(dispute);
                    _returnRequestService.UpdateApproveStatusToDispute(groupReturnRequest.Id); 
                }
                
                else
                {
                    _returnRequestService.InsertDispute(dispute);
                    _returnRequestService.UpdateApproveStatusToDispute(groupReturnRequest.Id);
                }
                    

                return continueEditing ? RedirectToAction("Edit", new { id = model.Id }) : RedirectToAction("List");
            }

            model = _returnRequestModelFactory.PrepareSellerDisputeModel(null, returnRequest[0]);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            //prepare model
            var model = _returnRequestModelFactory.PrepareSingleReturnRequestModel(null, returnRequest);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(ReturnRequestModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var groupId = returnRequest.GroupReturnRequestId;
                returnRequest = model.ToEntity(returnRequest);
                returnRequest.UpdatedOnUtc = DateTime.UtcNow;
                returnRequest.GroupReturnRequestId = groupId;

                _returnRequestService.UpdateReturnRequest(returnRequest);

                //activity log
                _customerActivityService.InsertActivity("EditReturnRequest",
                    string.Format(_localizationService.GetResource("ActivityLog.EditReturnRequest"), returnRequest.Id), returnRequest);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = returnRequest.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareSingleReturnRequestModel(model, returnRequest, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("notify-customer")]
        public virtual IActionResult NotifyCustomer(ReturnRequestModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(model.Id);
            if (returnRequest == null)
                return RedirectToAction("List");

            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem is null)
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.ReturnRequests.OrderItemDeleted"));
                return RedirectToAction("Edit", new { id = returnRequest.Id });
            }

            var order = _orderService.GetOrderById(orderItem.OrderId);

            var queuedEmailIds = _workflowMessageService.SendReturnRequestStatusChangedCustomerNotification(returnRequest, orderItem, order);
            if (queuedEmailIds.Any())
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Notified"));

            return RedirectToAction("Edit", new { id = returnRequest.Id });
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReturnRequests))
                return AccessDeniedView();

            //try to get a return request with the specified id
            var returnRequest = _returnRequestService.GetReturnRequestById(id);
            if (returnRequest == null)
                return RedirectToAction("List");

            _returnRequestService.DeleteReturnRequest(returnRequest);

            //activity log
            _customerActivityService.InsertActivity("DeleteReturnRequest",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteReturnRequest"), returnRequest.Id), returnRequest);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ReturnRequests.Deleted"));

            return RedirectToAction("List");
        }

        #region Return request reasons

        public virtual IActionResult ReturnRequestReasonList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate panel
            SaveSelectedPanelName("ordersettings-return-request");

            //we just redirect a user to the order settings page
            return RedirectToAction("Order", "Setting");
        }

        [HttpPost]
        public virtual IActionResult ReturnRequestReasonList(ReturnRequestReasonSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestReasonListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ReturnRequestReasonCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestReasonModel(new ReturnRequestReasonModel(), null);
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ReturnRequestReasonCreate(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnRequestReason = model.ToEntity<ReturnRequestReason>();
                _returnRequestService.InsertReturnRequestReason(returnRequestReason);

                //locales
                UpdateLocales(returnRequestReason, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Added"));

                return continueEditing 
                    ? RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id })
                    : RedirectToAction("ReturnRequestReasonList");
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareReturnRequestReasonModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ReturnRequestReasonEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = _returnRequestService.GetReturnRequestReasonById(id);
            if (returnRequestReason == null)
                return RedirectToAction("ReturnRequestReasonList");

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestReasonModel(null, returnRequestReason);
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ReturnRequestReasonEdit(ReturnRequestReasonModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = _returnRequestService.GetReturnRequestReasonById(model.Id);
            if (returnRequestReason == null)
                return RedirectToAction("ReturnRequestReasonList");

            if (ModelState.IsValid)
            {
                returnRequestReason = model.ToEntity(returnRequestReason);
                _returnRequestService.UpdateReturnRequestReason(returnRequestReason);

                //locales
                UpdateLocales(returnRequestReason, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Updated"));

                if (!continueEditing)
                    return RedirectToAction("ReturnRequestReasonList");
                
                return RedirectToAction("ReturnRequestReasonEdit", new { id = returnRequestReason.Id });
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareReturnRequestReasonModel(model, returnRequestReason, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ReturnRequestReasonDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request reason with the specified id
            var returnRequestReason = _returnRequestService.GetReturnRequestReasonById(id) 
                ?? throw new ArgumentException("No return request reason found with the specified id", nameof(id));

            _returnRequestService.DeleteReturnRequestReason(returnRequestReason);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestReasons.Deleted"));

            return RedirectToAction("ReturnRequestReasonList");
        }

        #endregion

        #region Return request actions

        public virtual IActionResult ReturnRequestActionList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate panel
            SaveSelectedPanelName("ordersettings-return-request");

            //we just redirect a user to the order settings page
            return RedirectToAction("Order", "Setting");
        }

        [HttpPost]
        public virtual IActionResult ReturnRequestActionList(ReturnRequestActionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestActionListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ReturnRequestActionCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestActionModel(new ReturnRequestActionModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ReturnRequestActionCreate(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var returnRequestAction = model.ToEntity<ReturnRequestAction>();
                _returnRequestService.InsertReturnRequestAction(returnRequestAction);

                //locales
                UpdateLocales(returnRequestAction, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Added"));

                return continueEditing 
                    ? RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id }) 
                    : RedirectToAction("ReturnRequestActionList");
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareReturnRequestActionModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual IActionResult ReturnRequestActionEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = _returnRequestService.GetReturnRequestActionById(id);
            if (returnRequestAction == null)
                return RedirectToAction("ReturnRequestActionList");

            //prepare model
            var model = _returnRequestModelFactory.PrepareReturnRequestActionModel(null, returnRequestAction);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ReturnRequestActionEdit(ReturnRequestActionModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = _returnRequestService.GetReturnRequestActionById(model.Id);
            if (returnRequestAction == null)
                return RedirectToAction("ReturnRequestActionList");

            if (ModelState.IsValid)
            {
                returnRequestAction = model.ToEntity(returnRequestAction);
                _returnRequestService.UpdateReturnRequestAction(returnRequestAction);

                //locales
                UpdateLocales(returnRequestAction, model);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Updated"));

                if (!continueEditing)
                    return RedirectToAction("ReturnRequestActionList");
                
                return RedirectToAction("ReturnRequestActionEdit", new { id = returnRequestAction.Id });
            }

            //prepare model
            model = _returnRequestModelFactory.PrepareReturnRequestActionModel(model, returnRequestAction, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ReturnRequestActionDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a return request action with the specified id
            var returnRequestAction = _returnRequestService.GetReturnRequestActionById(id)
                ?? throw new ArgumentException("No return request action found with the specified id", nameof(id));

            _returnRequestService.DeleteReturnRequestAction(returnRequestAction);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Settings.Order.ReturnRequestActions.Deleted"));

            return RedirectToAction("ReturnRequestActionList");
        }

        #endregion

        #endregion
    }
}