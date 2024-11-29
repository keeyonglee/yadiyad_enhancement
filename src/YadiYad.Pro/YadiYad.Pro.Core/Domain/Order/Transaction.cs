using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Order
{
    public class Transaction : BaseEntityExtension
    {
        public decimal Amount { get; set; }
        public int TransactionTypeId { get; set; }
        public int TransactionRefTypeId { get; set; }
        public int TransactionRefId { get; set; }
        public int Account { get; set; }
        public string Remarks { get; set; }
    }
}