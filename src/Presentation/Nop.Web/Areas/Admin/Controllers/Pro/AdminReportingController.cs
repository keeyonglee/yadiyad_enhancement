using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.ExportImport;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.YadiyadReporting;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Reportings;
using System;
using System.Linq;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class AdminReportingController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ContactsModelFactory _contactsModelFactory;
        private readonly IExportManager _exportManager;
        private readonly INotificationService _notificationService;
        private readonly IYadiyadReportingContactsService _yadiyadReportingContactsService; 

        #endregion

        #region Ctor

        public AdminReportingController(IPermissionService permissionService,
            ContactsModelFactory contactsModelFactory,
            IExportManager exportManager,
            INotificationService notificationService,
            IYadiyadReportingContactsService yadiyadReportingContactsService)
        {
            _permissionService = permissionService;
            _contactsModelFactory = contactsModelFactory;
            _exportManager = exportManager;
            _notificationService = notificationService;
            _yadiyadReportingContactsService = yadiyadReportingContactsService;

        }

        #endregion

        #region Methods

        #region Utilities

        public virtual IActionResult ExportXlsxOrganization(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportOrganizationToXlsx(_yadiyadReportingContactsService.GetAllOrganizationsExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "organizations.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsxRegistrationOnly(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportRegistrationOnlyToXlsx(_yadiyadReportingContactsService.GetAllRegistrationOnlyExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "registrations_only.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsxRegistrationProfile(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportRegistrationProfileToXlsx(_yadiyadReportingContactsService.GetAllRegistrationProfileExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "registrations_profile.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsxIndividualServiceJob(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportIndividualServiceJobToXlsx(_yadiyadReportingContactsService.GetAllIndividualServiceJobExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "individuals_service_job.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsxProRevenue(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportProRevenueToXlsx(_yadiyadReportingContactsService.GetAllProRevenueExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "pro_revenue.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public virtual IActionResult ExportXlsxProExpenses(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportProExpensesToXlsx(_yadiyadReportingContactsService.GetAllProExpensesExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "pro_expenses.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        
        public virtual IActionResult ExportXlsxShuqRevenue(DateTime? createdFrom, DateTime? createdTo)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager
                    .ExportShuqRevenueToXlsx(_yadiyadReportingContactsService.GetAllShuqRevenueExport(createdFrom, createdTo).ToList());

                return File(bytes, MimeTypes.TextXlsx, "shuq_revenue.xlsx");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }



        #endregion

        #region Contacts

        public IActionResult Index()
        {
            return RedirectToAction("Contacts");
        }

        public virtual IActionResult Contacts(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingContactsSearchModel(new ContactsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ContactsList(ContactsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingContactsListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult RegistrationOnlyList(ContactsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingRegistrationOnlyListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult RegistrationProfileList(ContactsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingRegistrationProfileListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult IndividualServiceJobList(ContactsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingIndividualServiceJobListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Pro

        public virtual IActionResult ProRevenue(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingProRevenueSearchModel(new RevenueExpenseSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProRevenueList(RevenueExpenseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingProRevenueListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ProExpenses(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingProExpenseSearchModel(new RevenueExpenseSearchModel());

            return View(model);
        }

        public virtual IActionResult ProExpensesList(RevenueExpenseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingProExpenseListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ShuqRevenue(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedView();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingShuqRevenueSearchModel(new RevenueExpenseSearchModel());

            return View(model);
        }

        public virtual IActionResult ShuqRevenueList(RevenueExpenseSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageReportings))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _contactsModelFactory.PrepareReportingShuqRevenueListModel(searchModel);

            return Json(model);
        }

        #endregion
        #endregion
    }
}
