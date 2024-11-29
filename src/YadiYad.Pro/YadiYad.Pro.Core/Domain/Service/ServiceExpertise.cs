using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Service
{
    public class ServiceExpertise : BaseEntityExtension
    {
        public int ServiceProfileId { get; set; }
        public int ExpertiseId { get; set; }
        public string OtherExpertise { get; set; }

        [ForeignKey("ServiceProfileId")]
        public ServiceProfile ServiceProfile { get; set; }
    }
}
