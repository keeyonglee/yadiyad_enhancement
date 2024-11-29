using AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.DTO.Service;
using Nop.Core.Domain.Localization;
using YadiYad.Pro.Core.Domain.Consultation;
using Newtonsoft.Json;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Questionnaire;
using Newtonsoft.Json.Linq;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Core.Domain.Order;
using LinqToDB;
using Nop.Services.Helpers;
using Nop.Services.Customers;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.DepositRequest;
using Nop.Core.Domain.Payments;
using Nop.Core;
using YadiYad.Pro.Core.Infrastructure.Cache;
using Nop.Core.Caching;
using YadiYad.Pro.Services.Services.Attentions;

namespace YadiYad.Pro.Services.Job
{
    public class JobApplicationService : IEngagementService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<JobApplication> _JobApplicationRepository;
        private readonly IRepository<JobInvitation> _JobInvitationRepository;
        private readonly IRepository<JobProfile> _JobProfileRepository;
        private readonly IRepository<JobProfileExpertise> _JobProfileExpertiseRepository;
        private readonly IRepository<City> _CityRepository;
        private readonly IRepository<StateProvince> _StateProvinceRepository;
        private readonly IRepository<Country> _CountryRepository;
        private readonly IRepository<JobServiceCategory> _JobServiceCategoryRepository;
        private readonly IRepository<Expertise> _ExpertiseRepository;
        private readonly IRepository<OrganizationProfile> _OrganizationProfileRepository;
        private readonly IRepository<ServiceProfile> _ServiceProfileRepository;
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<IndividualProfile> _IndividualProfileRepository;
        private readonly IRepository<ServiceExpertise> _serviceExpertiseRepository;
        private readonly IRepository<ServiceLicenseCertificate> _serviceLicenseCertificateRepository;
        private readonly IRepository<ServiceAcademicQualification> _serviceAcademicQualificationRepository;
        private readonly IRepository<ServiceLanguageProficiency> _serviceLanguageProficiencyRepository;
        private readonly IRepository<CommunicateLanguage> _communicateLanguageRepository;
        private readonly IRepository<ConsultationProfile> _ConsultationProfileRepository;
        private readonly IRepository<ConsultationInvitation> _ConsultationInvitationRepository;
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;
        private readonly IRepository<Core.Domain.Common.TimeZone> _TimeZoneRepository;
        private readonly IRepository<ServiceSubscription> _serviceSubscriptionRepository;
        private readonly IRepository<JobMilestone> _jobMilestoneRepository;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepository;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly IRepository<JobApplicationMilestone> _jobApplicationMilestoneRepository;
        private readonly IRepository<PayoutRequest> _payoutRequestRepository;
        private readonly ProEngagementSettings _proEngagementSettings;
        private readonly OrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerService _customerService;
        private readonly IRepository<ProOrderItem> _ProOrderItemRepository;
        private readonly IRepository<ProOrder> _proOrderRepository;
        private readonly IRepository<RefundRequest> _RefundRequestRepository;
        private readonly IRepository<DepositRequest> _DepositRequestRepository;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IndividualAttentionService _individualAttentionService;
        private readonly OrganizationAttentionService _organizationAttentionService;

        public EngagementType EngagementType => EngagementType.Job;
        public EngagementPartyTypeInfo EngagementPartyTypeInfo => new EngagementPartyTypeInfo
        {
            Buyer = "Organization",
            Seller = "Freelancer",
            Moderator = "Moderator"
        };

        #endregion

        #region Ctor

        public JobApplicationService
            (IMapper mapper,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IRepository<ConsultationProfile> ConsultationProfileRepository,
            IRepository<ConsultationInvitation> ConsultationInvitationRepository,
            IRepository<JobApplication> JobApplicationRepository,
            IRepository<JobInvitation> JobInvitationRepository,
            IRepository<JobProfile> JobProfileRepository,
            IRepository<JobProfileExpertise> JobProfileExpertiseRepository,
            IRepository<City> CityRepository,
            IRepository<StateProvince> StateProvinceRepository,
            IRepository<Country> CountryRepository,
            IRepository<JobServiceCategory> JobServiceCategoryRepository,
            IRepository<Expertise> ExpertiseRepository,
            IRepository<OrganizationProfile> OrganizationProfileRepository,
            IRepository<ServiceProfile> ServiceProfileRepository,
            IRepository<Customer> CustomerRepository,
            IRepository<IndividualProfile> IndividualProfileRepository,
            IRepository<ServiceExpertise> serviceExpertiseRepository,
            IRepository<ServiceLicenseCertificate> serviceLicenseCertificateRepository,
            IRepository<ServiceAcademicQualification> serviceAcademicQualificationRepository,
            IRepository<ServiceLanguageProficiency> serviceLanguageProficiencyRepository,
            IRepository<CommunicateLanguage> communicateLanguageRepository,
            IRepository<BusinessSegment> BusinessSegmentRepository,
            IRepository<Core.Domain.Common.TimeZone> TimeZoneRepository,
            IRepository<ServiceSubscription> ServiceSubscriptionRepository,
            IRepository<JobMilestone> JobMilestoneRepository,
            IRepository<JobSeekerProfile> JobSeekerProfileRepository,
            IRepository<JobApplicationMilestone> jobApplicationMilestoneRepository,
            JobSeekerProfileService jobSeekerProfileService,
            IRepository<PayoutRequest> payoutRequestRepository,
            IDateTimeHelper dateTimeHelper,
            OrderService orderService,
            ProEngagementSettings proEngagementSettings,
            IndividualAttentionService individualAttentionService,
            ICustomerService customerService,
            IRepository<ProOrderItem> proOrderItemRepository,
            IRepository<ProOrder> proOrderRepository,
            IRepository<RefundRequest> RefundRequestRepository,
            IRepository<DepositRequest> DepositRequestRepository,
            OrganizationAttentionService organizationAttentionService)
        {
            _mapper = mapper;
            _dateTimeHelper = dateTimeHelper;
            _ConsultationProfileRepository = ConsultationProfileRepository;
            _CustomerRepository = CustomerRepository;
            _ConsultationInvitationRepository = ConsultationInvitationRepository;
            _JobApplicationRepository = JobApplicationRepository;
            _JobInvitationRepository = JobInvitationRepository;
            _JobProfileRepository = JobProfileRepository;
            _JobProfileExpertiseRepository = JobProfileExpertiseRepository;
            _CityRepository = CityRepository;
            _StateProvinceRepository = StateProvinceRepository;
            _CountryRepository = CountryRepository;
            _JobServiceCategoryRepository = JobServiceCategoryRepository;
            _ExpertiseRepository = ExpertiseRepository;
            _OrganizationProfileRepository = OrganizationProfileRepository;
            _ServiceProfileRepository = ServiceProfileRepository;
            _CustomerRepository = CustomerRepository;
            _IndividualProfileRepository = IndividualProfileRepository;
            _serviceExpertiseRepository = serviceExpertiseRepository;
            _serviceLicenseCertificateRepository = serviceLicenseCertificateRepository;
            _serviceAcademicQualificationRepository = serviceAcademicQualificationRepository;
            _serviceLanguageProficiencyRepository = serviceLanguageProficiencyRepository;
            _communicateLanguageRepository = communicateLanguageRepository;
            _BusinessSegmentRepository = BusinessSegmentRepository;
            _TimeZoneRepository = TimeZoneRepository;
            _serviceSubscriptionRepository = ServiceSubscriptionRepository;
            _jobMilestoneRepository = JobMilestoneRepository;
            _jobSeekerProfileRepository = JobSeekerProfileRepository;
            _jobSeekerProfileService = jobSeekerProfileService;
            _jobApplicationMilestoneRepository = jobApplicationMilestoneRepository;
            _payoutRequestRepository = payoutRequestRepository;
            _orderService = orderService;
            _proEngagementSettings = proEngagementSettings;
            _customerService = customerService;
            _ProOrderItemRepository = proOrderItemRepository;
            _proOrderRepository = proOrderRepository;
            _RefundRequestRepository = RefundRequestRepository;
            _DepositRequestRepository = DepositRequestRepository;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _individualAttentionService = individualAttentionService;
            _organizationAttentionService = organizationAttentionService;

        }

