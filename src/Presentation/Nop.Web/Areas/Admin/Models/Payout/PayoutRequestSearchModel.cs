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
    public class PayoutRequestSearchModel : BaseSearchModel
    {
        public int GroupId { get; set; }
        public int BatchId { get; set; }
        public string BatchNumber { get; set; }
        public string CustomerName { get; set; }

    }

}
