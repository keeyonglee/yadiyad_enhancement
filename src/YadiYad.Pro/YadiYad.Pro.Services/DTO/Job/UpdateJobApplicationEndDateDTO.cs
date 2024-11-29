using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class UpdateJobApplicationEndDateDTO
    {
        public DateTime? EndDate { get; set; }
        public int? EndMilestoneId { get; set; }

        [Required]
        public string Remarks { get; set; }
    }
}
