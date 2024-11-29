using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Individual
{
    public class BillingAddress : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public int CityId { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
    }
}