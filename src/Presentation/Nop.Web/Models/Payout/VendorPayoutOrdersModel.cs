using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Models.Payout
{
    public class VendorPayoutOrdersModel : BaseNopEntityModel
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string DateText
        {
            get
            {
                return Date.ToShortDateString();
            }
        }
    }
}
