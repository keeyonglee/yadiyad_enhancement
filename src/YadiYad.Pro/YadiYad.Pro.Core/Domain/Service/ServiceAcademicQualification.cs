using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Service
{
    public class ServiceAcademicQualification : BaseEntityExtension
    {
        public int ServiceProfileId { get; set; }
        public int AcademicQualificationType { get; set; }
        public string AcademicQualificationName { get; set; }
        public string AcademicInstitution { get; set; }
        public bool IsHighest { get; set; }

        [ForeignKey("ServiceProfileId")]
        public ServiceProfile ServiceProfile { get; set; }
    }
}
