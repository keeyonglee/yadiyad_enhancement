using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerAcademicQualification : BaseEntityExtension
    {
        public int AcademicQualificationType { get; set; }
        public string AcademicQualificationName { get; set; }
        public string AcademicInstitution { get; set; }
        public bool IsHighest { get; set; }

        public int JobSeekerProfileId { get; set; }

        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; }
    }
}
