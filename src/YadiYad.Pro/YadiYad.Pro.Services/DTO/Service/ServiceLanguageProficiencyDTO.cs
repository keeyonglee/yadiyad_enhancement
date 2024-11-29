using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceLanguageProficiencyDTO
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public int ProficiencyLevel { get; set; }
        public int ProficiencyWrittenLevel { get; set; }
        public string ProficiencyLevelName { get; set; }
        public string ProficiencyWrittenLevelName { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }

    }
}
