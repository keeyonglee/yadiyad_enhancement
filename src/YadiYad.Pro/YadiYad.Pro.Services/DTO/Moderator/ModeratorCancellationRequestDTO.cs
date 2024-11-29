using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Services.DTO.Consultation;

namespace YadiYad.Pro.Services.DTO.Moderator
{
    public class ModeratorCancellationRequestDTO
    {
        public int EngagementId { get; set; }
        public string EngagementType { get; set; }
        public int ConsultationProfileId { get; set; }
        public string CancelledBy { get; set; }
        public string BuyerName { get; set; }
        public string SellerName { get; set; }
        public string BuyerEmail { get; set; }
        public string SellerEmail { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public int AttachmentId { get; set; }
        public List<TimeSlotDTO> ConsultantAvailableTimeSlots { get; set; } = new List<TimeSlotDTO>();
        public BlockCustomerDTO BlockStatusSeller { get; set; } = new BlockCustomerDTO();

        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
    }
}
