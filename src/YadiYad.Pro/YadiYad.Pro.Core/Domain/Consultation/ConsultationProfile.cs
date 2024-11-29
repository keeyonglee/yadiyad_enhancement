using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Organization;

namespace YadiYad.Pro.Core.Domain.Consultation
{
    public class ConsultationProfile : BaseEntityExtension
    {
        public int OrganizationProfileId { get; set; }
        public int SegmentId { get; set; }
        public int YearExperience { get; set; }
        public string Topic { get; set; }
        public string Objective { get; set; }
        public int TimeZoneId { get; set; }
        public string AvailableTimeSlot { get; set; }
        public string Questionnaire { get; set; }
        public double Duration { get; set; }
        public bool? IsApproved { get; set; }
        public string Remarks { get; set; }
        public bool DeletedFromUser { get; set; }

        [ForeignKey("OrganizationProfileId")]
        public OrganizationProfile OrganizationProfile { get; set; }
        public List<ConsultationExpertise> ConsultationExpertises { get; set; } = new List<ConsultationExpertise>();
    }
}
