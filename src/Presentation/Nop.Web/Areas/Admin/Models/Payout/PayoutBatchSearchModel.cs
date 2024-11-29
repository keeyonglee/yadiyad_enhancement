using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;

namespace Nop.Web.Areas.Admin.Models.Payout
{
    public class PayoutBatchSearchModel : BaseSearchModel
    {
        public PayoutBatchSearchModel()
        {
            StatusList = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Generated Date")]
        [UIHint("DateNullable")]
        public DateTime? GeneratedDate { get; set; }

        [NopResourceDisplayName("User")]
        public List<int> CustomerIds { get; set; }

        [NopResourceDisplayName("Status")]
        public int? Status { get; set; }

        public IList<SelectListItem> StatusList { get; set; }


    }

}