        #endregion

        #region Methods
        public virtual void CreateJobApplication(int actorId, JobApplicationDTO dto)
        {
            var model = _mapper.Map<JobApplication>(dto);

            //add pay amount
            var jobProfile = _JobProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == dto.JobProfileId)
                .Select(x => new
                {
                    x.PayAmount,
                    x.JobType,
                    x.JobRequired,
                    x.CustomerId
                })
                .First();

            if (jobProfile == null)
            {
                throw new InvalidOperationException("Pay amount cannot be 0.");
            }

            var jobSeekerProfile = _jobSeekerProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == dto.JobSeekerProfileId)
                .FirstOrDefault();

            if (jobSeekerProfile == null)
            {
                throw new InvalidOperationException("Job seeker profile not found.");
            }


            model.PayAmount = jobProfile.PayAmount;
            model.JobType = jobProfile.JobType;
            model.JobRequired = jobProfile.JobRequired;

            if (actorId != jobProfile.CustomerId)
            {
                model.IsRead = true;
                model.IsOrganizationRead = false;

                _organizationAttentionService.ClearOrganizationAttentionCache(jobProfile.CustomerId);
            }
            else if (actorId != jobSeekerProfile.CustomerId)
            {
                model.IsRead = false;
                model.IsOrganizationRead = true;

                _JobApplicationRepository.Insert(model);
                _individualAttentionService.ClearIndividualAttentionCache(jobSeekerProfile.CustomerId);
            }

            _JobApplicationRepository.Insert(model);

            //add job milestone
            var jobMilestones = _jobMilestoneRepository.Table
                .Where(x => x.Deleted == false
                && x.JobProfileId == dto.JobProfileId)
                .ToList();

            var jobApplicationMilestones = new List<JobApplicationMilestone>();

            foreach (var jobMilestone in jobMilestones)
            {
                var jobApplicationMilestone = _mapper.Map<JobApplicationMilestone>(jobMilestone);
                jobApplicationMilestone.JobApplicationId = model.Id;
                jobApplicationMilestone.CreateAudit(dto.CreatedById);

                jobApplicationMilestones.Add(jobApplicationMilestone);
            }

            _jobApplicationMilestoneRepository.Insert(jobApplicationMilestones);
        }

        public JobApplicationDTO UpdateJobApplicationStatus
            (int actorId,
            int jobApplicationId,
            JobApplicationStatus status,
            bool ensureIsEscrow = false)
        {
            var entity = _JobApplicationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == jobApplicationId)
                .FirstOrDefault();

            if (entity == null)
            {
                throw new KeyNotFoundException("Job application not found.");
            }

            #region validate job profile

            var jobProfile = _JobProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == entity.JobProfileId)
                .FirstOrDefault();

            if (status == JobApplicationStatus.Hired
                && jobProfile.Status != (int)JobProfileStatus.Publish)
            {
                throw new InvalidOperationException("The job ads is non long open for hiring.");
            }

            #endregion

            #region validate status update

            var currentStatus = (JobApplicationStatus)entity.JobApplicationStatus;

            if (ensureIsEscrow == true
                && entity.IsEscrow == true
                && status == JobApplicationStatus.Hired)
            {
                throw new InvalidOperationException("Job application status cannot update to Hired due to escrow.");
            }

            switch (status)
            {
                case JobApplicationStatus.UnderConsideration:
                case JobApplicationStatus.Interview:
                case JobApplicationStatus.Shortlisted:
                case JobApplicationStatus.KeepForFutureReference:
                    if (currentStatus != JobApplicationStatus.New
                        && currentStatus != JobApplicationStatus.UnderConsideration
                        && currentStatus != JobApplicationStatus.Interview
                        && currentStatus != JobApplicationStatus.Shortlisted
                        && currentStatus != JobApplicationStatus.KeepForFutureReference)
                    {
                        throw new InvalidOperationException($"Job application cannot update to \"{status.GetDescription()}\".");
                    }
                    break;
                case JobApplicationStatus.CancelledByIndividual:
                case JobApplicationStatus.CancelledByOrganization:
                    if (currentStatus != JobApplicationStatus.Hired)
                    {
                        throw new InvalidOperationException($"Job application cannot update to \"{status.GetDescription()}\".");
                    }
                    break;
                case JobApplicationStatus.PendingPaymentVerification:
                case JobApplicationStatus.RevisePaymentRequired:
                    switch (currentStatus)
                    {
                        case JobApplicationStatus.CancelledByIndividual:
                        case JobApplicationStatus.CancelledByOrganization:
                        case JobApplicationStatus.Hired:
                        case JobApplicationStatus.Completed:
                            throw new InvalidOperationException($"Job application cannot update to \"{status.GetDescription()}\".");
                    }
                    break;
            }

            if (status == JobApplicationStatus.PendingPaymentVerification
                || status == JobApplicationStatus.RevisePaymentRequired)
            {
                switch (status)
                {
                    case JobApplicationStatus.CancelledByIndividual:
                    case JobApplicationStatus.CancelledByOrganization:
                    case JobApplicationStatus.Hired:
                        throw new InvalidOperationException("Job application cannot update to pending payment verification");
                }
            }

            #endregion

            #region update job application

            JobApplication openForRehireJobApplication = null;

            if (status == JobApplicationStatus.Hired)
            {
                openForRehireJobApplication = GetOpenForRehireJobApplication(entity.JobProfileId);
                entity.NumberOfHiring = openForRehireJobApplication == null?1:2;
                entity.HiredTime = DateTime.UtcNow;
            }

            _individualAttentionService.ClearIndividualAttentionCache(actorId);

            entity.IsRead = false;
            entity.JobApplicationStatus = (int)status;
            entity.UpdateAudit(actorId);

            _JobApplicationRepository.Update(entity);

            #region update open for rehire job application

            if (openForRehireJobApplication != null)
            {
                openForRehireJobApplication.UpdateAudit(actorId);
                openForRehireJobApplication.RehiredobApplicationId = entity.Id;
                _JobApplicationRepository.Update(openForRehireJobApplication);
            }

            #endregion

            #endregion

            var jobseekerProfile = _jobSeekerProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == entity.JobSeekerProfileId)
                .FirstOrDefault();

            var dto = _mapper.Map<JobApplicationDTO>(entity);
            dto.JobProfile = _mapper.Map<JobProfileDTO>(jobProfile);
            dto.JobSeekerProfile = _mapper.Map<JobSeekerProfileDTO>(jobseekerProfile);

            _individualAttentionService.ClearIndividualAttentionCache(dto.JobSeekerProfile.CustomerId);

            return dto;
        }

        public JobApplicationDTO GetJobApplicationById(int id)
        {
            if (id == 0)
                return null;

            var query = 
                from ja in _JobApplicationRepository.Table
                .Where(ja=> ja.Deleted == false
                && ja.Id == id)
                from jp in _JobProfileRepository.Table
                .Where(jp=>jp.Deleted == false
                && jp.Id == ja.JobProfileId)
                from og in _OrganizationProfileRepository.Table
                .Where(og=>og.Deleted == false
                && og.Id == ja.OrganizationProfileId)
                from jsp in _jobSeekerProfileRepository.Table
                .Where(jsp=>jsp.Deleted == false
                && jsp.Id == ja.JobSeekerProfileId)
                from ip in _IndividualProfileRepository.Table
                .Where(ip=>ip.Deleted == false
                && ip.CustomerId == jsp.CustomerId)
                select new
                {
                    jobApplication = ja,
                    jobProfile = jp,
                    jobSeekerProfile = jsp,
                    serviceIndividualProfile = ip,
                    organizationProfile = og
                }
                ;

            var record = query
                .FirstOrDefault();

            if (record is null)
                return null;

            var response = _mapper.Map<JobApplicationDTO>(record.jobApplication);
            response.JobProfile = _mapper.Map<JobProfileDTO>(record.jobProfile);
            response.JobSeekerProfile = _mapper.Map<JobSeekerProfileDTO>(record.jobSeekerProfile);
            response.ServiceIndividualProfile = _mapper.Map<IndividualProfileDTO>(record.serviceIndividualProfile);
            response.OrganizationName = record.organizationProfile.Name;
            response.JobMilestones = _jobApplicationMilestoneRepository.Table
                .Where(x => x.Deleted == false
                && x.JobApplicationId == id)
                .Select(x => new JobMilestoneDTO
                {
                    Id = x.Id,
                    Sequence = x.Sequence,
                    Description = x.Description,
                    Amount = x.Amount
                })
                .ToList();

            return response;
        }

        public class ConsultationJobApplicationQueryModel
        {
            public int Id { get; set; }
            public int? ConsultationInvitationId { get; set; }
            public int? JobApplicationId { get; set; }
        }

        public PagedListDTO<JobApplicationDTO> GetJobApplications(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            JobApplicationListingFilterDTO filterDTO = null)
        {
            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;
            var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
            var localToday = localDateTime.Date;

            var record = (from cja
                          in (
                              (from ja in _JobApplicationRepository.Table
                              .Where(ja => !ja.Deleted)
                               select new ConsultationJobApplicationQueryModel
                               {
                                   Id = ja.Id,
                                   JobApplicationId = ja.Id,
                                   ConsultationInvitationId = null
                               })
                              .Concat(
                                from ci in _ConsultationInvitationRepository.Table
                                .Where(ci => !ci.Deleted
                                && ((ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                                    && ci.IsApproved == true)
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization))
                                select new ConsultationJobApplicationQueryModel
                                {
                                    Id = ci.Id,
                                    JobApplicationId = null,
                                    ConsultationInvitationId = ci.Id
                                })
                              )

                              //left join consultation invitation
                          from ci in _ConsultationInvitationRepository.Table
                          .Where(ci => !ci.Deleted
                          && ci.Id == cja.ConsultationInvitationId).DefaultIfEmpty()

                              //left join job application
                          from ja in _JobApplicationRepository.Table
                          .Where(ja => !ja.Deleted
                          && ja.Id == cja.JobApplicationId).DefaultIfEmpty()

                              //join service and individual profile
                          from jsp in _jobSeekerProfileRepository.Table
                          .Where(sp => !sp.Deleted
                          && ja != null
                          && sp.Id == ja.JobSeekerProfileId).DefaultIfEmpty()

                          from ssp in _ServiceProfileRepository.Table
                          .Where(ssp => !ssp.Deleted
                          && ci != null
                          && ssp.Id == ci.ServiceProfileId).DefaultIfEmpty()


                              //left join job profile
                          from jp in _JobProfileRepository.Table
                          .Where(j => !j.Deleted
                          && ja != null
                          && j.Id == ja.JobProfileId).DefaultIfEmpty()

                              //left join consultation profile
                          from cp in _ConsultationProfileRepository.Table
                          .Where(cp => !cp.Deleted
                          && ci != null
                          && cp.Id == ci.ConsultationProfileId).DefaultIfEmpty()

                              //inner join org profile with job or consultation job profile
                          from op in _OrganizationProfileRepository.Table
                          .Where(op => op.Deleted != true
                          && (op.CustomerId == jp.CustomerId
                          || op.Id == cp.OrganizationProfileId))

                              //left join job profile property
                          from s in _StateProvinceRepository.Table
                          .Where(s => s.Id == jp.StateProvinceId).DefaultIfEmpty()
                          from c in _CountryRepository.Table
                          .Where(c => c.Id == jp.CountryId).DefaultIfEmpty()
                          from ca in _JobServiceCategoryRepository.Table
                          .Where(ca => ca.Id == jp.CategoryId).DefaultIfEmpty()

                          from bs in _BusinessSegmentRepository.Table
                          .Where(bs => bs.Id == cp.SegmentId).DefaultIfEmpty()

                          from tz in _TimeZoneRepository.Table
                          .Where(tz => tz.Id == cp.TimeZoneId).DefaultIfEmpty()

                          select new
                          {
                              jobApplication = new JobApplicationDTO
                              {
                                  Id = ja != null ? ja.Id : ci.Id,
                                  JobApplicationStatus = ja != null ? ja.JobApplicationStatus : ci.ConsultationApplicationStatus,
                                  JobSeekerProfileId = ja != null ? ja.JobSeekerProfileId : 0,
                                  ServiceProfileId = ja != null ? 0 : ci.ServiceProfileId,
                                  JobProfileId = ja != null ? ja.JobProfileId : 0,
                                  ConsultationProfileId = ja != null ? 0 : ci.ConsultationProfileId,
                                  ModeratorCustomerId = ja != null ? 0 : ci.ModeratorCustomerId,
                                  CreatedOnUTC = ja != null ? ja.CreatedOnUTC : ci.CreatedOnUTC,
                                  UpdatedOnUTC = ja != null ? ja.UpdatedOnUTC : ci.UpdatedOnUTC,
                                  RatesPerSession = ja != null ? default(decimal?) : ci.RatesPerSession,
                                  IsRead = ja != null ? ja.IsRead : ci.IsIndividualRead,
                                  PayAmount = ja != null ? ja.PayAmount : 0,
                                  JobRequired = ja != null ? ja.JobRequired : default(int?),
                                  JobType = ja != null ? ja.JobType : 0,
                                  IsEscrow = ja != null ? ja.IsEscrow : false,
                                  StartDate = ja != null ? ja.StartDate : default(DateTime?),
                                  EndDate = ja != null ? ja.EndDate : default(DateTime?),
                                  EndMilestoneId = ja != null ? ja.EndMilestoneId : 0,
                                  DaysAbleToCancel = ja != null ? _proEngagementSettings.DaysAbleToCancel : 0,
                                  CanCancel = ja != null 
                                    ? (ja.StartDate.HasValue 
                                        && localToday <= ja.StartDate.Value.AddDays(_proEngagementSettings.DaysAbleToCancel) 
                                        && ja.JobApplicationStatus != (int)JobApplicationStatus.CancelledByIndividual
                                        && ja.JobApplicationStatus != (int)JobApplicationStatus.CancelledByOrganization) 
                                        ? true 
                                        : false 
                                    : false,
                                  AppointmentStartDate = ja != null ? default(DateTime?) : ci.AppointmentStartDate,
                                  AppointmentEndDate = ja != null ? default(DateTime?) : ci.AppointmentEndDate,
                                  RescheduleRemarks = ja != null ? default(string) : ci.RescheduleRemarks,
                                  CancellationEndRemarks = ja != null? ja.CancellationRemarks : null
                              },

                              serviceProfile = ssp,
                              jobSeekerProfile = jsp,

                              jobProfile = jp,
                              consultationProfile = cp,

                              organizationProfile = op,

                              state = s,
                              category = ca,
                              country = c,

                              Timezone = tz,
                              BusinessSegment = bs,
                              Questionnaire = ci.Questionnaire,
                              QuestionnaireAnswer = ci.QuestionnaireAnswer,
                              ConsultantAvailableTimeSlot = ci.ConsultantAvailableTimeSlot
                          });;


            if (filterDTO != null)
            {
                if (filterDTO.JobProfileId != 0)
                {
                    record = record.Where(x => x.jobApplication.JobProfileId == filterDTO.JobProfileId);
                }
                if (filterDTO.JobSeekerProfileId != 0)
                {
                    record = record.Where(x => x.jobApplication.JobSeekerProfileId == filterDTO.JobSeekerProfileId);
                }
                if (filterDTO.IndividualCustomerId != 0)
                {
                    record = record.Where(x => x.jobSeekerProfile.CustomerId == filterDTO.IndividualCustomerId
                    || x.serviceProfile.CustomerId == filterDTO.IndividualCustomerId);
                }
                if (filterDTO.OrganizationCustomerId != 0)
                {
                    record = record.Where(x => x.jobProfile.CustomerId == filterDTO.OrganizationCustomerId);
                }

                if (filterDTO.StartDate.HasValue)
                {
                    record = record.Where(x => x.jobApplication.CreatedOnUTC >= filterDTO.StartDate);
                }

                if (filterDTO.EndDate.HasValue)
                {
                    record = record.Where(x => x.jobApplication.CreatedOnUTC <= filterDTO.EndDate);
                }

            }

            var totalCount = record.Count();

            record = record.OrderByDescending(x => x.jobApplication.CreatedOnUTC);

            var records = record
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var response = records.Select(x =>
                {
                    var response = x.jobApplication;
                    if (x.jobSeekerProfile != null)
                    {
                        response.JobSeekerProfile = _mapper.Map<JobSeekerProfileDTO>(x.jobSeekerProfile);
                        response.JobSeekerProfile.EmploymentStatusName = ((EmploymentStatus)response.JobSeekerProfile.EmploymentStatus).GetDescription();
                    }
                    response.OrganizationName = x.organizationProfile.Name;
                    response.JobProfile = _mapper.Map<JobProfileDTO>(x.jobProfile);
                    response.JobTypeText =
                               response.JobType == (int)JobType.Freelancing
                                ? JobType.Freelancing.GetDescription()
                                : response.JobType == (int)JobType.ProjectBased
                                ? JobType.ProjectBased.GetDescription()
                                : response.JobType == (int)JobType.PartTime
                                ? JobType.PartTime.GetDescription()
                                : null;

                    if (response.JobProfile != null)
                    {
                        response.JobProfile.CategoryName = x.category.Name;
                        response.JobProfile.PreferredExperienceName =
                                   response.JobProfile.PreferredExperience == (int)ExperienceYear.YearLessThan10
                                    ? ExperienceYear.YearLessThan10.GetDescription()
                                    : response.JobProfile.PreferredExperience == (int)ExperienceYear.Year11To20
                                    ? ExperienceYear.Year11To20.GetDescription()
                                    : response.JobProfile.PreferredExperience == (int)ExperienceYear.Year21To30
                                    ? ExperienceYear.Year21To30.GetDescription()
                                    : null;
                    }

                    if (x.serviceProfile != null)
                    {
                        response.ServiceProfile = _mapper.Map<ServiceProfileDTO>(x.serviceProfile);
                        response.ServiceProfile.ExperienceYearName =
                                   response.ServiceProfile.YearExperience == (int)ExperienceYear.YearLessThan10
                                    ? ExperienceYear.YearLessThan10.GetDescription()
                                    : response.ServiceProfile.YearExperience == (int)ExperienceYear.Year11To20
                                    ? ExperienceYear.Year11To20.GetDescription()
                                    : response.ServiceProfile.YearExperience == (int)ExperienceYear.Year21To30
                                    ? ExperienceYear.Year21To30.GetDescription()
                                    : null;
                    }


                    //set consultation profile
                    response.ConsultationProfile = _mapper.Map<ConsultationProfileDTO>(x.consultationProfile);
                    if (response.ConsultationProfile != null)
                    {

                        response.ConsultationProfile.SegmentName = x.BusinessSegment.Name;
                        response.ConsultationProfile.TimeZoneName = x.Timezone.Name;

                        response.ConsultantAvailableTimeSlots
                            = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(
                                x.ConsultantAvailableTimeSlot ?? "null");

                        response.ConsultantReplys
                            = JsonConvert.DeserializeObject<List<QuestionDTO>>(
                                x.Questionnaire ?? "null");

                        var answers = JsonConvert.DeserializeObject<JObject>(x.QuestionnaireAnswer ?? "null");

                        if (answers != null)
                        {
                            foreach (var reply in response.ConsultantReplys)
                            {
                                var answer = answers.GetValue(reply.Name);

                                try
                                {
                                    reply.Answers = answer.ToObject<string>();
                                }
                                catch (ArgumentException)
                                {
                                    reply.Answers = answer.ToObject<List<string>>();
                                }
                            }
                        }
                    }

                    return response;
                }).ToList();

            //get service profile category
            var serviceProfileIds = response
                .Where(x => x.ServiceProfile != null)
                .Select(x => x.ServiceProfile.Id)
                .ToList();

            var serviceCategories =
                (from se in _serviceExpertiseRepository.Table
                .Where(se => se.Deleted == false
                && serviceProfileIds.Contains(se.ServiceProfileId))
                 from ex in _ExpertiseRepository.Table
                 .Where(ex => ex.Id == se.ExpertiseId)
                 from c in _JobServiceCategoryRepository.Table
                 .Where(c => c.Id == ex.JobServiceCategoryId)
                 select new
                 {
                     se.ServiceProfileId,
                     CategoryName = c.Name
                 })
                .Distinct()
                .ToList();

            var serviceProfiles = response.Where(x => x.ServiceProfile != null).ToList();

            serviceProfiles.ForEach(x =>
            {
                var categoryName = serviceCategories
                    .Where(y => y.ServiceProfileId == x.ServiceProfile.Id)
                    .Select(y => y.CategoryName)
                    .FirstOrDefault();

                x.ServiceProfile.CategoryName = categoryName;
            });

            //get job profile details.
            var jobProfileIds = response
                .Where(x => x.JobProfile != null)
                .Select(x => x.JobProfile.Id)
                .ToList();

            var jopApplicationIds = response
                    .Where(x => x.JobProfile != null)
                    .Select(x => x.Id)
                    .ToList();

            var jobExpertises = _JobProfileExpertiseRepository.Table
                .Where(x => jobProfileIds.Contains(x.JobProfieId))
                .Join(_ExpertiseRepository.Table,
                    jpe => jpe.ExpertiseId,
                    e => e.Id,
                    (jpe, e) => new
                    {
                        JobProfileExpertiseId = jpe.Id,
                        jpe.ExpertiseId,
                        ExpertiseName = e.Name,
                        JobProfileId = jpe.JobProfieId
                    })
                .ToList();

            foreach (var job in response)
            {
                if (job.JobProfile != null)
                {
                    job.JobProfile.RequiredExpertises = new List<ExpertiseDTO>();

                    var jobProfileExpertises = jobExpertises
                        .Where(x => x.JobProfileId == job.JobProfileId)
                        .Select(x => new ExpertiseDTO
                        {
                            Id = x.ExpertiseId,
                            Name = x.ExpertiseName
                        })
                        .ToList();

                    job.JobProfile.RequiredExpertises.AddRange(jobProfileExpertises);
                }
            }

            var milestones = _jobApplicationMilestoneRepository.Table
                .Where(x => x.Deleted == false
                && jopApplicationIds.Contains(x.JobApplicationId))
                .ToList();

            foreach (var jobApplication in response)
            {
                jobApplication.JobMilestones = milestones.Where(x => x.JobApplicationId == jobApplication.Id)
                    .OrderBy(x => x.Sequence)
                    .Select(x => new JobMilestoneDTO
                    {
                        Id = x.Id,
                        Sequence = x.Sequence,
                        Description = x.Description,
                        Amount = x.Amount
                    }).ToList();
            }

            var jobSeekerProfileIds = response.Where(x => x.JobSeekerProfile != null).Select(x => x.JobSeekerProfileId).ToList();

            var jobSeekerProfileDetails = _jobSeekerProfileService.GetJobSeekerProfileDetails(jobSeekerProfileIds);

            response = response
                .Select(x =>
                {
                    if (x.JobSeekerProfile != null)
                    {
                        var selectedJobSeekerProfileDetails = jobSeekerProfileDetails.Where(y => y.Id == x.JobSeekerProfileId).FirstOrDefault();
                        x.JobSeekerProfile.Categories = selectedJobSeekerProfileDetails.Categories;
                        x.JobSeekerProfile.AcademicQualifications = selectedJobSeekerProfileDetails.AcademicQualifications;
                        x.JobSeekerProfile.LicenseCertificates = selectedJobSeekerProfileDetails.LicenseCertificates;
                        x.JobSeekerProfile.LanguageProficiencies = selectedJobSeekerProfileDetails.LanguageProficiencies;
                        x.JobSeekerProfile.PreferredLocations = selectedJobSeekerProfileDetails.PreferredLocations;
                    }

                    return x;
                })
                    .ToList();

            foreach (var job in response)
            {
                if (job.JobProfile != null)
                {
                    if (job.JobProfile.CountryId != null)
                    {
                        var countryRecord = _CountryRepository.Table.Where(x => x.Id == job.JobProfile.CountryId).FirstOrDefault();
                        job.JobProfile.CountryName = countryRecord.Name;

                    }

                    if (job.JobProfile.StateProvinceId != null)
                    {
                        var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == job.JobProfile.StateProvinceId).FirstOrDefault();
                        job.JobProfile.StateProvinceName = stateProvinceRecord.Name;

                    }

                    if (job.JobProfile.CityId != null)
                    {
                        var cityRecord = _CityRepository.Table.Where(x => x.Id == job.JobProfile.CityId).FirstOrDefault();
                        job.JobProfile.CityName = cityRecord.Name;

                    }
                }
            }


            //get organization subscription status
            foreach (var job in response)
            {
                if (job.JobApplicationStatus != (int)JobApplicationStatus.Hired
                    && job.JobApplicationStatus != (int)JobApplicationStatus.Completed
                    && job.JobApplicationStatus != (int)JobApplicationStatus.CancelledByOrganization
                    && job.JobApplicationStatus != (int)JobApplicationStatus.CancelledByIndividual
                    && job.JobApplicationStatus != (int)JobApplicationStatus.Rejected
                    && job.JobProfile != null)
                {
                    var utcNow = DateTime.UtcNow;
                    var viewProfileEntitledEndDateTime = _serviceSubscriptionRepository.Table.Where(x =>
                    !x.Deleted
                    && x.CustomerId == job.JobProfile.CustomerId
                    && x.RefId == job.JobProfile.Id
                    && x.EndDate >= utcNow
                    && x.StartDate <= utcNow
                    && x.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile)
                    .Select(x => (DateTime?)(x.StopDate != null ? x.StopDate.Value : x.EndDate))
                    .OrderByDescending(x => x)
                    .FirstOrDefault<DateTime?>();

                    if (!viewProfileEntitledEndDateTime.HasValue
                        || viewProfileEntitledEndDateTime.Value < DateTime.UtcNow)
                    {
                        job.JobApplicationStatus = (int)JobApplicationStatus.Expired;
                        job.OrganizationName = null;
                        job.ServiceIndividualProfile = null;
                    }
                }
            }

            foreach (var item in response)
            {
                if (item.ModeratorCustomerId != 0)
                {
                    var cust = _customerService.GetCustomerById((int)item.ModeratorCustomerId);

                    item.ModeratorEmail = cust.Email;
                }
            }
            var responseDTOs = new PagedListDTO<JobApplicationDTO>(response, pageIndex, pageSize, totalCount);

            return responseDTOs;
        }

        public PagedListDTO<JobApplicationDTO> GetJobApplicationsForOrganization(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            JobApplicationListingFilterDTO filterDTO = null)
        {
            var query = _JobApplicationRepository.Table;

            var record = (from cja in _JobApplicationRepository.Table
                            .Where(ja => !ja.Deleted)
                          from sp in _jobSeekerProfileRepository.Table
                          .Where(sp => !sp.Deleted
                          && sp.Id == cja.JobSeekerProfileId)
                          from ip in _IndividualProfileRepository.Table
                          .Where(ip => !ip.Deleted
                          && ip.CustomerId == sp.CustomerId)

                              //inner join job profile
                          from jp in _JobProfileRepository.Table
                          .Where(j => !j.Deleted
                          && j.Id == cja.JobProfileId)

                              //inner join org profile with job profile
                          from op in _OrganizationProfileRepository.Table
                          .Where(op => op.Deleted != true
                          && (op.CustomerId == jp.CustomerId
                          || op.Id == cja.OrganizationProfileId))

                              //inner join job profile property
                          from s in _StateProvinceRepository.Table
                          .Where(s => s.Id == jp.StateProvinceId).DefaultIfEmpty()
                          from c in _CountryRepository.Table
                          .Where(c => c.Id == jp.CountryId).DefaultIfEmpty()
                          from ca in _JobServiceCategoryRepository.Table
                          .Where(ca => ca.Id == jp.CategoryId)


                          from ids in _StateProvinceRepository.Table
                          .Where(ids => ids.Id == ip.StateProvinceId).DefaultIfEmpty()
                          from idc in _CountryRepository.Table
                          .Where(idc => idc.Id == ip.CountryId).DefaultIfEmpty()
                          from idn in _CountryRepository.Table
                          .Where(idn => idn.Id == ip.NationalityId).DefaultIfEmpty()
                          select new
                          {
                              NationalityName = idn.Name,
                              StateProvinceName = ids.Name,
                              CountryName = idc.Name,

                              jobApplication = cja,

                              jobSeekerProfile = sp,
                              serviceIndividualProfile = ip,


                              jobProfile = jp,

                              organizationProfile = op,

                              state = s,
                              category = ca,
                              country = c
                          });


            if (filterDTO != null)
            {
                if (filterDTO.JobApplicationStatus != null)
                {
                    record = record.Where(x => filterDTO.JobApplicationStatus.Contains(x.jobApplication.JobApplicationStatus));
                }
                if (filterDTO.JobProfileId != 0)
                {
                    record = record.Where(x => x.jobApplication.JobProfileId == filterDTO.JobProfileId);
                }
                if (filterDTO.JobSeekerProfileId != 0)
                {
                    record = record.Where(x => x.jobApplication.JobSeekerProfileId == filterDTO.JobSeekerProfileId);
                }
                if (filterDTO.IndividualCustomerId != 0)
                {
                    record = record.Where(x => x.jobSeekerProfile.CustomerId == filterDTO.IndividualCustomerId);
                }
                if (filterDTO.OrganizationCustomerId != 0)
                {
                    record = record.Where(x => x.jobProfile.CustomerId == filterDTO.OrganizationCustomerId);
                }

                if (filterDTO.StartDate.HasValue)
                {
                    record = record.Where(x => x.jobApplication.CreatedOnUTC >= filterDTO.StartDate);
                }

                if (filterDTO.EndDate.HasValue)
                {
                    record = record.Where(x => x.jobApplication.CreatedOnUTC <= filterDTO.EndDate);
                }

                if (filterDTO.ExcludePendingApplicationIfHired)
                {
                    record = record.Where(x => 
                    x.jobApplication.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual
                    || x.jobApplication.JobApplicationStatus == (int)JobApplicationStatus.Completed
                    || x.jobApplication.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization
                    || x.jobApplication.JobApplicationStatus == (int)JobApplicationStatus.Hired);
                }
            }

            var totalCount = record.Count();

            record = record.OrderByDescending(x => x.jobApplication.CreatedOnUTC);

            var records = record
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            var response = records.Select(x =>
            {
                var response = _mapper.Map<JobApplicationDTO>(x.jobApplication);
                response.CancellationEndRemarks = x.jobApplication.CancellationRemarks;
                response.DaysAbleToCancel = _proEngagementSettings.DaysAbleToCancel;
                response.CanCancel = response.StartDate != null && DateTime.UtcNow <= (DateTime?)response.StartDate.Value.AddDays(_proEngagementSettings.DaysAbleToCancel) && 
                                        response.JobApplicationStatus != 12 && response.JobApplicationStatus != 13 ? true : false;
                response.JobSeekerProfile = _mapper.Map<JobSeekerProfileDTO>(x.jobSeekerProfile);
                response.ServiceIndividualProfile = _mapper.Map<IndividualProfileDTO>(x.serviceIndividualProfile);
                response.OrganizationName = x.organizationProfile.Name;

                response.JobSeekerProfile.EmploymentStatusName = ((EmploymentStatus)response.JobSeekerProfile.EmploymentStatus).GetDescription();

                response.ServiceIndividualProfile.GenderText = ((Gender)response.ServiceIndividualProfile.Gender).GetDescription();
                response.ServiceIndividualProfile.NationalityName = x.NationalityName;
                response.ServiceIndividualProfile.StateProvinceName = x.StateProvinceName;
                response.ServiceIndividualProfile.CountryName = x.CountryName;

                response.JobProfile = _mapper.Map<JobProfileDTO>(x.jobProfile);
                if (response.JobProfile != null)
                {
                    response.JobProfile.CategoryName = x.category.Name;
                    response.JobProfile.PreferredExperienceName =
                               response.JobProfile.PreferredExperience == (int)ExperienceYear.YearLessThan10
                                ? ExperienceYear.YearLessThan10.GetDescription()
                                : response.JobProfile.PreferredExperience == (int)ExperienceYear.Year11To20
                                ? ExperienceYear.Year11To20.GetDescription()
                                : response.JobProfile.PreferredExperience == (int)ExperienceYear.Year21To30
                                ? ExperienceYear.Year21To30.GetDescription()
                                : null;
                }


                return response;
            }).ToList();

            var jobProfileIds = response
                .Where(x => x.JobProfile != null)
                .Select(x => x.JobProfile.Id)
                .ToList();

            var jobExpertises = _JobProfileExpertiseRepository.Table
                .Where(x => jobProfileIds.Contains(x.JobProfieId))
                .Join(_ExpertiseRepository.Table,
                    jpe => jpe.ExpertiseId,
                    e => e.Id,
                    (jpe, e) => new
                    {
                        JobProfileExpertiseId = jpe.Id,
                        jpe.ExpertiseId,
                        ExpertiseName = e.Name,
                        JobProfileId = jpe.JobProfieId
                    })
                .ToList();

            foreach (var job in response)
            {
                if (job.JobProfile != null)
                {
                    job.JobProfile.RequiredExpertises = new List<ExpertiseDTO>();

                    var jobProfileExpertises = jobExpertises
                        .Where(x => x.JobProfileId == job.JobProfileId)
                        .Select(x => new ExpertiseDTO
                        {
                            Id = x.ExpertiseId,
                            Name = x.ExpertiseName
                        })
                        .ToList();

                    job.JobProfile.RequiredExpertises.AddRange(jobProfileExpertises);
                }
            }

            var jobSeekerProfileIds = response.Select(x => x.JobSeekerProfileId).ToList();

            var jobSeekerProfileDetails = _jobSeekerProfileService.GetJobSeekerProfileDetails(jobSeekerProfileIds);

            response = response
                .Select(x =>
                {
                    var selectedJobSeekerProfileDetails = jobSeekerProfileDetails.Where(y => y.Id == x.JobSeekerProfileId).FirstOrDefault();
                    x.JobSeekerProfile.Categories = selectedJobSeekerProfileDetails.Categories;
                    x.JobSeekerProfile.AcademicQualifications = selectedJobSeekerProfileDetails.AcademicQualifications;
                    x.JobSeekerProfile.LicenseCertificates = selectedJobSeekerProfileDetails.LicenseCertificates;
                    x.JobSeekerProfile.LanguageProficiencies = selectedJobSeekerProfileDetails.LanguageProficiencies;
                    x.JobSeekerProfile.PreferredLocations = selectedJobSeekerProfileDetails.PreferredLocations;
                    x.JobSeekerProfile.Cv = selectedJobSeekerProfileDetails.Cv;

                    return x;
                })
                    .ToList();

            foreach (var job in response)
            {
                if (job.JobProfile != null)
                {
                    if (job.JobProfile.CountryId != null)
                    {
                        var countryRecord = _CountryRepository.Table.Where(x => x.Id == job.JobProfile.CountryId).FirstOrDefault();
                        job.JobProfile.CountryName = countryRecord.Name;

                    }

                    if (job.JobProfile.StateProvinceId != null)
                    {
                        var stateProvinceRecord = _StateProvinceRepository.Table.Where(x => x.Id == job.JobProfile.StateProvinceId).FirstOrDefault();
                        job.JobProfile.StateProvinceName = stateProvinceRecord.Name;

                    }

                    if (job.JobProfile.CityId != null)
                    {
                        var cityRecord = _CityRepository.Table.Where(x => x.Id == job.JobProfile.CityId).FirstOrDefault();
                        job.JobProfile.CityName = cityRecord.Name;

                    }
                }
            }

            //get organization subscription status
            foreach (var job in response)
            {
                if (job.JobApplicationStatus != (int)JobApplicationStatus.Hired
                    && job.JobApplicationStatus != (int)JobApplicationStatus.Completed
                    && job.JobApplicationStatus != (int)JobApplicationStatus.CancelledByOrganization
                    && job.JobApplicationStatus != (int)JobApplicationStatus.CancelledByIndividual
                    && job.JobApplicationStatus != (int)JobApplicationStatus.Rejected
                    && job.JobProfile != null)
                {
                    var utcNow = DateTime.UtcNow;
                    var viewProfileEntitledEndDateTime = _serviceSubscriptionRepository.Table.Where(x =>
                    !x.Deleted
                    && x.CustomerId == job.JobProfile.CustomerId
                    && x.RefId == job.JobProfile.Id
                    && x.EndDate >= utcNow
                    && x.StartDate <= utcNow
                    && x.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile)
                    .Select(x => (DateTime?)(x.StopDate != null ? x.StopDate.Value : x.EndDate))
                    .OrderByDescending(x => x)
                    .FirstOrDefault<DateTime?>();

                    if (!viewProfileEntitledEndDateTime.HasValue || viewProfileEntitledEndDateTime.Value < DateTime.UtcNow)
                    {
                        job.JobApplicationStatus = (int)JobApplicationStatus.Expired;
                        job.OrganizationName = null;
                        job.ServiceIndividualProfile = null;
                    }
                }
            }

            var jopApplicationIds = response
                .Where(x => x.JobProfile != null)
                .Select(x => x.Id)
                .ToList();

            var milestones = _jobApplicationMilestoneRepository.Table
                .Where(x => x.Deleted == false
                && jopApplicationIds.Contains(x.JobApplicationId))
                .ToList();

            var posiibleRefundableEngagementIds = response
                .Where(x => x.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual)
                .Select(x => x.Id)
                .ToList();

            var refundableOrderItems = _orderService.GetRefundableOrderItems(ProductType.JobEnagegementFee, posiibleRefundableEngagementIds);

            foreach (var jobApplication in response)
            {
                jobApplication.JobMilestones = milestones.Where(x => x.JobApplicationId == jobApplication.Id)
                    .OrderBy(x => x.Sequence)
                    .Select(x => new JobMilestoneDTO
                    {
                        Id = x.Id,
                        Sequence = x.Sequence,
                        Description = x.Description,
                        Amount = x.Amount
                    }).ToList();

                jobApplication.CanRefund = refundableOrderItems.Any(x => x.RefId == jobApplication.Id);
                jobApplication.JobCancellationStatus = CheckJobApplicationCancellationStatus(jobApplication.Id);
            }

            var responseDTOs = new PagedListDTO<JobApplicationDTO>(response, pageIndex, pageSize, totalCount);

            //update read
            var unreadApplications =
                records.Where(x => x.jobApplication.IsOrganizationRead == false
                && x.jobApplication.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration)
                .Select(x => x.jobApplication)
                .ToList();

            if (unreadApplications != null
                && unreadApplications.Count() > 0)
            {
                foreach (var unreadApplication in unreadApplications)
                {
                    unreadApplication.IsOrganizationRead = true;
                    _JobApplicationRepository.Update(unreadApplication);
                }

                _organizationAttentionService.ClearOrganizationAttentionCache(filterDTO.OrganizationCustomerId);
            }

            return responseDTOs;
        }

        public void UpdateJobSeekJobApplicationRead(int jobApplicationId, int actorId)
        {
            var jobApplication =
                    (from ja in _JobApplicationRepository.Table
                    .Where(ja => ja.Deleted == false
                    && ja.Id == jobApplicationId
                    && (ja.JobApplicationStatus == (int)JobApplicationStatus.Hired
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.Interview
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.KeepForFutureReference
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.Shortlisted
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.Rejected
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization)
                    )
                     from sp in _jobSeekerProfileRepository.Table
                     .Where(sp => sp.Deleted == false
                     && sp.Id == ja.JobSeekerProfileId
                     && sp.CustomerId == actorId)
                     select ja)
                    .FirstOrDefault();

            if (jobApplication != null)
            {
                //jobApplication.UpdateAudit(actorId);
                jobApplication.IsRead = true;

                _JobApplicationRepository.Update(jobApplication);
                _individualAttentionService.ClearIndividualAttentionCache(actorId);
            }
        }

        public void UpdateOrgJobApplicationRead(int jobApplicationId, int actorId)
        {
            var jobApplication =
                    (from ja in _JobApplicationRepository.Table
                    where ja.Deleted == false
                    && ja.Id == jobApplicationId
                    from jp in _JobProfileRepository.Table
                    where jp.Deleted == false
                    && jp.Id == ja.JobProfileId
                    && jp.CustomerId == actorId
                     select ja)
                      .FirstOrDefault();

            if (jobApplication != null)
            {
                jobApplication.IsOrganizationRead = true;

                _JobApplicationRepository.Update(jobApplication);
                _organizationAttentionService.ClearOrganizationAttentionCache(actorId);
            }
        }

        public virtual void ReviewJobApplication(int actorId, int id, JobApplicationReviewDTO dto)
        {
            var model = _JobApplicationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }
            model.UpdateAudit(actorId);

            model.ReviewDateTime = DateTime.UtcNow;
            model.ReviewText = dto.ReviewText;
            model.Rating = (dto.KnowledgenessRating
                + dto.ProfessionalismRating
                + dto.RelevanceRating
                + dto.RespondingRating
                + dto.ClearnessRating) / 5.0m;
            model.KnowledgenessRating = dto.KnowledgenessRating;
            model.ProfessionalismRating = dto.ProfessionalismRating;
            model.RelevanceRating = dto.RelevanceRating;
            model.RespondingRating = dto.RespondingRating;
            model.ClearnessRating = dto.ClearnessRating;

            _JobApplicationRepository.Update(model);
        }

        public bool UpdateJobApplicationStartDate(
            int actorId,
            int id,
            UpdateJobApplicationStartDateDTO request)
        {
            var result = false;

            var jobApplication =
                    (from ja in _JobApplicationRepository.Table
                        .Where(ja => ja.Deleted == false
                        && ja.Id == id
                        && (ja.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration
                            || ja.JobApplicationStatus == (int)JobApplicationStatus.Shortlisted
                            || ja.JobApplicationStatus == (int)JobApplicationStatus.KeepForFutureReference
                            || ja.JobApplicationStatus == (int)JobApplicationStatus.New))
                     from jp in _JobProfileRepository.Table
                         .Where(jp => jp.Deleted == false
                         && jp.Id == ja.JobProfileId
                         && jp.CustomerId == actorId)
                     select ja)
                    .FirstOrDefault();

            if (jobApplication == null)
            {
                throw new InvalidOperationException("Job application not found.");
            }

            jobApplication.StartDate = request.StartDate;
            jobApplication.UpdateAudit(actorId);

            _JobApplicationRepository.Update(jobApplication);

            result = true;

            return result;
        }
        public bool UpdateJobApplicationEndDate(
            int actorId,
            int id,
            UpdateJobApplicationEndDateDTO request)
        {
            var result = false;

            var jobApplication =
                    (from ja in _JobApplicationRepository.Table
                        .Where(ja => ja.Deleted == false
                        && ja.Id == id
                        && (ja.JobApplicationStatus == (int)JobApplicationStatus.Hired)
                            )
                     from jp in _JobProfileRepository.Table
                         .Where(jp => jp.Deleted == false
                         && jp.Id == ja.JobProfileId
                         && jp.CustomerId == actorId)
                     select ja)
                    .FirstOrDefault();

            if (jobApplication == null)
            {
                throw new InvalidOperationException("Job application not found.");
            }

            if(jobApplication.JobType == (int)JobType.ProjectBased)
            {
                if(request.EndMilestoneId == null)
                {
                    throw new InvalidOperationException("End milestone id cannot be null.");
                }
                jobApplication.EndMilestoneId = request.EndMilestoneId;

            }
            else
            {
                if (request.EndDate == null)
                {
                    throw new InvalidOperationException("End date cannot be null.");
                }
                jobApplication.EndDate = request.EndDate;

            }

            jobApplication.CancellationRemarks = request.Remarks;
            jobApplication.UpdateAudit(actorId);

            _JobApplicationRepository.Update(jobApplication);

            result = true;

            return result;
        }

        public EngagementPartyInfo GetEngagingParties(int jobApplicationId)
        {
            return QueryableEngagementParties().Where(q => q.EngagementId == jobApplicationId).FirstOrDefault();
        }

        public IQueryable<EngagementPartyInfo> QueryableEngagementParties()
        {
            return from ja in _JobApplicationRepository.Table
                   join oc in _OrganizationProfileRepository.Table on ja.OrganizationProfileId equals oc.Id
                   join js in _jobSeekerProfileRepository.Table on ja.JobSeekerProfileId equals js.Id
                   join c in _IndividualProfileRepository.Table on js.CustomerId equals c.CustomerId
                   select new EngagementPartyInfo
                   {
                       EngagementId = ja.Id,
                       EngagementType = EngagementType,
                       IsEscrow = ja.IsEscrow,
                       BuyerId = oc.CustomerId,
                       BuyerName = oc.Name,
                       SellerId = c.CustomerId,
                       SellerName = c.FullName,
                   };
        }

        public bool Cancel(int jobApplicationId, EngagementParty cancellingParty, int actorId)
        {
            var jobApplication = _JobApplicationRepository.Table
                                .Where(s => s.Id == jobApplicationId)
                                .FirstOrDefault();
                                
            // validate before cancel
            if(jobApplication.JobApplicationStatus != (int)JobApplicationStatus.Hired)
                return false;
            //

            if (cancellingParty == EngagementParty.Seller)
                jobApplication.JobApplicationStatus = (int)JobApplicationStatus.CancelledByIndividual;
            else
            {
                jobApplication.JobApplicationStatus = (int)JobApplicationStatus.CancelledByOrganization;
            }

            jobApplication.UpdateAudit(actorId);

            _JobApplicationRepository.Update(jobApplication);

            var jobProfile = _JobProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == jobApplication.JobProfileId)
                .FirstOrDefault();

            if(jobProfile.Status != (int)JobProfileStatus.Hired)
            {
                return false;
            }

            jobProfile.Status = (int)JobProfileStatus.Publish;
            jobProfile.UpdateAudit(actorId);
            _JobProfileRepository.Update(jobProfile);

            return true;
        }

        public void UpdateCancel(
            int jobApplicationId,
            DateTime submissionTime,
            string userRemarks,
            int reasonId,
            int? attachmentId,
            EngagementParty cancellationParty)
        {
            var jobApplication = _JobApplicationRepository.Table
                                .Where(s => s.Id == jobApplicationId)
                                .FirstOrDefault();

            jobApplication.CancellationReasonId = reasonId;
            jobApplication.CancellationRemarks = userRemarks;

            if (cancellationParty != EngagementParty.Buyer)
            {
                jobApplication.IsOrganizationRead = false;
            }
            if (cancellationParty != EngagementParty.Seller)
            {
                jobApplication.IsRead = false;
            }
            
            jobApplication.CancellationDownloadId = attachmentId;
            jobApplication.CancellationDateTime = submissionTime;

            _JobApplicationRepository.Update(jobApplication);

        }

        public DateTime? GetStartDate(int jobApplicationId)
        {
            return _JobApplicationRepository.Table
            .Where(q => q.Id == jobApplicationId)
            .Select(q => q.StartDate)
            .FirstOrDefault();
        }

        public List<JobMilestoneDTO> GetEntitledTerminateMilestone(int jobApplicationId)
        {
            var payoutRequest = _payoutRequestRepository.Table
                .Where(x => x.JobApplicationMilestoneId != null)
                .Select(x => x.JobApplicationMilestoneId)
                .ToList();

            var milestones = _jobApplicationMilestoneRepository.Table
                .Where(x => x.Deleted == false
                && x.JobApplicationId == jobApplicationId
                && !payoutRequest.Contains(x.Id))
                .Select(x => new JobMilestoneDTO
                {
                    Id = x.Id,
                    Sequence = x.Sequence,
                    Description = x.Description,
                    Amount = x.Amount
                })
                .OrderBy(x => x.Sequence)
                .ToList();

            return milestones;
        }

        public JobApplication GetOpenForRehireJobApplication(int jobProfileId)
        {
            var lastHireJobApplication
                = (from ja in _JobApplicationRepository.Table
                    .Where(ja => ja.Deleted == false
                    && ja.JobProfileId == jobProfileId
                    && (ja.JobApplicationStatus == (int)JobApplicationStatus.Hired
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual
                        || ja.JobApplicationStatus == (int)JobApplicationStatus.Completed))
                   from poi in _ProOrderItemRepository.Table
                       .Where(poi => poi.Deleted == false
                       && poi.RefId == ja.Id
                       && poi.ProductTypeId == (int)ProductType.JobEnagegementFee)
                       .DefaultIfEmpty()
                   from po in _proOrderRepository.Table
                       .Where(po => po.Deleted == false
                       && po.Id == poi.OrderId
                       && po.PaymentStatusId == (int)PaymentStatus.Paid)
                       .DefaultIfEmpty()
                   select new
                   {
                       JobApplication = ja,
                       IsOpenForRematch = poi != null && po != null ? poi.Status == (int)ProOrderItemStatus.OpenForRematch : false,
                       ja.IsEscrow,
                       ja.HiredTime,
                       ja.NumberOfHiring
                   })
                .OrderByDescending(ja => ja.HiredTime)
                .FirstOrDefault();

            if (lastHireJobApplication == null)
            {
                return null;
            }
            else if (lastHireJobApplication.IsEscrow == false
                && lastHireJobApplication.NumberOfHiring == 2)
            {
                return null;
            }
            else if (lastHireJobApplication.IsEscrow == false
                && lastHireJobApplication.NumberOfHiring == 1)
            {
                return lastHireJobApplication.JobApplication;
            }
            else if(lastHireJobApplication.IsEscrow == true
                && lastHireJobApplication.IsOpenForRematch == true)
            {
                return lastHireJobApplication.JobApplication;
            }
            else if (lastHireJobApplication.IsEscrow == true
                && lastHireJobApplication.IsOpenForRematch == false)
            {
                return null;
            }

            throw new NopException("Fail to identify next hiring number.");
        }

        public int? CheckJobApplicationCancellationStatus(int jobApplicationId)
        {
            int? jobCancellationStatus = null;

            var jobApplication = _JobApplicationRepository.Table
                .Where(x => x.Id == jobApplicationId)
                .FirstOrDefault();

            if (jobApplication.NumberOfHiring == 1 && jobApplication.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual)
            {
                if (jobApplication.RehiredobApplicationId != null)
                {
                    jobCancellationStatus = (int)JobCancellationStatus.FirstCancellationAlreadyRehired;
                }
                else
                {
                    var proOrderItemId = _ProOrderItemRepository.Table
                            .Where(x => x.ProductTypeId == (int)ProductType.JobEnagegementFee
                                && x.RefId == jobApplicationId
                                && x.Status == (int)ProOrderItemStatus.Paid)
                            .Select(x => x.Id)
                            .FirstOrDefault();

                    var raisedRefundRequest = _RefundRequestRepository.Table
                                              .Where(x => x.OrderItemId == proOrderItemId)
                                              .FirstOrDefault();

                    if (raisedRefundRequest != null)
                    {
                        jobCancellationStatus = (int)JobCancellationStatus.FirstCancellationProceedRefund;
                    }
                    else
                    {
                        jobCancellationStatus = (int)JobCancellationStatus.FirstCancellationProceedRehire;

                    }
                }
            }
            else if (jobApplication.NumberOfHiring == 2 && jobApplication.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual)
            {
                if (jobApplication.IsEscrow)
                {
                    jobCancellationStatus = (int)JobCancellationStatus.SecondCancellationDepositCollected;
                }
                else
                {
                    jobCancellationStatus = (int)JobCancellationStatus.SecondCancellationDepositNotCollected;
                }
            }
            return jobCancellationStatus;
        } 

        #endregion
    }
}
