using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobApplicationListingFilterDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int OrganizationProfileId { get; set; }
        public int JobProfileId { get; set; }
        public int OrganizationCustomerId { get; set; }
        public int IndividualCustomerId { get; set; }
        public List<int> JobApplicationStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ExcludePendingApplicationIfHired { get; set; }


    }
}
