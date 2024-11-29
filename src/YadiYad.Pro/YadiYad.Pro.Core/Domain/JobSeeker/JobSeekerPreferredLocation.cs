using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.JobSeeker
{
    public class JobSeekerPreferredLocation : BaseEntityExtension
    {
        public int? CityId { get; set; }
        public int? StateProvinceId { get; set; }
        public int CountryId { get; set; }

        public int JobSeekerProfileId { get; set; }

        [ForeignKey("JobSeekerProfileId")]
        public JobSeekerProfile JobSeekerProfile { get; set; }
    }
}
