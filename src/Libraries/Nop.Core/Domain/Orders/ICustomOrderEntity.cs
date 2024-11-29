using System;
using Nop.Core.Domain.Payments;

namespace Nop.Core.Domain.Orders
{
    public interface ICustomOrderEntity
    {
        int Id { get; set; }
        int CustomerId { get; set; }
        int OrderStatusId { get; set; }
        int PaymentStatusId { get; set; }
        string PaymentMethodSystemName { get; set; }
        int StoreId { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal RefundedAmount { get; set; }
        public DateTime? PaidOnUTC { get; set; }
        public string CustomOrderNumber { get; set; }

        OrderStatus OrderStatus { get; set; }
        PaymentStatus PaymentStatus { get; set; }
    }
}
