using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerLanguageProficiency : BaseEntityExtension
    {
        public int LanguageId { get; set; }
        public int ProficiencyLevel { get; set; }
        public int ProficiencyWrittenLevel { get; set; }

        public int JobSeekerProfileId { get; set; }

        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; }
    }
}
