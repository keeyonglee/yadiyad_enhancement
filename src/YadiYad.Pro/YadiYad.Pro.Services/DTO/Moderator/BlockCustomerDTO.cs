using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Moderator
{
    public class BlockCustomerDTO
    {
        public bool IsCurrentlyBlock { get; set; }
        public bool WasBlockLast90Days { get; set; }
        public int BlockQuantity { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
