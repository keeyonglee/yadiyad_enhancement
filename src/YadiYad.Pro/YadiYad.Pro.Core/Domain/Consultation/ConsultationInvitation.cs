using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Consultation
{
    public class ConsultationInvitation : BaseEntityExtension
    {
        public int ServiceProfileId { get; set; }
        public int IndividualCustomerId { get; set; }
        public int ConsultationProfileId { get; set; }
        public int OrganizationProfileId { get; set; }
        public bool IsIndividualRead { get; set; }
        public bool IsOrganizationRead { get; set; }
        public int ConsultationApplicationStatus { get; set; }
        public string ConsultantAvailableTimeSlot { get; set; }
        public string QuestionnaireAnswer { get; set; }
        public string Questionnaire { get; set; }
        public decimal? Rating { get; set; }
        public decimal? KnowledgenessRating { get; set; }
        public decimal? RelevanceRating { get; set; }
        public decimal? RespondingRating { get; set; }
        public decimal? ClearnessRating { get; set; }
        public decimal? ProfessionalismRating { get; set; }
        public decimal? ModeratorKnowledgenessRating { get; set; }
        public decimal? ModeratorRelevanceRating { get; set; }
        public decimal? ModeratorRespondingRating { get; set; }
        public decimal? ModeratorClearnessRating { get; set; }
        public decimal? ModeratorProfessionalismRating { get; set; }
        public string ModeratorReviewText { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReviewDateTime { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovalRemarks { get; set; }
        public string StatusRemarks { get; set; }
        public string DeclineReasons { get; set; }
        public string RescheduleRemarks { get; set; }
        public int? ModeratorCustomerId { get; set; }
        public decimal? RatesPerSession { get; set; }
        public string CancellationRemarks { get; set; }
        public int? CancellationReasonId { get; set; }
        public int? CancellationDownloadId { get; set; }
        public DateTime? CancellationDateTime { get; set; }
        [ForeignKey("ModeratorCustomerId")]
        public Customer Customer { get; set; }
        [ForeignKey("CancellationReasonId")]
        public Reason Reason { get; set; }
        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
    }
}
