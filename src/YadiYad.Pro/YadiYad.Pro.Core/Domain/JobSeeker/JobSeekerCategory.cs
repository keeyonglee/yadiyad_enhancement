using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerCategory : BaseEntityExtension
    {
        public int CategoryId { get; set; }
        public int YearExperience { get; set; }
        /// <summary>
        /// in format |1|2|32| filter with LIKE %|2|% OR LIKE '%|21|%'
        /// </summary>
        public string Expertises { get; set; }

        public int JobSeekerProfileId { get; set; }

        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; }
    }
}
