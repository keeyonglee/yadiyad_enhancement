using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Payout;

namespace Nop.Web.Areas.Admin.Models.Payout
{
    public class PayoutBatchModel : BaseNopEntityModel
    {
        public int Id { get; set; }
        public DateTime GeneratedDateTime { get; set; }
        public DateTime? DownloadDateTime { get; set; }
        public DateTime? ReconDateTime { get; set; }
        public int Status { get; set; }
        public Platform Platform { get; set; }
        public string PlatformText { get; set; }
        public string StatusText { get; set; }
        public string StatusRemarks { get; set; }
        [UIHint("Download")]
        [Required]
        public int? ReconFileDownloadId { get; set; }
        public Guid? ReconFileDownloadGuid { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public int PayoutGroupCount { get; set; }
        public decimal Amount { get; set; }
        public string PayoutBatchNumber { get; set; }
    }
}
