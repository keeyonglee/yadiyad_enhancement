using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Deposit
{
    public class DepositRequest : BaseEntityExtension
    {
        public int? OrderItemId { get; set; }
        public int BaseDepositNumber { get; set; }
        public string DepositNumber { get; set; }
        public decimal Amount { get; set; }
        public int DepositTo { get; set; }
        public int DepositFrom { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? CycleStart { get; set; }
        public DateTime? CycleEnd { get; set; }
        public int Status { get; set; }
        public int ReminderCount { get; set; }
        public int ProductTypeId { get; set; }
        public int RefId { get; set; }


        //project based usage
        public int PaymentChannelId { get; set; }

        public int? BankId { get; set; }
        public DateTime? BankInDate { get; set; }
        public string BankInReference { get; set; }
        public int? BankInSlipDownloadId { get; set; }
        public string ApproveRemarks { get; set; }
    }
}
