using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class StatementItemDTO
    {
        public string ItemName { get; set; }
        public List<string> ItemNames { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
    }
}
