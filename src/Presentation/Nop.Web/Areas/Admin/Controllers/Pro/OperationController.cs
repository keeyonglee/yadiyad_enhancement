using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Areas.Admin.Models.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class OperationController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly OperationModelFactory _operationModelFactory;

        #endregion

        #region Ctor

        public OperationController(IPermissionService permissionService,
            OperationModelFactory operationModelFactory)
        {
            _permissionService = permissionService;
            _operationModelFactory = operationModelFactory;
        }

        #endregion

        #region Methods

        //public virtual IActionResult Index()
        //{
        //    return View();
        //}

        [HttpPost]
        public virtual IActionResult NewVendorApplications(OperationSearchModel searchModel)
        {

            //prepare model
            var model = _operationModelFactory.PrepareNewVendorApplicationListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult NewProductsPublish(OperationSearchModel searchModel)
        {

            //prepare model
            var model = _operationModelFactory.PrepareNewProductsPublishListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult NewBankAccounts(OperationSearchModel searchModel)
        {

            //prepare model
            var model = _operationModelFactory.PrepareNewBankAccountsListModel(searchModel);

            return Json(model);
        }

        #endregion

    }
}
