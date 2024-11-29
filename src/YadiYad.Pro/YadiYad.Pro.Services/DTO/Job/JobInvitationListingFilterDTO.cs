using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobInvitationListingFilterDTO
    {
        public int JobProfileId { get; set; }
        public int IndividualCustomerId { get; set; }
        public int OrganizationCustomerId { get; set; }
        public List<int> JobInvitationStatus { get; set; }
    }
}
