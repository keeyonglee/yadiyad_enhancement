using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Consultation
{
    public class ConsultationFacilitatingSearchFilterDTO
    {
        public int ModeratorId { get; set; }
        public DateTime? Date { get; set; }
        public int StatusId { get; set; }
        public string Name { get; set; }
        public bool IncludeModeratorEmail { get; set; }
    }
}
