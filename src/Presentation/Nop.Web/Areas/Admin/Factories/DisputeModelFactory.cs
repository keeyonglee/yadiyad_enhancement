using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
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
    public partial class DisputeModelFactory : IDisputeModelFactory
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
        private readonly IPictureService _pictureService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public DisputeModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IOrderService orderService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IPictureService pictureService,
            IVendorService vendorService)
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
            _pictureService = pictureService;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        public virtual DisputeSearchModel PrepareDisputeSearchModel(DisputeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available return request statuses
            //_baseAdminModelFactory.PrepareReturnRequestStatuses(searchModel.ReturnRequestStatusList, false);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual DisputeListModel PrepareDisputeListModel(DisputeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter emails
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone);
            var oId = searchModel.OrderId == -1 ? 0 : searchModel.OrderId;
            //var returnRequestStatus = searchModel.ReturnRequestStatusId == -1 ? null : (ReturnRequestStatus?)searchModel.ReturnRequestStatusId;


            //get return requests
            var disputes = _returnRequestService.SearchDispute(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                orderId: oId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new DisputeListModel().PrepareToGrid(searchModel, disputes, () =>
            {
                return disputes.Select(dispute =>
                {
                    //fill in model values from the entity
                    var disputeModel = new DisputeModel();

                    var vendor = _vendorService.GetVendorById(dispute.VendorId);

                    disputeModel.Id = dispute.Id;
                    disputeModel.DisputeDetail = dispute.DisputeDetail;
                    disputeModel.DisputeReasonId = dispute.DisputeReasonId;
                    disputeModel.GroupReturnRequestId = dispute.GroupReturnRequestId;
                    disputeModel.CreatedOnUtc = dispute.CreatedOnUtc;
                    disputeModel.UpdatedOnUtc = dispute.UpdatedOnUtc;
                    disputeModel.OrderId = dispute.OrderId;
                    disputeModel.VendorId = dispute.VendorId;
                    disputeModel.VendorName = vendor.Name;
                    disputeModel.DisputeActionId = dispute.DisputeAction;
                    disputeModel.DisputeActionStr = _localizationService.GetLocalizedEnum(dispute.DisputeActionStr);

                    return disputeModel;
                });
            });

            return model;
        }

        public virtual SellerDisputePictureListModel PrepareSellerDisputePictureListModel(SellerDisputePictureSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product pictures
            var pictures = _returnRequestService.GetSellerDisputePictureByGroupReturnRequestId(searchModel.GroupReturnRequestId).ToPagedList(searchModel);

            //prepare grid model
            var model = new SellerDisputePictureListModel().PrepareToGrid(searchModel, pictures, () =>
            {
                return pictures.Select(disputePicture =>
                {
                    //fill in model values from the entity
                    var pictureModel = new SellerDisputePictureModel();

                    //fill in additional values (not existing in the entity)
                    var picture = _pictureService.GetPictureById(disputePicture.PictureId)
                                  ?? throw new Exception("Picture cannot be loaded");

                    pictureModel.Id = disputePicture.Id;
                    pictureModel.PictureId = disputePicture.PictureId;
                    pictureModel.DisplayOrder = disputePicture.DisplayOrder;
                    pictureModel.PictureUrl = _pictureService.GetPictureUrl(ref picture);
                    pictureModel.OverrideAltAttribute = picture.AltAttribute;
                    pictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    return pictureModel;
                });
            });

            return model;
        }



        #endregion
    }
}