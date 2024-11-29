using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int TransactionTypeId { get; set; }
        public int TransactionRefTypeId { get; set; }
        public int TransactionRefId { get; set; }
        public int Account { get; set; }
        public string Remarks { get; set; }
        public Transaction ToModel(IMapper mapper)
        {
            var transaction = mapper.Map<Transaction>(this);

            return transaction;
        }
    }
}
