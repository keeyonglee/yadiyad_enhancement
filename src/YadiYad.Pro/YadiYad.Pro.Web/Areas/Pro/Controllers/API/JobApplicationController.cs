using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Web.Framework.Models.Extensions;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Web.Filters;
using Nop.Services.Security;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Deposit;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Services.Subscription;
using Nop.Core.Caching;
using Nop.Services.Caching;
using YadiYad.Pro.Web.Infrastructure.Cache;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class JobApplicationController : BaseController
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly JobProfileService _jobProfileService;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly DepositRequestService _depositRequestService;
        private readonly PayoutRequestService _payoutRequestService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly EngagementCancellationManager _engagementCancellationManager;
        private readonly OrderProcessingService _orderProcessingService;
        private readonly RefundRequestService _refundRequestService;
        private readonly OrderService _orderService;
        private readonly ServiceSubscriptionService _serviceSubscriptionService;
        #endregion

        #region Ctor

        public JobApplicationController(
            IStaticCacheManager staticCacheManager,
            ICacheKeyService cacheKeyService,
            IWorkContext workContext,
            AccountContext accountContext,
            JobProfileService jobProfileService,
            OrganizationProfileService organizationProfileService,
            JobApplicationService jobApplicationService,
            ServiceProfileService serviceProfileService,
            JobSeekerProfileService jobSeekerProfileService,
            ProWorkflowMessageService proWorkflowMessageService,
            DepositRequestService depositRequestService,
            PayoutRequestService payoutRequestService,
            FeeCalculationService feeCalculationService,
            EngagementCancellationManager engagementCancellationManager,
            OrderProcessingService orderProcessingService,
            RefundRequestService refundRequestService,
            OrderService orderService,
            ServiceSubscriptionService serviceSubscriptionService)
        {
            _staticCacheManager = staticCacheManager;
            _cacheKeyService = cacheKeyService;
            _workContext = workContext;
            _accountContext = accountContext;
            _jobProfileService = jobProfileService;
            _organizationProfileService = organizationProfileService;
            _jobApplicationService = jobApplicationService;
            _serviceProfileService = serviceProfileService;
            _jobSeekerProfileService = jobSeekerProfileService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _depositRequestService = depositRequestService;
            _payoutRequestService = payoutRequestService;
            _feeCalculationService = feeCalculationService;
            _engagementCancellationManager = engagementCancellationManager;
            _orderProcessingService = orderProcessingService;
            _refundRequestService = refundRequestService;
            _orderService = orderService;
            _serviceSubscriptionService = serviceSubscriptionService;
        }

        #endregion

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("applications")]
        public virtual IActionResult GetJobApplicationsForJobSeeker([FromBody]ListFilterDTO<JobApplicationListingFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.IndividualCustomerId = customerId;
            var data = _jobApplicationService.GetJobApplications(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            // mask org name
            data.Data = data.Data
                .Select(x =>
                {
                    if ((x.JobApplicationStatus != (int)JobApplicationStatus.Hired
                    && x.JobApplicationStatus != (int)JobApplicationStatus.Completed
                    && x.JobApplicationStatus != (int)JobApplicationStatus.CancelledByOrganization
                    && x.JobApplicationStatus != (int)JobApplicationStatus.CancelledByIndividual)
                    || x.ConsultationProfile != null)
                    {
                        x.OrganizationName = x.OrganizationName?.ToMask();
                    }
                    return x;
                })
                .ToList();

            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("individualapplications")]
        public virtual IActionResult GetJobApplicationsForJobSeekerIndividual([FromBody] ListFilterDTO<JobApplicationListingFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.IndividualCustomerId = customerId;
            var data = _jobApplicationService.GetJobApplications(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("applicants")]
        public virtual IActionResult GetJobApplicationsForOrganization([FromBody]ListFilterDTO<JobApplicationListingFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.OrganizationCustomerId = customerId;
            if (searchDTO.AdvancedFilter.JobProfileId == 0)
            {
                searchDTO.AdvancedFilter.ExcludePendingApplicationIfHired = true;
            }
            var data = _jobApplicationService.GetJobApplicationsForOrganization(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            response.SetResponse(data);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetJobApplicationById(int id)
        {
            var response = new ResponseDTO();
            var data = _jobApplicationService.GetJobApplicationById(id);
            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(
            nameof(StandardPermissionProvider.OrganizationJob),
            nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("{id}")]
        public IActionResult SubmitJobApplication(int id, [FromBody] JobApplicationDTO dto)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                if(_accountContext.CurrentAccount.IsEntitledApplyJob == false)
                {
                    return BadRequest("Please pay for Pay-to-Apply Jobs (PAJ).");
                }

                JobProfileDTO jobProfileDTO = _jobProfileService.GetJobProfileById(dto.JobProfileId);
                OrganizationProfileDTO organizationProfileDTO = _organizationProfileService.GetOrganizationProfileByCustomerId(jobProfileDTO.CustomerId);
                JobSeekerProfileDTO jobSeekerProfileDTO = _jobSeekerProfileService.GetJobSeekerProfileByCustomerId(_workContext.CurrentCustomer.Id);

                dto.JobSeekerProfileId = jobSeekerProfileDTO.Id;
                dto.OrganizationProfileId = organizationProfileDTO.Id;
                dto.JobApplicationStatus = (int)JobApplicationStatus.UnderConsideration;
                dto.IsEscrow = dto.IsEscrow;
                dto.CreatedById = _workContext.CurrentCustomer.Id;
                dto.CreatedOnUTC = DateTime.UtcNow;
                _jobApplicationService.CreateJobApplication(_workContext.CurrentCustomer.Id, dto);

                UpdateJobCounterCache(jobSeekerProfileDTO.CustomerId, organizationProfileDTO.CustomerId, jobProfileDTO.Id);
            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;
                var updatedJobApplication = _jobApplicationService.UpdateJobApplicationStatus
                    (actorId,
                    id,
                    (JobApplicationStatus)dto.JobApplicationStatus,
                    true);

                switch ((JobApplicationStatus)dto.JobApplicationStatus)
                {
                    case JobApplicationStatus.Shortlisted:
                        _proWorkflowMessageService.SendIndividualJobApplicationShortlist(id, _workContext.WorkingLanguage.Id);
                        break;

                    case JobApplicationStatus.KeepForFutureReference:
                        _proWorkflowMessageService.SendIndividualJobApplicationFutureReference(id, _workContext.WorkingLanguage.Id);
                        break;

                    case JobApplicationStatus.Hired:
                        {
                            if (updatedJobApplication.NumberOfHiring == 2)
                            {
                                //stop PVI subscription if it is 2nd hiring for same job profile within the subscription
                                _serviceSubscriptionService.StopAllActiveServiceSubscription(
                                    actorId,
                                    SubscriptionType.ViewJobCandidateFulleProfile,
                                    updatedJobApplication.JobProfileId);
                            }

                            _jobProfileService.UpdateJobProfileToHiredStatus(_workContext.CurrentCustomer.Id, updatedJobApplication.JobProfileId);
                            var refundRequestDTO = _orderProcessingService.ProcessRefundIfAny(_workContext.CurrentCustomer.Id, ProductType.JobEnagegementFee, id);

                            if(refundRequestDTO != null)
                            {

                            }

                            _proWorkflowMessageService.SendIndividualJobApplicationHire(id, _workContext.WorkingLanguage.Id);
                        }
                        break;

                    default:
                        break;
                }
                UpdateJobCounterCache(updatedJobApplication.JobSeekerProfile.CustomerId, updatedJobApplication.JobProfile.CustomerId, updatedJobApplication.JobProfile.Id);
            }
            response.SetResponse(ResponseStatusCode.Success);


            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPut("{id}/seeker/read")]
        public IActionResult UpdateJobSeekJobApplicationRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _jobApplicationService.UpdateJobSeekJobApplicationRead(id, customerId);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("{id}/review")]
        public IActionResult Review(int id, [FromBody] JobApplicationReviewDTO dto)
        {
            var response = new ResponseDTO();

            _jobApplicationService.ReviewJobApplication(
                _workContext.CurrentCustomer.Id,
                id,
                dto);

            return Ok(response);
        }

        [HttpPut("{id}")]
        public virtual IActionResult UpdateJobApplicationStartDate(
            int id,
            [FromBody]UpdateJobApplicationStartDateDTO request)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid == false)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;

                var result = _jobApplicationService
                    .UpdateJobApplicationStartDate(
                    actorId,
                    id,
                    request);

                response.SetResponse(result);
            }

            return Ok(response);
        }

        [HttpPut("{id}/terminate")]
        public virtual IActionResult UpdateJobApplicationEndDate(int id, [FromBody]UpdateJobApplicationEndDateDTO request)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid == false)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;

                var result = _jobApplicationService
                    .UpdateJobApplicationEndDate(
                    actorId,
                    id,
                    request);

                response.SetResponse(result);
            }

            return Ok(response);
        }

        [HttpPost("{id}/cancel")]
        public virtual IActionResult UpdateJobApplicationCancel(int id, [FromBody] UpdateJobApplicationCancelDTO request)
        {
            var response = new ResponseDTO();
            if (ModelState.IsValid == false)
            {
                response.SetResponse(false);
            }
            else
            {
                var actorId = _workContext.CurrentCustomer.Id;
                var actorName = _accountContext.CurrentAccount.Name;
                _engagementCancellationManager.CancelEngagement(id, EngagementType.Job, actorId, actorName, request.ReasonId, request.Remarks);
                response.SetResponse(true);
            }

            return Ok(response);
        }

        [HttpGet("{id}/depositPayout")]
        public virtual IActionResult GetDepositPayoutByJobApplicationId(int id)
        {
            var response = new ResponseDTO();
            var refId = id;
            var record = _jobApplicationService.GetJobApplicationById(id);
            var productTypeId = (int)ProductType.JobEnagegementFee;
            var depositData = _depositRequestService.GetDepositDetails(refId, productTypeId);
            var payoutData = _payoutRequestService.GetPayoutRequestSummary(refId, productTypeId);
            var feeData = record.JobType != (int)JobType.ProjectBased ? _feeCalculationService.CalculatePayout(new PayoutRequestDTO()
            {
                RefId = refId,
                ProductTypeId = productTypeId
            }) : new CalculatedFeeDTO();

            var data = new JobApplicationDepositPayoutDetailDTO()
            {
                Deposit = depositData,
                Payout = payoutData,
                EngagementFee = feeData
            };
            response.SetResponse(data);
            return Ok(response);
        }

        [HttpPost("{id}/terminate/milestone")]
        public IActionResult GetMilestoneByJobApplicationId(int id)
        {
            var actorId = _workContext.CurrentCustomer.Id;
            var response = new ResponseDTO();
            var data = _jobApplicationService.GetEntitledTerminateMilestone(id);
            response.SetResponse(data);
            return Ok(response);
        }

        [HttpPost("{id}/refund")]
        public virtual IActionResult UpdateJobApplicationRefund(int id)
        {
            var response = new ResponseDTO();
            var actorId = _workContext.CurrentCustomer.Id;
            var actorName = _accountContext.CurrentAccount.Name;

            var jobApplication = _jobApplicationService.GetJobApplicationById(id);

            if (jobApplication.JobApplicationStatus != (int)JobApplicationStatus.CancelledByIndividual)
            {
                return BadRequest("Fail to submit refund request due to service application is not cancelled by individual");

            }
            var refundRecord = _refundRequestService.GetByOrderItemId(id);
            if (refundRecord != null)
            {
                return BadRequest("Fail to submit refund request due to refund request already submitted");

            }
            var orderRecord = _orderService.GetOrderItem((int)ProductType.JobEnagegementFee, id);
            if (orderRecord.Status != (int)ProOrderItemStatus.OpenForRematch)
            {
                return BadRequest("Fail to submit refund request due to order is not open for rematched");

            }

            _orderService.SetOpenForRematchedOrderItemToPaid(actorId, orderRecord.Id);
            _refundRequestService.CreateRefundRequest(actorId, orderRecord.Id, orderRecord.Price);

            //if organization choose for refund, stop PVI subscription
            _serviceSubscriptionService.StopAllActiveServiceSubscription(actorId, SubscriptionType.ViewJobCandidateFulleProfile, jobApplication.JobProfileId);

            response.SetResponse(true);
            return Ok(response);
        }

        private void UpdateJobCounterCache(int jobSeekerCustomerId, int jobProfileCustomerId, int jobProfileId)
        {
            var cacheKey = _cacheKeyService.PrepareKeyForShortTermCache(
                NopModelCacheDefaults.JobSeekerItemCounterCacheKeyKey,
                jobSeekerCustomerId);

            _staticCacheManager.Remove(cacheKey);

            cacheKey = _cacheKeyService.PrepareKeyForShortTermCache(
                NopModelCacheDefaults.OrganizationItemCounterCacheKeyKey,
                jobProfileCustomerId, jobProfileId);

            _staticCacheManager.Remove(cacheKey);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPut("{id}/org/read")]
        public IActionResult UpdateOrganizationJobApplicationRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _jobApplicationService.UpdateOrgJobApplicationRead(id, customerId);

            return Ok(response);
        }
    }
}
