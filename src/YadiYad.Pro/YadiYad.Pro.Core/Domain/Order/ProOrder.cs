using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class ProOrder : BaseEntityExtension, ICustomOrderEntity
    {
        #region Properties
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
        public string MoreInfo { get; set; }

        #endregion

        #region Custom properties

        public OrderStatus OrderStatus
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (int)value;
        }
        public PaymentStatus PaymentStatus
        {
            get => (PaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }

        #endregion

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public List<ProOrderItem> OrderItems { get; set; } = new List<ProOrderItem>();

        /// <summary>
        /// Gets or sets a payment method identifier
        /// </summary>
        public string PaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets a store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the serialized CustomValues (values from ProcessPaymentRequest)
        /// </summary>
        public string CustomValuesXml { get; set; }

    }
}