using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Common
{
    public class LocationDTO
    {
        public int CityId { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public int StateProvinceId { get; set; }
        public int CountryId { get; set; }
        public string Country { get; set; }
        public string DisplayName { get; set; }
    }
}
