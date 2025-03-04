using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Core.Caching;
using Nop.Services.Caching;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Core.Domain.Subscription;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.JobSeeker;
using YadiYad.Pro.Services.Services.Base;
using YadiYad.Pro.Core.Infrastructure.Cache;
using DocumentFormat.OpenXml.InkML;
using YadiYad.Pro.Core.Domain.Deposit;
using DocumentFormat.OpenXml.Bibliography;
using Nop.Core.Domain.Payments;

namespace YadiYad.Pro.Services.Job
{
    public class JobProfileService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<JobProfile> _jobProfileRepository;
        private readonly IRepository<JobProfileExpertise> _jobProfileExpertiseRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<JobServiceCategory> _jobServiceCategoryRepository;
        private readonly IRepository<Expertise> _expertiseRepository;
        private readonly IRepository<OrganizationProfile> _organizationProfileRepository;
        private readonly IRepository<ServiceProfile> _serviceProfileRepository;
        private readonly IRepository<ServiceExpertise> _serviceExpertiseRepository;
        private readonly IRepository<ServiceLicenseCertificate> _serviceLicenseCertificateRepository;
        private readonly IRepository<ServiceAcademicQualification> _serviceAcademicQualificationRepository;
        private readonly IRepository<ServiceLanguageProficiency> _serviceLanguageProficiencyRepository;
        private readonly IRepository<CommunicateLanguage> _communicateLanguageRepository;
        private readonly IRepository<IndividualProfile> _individualeRepository;
        private readonly IRepository<JobApplication> _JobApplicationRepository;
        private readonly IRepository<JobInvitation> _JobInvitationRepository;
        private readonly IRepository<ServiceSubscription> _serviceSubscriptionRepository;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepository;
        private readonly IRepository<ConsultationProfile> _consultationProfileRepository;
        private readonly IRepository<JobMilestone> _jobMilestoneRepository;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepository;
        private readonly IRepository<JobSeekerCategory> _jobSeekerCategoryRepository;
        private readonly IRepository<JobSeekerAcademicQualification> _jobSeekerAcademicQualificationRepository;
        private readonly IRepository<JobSeekerLanguageProficiency> _jobSeekerLanguageProficiencyRepository;
        private readonly IRepository<JobSeekerLicenseCertificate> _jobSeekerLicenseCertificateRepository;
        private readonly IRepository<JobSeekerPreferredLocation> _jobSeekerPreferredLocationRepository;
        private readonly IRepository<PayoutRequest> _payoutRequestRepository;
        private readonly IRepository<DepositRequest> _depositRequestRepository;
        private readonly JobSeekerProfileService _jobSeekerProfileService;
        //private readonly ICacheKeyService _cacheKeyService;
        //private readonly IStaticCacheManager _staticCacheManager;

        //private static string subKeyTotalJobInvited = "TotalJobInvited";
        //private static string subKeyTotalJobApplied = "TotalJobApplied";
        //private static string subKeyTotalJobInvitation = "TotalJobInvitation";
        //private static string subKeyTotalJobApplication = "TotalJobApplication";
        //private static string subKeyTotalJobHired = "TotalJobHired";

        #endregion

        #region Ctor

        public JobProfileService
            (IMapper mapper,
            IRepository<JobProfile> JobProfileRepository,
            IRepository<JobProfileExpertise> JobProfileExpertiseRepository,
            IRepository<City> CityRepository,
            IRepository<StateProvince> StateProvinceRepository,
            IRepository<Country> CountryRepository,
            IRepository<JobServiceCategory> JobServiceCategoryRepository,
            IRepository<Expertise> ExpertiseRepository,
            IRepository<OrganizationProfile> organizationProfileRepository,
            IRepository<ServiceProfile> serviceProfileRepository,
            IRepository<ServiceExpertise> serviceExpertiseRepository,
            IRepository<ServiceLicenseCertificate> serviceLicenseCertificate,
            IRepository<ServiceAcademicQualification> serviceAcademicQualification,
            IRepository<ServiceLanguageProficiency> serviceLanguageProficiency,
            IRepository<CommunicateLanguage> communicateLanguageeRepository,
            IRepository<IndividualProfile> individualeRepository,
            IRepository<JobInvitation> JobInvitationRepository,
            IRepository<JobApplication> JobApplicationRepository,
            IRepository<ServiceSubscription> ServiceSubscriptionRepository,
            IRepository<ConsultationInvitation> consultationInvitationRepository,
            IRepository<ConsultationProfile> consultationProfileRepository,
            IRepository<JobMilestone> jobMilestone,
            IRepository<JobSeekerProfile> jobSeekerProfileRepository,
            IRepository<JobSeekerCategory> jobSeekerCategoryRepository,
            IRepository<JobSeekerAcademicQualification> jobSeekerAcademicQualificationRepository,
            IRepository<JobSeekerLanguageProficiency> jobSeekerLanguageProficiencyRepository,
            IRepository<JobSeekerLicenseCertificate> jobSeekerLicenseCertificateRepository,
            IRepository<JobSeekerPreferredLocation> jobSeekerPreferredLocationRepository,
            IRepository<PayoutRequest> payoutRequestRepository,
            IRepository<DepositRequest> depositRequestRepository,
            JobSeekerProfileService jobSeekerProfileService)
        {
            _mapper = mapper;
            _jobProfileRepository = JobProfileRepository;
            _jobProfileExpertiseRepository = JobProfileExpertiseRepository;
            _cityRepository = CityRepository;
            _stateProvinceRepository = StateProvinceRepository;
            _countryRepository = CountryRepository;
            _jobServiceCategoryRepository = JobServiceCategoryRepository;
            _expertiseRepository = ExpertiseRepository;
            _organizationProfileRepository = organizationProfileRepository;

            _serviceProfileRepository = serviceProfileRepository;
            _serviceExpertiseRepository = serviceExpertiseRepository;
            _serviceLicenseCertificateRepository = serviceLicenseCertificate;
            _serviceAcademicQualificationRepository = serviceAcademicQualification;
            _serviceLanguageProficiencyRepository = serviceLanguageProficiency;

            _communicateLanguageRepository = communicateLanguageeRepository;
            _individualeRepository = individualeRepository;
            _JobApplicationRepository = JobApplicationRepository;
            _JobInvitationRepository = JobInvitationRepository;

            _serviceSubscriptionRepository = ServiceSubscriptionRepository;
            _consultationInvitationRepository = consultationInvitationRepository;
            _consultationProfileRepository = consultationProfileRepository;

            _jobMilestoneRepository = jobMilestone;
            _jobSeekerProfileRepository = jobSeekerProfileRepository;
            _jobSeekerCategoryRepository = jobSeekerCategoryRepository;
            _jobSeekerAcademicQualificationRepository = jobSeekerAcademicQualificationRepository;
            _jobSeekerLanguageProficiencyRepository = jobSeekerLanguageProficiencyRepository;
            _jobSeekerLicenseCertificateRepository = jobSeekerLicenseCertificateRepository;
            _jobSeekerPreferredLocationRepository = jobSeekerPreferredLocationRepository;
            _payoutRequestRepository = payoutRequestRepository;
            _depositRequestRepository = depositRequestRepository;

            _jobSeekerProfileService = jobSeekerProfileService;
        }

