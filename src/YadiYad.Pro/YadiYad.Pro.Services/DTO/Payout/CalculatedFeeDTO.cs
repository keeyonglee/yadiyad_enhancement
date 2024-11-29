using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Services.DTO.Order;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class CalculatedFeeDTO
    {
        public decimal Fee { get; set; }
        public int ProductTypeId { get; set; }
        public int SubProductTypeId { get; set; }
        public int RefId { get; set; }
        public int MaxMonthlyWorkDuration { get; set; }
        public decimal ProratedPercentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
