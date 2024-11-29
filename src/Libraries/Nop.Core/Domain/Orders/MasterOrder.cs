using System;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public partial class MasterOrder : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public Guid MasterOrderGuid { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the billing address identifier
        /// </summary>
        public int BillingAddressId { get; set; }

        /// <summary>
        /// Gets or sets the payment status identifier
        /// </summary>
        public int PaymentStatusId { get; set; }

        /// <summary>
        /// Gets or sets the payment method system name
        /// </summary>
        public string PaymentMethodSystemName { get; set; }

        /// <summary>
        /// Gets or sets the customer currency code (at the moment of order placing)
        /// </summary>
        public string CustomerCurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets the currency rate
        /// </summary>
        public decimal CurrencyRate { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (include tax)
        /// </summary>
        public decimal OrderSubtotalInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal (exclude tax)
        /// </summary>
        public decimal OrderSubtotalExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (include tax)
        /// </summary>
        public decimal OrderSubTotalDiscountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order subtotal discount (exclude tax)
        /// </summary>
        public decimal OrderSubTotalDiscountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (include tax)
        /// </summary>
        public decimal OrderShippingInclTax { get; set; }

        /// <summary>
        /// Gets or sets the order shipping (exclude tax)
        /// </summary>
        public decimal OrderShippingExclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (incl tax)
        /// </summary>
        public decimal PaymentMethodAdditionalFeeInclTax { get; set; }

        /// <summary>
        /// Gets or sets the payment method additional fee (exclude tax)
        /// </summary>
        public decimal PaymentMethodAdditionalFeeExclTax { get; set; }

        /// <summary>
        /// Gets or sets the tax rates
        /// </summary>
        public string TaxRates { get; set; }

        /// <summary>
        /// Gets or sets the order tax
        /// </summary>
        public decimal OrderTax { get; set; }

        /// <summary>
        /// Gets or sets the order discount (applied to order total)
        /// </summary>
        public decimal OrderDiscount { get; set; }

        /// <summary>
        /// Gets or sets the order total
        /// </summary>
        public decimal OrderTotal { get; set; }

        /// <summary>
        /// Gets or sets the refunded amount
        /// </summary>
        public decimal RefundedAmount { get; set; }

        /// <summary>
        /// Gets or sets the paid date and time
        /// </summary>
        public DateTime? PaidDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the serialized CustomValues (values from ProcessPaymentRequest)
        /// </summary>
        public string CustomValuesXml { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the date and time of order creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the custom order number without prefix
        /// </summary>
        public string CustomOrderNumber { get; set; }

        public decimal TotalVendorShippingDiscount { get; set; }

        public decimal TotalPlatformShippingDiscount { get; set; }
        public decimal TotalPlatformSubTotalDiscount { get; set; }


        #endregion

        #region Custom properties

        /// <summary>
        /// Gets or sets the payment status
        /// </summary>
        public PaymentStatus PaymentStatus
        {
            get => (PaymentStatus)PaymentStatusId;
            set => PaymentStatusId = (int)value;
        }

        #endregion
    }
}