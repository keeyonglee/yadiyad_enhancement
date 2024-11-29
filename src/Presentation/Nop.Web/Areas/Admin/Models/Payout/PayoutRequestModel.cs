using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Payout
{
    public class PayoutRequestModel : BaseNopEntityModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public int RequestType { get; set; }
        public string RequestTypeText { get; set; }
        public decimal ListedProfessionalFee { get; set; }
        public decimal PayoutCharges { get; set; }
        public decimal PayoutAmount { get; set; }
    }
}