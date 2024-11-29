using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Payout
{
    public class PayoutGroupModel : BaseNopEntityModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int RequestCount { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
    }
}