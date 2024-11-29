using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutBatchFilterDTO
    {
        public DateTime? GeneratedDate { get; set; }
        public List<int> CustomerIds { get; set; }
        public int? Status { get; set; }
    }
}
