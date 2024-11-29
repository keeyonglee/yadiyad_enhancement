using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobSearchFilterDTO
    {
        public List<int> CategoryIds { get; set; } = new List<int>();
        public int JobTypeId { get; set; }
        public int JobModelId { get; set; }
        public int JobScheduleId { get; set; }
        public int StateProvinceId { get; set; }
        public int CustomerId { get; set; }
        public int JobSeekerProfileId { get; set; }

        public bool IsFreelanceHourly { get; set; }
        public bool IsFreelanceDaily { get; set; }
        public bool IsProjectBased { get; set; }

        public bool IsOnSite { get; set; }
        public bool IsPartialOnSite { get; set; }
        public bool IsRemote { get; set; }
    }
}
