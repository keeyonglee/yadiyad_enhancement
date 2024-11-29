using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.CancellationRequest
{
    public class CancellationRequestModel : BaseNopEntityModel
    {
        public int EngagementId { get; set; }
        public int EngagementTypeId { get; set; }
        public string EngagementType { get; set; }
        public string CancelledBy { get; set; }
        public string BuyerName { get; set; }
        public string SellerName { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public DateTime SubmissionDate { get; set; }
        [UIHint("Picture")]
        public int ReasonAttachmentDownloadId { get; set; }
        public Dictionary<int, string> EngagementDict { get; set; } = new Dictionary<int, string>();
        public string EngagementDictString { get; set; }
    }
}
