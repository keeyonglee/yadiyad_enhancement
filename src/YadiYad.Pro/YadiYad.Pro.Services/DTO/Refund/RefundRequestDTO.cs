using Newtonsoft.Json;
using Nop.Core.Domain.Payout;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YadiYad.Pro.Core.Domain.Refund;

namespace YadiYad.Pro.Services.DTO.Refund
{
    public class RefundRequestDTO
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }
        public string RefundNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceCharge { get; set; }
        public int RefundTo { get; set; }
        public int Status { get; set; }
        public string StatusName
        {
            get
            {
                var statusName =
                    Status == (int)RefundStatus.New
                    ? RefundStatus.New.GetDescription()
                    : Status == (int)RefundStatus.Paid
                    ? RefundStatus.Paid.GetDescription()
                    : "Unknown";
                return statusName;
            }
        }

        public DateTime CreatedOnUTC { get; set; }
    }
}
