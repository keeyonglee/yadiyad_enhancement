using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class City : BaseEntity
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public int? CountryId { get; set; }
        public int? StateProvinceId { get; set; }
    }
}
