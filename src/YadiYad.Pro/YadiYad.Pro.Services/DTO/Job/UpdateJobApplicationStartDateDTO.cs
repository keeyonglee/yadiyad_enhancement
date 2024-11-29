using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class UpdateJobApplicationStartDateDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
    }
}
