using AutoMapper;
using Nop.Core;
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
using YadiYad.Pro.Services.DTO.Consultation;
using Newtonsoft.Json;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.JobSeeker;
using Newtonsoft.Json.Linq;
using YadiYad.Pro.Services.Services.Attentions;

namespace YadiYad.Pro.Services.Job
{
    public class JobInvitationService
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
        private readonly JobApplicationService _jobApplicationService;
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;
        private readonly IRepository<Core.Domain.Common.TimeZone> _TimeZoneRepository;
        private readonly IRepository<ServiceExpertise> _serviceExpertiseRepository;
        private readonly IRepository<ServiceLicenseCertificate> _serviceLicenseCertificateRepository;
        private readonly IRepository<ServiceAcademicQualification> _serviceAcademicQualificationRepository;
        private readonly IRepository<ServiceLanguageProficiency> _serviceLanguageProficiencyRepository;
        private readonly IRepository<CommunicateLanguage> _communicateLanguageRepository;
        private readonly IRepository<ConsultationProfile> _ConsultationProfileRepository;
        private readonly IRepository<ConsultationInvitation> _ConsultationInvitationRepository;
        private readonly IRepository<ServiceSubscription> _serviceSubscriptionRepository;
        private readonly IRepository<JobMilestone> _jobMilestoneRepository;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepository;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        private readonly IndividualAttentionService _individualAttentionService;


        #endregion

        #region Ctor

        public JobInvitationService
            (IMapper mapper,
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
            JobApplicationService JobApplicationService,
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
            JobSeekerProfileService jobSeekerProfileService,
            IndividualAttentionService individualAttentionService)
        {
            _mapper = mapper;
            _ConsultationProfileRepository = ConsultationProfileRepository;
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
            _jobApplicationService = JobApplicationService;
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
            _individualAttentionService = individualAttentionService;


        }

        #endregion


        #region Methods
        // only organization can create jobInvitation
        public virtual void CreateJobInvitation(JobInvitationDTO dto)
        {
            var request = _mapper.Map<JobInvitation>(dto);

            _individualAttentionService.ClearIndividualAttentionCache(dto.JobSeekerProfile.CustomerId);

            _JobInvitationRepository.Insert(request);
        }

        // only jobseeker can update jobInvitation
        public virtual void UpdateJobInvitation(JobInvitationDTO dto)
        {
            var request = _mapper.Map<JobInvitation>(dto);
            _JobInvitationRepository.Update(request);
        }

        public JobInvitationDTO GetJobInvitationById(int id)
        {
            if (id == 0)
                return null;

            var query = _JobInvitationRepository.Table;

            var record = query
                .Join(_JobProfileRepository.Table, x => x.JobProfileId, jobProfile => jobProfile.Id, (jobInvitation, jobProfile) => new { jobInvitation, jobProfile })
                .Join(_OrganizationProfileRepository.Table, x => x.jobInvitation.OrganizationProfileId, organizationProfile => organizationProfile.Id, (x, organizationProfile) => new { x.jobInvitation, x.jobProfile, organizationProfile })
                .Join(_jobSeekerProfileRepository.Table, x => x.jobInvitation.JobSeekerProfileId, jobSeekerProfile => jobSeekerProfile.Id, (x, jobSeekerProfile) => new { x.jobInvitation, x.jobProfile, x.organizationProfile, jobSeekerProfile })
                .Join(_IndividualProfileRepository.Table, x => x.jobSeekerProfile.CustomerId, individualProfile => individualProfile.CustomerId, (x, serviceIndividualProfile) => new { x.jobInvitation, x.jobProfile, x.organizationProfile, x.jobSeekerProfile, serviceIndividualProfile })
                .Where(x => x.jobInvitation.Id == id && !x.jobInvitation.Deleted)
                .FirstOrDefault();

            if (record is null)
                return null;

            var response = _mapper.Map<JobInvitationDTO>(record.jobInvitation);
            response.JobProfile = _mapper.Map<JobProfileDTO>(record.jobProfile);
            response.JobSeekerProfile = _mapper.Map<JobSeekerProfileDTO>(record.jobSeekerProfile);
            response.ServiceIndividualProfile = _mapper.Map<IndividualProfileDTO>(record.serviceIndividualProfile);
            response.OrganizationName = record.organizationProfile.Name;


            return response;
        }

