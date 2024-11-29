using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Attentions;
using YadiYad.Pro.Web.Enums;

namespace YadiYad.Pro.Web.Models.Services
{
    public class ServiceSellerMenuModel
    {
        public ServiceSellerMenuType ServiceSellerMenuType { get; set; }
        public IndividualAttentionDTO IndividualAttentionDTO { get; set; }
    }
}
