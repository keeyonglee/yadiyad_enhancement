using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class ServiceApplicationController : BaseController
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly DepositRequestService _depositRequestService;
        private readonly PayoutRequestService _payoutRequestService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly RefundRequestService _refundRequestService;
        private readonly OrderService _orderService;

        private readonly EngagementCancellationManager _engagementCancellationManager;
        private readonly ProEngagementSettings _proEngagementSettings;
        private readonly IndividualProfileService _individualProfileService;
        #endregion

        #region Ctor

        public ServiceApplicationController(
            IMapper mapper,
            IWorkContext workContext,
            AccountContext accountContext,
            ServiceProfileService serviceProfileService,
            ServiceApplicationService serviceApplicationService,
            ProWorkflowMessageService proWorkflowMessageService,
            DepositRequestService depositRequestService,
            PayoutRequestService payoutRequestService,
            FeeCalculationService feeCalculationService,
            EngagementCancellationManager engagementCancellationManager,
            RefundRequestService refundRequestService,
            OrderService orderService,
            ProEngagementSettings proEngagementSettings,
            IndividualProfileService individualProfileService)
        {
            _mapper = mapper;
            _workContext = workContext;
            _accountContext = accountContext;
            _serviceProfileService = serviceProfileService;
            _serviceApplicationService = serviceApplicationService;
            _serviceProfileService = serviceProfileService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _depositRequestService = depositRequestService;
            _payoutRequestService = payoutRequestService;
            _feeCalculationService = feeCalculationService;
            _engagementCancellationManager = engagementCancellationManager;
            _refundRequestService = refundRequestService;
            _orderService = orderService;
            _proEngagementSettings = proEngagementSettings;
            _individualProfileService = individualProfileService;
        }

        #endregion

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("request")]
        public virtual IActionResult CreateServiceApplication([FromBody] ServiceApplicationDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            dto.IsEscrow = true;
            _serviceApplicationService.CreateServiceApplication(customerId, customerId, dto);
            _proWorkflowMessageService.SendServiceSellerRequest(dto, _workContext.WorkingLanguage.Id);
            response.SetResponse(ResponseStatusCode.Success);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("accept")]
        public virtual IActionResult AcceptServiceApplication([FromBody] ServiceApplicationDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            dto.Status = (int)ServiceApplicationStatus.Accepted;
            _serviceApplicationService.UpdateServiceApplicationStatus(dto.Id, customerId, ServiceApplicationStatus.Accepted);
            _proWorkflowMessageService.SendServiceBuyerAccepted(dto, _workContext.WorkingLanguage.Id);
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("decline")]
        public virtual IActionResult DeclineServiceApplication([FromBody] ServiceApplicationDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            dto.Status = (int)ServiceApplicationStatus.Rejected;
            _serviceApplicationService.UpdateServiceApplicationStatus(dto.Id, customerId, ServiceApplicationStatus.Rejected);
            _proWorkflowMessageService.SendServiceBuyerDeclined(dto, _workContext.WorkingLanguage.Id);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("payment")]
        public virtual IActionResult PaymentServiceApplication([FromBody] ServiceApplicationDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            dto.Status = (int)ServiceApplicationStatus.Paid;
            _serviceApplicationService.UpdateServiceApplicationStatus(dto.Id, customerId, ServiceApplicationStatus.Paid);
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("repropose")]
        public virtual IActionResult ReproposeServiceApplication([FromBody] ServiceApplicationDTO dto)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            dto.Status = (int)ServiceApplicationStatus.Reproposed;
            _serviceApplicationService.ReproposeServiceApplication(dto.Id, customerId, dto.StartDate, dto.Duration);
            _proWorkflowMessageService.SendServiceBuyerReproposed(dto, _workContext.WorkingLanguage.Id);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("requests")]
        public IActionResult GetRequestedServiceApplications([FromBody] ListFilterDTO<ServiceApplicationSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.RequesterCustomerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.Status = new List<int>();
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.New);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Accepted);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Rejected);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Reproposed);

            var data = _serviceApplicationService.SearchServiceApplications(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<ServiceApplicationDTO>(data);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("confirms")]
        public IActionResult GetConfirmedServiceApplications([FromBody] ListFilterDTO<ServiceApplicationSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.RequesterCustomerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.Status = new List<int>();
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Paid);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Completed);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.CancelledByBuyer);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.CancelledBySeller);
            //searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Completed);

            var data = _serviceApplicationService.SearchServiceApplications(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            if (data.Count > 0)
            {
                //process most recent service application
                data[0] = _serviceApplicationService.ProcessCancellationCount(data[0]);
            }

            var dto = new PagedListDTO<ServiceApplicationDTO>(data);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("receives")]
        public IActionResult GetReceivedServiceApplications([FromBody] ListFilterDTO<ServiceApplicationSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.ProviderCustomerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.Status = new List<int>();
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.New);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Reproposed);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Accepted);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Rejected);

            var data = _serviceApplicationService.SearchServiceApplications(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<ServiceApplicationDTO>(data);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPost("hires")]
        public IActionResult GetHiredServiceApplications([FromBody] ListFilterDTO<ServiceApplicationSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.ProviderCustomerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.Status = new List<int>();
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Paid);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.Completed);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.CancelledByBuyer);
            searchDTO.AdvancedFilter.Status.Add((int)ServiceApplicationStatus.CancelledBySeller);

            var data = _serviceApplicationService.SearchServiceApplications(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<ServiceApplicationDTO>(data);
            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpGet("buyer/counter")]
        public IActionResult GetServiceBuyerItemCounter()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var dto = _serviceApplicationService.GetServiceBuyerItemCounter(customerId);
            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpGet("seller/counter")]
        public IActionResult GetSellerItemCounter()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var dto = _serviceApplicationService.GetServiceSellerItemCounter(customerId);
            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPut("{id}/seller/read")]
        public IActionResult UpdateServiceSellerRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _serviceApplicationService.UpdateServiceSellerRead(id, customerId);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualService))]
        [HttpPut("{id}/buyer/read")]
        public IActionResult UpdateServiceBuyerRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _serviceApplicationService.UpdateServiceBuyerRead(id, customerId);

            return Ok(response);
        }

        [HttpGet("{id}/depositPayout")]
        public virtual IActionResult GetDepositPayoutByServiceApplicationId(int id)
        {
            var response = new ResponseDTO();
            var refId = id;
            var productTypeId = (int)ProductType.ServiceEnagegementFee;

            var depositData = _depositRequestService.GetDepositDetails(refId, productTypeId);
            var payoutData = _payoutRequestService.GetPayoutRequestSummary(refId, productTypeId);
            var feeData = _feeCalculationService.CalculatePayout(new PayoutRequestDTO()
            {
                RefId = refId,
                ProductTypeId = productTypeId
            });

            var data = new ServiceApplicationDepositPayoutDetailDTO()
            {
                Deposit = depositData,
                Payout = payoutData,
                EngagementFee = feeData
            };
            response.SetResponse(data);
            return Ok(response);
        }

        [HttpPut("{id}/terminate")]
        public virtual IActionResult UpdateServiceApplicationEndDate(int id, [FromBody] UpdateServiceApplicationEndDateDTO request)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid == false)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;

                var result = _serviceApplicationService
                    .UpdateServiceApplicationEndDate(
                    actorId,
                    id,
                    request);

                response.SetResponse(result);
            }

            return Ok(response);
        }

        [HttpPost("{id}/cancel")]
        public virtual IActionResult UpdateServiceApplicationCancel(int id, [FromBody] UpdateServiceApplicationCancelDTO request)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid == false)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;
                var actorName = _accountContext.CurrentAccount.Name;
                _engagementCancellationManager.CancelEngagement(id, EngagementType.Service, actorId, actorName, request.ReasonId, request.Remarks);
            }

            return Ok(response);
        }

        [HttpPost("{id}/refund")]
        public virtual IActionResult UpdateServiceApplicationRefund(int id)
        {
            var response = new ResponseDTO();
            var actorId = _workContext.CurrentCustomer.Id;
            var actorName = _accountContext.CurrentAccount.Name;

            var serviceApplication = _serviceApplicationService.GetServiceApplicationById(id);
            var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(serviceApplication.CustomerId);

            if (serviceApplication.Status != (int)ServiceApplicationStatus.CancelledBySeller)
            {
                return BadRequest("Fail to submit refund request due to service application is not cancelled by seller");

            }
            var refundRecord = _refundRequestService.GetByOrderItemId(id);
            if (refundRecord != null)
            {
                return BadRequest("Fail to submit refund request due to refund request already submitted");

            }
            var orderRecord = _orderService.GetOrderItem((int)ProductType.ServiceEnagegementFee, id);
            if (orderRecord.Status != (int)ProOrderItemStatus.OpenForRematch)
            {
                return BadRequest("Fail to submit refund request due to order is not open for rematched");

            }
            _orderService.SetOpenForRematchedOrderItemToPaid(actorId, orderRecord.Id);
            _refundRequestService.CreateRefundRequest(actorId, orderRecord.Id, orderRecord.Price);

            if (individualProfile.NumberOfCancellation == 1)
            {
                _serviceApplicationService.CancelRehire(serviceApplication.Id);
                _individualProfileService.ResetNumberOfCancellation(actorId, individualProfile);
            }

            response.SetResponse(true);
            return Ok(response);
        }
    }
}