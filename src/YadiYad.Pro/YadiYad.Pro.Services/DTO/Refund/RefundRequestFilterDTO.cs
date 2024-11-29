using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Refund
{
    public class RefundRequestFilterDTO
    {
        public int RefId { get; set; }
        public List<int> ProductTypeIDs { get; set; }
        public int CustomerId { get; set; }
    }
}
