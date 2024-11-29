using System;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return request
    /// </summary>
    public partial class ReturnOrderItem : BaseEntity
    {
        public int ReturnOrderId { get; set; }
        public int ReturnRequestId { get; set; }
    }
}
