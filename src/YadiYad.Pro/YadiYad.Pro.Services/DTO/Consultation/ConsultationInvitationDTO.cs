using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.DTO.Questionnaire;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.DTO.Consultation
{
    public class ConsultationInvitationDTO
    {
        public int Id { get; set; }
        public int ServiceProfileId { get; set; }
        public List<int> ServiceProfileIds { get; set; }
        public int IndividualCustomerId { get; set; }
        public int ConsultationProfileId { get; set; }
        public int OrganizationProfileId { get; set; }
        public bool IsIndividualRead { get; set; }
        public bool isOrganizationRead { get; set; }
        public int ConsultationApplicationStatus { get; set; }
        public string OrganizationName { get; set; }
        public string ConsultationApplicationStatusText { get
            {
                var text =
                IsApproved == false
                ? ConsultationInvitationStatus.New.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Accepted
                ? ConsultationInvitationStatus.Accepted.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Completed
                ? ConsultationInvitationStatus.Completed.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.New
                ? ConsultationInvitationStatus.New.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.Paid
                ? ConsultationInvitationStatus.Paid.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByIndividual
                ? ConsultationInvitationStatus.DeclinedByIndividual.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByOrganization
                ? ConsultationInvitationStatus.CancelledByOrganization.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.CancelledByIndividual
                ? ConsultationInvitationStatus.CancelledByIndividual.GetDescription()
                : ConsultationApplicationStatus == (int)ConsultationInvitationStatus.DeclinedByOrganization
                ? ConsultationInvitationStatus.DeclinedByOrganization.GetDescription()
                : "Unknown";

                return text;
            }
        }
        public string ConsultantAvailableTimeSlot { get; set; }
        public string QuestionnaireAnswer { get; set; }
        public string Questionnaire { get; set; }
        public decimal? Rating { get; set; }
        public decimal? KnowledgenessRating { get; set; }
        public decimal? RelevanceRating { get; set; }
        public decimal? RespondingRating { get; set; }
        public decimal? ClearnessRating { get; set; }
        public decimal? ProfessionalismRating { get; set; }
        public bool? IsApproved { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDateTime { get; set; }
        public decimal? RatesPerSession { get; set; }
        public decimal? ServiceChargeRate { get; set; }
        public int? ServiceChargeType { get; set; }
        public int? ModeratorCustomerId{ get; set; }
        public string StatusRemarks { get; set; }

        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }


        public string SegmentName { get; set; }
        public string TimeZoneName { get; set; }
        public string Topic { get; set; }
        public string Objective { get; set; }

        public string DeclineReasons { get; set; }
        public string RescheduleRemarks { get; set; }

        public DateTime? CancellationDateTime { get; set; }
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public string AppointmentStartTimeText { get; set; }
        public string AppointmentEndTimeText { get; set; }
        public string AppointmentDateText { get; set; }
        public List<QuestionDTO> ConsultantReplys { get; set; } = new List<QuestionDTO>();
        public List<TimeSlotDTO> ConsultantAvailableTimeSlots { get; set; } = new List<TimeSlotDTO>();

        public ServiceProfileDTO ServiceProfile { get; set; }
        public OrganizationProfileDTO OrganizationProfile { get; set; }
        public ConsultationProfileDTO ConsultationProfile { get; set; }
        public IndividualProfileDTO IndividualProfile {  get; set; }
        public bool CanRefund { get; set; }
        public string ModeratorEmail { get; set; }
        public string ConsultantName { get; set; }

        public string Code
        {
            get
            {
                string format = "00000";
                string referenceNumber = "YC" + ("000000" + Id).PadRight(format.Length);

                return referenceNumber;
            }
        }

    }
}