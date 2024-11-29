using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Payout
{
    public partial class OrderPayoutRequest : BaseEntity
    {
        public int OrderId​ { get; set; }
        public int CustomerId { get; set; }
        public int VendorCustomerId { get; set; }

        public decimal OrderTotal​ { get; set; }

        //product price used for calculate service charge
        public decimal ProductPriceInclTax​ { get; set; }
        public decimal ProductPriceExclTax​ { get; set; }

        //fullfilment shipping
        public decimal FulfillmentShipmentInclTax​ { get; set; }
        public decimal FulfillmentShipmentExclTax​ { get; set; }

        //return shipping
        public decimal ReturnShipmentInclTax​ { get; set; }
        public decimal ReturnShipmentExclTax​ { get; set; }

        public decimal Insurance { get; set; }

        //refund
        public decimal RefundAmount { get; set; }

        //platform charges
        public decimal? ServiceCharge { get; set; }
        public decimal? ServiceChargeRate { get; set; }
        public decimal? ServiceChargeSST { get; set; }

        public decimal? ServiceChargesWaiver { get; set; }
        public DateTime? ServiceChargesUTC { get; set; }

        //order date time
        public DateTime TransactionDate​ { get; set; }
        public int OrderPayoutStatusId { get; set; }

        public int? InvoiceId { get; set; }

        public bool Deleted { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

        public decimal PlatformShippingDiscount { get; set; }
        public decimal PlatformSubTotalDiscount { get; set; }

        public decimal GrossPayoutAmount()
        {
            return OrderTotal
                     - FulfillmentShipmentInclTax
                     - ReturnShipmentInclTax
                     - RefundAmount
                     - Insurance
                     + PlatformShippingDiscount
                     + PlatformSubTotalDiscount;
        }

        public decimal ChargablePayoutAmount()
        {
            return ProductPriceExclTax - RefundAmount;
        }

        public void CreateAudit(int actorId)
        {
            CreatedById = actorId;
            CreatedOnUTC = DateTime.UtcNow;
            UpdatedById = actorId;
            UpdatedOnUTC = DateTime.UtcNow;
        }

        public void UpdateAudit(int actorId)
        {
            UpdatedById = actorId;
            UpdatedOnUTC = DateTime.UtcNow;
        }

        public void UpdateAudit(BaseEntityExtension oldModel, int actorId)
        {
            CreatedById = oldModel.CreatedById;
            CreatedOnUTC = oldModel.CreatedOnUTC;
            UpdatedById = actorId;
            UpdatedOnUTC = DateTime.UtcNow;
        }

        public void ApplyChargeWaiver(decimal serviceCharge)
        {
            
        }

        #region Custom properties

        public OrderPayoutStatus OrderPayoutStatus
        {
            get => (OrderPayoutStatus)OrderPayoutStatusId;
            set => OrderPayoutStatusId = (int)value;
        }

        #endregion
    }
}
