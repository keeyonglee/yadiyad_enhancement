using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Caching;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Infrastructure.Cache;
using YadiYad.Pro.Web.Models.Moderator;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class ConsultationInvitationController : BaseController
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly ConsultationProfileService _consultationProfileService;
        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly ChargeService _chargeService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly EngagementCancellationManager _engagementCancellationManager;
        private readonly RefundRequestService _refundRequestService;
        private readonly OrderService _orderService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly IPermissionService _permissionService;
        #endregion

        #region Ctor

        public ConsultationInvitationController(
            IStaticCacheManager staticCacheManager,
            ICacheKeyService cacheKeyService,
            IWorkContext workContext,
            AccountContext accountContext,
            ConsultationProfileService consultationProfileService,
            ConsultationInvitationService consultationInvitationService,
            ServiceProfileService serviceProfileService,
            OrganizationProfileService organizationProfileService,
            ChargeService chargeService,
            ProWorkflowMessageService proWorkflowMessageService,
            FeeCalculationService feeCalculationService,
            EngagementCancellationManager engagementCancellationManager,
            RefundRequestService refundRequestService,
            IndividualProfileService individualProfileService,
            OrderService orderService,
            IPermissionService permissionService)
        {
            _staticCacheManager = staticCacheManager;
            _cacheKeyService = cacheKeyService;
            _workContext = workContext;
            _accountContext = accountContext;
            _consultationProfileService = consultationProfileService;
            _consultationInvitationService = consultationInvitationService;
            _serviceProfileService = serviceProfileService;
            _organizationProfileService = organizationProfileService;
            _chargeService = chargeService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _feeCalculationService = feeCalculationService;
            _engagementCancellationManager = engagementCancellationManager;
            _refundRequestService = refundRequestService;
            _orderService = orderService;
            _individualProfileService = individualProfileService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        [HttpPost("user/list")]
        public virtual IActionResult GetConsultationInvitationByCustomerId([FromBody] ListFilterDTO<ConsultationInvitationListingFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;
            if (searchDTO.AdvancedFilter == null) { searchDTO.AdvancedFilter = new ConsultationInvitationListingFilterDTO(); }

            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.IndividualCustomerId = customerId;
            var data = _consultationInvitationService.GetConsultationInvitations(
                pageIndex : searchDTO.Offset / searchDTO.RecordSize,
                pageSize : searchDTO.RecordSize,
                filterDTO: searchDTO.AdvancedFilter);
            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpPost("organization/list")]
        public virtual IActionResult GetConsultationInvitationByOrganizationProfileId([FromBody] ListFilterDTO<ConsultationInvitationListingFilterDTO> searchDTO)
        {
            if (searchDTO.AdvancedFilter == null) { searchDTO.AdvancedFilter = new ConsultationInvitationListingFilterDTO(); }

            var customerId = _workContext.CurrentCustomer.Id;

            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.OrganizationProfileId = _accountContext.CurrentAccount.OrganizationProfileId.Value;
            var data = _consultationInvitationService.GetConsultationInvitations(
                customerId,
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.AdvancedFilter);

            if (data != null && data.Data.Count() > 0)
            {
                var serviceCharges = _chargeService.GetLatestCharge((int)ProductType.ConsultationEngagementMatchingFee, 0);

                foreach (var invitation in data.Data)
                {
                    invitation.ServiceChargeRate = serviceCharges.Value;
                    invitation.ServiceChargeType = serviceCharges.ValueType;
                }
            }

            response.SetResponse(data);
            return Ok(response);
        }

        [HttpPost("operator/list")]
        public virtual IActionResult GetConsultationInvitations([FromBody] ListFilterDTO<ConsultationInvitationListingFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;
            if (searchDTO.AdvancedFilter == null) { searchDTO.AdvancedFilter = new ConsultationInvitationListingFilterDTO(); }

            var response = new ResponseDTO();
            var data = _consultationInvitationService.GetConsultationInvitations(
                pageIndex : searchDTO.Offset / searchDTO.RecordSize,
                pageSize : searchDTO.RecordSize,
                filterDTO : searchDTO.AdvancedFilter);
            response.SetResponse(data);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetConsultationInvitationById(int id)
        {
            var response = new ResponseDTO();
            var data = _consultationInvitationService.GetConsultationInvitationById(id);
            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("{id}")]
        public IActionResult SubmitConsultationInvitation(int id, [FromBody] ConsultationInvitationDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                var customerId = _workContext.CurrentCustomer.Id;
                var consultationProfile = _consultationProfileService.GetConsultationProfileById(dto.ConsultationProfileId);
                var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(customerId);
                dto.OrganizationProfileId = organizationProfile.Id;
                dto.ConsultationApplicationStatus = (int)ConsultationInvitationStatus.New;
                dto.Questionnaire = JsonConvert.SerializeObject(consultationProfile.Questions);
                dto.ModeratorCustomerId = _consultationInvitationService.GetModeratorWithLeastAssign().First();
                _consultationInvitationService.CreateConsultationInvitation(customerId, dto);
            }
            else
            {
                if (_accountContext.CurrentAccount.IsEntitledApplyJob == false
                    && dto.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted)
                {
                    return BadRequest("Please pay for Pay-to-Apply Jobs (PAJ).");
                }

                var customerId = _workContext.CurrentCustomer.Id;
                var existing = _consultationInvitationService.GetConsultationInvitationById(id);
                if (existing.ConsultationApplicationStatus != (int)ConsultationInvitationStatus.New
                    && (
                        dto.ConsultationApplicationStatus != (int)ConsultationInvitationStatus.Accepted
                        && dto.ConsultationApplicationStatus != (int)ConsultationInvitationStatus.DeclinedByIndividual
                    )
                    && (string.IsNullOrWhiteSpace(dto.QuestionnaireAnswer)
                        || string.IsNullOrWhiteSpace(dto.ConsultantAvailableTimeSlot)))
                {
                    //throw;
                }
                dto.Id = id;
                _consultationInvitationService.UpdateConsultationInvitation(customerId, dto);

                if(dto.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual)
                {
                    var consultInvitationDto = _consultationInvitationService.GetConsultationInvitationById(dto.Id);
                    var consultProfileDto = _consultationProfileService.GetConsultationProfileById(consultInvitationDto.ConsultationProfileId);
                    var individualProfileDto = _individualProfileService.GetIndividualProfileByCustomerId(consultInvitationDto.IndividualCustomerId);
                    _proWorkflowMessageService.SendConsultationOrganizationDeclined(
                        _workContext.WorkingLanguage.Id, consultProfileDto, individualProfileDto);
                }
            }

            UpdateJobCounterCache(_workContext.CurrentCustomer.Id);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpPost("{id}/review")]
        public IActionResult Review(int id, [FromBody] ConsultationInvitationReviewDTO dto)
        {
            var response = new ResponseDTO();

            _consultationInvitationService.ReviewConsultationInvitation(
                _workContext.CurrentCustomer.Id,
                id,
                dto);

            return Ok(response);
        }

        [HttpGet("list")]
        public virtual IActionResult GetConsultationInvitations()
        {
            var response = new ResponseDTO();
            var organizationProfileId = _accountContext.CurrentAccount.OrganizationProfileId.Value;
            var data = _consultationInvitationService.GetConsultationInvitations();
            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("checking/list")]
        public IActionResult GetConsultationInvitationsChecking([FromBody] ListFilterDTO<ConsultationReplyReviewSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var data = _consultationInvitationService.GetAllConsultationInvitationsChecking(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            var dto = new PagedListDTO<ConsultationInvitationDTO>(data);

            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("facilitating/list")]
        public IActionResult GetConsultationInvitationsFacilitating([FromBody] ListFilterDTO<ConsultationFacilitatingSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();

            searchDTO.AdvancedFilter.ModeratorId = _workContext.CurrentCustomer.Id;
            if (_permissionService.Authorize(StandardPermissionProvider.FacilitateAllSession))
            {
                searchDTO.AdvancedFilter.ModeratorId = 0;
                searchDTO.AdvancedFilter.IncludeModeratorEmail = true;
            }

            var customerId = _workContext.CurrentCustomer.Id;
            var data = _consultationInvitationService.GetAllConsultationInvitationsComplete(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            var dto = new PagedListDTO<ConsultationInvitationDTO>(data);

            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("approval/{id}")]
        public IActionResult ApprovalInvitation(int id, [FromBody] ConsultationProfileDTO dto)
        {
            var response = new ResponseDTO();
            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                response.SetResponse(ResponseStatusCode.Warning);

            }
            else
            {
                _consultationInvitationService.UpdateConsultationInvitationApproval(_workContext.CurrentCustomer.Id, id, dto);
                if (dto.IsApproved == true)
                {
                    var consultInvitationDto = _consultationInvitationService.GetConsultationInvitationById(id);
                    _proWorkflowMessageService.SendConsultationOrganizationAccepted(
                    _workContext.WorkingLanguage.Id, consultInvitationDto);

                    UpdateJobCounterCache(consultInvitationDto.ServiceProfile.CustomerId);

                    response.SetResponse(true);
                }
                else
                {
                    response.SetResponse(false);
                }
            }
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("complete")]
        public IActionResult CompleteProfile([FromBody]ModeratorConsultationModel data)
        {
            var response = new ResponseDTO();

            if (data.Id == 0)
            {
                response.SetResponse(ResponseStatusCode.Warning);

            }
            else
            {
                var hasFacilitateAllSessionRights = _permissionService.Authorize(StandardPermissionProvider.FacilitateAllSession);

                var actorId = _workContext.CurrentCustomer.Id;
                var actorName = _accountContext.CurrentAccount.Name;

                _consultationInvitationService.UpdateConsultationInvitationComplete(
                    actorId, data.Id, data.Dict, data.Remarks, data.OrganizationRating, data.OrganizationRemarks);

                //create moderator payout
                var payoutRequestDTO = new PayoutRequestDTO
                {
                    ProductTypeId = (int)ProductType.ModeratorFacilitateConsultationFee,
                    RefId = data.Id
                };
                _feeCalculationService.ProcessPayoutRequest(actorId, actorName, payoutRequestDTO, hasFacilitateAllSessionRights);

                //creat consultant payout
                var consultationEngagementFeePayoutRequestDTO = new PayoutRequestDTO
                {
                    ProductTypeId = (int)ProductType.ConsultationEngagementFee,
                    RefId = data.Id
                };
                _feeCalculationService.ProcessPayoutRequest(actorId, actorName, consultationEngagementFeePayoutRequestDTO, hasFacilitateAllSessionRights);

                _proWorkflowMessageService.SendConsultationCompleted(_workContext.WorkingLanguage.Id, data.Id);
            }
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("cancel")]
        public IActionResult CancelConsultation([FromBody] ModeratorConsultationCancelModel data)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (data.Id == 0)
            {
                response.SetResponse(ResponseStatusCode.Warning);

            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;
                var actorName = _accountContext.CurrentAccount.Name;
                var postCancellationAction = data.Rehire == true ? PostCancellationAction.Rehire : PostCancellationAction.Refund;
                
                if (data.CancelledBy == EngagementParty.Buyer.GetDescription())
                {
                    data.ReasonId = data.ReasonIdBuyer;
                    data.ReasonRemarks = data.ReasonOthersBuyer;
                }
                else if (data.CancelledBy == EngagementParty.Seller.GetDescription())
                {
                    data.ReasonId = data.ReasonIdSeller;
                    data.ReasonRemarks = data.ReasonOthersSeller;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Unable to determine Cancelling Party from {nameof(data.CancelledById)}");
                }

                _engagementCancellationManager.CancelEngagement(data.Id, EngagementType.Consultation, actorId, actorName, data.ReasonId, data.ReasonRemarks,
                    postCancellationAction);
                _proWorkflowMessageService.SendConsultationCancellation(_workContext.WorkingLanguage.Id, data.Id);

                response.SetResponse(ResponseStatusCode.Success);

            }

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("reschedule")]
        public IActionResult RescheduleConsultation([FromBody] ModeratorConsultantRescheduleModel data)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }
            //var newDateStart = data.Date.AddTicks(data.Start.TimeOfDay.Ticks);
            //var newDateEnd = data.Date.AddTicks(data.End.TimeOfDay.Ticks);
            //var newTimeSlot = new TimeSlotDTO();
            //newTimeSlot.StartDate = newDateStart;
            //newTimeSlot.EndDate = newDateEnd;
            //var newTimeSlotList = new List<TimeSlotDTO>();
            //newTimeSlotList.Add(newTimeSlot);
            //if (data.Id == 0)
            //{
            //    response.SetResponse(ResponseStatusCode.Warning);
            //}
            //else
            //{
            //    _consultationInvitationService.UpdateConsultationInvitationReschedule(data.Id, data.RescheduleRemarks, newTimeSlotList);
            //    _proWorkflowMessageService.SendConsultationReschedule(_workContext.WorkingLanguage.Id, data.Id);
            //    response.SetResponse(ResponseStatusCode.Success);
            //}

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.ModeratorReview))]
        [HttpPost("set-appointment")]
        public IActionResult SetAppointmentConsultation([FromBody] ModeratorConsultantRescheduleModel data)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (data.Id == 0 || data == null)
            {
                response.SetResponse(ResponseStatusCode.Warning);
            }
            else
            {
                var startDate = data.Date.Add(data.Start.TimeOfDay);
                var endDate = data.Date.Add(data.End.TimeOfDay);
                _consultationInvitationService.UpdateConsultationInvitationReschedule(data.Id, data.RescheduleRemarks, startDate, endDate);
                _proWorkflowMessageService.SendConsultationReschedule(_workContext.WorkingLanguage.Id, data.Id);
                response.SetResponse(ResponseStatusCode.Success);
                if (!String.IsNullOrEmpty(data.RescheduleRemarks))
                {
                    response.SetResponse("reschedule");
                }
                else
                {
                    response.SetResponse("setAppointment");
                }
            }

            return Ok(response);
        }

        [HttpGet("org/counter")]
        public IActionResult GetOrganizationItemCounter()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var dto = _consultationInvitationService.GetConsultationJobOrgCounter(customerId);
            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationConsultation))]
        [HttpPut("{id}/org/read")]
        public IActionResult UpdateOrganizationConsultationInvitationRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _consultationInvitationService.UpdateOrganizationConsultationInvitationRead(id, customerId);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPut("{id}/consultant/read")]
        public IActionResult UpdateIndividualConsultationInvitationRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _consultationInvitationService.UpdateIndividualConsultationInvitationRead(id, customerId);

            return Ok(response);
        }

        [HttpPost("{id}/refund")]
        public virtual IActionResult UpdateConsultationRefund(int id)
        {
            var response = new ResponseDTO();
            var actorId = _workContext.CurrentCustomer.Id;
            var actorName = _accountContext.CurrentAccount.Name;

            var jobApplication = _consultationInvitationService.GetConsultationInvitationById(id);

            if (jobApplication.ConsultationApplicationStatus != (int)ConsultationInvitationStatus.CancelledByIndividual)
            {
                return BadRequest("Fail to submit refund request due to service application is not cancelled by consultant");

            }
            var refundRecord = _refundRequestService.GetByOrderItemId(id);
            if (refundRecord != null)
            {
                return BadRequest("Fail to submit refund request due to refund request already submitted");

            }
            var orderRecord = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementFee, id);
            if (orderRecord.Status != (int)ProOrderItemStatus.OpenForRematch)
            {
                return BadRequest("Fail to submit refund request due to order is not open for rematched");

            }
            _orderService.SetOpenForRematchedOrderItemToPaid(actorId, orderRecord.Id);
            _refundRequestService.CreateRefundRequest(actorId, orderRecord.Id, orderRecord.Price);

            //refund consultation engagement fee
            var refundCharge = _chargeService.GetLatestCharge((int)ProductType.ConsultationEngagementMatchingFeeRefundAdminCharges, 0);
            var consultationMatchingFeeOrderItem = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementMatchingFee, id);

            var adminCharges = refundCharge.ValueType == (int)ChargeValueType.Amount? refundCharge.Value :  consultationMatchingFeeOrderItem.Price * refundCharge.Value;
            var refundAmount = consultationMatchingFeeOrderItem.Price - adminCharges;

            _refundRequestService.CreateRefundRequest(actorId, consultationMatchingFeeOrderItem.Id, refundAmount, adminCharges, true);

            //create moderator payout
            var payoutRequestDTO = new PayoutRequestDTO
            {
                ProductTypeId = (int)ProductType.ModeratorFacilitateConsultationFee,
                RefId = id
            };
            _feeCalculationService.ProcessPayoutRequest(actorId, actorName, payoutRequestDTO, false);

            response.SetResponse(true);
            return Ok(response);
        }

        #endregion

        private void UpdateJobCounterCache(int jobSeekerCustomerId)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForShortTermCache(
                NopModelCacheDefaults.JobSeekerItemCounterCacheKeyKey,
                jobSeekerCustomerId);

            _staticCacheManager.Remove(cacheKey);
        }

    }
}
