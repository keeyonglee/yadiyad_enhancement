using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.DTO.JobSeeker;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Base;
using YadiYad.Pro.Web.Infrastructure;

namespace YadiYad.Pro.Services.JobSeeker
{
    public class JobSeekerProfileService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepository;
        private readonly IRepository<JobSeekerCategory> _jobSeekerCategoryRepository;
        private readonly IRepository<JobSeekerAcademicQualification> _JobSeekerAcademicQualificationRepository;
        private readonly IRepository<JobSeekerLanguageProficiency> _JobSeekerLanguageProficiencyRepository;
        private readonly IRepository<JobSeekerLicenseCertificate> _JobSeekerLicenseCertificateRepository;
        private readonly IRepository<JobSeekerPreferredLocation> _JobSeekerPreferredLocationRepository;
        private readonly IRepository<JobInvitation> _JobInvitationRepository;
        private readonly IRepository<JobApplication> _jobApplicationRepository;
        private readonly IRepository<Download> _DownloadRepository;

        private readonly IRepository<Expertise> _expertiseRepository;
        private readonly IRepository<CommunicateLanguage> _LanguageRepository;
        private readonly IRepository<Country> _CountryRepository;
        private readonly IRepository<StateProvince> _StateProvinceRepository;
        private readonly IRepository<City> _CityRepository;
        private readonly IRepository<JobServiceCategory> _JobServiceCategory;
        private readonly IRepository<IndividualProfile> _individualRepository;
        private readonly IRepository<JobProfile> _jobProfileRepository;
        private readonly IRepository<JobSeekerCV> _jobSeekerCVRepository;

        #endregion

        #region Ctor

        public JobSeekerProfileService
            (IMapper mapper,
         IRepository<JobSeekerProfile> jobSeekerProfileRepository,
         IRepository<JobSeekerCategory> jobSeekerCategoryRepository,
         IRepository<JobSeekerAcademicQualification> jobSeekerAcademicQualificationRepository,
         IRepository<JobSeekerLanguageProficiency> jobSeekerLanguageProficiencyRepository,
         IRepository<JobSeekerLicenseCertificate> jobSeekerLicenseCertificateRepository,
         IRepository<JobSeekerPreferredLocation> jobSeekerPreferredLocationRepository,
         IRepository<JobServiceCategory> jobServiceCategory,
         IRepository<Expertise> expertiseRepository,
         IRepository<CommunicateLanguage> languageRepository,
         IRepository<Country> countryRepository,
         IRepository<StateProvince> stateProvinceRepository,
         IRepository<City> cityRepository,
         IRepository<Download> downloadRepository,
         IRepository<JobInvitation> jobInvitationRepository,
         IRepository<JobProfile> jobProfileRepository,
         IRepository<IndividualProfile> individualRepository,
         IRepository<JobApplication> jobApplicationRepository,
         IRepository<JobSeekerCV> jobSeekerCVRepository)
        {
            _mapper = mapper;
            _jobSeekerProfileRepository = jobSeekerProfileRepository;
            _jobSeekerCategoryRepository = jobSeekerCategoryRepository;
            _JobSeekerAcademicQualificationRepository = jobSeekerAcademicQualificationRepository;
            _JobSeekerLanguageProficiencyRepository = jobSeekerLanguageProficiencyRepository;
            _JobSeekerLicenseCertificateRepository = jobSeekerLicenseCertificateRepository;
            _JobSeekerPreferredLocationRepository = jobSeekerPreferredLocationRepository;
            _expertiseRepository = expertiseRepository;
            _LanguageRepository = languageRepository;
            _CountryRepository = countryRepository;
            _StateProvinceRepository = stateProvinceRepository;
            _CityRepository = cityRepository;
            _JobServiceCategory = jobServiceCategory;
            _DownloadRepository = downloadRepository;
            _JobInvitationRepository = jobInvitationRepository;
            _jobProfileRepository = jobProfileRepository;
            _individualRepository = individualRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobSeekerCVRepository = jobSeekerCVRepository;
        }

        #endregion


        #region Methods

