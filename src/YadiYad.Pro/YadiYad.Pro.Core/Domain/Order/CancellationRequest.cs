using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class CancellationRequest : EngagementBaseEntity
    {
        public EngagementParty CancelledBy { get; set; }
        public int ReasonId { get; set; }
        public string UserRemarks { get; set; }
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;
        public CancellationStatus Status { get; set; } = CancellationStatus.New;
        public PostCancellationAction PostCancellationAction { get; set; } = PostCancellationAction.Refund;
        public int? AttachmentId { get; set; }
        public string AdminRemarks { get; set; }
        public int CustomerBlockDays { get; set; } = 0;
    }

    public class EngagementBaseEntity : BaseEntityExtension
    {
        public int EngagementId { get; set; }
        public EngagementType EngagementType { get; set; }
    }

    public enum CancellationStatus
    {
        New = 1,
        UnderInvestigation = 2,
        Closed = 3
    }

    public enum PostCancellationAction
    {
        Refund = 1,
        Rehire = 2
    }
}