        #endregion


        #region Methods

        public virtual void CreateJobProfile(int actorId, JobProfileDTO dto)
        {
            try
            {
                var request = _mapper.Map<JobProfile>(dto);

                request.Status =
                    dto.Status == (int)JobProfileStatus.Publish
                    ? (int)JobProfileStatus.Publish
                    : (int)JobProfileStatus.Draft;

                _jobProfileRepository.Insert(request);

                foreach (var milestone in request.JobMilestones)
                {
                    milestone.JobProfileId = request.Id;
                    CreateAudit(milestone, actorId);
                }
                _jobMilestoneRepository.Insert(request.JobMilestones);

                var jobProfileExpertiseRequest = dto.RequiredExpertises
                    .Select(x => new JobProfileExpertise
                    {
                        CustomerId = request.CustomerId,
                        JobProfieId = request.Id,
                        ExpertiseId = x.Id,
                        CreatedById = request.CustomerId,
                        CreatedOnUTC = DateTime.UtcNow
                    }).ToList();
                _jobProfileExpertiseRepository.Insert(jobProfileExpertiseRequest);

                dto.Id = request.Id;
            }
            catch
            {
                throw;
            }
        }

        public virtual void PublishJobProfile(int actorId, int jobProfileId)
        {
            var model = _jobProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Status == (int)JobProfileStatus.Draft
                && x.Id == jobProfileId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException($"Job profile not found.");
            }
            model.Status = (int)JobProfileStatus.Publish;
            model.UpdateAudit(actorId);

