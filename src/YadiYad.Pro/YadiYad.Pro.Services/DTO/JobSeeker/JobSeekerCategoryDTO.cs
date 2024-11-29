using System;
using System.Collections.Generic;
using System.Text;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Service;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerCategoryDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int YearExperience { get; set; }
        public string YearExperienceName { get; set; }
        public List<ExpertiseDTO> CategoryExpertises { get; set; } = new List<ExpertiseDTO>();
        public List<int> ExpertiseIds { get; set; }
        public string Expertises { get; set; }
    }
}
