using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Order
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string InvoiceNumber { get; set; }
        public int InvoiceType { get; set; }
        public int RefType { get; set; }
        public int InvoiceTo { get; set; }
        public int? InvoiceFrom { get; set; }
        public DateTime CreatedOnUTC { get; set; }

        //InvoiceFrom
        public string InvoiceFromName { get; set; }
        public string InvoiceFromBusinessName { get; set; }
        public string InvoiceFromAddress { get; set; }
        public string InvoiceFromState { get; set; }
        public string InvoiceFromCountry { get; set; }

        //InvoiceTo
        public string InvoiceToName { get; set; }
        public string InvoiceToBusinessName { get; set; }
        public string InvoiceToAddress { get; set; }
        public string InvoiceToState { get; set; }
        public string InvoiceToCountry { get; set; }

        //Amounts
        public string SSTRegNo { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }

        //Items
        public List<InvoiceItemDTO> InvoiceItems { get; set; }
        
        public InvoiceRefType InvoiceRefType
        {
            get => (InvoiceRefType)RefType;
            set => RefType = (int)value;
        }
    }
}
