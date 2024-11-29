using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Service
{
    public class ServiceLanguageProficiency : BaseEntityExtension
    {
        public int ServiceProfileId { get; set; }
        public int LanguageId { get; set; }
        public int ProficiencyLevel { get; set; }
        public int ProficiencyWrittenLevel { get; set; }


        [ForeignKey("ServiceProfileId")]
        public ServiceProfile ServiceProfile { get; set; }
    }
}
