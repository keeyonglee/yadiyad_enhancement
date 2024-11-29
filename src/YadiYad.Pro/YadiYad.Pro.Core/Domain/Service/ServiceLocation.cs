//using Nop.Core.Domain.Directory;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Text;
//using YadiYad.Pro.Core.Domain.Common;

//namespace YadiYad.Pro.Core.Domain.Service
//{
//    public class ServiceLocation : BaseEntityExtension
//    {
//        public int ServiceProfileId { get; set; }
//        public int? CountryId { get; set; }
//        public int? StateProvinceId { get; set; }
//        public int? CityId { get; set; }

//        [ForeignKey("ServiceProfileId")]
//        public ServiceProfile ServiceProfile { get; set; }

//        [ForeignKey("CityId")]
//        public City City { get; set; }
//    }
//}
