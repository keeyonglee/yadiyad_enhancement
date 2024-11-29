using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.AdminDashboard;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.AdminDashboard;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public partial class AdminDashboardController : BaseAdminController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IAdminDashboardService _adminDashboardService;
        private readonly IAdminDashboardModelFactory _adminDashboardModelFactory;
        private readonly AdminDashboardSettings _adminDashboardSettings;

        #endregion

        #region Ctor

        public AdminDashboardController(
           IWorkContext workContext,
           IAdminDashboardService adminDashboardService,
           IAdminDashboardModelFactory adminDashboardModelFactory,
           AdminDashboardSettings adminDashboardSettings)
        {
            _workContext = workContext;
            _adminDashboardService = adminDashboardService;
            _adminDashboardModelFactory = adminDashboardModelFactory;
            _adminDashboardSettings = adminDashboardSettings;
        }

        #endregion

        #region Methods

        #region Main

        public virtual IActionResult LoadMemberSignupStatistics(string period)
        {

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var result = new List<object>();

            switch (period)
            {
                case "month":
                    var thisMonthDT = DateTime.UtcNow;
                    var lastMonthDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "This",
                        value = _adminDashboardService.GetMemberSignUpMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Last",
                        value = _adminDashboardService.GetMemberSignUpMonthly(lastMonthDT)
                    });

                    break;
                case "week":
                default:
                    var thisWeekDT = DateTime.UtcNow;
                    var lastWeekDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "This",
                        value = _adminDashboardService.GetMemberSignUpWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Last",
                        value = _adminDashboardService.GetMemberSignUpWeekly(lastWeekDT)
                    });
                    break;
            }

            return Json(result);
        }

        public virtual IActionResult LoadOtherSignupStatistics(string period)
        {

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var result = new List<object>();

            switch (period)
            {
                case "month":
                    var thisMonthDT = DateTime.UtcNow;
                    var lastMonthDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "Pro Organization",
                        value = _adminDashboardService.GetProOrganizationSignUpMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Pro Organization",
                        value = _adminDashboardService.GetProOrganizationSignUpMonthly(lastMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq Merchant",
                        value = _adminDashboardService.GetShuqVendorSignUpMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq Merchant",
                        value = _adminDashboardService.GetShuqVendorSignUpMonthly(lastMonthDT)
                    });
                    break;
                case "week":
                default:
                    var thisWeekDT = DateTime.UtcNow;
                    var lastWeekDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "Pro Organization",
                        value = _adminDashboardService.GetProOrganizationSignUpWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Pro Organization",
                        value = _adminDashboardService.GetProOrganizationSignUpWeekly(lastWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq Merchant",
                        value = _adminDashboardService.GetShuqVendorSignUpWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq Merchant",
                        value = _adminDashboardService.GetShuqVendorSignUpWeekly(lastWeekDT)
                    });
                    break;
            }

            return Json(result);
        }

        public virtual IActionResult LoadTransactionValue(string period)
        {

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var result = new List<object>();

            switch (period)
            {
                case "month":
                    var thisMonthDT = DateTime.UtcNow;
                    var lastMonthDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetTransactionValueProMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetTransactionValueProMonthly(lastMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetTransactionValueShuqMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetTransactionValueShuqMonthly(lastMonthDT)
                    });
                    break;
                case "week":
                default:
                    var thisWeekDT = DateTime.UtcNow;
                    var lastWeekDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetTransactionValueProWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetTransactionValueProWeekly(lastWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetTransactionValueShuqWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetTransactionValueShuqWeekly(lastWeekDT)
                    });
                    break;
            }

            return Json(result);
        }

        public virtual IActionResult LoadServiceCharge(string period)
        {

            //a vendor doesn't have access to this report
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var result = new List<object>();

            switch (period)
            {
                case "month":
                    var thisMonthDT = DateTime.UtcNow;
                    var lastMonthDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetServiceChargeProMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetServiceChargeProMonthly(lastMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetServiceChargeShuqMonthly(thisMonthDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetServiceChargeShuqMonthly(lastMonthDT)
                    });
                    break;
                case "week":
                default:
                    var thisWeekDT = DateTime.UtcNow;
                    var lastWeekDT = DateTime.UtcNow.AddMonths(-1);
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetServiceChargeProWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Pro",
                        value = _adminDashboardService.GetServiceChargeProWeekly(lastWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetServiceChargeShuqWeekly(thisWeekDT)
                    });
                    result.Add(new
                    {
                        date = "Shuq",
                        value = _adminDashboardService.GetServiceChargeShuqWeekly(lastWeekDT)
                    });
                    break;
            }

            return Json(result);
        }

        #endregion

        #region Pro

        public virtual IActionResult LoadTopJobCVSegments(string period)
        {
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var colorCodes = _adminDashboardSettings.CategoryChartColourMapping;

            var result = new List<TopSegmentsChartModel>();

            switch (period)
            {
                case "profile":
                    result = _adminDashboardService.GetTopJobCVSegments(colorCodes);

                    break;
                case "charge":
                default:
                    result = _adminDashboardService.GetTopJobCVSegmentsCharge(colorCodes);

                    break;
            }

            return Json(result);
        }

        public virtual IActionResult LoadTopServiceSegments(string period)
        {
            if (_workContext.CurrentVendor != null)
                return Content(string.Empty);

            var colorCodes = _adminDashboardSettings.CategoryChartColourMapping;

            var result = new List<TopSegmentsChartModel>();

            switch (period)
            {
                case "profile":
                    result = _adminDashboardService.GetTopServiceSegments(colorCodes);

                    break;
                case "charge":
                default:
                    result = _adminDashboardService.GetTopServiceSegmentsCharge(colorCodes);

                    break;
            }

            return Json(result);
        }

        [HttpPost]
        public virtual IActionResult TopOrganizationsList(TopOrganizationsSearchModel searchModel)
        {

            //prepare model
            var model = _adminDashboardModelFactory.PrepareTopOrganizationsListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult TopIndividualsList(TopOrganizationsSearchModel searchModel)
        {

            //prepare model
            var model = _adminDashboardModelFactory.PrepareTopIndividualsListModel(searchModel);

            return Json(model);
        }

        #endregion

        #endregion
    }
}