        public PagedListDTO<JobInvitationDTO> GetJobInvitations(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            JobInvitationListingFilterDTO filterDTO = null)
        {
            var record = (from cji
                          in (
                              (from ji in _JobInvitationRepository.Table
                              .Where(ja => !ja.Deleted
                              && ja.JobInvitationStatus != (int)JobInvitationStatus.Accepted)
                               select new JobInvitationDTO
                               {
                                   Id = ji.Id,
                                   ConsultationInvitationId = 0,
                                   JobInvitationStatus = ji.JobInvitationStatus,
                                   JobSeekerProfileId = ji.JobSeekerProfileId,
                                   JobProfileId = ji.JobProfileId,
                                   ConsultationProfileId = 0,
                                   Questionnaire = default(string),
                                   CreatedOnUTC = ji.CreatedOnUTC,
                                   UpdatedOnUTC = ji.UpdatedOnUTC,
                                   IsApproved = null,
                                   ApprovalRemarks = null
                               })
                              .Union(
                                from ci in _ConsultationInvitationRepository.Table
                                .Where(ci => !ci.Deleted
                                && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByOrganization
                                || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted))
                                select new JobInvitationDTO
                                {
                                    Id = ci.Id,
                                    ConsultationInvitationId = ci.Id,
                                    JobInvitationStatus = ci.ConsultationApplicationStatus,
                                    JobSeekerProfileId = 0,
                                    ServiceProfileId = ci.ServiceProfileId,
                                    JobProfileId = 0,
                                    ConsultationProfileId = ci.ConsultationProfileId,
                                    Questionnaire = ci.Questionnaire,
                                    CreatedOnUTC = ci.CreatedOnUTC,
                                    UpdatedOnUTC = ci.UpdatedOnUTC,
                                    IsApproved = ci.IsApproved,
                                    ApprovalRemarks = ci.ApprovalRemarks
                                })
                              )
                              //join service and individual profile
                          from jsp in _jobSeekerProfileRepository.Table
                          .Where(jsp => !jsp.Deleted
                          && jsp.Id == cji.JobSeekerProfileId).DefaultIfEmpty()

                          from ssp in _ServiceProfileRepository.Table
                          .Where(ssp => !ssp.Deleted
                          && ssp.Id == cji.ServiceProfileId).DefaultIfEmpty()

                          from ip in _IndividualProfileRepository.Table
                         .Where(ip => !ip.Deleted
                         && ((jsp != null && ip.CustomerId == jsp.CustomerId)
                         || (ssp != null && ip.CustomerId == ssp.CustomerId))
                         )

                              //left join consultation invitation
                          from ci in _ConsultationInvitationRepository.Table
                          .Where(ci => !ci.Deleted
                          && cji.ConsultationProfileId != 0
                          && ci.Id == cji.Id).DefaultIfEmpty()

                              //left join job profile
                          from jp in _JobProfileRepository.Table
                          .Where(j => !j.Deleted
                          && j.Id == cji.JobProfileId).DefaultIfEmpty()

                              //left join consultation profile
                          from cp in _ConsultationProfileRepository.Table
                          .Where(cp => !cp.Deleted
                          && cp.IsApproved == true
                          && cp.Id == cji.ConsultationProfileId).DefaultIfEmpty()

                              //inner join org profile with job or consultation job profile
                          from op in _OrganizationProfileRepository.Table
                          .Where(op => op.Deleted != true
                          && ((cp == null 
                            && op.CustomerId == jp.CustomerId)
                          || (cp != null 
                            && op.Id == cp.OrganizationProfileId)))

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
                              jobInvitation = cji,

                              jobSeekerProfile = jsp,
                              serviceProfile = ssp,

                              serviceIndividualProfile = ip,

                              jobProfile = jp,
                              consultationProfile = cp,

                              organizationProfile = op,

                              state = s,
                              category = ca,
                              country = c,

                              Timezone = tz,
                              BusinessSegment = bs,
                              QuestionnaireAnswer = ci.QuestionnaireAnswer,
                              ConsultantAvailableTimeSlot = ci.ConsultantAvailableTimeSlot,
                              RatesPerSession = ci.RatesPerSession
                          });

            //list consultation job which approved by moderator.
            //record = record.Where(x =>
            //x.consultationProfile == null ||
            //    (x.consultationProfile != null
            //    && x.consultationProfile.IsApproved == true));

            record = record.OrderByDescending(x => x.jobInvitation.CreatedOnUTC);

            if (filterDTO != null)
            {
                if (filterDTO.IndividualCustomerId != 0)
                {
                    record = record.Where(x => 
                        (x.jobSeekerProfile != null && x.jobSeekerProfile.CustomerId == filterDTO.IndividualCustomerId)
                        || (x.serviceProfile != null && x.serviceProfile.CustomerId == filterDTO.IndividualCustomerId));
                }
                if (filterDTO.OrganizationCustomerId != 0)
                {
                    record = record.Where(x => x.jobProfile.CustomerId == filterDTO.OrganizationCustomerId);
                }
                if (filterDTO.JobInvitationStatus.Count > 0)
                {
                    record = record.Where(x => filterDTO.JobInvitationStatus.Contains(
                        x.consultationProfile == null
                        ? x.jobInvitation.JobInvitationStatus
                        : x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.New
                        ? (int)JobInvitationStatus.Pending
                        : (x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.Accepted
                            && x.jobInvitation.IsApproved == null)
                        ? (int)JobInvitationStatus.Reviewing
                        : (x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.Accepted
                            && x.jobInvitation.IsApproved == true)
                        ? (int)JobInvitationStatus.Accepted
                        : (x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.Accepted
                            && x.jobInvitation.IsApproved == false)
                        ? (int)JobInvitationStatus.UpdateRequired
                        : x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.Accepted
                        ? (int)JobInvitationStatus.Accepted
                        : x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual || x.jobInvitation.JobInvitationStatus == (int)ConsultationInvitationStatus.DeclinedByOrganization
                        ? (int)JobInvitationStatus.Declined
                        : 0
                        ));
                }
                if (filterDTO.JobProfileId != 0)
                {
                    record = record.Where(x => x.jobProfile.Id == filterDTO.JobProfileId);
                }
            }

            var totalCount = record.Count();
            var records = record
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();



            var response = records
                .Select(x =>
                {
                    var response = x.jobInvitation;
                    response.OrganizationName = x.organizationProfile.Name;

                    response.JobInvitationStatusName =
                        response.JobInvitationStatus == (int)JobInvitationStatus.Pending
                        ? JobInvitationStatus.Pending.GetDescription()
                        : response.JobInvitationStatus == (int)JobInvitationStatus.Accepted
                        ? JobInvitationStatus.Accepted.GetDescription()
                        : response.JobInvitationStatus == (int)JobInvitationStatus.Declined
                        ? JobInvitationStatus.Declined.GetDescription()
                        : response.JobInvitationStatus == (int)ConsultationInvitationStatus.DeclinedByOrganization
                        ? JobInvitationStatus.Declined.GetDescription()
                        : null;


                    response.ServiceIndividualProfile = _mapper.Map<IndividualProfileDTO>(x.serviceIndividualProfile);
                    response.ServiceIndividualProfile.GenderText = ((Gender)response.ServiceIndividualProfile.Gender).GetDescription();
                    response.ServiceIndividualProfile.NationalityName = response.ServiceIndividualProfile.NationalityId.ToString();
                    response.ServiceIndividualProfile.StateProvinceName = response.ServiceIndividualProfile.StateProvinceId.ToString();
                    response.ServiceIndividualProfile.CountryName = response.ServiceIndividualProfile.CountryId.ToString();


                    if (x.jobSeekerProfile != null)
                    {
                        response.JobSeekerProfile = _mapper.Map<JobSeekerProfileDTO>(x.jobSeekerProfile);
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

                    if (x.jobProfile != null)
                    {
                        response.JobProfile = _mapper.Map<JobProfileDTO>(x.jobProfile);
                        response.JobProfile.StateProvinceName = x.state?.Name;
                        response.JobProfile.CategoryName = x.category?.Name;
                        response.JobProfile.CountryName = x.country?.Name;
                        response.JobProfile.JobSchedule = x.jobProfile.JobSchedule;

                        response.JobProfile.PreferredExperienceName =
                                   response.JobProfile.PreferredExperience == (int)ExperienceYear.YearLessThan10
                                    ? ExperienceYear.YearLessThan10.GetDescription()
                                    : response.JobProfile.PreferredExperience == (int)ExperienceYear.Year11To20
                                    ? ExperienceYear.Year11To20.GetDescription()
                                    : response.JobProfile.PreferredExperience == (int)ExperienceYear.Year21To30
                                    ? ExperienceYear.Year21To30.GetDescription()
                                    : null;
                    }

                    //set consultation profile
                    if (x.consultationProfile != null)
                    {
                        response.ConsultationProfile = _mapper.Map<ConsultationProfileDTO>(x.consultationProfile);
                        response.ConsultationProfile.SegmentName = x.BusinessSegment.Name;
                        response.ConsultationProfile.TimeZoneName = x.Timezone.Name;

                        response.ConsultationProfile.Questions
                            = JsonConvert.DeserializeObject<List<QuestionDTO>>(response.ConsultationProfile.Questionnaire);
                        response.ConsultationProfile.TimeSlots
                            = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(response.ConsultationProfile.AvailableTimeSlot);

                        if(string.IsNullOrWhiteSpace(x.ConsultantAvailableTimeSlot) == false)
                        {
                            response.ConsultantAvailableTimeSlots
                                = JsonConvert.DeserializeObject<List<TimeSlotDTO>>(x.ConsultantAvailableTimeSlot);

                            foreach(var timeslot in response.ConsultationProfile.TimeSlots)
                            {
                                var isTimeSlotSelected = response.ConsultantAvailableTimeSlots.Any(x => 
                                x != null 
                                && x.EndDate == timeslot.EndDate 
                                && x.StartDate == timeslot.StartDate);

                                timeslot.Selected = isTimeSlotSelected;
                            }
                        }

                        response.RatesPerSession = x.RatesPerSession;
                        response.ConsultantReplys
                            = JsonConvert.DeserializeObject<List<QuestionDTO>>(response.ConsultationProfile.Questionnaire);

                        if (string.IsNullOrWhiteSpace(x.QuestionnaireAnswer) == false)
                        {
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

                        response.ConsultantReplys.Where(x => x.Type == "checkbox").ToList().ForEach(x =>
                          {
                              if (x.Answers == null)
                              {
                                  x.Answers = new List<string>();
                              }
                          });
                    }

                    return response;
                }).ToList();

            //get service profile category
            var serviceProfileIds = response
                .Where(x=>x.ServiceProfile != null)
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
                    .Select(y=>y.CategoryName)
                    .FirstOrDefault();

                x.ServiceProfile.CategoryName = categoryName;
            });

            var jobProfileIds = response
                .Where(x => x.JobProfile != null)
                .Select(x => x.JobProfile.Id).ToList();

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

            var milestones = _jobMilestoneRepository.Table
                    .Where(x => x.Deleted == false && jobProfileIds.Contains(x.JobProfileId))
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

                    job.JobProfile.JobMilestones = milestones.Where(x => x.JobProfileId == job.JobProfileId)
                        .OrderBy(x => x.Sequence)
                        .Select(x => new JobMilestoneDTO
                        {
                            Id = x.Id,
                            Sequence = x.Sequence,
                            Description = x.Description,
                            Amount = x.Amount
                        }).ToList();
                }
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
                        x.JobSeekerProfile.Cv = selectedJobSeekerProfileDetails.Cv;
                    }

                    return x;
                })
                    .ToList();

            //get organization subscription status
            foreach (var job in response)
            {
                if (job.JobProfile != null)
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
                        job.JobInvitationStatus = (int)JobInvitationStatus.Expired;
                        job.JobInvitationStatusName = JobInvitationStatus.Expired.GetDescription();
                        job.OrganizationName = null;
                        job.ServiceIndividualProfile = null;

                    }
                }
            }


            var responseDTOs = new PagedListDTO<JobInvitationDTO>(response, pageIndex, pageSize, totalCount);

            return responseDTOs;
        }

        public void UpdateOrganizationJobInvitationRead(int jobInvitationId, int actorId)
        {
            var jobInvitation =
                    (from ji in _JobInvitationRepository.Table
                    .Where(ji => ji.Deleted == false
                    && ji.JobInvitationStatus == (int)JobInvitationStatus.Declined
                    && ji.Id == jobInvitationId)
                     from org in _OrganizationProfileRepository.Table
                     .Where(org => org.Deleted == false
                     && org.Id == ji.OrganizationProfileId
                     && org.CustomerId == actorId)
                     select ji)
                    .FirstOrDefault();

            if (jobInvitation != null)
            {
                jobInvitation.UpdateAudit(actorId);
                jobInvitation.IsRead = true;

                _JobInvitationRepository.Update(jobInvitation);
            }
        }

        #endregion
    }
}
