using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class ServiceChargeDTO
    {
        public int ValidityDays { get; set; }
        public int ChargeValueType { get; set; }
        public decimal Value { get; set; }
    }
}
