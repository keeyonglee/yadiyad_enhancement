using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Consultation
{
    public class ConsultationExpertise : BaseEntityExtension
    {
        public int ConsultationProfileId { get; set; }
        public int ExpertiseId { get; set; }
        public string OtherExpertise { get; set; }

        [ForeignKey("ConsultationProfileId")]
        public ConsultationProfile ConsultationProfile{ get; set; }
    }
}
