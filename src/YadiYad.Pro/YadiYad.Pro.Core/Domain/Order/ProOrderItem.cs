using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class ProOrderItem : BaseEntityExtension
    {
        public int OrderId { get; set; }

        public int ProductTypeId { get; set; }
        public int RefId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int? InvoiceId { get; set; }

        //for rematch
        public int Status { get; set; }
        public int? OffsetProOrderItemId { get; set; }

        [ForeignKey("OrderId")]
        public ProOrder Order { get; set; }
    }
}