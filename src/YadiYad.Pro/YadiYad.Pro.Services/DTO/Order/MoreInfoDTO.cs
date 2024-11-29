using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class MoreInfoDTO
    {        public int ServiceProfileServiceTypeId { get; set; }
        public int ChargeId { get; set; }
        public int ValidityDays { get; set; }
        public int ChargeValueType { get; set; }
        public decimal Value { get; set; }
        public decimal MonthlyPayment { get; set; }
        public decimal PayAmount { get; set; }
    }
}
