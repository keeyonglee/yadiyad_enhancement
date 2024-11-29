using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Job
{
    public class JobMilestone : BaseEntityExtension
    {
        public int JobProfileId { get; set; }
        public int Sequence { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }


        [ForeignKey("JobProfileId")]
        public JobProfile JobProfile { get; set; }
    }
}
