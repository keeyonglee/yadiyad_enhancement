using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;
using Nop.Services.ShuqOrders;
using Nop.Services.Vendors;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.Order;
using Nop.Web.Models.Vendors;
using Nop.Web.Utilities;
using YadiYad.Pro.Services.Payout;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the order model factory
    /// </summary>
    public partial class OrderModelFactory : IOrderModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IVendorModelFactory _vendorModelFactory;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly MediaSettings _mediaSettings;
        private readonly IShuqOrderService _shuqOrderService;
        private readonly ShipmentCarrierResolver _shipmentCarrierResolver;
        private readonly PayoutBatchService _payoutBatchService;
        private readonly ShippingJntSettings _shippingJntSettings;

        #endregion

        #region Ctor

        public OrderModelFactory(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            PdfSettings pdfSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            IReturnRequestService returnRequestService,
            IVendorModelFactory vendorModelFactory,
            IProductModelFactory productModelFactory,
            ICatalogModelFactory catalogModelFactory,
            ICheckoutAttributeService checkoutAttributeService,
            ICheckoutAttributeParser checkoutAttributeParser,
            IPictureService pictureService,
            IStaticCacheManager staticCacheManager,
            MediaSettings mediaSettings,
            IShuqOrderService shuqOrderService,
            ShipmentCarrierResolver shipmentCarrierResolver,
            PayoutBatchService payoutBatchService,
            ShippingJntSettings shippingJntSettings)
        {
            _addressSettings = addressSettings;
            _catalogSettings = catalogSettings;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _pdfSettings = pdfSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _returnRequestService = returnRequestService;
            _vendorModelFactory = vendorModelFactory;
            _productModelFactory = productModelFactory;
            _catalogModelFactory = catalogModelFactory;
            _checkoutAttributeService = checkoutAttributeService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _pictureService = pictureService;
            _staticCacheManager = staticCacheManager;
            _mediaSettings = mediaSettings;
            _shuqOrderService = shuqOrderService;
            _shipmentCarrierResolver = shipmentCarrierResolver;
            _payoutBatchService = payoutBatchService;
            _shippingJntSettings = shippingJntSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer order list model
        /// </summary>
        /// <returns>Customer order list model</returns>
        public virtual CustomerOrderListModel PrepareCustomerOrderListModel(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            int vendorId = 0,
            OrderSortBy? orderSortBy = null)
        {
            var customerId = vendorId == 0 ? _workContext.CurrentCustomer.Id : 0;

            var model = new CustomerOrderListModel();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: customerId,
                vendorId: vendorId,
                orderSortBy: orderSortBy);

            var ordersPaged = new PagedList<Order>(orders, pageIndex, pageSize);

            foreach (var order in ordersPaged)
            {
                var orderItem = _orderService.GetOrderItemsByOrderId(order.Id).FirstOrDefault();
                var product = _orderService.GetProductByOrderItemId(orderItem.Id);
                var vendor = _vendorService.GetVendorByProductId(product.Id);
                var groupReturnRequest = _returnRequestService.GetGroupReturnRequestByOrderId(order.Id).FirstOrDefault();
                var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
                if (carrier == null)
                    throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
                var shipments = _shipmentService.GetShipmentsByOrderId(order.Id);
                var vendorOrderItems = _orderService.GetOrderItemsByOrderVendorId(order.Id, vendor.Id);

                var queryModel = new CatalogPagingFilteringModel();

                var orderModel = new CustomerOrderListModel.OrderModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    OrderStatusEnum = order.OrderStatus,
                    OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                    PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus),
                    ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                    IsCancelOrderAllowed = _orderProcessingService.CanOrderCancel(order),
                    CustomOrderNumber = order.CustomOrderNumber,
                    //OrderDetails = PrepareOrderDetailsModel(order),
                    ImageModel = _productModelFactory.PrepareProductOverviewPictureModel(product, null),
                    Vendor = _catalogModelFactory.PrepareVendorModel(vendor, queryModel),
                    ShippingMethod = carrier.CompanyName,
                    IsReadyToReceive = _orderProcessingService.IsReadyToReceive((List<Shipment>)shipments, order)
                };

                orderModel.VendorOrder = new VendorOrderDetailsModel.VendorOrderModel
                {
                    VendorId = vendor.Id,
                    VendorName = vendor.Name,
                    OrderItems = PrepareOrderItemModel(vendorOrderItems)
                };

                if (carrier.RequireCheckoutDeliveryDateAndTimeslot)
                {
                    orderModel.DeliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
                    orderModel.DeliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);

                    var deliveryDateTimeUtc = _shuqOrderService.GetCheckoutDateTimeSlot(order);
                    orderModel.DeliveryDateTime = deliveryDateTimeUtc != null ? _dateTimeHelper.ConvertToUserTime(deliveryDateTimeUtc.Value, DateTimeKind.Utc) : deliveryDateTimeUtc;
                }
                else
                {
                    orderModel.ShipBeforeDate = order.PaidDateUtc?.AddDays(_shippingJntSettings.ShipBeforeDateAdvanceDay);
                }

                if (groupReturnRequest != null)
                {
                    if (groupReturnRequest.ApproveStatus == ApproveStatusEnum.Pending ||
                        groupReturnRequest.ApproveStatus == ApproveStatusEnum.InDispute ||
                        (groupReturnRequest.NeedReturnShipping == true && groupReturnRequest.ReturnConditionId == (int)ReturnConditionEnum.Pending))
                        orderModel.OrderStatus = "Return / Refund Processing";
                    else
                        orderModel.OrderStatus = "Return / Refund Completed";

                    orderModel.ReturnRequestApprovalStatus = _localizationService.GetLocalizedEnum(groupReturnRequest.ApproveStatus);
                    orderModel.ReturnRequestStatusStr = _returnRequestService.GetReturnRequestStatus(groupReturnRequest);
                }
                else
                {
                    if (order.ShippingStatusId == (int)ShippingStatus.Shipped)
                        //orderModel.OrderStatus = "Shipped";
                        orderModel.OrderStatus = ShippingStatus.Shipped.ToString();
                    else if (order.ShippingStatusId == (int)ShippingStatus.Delivered && orderModel.IsReadyToReceive)
                        //orderModel.OrderStatus = "Delivered";
                        orderModel.OrderStatus = ShippingStatus.Delivered.ToString(); 
                    else if (order.ShippingStatusId == (int)ShippingStatus.Delivered && !orderModel.IsReadyToReceive)
                        //orderModel.OrderStatus = "Completed";
                        orderModel.OrderStatus = OrderStatus.Complete.ToString();

                    orderModel.ReturnRequestApprovalStatus = _localizationService.GetLocalizedEnum(ApproveStatusEnum.Pending);
                }

                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage.Id);

                model.Orders.Add(orderModel);
            }

            //var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
            //    _workContext.CurrentCustomer.Id);
            //foreach (var recurringPayment in recurringPayments)
            //{
            //    var order = _orderService.GetOrderById(recurringPayment.InitialOrderId);

            //    var recurringPaymentModel = new CustomerOrderListModel.RecurringOrderModel
            //    {
            //        Id = recurringPayment.Id,
            //        StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
            //        CycleInfo = $"{recurringPayment.CycleLength} {_localizationService.GetLocalizedEnum(recurringPayment.CyclePeriod)}",
            //        NextPayment = _orderProcessingService.GetNextPaymentDate(recurringPayment) is DateTime nextPaymentDate ? _dateTimeHelper.ConvertToUserTime(nextPaymentDate, DateTimeKind.Utc).ToString() : "",
            //        TotalCycles = recurringPayment.TotalCycles,
            //        CyclesRemaining = _orderProcessingService.GetCyclesRemaining(recurringPayment),
            //        InitialOrderId = order.Id,
            //        InitialOrderNumber = order.CustomOrderNumber,
            //        CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
            //        CanRetryLastPayment = _orderProcessingService.CanRetryLastRecurringPayment(_workContext.CurrentCustomer, recurringPayment)
            //    };

            //    model.RecurringOrders.Add(recurringPaymentModel);
            //}

            var pagerModel = new PagerModel
            {
                PageSize = pageSize,
                TotalRecords = orders.TotalCount,
                PageIndex = pageIndex,
                ShowTotalSummary = false,
                RouteActionName = "OrderHistoryPaged",
                UseRouteLinks = true
            };

            model.PagerModel = pagerModel;

            return model;
        }

        public virtual VendorOrderDetailsModel PrepareCustomerOrderModel(int orderId)
        {
            var model = new VendorOrderDetailsModel();
            model.Order = new VendorOrderDetailsModel.OrderModel();
            var order = _orderService.GetOrderById(orderId);
            var shipment = _shipmentService.GetShipmentsByOrderId(orderId);
            var orderItem = _orderService.GetOrderItemsByOrderId(order.Id);
            var vendorIds = new List<int>();

            var orderModel = new VendorOrderDetailsModel.OrderModel
            {
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatusEnum = order.OrderStatus,
                OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus) == "Return Refund Processing" ? "Return / Refund Processing" : _localizationService.GetLocalizedEnum(order.OrderStatus),
                PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus),
                ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                IsCancelOrderAllowed = _orderProcessingService.CanOrderCancel(order),
                CustomOrderNumber = order.CustomOrderNumber,
                OrderDetails = PrepareVendorOrderDetailsModel(order)
            };

            model.Order = orderModel;

            if (shipment.Count > 0)
                orderModel.IsOrderReceiveAllowed = _orderProcessingService.IsReadyToReceive((List<Shipment>)shipment, order);
            
            foreach (var item in orderItem)
            {
                var vendor = _vendorService.GetVendorByProductId(item.ProductId);
                if (!vendorIds.Contains(vendor.Id))
                    vendorIds.Add(vendor.Id);
            }

            IList<VendorOrderDetailsModel.VendorOrderModel> vendorOrders = new List<VendorOrderDetailsModel.VendorOrderModel>();
            foreach (var vendor in vendorIds)
            {

                var vendorProfile = _vendorService.GetVendorById(vendor);
                var vendorOrderItems = _orderService.GetOrderItemsByOrderVendorId(orderId, vendorProfile.Id);
                var vendorOrderModel = new VendorOrderDetailsModel.VendorOrderModel
                {
                    VendorId = vendorProfile.Id,
                    VendorName = vendorProfile.Name,
                    OrderItems = PrepareOrderItemModel(vendorOrderItems)
                };
                var pictureSize = _mediaSettings.CategoryThumbPictureSize;

                var picture = _pictureService.GetPictureById(vendorProfile.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                    ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Vendor.ImageLinkTitleFormat"), vendorProfile.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Vendor.ImageAlternateTextFormat"), vendorProfile.Name)
                };

                vendorOrderModel.PictureModel = pictureModel;

                vendorOrders.Add(vendorOrderModel);
            }

            model.Order.Vendors = vendorOrders;

            return model;
        }

        protected virtual IList<VendorOrderDetailsModel.OrderItemModel> PrepareOrderItemModel(IList<OrderItem> orderItems)
        {
            IList<VendorOrderDetailsModel.OrderItemModel> item = new List<VendorOrderDetailsModel.OrderItemModel>();

            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);
                var vendor = _vendorService.GetVendorById(product.VendorId);
                var orderItemModel = new VendorOrderDetailsModel.OrderItemModel
                {
                    ImageModel = _productModelFactory.PrepareProductOverviewPictureModel(product, null),
                    Id = orderItem.Id,
                    OrderItemGuid = orderItem.OrderItemGuid,
                    Sku = _productService.FormatSku(product, orderItem.AttributesXml),
                    VendorName = vendor.Name,
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                    VendorId = vendor.Id,
                    HasReview = _shuqOrderService.CheckBuyerHasReviewProduct(orderItem.OrderId, product.Id),
                    UnitPrice = _priceFormatter.FormatPrice(orderItem.UnitPriceExclTax),
                    SubTotal = _priceFormatter.FormatPrice(orderItem.PriceInclTax),
                    AllowCustomerReviews = product.AllowCustomerReviews
                };
                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //downloadable products
                if (_orderService.IsDownloadAllowed(orderItem))
                    orderItemModel.DownloadId = product.DownloadId;
                if (_orderService.IsLicenseDownloadAllowed(orderItem))
                    orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;

                item.Add(orderItemModel);
            }

            return item;
        }

        public virtual VendorOrderDetailsModel.OrderDetailsModel PrepareVendorOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestByOrderId(order.Id).FirstOrDefault();
            var vendor = _vendorService.GetVendorByOrderId(order.Id);
            var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
            if (carrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            var returnOrder = new ReturnOrder();
            var returnShipment = new Shipment();

            
            
            ApproveStatusEnum approvalStatus = ApproveStatusEnum.Pending;

            if (groupReturnRequest != null)
            {
                returnOrder = _returnRequestService.GetReturnOrderByGroupReturnRequestId(groupReturnRequest.Id).FirstOrDefault();
                if (returnOrder != null)
                    returnShipment = _shipmentService.GetShipmentsByReturnOrderId(returnOrder.Id).FirstOrDefault();

                approvalStatus = groupReturnRequest.ApproveStatus;
            }

            var returnShipmentId = 0;

            if (returnShipment != null)
                returnShipmentId = returnShipment.Id;

            var model = new VendorOrderDetailsModel.OrderDetailsModel
            {
                Id = order.Id,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus) == "Return Refund Processing" ? "Return / Refund Processing" : _localizationService.GetLocalizedEnum(order.OrderStatus),
                IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
                IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                IsCancelOrderAllowed = _orderProcessingService.CanOrderCancel(order),
                PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending,
                CustomOrderNumber = order.CustomOrderNumber,
                NeedReturn = _orderProcessingService.CheckIfNeedReturn(order),
                ReturnShipmentId = returnShipmentId,
                ReturnRequestApprovalStatus = _localizationService.GetLocalizedEnum(approvalStatus),
                //shipping info
                ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                ShippingMethod = carrier.CompanyName,
                
            };

            if (carrier.RequireCheckoutDeliveryDateAndTimeslot)
            {
                model.DeliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
                model.DeliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);

                var deliveryDateTimeUtc = _shuqOrderService.GetCheckoutDateTimeSlot(order);
                model.DeliveryDateTime = deliveryDateTimeUtc != null ? _dateTimeHelper.ConvertToUserTime(deliveryDateTimeUtc.Value, DateTimeKind.Utc) : deliveryDateTimeUtc;
            }
            else
            {
                model.ShipBeforeDate = order.PaidDateUtc?.AddDays(_shippingJntSettings.ShipBeforeDateAdvanceDay);
            }

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickupInStore = order.PickupInStore;
                if (!order.PickupInStore)
                {
                    var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId ?? 0);

                    _addressModelFactory.PrepareAddressModel(model.ShippingAddress,
                        address: shippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
                }
                else if (order.PickupAddressId.HasValue && _addressService.GetAddressById(order.PickupAddressId.Value) is Address pickupAddress)
                {
                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupAddress.Address1,
                        City = pickupAddress.City,
                        County = pickupAddress.County,
                        CountryName = _countryService.GetCountryByAddress(pickupAddress)?.Name ?? string.Empty,
                        ZipPostalCode = pickupAddress.ZipPostalCode
                    };
                }

                //shipments (only already shipped)
                var shipments = _shipmentService.GetShipmentsByOrderId(order.Id).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new VendorOrderDetailsModel.OrderDetailsModel.ShipmentBriefModel
                    {
                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };
                    var shipmentService = EngineContext.Current.Resolve<ShipmentCarrierResolver>();
                    var shipmentTracker = shipmentService.ResolveByCourierSetting(vendor);
                    if (shipmentTracker != null)
                    {
                        shipmentModel.TrackingUrl = !String.IsNullOrEmpty(shipment.TrackingNumber) ? shipmentTracker.GetUrl(shipment.TrackingNumber, shipment.MarketCode) : "";
                        if (shipmentTracker.RequireTrackingNumberBarCode)
                            shipmentModel.BarCode = NetBarCode.BarCodeGenerator(shipment.TrackingNumber);
                    }

                    if (shipment.ShippedDateUtc.HasValue)
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                    if (shipment.DeliveryDateUtc.HasValue)
                        shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);

                    if (!String.IsNullOrEmpty(shipment.TrackingNumber))
                    {
                        model.TrackingNumberUrl = carrier.GetUrl(shipment.TrackingNumber, shipment.MarketCode);
                    }

                    model.Shipments.Add(shipmentModel);
                }

                model.IsReadyToReceive = _orderProcessingService.IsReadyToReceive(shipments, order);
            }

            var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

            //billing info
            _addressModelFactory.PrepareAddressModel(model.BillingAddress,
                address: billingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);

            //VAT number
            model.VatNumber = order.VatNumber;

            var languageId = _workContext.WorkingLanguage.Id;

            //payment method
            var customer = _customerService.GetCustomerById(order.CustomerId);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(order.PaymentMethodSystemName, customer, order.StoreId);
            model.PaymentMethod = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, languageId) : order.PaymentMethodSystemName;
            model.PaymentMethodStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            //tax
            var displayTax = true;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new VendorOrderDetailsModel.OrderDetailsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, languageId),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            //gift cards
            foreach (var gcuh in _giftCardService.GetGiftCardUsageHistory(order))
            {
                model.GiftCards.Add(new VendorOrderDetailsModel.OrderDetailsModel.GiftCard
                {
                    CouponCode = _giftCardService.GetGiftCardById(gcuh.GiftCardId).GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntryId.HasValue && _rewardPointService.GetRewardPointsHistoryEntryById(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            if (!String.IsNullOrWhiteSpace(order.CheckoutAttributesXml))
            {
                model.DeliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(order.CheckoutAttributesXml);
                model.DeliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(order.CheckoutAttributesXml);
            }


            //order notes
            foreach (var orderNote in _orderService.GetOrderNotesByOrderId(order.Id, true)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new VendorOrderDetailsModel.OrderDetailsModel.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = _orderService.FormatOrderNoteText(orderNote),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            //purchased products
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;

            return model;
        }

        /// <summary>
        /// Prepare the order details model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Order details model</returns>
        public virtual OrderDetailsModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestByOrderId(order.Id).FirstOrDefault();
            IList<ReturnOrder> returnOrder = new List<ReturnOrder>();
            IList<Shipment> returnShipment = new List<Shipment>();
            if (groupReturnRequest != null)
            {
                returnOrder = _returnRequestService.GetReturnOrderByGroupReturnRequestId(groupReturnRequest.Id);
                if (returnOrder.Count > 0)
                    returnShipment = _shipmentService.GetShipmentsByReturnOrderId(returnOrder[0].Id);
            }

            var returnShipmentId = 0;

            if (returnShipment.Count > 0)
                returnShipmentId = returnShipment[0].Id;

            var model = new OrderDetailsModel
            {
                Id = order.Id,
                CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus),
                OrderStatusId = order.OrderStatusId,
                IsReOrderAllowed = _orderSettings.IsReOrderAllowed,
                IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order),
                IsCancelOrderAllowed = _orderProcessingService.CanOrderCancel(order),
                PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending,
                CustomOrderNumber = order.CustomOrderNumber,
                NeedReturn = _orderProcessingService.CheckIfNeedReturn(order),
                ReturnShipmentId = returnShipmentId,
                //shipping info
                ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                CheckoutAttributeXML = order.CheckoutAttributesXml,
                OrderCancellationReasonId = order.CancellationReason,
                CancellationReasonStr = order.OrderCancellation.GetDescription()
            };

            if (order.ReceivedDateUtc != null)
                model.OrderReceived = true;

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickupInStore = order.PickupInStore;

                if (!order.PickupInStore)
                {
                    var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId ?? 0);

                    _addressModelFactory.PrepareAddressModel(model.ShippingAddress,
                        address: shippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
                }
                else if (order.PickupAddressId.HasValue && _addressService.GetAddressById(order.PickupAddressId.Value) is Address pickupAddress)
                {
                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupAddress.Address1,
                        City = pickupAddress.City,
                        County = pickupAddress.County,
                        CountryName = _countryService.GetCountryByAddress(pickupAddress)?.Name ?? string.Empty,
                        ZipPostalCode = pickupAddress.ZipPostalCode
                    };
                }

                model.ShippingMethod = order.ShippingMethod;

                //shipments (only already shipped)
                var shipments = _shipmentService.GetShipmentsByOrderId(order.Id, true).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new OrderDetailsModel.ShipmentBriefModel
                    {
                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };

                    if (shipment.ShippedDateUtc.HasValue)
                    {
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);

                    }

                    model.Shipments.Add(shipmentModel);
                }
                model.IsReadyToReceive = _orderProcessingService.IsReadyToReceive(shipments, order);
                if (groupReturnRequest != null)
                {
                    if (groupReturnRequest.ApproveStatus == ApproveStatusEnum.Pending ||
                         groupReturnRequest.ApproveStatus == ApproveStatusEnum.InDispute ||
                         (groupReturnRequest.NeedReturnShipping == true && groupReturnRequest.ReturnConditionId == (int)ReturnConditionEnum.Pending))
                        model.OrderStatus = "Return Refund Processing";
                    else
                        model.OrderStatus = "Return Refund Completed";

                }
                else
                {
                    if (order.ShippingStatusId == (int)ShippingStatus.Shipped)
                        //model.OrderStatus = "Shipped";
                        model.OrderStatus = ShippingStatus.Shipped.ToString();
                    else if (order.ShippingStatusId == (int)ShippingStatus.Delivered && model.IsReadyToReceive)
                        //model.OrderStatus = "Delivered";
                        model.OrderStatus = ShippingStatus.Delivered.ToString();
                    else if (order.ShippingStatusId == (int)ShippingStatus.Delivered && !model.IsReadyToReceive)
                        //model.OrderStatus = "Completed";
                        model.OrderStatus = OrderStatus.Complete.ToString();
                }
            }

            var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

            //billing info
            _addressModelFactory.PrepareAddressModel(model.BillingAddress,
                address: billingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings);

            //VAT number
            model.VatNumber = order.VatNumber;

            var languageId = _workContext.WorkingLanguage.Id;

            //payment method
            var customer = _customerService.GetCustomerById(order.CustomerId);
            var paymentMethod = _paymentPluginManager
                .LoadPluginBySystemName(order.PaymentMethodSystemName, customer, order.StoreId);
            model.PaymentMethod = paymentMethod != null ? _localizationService.GetLocalizedFriendlyName(paymentMethod, languageId) : order.PaymentMethodSystemName;
            model.PaymentMethodStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
            model.PaymentStatusId = order.PaymentStatusId;
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = _paymentService.DeserializeCustomValues(order);

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            //tax
            var displayTax = true;
            var displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var taxRates = _orderService.ParseTaxRates(order, order.TaxRates);
                    displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

                    foreach (var tr in taxRates)
                    {
                        model.TaxRates.Add(new OrderDetailsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, languageId),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            //gift cards
            foreach (var gcuh in _giftCardService.GetGiftCardUsageHistory(order))
            {
                model.GiftCards.Add(new OrderDetailsModel.GiftCard
                {
                    CouponCode = _giftCardService.GetGiftCardById(gcuh.GiftCardId).GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntryId.HasValue && _rewardPointService.GetRewardPointsHistoryEntryById(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
            {
                model.RedeemedRewardPoints = -redeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, languageId);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in _orderService.GetOrderNotesByOrderId(order.Id, true)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsModel.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = _orderService.FormatOrderNoteText(orderNote),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            //purchased products
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
            model.IsOrderReviewed = true;

            var orderItems = _orderService.GetOrderItems(order.Id);
            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);
                var vendor = _vendorService.GetVendorById(product.VendorId);
                var orderItemModel = new OrderDetailsModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    OrderItemGuid = orderItem.OrderItemGuid,
                    Sku = _productService.FormatSku(product, orderItem.AttributesXml),
                    VendorName = vendor.Name,
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                    VendorId = vendor.Id,
                    AllowCustomerReviews = product.AllowCustomerReviews,
                    HasReview = _shuqOrderService.CheckBuyerHasReviewProduct(orderItem.OrderId, product.Id),
                };

                if (orderItemModel.HasReview == false)
                    model.IsOrderReviewed = false;

                //picture
                var orderItemPicture = _pictureService.GetProductPicture(product, orderItem.AttributesXml);
                orderItemModel.PictureThumbnailUrl = _pictureService.GetPictureUrl(ref orderItemPicture, 75);

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(orderItemModel);

                //unit price, subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                }

                //downloadable products
                if (_orderService.IsDownloadAllowed(orderItem))
                    orderItemModel.DownloadId = product.DownloadId;
                if (_orderService.IsLicenseDownloadAllowed(orderItem))
                    orderItemModel.LicenseId = orderItem.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the shipment details model
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Shipment details model</returns>
        public virtual ShipmentDetailsModel PrepareShipmentDetailsModel(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            var order = _orderService.GetOrderById(shipment.OrderId);

            if (order == null)
                throw new Exception("order cannot be loaded");
            var model = new ShipmentDetailsModel
            {
                Id = shipment.Id
            };
            if (shipment.ShippedDateUtc.HasValue)
                model.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
            if (shipment.DeliveryDateUtc.HasValue)
                model.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);

            //tracking number and shipment information
            if (!string.IsNullOrEmpty(shipment.TrackingNumber))
            {
                model.TrackingNumber = shipment.TrackingNumber;
                var shipmentTracker = _shipmentService.GetShipmentTracker(shipment);
                if (shipmentTracker != null)
                {
                    model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
                    if (_shippingSettings.DisplayShipmentEventsToCustomers)
                    {
                        var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
                        if (shipmentEvents != null)
                            foreach (var shipmentEvent in shipmentEvents)
                            {
                                var shipmentStatusEventModel = new ShipmentDetailsModel.ShipmentStatusEventModel();
                                var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                                shipmentStatusEventModel.Country = shipmentEventCountry != null
                                    ? _localizationService.GetLocalized(shipmentEventCountry, x => x.Name) : shipmentEvent.CountryCode;
                                shipmentStatusEventModel.Date = shipmentEvent.Date;
                                shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                                shipmentStatusEventModel.Location = shipmentEvent.Location;
                                model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                            }
                    }
                }
            }

            //products in this shipment
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            foreach (var shipmentItem in _shipmentService.GetShipmentItemsByShipmentId(shipment.Id))
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var product = _productService.GetProductById(orderItem.ProductId);

                var shipmentItemModel = new ShipmentDetailsModel.ShipmentItemModel
                {
                    Id = shipmentItem.Id,
                    Sku = _productService.FormatSku(product, orderItem.AttributesXml),
                    ProductId = product.Id,
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    AttributeInfo = orderItem.AttributeDescription,
                    QuantityOrdered = orderItem.Quantity,
                    QuantityShipped = shipmentItem.Quantity,
                };
                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value) : "";
                    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(shipmentItemModel);
            }

            //order details model
            model.Order = PrepareOrderDetailsModel(order);

            return model;
        }

        /// <summary>
        /// Prepare the customer reward points model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>Customer reward points model</returns>
        public virtual CustomerRewardPointsModel PrepareCustomerRewardPoints(int? page)
        {
            //get reward points history
            var customer = _workContext.CurrentCustomer;
            var store = _storeContext.CurrentStore;
            var pageSize = _rewardPointsSettings.PageSize;
            var rewardPoints = _rewardPointService.GetRewardPointsHistory(customer.Id, store.Id, true, pageIndex: --page ?? 0, pageSize: pageSize);

            //prepare model
            var model = new CustomerRewardPointsModel
            {
                RewardPoints = rewardPoints.Select(historyEntry =>
                {
                    var activatingDate = _dateTimeHelper.ConvertToUserTime(historyEntry.CreatedOnUtc, DateTimeKind.Utc);
                    return new CustomerRewardPointsModel.RewardPointsHistoryModel
                    {
                        Points = historyEntry.Points,
                        PointsBalance = historyEntry.PointsBalance.HasValue ? historyEntry.PointsBalance.ToString()
                            : string.Format(_localizationService.GetResource("RewardPoints.ActivatedLater"), activatingDate),
                        Message = historyEntry.Message,
                        CreatedOn = activatingDate,
                        EndDate = !historyEntry.EndDateUtc.HasValue ? null :
                            (DateTime?)_dateTimeHelper.ConvertToUserTime(historyEntry.EndDateUtc.Value, DateTimeKind.Utc)
                    };
                }).ToList(),

                PagerModel = new PagerModel
                {
                    PageSize = rewardPoints.PageSize,
                    TotalRecords = rewardPoints.TotalCount,
                    PageIndex = rewardPoints.PageIndex,
                    ShowTotalSummary = true,
                    RouteActionName = "CustomerRewardPointsPaged",
                    UseRouteLinks = true,
                    RouteValues = new RewardPointsRouteValues { pageNumber = page ?? 0 }
                }
            };

            //current amount/balance
            var rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            var rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            var rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
            model.RewardPointsBalance = rewardPointsBalance;
            model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);

            //minimum amount/balance
            var minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
            var minimumRewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(minimumRewardPointsBalance);
            var minimumRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minimumRewardPointsAmountBase, _workContext.WorkingCurrency);
            model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
            model.MinimumRewardPointsAmount = _priceFormatter.FormatPrice(minimumRewardPointsAmount, true, false);

            return model;
        }

        public VendorSummaryModel PrepareVendorSummaryModel(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var model = new VendorSummaryModel();
            var vendor = _workContext.CurrentVendor;

            var orders = _orderService.SearchOrders(vendorId: vendor.Id,
                pageSize: 5);

            var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
            if (carrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            
            foreach (var o in orders)
            {
                var orderModel = new VendorSummaryModel.OrderModel
                {
                    Id = o.Id,
                    OrderTotal = _priceFormatter.FormatPrice(o.OrderTotal),
                    ShippingStatusEnum = o.ShippingStatus,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(o.CreatedOnUtc, DateTimeKind.Utc),
                };
                if (carrier.RequireCheckoutDeliveryDateAndTimeslot)
                {
                    orderModel.DeliveryDateText = _shuqOrderService.GetCheckoutDeliveryDate(o.CheckoutAttributesXml);
                    orderModel.DeliveryTimeText = _shuqOrderService.GetCheckoutDeliveryTimeslot(o.CheckoutAttributesXml);

                    var deliveryDateTimeUtc =  _shuqOrderService.GetCheckoutDateTimeSlot(o);
                    orderModel.DeliveryDateTime = deliveryDateTimeUtc != null ? _dateTimeHelper.ConvertToUserTime(deliveryDateTimeUtc.Value, DateTimeKind.Utc) : deliveryDateTimeUtc;
                }
                else
                {
                    orderModel.ShipBeforeDate = o.PaidDateUtc?.AddDays(_shippingJntSettings.ShipBeforeDateAdvanceDay);
                }
                model.Orders.Add(orderModel);
            }

            var payouts = _payoutBatchService.GetPayoutVendors(vendor.Id, status: null, pageSize: pageSize);

            foreach (var p in payouts)
            {
                var payoutModel = new VendorSummaryModel.PayoutModel
                {
                    Date = p.Date,
                    Amount = _priceFormatter.FormatPrice(p.Amount),
                    StatusId = p.StatusId,
                    NumberOfOrders = p.NumberOfOrders,
                    PayoutGroupId = p.PayoutGroupId
                };
                model.Payouts.Add(payoutModel);
            }
            model.OrdersAwaitingShip = _orderService.GetNumberOfOrdersWithoutShipment(vendor.Id).Count;
            model.ReturnOrdersAwatingProcess = _returnRequestService.SearchGroupReturnRequests(ap: ApproveStatusEnum.Pending, vendorId: vendor.Id).Count;
            model.PayoutThisCycle = _priceFormatter.FormatPrice(_payoutBatchService.GetPayoutLastCycleVendorsTotal(vendor.Id)); 
            return model;
        }

        #endregion
    }
}