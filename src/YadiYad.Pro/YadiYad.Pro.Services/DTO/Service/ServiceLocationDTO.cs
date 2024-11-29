using Nop.Core.Domain.Directory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceLocationDTO
    {
        public int CityId { get; set; }
        public string DisplayName { get; set; }
    }
}
