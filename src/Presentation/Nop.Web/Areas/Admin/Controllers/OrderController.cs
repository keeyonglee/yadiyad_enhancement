﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.Admin.Models.Reports;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Utilities;
using YadiYad.Pro.Services.Individual;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.ShippingShuq;
using Nop.Web.Models.Vendors;
using NetBarcode;
using Nop.Services.Vendors.Dashboard;
using Nop.Core.Domain.Vendors;
using Nop.Services.ShuqOrders;
using Nop.Services.Directory;
using YadiYad.Pro.Services.Payout;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.ShippingShuq;
using Nop.Services.Payout;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class OrderController : BaseAdminController
    {
        #region Fields

        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressService _addressService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IEncryptionService _encryptionService;
        private readonly IExportManager _exportManager;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly Factories.IOrderModelFactory _orderModelFactory;
        private readonly IShuqOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IPdfService _pdfService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductService _productService;
        private readonly IShipmentService _shipmentService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IVendorService _vendorService;
        private readonly OrderSettings _orderSettings;
        private readonly IndividualProfileService _individualProfileService;
        private readonly ShipmentCarrierResolver _shipmentCarrierResolver;
        private readonly Web.Factories.IVendorModelFactory _vendorModelFactory;
        private readonly ShippingMethodService _shippingMethodService;
        private readonly IVendorDashboardService _vendorDashboardService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IShipmentProcessor _shipmentProcessor;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly PayoutBatchService _payoutBatchService;
        private readonly OrderPayoutService _orderPayoutService;
        private readonly ShippingBorzoSettings _shippingBorzoSettings;

        #endregion

        #region Ctor

        public OrderController(IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IEncryptionService encryptionService,
            IExportManager exportManager,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            Factories.IOrderModelFactory orderModelFactory,
            IShuqOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            IVendorService vendorService,
            OrderSettings orderSettings,
            IndividualProfileService individualProfileService,
            ShipmentCarrierResolver shipmentCarrierResolver,
            Web.Factories.IVendorModelFactory vendorModelFactory,
            ShippingMethodService shippingMethodService,
            IVendorDashboardService vendorDashboardService,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            IShipmentProcessor shipmentProcessor,
            IStateProvinceService stateProvinceService,
            IShuqOrderService shuqOrderService,
            PayoutBatchService payoutBatchService,
            OrderPayoutService orderPayoutService,
            ShippingBorzoSettings shippingBorzoSettings)
        {
            _addressAttributeParser = addressAttributeParser;
            _addressService = addressService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _encryptionService = encryptionService;
            _exportManager = exportManager;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderModelFactory = orderModelFactory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentService = paymentService;
            _pdfService = pdfService;
            _permissionService = permissionService;
            _priceCalculationService = priceCalculationService;
            _productAttributeFormatter = productAttributeFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _shipmentService = shipmentService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _orderSettings = orderSettings;
            _individualProfileService = individualProfileService;
            _vendorService = vendorService;
            _shipmentCarrierResolver = shipmentCarrierResolver;
            _vendorModelFactory = vendorModelFactory;
            _shippingMethodService = shippingMethodService;
            _vendorDashboardService = vendorDashboardService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _shipmentProcessor = shipmentProcessor;
            _stateProvinceService = stateProvinceService;
            _shuqOrderService = shuqOrderService;
            _payoutBatchService = payoutBatchService;
            _orderPayoutService = orderPayoutService;
            _shippingBorzoSettings = shippingBorzoSettings;
        }

        #endregion

        #region Utilities

        protected virtual bool HasAccessToOrder(Order order)
        {
            return order != null && HasAccessToOrder(order.Id);
        }

        protected virtual bool HasAccessToOrder(int orderId)
        {
            if (orderId == 0)
                return false;

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            var hasVendorProducts = _orderService.GetOrderItems(orderId, vendorId: vendorId).Any();

            return hasVendorProducts;
        }
        
        protected virtual bool HasAccessToProduct(OrderItem orderItem)
        {
            if (orderItem == null || orderItem.ProductId == 0)
                return false;

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;

            return _productService.GetProductById(orderItem.ProductId)?.VendorId == vendorId;
        }
        
        protected virtual bool HasAccessToShipment(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            return HasAccessToOrder(shipment.OrderId);
        }

        protected virtual void LogEditOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            _customerActivityService.InsertActivity("EditOrder",
                string.Format(_localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber), order);
        }

        protected virtual string AddGiftCards(IFormCollection form, Product product, string attributesXml,
           out string recipientName, out string recipientEmail, out string senderName, out string senderEmail,
           out string giftCardMessage)
        {
            recipientName = string.Empty;
            recipientEmail = string.Empty;
            senderName = string.Empty;
            senderEmail = string.Empty;
            giftCardMessage = string.Empty;

            if (!product.IsGiftCard)
                return attributesXml;

            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals("giftcard.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }

                if (formKey.Equals("giftcard.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }

                if (formKey.Equals("giftcard.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }

                if (formKey.Equals("giftcard.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }

                if (formKey.Equals("giftcard.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);

            return attributesXml;
        }

        protected virtual bool GetVendorRequireCheckoutDateTime(Vendor vendor)
        {
            var shippingCarrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
            if (shippingCarrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            return shippingCarrier.RequireCheckoutDeliveryDateAndTimeslot;
        }

        protected virtual bool CheckIsVendor(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            return _customerService.IsVendor(customer);
        }

        #endregion

        #region Order list

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _orderModelFactory.PrepareOrderSearchModel(new OrderSearchModel
            {
                OrderStatusIds = orderStatuses,
                PaymentStatusIds = paymentStatuses,
                ShippingStatusIds = shippingStatuses
            });

            var isVendor = CheckIsVendor(_workContext.CurrentCustomer.Id);
            if (isVendor)
            {
                var vendorId = _workContext.CurrentVendor.Id;
                var vendor = _vendorService.GetVendorById(vendorId);
                model.RequireCheckoutDeliveryDateAndTimeslot = GetVendorRequireCheckoutDateTime(vendor);
            }
            else
            {
                model.RequireCheckoutDeliveryDateAndTimeslot = false;
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OrderList(OrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            var isVendor = CheckIsVendor(_workContext.CurrentCustomer.Id);

            if (isVendor)
            {
                var vendorId = _workContext.CurrentVendor.Id;
                var vendor = _vendorService.GetVendorById(vendorId);
                searchModel.RequireCheckoutDeliveryDateAndTimeslot = GetVendorRequireCheckoutDateTime(vendor);
                searchModel.IsLoggedInAsVendor = true;
            }
            else
            {
                searchModel.RequireCheckoutDeliveryDateAndTimeslot = false;
            }
         
            //prepare model
            var model = _orderModelFactory.PrepareOrderListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult OrderPendingShipment(OrderSearchModel searchModel)
        {
            var model = _orderModelFactory.PrepareVendorOrderShipmentModel(searchModel);

            return Json(model);
        }        
        [HttpPost]
        public virtual IActionResult OrderNeedReturnAction(OrderSearchModel searchModel)
        {
            var model = _orderModelFactory.PrepareVendorOrderReturnModel(searchModel);

            return Json(model);
        }
        [HttpPost]
        public virtual IActionResult TopSoldProducts(OrderSearchModel searchModel)
        {
            var model = _orderModelFactory.PrepareVendorTopSoldProductsModel(searchModel);

            return Json(model);
        }
        [HttpPost]
        public virtual IActionResult TopReturnedProducts(OrderSearchModel searchModel)
        {
            var model = _orderModelFactory.PrepareVendorTopReturnedProductsModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ReportAggregates(OrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _orderModelFactory.PrepareOrderAggregatorModel(searchModel);

            return Json(model);
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("go-to-order-by-number")]
        public virtual IActionResult GoToOrderId(OrderSearchModel model)
        {
            var order = _orderService.GetOrderByCustomOrderNumber(model.GoDirectlyToCustomOrderNumber);

            if (order == null)
                return List();

            return RedirectToAction("Edit", "Order", new { id = order.Id });
        }

        #endregion

        #region Export / Import

        [HttpPost, ActionName("ExportXml")]
        [FormValueRequired("exportxml-all")]
        public virtual IActionResult ExportXmlAll(OrderSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: model.BillingPhone,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            //ensure that we at least one order selected
            if (!orders.Any())
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                var xml = _exportManager.ExportOrdersToXml(orders);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportXmlSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids).Where(HasAccessToOrder));
            }

            try
            {
                var xml = _exportManager.ExportOrdersToXml(orders);
                return File(Encoding.UTF8.GetBytes(xml), MimeTypes.ApplicationXml, "orders.xml");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("ExportExcel")]
        [FormValueRequired("exportexcel-all")]
        public virtual IActionResult ExportExcelAll(OrderSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: model.BillingPhone,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            //ensure that we at least one order selected
            if (!orders.Any())
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                var bytes = _exportManager.ExportOrdersToXlsx(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult ExportExcelSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids).Where(HasAccessToOrder));
            }

            try
            {
                var bytes = _exportManager.ExportOrdersToXlsx(orders);
                return File(bytes, MimeTypes.TextXlsx, "orders.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Order details

        #region Payments and other order workflow

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("cancelorder")]
        public virtual IActionResult CancelOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                _orderProcessingService.CancelOrder(order, true);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("captureorder")]
        public virtual IActionResult CaptureOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var errors = _orderProcessingService.Capture(order);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("markorderaspaid")]
        public virtual IActionResult MarkOrderAsPaid(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                _orderProcessingService.MarkOrderAsPaid(order);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorder")]
        public virtual IActionResult RefundOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var errors = _orderProcessingService.Refund(order);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("refundorderoffline")]
        public virtual IActionResult RefundOrderOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                _orderProcessingService.RefundOffline(order);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorder")]
        public virtual IActionResult VoidOrder(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var errors = _orderProcessingService.Void(order);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("voidorderoffline")]
        public virtual IActionResult VoidOrderOffline(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                _orderProcessingService.VoidOffline(order);
                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        public virtual IActionResult PartiallyRefundOrderPopup(int id, bool online)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            //prepare model
            var model = _orderModelFactory.PrepareOrderModel(null, order);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("partialrefundorder")]
        public virtual IActionResult PartiallyRefundOrderPopup(int id, bool online, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                var amountToRefund = model.AmountToRefund;
                if (amountToRefund <= decimal.Zero)
                    throw new NopException("Enter amount to refund");

                var maxAmountToRefund = order.OrderTotal - order.RefundedAmount;
                if (amountToRefund > maxAmountToRefund)
                    amountToRefund = maxAmountToRefund;

                var errors = new List<string>();
                if (online)
                    errors = _orderProcessingService.PartiallyRefund(order, amountToRefund).ToList();
                else
                    _orderProcessingService.PartiallyRefundOffline(order, amountToRefund);

                LogEditOrder(order.Id);

                if (!errors.Any())
                {
                    //success
                    ViewBag.RefreshPage = true;

                    //prepare model
                    model = _orderModelFactory.PrepareOrderModel(model, order);

                    return View(model);
                }

                //prepare model
                model = _orderModelFactory.PrepareOrderModel(model, order);

                foreach (var error in errors)
                    _notificationService.ErrorNotification(error);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                model = _orderModelFactory.PrepareOrderModel(model, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderStatus")]
        public virtual IActionResult ChangeOrderStatus(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                order.OrderStatusId = model.OrderStatusId;
                _orderService.UpdateOrder(order);

                //add a note
                _orderService.InsertOrderNote(new OrderNote
                {
                    OrderId = order.Id,
                    Note = $"Order status has been edited. New status: {_localizationService.GetLocalizedEnum(order.OrderStatus)}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                LogEditOrder(order.Id);

                //prepare model
                model = _orderModelFactory.PrepareOrderModel(model, order);

                return View(model);
            }
            catch (Exception exc)
            {
                //prepare model
                model = _orderModelFactory.PrepareOrderModel(model, order);

                _notificationService.ErrorNotification(exc);
                return View(model);
            }
        }

        #endregion

        #region Edit, delete

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null || order.Deleted)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var shipments = _shipmentService.GetShipmentsByOrderId(order.Id);
            //prepare model
            var model = _orderModelFactory.PrepareOrderModel(null, order);
            model.ShipmentCreated = shipments.Any();

            var isVendor = CheckIsVendor(_workContext.CurrentCustomer.Id);
            if (isVendor)
            {
                var vendorId = _workContext.CurrentVendor.Id;
                var vendor = _vendorService.GetVendorById(vendorId);
                if (GetVendorRequireCheckoutDateTime(vendor))
                {
                    model.DeliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(model.CheckoutAttributesXml);
                    model.DeliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(model.CheckoutAttributesXml);
                }

                var shippingCarrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
                if (shippingCarrier == null)
                    throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
                model.SetPreparingCreateShipment = shippingCarrier.SetPreparingCreateShipment;
            }
            if (order.OrderStatusId == (int)OrderStatus.Complete)
            {
                var orderPayoutRequest = _orderPayoutService.GetOrderPayoutRequestByOrderId(order.Id);
                if (orderPayoutRequest != null && orderPayoutRequest.OrderPayoutStatusId == (int)OrderPayoutStatus.Paid)
                {
                    model.CanViewServiceChargeInvoice = true;
                }
            }
            
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("setasprepared")]
        public virtual IActionResult SetAsPrepared(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            var deliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
            var deliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);
            var deliveryScheduleAt = _shipmentService.CreateLalamoveScheduleAt(deliveryDateText, deliveryTimeText);
            _orderProcessingService.StartPreparing(order, deliveryScheduleAt);

            return RedirectToAction("List");
        }
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            _orderProcessingService.DeleteOrder(order);

            //activity log
            _customerActivityService.InsertActivity("DeleteOrder",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteOrder"), order.Id), order);

            return RedirectToAction("List");
        }

        public virtual IActionResult PdfInvoice(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                vendorId = _workContext.CurrentVendor.Id;
            }

            var order = _orderService.GetOrderById(orderId);

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintShuqVendorInvoiceToPdf(stream, order, order.CustomerLanguageId);
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
        }

        [HttpPost, ActionName("PdfInvoice")]
        [FormValueRequired("pdf-invoice-all")]
        public virtual IActionResult PdfInvoiceAll(OrderSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                model.VendorId = _workContext.CurrentVendor.Id;
            }

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = model.OrderStatusIds != null && !model.OrderStatusIds.Contains(0)
                ? model.OrderStatusIds.ToList()
                : null;
            var paymentStatusIds = model.PaymentStatusIds != null && !model.PaymentStatusIds.Contains(0)
                ? model.PaymentStatusIds.ToList()
                : null;
            var shippingStatusIds = model.ShippingStatusIds != null && !model.ShippingStatusIds.Contains(0)
                ? model.ShippingStatusIds.ToList()
                : null;

            var filterByProductId = 0;
            var product = _productService.GetProductById(model.ProductId);
            if (product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id))
                filterByProductId = model.ProductId;

            //load orders
            var orders = _orderService.SearchOrders(storeId: model.StoreId,
                vendorId: model.VendorId,
                productId: filterByProductId,
                warehouseId: model.WarehouseId,
                paymentMethodSystemName: model.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: model.BillingPhone,
                billingEmail: model.BillingEmail,
                billingLastName: model.BillingLastName,
                billingCountryId: model.BillingCountryId,
                orderNotes: model.OrderNotes);

            //ensure that we at least one order selected
            if (!orders.Any())
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.NoOrders"));
                return RedirectToAction("List");
            }

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintOrdersToPdf(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, model.VendorId);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "orders.pdf");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public virtual IActionResult PdfInvoiceSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var orders = new List<Order>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                orders.AddRange(_orderService.GetOrdersByIds(ids));
            }

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
            {
                orders = orders.Where(HasAccessToOrder).ToList();
                vendorId = _workContext.CurrentVendor.Id;
            }

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintOrdersToPdf(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, vendorId);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "orders.pdf");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        //currently we use this method on the add product to order details pages
        [HttpPost]
        public virtual IActionResult ProductDetails_AttributeChange(int productId, bool validateAttributeConditions, IFormCollection form)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            var attributeXml = _productAttributeParser.ParseProductAttributes(product, form, errors);

            //conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributeXml);
                    if (!conditionMet.HasValue)
                        continue;

                    if (conditionMet.Value)
                        enabledAttributeMappingIds.Add(attribute.Id);
                    else
                        disabledAttributeMappingIds.Add(attribute.Id);
                }
            }

            return Json(new
            {
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveCC")]
        public virtual IActionResult EditCreditCardInfo(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            if (order.AllowStoringCreditCardNumber)
            {
                var cardType = model.CardType;
                var cardName = model.CardName;
                var cardNumber = model.CardNumber;
                var cardCvv2 = model.CardCvv2;
                var cardExpirationMonth = model.CardExpirationMonth;
                var cardExpirationYear = model.CardExpirationYear;

                order.CardType = _encryptionService.EncryptText(cardType);
                order.CardName = _encryptionService.EncryptText(cardName);
                order.CardNumber = _encryptionService.EncryptText(cardNumber);
                order.MaskedCreditCardNumber = _encryptionService.EncryptText(_paymentService.GetMaskedCreditCardNumber(cardNumber));
                order.CardCvv2 = _encryptionService.EncryptText(cardCvv2);
                order.CardExpirationMonth = _encryptionService.EncryptText(cardExpirationMonth);
                order.CardExpirationYear = _encryptionService.EncryptText(cardExpirationYear);
                _orderService.UpdateOrder(order);
            }

            //add a note
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = "Credit card info has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            LogEditOrder(order.Id);

            //prepare model
            model = _orderModelFactory.PrepareOrderModel(model, order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveOrderTotals")]
        public virtual IActionResult EditOrderTotals(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            order.OrderSubtotalInclTax = model.OrderSubtotalInclTaxValue;
            order.OrderSubtotalExclTax = model.OrderSubtotalExclTaxValue;
            order.OrderSubTotalDiscountInclTax = model.OrderSubTotalDiscountInclTaxValue;
            order.OrderSubTotalDiscountExclTax = model.OrderSubTotalDiscountExclTaxValue;
            order.OrderShippingInclTax = model.OrderShippingInclTaxValue;
            order.OrderShippingExclTax = model.OrderShippingExclTaxValue;
            order.PaymentMethodAdditionalFeeInclTax = model.PaymentMethodAdditionalFeeInclTaxValue;
            order.PaymentMethodAdditionalFeeExclTax = model.PaymentMethodAdditionalFeeExclTaxValue;
            order.TaxRates = model.TaxRatesValue;
            order.OrderTax = model.TaxValue;
            order.OrderDiscount = model.OrderTotalDiscountValue;
            order.OrderTotal = model.OrderTotalValue;
            _orderService.UpdateOrder(order);

            //add a note
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order totals have been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            LogEditOrder(order.Id);

            //prepare model
            model = _orderModelFactory.PrepareOrderModel(model, order);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("save-shipping-method")]
        public virtual IActionResult EditShippingMethod(int id, OrderModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            order.ShippingMethod = model.ShippingMethod;
            _orderService.UpdateOrder(order);

            //add a note
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = "Shipping method has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            LogEditOrder(order.Id);

            //prepare model
            model = _orderModelFactory.PrepareOrderModel(model, order);

            //selected panel
            SaveSelectedPanelName("order-billing-shipping", persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnSaveOrderItem")]
        public virtual IActionResult EditOrderItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnSaveOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnSaveOrderItem".Length));

            var orderItem = _orderService.GetOrderItemById(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            if (!decimal.TryParse(form["pvUnitPriceInclTax" + orderItemId], out var unitPriceInclTax))
                unitPriceInclTax = orderItem.UnitPriceInclTax;
            if (!decimal.TryParse(form["pvUnitPriceExclTax" + orderItemId], out var unitPriceExclTax))
                unitPriceExclTax = orderItem.UnitPriceExclTax;
            if (!int.TryParse(form["pvQuantity" + orderItemId], out var quantity))
                quantity = orderItem.Quantity;
            if (!decimal.TryParse(form["pvDiscountInclTax" + orderItemId], out var discountInclTax))
                discountInclTax = orderItem.DiscountAmountInclTax;
            if (!decimal.TryParse(form["pvDiscountExclTax" + orderItemId], out var discountExclTax))
                discountExclTax = orderItem.DiscountAmountExclTax;
            if (!decimal.TryParse(form["pvPriceInclTax" + orderItemId], out var priceInclTax))
                priceInclTax = orderItem.PriceInclTax;
            if (!decimal.TryParse(form["pvPriceExclTax" + orderItemId], out var priceExclTax))
                priceExclTax = orderItem.PriceExclTax;

            var product = _productService.GetProductById(orderItem.ProductId);

            if (quantity > 0)
            {
                var qtyDifference = orderItem.Quantity - quantity;

                if (!_orderSettings.AutoUpdateOrderTotalsOnEditingOrder)
                {
                    orderItem.UnitPriceInclTax = unitPriceInclTax;
                    orderItem.UnitPriceExclTax = unitPriceExclTax;
                    orderItem.Quantity = quantity;
                    orderItem.DiscountAmountInclTax = discountInclTax;
                    orderItem.DiscountAmountExclTax = discountExclTax;
                    orderItem.PriceInclTax = priceInclTax;
                    orderItem.PriceExclTax = priceExclTax;
                    _orderService.UpdateOrderItem(orderItem);
                }

                //adjust inventory
                _productService.AdjustInventory(product, qtyDifference, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));
            }
            else
            {
                //adjust inventory
                _productService.AdjustInventory(product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                _orderService.DeleteOrderItem(orderItem);
            }

            //update order totals
            var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
            {
                PriceInclTax = unitPriceInclTax,
                PriceExclTax = unitPriceExclTax,
                DiscountAmountInclTax = discountInclTax,
                DiscountAmountExclTax = discountExclTax,
                SubTotalInclTax = priceInclTax,
                SubTotalExclTax = priceExclTax,
                Quantity = quantity
            };
            _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

            //add a note
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = "Order item has been edited",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            LogEditOrder(order.Id);

            //prepare model
            var model = _orderModelFactory.PrepareOrderModel(null, order);

            foreach (var warning in updateOrderParameters.Warnings)
                _notificationService.WarningNotification(warning);

            //selected panel
            SaveSelectedPanelName("order-products", persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnDeleteOrderItem")]
        public virtual IActionResult DeleteOrderItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id });

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnDeleteOrderItem", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnDeleteOrderItem".Length));

            var orderItem = _orderService.GetOrderItemById(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            if (_giftCardService.GetGiftCardsByPurchasedWithOrderItemId(orderItem.Id).Any())
            {
                //we cannot delete an order item with associated gift cards
                //a store owner should delete them first

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.OrderItem.DeleteAssociatedGiftCardRecordError"));

                //selected panel
                SaveSelectedPanelName("order-products", persistForTheNextRequest: false);

                return View(model);
            }
            else
            {
                var product = _productService.GetProductById(orderItem.ProductId);

                //adjust inventory
                _productService.AdjustInventory(product, orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteOrderItem"), order.Id));

                //delete item
                _orderService.DeleteOrderItem(orderItem);

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters(order, orderItem);
                _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

                //add a note
                _orderService.InsertOrderNote(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "Order item has been deleted",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                LogEditOrder(order.Id);

                //prepare model
                var model = _orderModelFactory.PrepareOrderModel(null, order);

                foreach (var warning in updateOrderParameters.Warnings)
                    _notificationService.WarningNotification(warning);

                //selected panel
                SaveSelectedPanelName("order-products", persistForTheNextRequest: false);

                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnResetDownloadCount")]
        public virtual IActionResult ResetDownloadCount(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnResetDownloadCount", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnResetDownloadCount".Length));

            var orderItem = _orderService.GetOrderItemById(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToProduct(orderItem))
                return RedirectToAction("List");

            orderItem.DownloadCount = 0;
            _orderService.UpdateOrderItem(orderItem);
            LogEditOrder(order.Id);

            //prepare model
            var model = _orderModelFactory.PrepareOrderModel(null, order);

            //selected panel
            SaveSelectedPanelName("order-products", persistForTheNextRequest: false);

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired(FormValueRequirement.StartsWith, "btnPvActivateDownload")]
        public virtual IActionResult ActivateDownloadItem(int id, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //get order item identifier
            var orderItemId = 0;
            foreach (var formValue in form.Keys)
                if (formValue.StartsWith("btnPvActivateDownload", StringComparison.InvariantCultureIgnoreCase))
                    orderItemId = Convert.ToInt32(formValue.Substring("btnPvActivateDownload".Length));

            var orderItem = _orderService.GetOrderItemById(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToProduct(orderItem))
                return RedirectToAction("List");

            orderItem.IsDownloadActivated = !orderItem.IsDownloadActivated;
            _orderService.UpdateOrderItem(orderItem);

            LogEditOrder(order.Id);

            //prepare model
            var model = _orderModelFactory.PrepareOrderModel(null, order);

            //selected panel
            SaveSelectedPanelName("order-products", persistForTheNextRequest: false);
            return View(model);
        }

        public virtual IActionResult UploadLicenseFilePopup(int id, int orderItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            //try to get an order item with the specified id
            var orderItem = _orderService.GetOrderItemById(orderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            var product = _productService.GetProductById(orderItem.ProductId)
                ?? throw new ArgumentException("No product found with the specified order item id");

            if (!product.IsDownload)
                throw new ArgumentException("Product is not downloadable");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToProduct(orderItem))
                return RedirectToAction("List");

            //prepare model
            var model = _orderModelFactory.PrepareUploadLicenseModel(new UploadLicenseModel(), order, orderItem);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("uploadlicense")]
        public virtual IActionResult UploadLicenseFilePopup(UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            var orderItem = _orderService.GetOrderItemById(model.OrderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToProduct(orderItem))
                return RedirectToAction("List");

            //attach license
            if (model.LicenseDownloadId > 0)
                orderItem.LicenseDownloadId = model.LicenseDownloadId;
            else
                orderItem.LicenseDownloadId = null;

            _orderService.UpdateOrderItem(orderItem);

            LogEditOrder(order.Id);

            //success
            ViewBag.RefreshPage = true;

            return View(model);
        }

        [HttpPost, ActionName("UploadLicenseFilePopup")]
        [FormValueRequired("deletelicense")]
        public virtual IActionResult DeleteLicenseFilePopup(UploadLicenseModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            var orderItem = _orderService.GetOrderItemById(model.OrderItemId)
                ?? throw new ArgumentException("No order item found with the specified id");

            //ensure a vendor has access only to his products 
            if (_workContext.CurrentVendor != null && !HasAccessToProduct(orderItem))
                return RedirectToAction("List");

            //attach license
            orderItem.LicenseDownloadId = null;

            _orderService.UpdateOrderItem(orderItem);

            LogEditOrder(order.Id);

            //success
            ViewBag.RefreshPage = true;

            return View(model);
        }

        public virtual IActionResult AddProductToOrder(int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //prepare model
            var model = _orderModelFactory.PrepareAddProductToOrderSearchModel(new AddProductToOrderSearchModel(), order);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddProductToOrder(AddProductToOrderSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            //prepare model
            var model = _orderModelFactory.PrepareAddProductToOrderListModel(searchModel, order);

            return Json(model);
        }

        public virtual IActionResult AddProductToOrderDetails(int orderId, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //prepare model
            var model = _orderModelFactory.PrepareAddProductToOrderModel(new AddProductToOrderModel(), order, product);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddProductToOrderDetails(int orderId, int productId, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //try to get a product with the specified id
            var product = _productService.GetProductById(productId)
                ?? throw new ArgumentException("No product found with the specified id");

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(order.CustomerId)
                ?? throw new ArgumentException("No customer found with the specified id");

            //basic properties
            decimal.TryParse(form["UnitPriceInclTax"], out var unitPriceInclTax);
            decimal.TryParse(form["UnitPriceExclTax"], out var unitPriceExclTax);
            int.TryParse(form["Quantity"], out var quantity);
            decimal.TryParse(form["SubTotalInclTax"], out var priceInclTax);
            decimal.TryParse(form["SubTotalExclTax"], out var priceExclTax);

            //warnings
            var warnings = new List<string>();

            //attributes
            var attributesXml = _productAttributeParser.ParseProductAttributes(product, form, warnings);

            //gift cards
            attributesXml = AddGiftCards(form, product, attributesXml, out var recipientName, out var recipientEmail, out var senderName, out var senderEmail, out var giftCardMessage);

            //rental product
            _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

            //warnings
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemAttributeWarnings(customer, ShoppingCartType.ShoppingCart, product, quantity, attributesXml));
            warnings.AddRange(_shoppingCartService.GetShoppingCartItemGiftCardWarnings(ShoppingCartType.ShoppingCart, product, attributesXml));
            warnings.AddRange(_shoppingCartService.GetRentalProductWarnings(product, rentalStartDate, rentalEndDate));
            if (!warnings.Any())
            {
                //no errors

                //attributes
                var attributeDescription = _productAttributeFormatter.FormatAttributes(product, attributesXml, customer);

                //weight
                var itemWeight = _shippingService.GetShoppingCartItemWeight(product, attributesXml);

                //save item
                var orderItem = new OrderItem
                {
                    OrderItemGuid = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    UnitPriceInclTax = unitPriceInclTax,
                    UnitPriceExclTax = unitPriceExclTax,
                    PriceInclTax = priceInclTax,
                    PriceExclTax = priceExclTax,
                    OriginalProductCost = _priceCalculationService.GetProductCost(product, attributesXml),
                    AttributeDescription = attributeDescription,
                    AttributesXml = attributesXml,
                    Quantity = quantity,
                    DiscountAmountInclTax = decimal.Zero,
                    DiscountAmountExclTax = decimal.Zero,
                    DownloadCount = 0,
                    IsDownloadActivated = false,
                    LicenseDownloadId = 0,
                    ItemWeight = itemWeight,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate
                };

                _orderService.InsertOrderItem(orderItem);                

                //adjust inventory
                _productService.AdjustInventory(product, -orderItem.Quantity, orderItem.AttributesXml,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.EditOrder"), order.Id));

                //update order totals
                var updateOrderParameters = new UpdateOrderParameters(order, orderItem)
                {
                    PriceInclTax = unitPriceInclTax,
                    PriceExclTax = unitPriceExclTax,
                    SubTotalInclTax = priceInclTax,
                    SubTotalExclTax = priceExclTax,
                    Quantity = quantity
                };
                _orderProcessingService.UpdateOrderTotals(updateOrderParameters);

                //add a note
                _orderService.InsertOrderNote(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A new order item has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                LogEditOrder(order.Id);

                //gift cards
                if (product.IsGiftCard)
                {
                    for (var i = 0; i < orderItem.Quantity; i++)
                    {
                        var gc = new GiftCard
                        {
                            GiftCardType = product.GiftCardType,
                            PurchasedWithOrderItemId = orderItem.Id,
                            Amount = unitPriceExclTax,
                            IsGiftCardActivated = false,
                            GiftCardCouponCode = _giftCardService.GenerateGiftCardCode(),
                            RecipientName = recipientName,
                            RecipientEmail = recipientEmail,
                            SenderName = senderName,
                            SenderEmail = senderEmail,
                            Message = giftCardMessage,
                            IsRecipientNotified = false,
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        _giftCardService.InsertGiftCard(gc);
                    }
                }

                //redirect to order details page
                foreach (var warning in updateOrderParameters.Warnings)
                    _notificationService.WarningNotification(warning);

                //selected panel
                SaveSelectedPanelName("order-products");
                return RedirectToAction("Edit", "Order", new { id = order.Id });
            }

            //prepare model
            var model = _orderModelFactory.PrepareAddProductToOrderModel(new AddProductToOrderModel(), order, product);
            model.Warnings.AddRange(warnings);

            return View(model);
        }

        #endregion

        #endregion

        #region Addresses

        public virtual IActionResult AddressEdit(int addressId, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an address with the specified id
            var address = _addressService.GetAddressById(addressId)
                ?? throw new ArgumentException("No address found with the specified id", nameof(addressId));

            //prepare model
            var model = _orderModelFactory.PrepareOrderAddressModel(new OrderAddressModel(), order, address);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressEdit(OrderAddressModel model, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = order.Id });

            //try to get an address with the specified id
            var address = _addressService.GetAddressById(model.Address.Id)
                ?? throw new ArgumentException("No address found with the specified id");

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                //add a note
                _orderService.InsertOrderNote(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "Address has been edited",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                LogEditOrder(order.Id);

                return RedirectToAction("AddressEdit", new { addressId = model.Address.Id, orderId = model.OrderId });
            }

            //prepare model
            model = _orderModelFactory.PrepareOrderAddressModel(model, order, address);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Shipments

        public virtual IActionResult ShipmentList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = _orderModelFactory.PrepareShipmentSearchModel(new ShipmentSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ShipmentListSelect(ShipmentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _orderModelFactory.PrepareShipmentListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ShipmentsByOrder(OrderShipmentSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return Content(string.Empty);

            //prepare model
            var model = _orderModelFactory.PrepareOrderShipmentListModel(searchModel, order);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ShipmentsItemsByShipmentId(ShipmentItemSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(searchModel.ShipmentId)
                ?? throw new ArgumentException("No shipment found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return Content(string.Empty);

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(shipment.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return Content(string.Empty);

            //prepare model
            searchModel.SetGridPageSize();
            var model = _orderModelFactory.PrepareShipmentItemListModel(searchModel, shipment);

            return Json(model);
        }

        public virtual IActionResult AddShipment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return RedirectToAction("List");

            var orderItems = _orderService.GetOrderItemsByOrderId(order.Id);
            var product = _orderService.GetProductByOrderItemId(orderItems[0].Id);
            var vendor = _vendorService.GetVendorById(product.VendorId);
            var businessNature = _vendorService.GetVendorAttributeValue(vendor, NopVendorDefaults.VendorBusinessNature);

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            //prepare model
            var model = _orderModelFactory.PrepareShipmentModel(new ShipmentModel(), null, order);
            model.VendorBusinessNature = businessNature.Name;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult AddShipment(ShipmentModel model, IFormCollection form, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            var orderItems = _orderService.GetOrderItems(order.Id, isShipEnabled: true);
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                orderItems = orderItems.Where(HasAccessToProduct).ToList();
            }

            var shipment = new Shipment
            {
                OrderId = order.Id,
                TrackingNumber = model.TrackingNumber,
                TotalWeight = null,
                AdminComment = model.AdminComment,
                CreatedOnUtc = DateTime.UtcNow
            };

            var shipmentItems = new List<ShipmentItem>();

            decimal? totalWeight = null;

            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);

                //ensure that this product can be shipped (have at least one item to ship)
                var maxQtyToAdd = _orderService.GetTotalNumberOfItemsCanBeAddedToShipment(orderItem);
                if (maxQtyToAdd <= 0)
                    continue;

                var qtyToAdd = 0; //parse quantity
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out qtyToAdd);
                        break;
                    }

                var warehouseId = 0;
                if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                    product.UseMultipleWarehouses)
                {
                    //multiple warehouses supported
                    //warehouse is chosen by a store owner
                    foreach (var formKey in form.Keys)
                        if (formKey.Equals($"warehouse_{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int.TryParse(form[formKey], out warehouseId);
                            break;
                        }
                }
                else
                {
                    //multiple warehouses are not supported
                    warehouseId = product.WarehouseId;
                }

                //validate quantity
                if (qtyToAdd <= 0)
                    continue;
                if (qtyToAdd > maxQtyToAdd)
                    qtyToAdd = maxQtyToAdd;

                //ok. we have at least one item. let's create a shipment (if it does not exist)

                var orderItemTotalWeight = orderItem.ItemWeight * qtyToAdd;
                if (orderItemTotalWeight.HasValue)
                {
                    if (!totalWeight.HasValue)
                        totalWeight = 0;
                    totalWeight += orderItemTotalWeight.Value;
                }

                //create a shipment item
                shipmentItems.Add(new ShipmentItem
                {
                    OrderItemId = orderItem.Id,
                    Quantity = qtyToAdd,
                    WarehouseId = warehouseId
                });

                var quantityWithReserved = _productService.GetTotalStockQuantity(product, true, warehouseId);
                var quantityTotal = _productService.GetTotalStockQuantity(product, false, warehouseId);

                //currently reserved in current stock
                var quantityReserved = quantityTotal - quantityWithReserved;

                //If the quantity of the reserve product in the warehouse does not coincide with the total quantity of goods in the basket, 
                //it is necessary to redistribute the reserve to the warehouse
                if (!(quantityReserved == qtyToAdd && quantityReserved == maxQtyToAdd))
                    _productService.BalanceInventory(product, warehouseId, qtyToAdd);
            }

            //if we have at least one item in the shipment, then save it
            if (shipmentItems.Any())
            {
                shipment.TotalWeight = totalWeight;
                _shipmentService.InsertShipment(shipment);

                foreach (var shipmentItem in shipmentItems)
                {
                    shipmentItem.ShipmentId = shipment.Id;
                    _shipmentService.InsertShipmentItem(shipmentItem);
                }

                //add a note
                _orderService.InsertOrderNote(new OrderNote
                {
                    OrderId = order.Id,
                    Note = "A shipment has been added",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                if(model.CanShip)
                    _orderProcessingService.Ship(shipment, true);

                if(model.CanShip && model.CanDeliver)
                    _orderProcessingService.Deliver(shipment, true);

                LogEditOrder(order.Id);
                
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Added"));
                return continueEditing
                        ? RedirectToAction("ShipmentDetails", new { id = shipment.Id })
                        : RedirectToAction("Edit", new { id = model.OrderId });
            }

            _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoProductsSelected"));

            return RedirectToAction("AddShipment", model);
        }

        //[HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        //[FormValueRequired("save", "save-continue")]
        //public virtual IActionResult AddShipmentShuq(ShipmentModel model, IFormCollection form, bool continueEditing)
        //{
        //    if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
        //        return AccessDeniedView();

        //    //try to get an order with the specified id
        //    var order = _orderService.GetOrderById(model.OrderId);
        //    if (order == null)
        //        return RedirectToAction("List");

        //    //a vendor should have access only to his products
        //    if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
        //        return RedirectToAction("List");

        //    var orderItems = _orderService.GetOrderItems(order.Id, isShipEnabled: true);
        //    //a vendor should have access only to his products
        //    if (_workContext.CurrentVendor != null)
        //    {
        //        orderItems = orderItems.Where(HasAccessToProduct).ToList();
        //    }

        //    var shipment = new Shipment
        //    {
        //        OrderId = order.Id,
        //        TrackingNumber = model.TrackingNumber,
        //        TotalWeight = null,
        //        AdminComment = model.AdminComment,
        //        CreatedOnUtc = DateTime.UtcNow
        //    };

        //    var shipmentItems = new List<ShipmentItem>();

        //    decimal totalWeight = 0;
        //    decimal totalCost = 0;

        //    foreach (var orderItem in orderItems)
        //    {
        //        var product = _productService.GetProductById(orderItem.ProductId);
        //        //ensure that this product can be shipped (have at least one item to ship)
        //        var maxQtyToAdd = _orderService.GetTotalNumberOfItemsCanBeAddedToShipment(orderItem);
        //        if (maxQtyToAdd <= 0)
        //            continue;

        //        var qtyToAdd = orderItem.Quantity; //parse quantity
        //        //foreach (var formKey in form.Keys)
        //        //    if (formKey.Equals($"qtyToAdd{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
        //        //    {
        //        //        int.TryParse(form[formKey], out qtyToAdd);
        //        //        break;
        //        //    }

        //        var warehouseId = 0;
        //        if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
        //            product.UseMultipleWarehouses)
        //        {
        //            //multiple warehouses supported
        //            //warehouse is chosen by a store owner
        //            foreach (var formKey in form.Keys)
        //                if (formKey.Equals($"warehouse_{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
        //                {
        //                    int.TryParse(form[formKey], out warehouseId);
        //                    break;
        //                }
        //        }
        //        else
        //        {
        //            //multiple warehouses are not supported
        //            warehouseId = product.WarehouseId;
        //        }

        //        //validate quantity
        //        if (qtyToAdd <= 0)
        //            continue;
        //        if (qtyToAdd > maxQtyToAdd)
        //            qtyToAdd = maxQtyToAdd;

        //        //ok. we have at least one item. let's create a shipment (if it does not exist)

        //        //var orderItemTotalWeight = orderItem.ItemWeight * qtyToAdd;
        //        //if (orderItemTotalWeight.HasValue)
        //        //{
        //        //    if (!totalWeight.HasValue)
        //        //        totalWeight = 0;
        //        //    totalWeight += orderItemTotalWeight.Value;
        //        //}

        //        var orderItemTotalWeight = product.Weight* qtyToAdd;

        //        totalWeight += orderItemTotalWeight;
        //        totalCost += orderItem.PriceInclTax * qtyToAdd;
        //        //create a shipment item
        //        shipmentItems.Add(new ShipmentItem
        //        {
        //            OrderItemId = orderItem.Id,
        //            Quantity = qtyToAdd,
        //            WarehouseId = warehouseId
        //        });

        //        var quantityWithReserved = _productService.GetTotalStockQuantity(product, true, warehouseId);
        //        var quantityTotal = _productService.GetTotalStockQuantity(product, false, warehouseId);

        //        //currently reserved in current stock
        //        var quantityReserved = quantityTotal - quantityWithReserved;

        //        //If the quantity of the reserve product in the warehouse does not coincide with the total quantity of goods in the basket, 
        //        //it is necessary to redistribute the reserve to the warehouse
        //        if (!(quantityReserved == qtyToAdd && quantityReserved == maxQtyToAdd))
        //            _productService.BalanceInventory(product, warehouseId, qtyToAdd);
        //    }

            

        //    //if we have at least one item in the shipment, then save it
        //    if (shipmentItems.Any()) 
        //    {
        //        //CreateOrder JNT api
        //        var shippingAddress = new Address();
        //        if (order.ShippingAddressId != null)
        //        {
        //            shippingAddress = _addressService.GetAddressById(order.ShippingAddressId.Value)
        //            ?? throw new ArgumentException("No address found with the buyer address id", nameof(order.ShippingAddressId.Value));
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Buyer address id is null", nameof(order.ShippingAddressId.Value));
        //        }

        //        var shippingProduct = _productService.GetProductById(orderItems[0].ProductId);
        //        var vendor = _vendorService.GetVendorById(shippingProduct.VendorId);
        //        var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
                
        //        var shippingDetails = new ShipmentJnt
        //        {
        //            OrderId = $"#{order.Id}",
        //            ShipperName = vendorAddress.FirstName + vendorAddress.LastName,
        //            ShipperAddress = vendorAddress.Address1 + vendorAddress.Address2,
        //            ShipperContact = vendorAddress.Company,
        //            ShipperPhone = vendorAddress.PhoneNumber,
        //            SenderZip = vendorAddress.ZipPostalCode,
        //            ReceiverName = shippingAddress.FirstName + shippingAddress.LastName,
        //            ReceiverAddress = shippingAddress.Address1 + shippingAddress.Address2,
        //            ReceiverZip = shippingAddress.ZipPostalCode,
        //            ReceiverPhone = shippingAddress.PhoneNumber,
        //            Quantity = "1",
        //            Weight = totalWeight.ToString(),
        //            ServiceType = "6",
        //            ItemName = shippingProduct.Name,
        //            GoodsDesc = model.Remarks,
        //            GoodsValue = totalCost,
        //            GoodsType = "PARCEL",
        //            PayType = "PP_PM",
        //            OfferFeeFlag = model.RequireInsurance == true ? 1 : 0,
        //        };
        //        var response = _jntService
        //            .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shippingDetails, new CancellationToken())
        //            .GetAwaiter()
        //            .GetResult();

        //        if (response.Details[0].AwbNo != "")
        //        {
        //            shipment.TotalWeight = totalWeight;
        //            shipment.TrackingNumber = response.Details[0].AwbNo;
        //            shipment.Type = 1;
        //            shipment.Insurance = response.Details[0].Data.InsuranceFee;
        //            _shipmentService.InsertShipment(shipment);

        //            foreach (var shipmentItem in shipmentItems)
        //            {
        //                shipmentItem.ShipmentId = shipment.Id;
        //                _shipmentService.InsertShipmentItem(shipmentItem);
        //            }

        //            //update estimate shipping in order
        //            order.OrderShippingExclTax = (decimal)response.Details[0].Data.Price;
        //            _orderService.UpdateOrder(order);

        //            //add a note
        //            _orderService.InsertOrderNote(new OrderNote
        //            {
        //                OrderId = order.Id,
        //                Note = "A shipment has been added",
        //                DisplayToCustomer = false,
        //                CreatedOnUtc = DateTime.UtcNow
        //            });

        //            if (model.CanShip)
        //                _orderProcessingService.Ship(shipment, true);

        //            if (model.CanShip && model.CanDeliver)
        //                _orderProcessingService.Deliver(shipment, true);

        //            LogEditOrder(order.Id);

        //            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Added"));
        //            return continueEditing
        //                    ? RedirectToAction("ShipmentDetails", new { id = shipment.Id })
        //                    : RedirectToAction("Edit", new { id = model.OrderId });
        //        }
        //        else
        //        {
        //            _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.ShippingOrderNotCreated"));
        //        }
        //    }
        //    else
        //    {
        //        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoProductsSelected"));

        //    }

        //    return RedirectToAction("AddShipment", model);
        //}

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult AddShipmentShuq(ShipmentModel model, IFormCollection form, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(model.OrderId);
            if (order == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToOrder(order))
                return RedirectToAction("List");

            //arrange shipment
            _orderProcessingService.ArrangeShipment(order, model.RequireInsurance);

            //add a note
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = "A shipment has been added",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            LogEditOrder(order.Id);
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Added"));
            return  RedirectToAction("Edit", new { id = model.OrderId });
        }

        public virtual IActionResult ShipmentDetails(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                return RedirectToAction("List");
            
            var order = _orderService.GetOrderById(shipment.OrderId);
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var vendorState = "";
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            if (_workContext.CurrentVendor != null)
            {
                var vendorAddress = _addressService.GetAddressById(_workContext.CurrentVendor.AddressId);
                vendorState = vendorAddress.StateProvinceId != null ? _stateProvinceService.GetStateProvinceById(vendorAddress.StateProvinceId.Value).Name : "";
            }

            //prepare model
            var model = _orderModelFactory.PrepareShipmentModel(null, shipment, null);

            var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
            if (carrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            model.RequireBarCode = carrier.RequireTrackingNumberBarCode;
            
            if (carrier.RequireVendorSideTracking)
            {
                model.TrackingNumberUrl = !String.IsNullOrEmpty(shipment.TrackingNumber) ? carrier.GetUrl(shipment.TrackingNumber, shipment.MarketCode) : string.Empty;
            }
            else if (carrier.RequireVendorClientSideTracking)
            {
                model.TrackingNumberUrl = !String.IsNullOrEmpty(shipment.TrackingNumber) ? carrier.GetUrl(shipment.TrackingNumber, _shippingBorzoSettings.VendorUrl) : string.Empty;
            }

            if (carrier.RequireTrackingNumberBarCode)
            {
                ViewBag.BarCode = NetBarCode.BarCodeGenerator(model.TrackingNumber);
            }

            if (model.Items.Count > 0)
            {
                var product = _productService.GetProductById(model.Items[0].ProductId);
                var businessNature = _vendorService.GetVendorAttributeValue(vendor, NopVendorDefaults.VendorBusinessNature);
                model.VendorBusinessNature = businessNature.Name;
            }

            model.ShippingStatusId = order.ShippingStatusId;
            model.RequireVendorSideTracking = carrier.RequireVendorSideTracking || carrier.RequireVendorClientSideTracking;

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteShipment(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");
            
            foreach (var shipmentItem in _shipmentService.GetShipmentItemsByShipmentId(shipment.Id))
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = _productService.GetProductById(orderItem.ProductId);

                _productService.ReverseBookedInventory(product, shipmentItem,
                    string.Format(_localizationService.GetResource("Admin.StockQuantityHistory.Messages.DeleteShipment"), shipment.OrderId));
            }

            var orderId = shipment.OrderId;
            _shipmentService.DeleteShipment(shipment);

            var order = _orderService.GetOrderById(orderId);
            order.ShippingStatusId = 20;
            _orderService.UpdateOrder(order);
            //add a note
            _orderService.InsertOrderNote(new OrderNote
            {
                OrderId = order.Id,
                Note = "A shipment has been deleted",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow
            });

            LogEditOrder(order.Id);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Orders.Shipments.Deleted"));
            return RedirectToAction("Edit", new { id = orderId });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("settrackingnumber")]
        public virtual IActionResult SetTrackingNumber(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            shipment.TrackingNumber = model.TrackingNumber;
            _shipmentService.UpdateShipment(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setadmincomment")]
        public virtual IActionResult SetShipmentAdminComment(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            shipment.AdminComment = model.AdminComment;
            _shipmentService.UpdateShipment(shipment);

            return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasshipped")]
        public virtual IActionResult SetAsShipped(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                _orderProcessingService.Ship(shipment, true);
                LogEditOrder(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("saveshippeddate")]
        public virtual IActionResult EditShippedDate(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.ShippedDateUtc.HasValue)
                {
                    throw new Exception("Enter shipped date");
                }

                shipment.ShippedDateUtc = model.ShippedDateUtc;
                _shipmentService.UpdateShipment(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("setasdelivered")]
        public virtual IActionResult SetAsDelivered(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                _orderProcessingService.Deliver(shipment, true);
                LogEditOrder(shipment.OrderId);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        [HttpPost, ActionName("ShipmentDetails")]
        [FormValueRequired("savedeliverydate")]
        public virtual IActionResult EditDeliveryDate(ShipmentModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(model.Id);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            try
            {
                if (!model.DeliveryDateUtc.HasValue)
                {
                    throw new Exception("Enter delivery date");
                }

                shipment.DeliveryDateUtc = model.DeliveryDateUtc;
                _shipmentService.UpdateShipment(shipment);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
            catch (Exception exc)
            {
                //error
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("ShipmentDetails", new { id = shipment.Id });
            }
        }

        public virtual IActionResult PdfPackagingSlip(int shipmentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a shipment with the specified id
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return RedirectToAction("List");

            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null && !HasAccessToShipment(shipment))
                return RedirectToAction("List");

            var shipments = new List<Shipment>
            {
                shipment
            };

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id);
                bytes = stream.ToArray();
            }

            return File(bytes, MimeTypes.ApplicationPdf, $"packagingslip_{shipment.Id}.pdf");
        }

        public virtual IActionResult DownloadAWB(int shipmentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment.ShippingMethodId == null)
            {
                throw new ArgumentNullException("Shipping Method Id cannot be null");
            }

            var vendor = _vendorService.GetVendorByOrderId(shipment.OrderId);
            var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
            if (carrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            
            var awbNote = carrier.GetConsignmentNote(shipment.TrackingNumber);
            return File(awbNote, MimeTypes.ApplicationPdf, $"JNT-AWB-{shipment.TrackingNumber}.pdf");
        }

        [HttpPost, ActionName("ShipmentList")]
        [FormValueRequired("exportpackagingslips-all")]
        public virtual IActionResult PdfPackagingSlipAll(ShipmentSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var startDateValue = model.StartDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            var endDateValue = model.EndDate == null ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //a vendor should have access only to his products
            var vendorId = 0;
            if (_workContext.CurrentVendor != null)
                vendorId = _workContext.CurrentVendor.Id;

            //load shipments
            var shipments = _shipmentService.GetAllShipments(vendorId: vendorId,
                warehouseId: model.WarehouseId,
                shippingCountryId: model.CountryId,
                shippingStateId: model.StateProvinceId,
                shippingCounty: model.County,
                shippingCity: model.City,
                trackingNumber: model.TrackingNumber,
                loadNotShipped: model.LoadNotShipped,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue);

            //ensure that we at least one shipment selected
            if (!shipments.Any())
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Orders.Shipments.NoShipmentsSelected"));
                return RedirectToAction("ShipmentList");
            }

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "packagingslips.pdf");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("ShipmentList");
            }
        }

        [HttpPost]
        public virtual IActionResult PdfPackagingSlipSelected(string selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                var ids = selectedIds
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();
                shipments.AddRange(_shipmentService.GetShipmentsByIds(ids));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }

            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    _pdfService.PrintPackagingSlipsToPdf(stream, shipments, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id);
                    bytes = stream.ToArray();
                }

                return File(bytes, MimeTypes.ApplicationPdf, "packagingslips.pdf");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("ShipmentList");
            }
        }

        [HttpPost]
        public virtual IActionResult SetAsShippedSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                shipments.AddRange(_shipmentService.GetShipmentsByIds(selectedIds.ToArray()));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }

            foreach (var shipment in shipments)
            {
                try
                {
                    _orderProcessingService.Ship(shipment, true);
                }
                catch
                {
                    //ignore any exception
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult SetAsDeliveredSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipments = new List<Shipment>();
            if (selectedIds != null)
            {
                shipments.AddRange(_shipmentService.GetShipmentsByIds(selectedIds.ToArray()));
            }
            //a vendor should have access only to his products
            if (_workContext.CurrentVendor != null)
            {
                shipments = shipments.Where(HasAccessToShipment).ToList();
            }

            foreach (var shipment in shipments)
            {
                try
                {
                    _orderProcessingService.Deliver(shipment, true);
                }
                catch
                {
                    //ignore any exception
                }
            }

            return Json(new { Result = true });
        }

        public virtual IActionResult RetryShipping(int shipmentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                throw new InvalidOperationException($"Invalid Shipment Id");

            var order = _orderService.GetOrderById(shipment.OrderId);
            if (order == null)
                throw new InvalidOperationException("Unable to Find Order for Shipment");

            _orderProcessingService.RetryShipping(order);

            return RedirectToAction("ShipmentDetails", new { id = shipmentId });
        }

        #endregion

        #region Order notes

        [HttpPost]
        public virtual IActionResult OrderNotesSelect(OrderNoteSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(searchModel.OrderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            //prepare model
            var model = _orderModelFactory.PrepareOrderNoteListModel(searchModel, order);

            return Json(model);
        }

        public virtual IActionResult OrderNoteAdd(int orderId, int downloadId, bool displayToCustomer, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(message))
                return ErrorJson(_localizationService.GetResource("Admin.Orders.OrderNotes.Fields.Note.Validation"));

            //try to get an order with the specified id
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
                return ErrorJson("Order cannot be loaded");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return ErrorJson("No access for vendors");

            var orderNote = new OrderNote
            {
                OrderId = order.Id,
                DisplayToCustomer = displayToCustomer,
                Note = message,
                DownloadId = downloadId,
                CreatedOnUtc = DateTime.UtcNow
            };

            _orderService.InsertOrderNote(orderNote);

            //new order notification
            if (displayToCustomer)
            {
                //email
                _workflowMessageService.SendNewOrderNoteAddedCustomerNotification(orderNote, _workContext.WorkingLanguage.Id);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult OrderNoteDelete(int id, int orderId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            _ = _orderService.GetOrderById(orderId)
                ?? throw new ArgumentException("No order found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = orderId });

            //try to get an order note with the specified id
            var orderNote = _orderService.GetOrderNoteById(id)
                ?? throw new ArgumentException("No order note found with the specified id");

            _orderService.DeleteOrderNote(orderNote);

            return new NullJsonResult();
        }

        #endregion

        #region Reports

        [HttpPost]
        public virtual IActionResult BestsellersBriefReportByQuantityList(BestsellerBriefSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _orderModelFactory.PrepareBestsellerBriefListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult BestsellersBriefReportByAmountList(BestsellerBriefSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _orderModelFactory.PrepareBestsellerBriefListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult OrderAverageReportList(OrderAverageReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            //prepare model
            var model = _orderModelFactory.PrepareOrderAverageReportListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult OrderIncompleteReportList(OrderIncompleteReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedDataTablesJson();

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            //prepare model
            var model = _orderModelFactory.PrepareOrderIncompleteReportListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult LoadOrderStatistics(string period)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return Content(string.Empty);

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var result = new List<object>();

            var nowDt = _dateTimeHelper.ConvertToUserTime(DateTime.Now);
            var timeZone = _dateTimeHelper.CurrentTimeZone;

            var culture = new CultureInfo(_workContext.WorkingLanguage.LanguageCulture);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    for (var i = 0; i <= 12; i++)
                    {
                        result.Add(new
                        {
                            date = searchYearDateUser.Date.ToString("Y", culture),
                            value = _orderService.SearchOrders(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1), timeZone),
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
                        });

                        searchYearDateUser = searchYearDateUser.AddMonths(1);
                    }

                    break;
                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    for (var i = 0; i <= 30; i++)
                    {
                        result.Add(new
                        {
                            date = searchMonthDateUser.Date.ToString("M", culture),
                            value = _orderService.SearchOrders(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1), timeZone),
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
                        });

                        searchMonthDateUser = searchMonthDateUser.AddDays(1);
                    }

                    break;
                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    for (var i = 0; i <= 7; i++)
                    {
                        result.Add(new
                        {
                            date = searchWeekDateUser.Date.ToString("d dddd", culture),
                            value = _orderService.SearchOrders(
                                createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1), timeZone),
                                pageIndex: 0,
                                pageSize: 1, getOnlyTotalCount: true).TotalCount.ToString()
                        });

                        searchWeekDateUser = searchWeekDateUser.AddDays(1);
                    }

                    break;
            }

            return Json(result);
        }

        #endregion
    }
}
