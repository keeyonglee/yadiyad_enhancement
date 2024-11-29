using Nop.Core;
using Nop.Core.Domain.Orders;
using System;

namespace Nop.Plugin.Payments.IPay88.Domain
{
    /// <summary>
    /// Represents a shipping by weight record
    /// </summary>
    public partial class IPay88PaymentRecord : BaseEntity
    {
        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>      
        public string PaymentNo { get; set; }
        public string Signature { get; set; }
        public string SignatureType { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
      
        public string ErrorDesc { get; set; }
        public string ProdDesc { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserContact { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public int StoreId { get; set; }

        public OrderType OrderType
        {
            get => (OrderType)OrderTypeId;
            set => OrderTypeId = (int)value;
        }

        public int OrderTypeId { get; set; }
        public int UniqueId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }
}