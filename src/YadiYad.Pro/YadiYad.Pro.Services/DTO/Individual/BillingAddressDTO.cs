using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Individual
{
    public class BillingAddressDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int StateProvinceId { get; set; }
        public string StateProvinceName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
    }
}
