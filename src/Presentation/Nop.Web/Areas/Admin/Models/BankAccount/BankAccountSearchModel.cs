using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;

namespace Nop.Web.Areas.Admin.Models.BankAccount
{
    public class BankAccountSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Account Holder Name")]
        public string AccountHolderName { get; set; }
        public bool? IsVerified { get; set; }
    }

}
