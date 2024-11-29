using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Payout;

namespace Nop.Web.Areas.Admin.Models.Payout
{
    public class PayoutVendorModel : BaseNopEntityModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int StatusId { get; set; }
        public decimal Amount { get; set; }
        public int NumberOfOrders { get; set; }
        public string Remarks { get; set; }
        [NopResourceDisplayName("Admin.Customers.BankAccount.Fields.BankName")]
        public string BankName { get; set; }
        [NopResourceDisplayName("Admin.Customers.BankAccount.Fields.AccountNumber")]
        public string BankAccount { get; set; }
        public int OrderId { get; set; }
        public int PayoutGroupId { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.ShipmentStatusEvents.Date")]
        public string DateText
        {
            get
            {
                return Date.ToShortDateString();
            }
        }
        [NopResourceDisplayName("Admin.Customers.BankAccount.Fields.Status")]
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
