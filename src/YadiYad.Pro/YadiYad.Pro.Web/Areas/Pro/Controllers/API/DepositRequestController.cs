using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Services.Services.Operator;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.DataTables;
using static YadiYad.Pro.Web.Models.DataTables.DataTableAjaxPostModel;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class DepositRequestController : BaseController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly DepositRequestService _depositRequestService;
        private readonly OrderProcessingService _orderProcessingService;
        private readonly OrderService _orderService;
        private readonly IOperatorService _operatorService;

        #endregion

        #region Ctor

        public DepositRequestController(
            IWorkContext workContext,
            ProWorkflowMessageService proWorkflowMessageService,
            DepositRequestService depositRequestService,
            OrderService orderService,
            OrderProcessingService orderProcessingService,
            IOperatorService operatorService)
        {
            _workContext = workContext;
            _proWorkflowMessageService = proWorkflowMessageService;
            _depositRequestService = depositRequestService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _operatorService = operatorService;

        }

        #endregion

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.OrganizationJob)
            )]
        [HttpPost("jobapplication/{id}")]
        public IActionResult GetPayoutRequestByJobApplicationId(
            [FromRoute(Name = "Id")] int jobApplicationId,
            DataTableRequestModel<DepositRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new DepositRequestFilterDTO();
            model.CustomFilter.RefId = jobApplicationId;
            model.CustomFilter.ProductTypeId = (int)ProductType.JobEnagegementFee;
            model.CustomFilter.CustomerId = customerId;

            var recordDTO = _depositRequestService.GetDepositRequests(
                model.Start,
                model.Length,
                model.CustomFilter);

            response.SetResponse(recordDTO);

            return Ok(response);
        }


        [AuthorizeAccess(
            nameof(StandardPermissionProvider.IndividualService)
            )]
        [HttpPost("serviceApplication/{id}")]
        public IActionResult GetPayoutRequestByServiceApplicationId(
            [FromRoute(Name = "Id")] int serviceApplicationId,
            DataTableRequestModel<DepositRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new DepositRequestFilterDTO();
            model.CustomFilter.RefId = serviceApplicationId;
            model.CustomFilter.ProductTypeId = (int)ProductType.ServiceEnagegementFee;
            model.CustomFilter.CustomerId = customerId;

            var recordDTO = _depositRequestService.GetDepositRequests(
                model.Start,
                model.Length,
                model.CustomFilter);

            response.SetResponse(recordDTO);

            return Ok(response);
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.ConsultationConfirmed)
            )]
        [HttpPost("consultationinvitation/{id}")]
        public IActionResult GetPayoutRequestByConsultationInvitationId(
            [FromRoute(Name = "Id")] int consultationInvitationId,
            DataTableRequestModel<DepositRequestFilterDTO> model)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            model.CustomFilter = new DepositRequestFilterDTO();
            model.CustomFilter.RefId = consultationInvitationId;
            model.CustomFilter.ProductTypeId = (int)ProductType.ConsultationEngagementFee;
            model.CustomFilter.CustomerId = customerId;

            var recordDTO = _depositRequestService.GetDepositRequests(
                model.Start,
                model.Length,
                model.CustomFilter);

            response.SetResponse(recordDTO);

            return Ok(response);
        }

        //[AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        //[HttpGet("jobapplication/{id}/detail")]
        //public IActionResult GetDepositRequestDetailByJobApplicationId([FromRoute(Name = "Id")] int jobApplicationId)
        //{
        //    var response = new ResponseDTO();
        //    var data = _depositRequestService.GetDepositDetails(jobApplicationId, (int)ProductType.HireCandidate);
        //    response.SetResponse(data);
        //    return Ok(response);
        //}

        //[AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        //[HttpGet("serviceApplication/{id}/detail")]
        //public IActionResult GetDepositRequestDetailByServiceApplicationId([FromRoute(Name = "Id")] int serviceApplicationId)
        //{
        //    var response = new ResponseDTO();
        //    var data = _depositRequestService.GetDepositDetails(serviceApplicationId, (int)ProductType.ServiceEscrow);
        //    response.SetResponse(data);
        //    return Ok(response);
        //}

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("projectbasedjob/{id}")]
        public IActionResult CreateUpdateDeposit(
            [FromRoute(Name = "Id")] int jobApplicationId,
            [FromBody]DepositRequestDTO dto)
        {
            var response = new ResponseDTO();
            var actorId = _workContext.CurrentCustomer.Id;

            var depositRequestDTO = _depositRequestService.CreateUpdateProjectBasedDepositRequest(actorId, jobApplicationId, dto);
            var offsetableOrderItem = _orderService.GetOrderItemBForOffsetByProductTypeRefId(actorId, ProductType.JobEnagegementFee, jobApplicationId);

            if (offsetableOrderItem != null)
            {
                var orderDTO = _orderProcessingService.CreateOrder(new SubmitOrderDTO
                {
                    RefId = jobApplicationId,
                    ProductTypeId = (int)ProductType.JobEnagegementFee,
                    DepositRequestId = depositRequestDTO.Id
                });

                var proOrder = _orderService.GetCustomOrder(orderDTO.Id);

                proOrder.OrderStatus = OrderStatus.Complete;
                proOrder.PaymentStatus = PaymentStatus.Paid;

                _orderProcessingService.ProcessOrder(proOrder);

                depositRequestDTO.Status = (int)DepositRequestStatus.Paid;
            }
            else
            {
                var opIds = _operatorService.GetAllOperatorCustomerIds();
                foreach (var id in opIds)
                {
                    _proWorkflowMessageService.SendDepositRequestPaymentVerificationNotification(_workContext.WorkingLanguage.Id, depositRequestDTO, id);
                }
            }


            response.SetResponse(depositRequestDTO);

            return Ok(response);
        }

    }
}