            _jobProfileRepository.Update(model);
        }

        public virtual void UpdateJobProfileToHiredStatus(int actorId, int jobProfileId)
        {
            var model = _jobProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Status == (int)JobProfileStatus.Publish
                && x.Id == jobProfileId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException($"Job profile not found.");
            }
            model.Status = (int)JobProfileStatus.Hired;
            model.UpdateAudit(actorId);

            _jobProfileRepository.Update(model);
        }

        public virtual void UpdateJobProfile(int actorId, JobProfileDTO dto)
        {
            var model = _jobProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.Status == (int)JobProfileStatus.Draft
                && x.Id == dto.Id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException($"Job profile not found.");
            }

            var request = _mapper.Map<JobProfile>(dto);

            request.Status =
                dto.Status == (int)JobProfileStatus.Publish
                ? (int)JobProfileStatus.Publish
                : (int)JobProfileStatus.Draft;

            _jobProfileRepository.Update(request);

            var jobProfileExpertiseRequest = dto.RequiredExpertises
                .Select(x => new JobProfileExpertise
                {
                    CustomerId = request.CustomerId,
                    JobProfieId = request.Id,
                    ExpertiseId = x.Id,
                    CreatedById = request.CustomerId,
                    CreatedOnUTC = DateTime.UtcNow
                }).ToList();
            var jobProfileExpertiseRecord = _jobProfileExpertiseRepository.Table.Where(x => x.JobProfieId == request.Id).ToList();
            _jobProfileExpertiseRepository.Delete(jobProfileExpertiseRecord);
            _jobProfileExpertiseRepository.Insert(jobProfileExpertiseRequest);

            #region update job milestone

            var jobMilestones = _jobMilestoneRepository.Table
                .Where(x => x.JobProfileId == request.Id
                && x.Deleted == false)
                .ToList();

            var deletingJobMilestones = jobMilestones
                .Where(x => request.JobMilestones
                    .Any(y => y.Sequence == x.Sequence
                    && y.Description == x.Description
                    && y.Amount == x.Amount) == false)
                .ToList();

            foreach (var deletingJobMilestone in deletingJobMilestones)
            {
                deletingJobMilestone.Deleted = true;
                UpdateAudit(deletingJobMilestone, deletingJobMilestone, actorId);
            }

            _jobMilestoneRepository.Update(deletingJobMilestones);

            var creatingJobMilestones = request.JobMilestones
                .Where(x => jobMilestones
                    .Any(y => y.Sequence == x.Sequence
                    && y.Description == x.Description
                    && y.Amount == x.Amount) == false)
                .ToList();

            foreach (var creatingJobMilestone in creatingJobMilestones)
            {
                creatingJobMilestone.JobProfileId = request.Id;
                CreateAudit(creatingJobMilestone, actorId);
            }

            _jobMilestoneRepository.Insert(creatingJobMilestones);

            #endregion
        }

        public DateTime? GetJobProfileViewJobCandidateFulleProfileEndDate(int jobProfileId)
        {
            var utcNow = DateTime.UtcNow;

            var endDate = _serviceSubscriptionRepository.Table.Where(x =>
                        !x.Deleted
                        && x.RefId == jobProfileId
                        && x.EndDate >= utcNow
                        && x.StartDate <= utcNow
                        && x.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile)
                        .Select(x => (DateTime?)(x.StopDate != null ? x.StopDate.Value : x.EndDate))
                        .OrderByDescending(x => x)
                        .FirstOrDefault<DateTime?>();

            return endDate;
        }

        public JobProfileDTO GetJobProfileById(int id)
        {
            if (id == 0)
                return null;

            var query = _jobProfileRepository.Table;

            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var categoryRecord = _jobServiceCategoryRepository.Table.Where(x => x.Id == record.CategoryId).FirstOrDefault();
            var experienceRecord = ((ExperienceYear)record.PreferredExperience).GetDescription();
            var expertiseQuery = from b in _expertiseRepository.Table
                                 join u in _jobProfileExpertiseRepository.Table on b.Id equals u.ExpertiseId
                                 where u.JobProfieId == record.Id
                                 select b;
            var expertiseRecord = expertiseQuery.Select(x => new ExpertiseDTO { Id = x.Id, Name = x.Name }).ToList();

            var response = _mapper.Map<JobProfileDTO>(record);

            response.CategoryName = categoryRecord.Name;
            response.PreferredExperienceName = experienceRecord;
            response.RequiredExpertises = expertiseRecord;

            if (record.CountryId != null)
            {
                var countryRecord = _countryRepository.Table.Where(x => x.Id == record.CountryId).FirstOrDefault();
                response.CountryName = countryRecord.Name;

            }

            if (record.StateProvinceId != null)
            {
                var stateProvinceRecord = _stateProvinceRepository.Table.Where(x => x.Id == record.StateProvinceId).FirstOrDefault();
                response.StateProvinceName = stateProvinceRecord.Name;

            }

            if (record.CityId != null)
            {
                var cityRecord = _cityRepository.Table.Where(x => x.Id == record.CityId).FirstOrDefault();
                response.CityName = cityRecord.Name;

            }

            var viewProfileEntitledEndDateTime = GetJobProfileViewJobCandidateFulleProfileEndDate(response.Id);

            response.ViewJobCandidateFullProfileSubscriptionEndDate = viewProfileEntitledEndDateTime;
            if (viewProfileEntitledEndDateTime.HasValue && viewProfileEntitledEndDateTime.Value > DateTime.UtcNow)
            {
                response.ViewJobCandidateFullProfileSubscriptionEndDays = (int)viewProfileEntitledEndDateTime.Value.Subtract(DateTime.UtcNow).TotalDays;
            }

            var milestones = _jobMilestoneRepository.Table
                .Where(x => x.Deleted == false
                && x.JobProfileId == response.Id)
                .Select(x => new JobMilestoneDTO
                {
                    Id = x.Id,
                    Sequence = x.Sequence,
                    Description = x.Description,
                    Amount = x.Amount
                })
                .OrderBy(x => x.Sequence)
                .ToList();

            response.JobMilestones = milestones;

            return response;
        }

        public JobProfileDTO GetJobProfileInfoById(int actorId, int id)
        {
            var jobProfile =
                (from jp in _jobProfileRepository.Table
                .Where(jp => jp.Deleted == false
                && jp.Id == id
                && jp.CustomerId == actorId)
                 from c in _cityRepository.Table
                .Where(c => c.Id == jp.CityId).DefaultIfEmpty()
                 from s in _stateProvinceRepository.Table
                .Where(s => s.Id == jp.StateProvinceId).DefaultIfEmpty()
                 from co in _countryRepository.Table
                .Where(co => co.Id == jp.CountryId).DefaultIfEmpty()
                 from ca in _jobServiceCategoryRepository.Table
                .Where(ca => ca.Id == jp.CategoryId)
                 from ss in _serviceSubscriptionRepository.Table
                .Where(ss => ss.Deleted == false
                && ss.RefId == jp.Id
                && ss.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile)
                .DefaultIfEmpty()
                 select new
                 {
                     jp,
                     c,
                     s,
                     co,
                     ca,
                     ss
                 })
                .GroupBy(x => new
                {
                    x.jp,
                    x.c,
                    x.s,
                    x.co,
                    x.ca
                })
               .Select(x => new
               {
                   x.Key.jp,
                   x.Key.c,
                   x.Key.s,
                   x.Key.co,
                   x.Key.ca,
                   ViewJobCandidateFullProfileSubscriptionEndDate = x.Max(y => (DateTime?)(y.ss.StopDate != null ? y.ss.StopDate.Value : y.ss.EndDate))
               })
               .FirstOrDefault();


            if (jobProfile == null)
            {
                throw new KeyNotFoundException();
            }

            var jobProfileDTO = _mapper.Map<JobProfileDTO>(jobProfile.jp);

            jobProfileDTO.ViewJobCandidateFullProfileSubscriptionEndDate = jobProfile.ViewJobCandidateFullProfileSubscriptionEndDate;
            jobProfileDTO.CityId = jobProfile.c?.Id;
            jobProfileDTO.CityName = jobProfile.c?.Name;
            jobProfileDTO.StateProvinceId = jobProfile.s?.Id;
            jobProfileDTO.StateProvinceName = jobProfile.s?.Name;
            jobProfileDTO.CountryId = jobProfile.co?.Id;
            jobProfileDTO.CountryName = jobProfile.co?.Name;
            jobProfileDTO.CategoryId = jobProfile.ca.Id;
            jobProfileDTO.CategoryName = jobProfile.ca.Name;

            if (jobProfileDTO.ViewJobCandidateFullProfileSubscriptionEndDate.HasValue
                && jobProfileDTO.ViewJobCandidateFullProfileSubscriptionEndDate.Value > DateTime.UtcNow)
            {
                jobProfileDTO.ViewJobCandidateFullProfileSubscriptionEndDays
                    = (int)jobProfileDTO.ViewJobCandidateFullProfileSubscriptionEndDate.Value.Date
                    .Subtract(DateTime.Today).TotalDays;
            }

            return jobProfileDTO;
        }

        public JobProfile GetJobProfileDomainById(int id)
        {
            if (id == 0)
                return null;

            var query = _jobProfileRepository.Table;

            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            return record;
        }

        //public List<JobProfileDTO> GetJobProfilesByCustomerId(int customerId)
        //{
        //    if (customerId == 0)
        //        return null;

        //    var record = from x in
        //                     (from j in _jobProfileRepository.Table
        //                     .Where(j => j.CustomerId == customerId && !j.Deleted)
        //                     .OrderByDescending(j => j.CreatedOnUTC)
        //                      from c in _cityRepository.Table
        //                     .Where(c => c.Id == j.CityId)
        //                      from s in _stateProvinceRepository.Table
        //                     .Where(s => s.Id == j.StateProvinceId)
        //                      from co in _countryRepository.Table
        //                     .Where(co => co.Id == j.CountryId)
        //                      from ca in _jobServiceCategoryRepository.Table
        //                     .Where(ca => ca.Id == j.CategoryId)
        //                      select new { j, c, s, co, ca })
        //                 join y in
        //                    (from jpe in _jobProfileExpertiseRepository.Table
        //                    .Where(jpe => !jpe.Deleted)
        //                     from jpec in _jobServiceCategoryRepository.Table
        //                    .Where(jpec => jpec.Id == jpe.ExpertiseId)
        //                     select new { jpe, jpec })
        //                 on x.c.Id equals y.jpe.JobProfieId
        //                 select new { x, y } into g
        //                 group g by new { g.x } into r
        //                 select new JobProfileDTO
        //                 {
        //                     Id = r.Key.x.j.Id,
        //                     CustomerId = r.Key.x.j.CustomerId,
        //                     CategoryId = r.Key.x.j.CategoryId,
        //                     CategoryName = r.Key.x.ca.Name
        //                 };

        //    var jobProfileIds = record.Select(x => x.Id).ToList();
        //    var milestones = _jobMilestoneRepository.Table
        //            .Where(x => x.Deleted == false && jobProfileIds.Contains(x.JobProfileId))
        //            .ToList();

        //    foreach (var rec in record)
        //    {
        //        rec.JobMilestones = milestones.Where(x => x.JobProfileId == rec.Id)
        //            .OrderBy(x => x.Sequence)
        //            .Select(x => new JobMilestoneDTO
        //            {
        //                Id = x.Id,
        //                Sequence = x.Sequence,
        //                Description = x.Description,
        //                Amount = x.Amount
        //            }).ToList();
        //    }

        //    var response = record.ToList();
        //    return response;
        //}

        public IPagedList<JobProfileDTO> SearchJobProfiles(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            JobSearchFilterDTO filterDTO = null)
        {
            var formattedKeyword = Regex.Replace(keyword ?? "", @"(\s+|@|'|""|\\|/|\(|\)|<|>|\+|-|\*|\?)", " ")
                .Trim()
                .Split(' ')
                .ToList()
                .Where(x => x != string.Empty)
                .Select(x => $"{x}*")
                .ToList();
            //prepare input parameters
            var pKeyword = SqlParameterHelper.GetStringParameter("Keyword", string.Join(' ', formattedKeyword));
            var pCategoryIds = SqlParameterHelper.GetStringParameter("CategoryIds", String.Join(",", filterDTO.CategoryIds));
            var pServiceTypeId = SqlParameterHelper.GetInt32Parameter("ServiceTypeId", filterDTO.JobTypeId);
            var pServiceModelId = SqlParameterHelper.GetInt32Parameter("ServiceModelId", filterDTO.JobModelId);
            var pStateProvinanceId = SqlParameterHelper.GetInt32Parameter("StateProvinanceId", filterDTO.StateProvinceId);
            var pCustomerId = SqlParameterHelper.GetInt32Parameter("CustomerId", filterDTO.CustomerId);
            var pJobSeekerProfileId = SqlParameterHelper.GetInt32Parameter("JobSeekerProfileId", filterDTO.JobSeekerProfileId);

            var pIsFreelanceHourly = SqlParameterHelper.GetBooleanParameter("IsFreelanceHourly", filterDTO.IsFreelanceHourly);
            var pIsFreelanceDaily = SqlParameterHelper.GetBooleanParameter("IsFreelanceDaily", filterDTO.IsFreelanceDaily);
            var pIsProjectBased = SqlParameterHelper.GetBooleanParameter("IsProjectBased", filterDTO.IsProjectBased);
            var pIsOnSite = SqlParameterHelper.GetBooleanParameter("IsOnSite", filterDTO.IsOnSite);
            var pIsPartialOnSite = SqlParameterHelper.GetBooleanParameter("IsPartialOnSite", filterDTO.IsPartialOnSite);
            var pIsRemote = SqlParameterHelper.GetBooleanParameter("IsRemote", filterDTO.IsRemote);

            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);
            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var jobProiflesQuery = _jobProfileRepository.EntityFromSql("usp_SearchJobProfile",
                pKeyword,
                pCategoryIds,
                pServiceTypeId,
                pServiceModelId,
                pStateProvinanceId,
                pCustomerId,
                pJobSeekerProfileId,
                pIsFreelanceHourly,
                pIsFreelanceDaily,
                pIsProjectBased,
                pIsOnSite,
                pIsPartialOnSite,
                pIsRemote,
                pPageIndex,
                pPageSize,
                pTotalRecords
                );

            var jobProifles = jobProiflesQuery.ToList();

            var jobProifleDTOs = jobProifles
                .Select(x => _mapper.Map<JobProfileDTO>(x))
                .OrderByDescending(x => x.CreatedOnUTC)
                .ToList()
                .Select(x =>
                {
                    x.PreferredExperienceName =
                       x.PreferredExperience == (int)ExperienceYear.YearLessThan10
                       ? ExperienceYear.YearLessThan10.GetDescription()
                       : x.PreferredExperience == (int)ExperienceYear.Year11To20
                       ? ExperienceYear.Year11To20.GetDescription()
                       : x.PreferredExperience == (int)ExperienceYear.Year21To30
                       ? ExperienceYear.Year21To30.GetDescription()
                       : null;
                    return x;
                });

            var jobProfileIds = jobProifleDTOs.Select(x => x.Id).ToList();

            var jobProfileSubscriptions =
            _serviceSubscriptionRepository.Table
                .Where(x => x.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile
                && jobProfileIds.Contains(x.RefId))
                .ToList();

            if (jobProifleDTOs.Count() > 0)
            {
                var jobServiceCategoryIds = jobProifleDTOs.Select(x => x.CategoryId).ToList();
                var jobServiceCategories = _jobServiceCategoryRepository.Table.Where(x => jobServiceCategoryIds.Contains(x.Id)).ToList();

                var jobProfileExpertises = _jobProfileExpertiseRepository.Table.Where(x => jobProfileIds.Contains(x.JobProfieId)).ToList();
                var expertiseIds = jobProfileExpertises.Select(x => x.ExpertiseId).ToList();
                var expertises = _expertiseRepository.Table.Where(x => expertiseIds.Contains(x.Id))
                    .Select(x => new ExpertiseDTO
                    {
                        Id = x.Id,
                        Name = x.Name
                    })
                    .ToList();

                var countryIds = jobProifleDTOs.Where(x => x.CountryId != null).Select(x => x.CountryId).ToList();
                var countries = _countryRepository.Table
                    .Where(x => countryIds.Contains(x.Id))
                    .ToList();
                var stateIds = jobProifleDTOs.Where(x => x.StateProvinceId != null).Select(x => x.StateProvinceId).ToList();
                var states = _stateProvinceRepository.Table
                    .Where(x => stateIds.Contains(x.Id))
                    .ToList();
                var cityIds = jobProifleDTOs.Where(x => x.CityId != null).Select(x => x.CityId).ToList();
                var cities = _cityRepository.Table
                    .Where(x => cityIds.Contains(x.Id))
                    .ToList();

                var milestones = _jobMilestoneRepository.Table.Where(x => x.Deleted == false && jobProfileIds.Contains(x.JobProfileId)).ToList();

                var orgCustIds = jobProifleDTOs.Select(x => x.CustomerId);

                var orgs = _organizationProfileRepository.Table
                    .Where(x => x.Deleted == false
                    && orgCustIds.Contains(x.CustomerId))
                    .ToList();

                jobProifleDTOs = jobProifleDTOs.Select(x =>
                {
                    x.CategoryName = jobServiceCategories.Where(y => y.Id == x.CategoryId).Select(y => y.Name).FirstOrDefault();
                    x.RequiredExpertises = expertises.Where(y => jobProfileExpertises.Where(z => z.JobProfieId == x.Id).Select(z => z.ExpertiseId).ToList().Contains(y.Id)).ToList();
                    x.CountryName = x.CountryId != null ? countries.Where(y => y.Id == x.CountryId).Select(y => y.Name).FirstOrDefault() : null;
                    x.StateProvinceName = x.StateProvinceId != null ? states.Where(y => y.Id == x.StateProvinceId).Select(y => y.Name).FirstOrDefault() : null;
                    x.CityName = x.CityId != null ? cities.Where(y => y.Id == x.CityId).Select(y => y.Name).FirstOrDefault() : null;
                    x.JobMilestones = milestones.Where(y => y.JobProfileId == x.Id)
                            .OrderBy(y => y.Sequence)
                            .Select(x => new JobMilestoneDTO
                            {
                                Id = x.Id,
                                Sequence = x.Sequence,
                                Description = x.Description,
                                Amount = x.Amount
                            }).ToList();
                    x.ViewJobCandidateFullProfileSubscriptionEndDate = jobProfileSubscriptions
                        .Where(y => y.RefId == x.Id)
                        .OrderByDescending(y => y.EndDate)
                        .Select(y => (DateTime?)(y.StopDate != null ? y.StopDate.Value : y.EndDate))
                        .FirstOrDefault();
                    x.OrganizationName = orgs.Where(y => y.CustomerId == x.CustomerId).First().Name;

                    return x;
                }).ToList();
            }

            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var response = new PagedList<JobProfileDTO>(jobProifleDTOs, pageIndex, pageSize, totalRecords);

            return response;
        }

        public IPagedList<JobProfileSummaryDTO> SearchJobProfilesSummary(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            JobSearchFilterDTO filterDTO = null)
        {
            var record = from j in _jobProfileRepository.Table
                         .Where(x => x.Deleted == false
                         && x.Status == (int)JobProfileStatus.Publish
                         && _JobApplicationRepository.Table
                            .Any(y => y.Deleted == false
                            && y.JobProfileId == x.Id
                            && y.JobApplicationStatus == (int)JobApplicationStatus.Hired) == false
                           )
                         orderby j.CreatedOnUTC descending
                         select new JobProfileSummaryDTO
                         {
                             Id = j.Id,
                             CustomerId = j.CustomerId,
                             JobTitle = j.JobTitle,
                             IsImmediate = j.IsImmediate,
                             StartDate = j.StartDate
                         };

            if (filterDTO != null)
            {
                if (filterDTO.CustomerId != 0)
                {
                    record = record.Where(x => x.CustomerId == filterDTO.CustomerId);
                }
            }

            var response = new PagedList<JobProfileSummaryDTO>(record, pageIndex, pageSize);

            var profileIds = response.Select(x => x.Id).ToList();
            var currentJobApplications = _JobApplicationRepository.Table
                 .Where(j => !j.Deleted && profileIds.Contains(j.JobProfileId))
                 .ToList();

            var viewProfileEntitledEndDateTimes = _serviceSubscriptionRepository.Table.Where(x =>
                        !x.Deleted
                        && profileIds.Contains(x.RefId)
                        && x.SubscriptionTypeId == (int)SubscriptionType.ViewJobCandidateFulleProfile)
                        .GroupBy(x => new
                        {
                            x.RefId,
                            x.SubscriptionTypeId
                        })
                        .Select(x => new ServiceSubscription
                        {
                            RefId = x.Key.RefId,
                            SubscriptionTypeId = x.Key.SubscriptionTypeId,
                            StartDate = x.Max(y => y.StartDate),
                            EndDate = x.Max(y => y.StopDate != null ? y.StopDate.Value : y.EndDate)
                        })
                        .ToList();

            foreach (var job in response)
            {
                job.ViewJobCandidateFullProfileSubscriptionEndDate = viewProfileEntitledEndDateTimes
                    .Where(x => x.RefId == job.Id)
                    .OrderByDescending(x => x.EndDate)
                    .Select(x => (DateTime?)x.EndDate)
                    .FirstOrDefault<DateTime?>();

                job.ViewJobCandidateFullProfileSubscriptionEndDays = job.ViewJobCandidateFullProfileSubscriptionEndDate != null ? job.ViewJobCandidateFullProfileSubscriptionEndDate.Value.Subtract(DateTime.UtcNow).TotalDays : 0;
                job.UnderConsiderationCount = currentJobApplications.Where(x => x.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration && x.JobProfileId == job.Id).Count();
                job.HiredCount = currentJobApplications.Where(x => x.JobApplicationStatus == (int)JobApplicationStatus.Hired && x.JobProfileId == job.Id).Count();
                job.ShortlistedCount = currentJobApplications.Where(x => x.JobApplicationStatus == (int)JobApplicationStatus.Shortlisted && x.JobProfileId == job.Id).Count();
                job.IsAttentionRequired = currentJobApplications
                    .Any(x =>
                        x.JobApplicationStatus == (int)JobApplicationStatus.UnderConsideration
                        && x.JobProfileId == job.Id 
                        && x.IsOrganizationRead == false);
            }

            return response;
        }

        public bool CheckValidForSearchCandidate(int jobProfileId)
        {
            var job = _jobProfileRepository.Table.Where(x => x.Id == jobProfileId && x.Deleted == false)
                .Select(jp => new
                {
                    jp.Status
                })
                .FirstOrDefault();

            if (job == null)
            {
                throw new KeyNotFoundException("Job advs not found.");
            }

            if (job.Status != (int)JobProfileStatus.Publish)
            {
                return false;
            }

            var endDate = GetJobProfileViewJobCandidateFulleProfileEndDate(jobProfileId);

            if (endDate == null || DateTime.UtcNow > endDate.Value)
            {
                return false;
            }

            return true;
        }

        public OrganizationItemCounterDTO GetOrganizationItemCounterMain(int customerId)
        {
            var dto = new OrganizationItemCounterDTO();

            var noJobHiredQuery =
                (from ja in _JobApplicationRepository.Table
                .Where(ja => ja.Deleted == false
                && (ja.JobApplicationStatus == (int)JobApplicationStatus.Hired
                || ja.JobApplicationStatus == (int)JobApplicationStatus.Completed
                || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByIndividual
                || ja.JobApplicationStatus == (int)JobApplicationStatus.CancelledByOrganization))

                 from og in _organizationProfileRepository.Table
                 .Where(og => og.Deleted == false
                 && og.Id == ja.OrganizationProfileId
                 && og.CustomerId == customerId)

                 from jp in _jobProfileRepository.Table
                 .Where(jp => og.Deleted == false
                 && jp.Id == ja.JobProfileId
                 && jp.CustomerId == customerId)

                 from jsp in _jobSeekerProfileRepository.Table
                 .Where(jsp => jsp.Deleted == false
                 && jsp.Id == ja.JobSeekerProfileId)
                 select ja.Id);

            //var keyjobHired = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.OrganizationJobCounter, customerId, subKeyTotalJobHired);

            //dto.NoJobHired = _staticCacheManager.Get(keyjobHired, () => noJobHiredQuery.Count());

            dto.NoJobHired = noJobHiredQuery.Count();

            return dto;
        }

        public OrganizationItemCounterDTO GetOrganizationItemCounter(int customerId, int jobId)
        {
            var dto = new OrganizationItemCounterDTO();

            var qJobInvitation =
                (from ji in _JobInvitationRepository.Table
                .Where(ji => ji.Deleted == false
                && (ji.JobInvitationStatus == (int)JobInvitationStatus.Pending
                || ji.JobInvitationStatus == (int)JobInvitationStatus.Declined)
                && ji.JobProfileId == jobId)

                 from og in _organizationProfileRepository.Table
                 .Where(og => og.Deleted == false
                 && og.Id == ji.OrganizationProfileId
                 && og.CustomerId == customerId)

                 from jp in _jobProfileRepository.Table
                 .Where(jp => og.Deleted == false
                 && jp.Id == ji.JobProfileId
                 && jp.CustomerId == customerId)

                 from jsp in _jobSeekerProfileRepository.Table
                 .Where(jsp => jsp.Deleted == false
                 && jsp.Id == ji.JobSeekerProfileId)

                 select ji.Id);

            //var keyjobInvitation = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.OrganizationJobCounter, customerId, subKeyTotalJobInvitation);

            //dto.NoJobInvitation = _staticCacheManager.Get(keyjobInvitation, () => qJobInvitation.Count());

            dto.NoJobInvitation = qJobInvitation.Count();

            var qJobApplicant =
                (from ja in _JobApplicationRepository.Table
                .Where(ja => ja.Deleted == false
                && ja.JobProfileId == jobId)

                 from og in _organizationProfileRepository.Table
                 .Where(og => og.Deleted == false
                 && og.Id == ja.OrganizationProfileId
                 && og.CustomerId == customerId)

                 from jp in _jobProfileRepository.Table
                 .Where(jp => og.Deleted == false
                 && jp.Id == ja.JobProfileId
                 && jp.CustomerId == customerId)

                 from jsp in _jobSeekerProfileRepository.Table
                 .Where(jsp => jsp.Deleted == false
                 && jsp.Id == ja.JobSeekerProfileId)
                 select ja.Id);

            //var keyjobApplicant = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.OrganizationJobCounter, customerId, subKeyTotalJobApplication);

            //dto.NoJobApplicant = _staticCacheManager.Get(keyjobApplicant, () => qJobApplicant.Count());

            dto.NoJobApplicant = qJobApplicant.Count();

            return dto;
        }


        public JobSeekerItemCounterDTO GetJobSeekerItemCounter(int customerId)
        {
            var dto = new JobSeekerItemCounterDTO();
            //dto.NoJobInvited =
            //    (from ji in _JobInvitationRepository.Table
            //    .Where(ji => ji.Deleted == false
            //    && ji.JobInvitationStatus == (int)JobInvitationStatus.Pending)
            //     from sp in _jobSeekerProfileRepository.Table
            //     .Where(sp => sp.Deleted == false
            //     && sp.Id == ji.JobSeekerProfileId
            //     && sp.CustomerId == customerId)
            //     select ji.Id)
            //    .Count()
            //    +
            //    (from ci in _consultationInvitationRepository.Table
            //     .Where(ci => ci.Deleted == false
            //     && ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New)
            //     from cp in _consultationProfileRepository.Table
            //     .Where(cp => cp.Deleted == false
            //     && cp.Id == ci.ConsultationProfileId
            //     && cp.IsApproved == true)
            //     from sp in _serviceProfileRepository.Table
            //     .Where(sp => sp.Deleted == false
            //     && sp.Id == ci.ServiceProfileId
            //     && sp.CustomerId == customerId)
            //     select ci.Id)
            //     .Count();

            var qJobInvited = (from ji in _JobInvitationRepository.Table
                                    .Where(ji => ji.Deleted == false
                                    && (ji.JobInvitationStatus == (int)JobInvitationStatus.Pending
                                        || ji.JobInvitationStatus == (int)JobInvitationStatus.Declined
                                        || ji.JobInvitationStatus == (int)JobInvitationStatus.Expired
                                        || ji.JobInvitationStatus == (int)JobInvitationStatus.Reviewing
                                        || ji.JobInvitationStatus == (int)JobInvitationStatus.UpdateRequired))
                               from sp in _jobSeekerProfileRepository.Table
                                   .Where(sp => sp.Deleted == false
                                   && sp.Id == ji.JobSeekerProfileId
                                   && sp.CustomerId == customerId)
                               select ji.Id);
            var qConsultationInvited = (from ci in _consultationInvitationRepository.Table
                                            .Where(ci => ci.Deleted == false
                                            && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New
                                               || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual
                                               || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByOrganization
                                               || (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                                                && ci.IsApproved != true)))
                                        from cp in _consultationProfileRepository.Table
                                            .Where(cp => cp.Deleted == false
                                            && cp.Id == ci.ConsultationProfileId
                                            && cp.IsApproved == true)
                                        from sp in _serviceProfileRepository.Table
                                            .Where(sp => sp.Deleted == false
                                            && sp.Id == ci.ServiceProfileId
                                            && sp.CustomerId == customerId)
                                        select ci.Id);

            dto.NoJobInvited = qJobInvited.Count() + qConsultationInvited.Count();

            //dto.NoJobApplied =
            //    (from ja in _JobApplicationRepository.Table
            //    .Where(ja => ja.Deleted == false
            //    && ja.IsRead == false
            //    && (ja.JobApplicationStatus == (int)JobApplicationStatus.Shortlisted
            //    || ja.JobApplicationStatus == (int)JobApplicationStatus.KeepForFutureReference
            //    || ja.JobApplicationStatus == (int)JobApplicationStatus.Hired
            //    || ja.JobApplicationStatus == (int)JobApplicationStatus.NotHired
            //    || ja.JobApplicationStatus == (int)JobApplicationStatus.Rejected))
            //     from sp in _jobSeekerProfileRepository.Table
            //     .Where(sp => sp.Deleted == false
            //     && sp.Id == ja.JobSeekerProfileId
            //     && sp.CustomerId == customerId)
            //     select ja.Id)
            //    .Count()
            //    +
            //    (from ci in _consultationInvitationRepository.Table
            //     .Where(ci => ci.Deleted == false
            //     && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
            //     || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted))
            //     from cp in _consultationProfileRepository.Table
            //     .Where(cp => cp.Deleted == false
            //     && cp.Id == ci.ConsultationProfileId
            //     && cp.IsApproved == true)
            //     from sp in _serviceProfileRepository.Table
            //     .Where(sp => sp.Deleted == false
            //     && sp.Id == ci.ServiceProfileId
            //     && sp.CustomerId == customerId)
            //     select ci.Id)
            //     .Count();

            var qJobApplied = (from ja in _JobApplicationRepository.Table
                                    .Where(ja => ja.Deleted == false)
                                from sp in _jobSeekerProfileRepository.Table
                                    .Where(jsp => jsp.Deleted == false
                                    && jsp.Id == ja.JobSeekerProfileId
                                    && jsp.CustomerId == customerId)
                                 select ja.Id);
            var qConsultationApplied = (from ci in _consultationInvitationRepository.Table
                                             .Where(ci => ci.Deleted == false
                                             && (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                                             || (ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                                                && ci.IsApproved == true)
                                             || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                                             || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization
                                             || ci.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed))
                                        from cp in _consultationProfileRepository.Table
                                            .Where(cp => cp.Deleted == false
                                            && cp.Id == ci.ConsultationProfileId
                                            && cp.IsApproved == true)
                                        from sp in _serviceProfileRepository.Table
                                            .Where(sp => sp.Deleted == false
                                            && sp.Id == ci.ServiceProfileId
                                            && sp.CustomerId == customerId)
                                        select ci.Id);

            //var keyTotalJobApplied = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.IndividualJobCounter, customerId, subKeyTotalJobApplied);

            //dto.NoJobApplied = _staticCacheManager.Get(keyTotalJobApplied, () => qJobApplied.Count() + qConsultationApplied.Count());

            dto.NoJobApplied = qJobApplied.Count() + qConsultationApplied.Count();

            return dto;
        }

        /// <summary>
        /// Send an invitation to an individual for a job profile.
        /// Requires PVI (Pay-to-View-and-Invite) before sending.
        /// </summary>
        public virtual void SendInvitation(int actorId, int jobProfileId, int individualId, int productId)
        {
            var jobProfile = _jobProfileRepository.GetById(jobProfileId);
            if (jobProfile == null || jobProfile.Deleted)
                throw new KeyNotFoundException($"Job profile not found.");

            // Ensure payment is recorded before sending an invite (PVI)
            var depositRequest = new DepositRequest
            {
                DepositFrom = actorId, 
                DepositTo = individualId, 
                Amount = 0, 
                ProductTypeId = productId, // Using ProductTypeId for PaymentType
                RefId = jobProfileId, // Reference to JobProfile
                Status = (int)PaymentStatus.Paid,
                RequestDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow, // Assuming immediate processing
                BaseDepositNumber = new Random().Next(100000, 999999), // Example Deposit Number
                DepositNumber = $"PVI-{Guid.NewGuid().ToString().Substring(0, 8)}",
                PaymentChannelId = 1 
            };

            _depositRequestRepository.Insert(depositRequest);

            var invitation = new JobInvitation
            {
                JobProfileId = jobProfileId,
                JobSeekerProfileId = individualId,
                JobInvitationStatus = 1, // Pending
                CreatedOnUTC = DateTime.UtcNow
            };

            _JobInvitationRepository.Insert(invitation);
        }

        /// <summary>
        /// Pay-to-View-and-Invite (PVI) process before sending invitations.
        /// </summary>
        public virtual bool PayToViewAndInvite(int actorId, int jobProfileId, int individualId, int productId)
        {
            var depositRequest = new DepositRequest
            {
                DepositFrom = actorId,
                DepositTo = individualId,
                Amount = 0, // If applicable, set PVI fee
                ProductTypeId = productId,
                RefId = jobProfileId,
                Status = (int)PaymentStatus.Paid,
                RequestDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow,
                BaseDepositNumber = new Random().Next(100000, 999999),
                DepositNumber = $"PVI-{Guid.NewGuid().ToString().Substring(0, 8)}",
                PaymentChannelId = 1
            };

            _depositRequestRepository.Insert(depositRequest);
            return true; // Assuming successful insert means payment is completed.
        }

        /// <summary>
        /// Hire an individual and process the 1st month professional fee payment.
        /// </summary>
        public virtual void HireIndividual(int actorId, int jobProfileId, int individualId, decimal amount, int productId)
        {
            var jobProfile = _jobProfileRepository.GetById(jobProfileId);
            if (jobProfile == null || jobProfile.Deleted)
                throw new KeyNotFoundException($"Job profile not found.");

            var invitation = _JobInvitationRepository.Table
                .FirstOrDefault(x => x.JobProfileId == jobProfileId && x.JobSeekerProfileId == individualId && x.JobInvitationStatus == 1);

            if (invitation == null)
                throw new KeyNotFoundException("No pending invitation found for this individual.");

            // Insert record for 1st month professional fee payment
            var professionalFeePayment = new DepositRequest
            {
                DepositFrom = actorId,
                DepositTo = individualId,
                Amount = amount, // Example fee, replace with actual fee logic
                ProductTypeId = productId,
                RefId = jobProfileId,
                Status = (int)PaymentStatus.Paid,
                RequestDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow,
                BaseDepositNumber = new Random().Next(100000, 999999),
                DepositNumber = $"FMP-{Guid.NewGuid().ToString().Substring(0, 8)}",
                PaymentChannelId = 1
            };

            _depositRequestRepository.Insert(professionalFeePayment);

            // Mark the individual as hired
            invitation.JobInvitationStatus = 2; // Hired
            _JobInvitationRepository.Update(invitation);

            // Update job profile status to "Hired"
            jobProfile.Status = (int)JobProfileStatus.Hired;
            jobProfile.UpdateAudit(actorId);
            _jobProfileRepository.Update(jobProfile);
        }



        #endregion
    }
}
