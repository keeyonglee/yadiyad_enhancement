using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class StatementDTO
    {
        public int Id { get; set; }
        public int? OrderItemId { get; set; }
        public string StatementNumber { get; set; }
        public int StatementType { get; set; }
        public int RefType { get; set; }
        public int StatementTo { get; set; }
        public int? StatementFrom { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        //InvoiceFrom
        public string StatementFromName { get; set; }
        public string StatementFromBusinessName { get; set; }
        public string StatementFromAddress1 { get; set; }
        public string StatementFromAddress2 { get; set; }
        public string StatementFromState { get; set; }
        public string StatementFromCity { get; set; }
        public string StatementFromZipPostalCode { get; set; }
        public string StatementFromCountry { get; set; }

        //InvoiceTo
        public string StatementToName { get; set; }
        public string StatementToBusinessName { get; set; }
        public string StatementToAddress1 { get; set; }
        public string StatementToAddress2 { get; set; }
        public string StatementToState { get; set; }
        public string StatementToCity { get; set; }
        public string StatementToZipPostalCode { get; set; }
        public string StatementToCountry { get; set; }

        //Amounts
        public string SSTRegNo { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }

        //Items
        public List<StatementItemDTO> StatementItems { get; set; }
    }
}
