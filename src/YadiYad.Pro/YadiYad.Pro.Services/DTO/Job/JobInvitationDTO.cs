using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.JobSeeker;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobInvitationDTO
    {
        public int Id { get; set; }
        public int ConsultationInvitationId { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovalRemarks { get; set; }
        public int JobProfileId { get; set; }
        // for 
        public int ServiceProfileId { get; set; }        
        public int ConsultationProfileId { get; set; }

        public int JobSeekerProfileId { get; set; }
        public int IndividualCustomerId { get; set; }

        public int OrganizationProfileId { get; set; }

        public string Questionnaire { get; set; }
        public List<TimeSlotDTO> ConsultantAvailableTimeSlots { get; set; } = new List<TimeSlotDTO>();
        public List<QuestionDTO> ConsultantReplys { get; set; } = new List<QuestionDTO>();
        public decimal? RatesPerSession { get; set; }

        //public List<int> ServiceProfileIds { get; set; }
        public bool IsRead { get; set; }

        public int JobInvitationStatus { get; set; }
        public string JobInvitationStatusName { get; set; }

        public bool IsEscrow { get; set; }


        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }


        public ConsultationProfileDTO ConsultationProfile { get; set; }

        public ServiceProfileDTO ServiceProfile { get; set; }
        public JobProfileDTO JobProfile { get; set; }
        public JobSeekerProfileDTO JobSeekerProfile { get; set; }
        public IndividualProfileDTO ServiceIndividualProfile { get; set; }
        public string OrganizationName { get; set; }

        public string Code
        {
            get
            {
                return "YP" + ("00000" + Id).PadRight(4);
            }
        }
    }
}