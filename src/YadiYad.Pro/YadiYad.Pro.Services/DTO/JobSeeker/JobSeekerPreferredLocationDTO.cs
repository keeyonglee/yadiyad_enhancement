using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerPreferredLocationDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int Id { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }
        public int? StateProvinceId { get; set; }
        public string StateProvinceName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
