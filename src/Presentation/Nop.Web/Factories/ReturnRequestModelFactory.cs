using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payout;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.ShippingShuq;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Models.Media;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the return request model factory
    /// </summary>
    public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
    {
        #region Fields

        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ICustomerService _customerService;
        private readonly OrderPayoutService _orderPayoutService;
        private readonly IShipmentService _shipmentService;
        private readonly ShipmentCarrierResolver _shipmentCarrierResolver;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public ReturnRequestModelFactory(ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IProductModelFactory productModelFactory,
            IReturnRequestService returnRequestService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ICustomerService customerService,
            OrderPayoutService orderPayoutService,
            IShipmentService shipmentService,
            ShipmentCarrierResolver shipmentCarrierResolver,
            IVendorService vendorService)
        {
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _localizationService = localizationService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _productModelFactory = productModelFactory;
            _returnRequestService = returnRequestService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _customerService = customerService;
            _orderPayoutService = orderPayoutService;
            _shipmentService = shipmentService;
            _shipmentCarrierResolver = shipmentCarrierResolver;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the order item model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <returns>Order item model</returns>
        public virtual SubmitReturnRequestModel.OrderItemModel PrepareSubmitReturnRequestOrderItemModel(OrderItem orderItem)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            var order = _orderService.GetOrderById(orderItem.OrderId);
            var product = _productService.GetProductById(orderItem.ProductId);


            var model = new SubmitReturnRequestModel.OrderItemModel
            {
                PictureModel = _productModelFactory.PrepareProductOverviewPictureModel(product, null),
                Id = orderItem.Id,
                ProductId = product.Id,
                ProductName = _localizationService.GetLocalized(product, x => x.Name),
                ProductSeName = _urlRecordService.GetSeName(product),
                AttributeInfo = orderItem.AttributeDescription,
                Quantity = orderItem.Quantity
            };

            var languageId = _workContext.WorkingLanguage.Id;

            //unit price
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax
                var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                model.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            }
            else
            {
                //excluding tax
                var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                model.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            }

            return model;
        }

        /// <summary>
        /// Prepare the submit return request model
        /// </summary>
        /// <param name="model">Submit return request model</param>
        /// <param name="order">Order</param>
        /// <returns>Submit return request model</returns>
        public virtual SubmitReturnRequestModel PrepareSubmitReturnRequestModel(SubmitReturnRequestModel model,
            Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.OrderId = order.Id;
            model.AllowFiles = _orderSettings.ReturnRequestsAllowFiles;
            model.CustomOrderNumber = order.CustomOrderNumber;

            //return reasons
            model.AvailableReturnReasons = _returnRequestService.GetAllReturnRequestReasons()
                .Select(rrr => new SubmitReturnRequestModel.ReturnRequestReasonModel
                {
                    Id = rrr.Id,
                    Name = _localizationService.GetLocalized(rrr, x => x.Name)
                }).ToList();

            //return actions
            model.AvailableReturnActions = _returnRequestService.GetAllReturnRequestActions()
                .Select(rra => new SubmitReturnRequestModel.ReturnRequestActionModel
                {
                    Id = rra.Id,
                    Name = _localizationService.GetLocalized(rra, x => x.Name)
                })
                .ToList();

            //returnable products
            var orderItems = _orderService.GetOrderItems(order.Id, isNotReturnable: false);
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = PrepareSubmitReturnRequestOrderItemModel(orderItem);
                model.Items.Add(orderItemModel);
            }

            return model;
        }

        public virtual GroupReturnRequestModel PrepareGroupReturnRequestModel(int orderId)
        {
            var groupReturnRequestModel = new GroupReturnRequestModel();
            if (orderId == 0)
                throw new ArgumentNullException(nameof(orderId));

            //get return requests
            var groupReturnRequest = _returnRequestService.GetGroupReturnRequestByOrderId(orderId).FirstOrDefault();
            var vendor = _vendorService.GetVendorByOrderId(orderId);
            ReturnOrder rOrder = new ReturnOrder();

            if (groupReturnRequest != null)
            {
                groupReturnRequestModel.Id = groupReturnRequest.Id;

                var returnItems = _returnRequestService.GetReturnRequestByGroupReturnRequestId(groupReturnRequest.Id).FirstOrDefault();
                PrepareReturnRequestAllListModel(groupReturnRequestModel.ReturnList, returnItems);
                var customer = _customerService.GetCustomerById(groupReturnRequestModel.ReturnList[0].CustomerId);
                groupReturnRequestModel.CustomerId = groupReturnRequestModel.ReturnList[0].CustomerId;
                groupReturnRequestModel.ApproveStatusId = groupReturnRequestModel.ApproveStatusId;
                groupReturnRequestModel.CustomerInfo = groupReturnRequestModel.ReturnList[0].CustomerInfo;
                groupReturnRequestModel.OrderId = groupReturnRequestModel.ReturnList[0].OrderId;
                groupReturnRequestModel.CustomOrderNumber = groupReturnRequestModel.ReturnList[0].CustomOrderNumber;
                groupReturnRequestModel.HasInsuranceCover = groupReturnRequest.HasInsuranceCover;
                groupReturnRequestModel.InsuranceClaimAmt = groupReturnRequest.InsuranceClaimAmt;
                groupReturnRequestModel.createdOnUtc = _dateTimeHelper.ConvertToUserTime(groupReturnRequest.CreatedOnUtc, DateTimeKind.Utc);
                groupReturnRequestModel.updatedOnUtc = _dateTimeHelper.ConvertToUserTime(groupReturnRequest.UpdatedOnUtc, DateTimeKind.Utc);
                groupReturnRequestModel.ApproveStatusStr = _localizationService.GetLocalizedEnum(groupReturnRequest.ApproveStatus); ;
                groupReturnRequestModel.ReturnRequestStatusStr = _returnRequestService.GetReturnRequestStatus(groupReturnRequest);
                groupReturnRequestModel.Pictures = PrepareReturnRequestDownloadModel(groupReturnRequest.Id);

                //return shipment
                rOrder = _returnRequestService.GetReturnOrderByGroupReturnRequestId(groupReturnRequest.Id).FirstOrDefault();
                if (rOrder != null)
                {
                    var returnShipment = _shipmentService.GetShipmentsByReturnOrderId(rOrder.Id).FirstOrDefault();
                    var carrier = _shipmentCarrierResolver.ResolveByCourierSetting(vendor);
                    if (carrier == null)
                        throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
                    groupReturnRequestModel.RequireBarCode = carrier.RequireTrackingNumberBarCode;
                    groupReturnRequestModel.TrackingNumberUrl = returnShipment.TrackingNumber;
                }
            }

            return groupReturnRequestModel;
        }

        public virtual IList<ReturnRequestPictureModel> PrepareReturnRequestPictureModel(int groupId)
        {
            if (groupId == 0)
                return null;

            IList<ReturnRequestPictureModel> pictureModel = new List<ReturnRequestPictureModel>();
            var returnPictures = _returnRequestService.GetReturnRequestImageByGroupReturnRequestId(groupId);

            foreach (var pic in returnPictures)
            {
                var picModel = new ReturnRequestPictureModel();
                var picture = _pictureService.GetPictureById(pic.PictureId)
                    ?? throw new Exception("Picture cannot be loaded");

                var picture2 = _downloadService.GetDownloadById(pic.PictureId)
                   ?? throw new Exception("Picture cannot be loaded");

                picModel.PictureId = pic.PictureId;
                picModel.DisplayOrder = pic.DisplayOrder;
                picModel.PictureUrl = _pictureService.GetPictureUrl(ref picture);
                picModel.OverrideAltAttribute = picture.AltAttribute;
                picModel.OverrideTitleAttribute = picture.TitleAttribute;

                pictureModel.Add(picModel);
            }

            return pictureModel;
        }

        private IList<ReturnRequestPictureModel> PrepareReturnRequestDownloadModel(int groupId)
        {
            if (groupId == 0)
                return null;

            IList<ReturnRequestPictureModel> pictureModel = new List<ReturnRequestPictureModel>();
            var returnPictures = _returnRequestService.GetReturnRequestImageByGroupReturnRequestId(groupId);

            foreach (var pic in returnPictures)
            {
                var documentModel = new ReturnRequestPictureModel();

                var download = _downloadService.GetDownloadById(pic.PictureId)
                   ?? throw new Exception("Picture cannot be loaded");

                documentModel.PictureId = pic.PictureId;
                documentModel.DisplayOrder = pic.DisplayOrder;
                documentModel.PictureUrl = _downloadService.GetDownloadUrl(download);
                documentModel.ContentType = download.ContentType;
                documentModel.DownloadGuid = download.DownloadGuid;
                documentModel.FileName = download.Filename;
                pictureModel.Add(documentModel);
            }

            return pictureModel;
        }

        protected virtual void PrepareReturnRequestAllListModel(IList<ReturnRequestModel> models, ReturnRequest returnRequest)
        {
            //get return items
            var returnItems = _returnRequestService.GetReturnRequestByGroupReturnRequestId(returnRequest.GroupReturnRequestId);

            foreach (var returnItem in returnItems)
            {
                //fill in model values from the entity
                var model = new ReturnRequestModel
                {
                    Id = returnItem.Id,
                    CustomNumber = returnItem.CustomNumber,
                    CustomerId = returnItem.CustomerId,
                    Quantity = returnItem.Quantity,
                    ActionAfterReturn = returnItem.ActionAfterReturn
                };

                var customer = _customerService.GetCustomerById(returnItem.CustomerId);
                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnItem.CreatedOnUtc, DateTimeKind.Utc);

                model.CustomerInfo = _customerService.IsRegistered(customer)
                    ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                model.UploadedFileGuid = _downloadService.GetDownloadById(returnItem.UploadedFileId)?.DownloadGuid ?? Guid.Empty;
                var orderItem = _orderService.GetOrderItemById(returnItem.OrderItemId);
                if (orderItem != null)
                {
                    var order = _orderService.GetOrderById(orderItem.OrderId);
                    var product = _productService.GetProductById(orderItem.ProductId);

                    model.ProductId = product.Id;
                    model.ProductName = product.Name;
                    model.OrderId = order.Id;
                    model.AttributeInfo = orderItem.AttributeDescription;
                    model.CustomOrderNumber = order.CustomOrderNumber;
                    model.UnitPrice = Math.Round(orderItem.UnitPriceInclTax, 2);

                    //pictures
                    _productModelFactory.PrepareProductDetailsPictureModel(product, false, out var allPictureModels);
                    model.ProductPictures = allPictureModels;
                }

                model.ReasonForReturn = returnItem.ReasonForReturn;
                model.RequestedAction = returnItem.RequestedAction;
                model.CustomerComments = returnItem.CustomerComments == null ? string.Empty : returnItem.CustomerComments;
                model.StaffNotes = returnItem.StaffNotes;

                models.Add(model);
            }
        }

        /// <summary>
        /// Prepare the customer return requests model
        /// </summary>
        /// <returns>Customer return requests model</returns>
        public virtual CustomerReturnRequestsModel PrepareCustomerReturnRequestsModel()
        {
            var customer = _workContext.CurrentCustomer;
            var model = new CustomerReturnRequestsModel();

            var groupReturn = _returnRequestService.SearchCustomerGroupReturnRequests(
                customerId: customer.Id);

            foreach (var returnRequest in groupReturn){
                var returnItems = _returnRequestService.GetReturnRequestByGroupReturnRequestId(returnRequest.Id).FirstOrDefault();
                var order = _orderService.GetOrderByOrderItem(returnItems.OrderItemId);
                var orderRefundRequest = _orderPayoutService.GetOrderRefundRequestByOrderId(order.Id);
                var dispute = _returnRequestService.GetDisputeByGroupReturnRequestId(returnRequest.Id).FirstOrDefault();
                
                IList<ReturnRequestModel> returnList = new List<ReturnRequestModel>();
                PrepareReturnRequestAllListModel(returnList, returnItems);
                var itemModel = new CustomerReturnRequestsModel.GroupReturnRequestModel
                {
                    Id = returnRequest.Id,
                    CustomerId = returnRequest.CustomerId,
                    ReturnList = returnList,
                    OrderId = order.Id,
                    ApproveStatusId = returnRequest.ApproveStatusId,
                    HasInsuranceCover = returnRequest.HasInsuranceCover,
                    InsuranceClaimAmt = returnRequest.InsuranceClaimAmt,
                    NeedReturnShipping = returnRequest.NeedReturnShipping,
                    createdOnUtc = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc),
                    updatedOnUtc = _dateTimeHelper.ConvertToUserTime(returnRequest.UpdatedOnUtc, DateTimeKind.Utc),
                    ApproveStatusStr = _returnRequestService.GetReturnRequestStatus(returnRequest),
                    RefundStatusStr = "New",
                    ReturnConditionId = returnRequest.ReturnConditionId,
                };
                
                if (orderRefundRequest != null)
                {
                    itemModel.RefundStatusId = orderRefundRequest.RefundStatusId;
                    
                    if (returnRequest.ApproveStatus == ApproveStatusEnum.Approved && dispute != null)
                        itemModel.RefundStatusStr = GetRefundStatus(dispute);
                    else
                        itemModel.RefundStatusStr = _localizationService.GetLocalizedEnum(orderRefundRequest.RefundStatus);
                    
                    itemModel.RefundAmount = orderRefundRequest.Amount;

                    if (orderRefundRequest.RefundStatus == RefundStatus.Paid)
                    {
                        itemModel.OrderRefundRequestId = orderRefundRequest.Id;
                    }
                }
                model.Items.Add(itemModel);
            }

            return model;
        }

        private string GetRefundStatus(Dispute dispute)
        {
            var refundStatusStr = "";
            var refundAction = dispute.DisputeAction;

            if (refundAction == (int) DisputeActionEnum.FullRefundFromBuyer)
                refundStatusStr = "Full Refund";
            else if (refundAction == (int) DisputeActionEnum.PartialRefund)
                refundStatusStr = "Partial Refund";

            return refundStatusStr;
        }

        public virtual GroupReturnRequestModel PrepareCustomerGroupReturnRequestsModel()
        {
            var groupReturnRequestModel = new GroupReturnRequestModel();

            var groupReturnRequests = _returnRequestService.SearchCustomerGroupReturnRequests(_storeContext.CurrentStore.Id, _workContext.CurrentCustomer.Id);
            foreach (var groupReturnRequest in groupReturnRequests)
            {
                groupReturnRequestModel.Id = groupReturnRequest.Id;

                var returnItems = _returnRequestService.GetReturnRequestByGroupReturnRequestId(groupReturnRequest.Id).FirstOrDefault();
                PrepareReturnRequestAllListModel(groupReturnRequestModel.ReturnList, returnItems);
                var customer = _customerService.GetCustomerById(groupReturnRequestModel.ReturnList[0].CustomerId);
                groupReturnRequestModel.CustomerId = groupReturnRequestModel.ReturnList[0].CustomerId;
                groupReturnRequestModel.ApproveStatusId = groupReturnRequestModel.ApproveStatusId;
                groupReturnRequestModel.CustomerInfo = groupReturnRequestModel.ReturnList[0].CustomerInfo;
                groupReturnRequestModel.OrderId = groupReturnRequestModel.ReturnList[0].OrderId;
                groupReturnRequestModel.CustomOrderNumber = groupReturnRequestModel.ReturnList[0].CustomOrderNumber;
                groupReturnRequestModel.HasInsuranceCover = groupReturnRequest.HasInsuranceCover;
                groupReturnRequestModel.InsuranceClaimAmt = groupReturnRequest.InsuranceClaimAmt;
                groupReturnRequestModel.createdOnUtc = _dateTimeHelper.ConvertToUserTime(groupReturnRequest.CreatedOnUtc, DateTimeKind.Utc);
                groupReturnRequestModel.updatedOnUtc = _dateTimeHelper.ConvertToUserTime(groupReturnRequest.UpdatedOnUtc, DateTimeKind.Utc);
                groupReturnRequestModel.ApproveStatusStr = _localizationService.GetLocalizedEnum(groupReturnRequest.ApproveStatus);
            }

            return groupReturnRequestModel;
        }


        #endregion
    }
}