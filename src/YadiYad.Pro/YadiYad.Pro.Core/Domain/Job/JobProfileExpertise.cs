using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobProfileExpertise : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int JobProfieId { get; set; }
        public int ExpertiseId { get; set; }
        public string OtherName { get; set; }
    }
}
