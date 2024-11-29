using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.ProOrder
{
    public class ProOrderModel : BaseNopEntityModel
    {
        public int CustomerId { get; set; }
        public decimal OrderTotal { get; set; }
        public string PaymentStatus { get; set; }
        public int PaymentStatusId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? PaidOnUTC { get; set; }
        public string Email { get; set; }
    }
}
