using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class PaymentDetailsSearchFilterDTO
    {
        public int CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int PaymentStatusId { get; set; } = 30;
    }
}
