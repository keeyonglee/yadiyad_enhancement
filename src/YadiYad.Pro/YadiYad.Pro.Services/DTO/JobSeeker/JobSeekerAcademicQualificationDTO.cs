using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerAcademicQualificationDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int Id { get; set; }
        [JsonProperty(PropertyName = "AcademicQualificationTypeId")]
        public int AcademicQualificationType { get; set; }
        public string AcademicQualificationTypeName { get; set; }
        public string AcademicQualificationName { get; set; }
        public string AcademicInstitution { get; set; }
        public bool IsHighest { get; set; }
    }
}
