using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Service
{
    public class ServiceProfileService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ServiceProfile> _serviceProfileRepository;
        private readonly IRepository<ServiceExpertise> _serviceExpertiseRepository;
        private readonly IRepository<ServiceLicenseCertificate> _serviceLicenseCertificateRepository;
        private readonly IRepository<ServiceAcademicQualification> _serviceAcademicQualificationRepository;
        private readonly IRepository<ServiceLanguageProficiency> _serviceLanguageProficiencyRepository;
        private readonly IRepository<CommunicateLanguage> _communicateLanguageRepository;
        private readonly IRepository<Expertise> _expertiseRepository;
        private readonly IRepository<JobServiceCategory> _jobServiceCategoryRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<IndividualProfile> _individualeRepository;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepository;
        private readonly IRepository<Download> _downloadRepository;

        #endregion

        #region Ctor

        public ServiceProfileService
            (IMapper mapper,
            IRepository<ServiceProfile> serviceProfileRepository,
            IRepository<IndividualProfile> individualeRepository,
            IRepository<ServiceExpertise> serviceExpertiseRepository,
            IRepository<ServiceLicenseCertificate> serviceLicenseCertificate,
            IRepository<ServiceAcademicQualification> serviceAcademicQualification,
            IRepository<ServiceLanguageProficiency> serviceLanguageProficiency,
            IRepository<CommunicateLanguage> communicateLanguageRepository,
            IRepository<Expertise> expertiseRepository,
            IRepository<JobServiceCategory> jobServiceCategoryRepository,
            IRepository<City> cityRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<ConsultationInvitation> consultationInvitationRepository,
            IRepository<Download> downloadRepository)
        {
            _mapper = mapper;
            _serviceProfileRepository = serviceProfileRepository;
            _individualeRepository = individualeRepository;
            _serviceExpertiseRepository = serviceExpertiseRepository;
            _serviceLicenseCertificateRepository = serviceLicenseCertificate;
            _serviceAcademicQualificationRepository = serviceAcademicQualification;
            _serviceLanguageProficiencyRepository = serviceLanguageProficiency;
            _communicateLanguageRepository = communicateLanguageRepository;
            _expertiseRepository = expertiseRepository;
            _jobServiceCategoryRepository = jobServiceCategoryRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _consultationInvitationRepository = consultationInvitationRepository;
            _downloadRepository = downloadRepository;
        }

        #endregion

        #region Methods

        public ServiceProfileDTO CreateServiceProfile(int actorId, int customerId, ServiceProfileDTO dto)
        {
            var serviceProfile = dto.ToModel(_mapper, customerId);

            CreateAudit(serviceProfile, actorId);

            _serviceProfileRepository.Insert(serviceProfile);

            foreach (var expertise in serviceProfile.ServiceExpertises)
            {
                expertise.ServiceProfileId = serviceProfile.Id;

                CreateAudit(expertise, actorId);
            }

            foreach (var licenseCert in serviceProfile.ServiceLicenseCertificates)
            {
                licenseCert.ServiceProfileId = serviceProfile.Id;

                CreateAudit(licenseCert, actorId);
            }

            foreach (var academicQ in serviceProfile.ServiceAcademicQualifications)
            {
                academicQ.ServiceProfileId = serviceProfile.Id;

                CreateAudit(academicQ, actorId);
            }

            foreach (var langProf in serviceProfile.ServiceLanguageProficiencies)
            {
                langProf.ServiceProfileId = serviceProfile.Id;

                CreateAudit(langProf, actorId);
            }

            _serviceExpertiseRepository.Insert(serviceProfile.ServiceExpertises);
            _serviceLicenseCertificateRepository.Insert(serviceProfile.ServiceLicenseCertificates);
            _serviceAcademicQualificationRepository.Insert(serviceProfile.ServiceAcademicQualifications);
            _serviceLanguageProficiencyRepository.Insert(serviceProfile.ServiceLanguageProficiencies);
            dto.Id = serviceProfile.Id;

            return dto;
        }

        public ServiceProfileDTO UpdateServiceProfile(int actorId, int customerId, ServiceProfileDTO dto)
        {
            #region update service profile

            var serviceProfile = _serviceProfileRepository.Table
                .Where(x => x.Id == dto.Id
                && x.CustomerId == customerId)
                .FirstOrDefault();

            var updatedingServiceProfile = dto.ToModel(_mapper, customerId);
            
            UpdateAudit(serviceProfile, updatedingServiceProfile, actorId);

            _serviceProfileRepository.Update(updatedingServiceProfile);

            #endregion

            #region update service license cert

            var serviceLicenses = _serviceLicenseCertificateRepository.Table
                .Where(x => x.ServiceProfileId == serviceProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingServiceLicenses = serviceLicenses
                .Where(x => updatedingServiceProfile.ServiceLicenseCertificates
                    .Any(y => y.LicenseCertificateName == x.LicenseCertificateName
                    && y.ProfessionalAssociationName == x.ProfessionalAssociationName
                    && y.DownloadId == x.DownloadId) == false)
                .ToList();

            foreach (var deletingServiceLicense in deletingServiceLicenses)
            {
                deletingServiceLicense.Deleted = true;
                UpdateAudit(deletingServiceLicense, deletingServiceLicense, actorId);
            }

            _serviceLicenseCertificateRepository.Update(deletingServiceLicenses);

            var creatingServiceLicenses = updatedingServiceProfile.ServiceLicenseCertificates
                .Where(x => serviceLicenses
                    .Any(y => y.LicenseCertificateName == x.LicenseCertificateName
                        && y.ProfessionalAssociationName == x.ProfessionalAssociationName
                        && y.DownloadId == x.DownloadId) == false)
                .ToList();

            foreach (var creatingServiceLicense in creatingServiceLicenses)
            {
                CreateAudit(creatingServiceLicense, actorId);
                creatingServiceLicense.ServiceProfileId = serviceProfile.Id;
            }

            _serviceLicenseCertificateRepository.Insert(creatingServiceLicenses);

            #endregion

            #region update service education

            var serviceEducations = _serviceAcademicQualificationRepository.Table
                .Where(x => x.ServiceProfileId == serviceProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingServiceEducations = serviceEducations
                .Where(x => updatedingServiceProfile.ServiceAcademicQualifications
                    .Any(y => y.AcademicInstitution == x.AcademicInstitution
                    && y.AcademicQualificationName == x.AcademicQualificationName
                    && y.AcademicQualificationType == x.AcademicQualificationType) == false)
                .ToList();

            foreach (var deletingServiceEducation in deletingServiceEducations)
            {
                deletingServiceEducation.Deleted = true;
                UpdateAudit(deletingServiceEducation, deletingServiceEducation, actorId);
            }

            _serviceAcademicQualificationRepository.Update(deletingServiceEducations);

            var creatingServiceEducations = updatedingServiceProfile.ServiceAcademicQualifications
                .Where(x => serviceEducations
                    .Any(y => y.AcademicInstitution == x.AcademicInstitution
                        && y.AcademicQualificationName == x.AcademicQualificationName
                        && y.AcademicQualificationType == x.AcademicQualificationType) == false)
                .ToList();

            foreach (var creatingServiceEducation in creatingServiceEducations)
            {
                CreateAudit(creatingServiceEducation, actorId);
                creatingServiceEducation.ServiceProfileId = serviceProfile.Id;
            }

            _serviceAcademicQualificationRepository.Insert(creatingServiceEducations);

            #endregion

            #region update service language

            var serviceLanguages = _serviceLanguageProficiencyRepository.Table
                .Where(x => x.ServiceProfileId == serviceProfile.Id
                && x.Deleted == false)
                .ToList();

            var deletingServiceLanguages = serviceLanguages
                .Where(x => updatedingServiceProfile.ServiceLanguageProficiencies
                    .Any(y => y.LanguageId == x.LanguageId
                    && y.ProficiencyLevel == x.ProficiencyLevel) == false)
                .ToList();

            foreach (var deletingServiceLanguage in deletingServiceLanguages)
            {
                deletingServiceLanguage.Deleted = true;
                UpdateAudit(deletingServiceLanguage, deletingServiceLanguage, actorId);
            }

            _serviceLanguageProficiencyRepository.Update(deletingServiceLanguages);

            var creatingServiceLanguages = updatedingServiceProfile.ServiceLanguageProficiencies
                .Where(x => serviceLanguages
                    .Any(y => y.LanguageId == x.LanguageId
                        && y.ProficiencyLevel == x.ProficiencyLevel) == false)
                .ToList();

            foreach (var creatingServiceLanguage in creatingServiceLanguages)
            {
                CreateAudit(creatingServiceLanguage, actorId);
                creatingServiceLanguage.ServiceProfileId = serviceProfile.Id;
            }

            _serviceLanguageProficiencyRepository.Insert(creatingServiceLanguages);

            #endregion

            return dto;
        }

        public void DeleteServiceProfile(int actorId, int customerId, int serviceProfileId)
        {
            var serviceProfile = _serviceProfileRepository.Table
                .Where(x => x.Deleted == false
                && x.DeletedFromUser == false
                && (customerId == 0
                || x.CustomerId == customerId))
                .FirstOrDefault();

            if(serviceProfile == null)
            {
                throw new KeyNotFoundException("Service profile not found.");
            }

            serviceProfile.DeletedFromUser = true;

            _serviceProfileRepository.Update(serviceProfile);
        }

        public ServiceProfileDTO GetServiceProfileById(int id)
        {
            if (id == 0)
                return null;

            var query = _serviceProfileRepository.Table;

            var dto = query.Where(x => x.Id == id && x.Deleted == false)
                .Select(x => new ServiceProfileDTO
                {
                    Id = x.Id,
                    CustomerId = x.CustomerId,
                    YearExperience = x.YearExperience,
                    ServiceTypeId = x.ServiceTypeId,
                    ServiceModelId = x.ServiceModelId,
                    EmploymentStatus = x.EmploymentStatus,
                    Company = x.Company,
                    Position = x.Position,
                    TenureStart = x.TenureStart,
                    TenureEnd = x.TenureEnd,
                    AchievementAward = x.AchievementAward,
                    ProjectBasedCharges = x.ServiceTypeId == (int)ServiceType.ProjectBased ? (decimal?)x.ServiceFee : null,
                    ConsultationCharges = x.ServiceTypeId == (int)ServiceType.Consultation ? (decimal?)x.ServiceFee : null,
                    FreelancingCharges = x.ServiceTypeId == (int)ServiceType.Freelancing ? (decimal?)x.ServiceFee : null,
                    PartTimeCharges = x.ServiceTypeId == (int)ServiceType.PartTime ? (decimal?)x.ServiceFee : null,
                    OnsiteCharges = x.OnsiteFee,
                    FreelancingAvailability = x.ServiceTypeId == (int)ServiceType.Freelancing ? (int?)x.Availability : null,
                    PartTimeAvailability = x.ServiceTypeId == (int)ServiceType.PartTime ? (int?)x.Availability : null,
                    CityId = x.CityId,
                    StateProvinceId = x.StateProvinceId,
                    CountryId = x.CountryId
                })
                .FirstOrDefault();

            if (dto != null)
            {
                var expertises = _serviceExpertiseRepository.Table
                    .Where(x => x.Deleted == false
                    && x.ServiceProfileId == dto.Id)
                    .Join(_expertiseRepository.Table,
                    x => x.ExpertiseId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId
                        ,
                        Id = x.ExpertiseId
                        ,
                        x.OtherExpertise
                        ,
                        y.JobServiceCategoryId
                        ,
                        y.Name
                    })
                    .Select(x => new ServiceExpertiseDTO
                    {
                        Id = x.Id,
                        OtherExpertise = x.OtherExpertise,
                        CategoryId = x.JobServiceCategoryId,
                        Name = x.Name
                    })
                    .ToList();

                var licenses = (from sl in _serviceLicenseCertificateRepository.Table
                               where sl.Deleted == false
                               && sl.ServiceProfileId == dto.Id
                               join dl in _downloadRepository.Table on sl.DownloadId equals dl.Id into sd
                               from dl in sd.DefaultIfEmpty()
                               select new ServiceLicenseCertificateDTO
                               {
                                   Id = sl.Id,
                                   ProfessionalAssociationName = sl.ProfessionalAssociationName,
                                   LicenseCertificateName = sl.LicenseCertificateName,
                                   DownloadId = sl.DownloadId,
                                   DownloadName = dl != null ? (dl.Filename + dl.Extension) : null,
                                   DownloadGuid = dl != null ? dl.DownloadGuid : (Guid?)null
                               }).ToList();

                var academics = _serviceAcademicQualificationRepository.Table
                    .Where(x => x.Deleted == false
                    && x.ServiceProfileId == dto.Id)
                    .Select(x => new ServiceAcademicQualificationDTO
                    {
                        Id = x.Id,
                        AcademicQualificationType = x.AcademicQualificationType,
                        AcademicQualificationTypeName = ((AcademicQualificationType)x.AcademicQualificationType).GetDescription(),
                        AcademicQualificationName = x.AcademicQualificationName,
                        AcademicInstitution = x.AcademicInstitution
                    })
                    .ToList();

                var languages = _serviceLanguageProficiencyRepository.Table
                    .Where(x => x.Deleted == false
                    && x.ServiceProfileId == dto.Id)
                    .Join(_communicateLanguageRepository.Table,
                    x => x.LanguageId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId
                        ,
                        x.Id
                        ,
                        x.ProficiencyLevel
                        ,
                        x.ProficiencyWrittenLevel
                        ,
                        x.LanguageId
                        ,
                        y.Name
                    })
                    .Select(x => new ServiceLanguageProficiencyDTO
                    {
                        Id = x.Id,
                        LanguageId = x.LanguageId,
                        LanguageName = x.Name,
                        ProficiencyLevel = x.ProficiencyLevel,
                        ProficiencyLevelName = ((LanguageSpokenWrittenProficiency)x.ProficiencyLevel).GetDescription(),
                        ProficiencyWrittenLevel = x.ProficiencyWrittenLevel,
                        ProficiencyWrittenLevelName = ((LanguageSpokenWrittenProficiency)x.ProficiencyWrittenLevel).GetDescription()
                    })
                    .ToList();

                dto.CategoryId = expertises.Select(x => x.CategoryId).Distinct().FirstOrDefault();
                var category = _jobServiceCategoryRepository.Table
                    .Where(x => x.Id == dto.CategoryId)
                    .FirstOrDefault();
                dto.CategoryName = category?.Name;

                dto.ExperienceYearName = ((ExperienceYear)dto.YearExperience).GetDescription();
                dto.ServiceTypeName = ((ServiceType)dto.ServiceTypeId).GetDescription();
                dto.ServiceModelName = ((ServiceModel)dto.ServiceModelId).GetDescription();

                dto.ServiceExpertises = expertises;
                dto.ServiceLicenseCertificates = licenses;
                dto.ServiceAcademicQualifications = academics;
                dto.ServiceLanguageProficiencies = languages;

                if (dto.CountryId != null)
                {
                    var countryRecord = _countryRepository.Table.Where(x => x.Id == dto.CountryId).FirstOrDefault();
                    dto.CountryName = countryRecord.Name;

                }

                if (dto.StateProvinceId != null)
                {
                    var stateProvinceRecord = _stateProvinceRepository.Table.Where(x => x.Id == dto.StateProvinceId).FirstOrDefault();
                    dto.StateProvinceName = stateProvinceRecord.Name;

                }

                if (dto.CityId != null)
                {
                    var cityRecord = _cityRepository.Table.Where(x => x.Id == dto.CityId).FirstOrDefault();
                    dto.CityName = cityRecord.Name;

                }

            }

            return dto;
        }

        public List<ServiceProfile> GetServiceProfilesByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = _serviceProfileRepository.Table;

            var record = query
                .Where(x => x.CustomerId == customerId && !x.Deleted)
                .OrderByDescending(x => x.CreatedOnUTC)
                .ToList();

            return record;
        }

        public PagedListDTO<ServiceProfileDTO> SearchServiceProfiles(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            ServiceProfileSearchFilterDTO filterDTO = null,
            int consultationProfileId = 0,
            int sortBy = 0)
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
            var pExpertiseIds = SqlParameterHelper.GetStringParameter("ExpertiseIds", String.Join(",", filterDTO.ExpertiseIds));
            var pExclServiceTypeIds = SqlParameterHelper.GetStringParameter("ExclServiceTypeIds", String.Join(",", filterDTO.ExcludeServiceTypeIds));
            var pYearExperience = SqlParameterHelper.GetInt32Parameter("YearExperience", filterDTO.YearExperience);
            var pServiceTypeId = SqlParameterHelper.GetInt32Parameter("ServiceTypeId", filterDTO.ServiceTypeId);
            var pServiceModelId = SqlParameterHelper.GetInt32Parameter("ServiceModelId", filterDTO.ServiceModelId);
            var pStateProvinceId = SqlParameterHelper.GetInt32Parameter("StateProvinceId", filterDTO.StateProvinceId);
            var pCustomerId = SqlParameterHelper.GetInt32Parameter("CustomerId", filterDTO.CustomerId);
            var pBuyerCustomerId = SqlParameterHelper.GetInt32Parameter("BuyerCustomerId", filterDTO.BuyerCustomerId);
            var pConsultationProfileId = SqlParameterHelper.GetInt32Parameter("ConsultationProfileId", filterDTO.ConsultationProfileId);
            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", pageSize);
            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var serviceProfiles = _serviceProfileRepository.EntityFromSql("usp_SearchServiceProfile",
                pKeyword,
                pCategoryIds,
                pExpertiseIds,
                pExclServiceTypeIds,
                pYearExperience,
                pServiceTypeId,
                pServiceModelId,
                pStateProvinceId,
                pCustomerId,
                pBuyerCustomerId,
                pConsultationProfileId,
                pPageIndex,
                pPageSize,
                pTotalRecords
                );

            if (sortBy != 0)
            {
                if (sortBy == (int)ServiceSearchSortBy.PriceLowToHigh)
                {
                    serviceProfiles = serviceProfiles.OrderBy(x => x.ServiceFee).ToList();
                }
                else if (sortBy == (int)ServiceSearchSortBy.PriceHighToLow)
                {
                    serviceProfiles = serviceProfiles.OrderByDescending(x => x.ServiceFee).ToList();
                }
            }

            var serviceProfileDTOs = serviceProfiles
                .Select(x =>
                {
                    var dto = _mapper.Map<ServiceProfileDTO>(x);

                    dto.ExperienceYearName =
                    x.YearExperience == (int)ExperienceYear.YearLessThan10
                    ? ExperienceYear.YearLessThan10.GetDescription()
                    : x.YearExperience == (int)ExperienceYear.Year11To20
                    ? ExperienceYear.Year11To20.GetDescription()
                    : x.YearExperience == (int)ExperienceYear.Year21To30
                    ? ExperienceYear.Year21To30.GetDescription()
                    : null;
                    dto.ServiceTypeName =
                    x.ServiceTypeId == (int)ServiceType.Freelancing
                    ? ServiceType.Freelancing.GetDescription()
                    : x.ServiceTypeId == (int)ServiceType.PartTime
                    ? ServiceType.PartTime.GetDescription()
                    : x.ServiceTypeId == (int)ServiceType.Consultation
                    ? ServiceType.Consultation.GetDescription()
                    : x.ServiceTypeId == (int)ServiceType.ProjectBased
                    ? ServiceType.ProjectBased.GetDescription()
                    : null;
                    dto.ServiceModelName =
                    x.ServiceModelId == (int)ServiceModel.Onsite
                    ? ServiceModel.Onsite.GetDescription()
                    : x.ServiceModelId == (int)ServiceModel.PartialOnsite
                    ? ServiceModel.PartialOnsite.GetDescription()
                    : x.ServiceModelId == (int)ServiceModel.Remote
                    ? ServiceModel.Remote.GetDescription()
                    : null;
                    dto.EmploymentStatusName = ((EmploymentStatus)x.EmploymentStatus).GetDescription();
                    dto.ProjectBasedCharges = x.ServiceTypeId == (int)ServiceType.ProjectBased ? (decimal?)x.ServiceFee : null;
                    dto.ConsultationCharges = x.ServiceTypeId == (int)ServiceType.Consultation ? (decimal?)x.ServiceFee : null;
                    dto.FreelancingCharges = x.ServiceTypeId == (int)ServiceType.Freelancing ? (decimal?)x.ServiceFee : null;
                    dto.PartTimeCharges = x.ServiceTypeId == (int)ServiceType.PartTime ? (decimal?)x.ServiceFee : null;
                    dto.OnsiteCharges = x.OnsiteFee;
                    dto.FreelancingAvailability = x.ServiceTypeId == (int)ServiceType.Freelancing ? (int?)x.Availability : null;
                    dto.PartTimeAvailability = x.ServiceTypeId == (int)ServiceType.PartTime ? (int?)x.Availability : null;
                    return dto;
                })
                .ToList();

            if (serviceProfileDTOs.Count() > 0)
            {
                var serviceProfileIds = serviceProfileDTOs.Select(x => x.Id).ToList();
                var customerIds = serviceProfileDTOs.Select(x => x.CustomerId).ToList();
                var countryIds = serviceProfileDTOs.Select(x => x.CountryId).ToList();
                var stateIds = serviceProfileDTOs.Select(x => x.StateProvinceId).ToList();
                var cityIds = serviceProfileDTOs.Select(x => x.CityId).ToList();

                var expertises = _serviceExpertiseRepository.Table
                    .Where(x => !x.Deleted && serviceProfileIds.Contains(x.ServiceProfileId))
                    .Join(_expertiseRepository.Table,
                    x => x.ExpertiseId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId,
                        Id = x.ExpertiseId,
                        x.OtherExpertise,
                        y.JobServiceCategoryId,
                        y.Name
                    })
                    .ToList();
                var expertiseCategoryIds = expertises.Select(x => x.JobServiceCategoryId).Distinct().ToList();

                var category = _jobServiceCategoryRepository.Table
                                .Where(x => expertiseCategoryIds.Contains(x.Id))
                                .ToList();

                var licenses = (from sl in _serviceLicenseCertificateRepository.Table
                                where sl.Deleted == false
                                && serviceProfileIds.Contains(sl.ServiceProfileId)
                                join dl in _downloadRepository.Table on sl.DownloadId equals dl.Id into sd
                                from dl in sd.DefaultIfEmpty()
                                select new ServiceLicenseCertificateDTO
                                {
                                    Id = sl.Id,
                                    ServiceProfileId = sl.ServiceProfileId,
                                    ProfessionalAssociationName = sl.ProfessionalAssociationName,
                                    LicenseCertificateName = sl.LicenseCertificateName,
                                    DownloadId = sl.DownloadId,
                                    DownloadName = dl != null ? (dl.Filename + dl.Extension) : null,
                                    DownloadGuid = dl != null ? dl.DownloadGuid : (Guid?)null
                                }).ToList();

                var academics = _serviceAcademicQualificationRepository.Table
                    .Where(x => !x.Deleted && serviceProfileIds.Contains(x.ServiceProfileId))
                    .ToList();

                var languages = _serviceLanguageProficiencyRepository.Table
                    .Where(x => !x.Deleted && serviceProfileIds.Contains(x.ServiceProfileId))
                    .Join(_communicateLanguageRepository.Table,
                    x => x.LanguageId,
                    y => y.Id,
                    (x, y) => new
                    {
                        x.ServiceProfileId,
                        x.Id,
                        x.ProficiencyLevel,
                        x.ProficiencyWrittenLevel,
                        x.LanguageId,
                        y.Name
                    })
                    .ToList();

                var countries = _countryRepository.Table.Where(x => countryIds.Contains(x.Id)).ToList();
                var states = _stateProvinceRepository.Table.Where(x => stateIds.Contains(x.Id)).ToList();
                var cities = _cityRepository.Table.Where(x => cityIds.Contains(x.Id)).ToList();

                var customers = _individualeRepository.Table
                    .Where(x => !x.Deleted && customerIds.Contains(x.CustomerId));

                var reviews = _consultationInvitationRepository.Table.Where(z =>
                        z.Deleted == false
                        && serviceProfileIds.Contains(z.ServiceProfileId)
                        && z.ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                                && z.Rating != null)
                    .Select(x => x.ServiceProfileId)
                    .ToList();

                serviceProfileDTOs = serviceProfileDTOs.Select(x =>
                 {
                     x.ServiceExpertises = expertises.Where(y => y.ServiceProfileId == x.Id).Select(y => new ServiceExpertiseDTO
                     {
                         Id = y.Id,
                         OtherExpertise = y.OtherExpertise,
                         CategoryId = y.JobServiceCategoryId,
                         Name = y.Name
                     }).ToList();
                     x.ServiceLicenseCertificates = licenses.Where(y => y.ServiceProfileId == x.Id).Select(y => new ServiceLicenseCertificateDTO
                     {
                         Id = y.Id,
                         ProfessionalAssociationName = y.ProfessionalAssociationName,
                         LicenseCertificateName = y.LicenseCertificateName,
                         DownloadId = y.DownloadId,
                         DownloadName = y.DownloadName,
                         DownloadGuid = y.DownloadGuid
                     }).ToList();
                     x.ServiceAcademicQualifications = academics.Where(y => y.ServiceProfileId == x.Id).Select(y => new ServiceAcademicQualificationDTO
                     {
                         Id = y.Id,
                         AcademicQualificationType = y.AcademicQualificationType,
                         AcademicQualificationTypeName = ((AcademicQualificationType)y.AcademicQualificationType).GetDescription(),
                         AcademicQualificationName = y.AcademicQualificationName,
                         AcademicInstitution = y.AcademicInstitution
                     })
                    .ToList();
                     x.ServiceLanguageProficiencies = languages.Where(y => y.ServiceProfileId == x.Id)
                    .Select(y => new ServiceLanguageProficiencyDTO
                    {
                        Id = y.Id,
                        LanguageId = y.LanguageId,
                        LanguageName = y.Name,
                        ProficiencyLevel = y.ProficiencyLevel,
                        ProficiencyLevelName = ((LanguageSpokenWrittenProficiency)y.ProficiencyLevel).GetDescription(),
                        ProficiencyWrittenLevel = y.ProficiencyWrittenLevel,
                        ProficiencyWrittenLevelName = ((LanguageSpokenWrittenProficiency)y.ProficiencyWrittenLevel).GetDescription(),
                    })
                    .ToList();
                     x.CategoryId = category.Where(y => x.ServiceExpertises.Select(z => z.CategoryId).FirstOrDefault() == y.Id).Select(y => y.Id).FirstOrDefault();
                     x.CategoryName = category.Where(y => x.ServiceExpertises.Select(z => z.CategoryId).FirstOrDefault() == y.Id).Select(y => y.Name).FirstOrDefault();

                     x.CountryName = x.CountryId != null ? countries.Where(y => y.Id == x.CountryId).Select(y => y.Name).FirstOrDefault() : null;
                     x.StateProvinceName = x.StateProvinceId != null ? states.Where(y => y.Id == x.StateProvinceId).Select(y => y.Name).FirstOrDefault() : null;
                     x.CityName = x.CityId != null ? cities.Where(y => y.Id == x.CityId).Select(y => y.Name).FirstOrDefault() : null;

                     x.ReviewCount = reviews.Where(y => y == x.Id).Count();

                     var customer = customers.Where(y => y.CustomerId == x.CustomerId).FirstOrDefault();
                     x.DOB = customer.DateOfBirth;
                     x.GenderName = ((Gender)customer.Gender).GetDescription();

                     return x;
                 })
                 .ToList();
            }

            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            var response = new PagedListDTO<ServiceProfileDTO>(serviceProfileDTOs, pageIndex, pageSize, totalRecords);

            return response;
        }

        public int GetCustomerId(int serviceProfileId)
        {
            var query = _serviceProfileRepository.Table;

            var record = query
                .Where(x => x.Id == serviceProfileId && !x.Deleted)
                .Select(x => x.CustomerId)
                .FirstOrDefault();

            return record;
        }
        #endregion
    }
}
