using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Models.ApproveDepositRequest;
using Nop.Web.Areas.Pro.Models.CampaignManagement;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Services.Deposit;

namespace Nop.Web.Areas.Pro.Factories
{
    public class ApproveDepositRequestModelFactory
    {
        #region Fields

        private readonly DepositRequestService _depositRequestService;
        private readonly BankService _bankService;
        #endregion

        #region Ctor

        public ApproveDepositRequestModelFactory(
            DepositRequestService depositRequestService,
            BankService bankService)
        {
            _depositRequestService = depositRequestService;
            _bankService = bankService;
        }
        #endregion

        #region Methods

        public virtual ApproveDepositRequestSearchModel PrepareSearchModel(ApproveDepositRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return searchModel;
        }

        public virtual ApproveDepositRequestListModel PrepareListModel(ApproveDepositRequestSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            var deposit = _depositRequestService.SearchApproveDepositRequestTable(searchModel.From, searchModel.Until, searchModel.StatusId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new ApproveDepositRequestListModel().PrepareToGrid(searchModel, deposit, () =>
            {
                return deposit.Select(entity =>
                {
                    var approveModel = new ApproveDepositRequestModel();
                    approveModel.BankInDate = entity.BankInDate;
                    approveModel.Id = entity.Id;
                    approveModel.EngagementId = entity.RefId;
                    approveModel.Total = entity.Amount;
                    approveModel.Reference = entity.BankInReference;
                    approveModel.BankInSlipDownloadId = entity.BankInSlipDownloadId;
                    approveModel.Bank = entity.BankId != null ? _bankService.GetBankById(entity.BankId.Value).Name : "-";
                    approveModel.DepositStatus = ((DepositRequestStatus)entity.Status).GetDescription();
                    approveModel.BankInDateText = entity.BankInDate != null ? entity.BankInDate.Value.ToShortDateString() : "-";
                    return approveModel;
                });
            });
            return model;
        }

        public virtual ApproveDepositRequestModel PrepareModel(ApproveDepositRequestModel model, DepositRequest deposit, bool excludeProperties = false)
        {
            if (deposit != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = deposit.ToModel<ApproveDepositRequestModel>();
                    model.BankInDate = deposit.BankInDate;
                    model.EngagementId = deposit.RefId;
                    model.Total = deposit.Amount;
                    model.Reference = deposit.BankInReference;
                    model.BankInSlipDownloadId = deposit.BankInSlipDownloadId;
                    model.Bank = deposit.BankId != null ? _bankService.GetBankById(deposit.BankId.Value).Name : "-";
                    model.DepositStatus = ((DepositRequestStatus)deposit.Status).GetDescription();
                    model.BankInDateText = deposit.BankInDate != null ? deposit.BankInDate.Value.ToShortDateString() : "-";
                    model.ApproveRemarks = "";
                }
            }
            return model;
        }

        #endregion
    }
}
