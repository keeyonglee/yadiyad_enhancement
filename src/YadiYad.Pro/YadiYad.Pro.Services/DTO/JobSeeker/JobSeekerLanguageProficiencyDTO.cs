using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.JobSeeker
{
    public class JobSeekerLanguageProficiencyDTO
    {
        public int JobSeekerProfileId { get; set; }
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public int ProficiencyLevel { get; set; }
        public int ProficiencyWrittenLevel { get; set; }
        public string ProficiencyLevelName { get; set; }
        public string ProficiencyWrittenLevelName { get; set; }

    }
}
