using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YadiYad.Pro.Core.Domain.DepositRequest;

namespace YadiYad.Pro.Services.DTO.DepositRequest
{
    public class DepositRequestDTO
    {
        public int Id { get; set; }
        public int? OrderItemId { get; set; }
        public string DepositNumber { get; set; }
        public decimal Amount { get; set; }
        public int DepositTo { get; set; }
        public int DepositFrom { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime CycleStart { get; set; }
        public DateTime CycleEnd { get; set; }
        public int Status { get; set; }
        public int ReminderCount { get; set; }
        public int ProductTypeId { get; set; }
        public int RefId { get; set; }
        public int ProductStatus { get; set; }
        public string StatusText
        {
            get
            {
                var text = "";

                text = Status == (int)DepositRequestStatus.New
                    ? DepositRequestStatus.New.GetDescription()
                    : Status == (int)DepositRequestStatus.Paid
                    ? DepositRequestStatus.Paid.GetDescription()
                    : Status == (int)DepositRequestStatus.Reminded
                    ? DepositRequestStatus.Reminded.GetDescription()
                    : "";

                return text;
            }
        }
        public string ItemName { get; set; }
        public string CustomOrderNumber{get;set;}

        //project based usage
        public int PaymentChannelId { get; set; }
        public PaymentChannel PaymentChannel 
        { 
            get
            {
                return (PaymentChannel)PaymentChannelId;
            }
            set
            {
                PaymentChannelId = (int)value;
            }
        }

        public int? BankId { get; set; }
        public string bankName { get; set; }

        public DateTime? BankInDate { get; set; }
        public string BankInReference { get; set; }

        [UIHint("Document")]
        public int? BankInSlipDownloadId { get; set; }

        public string TransfereeBankName { get; set; }
        public string TransfereeBankAccountNo { get; set; }

        public string ApproveRemarks { get; set; }

        //offset
        public decimal OffsetableAmount { get; set; }
        public string OffsetableEngagementCode { get; set; }

        public int? ServiceChargeInvoiceId { get; set; }
    }
}
