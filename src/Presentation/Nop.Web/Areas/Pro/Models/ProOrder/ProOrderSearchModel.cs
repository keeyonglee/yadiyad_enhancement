using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.ProOrder
{
    public class ProOrderSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Orders.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.List.EndDate")]
        [UIHint("DateNullable")]
        public DateTime? EndDate { get; set; }

        public string OrderNumber { get; set; }
        [UIHint("DateNullable")]
        public DateTime? OrderDate { get; set; }
        public int PaymentStatusId { get; set; }
    }
}
