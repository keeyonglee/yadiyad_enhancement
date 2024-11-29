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
using YadiYad.Pro.Core.Domain.Common;

namespace Nop.Web.Areas.Admin.Factories
{
    public class BankAccountModelFactory
    {
        #region Fields

        private readonly BankAccountService _bankAccountserviceService;


        #endregion

        #region Ctor
        public BankAccountModelFactory(BankAccountService bankAccountserviceService)
        {
            _bankAccountserviceService = bankAccountserviceService;
        }
        #endregion
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual BankAccountSearchModel PrepareBankAccountSearchModel(BankAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category list model</returns>
        public virtual BankAccountListModel PrepareBankAccountListModel(BankAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));


            var response = new ResponseDTO();
            var filterDto = new BankAccountSearchFilterDTO();

            filterDto.AccountHolderName = searchModel.AccountHolderName;

            var dto = _bankAccountserviceService.SearchBankAccountsAdmin(searchModel.AccountHolderName, searchModel.IsVerified,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new BankAccountListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(bankaccount =>
                {
                    var bankaccountModel = new BankAccountModel();
                    bankaccountModel.Id = bankaccount.Id;
                    bankaccountModel.CustomerId = bankaccount.CustomerId;
                    bankaccountModel.BankId = bankaccount.BankId;
                    bankaccountModel.AccountNumber = bankaccount.AccountNumber;
                    bankaccountModel.AccountHolderName = bankaccount.AccountHolderName;
                    bankaccountModel.BankStatementDownloadId = bankaccount.BankStatementDownloadId;
                    bankaccountModel.IsVerified = bankaccount.IsVerified;
                    bankaccountModel.Status = bankaccount.Status;
                    bankaccountModel.CreatedById = bankaccount.CreatedById;
                    bankaccountModel.UpdatedById = bankaccount.UpdatedById;
                    bankaccountModel.CreatedOnUTC = bankaccount.CreatedOnUTC;
                    bankaccountModel.UpdatedOnUTC = bankaccount.UpdatedOnUTC;
                    bankaccountModel.BankName = bankaccount.BankName;
                    bankaccountModel.FileName = bankaccount.FileName;
                    bankaccountModel.Extension = bankaccount.Extension;
                    bankaccountModel.BankStatementDownloadGuid = bankaccount.BankStatementDownloadGuid;
                    bankaccountModel.IdentityType = bankaccount.IdentityType;
                    bankaccountModel.IdentityTypeName = ((IdentityType)bankaccount.IdentityType).GetDescription();
                    bankaccountModel.Identity = bankaccount.Identity;
                    return bankaccountModel;
                });
            });
            return model;

        }

        public virtual BankAccountModel PrepareBankAccountModel(BankAccountModel model, BankAccountDTO bankaccount, bool excludeProperties = false)
        {
            if (bankaccount != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = new BankAccountModel();
                    model.Id = bankaccount.Id;
                    model.CustomerId = bankaccount.CustomerId;
                    model.BankId = bankaccount.BankId;
                    model.AccountNumber = bankaccount.AccountNumber;
                    model.AccountHolderName = bankaccount.AccountHolderName;
                    model.BankStatementDownloadId = bankaccount.BankStatementDownloadId;
                    model.IsVerified = bankaccount.IsVerified;
                    model.Status = bankaccount.Status;
                    model.CreatedById = bankaccount.CreatedById;
                    model.UpdatedById = bankaccount.UpdatedById;
                    model.CreatedOnUTC = bankaccount.CreatedOnUTC;
                    model.UpdatedOnUTC = bankaccount.UpdatedOnUTC;
                    model.Comment = bankaccount.Comment;
                    model.BankName = bankaccount.BankName;
                    model.FileName = bankaccount.FileName;
                    model.Extension = bankaccount.Extension;
                    model.BankStatementDownloadGuid = bankaccount.BankStatementDownloadGuid;
                    model.IdentityType = bankaccount.IdentityType;
                    model.IdentityTypeName = ((IdentityType)bankaccount.IdentityType).GetDescription();
                    model.Identity = bankaccount.Identity;
                }
            }
            return model;
        }


    }
}
