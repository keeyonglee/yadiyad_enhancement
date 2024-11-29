using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.ApproveDepositRequest
{
    public class ApproveDepositRequestModel : BaseNopEntityModel
    {
        public DateTime? CreatedDate { get; set; }
        public string Bank { get; set; }
        [UIHint("DateNullable")]
        public DateTime? BankInDate { get; set; }
        public string BankInDateText { get; set; }
        public string Reference { get; set; }
        public decimal Total { get; set; }
        public string DepositStatus { get; set; }
        public string DepositNumber { get; set; }
        public int EngagementId { get; set; }
        [UIHint("PDFViewer")]
        public int? BankInSlipDownloadId { get; set; }
        public bool? Validity { get; set; }
        public int? OrderItemId { get; set; }
        public string ApproveRemarks { get; set; }
    }
}
