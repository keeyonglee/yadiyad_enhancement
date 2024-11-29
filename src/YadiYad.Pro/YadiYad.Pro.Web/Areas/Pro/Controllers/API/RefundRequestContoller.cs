using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System.Linq;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.DataTables;
using static YadiYad.Pro.Web.Models.DataTables.DataTableAjaxPostModel;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Services.Engagement;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Services.Refund;
using System.Collections.Generic;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class RefundRequestController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly InvoiceService _invoiceService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly EngagementService _engagementService;
        private readonly RefundRequestService _refundRequestService;

        #endregion

        #region Ctor

        public RefundRequestController(
            IWorkContext workContext,
            AccountContext accountContext,
            InvoiceService invoiceService,
            FeeCalculationService feeCalculationService,
            EngagementService engagementService,
            RefundRequestService refundRequestService)
        {
            _workContext = workContext;
            _accountContext = accountContext;
            _invoiceService = invoiceService;
            _feeCalculationService = feeCalculationService;
            _engagementService = engagementService;
            _refundRequestService = refundRequestService;
        }

        #endregion

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("serviceapplication/{id}")]
        public IActionResult GetPayoutRequestByServiceApplicationId(
            [FromRoute(Name = "Id")] int serviceApplicationId,
            DataTableRequestModel<RefundRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new RefundRequestFilterDTO();
            model.CustomFilter.RefId = serviceApplicationId;
            model.CustomFilter.ProductTypeIDs = new List<int>{
                (int)ProductType.ServiceEnagegementFee
            };
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _refundRequestService.GetRefundRequestByRefId(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<RefundRequestDTO>
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
            DataTableRequestModel<RefundRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new RefundRequestFilterDTO();
            model.CustomFilter.RefId = jobApplicationId;
            model.CustomFilter.ProductTypeIDs = new List<int>{
                (int)ProductType.JobEnagegementFee
            };
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _refundRequestService.GetRefundRequestByRefId(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<RefundRequestDTO>
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

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("ConsultationInvitation/{id}")]
        public IActionResult GetPayoutRequestByConsultationInvitationId(
            [FromRoute(Name = "Id")] int consultationInvitationId,
            DataTableRequestModel<RefundRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new RefundRequestFilterDTO();
            model.CustomFilter.RefId = consultationInvitationId;
            model.CustomFilter.ProductTypeIDs = new List<int>{
                (int)ProductType.ConsultationEngagementFee,
                (int)ProductType.ConsultationEngagementMatchingFee
            };
            model.CustomFilter.CustomerId = customerId;
            var recordDTO = _refundRequestService.GetRefundRequestByRefId(model.Start, model.Length, model.CustomFilter);

            var dtResponseDTO = new DataTableResponseModel<RefundRequestDTO>
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
    }
}
