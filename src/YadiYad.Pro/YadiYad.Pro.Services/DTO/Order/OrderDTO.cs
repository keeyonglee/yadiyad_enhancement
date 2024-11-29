using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public decimal TotalPayableAmount { get; set; }
        public string TransactionNo { get; set; }
        //public dynamic MoreInfo { get; set; }
        public int CustomerId { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public string CustomerCurrencyCode { get; set; }
        public decimal OrderTax { get; set; }
        public decimal OrderDiscount { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal RefundedAmount { get; set; }
        public DateTime? PaidOnUTC { get; set; }
        public string CustomOrderNumber { get; set; }
        public object MoreInfo { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public ProOrder ToModel(IMapper mapper, int customerId)
        {
            var order = mapper.Map<ProOrder>(this);

            order.CustomerId = customerId;
            order.OrderItems = new List<ProOrderItem>();

            foreach (var orderItem in OrderItems)
            {
                order.OrderItems.Add(mapper.Map<ProOrderItem>(orderItem));
            }

            return order;
        }
    }
}
