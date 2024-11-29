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
    public class PayoutGroupSearchModel : BaseSearchModel
    {
        public PayoutGroupSearchModel()
        {
            StatusList = new List<SelectListItem>();
            CustomerList = new List<SelectListItem>();
        }

        public int BatchId { get; set; }
        public string BatchNumber { get; set; }

        [NopResourceDisplayName("User")]
        public int? CustomerId { get; set; }

        [NopResourceDisplayName("Status")]
        public int? Status { get; set; }

        public IList<SelectListItem> StatusList { get; set; }

        public IList<SelectListItem> CustomerList { get; set; }

    }

}
