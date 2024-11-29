using System;
using System.Collections.Generic;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public partial class Dispute : BaseEntity
    {
        #region Properties
        public int GroupReturnRequestId { get; set; }
        public int VendorId { get; set; }
        public int DisputeReasonId { get; set; }
        public string DisputeDetail { get; set; }
        public int OrderId { get; set; }
        public bool RaiseClaim { get; set; }
        public int DisputeAction { get; set; }
        public decimal? PartialAmount { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public bool IsReturnDispute { get; set; }
        #endregion

        #region Custom properties

        public DisputeReasonEnum DisputeReason
        {
            get => (DisputeReasonEnum)DisputeReasonId;
            set => DisputeReasonId = (int)value;
        }

        public DisputeActionEnum DisputeActionStr
        {
            get => (DisputeActionEnum)DisputeAction;
            set => DisputeAction = (int)value;
        }

        #endregion
    }
}