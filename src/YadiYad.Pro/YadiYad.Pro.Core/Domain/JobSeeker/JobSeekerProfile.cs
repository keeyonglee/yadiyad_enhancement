using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerProfile : BaseEntityExtension
    {
        public int CustomerId { get; set; }

        public int EmploymentStatus { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public DateTime? TenureStart { get; set; }
        public DateTime? TenureEnd { get; set; }
        public string AchievementAward { get; set; }

        public bool IsFreelanceHourly { get; set; }
        public bool IsFreelanceDaily { get; set; }
        public bool IsProjectBased { get; set; }

        public bool IsOnSite { get; set; }
        public bool IsPartialOnSite { get; set; }
        public bool IsRemote { get; set; }

        public decimal? HourlyPayAmount { get; set; }
        public decimal? DailyPayAmount { get; set; }

        public int? AvailableHours { get; set; }
        public int? AvailableDays { get; set; }

        public List<JobSeekerCategory> Categories { get; set; }
        public List<JobSeekerLicenseCertificate> LicenseCertificates { get; set; }
        public List<JobSeekerAcademicQualification> AcademicQualifications { get; set; }
        public List<JobSeekerLanguageProficiency> LanguageProficiencies { get; set; }
        public List<JobSeekerPreferredLocation> PreferredLocations { get; set; }
        public JobSeekerCV CV { get; set; }
    }
}