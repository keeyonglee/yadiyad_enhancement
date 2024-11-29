using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerProfileDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        public int EmploymentStatus { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime? TenureStart { get; set; }
        public DateTime? TenureEnd { get; set; }
        public string TenureStartName
        {
            get
            {
                return TenureStart != null ? TenureStart.Value.ToShortDateString() : "";
            }
        }
        public string TenureEndName
        {
            get
            {
                return TenureEnd != null ? TenureEnd.Value.ToShortDateString() : "";
            }
        }
        public bool IsPresentCompany
        {
            get
            {
                return TenureEnd == null && TenureStart != null;
            }
        }

        public string AchievementAward { get; set; }

        public bool IsFreelanceHourly { get; set; }
        public bool IsFreelanceDaily { get; set; }
        public bool IsProjectBased { get; set; }

        public bool IsOnSite { get; set; }
        public bool IsPartialOnSite { get; set; }
        public bool IsRemote { get; set; }

        public decimal? HourlyPayAmount { get; set; }
        public decimal? DailyPayAmount { get; set; }

        public int? AvailableDays { get; set; }
        public int? AvailableHours { get; set; }
        public int DownloadId { get; set; }

        public string EmploymentStatusName { get; set; }
        public int PreferredExperience { get; set; }
        public string PreferredExperienceName
        {
            get
            {
                return ((ExperienceYear)PreferredExperience).GetDescription();
            }
        }

        public List<JobSeekerLicenseCertificateDTO> LicenseCertificates { get; set; }
        public List<JobSeekerAcademicQualificationDTO> AcademicQualifications { get; set; }
        public List<JobSeekerLanguageProficiencyDTO> LanguageProficiencies { get; set; }
        public List<JobSeekerPreferredLocationDTO> PreferredLocations { get; set; }
        public List<JobSeekerCategoryDTO> Categories { get; set; }

        public JobSeekerCVDTO  Cv { get; set; }
        
        // individual info
        public string GenderName { get; set; }
        public int Age { get; set; }
        public DateTime DOB { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string NationalityName { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string StateProvinceName { get; set; }
        public string CountryName { get; set; }
        
    }
}
