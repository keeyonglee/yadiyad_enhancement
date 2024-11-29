using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutRequestFilterDTO
    {
        public int RefId { get; set; }
        public int ProductTypeID { get; set; }
        public int CustomerId { get; set; }
    }
}