        public virtual void VerifyJobSeekerProfile(JobSeekerProfileDTO dto)
        {
            if (dto.Categories != null)
            {
                if (dto.Categories.Count > 3)
                {
                    throw new InvalidOperationException("Maximum only 3 categories allowed.");
                }

                var duplicatedCategories = dto.Categories.GroupBy(x => new
                {
                    x.CategoryId
                })
                .Select(x => new
                {
                    x.Key.CategoryId,
                    SelectedCount = x.Count()
                })
                .Where(x => x.SelectedCount > 1)
                .ToList();

                if (duplicatedCategories.Count > 0)
                {
                    throw new InvalidOperationException("Selcted categories are not unique.");
                }
            }
        }

        public virtual void CreateJobSeekerProfile(int actorId, JobSeekerProfileDTO dto)
        {
            var request = _mapper.Map<JobSeekerProfile>(dto);

            CreateAudit(request, actorId);

            _jobSeekerProfileRepository.Insert(request);

            foreach (var model in request.Categories)
            {
                CreateAudit(model, actorId);
                model.JobSeekerProfileId = request.Id;
            }
            foreach (var model in request.AcademicQualifications)
            {
                CreateAudit(model, actorId);
                model.JobSeekerProfileId = request.Id;
            }
            foreach (var model in request.LanguageProficiencies)
            {
                CreateAudit(model, actorId);
                model.JobSeekerProfileId = request.Id;
            }
            foreach (var model in request.LicenseCertificates)
            {
                CreateAudit(model, actorId);
                model.JobSeekerProfileId = request.Id;
            }
            foreach (var model in request.PreferredLocations)
            {
                CreateAudit(model, actorId);
                model.JobSeekerProfileId = request.Id;
            }

            try
            {
                _jobSeekerCategoryRepository.Insert(request.Categories);
                _JobSeekerAcademicQualificationRepository.Insert(request.AcademicQualifications);
                _JobSeekerLanguageProficiencyRepository.Insert(request.LanguageProficiencies);
                _JobSeekerLicenseCertificateRepository.Insert(request.LicenseCertificates);
                _JobSeekerPreferredLocationRepository.Insert(request.PreferredLocations);
            }
            catch (Exception ex)
            {
                if (request.Id != 0)
                {
                    _jobSeekerProfileRepository.Delete(request);
                }
                throw;
            }
        }

        public JobSeekerProfileDTO GetJobSeekerProfileById(int id)
        {
            if (id == 0)
                return null;

            var query = _jobSeekerProfileRepository.Table;

            var record = query.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var response = _mapper.Map<JobSeekerProfileDTO>(record);

            var jobSeekerProfileDetails = GetJobSeekerProfileDetails(new List<int> { response.Id });

            var selectedJobSeekerProfileDetails = jobSeekerProfileDetails.Where(x => x.Id == response.Id).FirstOrDefault();
            response.Categories = selectedJobSeekerProfileDetails.Categories;
            response.AcademicQualifications = selectedJobSeekerProfileDetails.AcademicQualifications;
            response.LanguageProficiencies = selectedJobSeekerProfileDetails.LanguageProficiencies;
            response.LicenseCertificates = selectedJobSeekerProfileDetails.LicenseCertificates;
            response.PreferredLocations = selectedJobSeekerProfileDetails.PreferredLocations;

            return response;
        }

