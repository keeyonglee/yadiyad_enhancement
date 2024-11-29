using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Core.Domain.Payout
{
    public class OrderRefundRequest : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public int RefundStatusId { get; set; }
        public int OrderId { get; set; }

        [MaxLength(50)]
        public string DocumentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate​ { get; set; }

        #region Custom properties

        public RefundStatus RefundStatus
        {
            get => (RefundStatus)RefundStatusId;
            set => RefundStatusId = (int)value;
        }

        #endregion
    }
}
