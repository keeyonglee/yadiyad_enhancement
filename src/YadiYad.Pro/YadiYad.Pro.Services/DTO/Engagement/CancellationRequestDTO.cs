using System;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.DTO.Moderator;

namespace YadiYad.Pro.Services.DTO.Engagement
{
    public class CancellationRequestDTO
    {

        public CancellationRequestDTO()
        {
            EngagementPartyInfo = new EngagementPartyInfo();
        }
        public int Id { get; set; }
        public int EngagementId { get; set; }
        public EngagementType EngagementType { get; set; }
        public EngagementParty CancelledBy { get; set; }
        public int ReasonId { get; set; }
        public string UserRemarks { get; set; }
        public DateTime SubmissionDate { get; set; }
        public CancellationStatus Status { get; set; }
        public PostCancellationAction PostCancellationAction { get; set; }
        public int? AttachmentId { get; set; }
        public string AdminRemarks { get; set; }
        public int CustomerBlockDays { get; set; }

        #region AdditionalSettable
        public string CancelledByDescription { get; set; }
        #endregion

        #region Computed
        public string EngagementTypeDescription => EngagementType.GetDescription();
        public string StatusDescription => Status.GetDescription();
        public string PostCancellationActionDescription => PostCancellationAction.GetDescription();
        #endregion

        #region Relations
        public Reason Reason { get; set; } = new Reason();
        public EngagementPartyInfo EngagementPartyInfo { get; set; } = new EngagementPartyInfo();
        public BlockCustomerDTO SellerBlockStatus { get; set; } = new BlockCustomerDTO();
        #endregion
    }
}