using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Nop.Core;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Services.Events;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Infrastructure;
using Nop.Web.Framework.Models.Extensions;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Web.Contexts;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Web.Filters;
using Nop.Services.Security;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.Services.Subscription;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Core.Domain.Order;
using Nop.Services.Caching;
using YadiYad.Pro.Web.Infrastructure.Cache;
using Nop.Core.Caching;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class JobController : BaseController
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IWorkContext _workContext;
        private readonly JobProfileService _jobProfileService;
        private readonly AccountContext _accountContext;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly ServiceSubscriptionService _serviceSubscriptionService;
        private readonly ChargeService _chargeService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public JobController(
            IStaticCacheManager staticCacheManager,
            ICacheKeyService cacheKeyService,
            IWorkContext workContext,
            AccountContext accountContext,
            JobProfileService jobProfileService,
            JobSeekerProfileService jobSeekerProfileService,
            ServiceSubscriptionService serviceSubscriptionService,
            ChargeService chargeService,
            IEventPublisher eventPublisher)
        {
            _staticCacheManager = staticCacheManager;
            _cacheKeyService = cacheKeyService;
            _workContext = workContext;
            _accountContext = accountContext;
            _jobProfileService = jobProfileService;
            _jobSeekerProfileService = jobSeekerProfileService;
            _serviceSubscriptionService = serviceSubscriptionService;
            _chargeService = chargeService;
            _eventPublisher = eventPublisher;
        }

        #endregion

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("")]
        public virtual IActionResult GetJobProfileByCustomerId([FromBody] ListFilterDTO<JobSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            searchDTO.AdvancedFilter.CustomerId = _workContext.CurrentCustomer.Id;

            var data = _jobProfileService.SearchJobProfiles(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<JobProfileDTO>(data);

            dto.AdditionalData = new
            {
                IsEntitledApplyJob = _accountContext.CurrentAccount.IsEntitledApplyJob
            };

            response.SetResponse(dto);

            return Ok(response);
        }

        //[HttpPost("{id}/renew")]
        //public IActionResult RenewJobProfile(int id)
        //{
        //    var response = new ResponseDTO();
        //    try
        //    {
        //        if (id != 0)
        //        {
        //            var existing = _jobProfileService.GetJobProfileById(id);
        //            //existing.ViewJobCandidateFullProfileSubscriptionEndDate = DateTime.Now.AddMonths(defaultValidityMonth).AddDays(1).Date;
        //            //add 30 days to subscription
        //            _jobProfileService.UpdateJobProfile(existing);
        //        }
        //        response.SetResponse(ResponseStatusCode.Success);
        //    }
        //    catch (Exception exp)
        //    {
        //        response.SetResponse(ResponseStatusCode.Warning, exp.ToString());
        //    }

        //    return Ok(response);
        //}

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("search")]
        public IActionResult SearchJob([FromBody] ListFilterDTO<JobSearchFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();

            var jobSeekerProfile = _jobSeekerProfileService
                .GetJobSeekerProfileByCustomerId(_workContext.CurrentCustomer.Id);

            searchDTO.AdvancedFilter.JobSeekerProfileId = jobSeekerProfile.Id;
            searchDTO.AdvancedFilter.CategoryIds = jobSeekerProfile.Categories.Select(x=>x.CategoryId).ToList();
            searchDTO.AdvancedFilter.IsFreelanceHourly = jobSeekerProfile.IsFreelanceHourly;
            searchDTO.AdvancedFilter.IsFreelanceDaily = jobSeekerProfile.IsFreelanceDaily;
            searchDTO.AdvancedFilter.IsProjectBased = jobSeekerProfile.IsProjectBased;
            searchDTO.AdvancedFilter.IsOnSite = jobSeekerProfile.IsOnSite;
            searchDTO.AdvancedFilter.IsPartialOnSite = jobSeekerProfile.IsPartialOnSite;
            searchDTO.AdvancedFilter.IsRemote = jobSeekerProfile.IsRemote;

            if(jobSeekerProfile.PreferredLocations != null
                && jobSeekerProfile.PreferredLocations.Count > 0)
            {
                searchDTO.AdvancedFilter.StateProvinceId = jobSeekerProfile.PreferredLocations?[0].StateProvinceId ?? 0;
            }
            else
            {
                searchDTO.AdvancedFilter.StateProvinceId = 0;
            }

            var data = _jobProfileService.SearchJobProfiles(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<JobProfileDTO>(data);

            dto.AdditionalData = new
            {
                IsEntitledApplyJob = _accountContext.CurrentAccount.IsEntitledApplyJob
            };

            // mask org name
            dto.Data = dto.Data
                .Select(x =>
                {
                    x.OrganizationName = StringExtensions.ToMask(x.OrganizationName);
                    return x;
                })
                .ToList();

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("summary")]
        public IActionResult SearchJobSumamry([FromBody] ListFilterDTO<JobSearchFilterDTO> searchDTO)
        {
            searchDTO.AdvancedFilter.CustomerId = _workContext.CurrentCustomer.Id;
            var response = new ResponseDTO();

            var data = _jobProfileService.SearchJobProfilesSummary(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            var dto = new PagedListDTO<JobProfileSummaryDTO>(data);

            response.SetResponse(dto);

            return Ok(response);
        }

        [HttpPost("candidate")]
        public IActionResult SearchJobCandidate([FromBody] ListFilterDTO<JobCandidateSearchFilterDTO> searchDTO)
        {
            searchDTO.RecordSize = 10;

            var response = new ResponseDTO();

            var validToSearchCandidate = _jobProfileService.CheckValidForSearchCandidate(searchDTO.AdvancedFilter.JobProfileId);

            //if (validToSearchCandidate == false)
            //{
            //    return BadRequest("The job ads no longer eligible to search new candidate.");
            //}
            searchDTO.AdvancedFilter = searchDTO.AdvancedFilter ?? new JobCandidateSearchFilterDTO();

            searchDTO.AdvancedFilter.ShowFullProfile = validToSearchCandidate;

            var dto = _jobSeekerProfileService.SearchJobCandidates(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            response.SetResponse(dto);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpGet("{id}")]
        public virtual IActionResult GetJobProfileById(int id)
        {
            var response = new ResponseDTO();
            var data = _jobProfileService.GetJobProfileById(id);
            response.SetResponse(data ?? new JobProfileDTO());
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("profile/{id}")]
        public IActionResult SubmitJobProfile(int id, [FromBody] JobProfileDTO dto)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }
            else
            {
                if (id == 0)
                {
                    //dto.ViewJobCandidateFullProfileSubscriptionEndDate = DateTime.Now.AddMonths(defaultValidityMonth).AddDays(1).Date;
                    dto.CustomerId = _workContext.CurrentCustomer.Id;
                    dto.CreatedById = _workContext.CurrentCustomer.Id;
                    dto.CreatedOnUTC = DateTime.UtcNow;
                    _jobProfileService.CreateJobProfile(dto.CustomerId, dto);
                }
                else
                {
                    var existing = _jobProfileService.GetJobProfileById(dto.Id);
                    dto.CustomerId = existing.CustomerId;
                    //dto.ViewJobCandidateFullProfileSubscriptionEndDate = existing.ViewJobCandidateFullProfileSubscriptionEndDate;
                    dto.CreatedById = existing.CreatedById;
                    dto.CreatedOnUTC = existing.CreatedOnUTC;

                    dto.UpdatedById = _workContext.CurrentCustomer.Id;
                    dto.UpdatedOnUTC = DateTime.UtcNow;
                    _jobProfileService.UpdateJobProfile(dto.CustomerId, dto);
                }

                if (dto.Status == (int)JobProfileStatus.Publish)
                {
                    _eventPublisher.Publish(new JobPublishedEvent(dto.CustomerId, dto.Id));
                    // // create new subscription for 7 day
                    // var charge = _chargeService
                    //     .GetLatestCharge(
                    //         (int)ProductType.ViewJobCandidateFullProfileSubscription,
                    //         dto.JobType
                    //         , true);
                    //
                    // if (charge != null)
                    // {
                    //     _serviceSubscriptionService.CreateServiceSubscription(
                    //         dto.CreatedById,
                    //         dto.CustomerId,
                    //         SubscriptionType.ViewJobCandidateFulleProfile,
                    //         charge.ValidityDays,
                    //         dto.Id);
                    // }
                }

                response.SetResponse(ResponseStatusCode.Success);
            }
            return Ok(response);
        }

        [HttpGet("org/counter")]
        public IActionResult GetOrganizationItemCounter()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var dto = _jobProfileService.GetOrganizationItemCounterMain(customerId);
            response.SetResponse(dto);
            return Ok(response);
        }

        [HttpGet("{id}/org/counter")]
        public IActionResult GetOrganizationItemCounter(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            var dto = _jobProfileService.GetOrganizationItemCounter(customerId, id);
            response.SetResponse(dto);
            return Ok(response);
        }

        [HttpGet("seeker/counter")]
        public IActionResult GetJobSeekerItemCounter()
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;

            var cacheKey = _cacheKeyService.PrepareKeyForShortTermCache(
                NopModelCacheDefaults.JobSeekerItemCounterCacheKeyKey,
                customerId);

            var dto = _staticCacheManager.Get(cacheKey, () => _jobProfileService.GetJobSeekerItemCounter(customerId));

            response.SetResponse(dto);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpGet("{id}/info")]
        public virtual IActionResult GetJobProfileInfoById(int id)
        {
            var actorId = _workContext.CurrentCustomer.Id;
            var response = new ResponseDTO();
            var data = _jobProfileService.GetJobProfileInfoById(actorId, id);
            response.SetResponse(data);
            return Ok(response);
        }
    }
}
