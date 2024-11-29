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
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Core.Infrastructure.Cache;
using YadiYad.Pro.Services.DTO.Attentions;

namespace YadiYad.Pro.Services.Services.Attentions
{
    public class IndividualAttentionService
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

        public IndividualAttentionService(
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IRepository<ConsultationInvitation> consultationInvitationRepo,
            IRepository<JobApplication> jobApplicationRepo,
            IRepository<JobInvitation> JobInvitationRepo,
            IRepository<ServiceApplication> serviceApplicationRepo,
            IRepository<JobSeekerProfile> jobSeekerProfileRepo,
            IRepository<ConsultationProfile> consultationProfileRepo,
            IRepository<ServiceProfile> serviceProfileRepo,
            IRepository<ServiceSubscription> serviceSubscriptionRepo
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
        }

        public IndividualAttentionDTO GetIndividualUserAttention(int customerId)
        {
            var individualAttentionKey = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.IndividualAttention, customerId);

            var dto = _staticCacheManager.Get(individualAttentionKey, () =>
            {
                var dto = new IndividualAttentionDTO();

                dto.HasFreelanceJobAppliedAttention = hasFreelanceJobAppliedAttention(customerId);

                dto.HasFreelanceJobInvitesAttention = HasFreelanceJobInvitesAttention(customerId);

                dto.HasSellServiceRequestAttention = HasSellServiceRequestAttention(customerId);

                dto.HasSellServiceConfirmedOrderAttention = HasSellServiceConfirmedOrderAttention(customerId);

                dto.HasBuyServiceConfirmedOrderAttention = HasBuyServiceConfirmedOrderAttention(customerId);

                dto.HasBuyServiceRequestedOrderAttention = HasBuyServiceRequestedOrderAttention(customerId);

                return dto;
            });

