using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class Charge : BaseEntityExtension
    {
        public int ProductTypeId { get; set; }
        public int SubProductTypeId { get; set; }
        public int ValidityDays { get; set; }
        public int ValueType { get; set; }
        public decimal? MinRange { get; set; }
        public decimal? MaxRange { get; set; }
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
