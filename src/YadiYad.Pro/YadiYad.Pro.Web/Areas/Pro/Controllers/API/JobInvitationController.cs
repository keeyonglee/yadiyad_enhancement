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
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Services.Messages;
using YadiYad.Pro.Web.Filters;
using Nop.Services.Security;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Web.Contexts;
using Nop.Core.Caching;
using Nop.Services.Caching;
using YadiYad.Pro.Web.Infrastructure.Cache;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers.API
{
    [CamelCaseResponseFormatter]
    [Route("api/pro/[controller]")]
    public class JobInvitationController : BaseController
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IWorkContext _workContext;
        private readonly AccountContext _accountContext;
        private readonly JobProfileService _jobProfileService;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly JobInvitationService _jobInvitationService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;

        #endregion

        #region Ctor

        public JobInvitationController(
            IStaticCacheManager staticCacheManager,
            ICacheKeyService cacheKeyService,
            IWorkContext workContext,
            AccountContext accountContext,
            JobProfileService jobProfileService,
            OrganizationProfileService organizationProfileService,
            JobInvitationService jobInvitationService,
            JobApplicationService jobApplicationService,
            ServiceProfileService serviceProfileService,
            JobSeekerProfileService jobSeekerProfileService,
            IndividualProfileService individualProfileService,
            ProWorkflowMessageService proWorkflowMessageService)
        {
            _staticCacheManager = staticCacheManager;
            _cacheKeyService = cacheKeyService;
            _workContext = workContext;
            _accountContext = accountContext;
            _jobProfileService = jobProfileService;
            _organizationProfileService = organizationProfileService;
            _jobInvitationService = jobInvitationService;
            _jobApplicationService = jobApplicationService;
            _serviceProfileService = serviceProfileService;
            _jobSeekerProfileService = jobSeekerProfileService;
            _individualProfileService = individualProfileService;
            _proWorkflowMessageService = proWorkflowMessageService;
        }

        #endregion

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("invites")]
        public virtual IActionResult GetJobInvitationsForJobSeeker([FromBody]ListFilterDTO<JobInvitationListingFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            searchDTO.AdvancedFilter.JobInvitationStatus = new List<int>();
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Pending);
            //searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Accepted);
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Expired);
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Declined);
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Reviewing);
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.UpdateRequired);
            searchDTO.AdvancedFilter.IndividualCustomerId = customerId;

            var data = _jobInvitationService.GetJobInvitations(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);

            // mask org name
            data.Data = data.Data
                .Select(x =>
                {
                    x.OrganizationName = StringExtensions.ToMask(x.OrganizationName);
                    return x;
                })
                .ToList();

            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("inviteds")]
        public virtual IActionResult GetInvitedCandidatesForOrganization([FromBody]ListFilterDTO<JobInvitationListingFilterDTO> searchDTO)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;

            searchDTO.AdvancedFilter.OrganizationCustomerId = customerId;
            searchDTO.AdvancedFilter.JobInvitationStatus = new List<int>();
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Pending);
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Accepted);
            searchDTO.AdvancedFilter.JobInvitationStatus.Add((int)JobInvitationStatus.Declined);

            var data = _jobInvitationService.GetJobInvitations(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize,
                searchDTO.Filter,
                searchDTO.AdvancedFilter);
            response.SetResponse(data);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public virtual IActionResult GetJobInvitationById(int id)
        {
            var response = new ResponseDTO();
            var data = _jobInvitationService.GetJobInvitationById(id);
            response.SetResponse(data);
            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPost("invite")]
        public virtual IActionResult CreateJobInvitation([FromBody] JobInvitationDTO dto)
        {
            var response = new ResponseDTO();

            JobProfileDTO jobProfileDTO = _jobProfileService.GetJobProfileById(dto.JobProfileId);
            JobSeekerProfileDTO jobSeekerProfileDTO = _jobSeekerProfileService.GetJobSeekerProfileById(dto.JobSeekerProfileId);
            IndividualProfileDTO individualProfileDTO = _individualProfileService.GetIndividualProfileByCustomerId(jobSeekerProfileDTO.CustomerId);
            OrganizationProfileDTO organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(jobProfileDTO.CustomerId);

            dto.JobProfile = jobProfileDTO;
            dto.JobSeekerProfile = jobSeekerProfileDTO;
            dto.ServiceIndividualProfile = individualProfileDTO;
            dto.OrganizationProfileId = organizationProfile.Id;
            dto.JobInvitationStatus = (int)JobInvitationStatus.Pending;
            dto.CreatedById = _workContext.CurrentCustomer.Id;
            dto.CreatedOnUTC = DateTime.UtcNow;

            _jobInvitationService.CreateJobInvitation(dto);
            _proWorkflowMessageService.SendIndividualJobInvitationInvitedMessage(dto, _workContext.WorkingLanguage.Id);

            UpdateJobCounterCache(jobSeekerProfileDTO.CustomerId, jobProfileDTO.CustomerId, jobProfileDTO.Id);

            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.IndividualJob))]
        [HttpPost("{id}")]
        public IActionResult SubmitJobInvitation(int id, [FromBody] JobInvitationDTO dto)
        {
            var response = new ResponseDTO();

            if (!ModelState.IsValid)
            {
                response.SetResponse(ModelState);
            }

            if (id == 0)
            {
                JobProfileDTO jobProfileDTO = _jobProfileService.GetJobProfileById(dto.JobProfileId);
                OrganizationProfileDTO organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(jobProfileDTO.CustomerId);

                dto.OrganizationProfileId = organizationProfile.Id;
                dto.JobInvitationStatus = (int)JobInvitationStatus.Pending;
                dto.CreatedById = _workContext.CurrentCustomer.Id;
                dto.CreatedOnUTC = DateTime.UtcNow;
                _jobInvitationService.CreateJobInvitation(dto);
            }
            else
            {
                if(_accountContext.CurrentAccount.IsEntitledApplyJob == false 
                    && dto.JobInvitationStatus == (int)JobInvitationStatus.Accepted)
                {
                    return BadRequest("Please pay for Pay-to-Apply Jobs (PAJ).");
                }

                var existing = _jobInvitationService.GetJobInvitationById(id);

                if (existing.JobInvitationStatus == (int)JobInvitationStatus.Pending
                    && (dto.JobInvitationStatus == (int)JobInvitationStatus.Accepted ||
                    dto.JobInvitationStatus == (int)JobInvitationStatus.Declined))
                {
                    if (dto.JobInvitationStatus == (int)JobInvitationStatus.Accepted)
                    {
                        var jobApplicationDTO = new JobApplicationDTO
                        {
                            JobSeekerProfileId = existing.JobSeekerProfileId,
                            JobProfileId = existing.JobProfileId,
                            OrganizationProfileId = existing.OrganizationProfileId,
                            JobInvitationId = existing.Id,
                            IsEscrow = dto.IsEscrow,
                            JobApplicationStatus = (int)JobApplicationStatus.UnderConsideration,
                            CreatedById = _workContext.CurrentCustomer.Id,
                            CreatedOnUTC = DateTime.UtcNow,
                        };
                        _proWorkflowMessageService.SendOrganisationJobInvitationAcceptedMessage(id, _workContext.WorkingLanguage.Id);

                        _jobApplicationService.CreateJobApplication(_workContext.CurrentCustomer.Id, jobApplicationDTO);
                    }
                    else
                    {
                        _proWorkflowMessageService.SendOrganisationJobInvitationRejectedMessage(id, _workContext.WorkingLanguage.Id);
                    }

                    existing.JobInvitationStatus = dto.JobInvitationStatus;
                    existing.UpdatedById = _workContext.CurrentCustomer.Id;
                    existing.UpdatedOnUTC = DateTime.UtcNow;
                    _jobInvitationService.UpdateJobInvitation(existing);

                    UpdateJobCounterCache(existing.JobSeekerProfile.CustomerId, existing.JobProfile.CustomerId, existing.JobProfileId);

                }
            }
            response.SetResponse(ResponseStatusCode.Success);

            return Ok(response);
        }

        [AuthorizeAccess(nameof(StandardPermissionProvider.OrganizationJob))]
        [HttpPut("{id}/org/read")]
        public IActionResult UpdateOrganizationJobInvitationRead(int id)
        {
            var response = new ResponseDTO();
            var customerId = _workContext.CurrentCustomer.Id;
            _jobInvitationService.UpdateOrganizationJobInvitationRead(id, customerId);

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
    }
}