            return dto;
        }

        public bool HasFreelanceJobInvitesAttention(int customerId)
        {
            var utcNow = DateTime.UtcNow;

            var hasFreelanceJobInvitesAttention =
               (from ji in _jobInvitationRepo.Table
                .Where(ji => ji.Deleted == false
                && ji.JobInvitationStatus == (int)JobInvitationStatus.Pending)
                from sp in _jobSeekerProfileRepo.Table
                .Where(sp => sp.Deleted == false
                && sp.Id == ji.JobSeekerProfileId
                && sp.CustomerId == customerId)
                from ss in _serviceSubscriptionRepo.Table
                where ss.Deleted == false
                && ss.RefId == ji.JobProfileId
                && (DateTime?)(ss.StopDate != null ? ss.StopDate.Value : ss.EndDate) >= utcNow
                select ji.Id)
                   .Any()
                   ||
                   (from ci in _consultationInvitationRepo.Table
                    .Where(ci => ci.Deleted == false
                    && ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New)
                    from cp in _consultationProfileRepo.Table
                    .Where(cp => cp.Deleted == false
                    && cp.Id == ci.ConsultationProfileId
                    && cp.IsApproved == true)
                    from sp in _serviceProfileRepo.Table
                    .Where(sp => sp.Deleted == false
                    && sp.Id == ci.ServiceProfileId
                    && sp.CustomerId == customerId)
                    select ci.Id)
                   .Any();

            return hasFreelanceJobInvitesAttention;
        }

        public bool hasFreelanceJobAppliedAttention(int customerId)
        {
            var hasFreelanceJobAppliedAttention =
                (from ja in _jobApplicationRepo.Table
                .Where(ja => ja.Deleted == false
                && ja.IsRead == false
                && (ja.JobApplicationStatus == (int)JobApplicationStatus.Shortlisted
                || ja.JobApplicationStatus == (int)JobApplicationStatus.KeepForFutureReference
                || ja.JobApplicationStatus == (int)JobApplicationStatus.Hired
                || ja.JobApplicationStatus == (int)JobApplicationStatus.Rejected
                || ja.JobApplicationStatus == (int)JobApplicationStatus.Completed
                || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization
                || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual
                || ja.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration))
                 from sp in _jobSeekerProfileRepo.Table
                 .Where(sp => sp.Deleted == false
                 && sp.Id == ja.JobSeekerProfileId
                 && sp.CustomerId == customerId)
                 select ja.Id)
                .Any()
                ||
                (from ci in _consultationInvitationRepo.Table
                 .Where(ci => ci.Deleted == false
                 && ci.IsIndividualRead == false
                 && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                 || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                 || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization))
                 from cp in _consultationProfileRepo.Table
                 .Where(cp => cp.Deleted == false
                 && cp.Id == ci.ConsultationProfileId
                 && cp.IsApproved == true)
                 from sp in _serviceProfileRepo.Table
                 .Where(sp => sp.Deleted == false
                 && sp.Id == ci.ServiceProfileId
                 && sp.CustomerId == customerId)
                 select ci.Id)
                 .Any();

            return hasFreelanceJobAppliedAttention;
        }

        public bool HasSellServiceRequestAttention(int customerId)
        {
            var qNoReceivedRequests = (from sa in _serviceApplicationRepo.Table
                                        .Where(sa => sa.Deleted == false
                                        && sa.ProviderIsRead == false
                                        && (sa.Status == (int)ServiceApplicationStatus.New))
                                       from sp in _serviceProfileRepo.Table
                                       .Where(sp => sp.Deleted == false
                                           && sp.CustomerId == customerId
                                           && sp.Id == sa.ServiceProfileId)
                                       select sa.Id);

            var hasSellServiceRequestAttention = qNoReceivedRequests.Any();

            return hasSellServiceRequestAttention;
        }

        public bool HasSellServiceConfirmedOrderAttention(int customerId)
        {

            var qNoHiringRequests = (from sa in _serviceApplicationRepo.Table
                                    .Where(sa => sa.Deleted == false
                                        && sa.ProviderIsRead == false
                                        && (sa.Status == (int)ServiceApplicationStatus.Paid
                                            || sa.Status == (int)ServiceApplicationStatus.Completed
                                            || sa.Status == (int)ServiceApplicationStatus.CancelledByBuyer
                                        ))
                                     from sp in _serviceProfileRepo.Table
                                     .Where(sp => sp.Deleted == false
                                         && sp.CustomerId == customerId
                                        && sp.Id == sa.ServiceProfileId)
                                     select sa.Id);

            var hasSellServiceConfirmedOrderAttention = qNoHiringRequests.Any();

            return hasSellServiceConfirmedOrderAttention;
        }

        public bool HasBuyServiceConfirmedOrderAttention(int customerId)
        {
            var qNoConfirmedOrders = (from sa in _serviceApplicationRepo.Table
                                    .Where(sa => sa.Deleted == false
                                    && sa.CustomerId == customerId
                                    && sa.RequesterIsRead == false
                                    && (sa.Status == (int)ServiceApplicationStatus.Completed
                                        || sa.Status == (int)ServiceApplicationStatus.CancelledBySeller))
                                      select sa.Id);

            var hasBuyServiceConfirmedOrderAttention = qNoConfirmedOrders.Any();

            return hasBuyServiceConfirmedOrderAttention;
        }

        /// <summary>
        /// has accepted, rejected and haven't read
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public bool HasBuyServiceRequestedOrderAttention(int customerId)
        {
            var qNoRequestedOrders = (from sa in _serviceApplicationRepo.Table
                                        .Where(sa => sa.Deleted == false
                                        && sa.RequesterIsRead == false
                                        && sa.CustomerId == customerId
                                        && (sa.Status == (int)ServiceApplicationStatus.Accepted
                                                || sa.Status == (int)ServiceApplicationStatus.Rejected
                                                || sa.Status == (int)ServiceApplicationStatus.Reproposed
                                                )
                                        )
                                      select sa.Id);

            var hasBuyServiceRequestedOrderAttention = qNoRequestedOrders.Any();

            return hasBuyServiceRequestedOrderAttention;
        }

        public void ClearIndividualAttentionCache(int customerId)
        {
            var consultantAttentionKey = _cacheKeyService.PrepareKeyForShortTermCache(
                NopModelCacheDefaults.IndividualAttention,
                customerId
                );

            _staticCacheManager.Remove(consultantAttentionKey);
        }
    }
}
