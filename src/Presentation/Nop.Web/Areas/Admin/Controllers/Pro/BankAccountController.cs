using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.BankAccount;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.YadiyadNews;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Web.FactoriesPro;
using YadiYad.Pro.Web.Models.News;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{


    public class BankAccountController : BaseAdminController
    {
        #region Fields
        private readonly BankAccountService _bankAccountService;
        private readonly BankAccountModelFactory _bankAccountFactory;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private LocalizationSettings localizationSettings;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public BankAccountController(BankAccountService bankAccountService,
            BankAccountModelFactory bankAccountFactory,
            IPermissionService permissionService,
            IWorkflowMessageService workflowMessageService,
            ICustomerService customerService,
            IWorkContext workContext)
        {
            _bankAccountService = bankAccountService;
            _bankAccountFactory = bankAccountFactory;
            _customerService = customerService;
            _permissionService = permissionService;
            _workflowMessageService = workflowMessageService;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("BankAccount");
        }

        public virtual IActionResult BankAccount(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBankAccounts))
                return AccessDeniedView();

            //prepare model
            var model = _bankAccountFactory.PrepareBankAccountSearchModel(new BankAccountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(BankAccountSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBankAccounts))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _bankAccountFactory.PrepareBankAccountListModel(searchModel);


            return Json(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult BankAccountEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBankAccounts))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var bankaccountItem = _bankAccountService.GetBankAccountById(id);
            if (bankaccountItem == null)
                return RedirectToAction("BankAccount");

            //prepare model
            var model = _bankAccountFactory.PrepareBankAccountModel(null, bankaccountItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(BankAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBankAccounts))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var bankaccountItem = _bankAccountService.GetBankAccountById(model.Id);
            if (bankaccountItem == null)
                return RedirectToAction("BankAccount");

            if (ModelState.IsValid)
            {
                //bankaccountItem = model.ToEntity(bankaccountItem);
                //_yadiyadNewsService.Update(newsItem);

                bankaccountItem.Comment = model.Comment;
                bankaccountItem.IsVerified = true;
                _bankAccountService.UpdateBankAccountById(_workContext.CurrentCustomer.Id, bankaccountItem);
        
                var customer = _bankAccountService.GetCustomerByBankAccountId(model.Id);
                if (customer != null)
                    _workflowMessageService.SendBankApprovalEmail(customer, _workContext.WorkingLanguage.Id);
                if (!continueEditing)
                    return RedirectToAction("BankAccount");

                return RedirectToAction("BankAccountEdit", new { id = bankaccountItem.Id });
            }

            //prepare model
            model = _bankAccountFactory.PrepareBankAccountModel(model, bankaccountItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }


        [HttpPost, ActionName("Edit")]
        [FormValueRequired("bankaccount-reject")]
        public virtual IActionResult Reject(BankAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBankAccounts))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var bankaccountItem = _bankAccountService.GetBankAccountById(model.Id);
            if (bankaccountItem == null)
                return RedirectToAction("BankAccount");

            if (ModelState.IsValid)
            {
                //bankaccountItem = model.ToEntity(bankaccountItem);
                //_yadiyadNewsService.Update(newsItem);
                bankaccountItem.Comment = model.Comment;
                bankaccountItem.IsVerified = false;
                _bankAccountService.UpdateBankAccountById(_workContext.CurrentCustomer.Id, bankaccountItem);

                var customer = _bankAccountService.GetCustomerByBankAccountId(model.Id);
                if (customer != null)
                    _workflowMessageService.SendBankRejectedEmail(customer, _workContext.WorkingLanguage.Id);

                return RedirectToAction("BankAccount");
            };

            //prepare model
            model = _bankAccountFactory.PrepareBankAccountModel(model, bankaccountItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion
    }
}
