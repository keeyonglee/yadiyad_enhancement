using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class UpdateServiceApplicationEndDateDTO
    {
        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public String Remarks { get; set; }
    }
}
