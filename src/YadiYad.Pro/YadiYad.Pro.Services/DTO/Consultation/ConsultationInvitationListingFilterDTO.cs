using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Organization;

namespace YadiYad.Pro.Services.DTO.Consultation
{
    public class ConsultationInvitationListingFilterDTO
    {
        public int IndividualCustomerId { get; set; }
        public int OrganizationProfileId { get; set; }
        public int ServiceProfileId { get; set; }
        public List<int> ConsultationInvitationStatuses { get; set; }
    }
}