using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Attentions;
using YadiYad.Pro.Web.Enums;

namespace YadiYad.Pro.Web.Models.Job

{
    public class JobAdsHeaderModel
    {
        public JobAdsMenuType MenuType { get; set; }
        public int JobAdsId { get; set; }
        public OrganizationAttentionDTO OrganizationAttentionDTO { get; set; }
    }
}
