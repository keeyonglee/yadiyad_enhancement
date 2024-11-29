using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Job
{
    public class JobApplicationReviewDTO
    {
        public string ReviewText { get; set; }
        public decimal KnowledgenessRating { get; set; }
        public decimal RelevanceRating { get; set; }
        public decimal RespondingRating { get; set; }
        public decimal ClearnessRating { get; set; }
        public decimal ProfessionalismRating { get; set; }
    }
}
