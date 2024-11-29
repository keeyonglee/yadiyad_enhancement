using Newtonsoft.Json;
using Nop.Core.Domain.Payout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.DTO.Refund;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutAndGroupDTO
    {
        public int Id { get; set; }
        public int PayoutGroupId { get; set; }
        public int RefTypeId { get; set; }
        public int RefId { get; set; }
        public PayoutRequestDTO PayoutRequest { get; set; }
        public RefundRequestDTO RefundRequest { get; set; }
        public OrderPayoutRequest OrderPayoutRequest { get; set; }
        public OrderRefundRequest OrderRefundRequest { get; set; }
        public string CustomOrderNumber { get; set; }

        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
    }
}
