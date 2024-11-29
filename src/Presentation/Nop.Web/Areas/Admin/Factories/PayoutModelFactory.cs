using Nop.Core.Domain.News;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.BankAccount;
using YadiYad.Pro.Services.Individual;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Pro.Models.YadiyadNews;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Services.DTO.BankAccount;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Services.DTO.Individual;
using Nop.Web.Areas.Admin.Models.Payout;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Core.Domain.Payout;
using Nop.Services.Media;
using Nop.Web.Models.Payout;
using Nop.Core;
using Nop.Web.Models.Common;
using Nop.Services.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public class PayoutModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly PayoutBatchService _payoutBatchService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor
        public PayoutModelFactory(IBaseAdminModelFactory baseAdminModelFactory, 
            PayoutBatchService payoutBatchService,
            IPriceFormatter priceFormatter)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _payoutBatchService = payoutBatchService;
            _priceFormatter = priceFormatter;
        }
        #endregion

        public virtual PayoutBatchSearchModel PreparePayoutBatchSearchModel(PayoutBatchSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare request types
            _baseAdminModelFactory.PreparePayoutBatchStatusList(searchModel.StatusList);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual PayoutBatchListModel PreparePayoutBatchListModel(PayoutBatchSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var response = new ResponseDTO();
            var filterDto = new PayoutBatchFilterDTO();
            filterDto.GeneratedDate = searchModel.GeneratedDate;
            filterDto.Status = searchModel.Status;

            var dto = _payoutBatchService.GetPayoutBatchs(searchModel.Page - 1, searchModel.PageSize, filterDto);

            var model = new PayoutBatchListModel().PrepareToGrid(searchModel, dto, () =>
                        {
                            return dto.Select(payoutBatch =>
                            {
                                var payoutBatchModel = new PayoutBatchModel();
                                payoutBatchModel.Id = payoutBatch.Id;
                                payoutBatchModel.GeneratedDateTime = payoutBatch.GeneratedDateTime;
                                payoutBatchModel.DownloadDateTime = payoutBatch.DownloadDateTime;
                                payoutBatchModel.ReconDateTime = payoutBatch.ReconDateTime;
                                payoutBatchModel.Status = payoutBatch.Status;
                                payoutBatchModel.StatusText = ((PayoutBatchStatus)payoutBatch.Status).GetDescription();
                                payoutBatchModel.StatusRemarks = payoutBatch.StatusRemarks;
                                payoutBatchModel.ReconFileDownloadId = payoutBatch.ReconFileDownloadId;
                                payoutBatchModel.ReconFileDownloadGuid = payoutBatch.ReconFileDownloadGuid;
                                payoutBatchModel.CreatedOnUTC = payoutBatch.CreatedOnUTC;
                                payoutBatchModel.PayoutGroupCount = payoutBatch.PayoutGroupCount;
                                payoutBatchModel.Amount = payoutBatch.Amount;
                                payoutBatchModel.PayoutBatchNumber = payoutBatch.PayoutBatchNumber;
                                payoutBatchModel.Platform = payoutBatch.Platform;
                                payoutBatchModel.PlatformText = payoutBatchModel.Platform.GetDescription();

                                return payoutBatchModel;
                            });
                        });
            return model;
        }

        public virtual PayoutBatchModel PreparePayoutBatchModel(PayoutBatchModel model, PayoutBatchDTO payoutBatch, bool excludeProperties = false)
        {
            if (payoutBatch != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = new PayoutBatchModel();
                    model.Id = payoutBatch.Id;
                    model.GeneratedDateTime = payoutBatch.GeneratedDateTime;
                    model.DownloadDateTime = payoutBatch.DownloadDateTime;
                    model.ReconDateTime = payoutBatch.ReconDateTime;
                    model.Status = payoutBatch.Status;
                    model.ReconFileDownloadId = payoutBatch.ReconFileDownloadId;
                    model.Status = payoutBatch.Status;
                    model.PayoutGroupCount = payoutBatch.PayoutGroupCount;
                    model.Amount = payoutBatch.Amount;
                    model.PayoutBatchNumber = payoutBatch.PayoutBatchNumber;
                    model.CreatedOnUTC = payoutBatch.CreatedOnUTC;
                }
            }
            return model;
        }

        public virtual PayoutGroupSearchModel PreparePayoutGroupSearchModel(int batchId, PayoutGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare request types
            _baseAdminModelFactory.PreparePayoutGroupStatusList(searchModel.StatusList);
            _baseAdminModelFactory.PreparePayoutGroupCustomerList(batchId, searchModel.CustomerList);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual PayoutGroupListModel PreparePayoutGroupListModel(PayoutGroupSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var response = new ResponseDTO();
            var filterDto = new PayoutGroupFilterDTO();
            filterDto.CustomerId = searchModel.CustomerId;
            filterDto.Status = searchModel.Status;

            var dto = _payoutBatchService.GetPayoutGroupsByBatchId(searchModel.BatchId, searchModel.Page - 1, searchModel.PageSize, filterDto);

            var model = new PayoutGroupListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(payoutGroup =>
                {
                    var name = "";
                    if (payoutGroup.IndividualProfile != null)
                    {
                        name = payoutGroup.IndividualProfile.FullName;
                    }
                    else if (payoutGroup.OrganizationProfile != null)
                    {
                        name = payoutGroup.OrganizationProfile.Name;
                    }
                    else if (payoutGroup.Customer != null)
                    {
                        name = payoutGroup.Customer.Email;
                    }

                    var payoutGroupModel = new PayoutGroupModel();
                    payoutGroupModel.Id = payoutGroup.Id;
                    payoutGroupModel.CustomerName = payoutGroup.AccountHolderName;
                    payoutGroupModel.RequestCount = payoutGroup.RequestCount;
                    payoutGroupModel.Status = payoutGroup.Status;
                    payoutGroupModel.StatusText = ((PayoutGroupStatus)payoutGroup.Status).GetDescription();
                    payoutGroupModel.Remarks = payoutGroup.Remarks;
                    payoutGroupModel.Amount = payoutGroup.Amount;
                    payoutGroupModel.BankName = payoutGroup.BankName;
                    payoutGroupModel.AccountNumber = payoutGroup.AccountNumber;
                    return payoutGroupModel;
                });
            });
            return model;
        }

        public virtual PayoutRequestSearchModel PreparePayoutRequestSearchModel(PayoutRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual PayoutRequestListModel PreparePayoutRequestListModel(PayoutRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var response = new ResponseDTO();
            var filterDto = new PayoutRequestFilterDTO();

            var dto = _payoutBatchService.GetRequestByGroupId(searchModel.GroupId, searchModel.Page - 1, searchModel.PageSize);

            var model = new PayoutRequestListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(payoutRequest =>
                {
                    var payoutRequestModel = new PayoutRequestModel();
                    payoutRequestModel.Id = payoutRequest.Id;
                    payoutRequestModel.OrderNo = payoutRequest.CustomOrderNumber;

                    payoutRequestModel.RequestType = payoutRequest.RefTypeId;
                    payoutRequestModel.RequestTypeText = ((PayoutAndGroupRefType)payoutRequest.RefTypeId).GetDescription();

                    switch((PayoutAndGroupRefType)payoutRequest.RefTypeId)
                    {
                        case PayoutAndGroupRefType.PayoutRequest:
                            payoutRequestModel.ListedProfessionalFee = payoutRequest.PayoutRequest.Fee + payoutRequest.PayoutRequest.ServiceCharge;
                            payoutRequestModel.PayoutCharges = payoutRequest.PayoutRequest.ServiceCharge;
                            payoutRequestModel.PayoutAmount = payoutRequest.PayoutRequest.Fee;
                            break;
                        case PayoutAndGroupRefType.RefundRequest:
                            payoutRequestModel.ListedProfessionalFee = payoutRequest.RefundRequest.Amount + payoutRequest.RefundRequest.ServiceCharge;
                            payoutRequestModel.PayoutCharges = payoutRequest.RefundRequest.ServiceCharge;
                            payoutRequestModel.PayoutAmount = payoutRequest.RefundRequest.Amount;
                            break;
                        case PayoutAndGroupRefType.OrderPayoutRequest:
                            payoutRequestModel.ListedProfessionalFee =
                                payoutRequest.OrderPayoutRequest.GrossPayoutAmount();
                            payoutRequestModel.PayoutCharges = 
                                (payoutRequest.OrderPayoutRequest.ServiceCharge??0)
                                + (payoutRequest.OrderPayoutRequest.ServiceChargeSST ?? 0);
                            payoutRequestModel.PayoutAmount = payoutRequestModel.ListedProfessionalFee - payoutRequestModel.PayoutCharges;

                            break;
                        case PayoutAndGroupRefType.OrderRefundRequest:
                            payoutRequestModel.ListedProfessionalFee =
                                payoutRequest.OrderRefundRequest.Amount;
                            payoutRequestModel.PayoutCharges = 0;
                            payoutRequestModel.PayoutAmount =
                                payoutRequest.OrderRefundRequest.Amount;
                            break;
                    }
                    return payoutRequestModel;
                });
            });
            return model;
        }

        public virtual PayoutVendorListModel PreparePayoutVendorListModel(PayoutVendorSearchModel searchModel, int vendorId)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _payoutBatchService.GetPayoutVendorList(vendorId, searchModel.Page - 1, searchModel.PageSize);

            var model = new PayoutVendorListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(payoutBatch =>
                {
                    var payoutVendorModel = new PayoutVendorModel();
                    payoutVendorModel.Date = payoutBatch.Date;
                    payoutVendorModel.Amount = payoutBatch.Amount;
                    payoutVendorModel.StatusId = payoutBatch.StatusId;
                    payoutVendorModel.PayoutGroupId = payoutBatch.PayoutGroupId;
                    payoutVendorModel.NumberOfOrders = payoutBatch.NumberOfOrders;
                    return payoutVendorModel;
                });
            });
            return model;
        }

        public virtual PayoutVendorModel PrepareVendorPayoutDetailsModel(PayoutVendorModel model, PayoutGroupDTO payoutGroup, bool excludeProperties = false)
        {
            if (payoutGroup != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = new PayoutVendorModel();
                    model.Id = payoutGroup.Id;
                    model.Date = payoutGroup.CreatedOnUTC;
                    model.StatusId = payoutGroup.Status;
                    model.Amount = payoutGroup.Amount;
                    model.BankName = payoutGroup.BankName;
                    model.BankAccount = payoutGroup.AccountNumber;
                    model.Remarks = payoutGroup.Remarks;
                }
            }
            return model;
        }

        public virtual PayoutVendorListModel PreparePayoutVendorOrderListModel(PayoutVendorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _payoutBatchService.GetPayoutVendorOrderList(searchModel.PayoutGroupId, searchModel.Page - 1, searchModel.PageSize);

            var model = new PayoutVendorListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(payoutBatch =>
                {
                    var payoutVendorModel = new PayoutVendorModel();
                    payoutVendorModel.OrderId = payoutBatch.OrderId;
                    payoutVendorModel.Date = payoutBatch.Date;
                    payoutVendorModel.Amount = payoutBatch.Amount;
                    return payoutVendorModel;
                });
            });
            return model;
        }

        public virtual PayoutVendorSearchModel PreparePayoutVendorOrderListSearchModel(PayoutVendorSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual VendorPayoutListModel PreparePayoutVendorListApiModel(
            int vendorId,
            PayoutGroupStatus? status,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var model = new VendorPayoutListModel();

            var payouts = _payoutBatchService.GetPayoutVendors(vendorId, status);

            var payoutsPaged = new PagedList<PayoutVendorDTO>(payouts, pageIndex, pageSize);

            foreach (var p in payoutsPaged)
            {
                var payoutModel = new VendorPayoutListModel.PayoutModel
                {
                    PayoutGroupId = p.PayoutGroupId,
                    Date = p.Date,
                    Amount = _priceFormatter.FormatPrice(p.Amount),
                    StatusId = p.StatusId,
                    NumberOfOrders = p.NumberOfOrders,
                    PayoutBatchId = p.PayoutBatchId,
                    PayoutBatchIdText = $"PB{p.PayoutBatchId}"
                };
                model.Payouts.Add(payoutModel);
            }
            model.TotalPayout = _payoutBatchService.GetPayoutVendorsTotal(vendorId, status);

            var pagerModel = new PagerModel
            {
                PageSize = pageSize,
                TotalRecords = payouts.Count(),
                PageIndex = pageIndex,
                ShowTotalSummary = false,
                UseRouteLinks = false
            };

            model.PagerModel = pagerModel;

            return model;
        }

        public virtual VendorPayoutDetailsModel PrepareVendorPayoutDetailsApiModel(VendorPayoutDetailsModel model, PayoutGroupDTO payoutGroup, bool excludeProperties = false)
        {
            if (payoutGroup != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = new VendorPayoutDetailsModel();
                    model.PayoutGroupId = payoutGroup.PayoutGroupId;
                    model.Date = payoutGroup.CreatedOn;
                    model.StatusId = payoutGroup.Status;
                    model.Amount = _priceFormatter.FormatPrice(payoutGroup.Amount);
                    model.BankName = payoutGroup.BankName;
                    model.BankAccount = payoutGroup.AccountNumber;
                    model.Remarks = payoutGroup.Remarks;
                    model.NumberOfOrders = payoutGroup.RequestCount;
                    model.PayoutBatchId = payoutGroup.PayoutBatchId;
                    model.PayoutBatchIdText = $"PB{payoutGroup.PayoutBatchId}";
                }
            }
            return model;
        }

        public virtual VendorPayoutListModel PreparePayoutVendorOrderListApiModel(
            int payoutGroupId,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var model = new VendorPayoutListModel();

            var payoutOrders = _payoutBatchService.GetPayoutVendorOrders(payoutGroupId);

            var payoutOrdersPaged = new PagedList<PayoutVendorDTO>(payoutOrders, pageIndex, pageSize);

            foreach (var p in payoutOrdersPaged)
            {
                var payoutModel = new VendorPayoutListModel.PayoutOrderModel
                {
                    OrderId = p.OrderId,
                    Date = p.Date,
                    Amount = _priceFormatter.FormatPrice(p.Amount),
                };
                model.PayoutOrders.Add(payoutModel);
            }

            return model;
        }
    }
}