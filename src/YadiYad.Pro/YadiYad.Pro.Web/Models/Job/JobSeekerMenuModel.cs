using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Attentions;
using YadiYad.Pro.Web.Enums;

namespace YadiYad.Pro.Web.Models.Job
{
    public class JobSeekerMenuModel
    {
        public JobSeekerMenuType JobSeekerMenuType { get; set; }
        public IndividualAttentionDTO IndividualAttentionDTO { get; set; }
    }
}
