using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Moderator
{
    public class ModeratorConsultationCancelModel
    {
        public int Id { get; set; }
        public string CancelledBy { get; set; }
        public int CancelledById { get; set; }
        public int ReasonIdBuyer { get; set; }
        public int ReasonIdSeller { get; set; }
        public string ReasonOthersBuyer { get; set; }
        public string ReasonOthersSeller { get; set; }
        public int ReasonId { get; set; }
        public string ReasonRemarks { get; set; }
        public bool Rehire { get; set; }
        public string ModeratorRemarks { get; set; }
    }
}
