using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class ChargeDTO
    {
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public int SubProductTypeId { get; set; }
        public int ValidityDays { get; set; }
        public int ValueType { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public Charge ToModel(IMapper mapper)
        {
            var order = mapper.Map<Charge>(this);

            return order;
        }
    }
}
