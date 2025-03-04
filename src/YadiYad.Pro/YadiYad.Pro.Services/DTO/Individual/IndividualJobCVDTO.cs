using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class IndividualJobCVDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please enter summary")]
        public string Summary { get; set; }

        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
    }
}
