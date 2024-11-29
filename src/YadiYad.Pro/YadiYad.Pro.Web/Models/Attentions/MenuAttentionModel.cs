using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Attentions;

namespace YadiYad.Pro.Web.Models.Attentions
{
    public class MenuAttentionModel
    {
        public IndividualAttentionDTO IndividualAttentionDTO { get; set; } 
        public OrganizationAttentionDTO OrganizationAttnetionDTO { get; set; }

        public MenuAttentionModel()
        {
            IndividualAttentionDTO = new IndividualAttentionDTO();
            OrganizationAttnetionDTO = new OrganizationAttentionDTO();
        }
    }
}
