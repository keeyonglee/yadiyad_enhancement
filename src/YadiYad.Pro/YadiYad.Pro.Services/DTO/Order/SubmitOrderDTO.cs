using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class SubmitOrderDTO
    {
        public int DepositRequestId { get; set; }
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public int RefId { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
    }
}
