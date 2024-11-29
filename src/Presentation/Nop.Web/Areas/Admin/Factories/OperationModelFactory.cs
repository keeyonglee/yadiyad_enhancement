using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Operation;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Individual;

namespace Nop.Web.Areas.Admin.Factories
{
    public class OperationModelFactory
    {
        #region Fields

        private readonly IVendorApplicationService _vendorApplicationService;
        private readonly IProductService _productService;
        private readonly BankAccountService _bankAccountService;


        #endregion

        #region Ctor

        public OperationModelFactory(IVendorApplicationService vendorApplicationService,
            IProductService productService,
            BankAccountService bankAccountService)
        {
            _vendorApplicationService = vendorApplicationService;
            _productService = productService;
            _bankAccountService = bankAccountService;
        }

        #endregion

        #region Methods

        public virtual OperationSearchModel PrepareOperationSearchModel(OperationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }


        public virtual OperationListModel PrepareNewVendorApplicationListModel(OperationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _vendorApplicationService.GetAllVendorApplicationsPendingApproval(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new OperationListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new OperationModel();
                    model.Id = x.Id;
                    model.StoreName = x.StoreName;
                    model.CreatedDateTime = x.CreatedOnUtc;
                    model.BusinessNatureCategoryId = x.BusinessNatureCategoryId;
                    model.CustomerId = x.CustomerId;
                    return model;
                });
            });
            return model;

        }

        public virtual OperationListModel PrepareNewProductsPublishListModel(OperationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _productService.GetProductsNotApproved(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new OperationListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new OperationModel();
                    model.Id = x.Id;
                    model.ProductName = x.Name;
                    model.CreatedDateTime = x.CreatedOnUtc;
                    model.VendorId = x.VendorId;
                    return model;
                });
            });
            return model;

        }

        public virtual OperationListModel PrepareNewBankAccountsListModel(OperationSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var dto = _bankAccountService.GetBankAccountsUnverified(pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            var model = new OperationListModel().PrepareToGrid(searchModel, dto, () =>
            {
                return dto.Select(x =>
                {
                    var model = new OperationModel();
                    model.Id = x.Id;
                    model.AccountHolderName = x.AccountHolderName;
                    model.CreatedDateTime = x.CreatedOnUTC;
                    model.BankName = x.BankName;
                    model.Comment = x.Comment;
                    return model;
                });
            });
            return model;

        }

        #endregion
    }
}
