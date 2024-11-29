using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutDetailDTO
    {
        public decimal TotalAmount { get; set; }
    }
}
