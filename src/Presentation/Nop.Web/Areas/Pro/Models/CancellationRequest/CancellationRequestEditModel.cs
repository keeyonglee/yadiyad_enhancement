using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.CancellationRequest
{
    public class CancellationRequestEditModel
    {
        public int EngagementId { get; set; }
        public int EngagementTypeId { get; set; }
        public string EngagementType { get; set; }
        public string SellerName { get; set; }
        [UIHint("Picture")]
        public int ReasonAttachmentDownloadId { get; set; }
        public int ReasonId { get; set; }
        public string Remarks { get; set; }
        public string BlockLast90Days { get; set; }
        public bool BlockCurrently { get; set; }
        public bool BlockSeller { get; set; }
        public int SellerId { get; set; }
        public List<SelectListItem> BlockReasonList { get; set; } = new List<SelectListItem>();

    }
}
