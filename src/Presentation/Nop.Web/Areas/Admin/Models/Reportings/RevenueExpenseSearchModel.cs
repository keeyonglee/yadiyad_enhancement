using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Reportings
{
    public class RevenueExpenseSearchModel : BaseSearchModel
    {
        [Display(Name = "Created From")]
        [UIHint("DateNullable")]
        public DateTime? CreatedFrom { get; set; }
        [Display(Name = "Created To")]
        [UIHint("DateNullable")]
        public DateTime? CreatedTo { get; set; }
    }
}
