using Nop.Web.Framework.Models;
using System;
using YadiYad.Pro.Core.Domain.Payout;

namespace Nop.Web.Models.Payout
{
    public class VendorPayoutDetailsModel : BaseNopEntityModel
    {
        public DateTime Date { get; set; }
        public int StatusId { get; set; }
        public string Amount { get; set; }
        public int NumberOfOrders { get; set; }
        public string Remarks { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public int OrderId { get; set; }
        public int PayoutGroupId { get; set; }
        public int PayoutBatchId { get; set; }
        public string PayoutBatchIdText { get; set; }
        public string DateText
        {
            get
            {
                return Date.ToShortDateString();
            }
        }
        public string StatusText
        {
            get
            {
                var payoutEnum = (PayoutGroupStatus)StatusId;
                return payoutEnum.ToString();
            }
        }
    }
}
