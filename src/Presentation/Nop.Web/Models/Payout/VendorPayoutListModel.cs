using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using YadiYad.Pro.Core.Domain.Payout;

namespace Nop.Web.Models.Payout
{
    public class VendorPayoutListModel : BaseNopModel
    {
        public VendorPayoutListModel()
        {
            Payouts = new List<PayoutModel>();
            PayoutOrders = new List<PayoutOrderModel>();
        }

        public PagerModel PagerModel { get; set; }
        public IList<PayoutModel> Payouts { get; set; }
        public IList<PayoutOrderModel> PayoutOrders { get; set; }
        public decimal TotalPayout { get; set; }

        #region Nested classes

        public partial class PayoutModel : BaseNopEntityModel
        {
            public DateTime Date { get; set; }
            public int StatusId { get; set; }
            public int NumberOfOrders { get; set; }
            public string Remarks { get; set; }
            public string BankName { get; set; }
            public string BankAccount { get; set; }
            public int OrderId { get; set; }
            public int PayoutGroupId { get; set; }
            public string Amount { get; set; }
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
            public int PayoutBatchId { get; set; }
            public string PayoutBatchIdText { get; set; }
        }

        public partial class PayoutOrderModel : BaseNopEntityModel
        {
            public int OrderId { get; set; }
            public DateTime Date { get; set; }
            public string Amount { get; set; }
            public string DateText
            {
                get
                {
                    return Date.ToShortDateString();
                }
            }
        }

        #endregion
    }
}
