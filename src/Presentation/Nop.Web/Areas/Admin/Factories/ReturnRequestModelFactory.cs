using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.ShuqOrders;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the return request model factory implementation
    /// </summary>
    public partial class ReturnRequestModelFactory : IReturnRequestModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IVendorService _vendorService;
        private readonly IShipmentService _shipmentService;
        private readonly ICategoryService _categoryService;
        private readonly IWorkContext _workContext;
        private readonly IShuqOrderProcessingService _shuqOrderProcessingService;

        #endregion

        #region Ctor

        public ReturnRequestModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IOrderService orderService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IVendorService vendorService,
            IShipmentService shipmentService,
            ICategoryService categoryService,
            IWorkContext workContext,
            IShuqOrderProcessingService shuqOrderProcessingService)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _customerService = customerService;
            _localizationService = localizationService;
            _localizedModelFactory = localizedModelFactory;
            _orderService = orderService;
            _productService = productService;
            _returnRequestService = returnRequestService;
            _vendorService = vendorService;
            _shipmentService = shipmentService;
            _categoryService = categoryService;
            _workContext = workContext;
            _shuqOrderProcessingService = shuqOrderProcessingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare return request search model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>Return request search model</returns>
        public virtual ReturnRequestSearchModel PrepareReturnRequestSearchModel(ReturnRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available return request statuses
            _baseAdminModelFactory.PrepareReturnRequestStatuses(searchModel.ReturnRequestStatusList, false);
            _baseAdminModelFactory.PrepareApproveStatus(searchModel.ApproveStatusList, false);

            //for some reason, the standard default value (0) for the "All" item is already used for the "Pending" status, so here we use -1
            searchModel.ReturnRequestStatusId = -1;
            searchModel.ReturnRequestStatusList.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = _localizationService.GetResource("Admin.ReturnRequests.SearchReturnRequestStatus.All")
            });

            searchModel.ApproveStatusId = -1;
            searchModel.ApproveStatusList.Insert(0, new SelectListItem
            {
                Value = "-1",
                Text = _localizationService.GetResource("Admin.ReturnRequests.SearchReturnRequestStatus.All")
            });

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual ReturnOrderSearchModel PrepareReturnOrderSearchModel(ReturnOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual ReturnOrderModel PrepareReturnOrderModel(ReturnOrderModel model, int id,
            bool excludeProperties = false)
        {

            model = new ReturnOrderModel();

            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(id);

            var returnOrder = _returnRequestService.GetReturnOrderByGroupReturnRequestId(id);

            model.Id = returnOrder[0].Id;
            model.GroupReturnRequestId = returnOrder[0].GroupReturnRequestId;
            model.createdOnUtc = returnOrder[0].CreatedOnUtc;
            model.updatedOnUtc = returnOrder[0].UpdatedOnUtc;
            model.ActualShippingExclTax = returnOrder[0].ActualShippingExclTax;
            model.ActualShippingInclTax = returnOrder[0].ActualShippingInclTax;
            model.EstimatedShippingExclTax = returnOrder[0].EstimatedShippingExclTax;
            model.EstimatedShippingInclTax = returnOrder[0].EstimatedShippingInclTax;
            model.IsShipped = returnOrder[0].IsShipped;
            PrepareReturnRequestAllListModel(model.ReturnList, returnRequest[0]);

            return model;
        }

        public virtual ReturnOrderListModel PrepareReturnOrderListModel(ReturnOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            //get return orders
            var returnOrders = _returnRequestService.SearchReturnOrder(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ReturnOrderListModel().PrepareToGrid(searchModel, returnOrders, () =>
            {
                return returnOrders.Select(returnOrder =>
                {
                    //fill in model values from the entity
                    //var returnRequestModel = returnRequest.ToModel<GroupReturnRequestModel>();
                    var returnOrderModel = new ReturnOrderModel();

                    returnOrderModel.Id = returnOrder.Id;

                    //var returnItems = _returnRequestService.GetReturnRequestByGroupReturnRequestId(returnRequest.Id);
                    //PrepareReturnRequestAllListModel(groupReturnRequestModel.ReturnList, returnItems[0]);

                    returnOrderModel.GroupReturnRequestId = returnOrder.GroupReturnRequestId;
                    returnOrderModel.EstimatedShippingExclTax = returnOrder.EstimatedShippingExclTax;
                    returnOrderModel.EstimatedShippingInclTax = returnOrder.EstimatedShippingInclTax;
                    returnOrderModel.ActualShippingExclTax = returnOrder.ActualShippingExclTax;
                    returnOrderModel.ActualShippingInclTax = returnOrder.ActualShippingInclTax;
                    returnOrderModel.createdOnUtc = returnOrder.CreatedOnUtc;
                    returnOrderModel.updatedOnUtc = returnOrder.UpdatedOnUtc;


                    return returnOrderModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare paged return request list model
        /// </summary>
        /// <param name="searchModel">Return request search model</param>
        /// <returns>Return request list model</returns>
        public virtual GroupReturnRequestListModel PrepareGroupReturnRequestListModel(ReturnRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var returnRequestStatus = searchModel.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?)searchModel.ReturnRequestStatusId;
            var approveStatus = searchModel.ApproveStatusId == -1 ? null : (ApproveStatusEnum?)searchModel.ApproveStatusId;
            
            //get return requests
            var returnRequests = _returnRequestService.SearchGroupReturnRequests(
                ap: approveStatus,
                createdFromUtc: startDateValue,
                vendorId: _workContext.CurrentVendor?.Id ?? 0,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new GroupReturnRequestListModel().PrepareToGrid(searchModel, returnRequests, () =>
            {
                return returnRequests.Select(returnRequest =>
                {
                    //fill in model values from the entity
                    //var returnRequestModel = returnRequest.ToModel<GroupReturnRequestModel>();
                    var groupReturnRequestModel = new GroupReturnRequestModel();

                    groupReturnRequestModel.Id = returnRequest.Id;

                    var returnItems = _returnRequestService.GetReturnRequestByGroupReturnRequestId(returnRequest.Id);
                    PrepareReturnRequestAllListModel(groupReturnRequestModel.ReturnList, returnItems[0]);
                    var customer = _customerService.GetCustomerById(groupReturnRequestModel.ReturnList[0].CustomerId);
                    groupReturnRequestModel.CustomerId = groupReturnRequestModel.ReturnList[0].CustomerId;
                    groupReturnRequestModel.CustomerInfo = groupReturnRequestModel.ReturnList[0].CustomerInfo;
                    groupReturnRequestModel.OrderId = groupReturnRequestModel.ReturnList[0].OrderId;
                    groupReturnRequestModel.CustomOrderNumber = groupReturnRequestModel.ReturnList[0].CustomOrderNumber;
                    groupReturnRequestModel.HasInsuranceCover = returnRequest.HasInsuranceCover;
                    groupReturnRequestModel.InsuranceClaimAmt = returnRequest.InsuranceClaimAmt;
                    groupReturnRequestModel.createdOnUtc = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);
                    groupReturnRequestModel.ApproveStatusStr = _localizationService.GetLocalizedEnum(returnRequest.ApproveStatus);
                    groupReturnRequestModel.ApproveStatusDetailsStr = _returnRequestService.GetReturnRequestStatus(returnRequest);
                    groupReturnRequestModel.ApproveStatusId = returnRequest.ApproveStatusId;

                    var order = _orderService.GetOrderById(groupReturnRequestModel.ReturnList[0].OrderId);

                    return groupReturnRequestModel;
                });
            });

            return model;
        }

        public virtual ReturnRequestListModel PrepareReturnRequestListModel(ReturnRequestItemSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var returnRequestStatus = searchModel.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?)searchModel.ReturnRequestStatusId;

            //get return requests
            var returnRequests = _returnRequestService.SearchReturnRequests(customNumber: searchModel.CustomNumber,
                groupId: searchModel.GroupReturnRequestId,
                //rs: returnRequestStatus,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ReturnRequestListModel().PrepareToGrid(searchModel, returnRequests, () =>
            {
                return returnRequests.Select(returnRequest =>
                {
                    //fill in model values from the entity
                    var returnRequestModel = returnRequest.ToModel<ReturnRequestModel>();

                    var customer = _customerService.GetCustomerById(returnRequest.CustomerId);

                    //convert dates to the user time
                    returnRequestModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    returnRequestModel.CustomerInfo = _customerService.IsRegistered(customer)
                        ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
                    if (orderItem == null)
                        return returnRequestModel;

                    var order = _orderService.GetOrderById(orderItem.OrderId);
                    var product = _productService.GetProductById(orderItem.ProductId);

                    returnRequestModel.ProductId = orderItem.ProductId;
                    returnRequestModel.ProductName = product.Name;
                    returnRequestModel.OrderId = order.Id;
                    returnRequestModel.AttributeInfo = orderItem.AttributeDescription;
                    returnRequestModel.CustomOrderNumber = order.CustomOrderNumber;
                    returnRequestModel.PictureIds = _returnRequestService.GetReturnRequestImageByGroupReturnRequestId(returnRequest.GroupReturnRequestId);

                    return returnRequestModel;
                });
            });

            return model;
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
                }


                model.ReasonForReturn = returnItem.ReasonForReturn;
                model.RequestedAction = returnItem.RequestedAction;
                model.CustomerComments = returnItem.CustomerComments;
                model.StaffNotes = returnItem.StaffNotes;

                models.Add(model);
            }
        }

        public virtual ReturnRequestModel PrepareSingleReturnRequestModel(ReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties = false)
        {
            if (returnRequest == null)
                return model;

            //fill in model values from the entity
            model ??= new ReturnRequestModel
            {
                Id = returnRequest.Id,
                CustomNumber = returnRequest.CustomNumber,
                CustomerId = returnRequest.CustomerId,
                Quantity = returnRequest.Quantity
            };

            var customer = _customerService.GetCustomerById(returnRequest.CustomerId);

            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(returnRequest.CreatedOnUtc, DateTimeKind.Utc);

            model.CustomerInfo = _customerService.IsRegistered(customer)
                ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
            model.UploadedFileGuid = _downloadService.GetDownloadById(returnRequest.UploadedFileId)?.DownloadGuid ?? Guid.Empty;
            var orderItem = _orderService.GetOrderItemById(returnRequest.OrderItemId);
            if (orderItem != null)
            {
                var order = _orderService.GetOrderById(orderItem.OrderId);
                var product = _productService.GetProductById(orderItem.ProductId);

                model.ProductId = product.Id;
                model.ProductName = product.Name;
                model.OrderId = order.Id;
                model.AttributeInfo = orderItem.AttributeDescription;
                model.CustomOrderNumber = order.CustomOrderNumber;
            }

            if (excludeProperties)
                return model;

            model.ReasonForReturn = returnRequest.ReasonForReturn;
            model.RequestedAction = returnRequest.RequestedAction;
            model.CustomerComments = returnRequest.CustomerComments;
            model.StaffNotes = returnRequest.StaffNotes;
            model.PictureIds = _returnRequestService.GetReturnRequestImageByGroupReturnRequestId(returnRequest.GroupReturnRequestId);

            return model;
        }

        /// <summary>
        /// Prepare return request model
        /// </summary>
        /// <param name="model">Return request model</param>
        /// <param name="returnRequest">Return request</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request model</returns>
        public virtual GroupReturnRequestModel PrepareReturnRequestModel(GroupReturnRequestModel model,
            ReturnRequest returnRequest, bool excludeProperties = false)
        {
            if (returnRequest == null)
                return model;

            model = new GroupReturnRequestModel();

            var groupItems = _returnRequestService.GetGroupReturnRequestById(returnRequest.GroupReturnRequestId);

            model.Id = groupItems.Id;
            model.createdOnUtc = groupItems.CreatedOnUtc;
            model.updatedOnUtc = groupItems.UpdatedOnUtc;
            model.HasInsuranceCover = groupItems.HasInsuranceCover;
            model.InsuranceClaimAmt = groupItems.InsuranceClaimAmt;
            model.InsurancePayoutDate = groupItems.InsurancePayoutDate;
            model.InsuranceRef = groupItems.InsuranceRef;
            model.IsInsuranceClaim = groupItems.IsInsuranceClaim;
            model.ApproveStatusId = groupItems.ApproveStatusId;
            model.NeedReturnShipping = groupItems.NeedReturnShipping;
            model.ReturnConditionId = groupItems.ReturnConditionId;
            model.CanProductShip = _shuqOrderProcessingService.CanReturnProductShip(groupItems);

            PrepareReturnRequestAllListModel(model.ReturnList, returnRequest);

            var orderId = model.ReturnList[0].OrderId;
            var checkIfOrderHasShip = _shipmentService.GetShipmentsByOrderId(orderId);
            if (checkIfOrderHasShip.Count > 0)
                model.IsOrderShipped = true;

            IList<OrderItem> items = new List<OrderItem>();
            items = _orderService.GetOrderItems(orderId);
            IList<ProductCategory> categoryIds = new List<ProductCategory>();
            List<string> category = new List<string>();

            foreach (var item in items)
            {
                categoryIds = _categoryService.GetProductCategoriesByProductId(item.ProductId);
            }

            foreach (var cat in categoryIds)
            {
                var categoryId = _categoryService.GetCategoryById(cat.CategoryId);
                category.Add(categoryId.Name);
            }

            var vendor = _vendorService.GetVendorByOrderId(orderId);
            var vendorCategoryText = vendor.CategoryId == null ? string.Empty : _categoryService.GetCategoryById(vendor.CategoryId.Value).Name;

            //bool isShuqEats = category.Contains("Shuq Eats");
            bool isShuqEats = vendorCategoryText.Contains(NopVendorDefaults.VendorCategoryEats);

            if (!isShuqEats)
                model.IsCategoryValid = true;

            return model;
        }

        public virtual SellerDisputeModel PrepareSellerDisputeModel(SellerDisputeModel model,
            ReturnRequest returnRequest, bool excludeProperties = false)
        {
            if (returnRequest == null)
                return model;

            model = new SellerDisputeModel();

            var groupItems = _returnRequestService.GetGroupReturnRequestById(returnRequest.GroupReturnRequestId);
            var disputeItems = _returnRequestService.GetDisputeByGroupReturnRequestId(returnRequest.GroupReturnRequestId).FirstOrDefault();

            if(disputeItems != null)
            {
                model.DisputeDetail = disputeItems.DisputeDetail;
                model.DisputeReasonId = disputeItems.DisputeReasonId;
            }
            model.CanRaiseDispute = _shuqOrderProcessingService.CanRaiseDispute(groupItems);
            model.GroupReturnRequestId = groupItems.Id;
            model.SellerDisputePictureSearchModel.GroupReturnRequestId = groupItems.Id;
            model.GroupReturnRequestApprovalStatus = groupItems.ApproveStatus.ToString();

            PrepareReturnRequestAllListModel(model.ReturnList, returnRequest);

            return model;
        }

        protected virtual decimal CalculateReturnItemPrice(IList<ReturnRequest> returnItems)
        {
            if (returnItems == null)
                return 0;

            var total = 0.0m;

            foreach (var item in returnItems)
            {
                var returnAmount = 0.0m;

                var orderItem = _orderService.GetOrderItemById(item.OrderItemId);
                var product = _productService.GetProductById(orderItem.ProductId);

                returnAmount = product.Price * item.Quantity;

                total = total + returnAmount;
            }

            return total;
        }
        public virtual DisputeSubmitModel PrepareDisputeSubmitModel(DisputeSubmitModel model,
            int id, bool excludeProperties = false)
        {
            if (id == null)
                return model;

            model = new DisputeSubmitModel();

            var disputeItems = _returnRequestService.GetDisputeById(id);
            var groupItems = _returnRequestService.GetGroupReturnRequestById(disputeItems.GroupReturnRequestId);
            var returnRequest = _returnRequestService.GetReturnRequestByGroupReturnRequestId(disputeItems.GroupReturnRequestId);
            var vendor = _vendorService.GetVendorById(disputeItems.VendorId);

            if (disputeItems != null)
            {
                model.IsReturnDispute = disputeItems.IsReturnDispute;
                model.DisputeDetail = disputeItems.DisputeDetail;
                model.DisputeReasonStr = _localizationService.GetLocalizedEnum(disputeItems.DisputeReason);
                model.DisputeReasonId = disputeItems.DisputeReasonId;
                model.DisputeAction = disputeItems.DisputeAction;
                model.VendorName = vendor.Name;
                model.CanProductShip = _shuqOrderProcessingService.CanReturnProductShip(groupItems);
                model.NeedReturnShipping = groupItems.NeedReturnShipping;
                model.TotalReturnAmount = CalculateReturnItemPrice(returnRequest);
                model.PartialAmount = disputeItems.PartialAmount;
                model.Id = id;
            }
            model.GroupReturnRequestId = groupItems.Id;

            foreach (var item in returnRequest)
            {
                IList<ReturnRequestImage> pic = new List<ReturnRequestImage>();
                
                pic = _returnRequestService.GetReturnRequestImageByGroupReturnRequestId(item.GroupReturnRequestId);

                IList<ReturnRequestImage> flist = new List<ReturnRequestImage>(model.CustomerPictureIds.Concat(pic));
                model.CustomerPictureIds = flist;
            }

            model.SellerPictureIds = _returnRequestService.GetSellerDisputePictureByGroupReturnRequestId(model.GroupReturnRequestId);

            PrepareReturnRequestAllListModel(model.ReturnList, returnRequest[0]);

            return model;

        }
        /// <summary>
        /// Prepare return request reason search model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>Return request reason search model</returns>
        public virtual ReturnRequestReasonSearchModel PrepareReturnRequestReasonSearchModel(ReturnRequestReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged return request reason list model
        /// </summary>
        /// <param name="searchModel">Return request reason search model</param>
        /// <returns>Return request reason list model</returns>
        public virtual ReturnRequestReasonListModel PrepareReturnRequestReasonListModel(ReturnRequestReasonSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get return request reasons
            var reasons = _returnRequestService.GetAllReturnRequestReasons().ToPagedList(searchModel);

            //prepare list model
            var model = new ReturnRequestReasonListModel().PrepareToGrid(searchModel, reasons, () =>
            {
                return reasons.Select(reason => reason.ToModel<ReturnRequestReasonModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare return request reason model
        /// </summary>
        /// <param name="model">Return request reason model</param>
        /// <param name="returnRequestReason">Return request reason</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request reason model</returns>
        public virtual ReturnRequestReasonModel PrepareReturnRequestReasonModel(ReturnRequestReasonModel model,
            ReturnRequestReason returnRequestReason, bool excludeProperties = false)
        {
            Action<ReturnRequestReasonLocalizedModel, int> localizedModelConfiguration = null;

            if (returnRequestReason != null)
            {
                //fill in model values from the entity
                model ??= returnRequestReason.ToModel<ReturnRequestReasonModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(returnRequestReason, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        /// <summary>
        /// Prepare return request action search model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>Return request action search model</returns>
        public virtual ReturnRequestActionSearchModel PrepareReturnRequestActionSearchModel(ReturnRequestActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged return request action list model
        /// </summary>
        /// <param name="searchModel">Return request action search model</param>
        /// <returns>Return request action list model</returns>
        public virtual ReturnRequestActionListModel PrepareReturnRequestActionListModel(ReturnRequestActionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get return request actions
            var actions = _returnRequestService.GetAllReturnRequestActions().ToPagedList(searchModel);

            //prepare list model
            var model = new ReturnRequestActionListModel().PrepareToGrid(searchModel, actions, () =>
            {
                return actions.Select(reason => reason.ToModel<ReturnRequestActionModel>());
            });

            return model;
        }

        /// <summary>
        /// Prepare return request action model
        /// </summary>
        /// <param name="model">Return request action model</param>
        /// <param name="returnRequestAction">Return request action</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Return request action model</returns>
        public virtual ReturnRequestActionModel PrepareReturnRequestActionModel(ReturnRequestActionModel model,
            ReturnRequestAction returnRequestAction, bool excludeProperties = false)
        {
            Action<ReturnRequestActionLocalizedModel, int> localizedModelConfiguration = null;

            if (returnRequestAction != null)
            {
                //fill in model values from the entity
                model ??= returnRequestAction.ToModel<ReturnRequestActionModel>();

                //define localized model configuration action
                localizedModelConfiguration = (locale, languageId) =>
                {
                    locale.Name = _localizationService.GetLocalized(returnRequestAction, entity => entity.Name, languageId, false, false);
                };
            }

            //prepare localized models
            if (!excludeProperties)
                model.Locales = _localizedModelFactory.PrepareLocalizedModels(localizedModelConfiguration);

            return model;
        }

        #endregion
    }
}