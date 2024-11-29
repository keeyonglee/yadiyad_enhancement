using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.DTO.Consultation;

namespace YadiYad.Pro.Web.Models.Moderator
{
    public class ModeratorConsultantRescheduleModel
    {
        public int Id { get; set; }
        public string RescheduleRemarks { get; set; }
        public DateTime Date { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
