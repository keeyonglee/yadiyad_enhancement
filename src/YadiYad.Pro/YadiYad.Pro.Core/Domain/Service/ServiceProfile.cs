using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Service
{
    public class ServiceProfile : BaseEntityExtension
    {
        public int CustomerId { get; set; }

        public int YearExperience { get; set; }
        public int EmploymentStatus { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime? TenureStart { get; set; }
        public DateTime? TenureEnd { get; set; }
        public string AchievementAward { get; set; }

        public bool DeletedFromUser { get; set; }

        public int ServiceTypeId { get; set; }
        public int ServiceModelId { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal? OnsiteFee { get; set; }
        public int Availability { get; set; }
        public decimal Rating { get; set; }

        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }
        public int? CountryId { get; set; }

        public ServiceType ServiceType { get => (ServiceType)ServiceTypeId; set => ServiceTypeId = (int)value; }
        public ServiceModel ServiceModel { get => (ServiceModel)ServiceModelId; set => ServiceModelId = (int)value; }


        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public List<ServiceExpertise> ServiceExpertises { get; set; } = new List<ServiceExpertise>();
        public List<ServiceAcademicQualification> ServiceAcademicQualifications { get; set; } = new List<ServiceAcademicQualification>();
        public List<ServiceLanguageProficiency> ServiceLanguageProficiencies { get; set; } = new List<ServiceLanguageProficiency>();
        public List<ServiceLicenseCertificate> ServiceLicenseCertificates { get; set; } = new List<ServiceLicenseCertificate>();
    }
}
