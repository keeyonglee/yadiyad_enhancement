using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobProfile : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }
        public string JobTitle { get; set; }
        public int Status { get; set; }
        public int PreferredExperience { get; set; }
        public int JobType { get; set; }
        public int? JobRequired { get; set; }
        public bool IsImmediate { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsOnSite { get; set; }
        public bool IsPartialOnSite { get; set; }
        public bool IsRemote { get; set; }
        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }
        public int? CountryId { get; set; }
        public decimal PayAmount { get; set; }
        public int JobSchedule { get; set; }
        public List<JobMilestone> JobMilestones { get; set; } = new List<JobMilestone>();


    }
}
