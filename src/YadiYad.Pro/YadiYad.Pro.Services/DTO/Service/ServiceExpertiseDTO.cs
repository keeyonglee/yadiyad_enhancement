using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceExpertiseDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string OtherExpertise { get; set; }
        public string Name { get; set; }
    }
}
