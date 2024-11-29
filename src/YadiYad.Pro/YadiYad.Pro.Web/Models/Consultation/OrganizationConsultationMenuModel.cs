using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Attentions;
using YadiYad.Pro.Web.Enums;

namespace YadiYad.Pro.Web.Models.Consultation
{
    public class OrganizationConsultationMenuModel
    {
        public ConsultationMenuType ConsultationMenuType { get; set; }
        public OrganizationAttentionDTO OrganizationAttentionDTO { get; set; }
    }
}
