using System;
using Nop.Core.Domain.Shipping;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents a return request
    /// </summary>
    public partial class ReturnOrder : BaseEntity
    {
        public int GroupReturnRequestId { get; set; }

        public bool IsShipped { get; set; }
        public decimal? EstimatedShippingExclTax { get; set; }
        public decimal? EstimatedShippingInclTax { get; set; }
        public decimal? ActualShippingExclTax { get; set; }
        public decimal? ActualShippingInclTax { get; set; }
        public ShippingStatus ShippingStatus
        {
            get => IsShipped ? ShippingStatus.Shipped : ShippingStatus.NotYetShipped;
            set => IsShipped = ShippingStatus.Shipped == value ;
        }
        public DateTime? CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
