using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.CancellationRequest
{
    public class CancellationRequestSearchModel : BaseSearchModel
    {
        public int EngagementId { get; set; }
        [UIHint("DateNullable")]
        public DateTime? Date { get; set; }
        public int Type { get; set; }
        public int CancelledBy { get; set; }
        public string Buyer { get; set; }
    }
}
