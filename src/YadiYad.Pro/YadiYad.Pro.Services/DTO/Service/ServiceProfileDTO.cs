using AutoMapper;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Service;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceProfileDTO
    {
        public bool Deleted { get; set; }
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ServiceExpertiseDTO> ServiceExpertises { get; set; } = new List<ServiceExpertiseDTO>();
        public int YearExperience { get; set; }
        public string ExperienceYearName { get; set; }
        public string GenderName { get; set; }
        public int Age { 
            get
            {
                DateTime zeroTime = new DateTime(1, 1, 1);

                TimeSpan span = DateTime.Now.Date - DOB;
                int age = (zeroTime + span).Year - 1;

                return age;
            } 
        }
        public DateTime DOB { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string NationalityName { get; set; }
        public string Address { get; set; }
        public int ReviewCount { get; set; } = 0;
        public decimal Rating { get; set; } = 0;

        public int EmploymentStatus { get; set; }
        public string EmploymentStatusName { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime? TenureStart { get; set; }
        public DateTime? TenureEnd { get; set; }
        public string AchievementAward { get; set; }
        public bool Invited { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public List<ServiceAcademicQualificationDTO> ServiceAcademicQualifications { get; set; } = new List<ServiceAcademicQualificationDTO>();
        public List<ServiceLanguageProficiencyDTO> ServiceLanguageProficiencies { get; set; } = new List<ServiceLanguageProficiencyDTO>();
        public List<ServiceLicenseCertificateDTO> ServiceLicenseCertificates { get; set; } = new List<ServiceLicenseCertificateDTO>();

        public int ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; }

        public int ServiceModelId { get; set; }
        public string ServiceModelName { get; set; }
        public decimal? ProjectBasedCharges { get; set; }
        public decimal? ConsultationCharges { get; set; }
        public decimal? FreelancingCharges { get; set; }
        public decimal? PartTimeCharges { get; set; }
        public decimal? OnsiteCharges { get; set; }

        public int? FreelancingAvailability { get; set; }
        public int? PartTimeAvailability { get; set; }

        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }
        public int? CountryId { get; set; }

        public string CityName { get; set; }
        public string StateProvinceName { get; set; }
        public string CountryName { get; set; }

        public string Code
        {
            get
            {
                return "YP" + ("00000" + Id).PadRight(4);
            }
        }

        public ServiceProfile ToModel(IMapper mapper, int customerId)
        {
            var serviceProfile = mapper.Map<ServiceProfile>(this);
            serviceProfile.ServiceExpertises = new List<ServiceExpertise>();
            serviceProfile.ServiceLicenseCertificates = new List<ServiceLicenseCertificate>();
            serviceProfile.ServiceAcademicQualifications = new List<ServiceAcademicQualification>();
            serviceProfile.ServiceLanguageProficiencies = new List<ServiceLanguageProficiency>();


            serviceProfile.CustomerId = customerId;
            serviceProfile.OnsiteFee = OnsiteCharges;
            serviceProfile.ServiceExpertises = new List<ServiceExpertise>();

            switch (serviceProfile.ServiceType)
            {
                case ServiceType.ProjectBased:
                    serviceProfile.ServiceFee = this.ProjectBasedCharges ?? 0m;
                    break;
                case ServiceType.Consultation:
                    serviceProfile.ServiceFee = this.ConsultationCharges ?? 0m;
                    break;
                case ServiceType.Freelancing:
                    serviceProfile.ServiceFee = this.FreelancingCharges ?? 0m;
                    serviceProfile.Availability = this.FreelancingAvailability ?? 0;
                    break;
                case ServiceType.PartTime:
                    serviceProfile.ServiceFee = this.PartTimeCharges ?? 0m;
                    serviceProfile.Availability = this.PartTimeAvailability ?? 0;
                    break;
            }

            foreach (var expertise in ServiceExpertises)
            {
                serviceProfile.ServiceExpertises.Add(new ServiceExpertise
                {
                    ExpertiseId = expertise.Id,
                    OtherExpertise = expertise.OtherExpertise
                });
            }

            foreach (var license in ServiceLicenseCertificates)
            {
                serviceProfile.ServiceLicenseCertificates.Add(new ServiceLicenseCertificate
                {
                    ProfessionalAssociationName = license.ProfessionalAssociationName,
                    LicenseCertificateName = license.LicenseCertificateName,
                    DownloadId = license.DownloadId
                });
            }

            foreach (var academic in ServiceAcademicQualifications)
            {
                serviceProfile.ServiceAcademicQualifications.Add(new ServiceAcademicQualification
                {
                    AcademicQualificationType = academic.AcademicQualificationType,
                    AcademicQualificationName = academic.AcademicQualificationName,
                    AcademicInstitution = academic.AcademicInstitution,
                    IsHighest = academic.IsHighest
                });
            }

            foreach (var language in ServiceLanguageProficiencies)
            {
                serviceProfile.ServiceLanguageProficiencies.Add(new ServiceLanguageProficiency
                {
                    LanguageId = language.LanguageId,
                    ProficiencyLevel = language.ProficiencyLevel,
                    ProficiencyWrittenLevel = language.ProficiencyWrittenLevel
                });
            }

            return serviceProfile;
        }
    }
}
