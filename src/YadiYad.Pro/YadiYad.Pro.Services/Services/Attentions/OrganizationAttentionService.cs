using Nop.Core.Caching;
using Nop.Data;
using Nop.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Core.Infrastructure.Cache;
using YadiYad.Pro.Services.DTO.Attentions;
using YadiYad.Pro.Services.DTO.Consultation;

namespace YadiYad.Pro.Services.Services.Attentions
{
    /// <summary>
    /// Organization Attention on 
    /// - Offer Freelance Jobs
    ///     - *Current Postings
    ///     - Hired
    /// - Seek Consultations
    ///     - Invites
    ///     - Applicants
    ///     - Confirmed Orders
    /// </summary>
    public class OrganizationAttentionService
    {

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepo;
        private readonly IRepository<JobApplication> _jobApplicationRepo;
        private readonly IRepository<JobInvitation> _jobInvitationRepo;
        private readonly IRepository<ServiceApplication> _serviceApplicationRepo;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepo;
        private readonly IRepository<ConsultationProfile> _consultationProfileRepo;
        private readonly IRepository<ServiceProfile> _serviceProfileRepo;
        private readonly IRepository<ServiceSubscription> _serviceSubscriptionRepo;
        private readonly IRepository<OrganizationProfile> _organizationProfileRepo;
        private readonly IRepository<JobProfile> _jobProfileRepo;

        public OrganizationAttentionService(
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IRepository<ConsultationInvitation> consultationInvitationRepo,
            IRepository<JobApplication> jobApplicationRepo,
            IRepository<JobInvitation> JobInvitationRepo,
            IRepository<ServiceApplication> serviceApplicationRepo,
            IRepository<JobSeekerProfile> jobSeekerProfileRepo,
            IRepository<ConsultationProfile> consultationProfileRepo,
            IRepository<ServiceProfile> serviceProfileRepo,
            IRepository<ServiceSubscription> serviceSubscriptionRepo,
            IRepository<OrganizationProfile> organizationProfileRepo,
            IRepository<JobProfile> jobProfileRepo
            )
        {
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _consultationInvitationRepo = consultationInvitationRepo;
            _jobApplicationRepo = jobApplicationRepo;
            _jobInvitationRepo = JobInvitationRepo;
            _serviceApplicationRepo = serviceApplicationRepo;
            _jobSeekerProfileRepo = jobSeekerProfileRepo;
            _consultationProfileRepo = consultationProfileRepo;
            _serviceProfileRepo = serviceProfileRepo;
            _serviceSubscriptionRepo = serviceSubscriptionRepo;
            _organizationProfileRepo = organizationProfileRepo;
            _jobProfileRepo = jobProfileRepo;
        }

        public OrganizationAttentionDTO GetOrganizationUserAttention(int customerId)
        {
            var OrganizationAttentionKey = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.OrganizationAttention, customerId);

            var dto = _staticCacheManager.Get(OrganizationAttentionKey, () =>
            {
                var dto = new OrganizationAttentionDTO();

                dto.HasHiredJobsAttention = HasHiredJobsAttention(customerId);
                dto.HasNewJobApplicationAttention = HasNewJobApplicationAttention(customerId);

                var consultationJobOrgCounterDTO = GetConsultationJobOrgCounter(customerId);

                dto.HasConsultationInvitesAttention​ = consultationJobOrgCounterDTO.NoInvitation > 0;

                dto.HasConsultationApplicantsAttention​ = consultationJobOrgCounterDTO.NoApplicant > 0;

                dto.HasConsultationOrdersAttention​ = consultationJobOrgCounterDTO.NoConfirmedOrder > 0;

                return dto;
            });

            return dto;
        }

        public ConsultationJobOrgCounterDTO GetConsultationJobOrgCounter(int customerId)
        {
            var recordQuery = (from ji in _consultationInvitationRepo.Table
                           .Where(ji => ji.Deleted == false)
                               from og in _organizationProfileRepo.Table
                               .Where(og => og.Deleted == false
                               && og.Id == ji.OrganizationProfileId
                               && og.CustomerId == customerId)
                               select ji)
                          .GroupBy(x => 1)
                          .Select(x => new
                          {
                              NoInvitation = x.Any(y =>
                                (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual
                                    && y.IsOrganizationRead == false)) ? 1 : 0,
                              NoApplicant = x.Any(y =>
                                y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                                && y.IsApproved == true) ? 1 : 0,
                              NoConfirmedOrder = x.Any(y =>
                                (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                                    && y.IsOrganizationRead == false)
                                || (y.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                                    && y.IsOrganizationRead == false)) ? 1 : 0
                          });

            var record = recordQuery.FirstOrDefault();

            var dto = new ConsultationJobOrgCounterDTO();
            dto.NoInvitation = (record?.NoInvitation) ?? 0;
            dto.NoApplicant = (record?.NoApplicant) ?? 0;
            dto.NoConfirmedOrder = (record?.NoConfirmedOrder) ?? 0;

            return dto;
        }

        public bool HasHiredJobsAttention(int customerId)
        {
            var noJobHiredQuery =
                (from ja in _jobApplicationRepo.Table
                .Where(ja => ja.Deleted == false
                && ja.IsOrganizationRead == false
                && (ja.JobApplicationStatus == (int)JobApplicationStatus.Completed
                || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual
                || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization))

                 from og in _organizationProfileRepo.Table
                 .Where(og => og.Deleted == false
                 && og.Id == ja.OrganizationProfileId
                 && og.CustomerId == customerId)

                 from jp in _jobProfileRepo.Table
                 .Where(jp => og.Deleted == false
                 && jp.Id == ja.JobProfileId
                 && jp.CustomerId == customerId)

                 from jsp in _jobSeekerProfileRepo.Table
                 .Where(jsp => jsp.Deleted == false
                 && jsp.Id == ja.JobSeekerProfileId)
                 select ja.Id);

            return noJobHiredQuery.Any();
        }

        public bool HasNewJobApplicationAttention(int customerId)
        {
            var nJobApplicationQuery =
                (from ja in _jobApplicationRepo.Table
                .Where(ja => ja.Deleted == false
                && ja.IsOrganizationRead == false
                && ja.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration)

                 from og in _organizationProfileRepo.Table
                 .Where(og => og.Deleted == false
                 && og.Id == ja.OrganizationProfileId
                 && og.CustomerId == customerId)

                 from jp in _jobProfileRepo.Table
                 .Where(jp => og.Deleted == false
                 && jp.Id == ja.JobProfileId
                 && jp.CustomerId == customerId)
                 select jp);

            return nJobApplicationQuery.Any();
        }

        public void ClearOrganizationAttentionCache(int customerId)
        {
            var consultantAttentionKey = _cacheKeyService.PrepareKeyForShortTermCache(
                NopModelCacheDefaults.OrganizationAttention,
                customerId
                );

            _staticCacheManager.Remove(consultantAttentionKey);
        }
    }
}
