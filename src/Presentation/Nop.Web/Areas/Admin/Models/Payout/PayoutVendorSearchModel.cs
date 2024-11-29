using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Payout
{
    public class PayoutVendorSearchModel : BaseSearchModel
    {
        public int PayoutGroupId { get; set; }
    }
}
