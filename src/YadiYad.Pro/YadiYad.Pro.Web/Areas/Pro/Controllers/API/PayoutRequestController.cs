using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.DataTables;
using static YadiYad.Pro.Web.Models.DataTables.DataTableAjaxPostModel;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.Services.Engagement;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Job;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class PayoutRequestController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly PayoutRequestService _payoutRequestService;
        private readonly InvoiceService _invoiceService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly EngagementService _engagementService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly ServiceApplicationService _serviceApplicationService;

        #endregion

        #region Ctor

        public PayoutRequestController(
            IWorkContext workContext,
            AccountContext accountContext,
            InvoiceService invoiceService,
            PayoutRequestService payoutRequestService,
            FeeCalculationService feeCalculationService,
            EngagementService engagementService,
            JobApplicationService jobApplicationService,
            ServiceApplicationService serviceApplicationService)
        {
            _workContext = workContext;
            _accountContext = accountContext;
            _invoiceService = invoiceService;
            _payoutRequestService = payoutRequestService;
            _feeCalculationService = feeCalculationService;
            _engagementService = engagementService;
            _jobApplicationService = jobApplicationService;
            _serviceApplicationService = serviceApplicationService;
        }

        #endregion

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("serviceapplication/{id}")]
        public IActionResult GetPayoutRequestByServiceApplicationId(
            [FromRoute(Name = "Id")]int serviceApplicationId,
            DataTableRequestModel<PayoutRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new PayoutRequestFilterDTO();
            model.CustomFilter.RefId = serviceApplicationId;
            model.CustomFilter.ProductTypeID = (int)ProductType.ServiceEnagegementFee;
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _payoutRequestService.GetPayoutRequests(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<PayoutRequestDTO>
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

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualService),
            nameof(StandardPermissionProvider.OrganizationJob)
            )]
        [HttpPost("jobapplication/{id}")]
        public IActionResult GetPayoutRequestByJobApplicationId(
            [FromRoute(Name = "Id")] int jobApplicationId,
            DataTableRequestModel<PayoutRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new PayoutRequestFilterDTO();
            model.CustomFilter.RefId = jobApplicationId;
            model.CustomFilter.ProductTypeID = (int)ProductType.JobEnagegementFee;
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _payoutRequestService.GetPayoutRequests(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<PayoutRequestDTO>
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

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualProfile),
            nameof(StandardPermissionProvider.ConsultationConfirmed)
            )]
        [HttpPost("ConsultationInvitation/{id}")]
        public IActionResult GetPayoutRequestByConsultationInvitationId(
            [FromRoute(Name = "Id")] int consultationInvitationId,
            DataTableRequestModel<PayoutRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new PayoutRequestFilterDTO();
            model.CustomFilter.RefId = consultationInvitationId;
            model.CustomFilter.ProductTypeID = (int)ProductType.ConsultationEngagementFee;
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _payoutRequestService.GetPayoutRequests(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<PayoutRequestDTO>
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

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualService),
            nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("")]
        public IActionResult CreateUpdatePayoutRequest(
            [FromBody]PayoutRequestDTO dto)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid == false)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                var customerId = _workContext.CurrentCustomer.Id;
                var customerName = _accountContext.CurrentAccount.Name;
                _feeCalculationService.ProcessPayoutRequest(customerId, customerName, dto);
            }
            return Ok(response);
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualService),
            nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpGet("{id}")]
        public IActionResult GetPayoutRequest(
            [FromRoute]int id,
            [FromQuery] int refId,
            [FromQuery] int productTypeId)
        {
            var response = new ResponseDTO();

            var customerId = _workContext.CurrentCustomer.Id;
            var recordDTO = new PayoutRequestDTO();

            if (id == 0)
            {
                recordDTO = _payoutRequestService.GetNewPayoutRequest(customerId, productTypeId, refId);
            }
            else
            {
                recordDTO = _payoutRequestService.GetPayoutRequest(customerId, id);
            }

            response.SetResponse(recordDTO);

            return Ok(response);
        }

        //[HttpPost("{id}/paid")]
        //public IActionResult CreateUpdatePayoutRequest(
        //    [FromRoute] int id

        //    )
        //{
        //    var response = new ResponseDTO();
        //    var actorId = _workContext.CurrentCustomer.Id;

        //    var payoutrequest = _payoutRequestService.GetPayoutRequest(null, id);

        //    var engagement = _engagementService.GetEngagement(payoutrequest.ProductTypeId, payoutrequest.RefId);

        //    //if matched endDate or endMilestoneId or lastMilestoneId
        //    if (payoutrequest.ProductTypeId == (int)ProductType.JobEnagegementFee &&
        //        ((engagement.IsProjectPayout && (payoutrequest.JobMilestoneId == engagement.EndMilestoneId || payoutrequest.JobMilestoneId == engagement.LastMilestoneId)) ||
        //        (!engagement.IsProjectPayout && payoutrequest.EndDate == engagement.EndDate)))
        //    {
        //        _jobApplicationService.UpdateJobApplicationStatus(actorId, payoutrequest.RefId, JobApplicationStatus.Completed);
        //    }
        //    else if (payoutrequest.ProductTypeId == (int)ProductType.ServiceEnagegementFee &&
        //         payoutrequest.EndDate == engagement.EndDate)
        //    {
        //        _serviceApplicationService.UpdateServiceApplicationStatus(payoutrequest.RefId, actorId, (int)ServiceApplicationStatus.Completed);
        //    }

        //    var createdInvoice = _invoiceService.CreateInvoice(actorId, newInvoice);

        //    payoutrequest = _payoutRequestService.UpdatePayoutRequestPaidStatus(
        //        actorId,
        //        id,
        //        createdInvoice.Id,
        //        "Mannual update for testing purpose");

        //    response.SetResponse(payoutrequest);

        //    return Ok(response);
        //}


        //[AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        //[HttpGet("jobapplication/{id}/detail")]
        //public IActionResult GetPayoutRequestDetailByJobApplicationId([FromRoute(Name = "Id")] int jobApplicationId)
        //{
        //    var response = new ResponseDTO();
        //    var request = new PayoutRequestDTO()
        //    {
        //        RefId = jobApplicationId,
        //        ProductTypeId = (int)ProductType.HireCandidate
        //    };
        //    var feeData = _feeCalculationService.CalculateSalaryFee(request);
        //    var detailData = _payoutRequestService.GetPayoutDetails(jobApplicationId, (int)ProductType.HireCandidate);
        //    detailData.NextAmount = feeData.Fee;
        //    response.SetResponse(detailData);
        //    return Ok(response);
        //}

        //[AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        //[HttpGet("serviceApplication/{id}/detail")]
        //public IActionResult GetPayoutRequestDetailByServiceApplicationId([FromRoute(Name = "Id")] int serviceApplicationId)
        //{
        //    var response = new ResponseDTO();
        //    var request = new PayoutRequestDTO()
        //    {
        //        RefId = serviceApplicationId,
        //        ProductTypeId = (int)ProductType.ServiceEscrow
        //    };
        //    var feeData = _feeCalculationService.CalculateSalaryFee(request);
        //    var detailData = _payoutRequestService.GetPayoutDetails(serviceApplicationId, (int)ProductType.ServiceEscrow);
        //    detailData.NextAmount = feeData.Fee;
        //    response.SetResponse(detailData);
        //    return Ok(response);
        //}
    }
}
