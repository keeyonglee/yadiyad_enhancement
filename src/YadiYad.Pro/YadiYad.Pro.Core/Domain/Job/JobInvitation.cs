using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobInvitation : BaseEntityExtension
    {
        public int OrganizationProfileId { get; set; }
        public int JobProfileId { get; set; }
        public int JobSeekerProfileId { get; set; }
        public bool IsRead { get; set; }
        public int JobInvitationStatus { get; set; }
    }
}
