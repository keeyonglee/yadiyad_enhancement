using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Moderator
{
    public class ModeratorConsultationModel
    {
        public Dictionary<string, int> Dict { get; set; }
        public Dictionary<string, int> OrganizationRating { get; set; }
        public string Remarks { get; set; }
        public string OrganizationRemarks { get; set; }
        public int Id { get; set; }
        public int CancelledBy { get; set; }
        public bool Rehire { get; set; }
        public string ModeratorRemarks { get; set; }
        public int ReasonIdBuyer { get; set; }
        public int ReasonIdSeller { get; set; }
        public string ReasonOthersBuyer { get; set; }
        public string ReasonOthersSeller { get; set; }
    }
}
