using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Dashboard;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.DataTables;
using static YadiYad.Pro.Web.Models.DataTables.DataTableAjaxPostModel;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Services.Dashboard;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class AccountController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly DashboardService _dashboardService;
        private readonly RefundRequestService _refundRequestService;

        #endregion

        #region Ctor

        public AccountController(
            IWorkContext workContext,
            DashboardService dashboardService,
            RefundRequestService refundRequestService)
        {
            _workContext = workContext;
            _dashboardService = dashboardService;
            _refundRequestService = refundRequestService;
        }

        #endregion

        [HttpGet("Dashboard")]
        public virtual IActionResult GetDashboardData()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;

            var jobPendingRematch = _dashboardService.GetNoOfJobPendingRematch(customerId);
            var consultantJobPendingRematch = _dashboardService.GetNoOfConsultantJobPendingRematch(customerId);
            var depositReserve = _dashboardService.GetDepositReserve(customerId);
            var depositDue = _dashboardService.GetDepositDue(customerId);
            var newApplicant = _dashboardService.GetNewApplicant(customerId);

            var result = new DashboardCardDTO();


            result.NoOfJobPendingRematch = jobPendingRematch;
            result.NoOfConsultantJobPendingRematch = consultantJobPendingRematch;
            result.DepositReserve = depositReserve;
            result.DepositDue = depositDue;
            result.NewApplicant = newApplicant;
            

            response.SetResponse(result);
            return Ok(response);
        }

        [HttpPost("JobEngagement")]
        public IActionResult GetJobEngagementById(
            DataTableRequestModel<DashboardJobEngagementRequestFilter> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new DashboardJobEngagementRequestFilter();
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _dashboardService.GetJobEngagementList(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<DashboardJobEngagementDTO>
            {
                draw = model.Draw,
                recordsTotal = recordDTO.TotalCount,
                recordsFiltered = recordDTO.TotalCount,
                data = recordDTO.Data.ToList(),
                AdditionalData = recordDTO.AdditionalData
            };

            response.SetResponse(dtResponseDTO);

            return Ok(response);
        }

        [HttpPost("ConsultantEngagement")]
        public IActionResult GetConsultantEngagementById(
        DataTableRequestModel<DashboardJobEngagementRequestFilter> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new DashboardJobEngagementRequestFilter();
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _dashboardService.GetConsultationEngagementList(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<DashboardConsultationEngagementDTO>
            {
                draw = model.Draw,
                recordsTotal = recordDTO.TotalCount,
                recordsFiltered = recordDTO.TotalCount,
                data = recordDTO.Data.ToList(),
                AdditionalData = recordDTO.AdditionalData
            };

            response.SetResponse(dtResponseDTO);

            return Ok(response);
        }



        public IActionResult Index()
        {
            return View();
        }
    }
}
