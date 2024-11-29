using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutRequest : BaseEntityExtension
    {
        public int OrderItemId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? JobApplicationMilestoneId { get; set; }

        public int PayoutTo { get; set; }
        public int Status { get; set; }

        public string TimeSheetJson { get; set; }
        public string WorkDesc { get; set; }
        public int? AttachmentDownloadId { get; set; }
        public int? OnsiteDuration { get; set; }
        public int? ProratedWorkDuration { get; set; }
        public string Remark { get; set; }

        public decimal Fee { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal ServiceChargeRate { get; set; }
        public int ServiceChargeType { get; set; }

        public int ProductTypeId { get; set; }
        public int RefId { get; set; }

        public int? InvoiceId { get; set; }
        public int? ServiceChargeInvoiceId { get; set; }
    }
}
