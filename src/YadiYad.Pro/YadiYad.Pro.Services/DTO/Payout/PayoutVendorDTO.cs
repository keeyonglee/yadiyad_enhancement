using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutVendorDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string StatusText { get; set; }
        public int StatusId { get; set; }
        public decimal Amount { get; set; }
        public int NumberOfOrders { get; set; }
        public int PayoutGroupId { get; set; }
        public int PayoutBatchId { get; set; }
        public int OrderId { get; set; }
    }
}