        public JobSeekerProfileDTO GetJobSeekerProfileByCustomerId(int id)
        {
            if (id == 0)
                return null;

            var query = _jobSeekerProfileRepository.Table;

            var record = query.Where(x => x.CustomerId == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                return null;

            var response = _mapper.Map<JobSeekerProfileDTO>(record);

            var jobSeekerProfileDetails = GetJobSeekerProfileDetails(new List<int> { response.Id });

            var selectedJobSeekerProfileDetails = jobSeekerProfileDetails.Where(x => x.Id == response.Id).FirstOrDefault();
            response.Categories = selectedJobSeekerProfileDetails.Categories;
            response.AcademicQualifications = selectedJobSeekerProfileDetails.AcademicQualifications;
            response.LanguageProficiencies = selectedJobSeekerProfileDetails.LanguageProficiencies;
            response.LicenseCertificates = selectedJobSeekerProfileDetails.LicenseCertificates;
            response.PreferredLocations = selectedJobSeekerProfileDetails.PreferredLocations;
            response.Cv = selectedJobSeekerProfileDetails.Cv;
            return response;
        }

        public virtual void UpdateJobSeekerProfile(int actorId, JobSeekerProfileDTO dto)
        {
            var updatedjobSeekerProfile = _mapper.Map<JobSeekerProfile>(dto);
            var oldJobSeekerProfile = _jobSeekerProfileRepository.Table
                .Where(x => x.Deleted == false)
                .First();
            

            UpdateAudit(oldJobSeekerProfile, updatedjobSeekerProfile, actorId);

            _jobSeekerProfileRepository.Update(updatedjobSeekerProfile);

            #region update Category

            var categories = _jobSeekerCategoryRepository.Table
                .Where(x => x.JobSeekerProfileId == updatedjobSeekerProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingCategories = categories
                .Where(x => updatedjobSeekerProfile.Categories
                    .Any(y => y.CategoryId == x.CategoryId
                    && y.YearExperience == x.YearExperience
                    && y.Expertises == x.Expertises) == false)
                .ToList();

            foreach (var deletingCategory in deletingCategories)
            {
                deletingCategory.Deleted = true;
                UpdateAudit(deletingCategory, deletingCategory, actorId);
            }

            _jobSeekerCategoryRepository.Update(deletingCategories);

            var creatingCategories = updatedjobSeekerProfile.Categories
                .Where(x => categories
                    .Any(y => y.CategoryId == x.CategoryId
                    && y.YearExperience == x.YearExperience
                    && y.Expertises == x.Expertises) == false)
                .ToList();

            foreach (var creatingCategory in creatingCategories)
            {
                CreateAudit(creatingCategory, actorId);
                creatingCategory.JobSeekerProfileId = updatedjobSeekerProfile.Id;
            }

            _jobSeekerCategoryRepository.Insert(creatingCategories);

            #endregion

            #region update Academic Qualifications

            var academicQualifications = _JobSeekerAcademicQualificationRepository.Table
                .Where(x => x.JobSeekerProfileId == updatedjobSeekerProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingAcademicQualifications = academicQualifications
                .Where(x => updatedjobSeekerProfile.AcademicQualifications
                    .Any(y => y.AcademicInstitution == x.AcademicInstitution
                    && y.AcademicQualificationName == x.AcademicQualificationName
                    && y.AcademicQualificationType == x.AcademicQualificationType) == false)
                .ToList();

            foreach (var deletingAcademicQualification in deletingAcademicQualifications)
            {
                deletingAcademicQualification.Deleted = true;
                UpdateAudit(deletingAcademicQualification, deletingAcademicQualification, actorId);
            }

            _JobSeekerAcademicQualificationRepository.Update(deletingAcademicQualifications);

            var creatingAcademicQualifications = updatedjobSeekerProfile.AcademicQualifications
                .Where(x => academicQualifications
                    .Any(y => y.AcademicInstitution == x.AcademicInstitution
                    && y.AcademicQualificationName == x.AcademicQualificationName
                    && y.AcademicQualificationType == x.AcademicQualificationType) == false)
                .ToList();

            foreach (var creatingAcademicQualification in creatingAcademicQualifications)
            {
                CreateAudit(creatingAcademicQualification, actorId);
                creatingAcademicQualification.JobSeekerProfileId = updatedjobSeekerProfile.Id;
            }

            _JobSeekerAcademicQualificationRepository.Insert(creatingAcademicQualifications);

            #endregion

            #region update license cert

            var licenseCertificate = _JobSeekerLicenseCertificateRepository.Table
                .Where(x => x.JobSeekerProfileId == updatedjobSeekerProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingServiceLicenses = licenseCertificate
                .Where(x => updatedjobSeekerProfile.LicenseCertificates
                    .Any(y => y.LicenseCertificateName == x.LicenseCertificateName
                    && y.ProfessionalAssociationName == x.ProfessionalAssociationName
                    && y.DownloadId == x.DownloadId) == false)
                .ToList();

            foreach (var deletingServiceLicense in deletingServiceLicenses)
            {
                deletingServiceLicense.Deleted = true;
                UpdateAudit(deletingServiceLicense, deletingServiceLicense, actorId);
            }

            _JobSeekerLicenseCertificateRepository.Update(deletingServiceLicenses);

            var creatingServiceLicenses = updatedjobSeekerProfile.LicenseCertificates
                .Where(x => licenseCertificate
                    .Any(y => y.LicenseCertificateName == x.LicenseCertificateName
                        && y.ProfessionalAssociationName == x.ProfessionalAssociationName
                        && y.DownloadId == x.DownloadId) == false)
                .ToList();

            foreach (var creatingServiceLicense in creatingServiceLicenses)
            {
                CreateAudit(creatingServiceLicense, actorId);
                creatingServiceLicense.JobSeekerProfileId = updatedjobSeekerProfile.Id;
            }

            _JobSeekerLicenseCertificateRepository.Insert(creatingServiceLicenses);

            #endregion

            #region update Language Proficiencies

            var languageProficiencies = _JobSeekerLanguageProficiencyRepository.Table
                .Where(x => x.JobSeekerProfileId == updatedjobSeekerProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingLanguageProficiencies = languageProficiencies
                .Where(x => updatedjobSeekerProfile.LanguageProficiencies
                    .Any(y => y.LanguageId == x.LanguageId
                    && y.ProficiencyLevel == x.ProficiencyLevel) == false)
                .ToList();

            foreach (var deletingCategory in deletingLanguageProficiencies)
            {
                deletingCategory.Deleted = true;
                UpdateAudit(deletingCategory, deletingCategory, actorId);
            }

            _JobSeekerLanguageProficiencyRepository.Update(deletingLanguageProficiencies);

            var creatingLanguageProficiencies = updatedjobSeekerProfile.LanguageProficiencies
                .Where(x => languageProficiencies
                    .Any(y => y.LanguageId == x.LanguageId
                    && y.ProficiencyLevel == x.ProficiencyLevel) == false)
                .ToList();

            foreach (var creatingLanguageProficiencie in creatingLanguageProficiencies)
            {
                CreateAudit(creatingLanguageProficiencie, actorId);
                creatingLanguageProficiencie.JobSeekerProfileId = updatedjobSeekerProfile.Id;
            }

            _JobSeekerLanguageProficiencyRepository.Insert(creatingLanguageProficiencies);

            #endregion

            #region update preferred Location

            var preferredLocation = _JobSeekerPreferredLocationRepository.Table
                .Where(x => x.JobSeekerProfileId == updatedjobSeekerProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingpreferredLocations = preferredLocation
                .Where(x => updatedjobSeekerProfile.PreferredLocations
                    .Any(y => y.CountryId == x.CountryId
                    && y.StateProvinceId == x.StateProvinceId
                    && y.CityId == x.CityId) == false)
                .ToList();

            foreach (var deletingCategory in deletingpreferredLocations)
            {
                deletingCategory.Deleted = true;
                UpdateAudit(deletingCategory, deletingCategory, actorId);
            }

            _JobSeekerPreferredLocationRepository.Update(deletingpreferredLocations);

            var creatingpreferredLocations = updatedjobSeekerProfile.PreferredLocations
                .Where(x => preferredLocation
                    .Any(y => y.CountryId == x.CountryId
                    && y.StateProvinceId == x.StateProvinceId
                    && y.CityId == x.CityId) == false)
                .ToList();

            foreach (var creatingLanguageProficiencie in creatingpreferredLocations)
            {
                CreateAudit(creatingLanguageProficiencie, actorId);
                creatingLanguageProficiencie.JobSeekerProfileId = updatedjobSeekerProfile.Id;
            }

            _JobSeekerPreferredLocationRepository.Insert(creatingpreferredLocations);

            #endregion

            #region update job CV

            if (updatedjobSeekerProfile.CV.DownloadId != 0)
            {
                var lastSavedCv = (from cv in _jobSeekerCVRepository.Table
                    where cv.JobSeekerProfileId == updatedjobSeekerProfile.Id && !cv.Deleted
                    orderby cv.Id descending
                    select cv).FirstOrDefault();

                if (lastSavedCv != null)
                {
                    lastSavedCv.Deleted = true;
                    UpdateAudit(lastSavedCv, lastSavedCv, actorId);
                    _jobSeekerCVRepository.Update(lastSavedCv);
                }

                var createNewCv = updatedjobSeekerProfile.CV;
                createNewCv.JobSeekerProfileId = updatedjobSeekerProfile.Id;
                CreateAudit(createNewCv, actorId);
                _jobSeekerCVRepository.Insert(createNewCv);
            }
            
            #endregion
            
        }

        public bool HasJobSeekerProfile(int customerId)
        {
            var hasJobSeekerProfile = _jobSeekerProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.CustomerId == customerId)
                .Any();

            return hasJobSeekerProfile;
        }

        public List<JobSeekerProfileDTO> GetJobSeekerProfileDetails(List<int> jobSeekerProfileIds)
        {
            var categories =
                        (from jsc in _jobSeekerCategoryRepository.Table
                        .Where(jsc => !jsc.Deleted && jobSeekerProfileIds.Contains(jsc.JobSeekerProfileId))
                         from c in _JobServiceCategory.Table
                         .Where(c => c.Id == jsc.CategoryId)
                         select new JobSeekerCategoryDTO
                         {
                             Id = jsc.Id,
                             JobSeekerProfileId = jsc.JobSeekerProfileId,
                             CategoryId = jsc.CategoryId,
                             CategoryName = c.Name,
                             Expertises = jsc.Expertises,
                             ExpertiseIds = AutoMappingProfile.ToIntList('|', jsc.Expertises),
                             YearExperience = jsc.YearExperience,
                             YearExperienceName =
                                 jsc.YearExperience == (int)ExperienceYear.YearLessThan10
                                 ? ExperienceYear.YearLessThan10.GetDescription()
                                 : jsc.YearExperience == (int)ExperienceYear.Year11To20
                                 ? ExperienceYear.Year11To20.GetDescription()
                                 : jsc.YearExperience == (int)ExperienceYear.Year21To30
                                 ? ExperienceYear.Year21To30.GetDescription()
                                 : null
                         })
                     .ToList();

            var expertiseIds = categories.SelectMany(x => x.ExpertiseIds).Distinct().ToList();
            var expertises = _expertiseRepository.Table
                .Where(x => expertiseIds.Contains(x.Id))
                .Select(x => _mapper.Map<ExpertiseDTO>(x))
                .ToList();
            categories = categories
                .Select(x =>
                {
                    x.CategoryExpertises = expertises
                        .Where(y => x.ExpertiseIds.Contains(y.Id))
                        .Select(z => new ExpertiseDTO
                        {
                            Id = z.Id,
                            Name = z.Name
                        })
                        .ToList();
                    return x;
                })
                .ToList();

            var academicQualifications = _JobSeekerAcademicQualificationRepository.Table.Where(x => jobSeekerProfileIds.Contains(x.JobSeekerProfileId) && !x.Deleted)
                .Select(x => _mapper.Map<JobSeekerAcademicQualificationDTO>(x))
                .ToList()
                .Select(x =>
                {
                    x.AcademicQualificationTypeName = ((AcademicQualificationType)x.AcademicQualificationType).GetDescription();
                    return x;
                })
                .ToList();
            var languageProficiencies = _JobSeekerLanguageProficiencyRepository.Table
                .Where(x => !x.Deleted && jobSeekerProfileIds.Contains(x.JobSeekerProfileId))
                .Join(_LanguageRepository.Table,
                x => x.LanguageId,
                y => y.Id,
                (x, y) => new JobSeekerLanguageProficiencyDTO
                {
                    JobSeekerProfileId = x.JobSeekerProfileId,
                    Id = x.Id,
                    ProficiencyLevel = x.ProficiencyLevel,
                    LanguageId = x.LanguageId,
                    LanguageName = y.Name,
                    ProficiencyLevelName = ((LanguageSpokenWrittenProficiency)x.ProficiencyLevel).GetDescription(),
                    ProficiencyWrittenLevel = x.ProficiencyWrittenLevel,
                    ProficiencyWrittenLevelName = ((LanguageSpokenWrittenProficiency)x.ProficiencyWrittenLevel).GetDescription(),

                })
                .ToList();

            var licenseCertificates = (from sl in _JobSeekerLicenseCertificateRepository.Table
                                       where sl.Deleted == false
                                       && jobSeekerProfileIds.Contains(sl.JobSeekerProfileId)
                                       join dl in _DownloadRepository.Table on sl.DownloadId equals dl.Id into sd
                                       from dl in sd.DefaultIfEmpty()
                                       select new JobSeekerLicenseCertificateDTO
                                       {
                                           Id = sl.Id,
                                           JobSeekerProfileId = sl.JobSeekerProfileId,
                                           ProfessionalAssociationName = sl.ProfessionalAssociationName,
                                           LicenseCertificateName = sl.LicenseCertificateName,
                                           DownloadId = sl.DownloadId,
                                           DownloadName = dl != null ? (dl.Filename + dl.Extension) : null,
                                           DownloadGuid = dl != null ? dl.DownloadGuid : (Guid?)null
                                       }).ToList();

            var cV = (from cv in _jobSeekerCVRepository.Table
                join dl in _DownloadRepository.Table on cv.DownloadId equals dl.Id into cvdl
                from dl in cvdl.DefaultIfEmpty()
                where !cv.Deleted && jobSeekerProfileIds.Contains(cv.JobSeekerProfileId)
                orderby cv.Id descending
                select new JobSeekerCVDTO
                {
                    Id = cv.Id,
                    JobSeekerProfileId = cv.JobSeekerProfileId,
                    DownloadId = cv.DownloadId,
                    DownloadName = dl != null ? (dl.Filename + dl.Extension) : null,
                    DownloadGuid = dl != null ? dl.DownloadGuid : (Guid?) null
                }).ToList();

            var preferredLocations = 
                (from pl in _JobSeekerPreferredLocationRepository.Table
                .Where(pl => pl.Deleted == false
                && jobSeekerProfileIds.Contains(pl.JobSeekerProfileId))

                 from c in _CountryRepository.Table
                 .Where(c => c.Id == pl.CountryId)

                 from sp in _StateProvinceRepository.Table
                 .Where(sp => sp.Id == pl.StateProvinceId
                 && sp.CountryId == pl.CountryId)
                 .DefaultIfEmpty()

                 from ct in _CityRepository.Table
                 .Where(ct => ct.Id == pl.CityId
                 && ct.StateProvinceId == pl.StateProvinceId
                 && ct.CountryId == pl.CountryId)
                 .DefaultIfEmpty()

                 select new JobSeekerPreferredLocationDTO
                 {
                     JobSeekerProfileId = pl.JobSeekerProfileId,
                     CityId = pl.CityId,
                     CityName = ct.Name,
                     StateProvinceId = pl.StateProvinceId,
                     StateProvinceName = sp.Name,
                     CountryId = pl.CountryId,
                     CountryName = c.Name,
                 })
                .ToList();

            return jobSeekerProfileIds
                .Select(x => new JobSeekerProfileDTO
                {
                    Id = x,
                    Categories = categories.Where(y => y.JobSeekerProfileId == x).ToList(),
                    AcademicQualifications = academicQualifications.Where(y => y.JobSeekerProfileId == x).ToList(),
                    LanguageProficiencies = languageProficiencies.Where(y => y.JobSeekerProfileId == x).ToList(),
                    LicenseCertificates = licenseCertificates.Where(y => y.JobSeekerProfileId == x).ToList(),
                    PreferredLocations = preferredLocations.Where(y => y.JobSeekerProfileId == x).ToList(),
                    Cv = cV.Where(y => y.JobSeekerProfileId == x).OrderByDescending((y => y.Id)).FirstOrDefault()
                })
                .ToList();
        }

        public PagedListDTO<JobSeekerProfileDTO> SearchJobCandidates(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            JobCandidateSearchFilterDTO filterDTO = null)
        {
            var jobProfile = _jobProfileRepository.Table
                .Where(x => x.Id == filterDTO.JobProfileId)
                .Select(x => new
                {
                    x.JobType,
                    x.CategoryId,
                    x.Id,
                    x.PreferredExperience
                })
                .FirstOrDefault();

            if (jobProfile == null)
            {
                throw new KeyNotFoundException("Job profile not found.");
            }

            var query = from x in
                         (from j in _jobSeekerProfileRepository.Table.Join(
                             _individualRepository.Table,
                             j => j.CustomerId,
                             i => i.CustomerId,
                             (j, i) => new
                             {
                                 j,
                                 i
                             }
                             )
                          .Join(_StateProvinceRepository.Table, x => x.i.StateProvinceId, s => s.Id, (x, s) => new { x.j, x.i, s })
                          .Join(_CountryRepository.Table, x => x.i.CountryId, c => c.Id, (x, c) => new { x.j, x.i, x.s, c })
                          .Join(_CountryRepository.Table, x => x.i.NationalityId, c => c.Id, (x, n) => new { x.j, x.i, x.s, x.c, n })
                          .Where(x => x.i.Deleted == false
                          && x.j.Deleted == false)
                         .Select(x => new { x.j, x.i, x.s, x.c, x.n })
                          select new { j.j, j.i, j.s, j.c, j.n })
                        join y in
                   (from jpe in _jobSeekerCategoryRepository.Table
                   .Where(jpe => !jpe.Deleted
                    && ((jobProfile != null && jobProfile.CategoryId != 0 && jobProfile.CategoryId == jpe.CategoryId) || jobProfile == null)
                   )
                    from jpec in _expertiseRepository.Table
                   .Where(jpec => jpe.Expertises.Contains("|" + jpec.Id + "|"))
                    select new { jpe, jpec })
                   on x.j.Id equals y.jpe.JobSeekerProfileId into xy
                        from y in xy
                        select new { x, y } into g
                        group g by new { g.x } into r
                        select new JobSeekerProfileDTO
                        {
                            Id = r.Key.x.j.Id,
                            CustomerId = r.Key.x.j.CustomerId,
                            DOB = r.Key.x.i.DateOfBirth,
                            Age = DateTime.UtcNow.Year - r.Key.x.i.DateOfBirth.Year,
                            Name = r.Key.x.i.FullName,
                            NickName = r.Key.x.i.NickName,
                            GenderName = ((Gender)r.Key.x.i.Gender).GetDescription(),
                            Email = r.Key.x.i.Email,
                            ContactNo = r.Key.x.i.ContactNo,
                            NationalityName = r.Key.x.n.Name,
                            Address = r.Key.x.i.Address,
                            StateProvinceName = r.Key.x.s.Name,
                            CountryName = r.Key.x.c.Name,
                            EmploymentStatus = r.Key.x.j.EmploymentStatus,
                            EmploymentStatusName = ((EmploymentStatus)r.Key.x.j.EmploymentStatus).GetDescription(),
                            Company = r.Key.x.j.Company,
                            Position = r.Key.x.j.Position,
                            TenureStart = r.Key.x.j.TenureStart,
                            TenureEnd = r.Key.x.j.TenureEnd,
                            AchievementAward = r.Key.x.j.AchievementAward,
                            IsFreelanceHourly = r.Key.x.j.IsFreelanceHourly,
                            IsFreelanceDaily = r.Key.x.j.IsFreelanceDaily,
                            IsProjectBased = r.Key.x.j.IsProjectBased,
                            IsOnSite = r.Key.x.j.IsOnSite,
                            IsPartialOnSite = r.Key.x.j.IsPartialOnSite,
                            IsRemote = r.Key.x.j.IsRemote,
                            HourlyPayAmount = r.Key.x.j.HourlyPayAmount,
                            DailyPayAmount = r.Key.x.j.DailyPayAmount,
                            PreferredExperience = jobProfile.PreferredExperience
                        };
            if (jobProfile != null)
            {
                //// service model
                //query = query.Where(x => x.IsOnSite == jobProfile.IsOnsite
                //&& x.IsPartialOnSite == jobProfile.IsPartialOnsite
                //&& x.IsRemote == jobProfile.IsRemote
                //);
                // service type
                if (jobProfile.JobType != 0)
                {
                    if (jobProfile.JobType == (int)JobType.Freelancing || jobProfile.JobType == (int)JobType.PartTime)
                    {
                        query = query.Where(x => x.IsFreelanceHourly || x.IsFreelanceDaily);
                    }
                    //if (jobProfile.JobType == (int)JobType.Freelancing)
                    //{
                    //    query = query.Where(x => x.IsFreelanceHourly);
                    //}
                    //if (jobProfile.JobType == (int)JobType.PartTime)
                    //{
                    //    query = query.Where(x => x.IsFreelanceDaily);
                    //}
                    if (jobProfile.JobType == (int)JobType.ProjectBased)
                    {
                        query = query.Where(x => x.IsProjectBased);
                    }
                }

                var invitedJobSeekerProfileIds = _JobInvitationRepository.Table
                .Where(x => !x.Deleted
                    && x.JobProfileId == jobProfile.Id
                )
                .Select(x => x.JobSeekerProfileId)
                .ToList();


                var appliedJobSeekerProfileIds = _jobApplicationRepository.Table
                .Where(x => !x.Deleted
                    && x.JobProfileId == jobProfile.Id
                )
                .Select(x => x.JobSeekerProfileId)
                .ToList();
                query = query.Where(x => !invitedJobSeekerProfileIds.Contains(x.Id) && !appliedJobSeekerProfileIds.Contains(x.Id));
            }

            var totalCount = query.Count();
            var records = query.ToList();

            var response = new PagedListDTO<JobSeekerProfileDTO>(records, pageIndex, pageSize, totalCount);

            if (response.Data.Count() > 0)
            {
                var jobSeekerProfileIds = response.Data.Select(x => x.Id).ToList();
                var jobSeekerProfileDetails = GetJobSeekerProfileDetails(jobSeekerProfileIds);

                response.Data = response.Data
                    .Select(x =>
                    {
                        var selectedJobSeekerProfileDetails = jobSeekerProfileDetails.Where(y => y.Id == x.Id).FirstOrDefault();
                        x.Categories = selectedJobSeekerProfileDetails.Categories.Where(y => y.CategoryId == jobProfile.CategoryId).ToList();
                        x.AcademicQualifications = selectedJobSeekerProfileDetails.AcademicQualifications;
                        x.LicenseCertificates = selectedJobSeekerProfileDetails.LicenseCertificates;
                        x.LanguageProficiencies = selectedJobSeekerProfileDetails.LanguageProficiencies;
                        x.PreferredLocations = selectedJobSeekerProfileDetails.PreferredLocations;
                        x.Cv = selectedJobSeekerProfileDetails.Cv;

                        DateTime zeroTime = new DateTime(1, 1, 1);
                        TimeSpan span = DateTime.Now.Date - x.DOB;
                        int age = (zeroTime + span).Year - 1;
                        x.Age = age;
                        x.DOB = new DateTime(1990, 1, 1);

                        return x;
                    })
                    .ToList();


            }

            if (filterDTO.ShowFullProfile == false
                && response.Data != null)
            {
                foreach (var record in response.Data)
                {
                    MaskPersonlInfo(record);
                }
            }

            response.AdditionalData = new
            {
                ShowFullProfile = filterDTO.ShowFullProfile
            };

            return response;
        }

        public void MaskPersonlInfo(JobSeekerProfileDTO profileDTO)
        {
            profileDTO.Name = ("00000" + profileDTO.Id);
            profileDTO.Name = "YP" + profileDTO.Name.Substring(profileDTO.Name.Length - 5);
            profileDTO.NickName = profileDTO.Name;
            profileDTO.CustomerId = 0;
            profileDTO.Email = null;
            profileDTO.ContactNo = null;
            profileDTO.NationalityName = null;
            profileDTO.Address = null;
            profileDTO.StateProvinceName = null;
            profileDTO.CountryName = null;
            profileDTO.EmploymentStatus = 0;
            profileDTO.EmploymentStatusName = null;
            profileDTO.Company = null;
            profileDTO.Position = null;
            profileDTO.TenureStart = null;
            profileDTO.TenureEnd = null;
            profileDTO.AchievementAward = null;
        }

        #endregion
    }
}
