using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.ApproveDepositRequest
{
    public class ApproveDepositRequestSearchModel : BaseSearchModel
    {
        [UIHint("DateNullable")]
        public DateTime? From { get; set; }
        [UIHint("DateNullable")]
        public DateTime? Until { get; set; }
        public int? StatusId { get; set; }
    }
}
