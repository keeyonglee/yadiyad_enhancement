using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class OrderSearchFilterDTO
    {
        public List<int> CustomerIds { get; set; }
        public List<int> OrderStatusIds { get; set; }
        public List<int> PaymentStatusIds { get; set; }
        public List<int> ProductTypeIds { get; set; }
        public List<int> RefIds { get; set; }
    }
}
