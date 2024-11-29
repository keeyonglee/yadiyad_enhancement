using System;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return request
    /// </summary>
    public partial class GroupReturnRequest : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ApproveStatusId { get; set; }
        public DateTime? ApprovalDateUtc { get; set; }
        public bool IsInsuranceClaim { get; set; }
        public bool HasInsuranceCover { get; set; }
        public decimal? InsuranceClaimAmt { get; set; }
        public int InsuranceRef { get; set; }
        public DateTime? InsurancePayoutDate { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public int ReturnConditionId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        
        public bool NeedReturnShipping { get; set; }
        
        public ApproveStatusEnum ApproveStatus
        {
            get => (ApproveStatusEnum)ApproveStatusId;
            set => ApproveStatusId = (int)value;
        }

        public ReturnConditionEnum ReturnCondition
        {
            get => (ReturnConditionEnum)ReturnConditionId;
            set => ReturnConditionId = (int)value;
        }
    }
}
