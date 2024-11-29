using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Services.DTO.Questionnaire;

namespace YadiYad.Pro.Services.DTO.Consultation
{
    public class ConsultationProfileDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ConsultationExpertiseDTO> ConsultationExpertises { get; set; } = new List<ConsultationExpertiseDTO>();
        public int YearExperience { get; set; }
        public string ExperienceYearName { get; set; }
        public int OrganizationProfileId { get; set; }
        [Required(ErrorMessage = "Please select segment")]
        public int SegmentId { get; set; }
        [Required(ErrorMessage = "Please insert topic")]
        public string Topic { get; set; }
        [Required(ErrorMessage = "Please insert objective")]
        public string Objective { get; set; }
        [Required(ErrorMessage = "Please select time zone")]
        public int TimeZoneId { get; set; }
        public string AvailableTimeSlot { get; set; }
        public string Questionnaire { get; set; }
        public double Duration { get; set; }
        [Required(ErrorMessage = "Please approve or reject")]

        public bool? IsApproved { get; set; }
        public bool DeletedByUser { get; set; }
        public string Remarks { get; set; }
        public int CompletionStatus { get; set; }

        public List<QuestionDTO> Questions { get; set; } = new List<QuestionDTO>();
        public List<TimeSlotDTO> TimeSlots { get; set; } = new List<TimeSlotDTO>();


        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
        public string SegmentName { get; set; }
        public string TimeZoneName { get; set; }

        public ConsultationProfile ToModel(IMapper mapper)
        {
            var consultationProfile = mapper.Map<ConsultationProfile>(this);
            consultationProfile.ConsultationExpertises = new List<ConsultationExpertise>();

            foreach (var expertise in ConsultationExpertises)
            {
                consultationProfile.ConsultationExpertises.Add(new ConsultationExpertise
                {
                    ExpertiseId = expertise.Id,
                    OtherExpertise = expertise.OtherExpertise
                });
            }

            return consultationProfile;
        } 
    }
}