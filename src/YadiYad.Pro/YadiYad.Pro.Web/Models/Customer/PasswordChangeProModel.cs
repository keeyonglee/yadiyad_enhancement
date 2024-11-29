using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Customer
{
    public class PasswordChangeProModel : BaseNopModel
    {
        public int CustomerId { get; set; }
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        [NoTrim]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [NoTrim]
        public string ConfirmNewPassword { get; set; }
        public string Result { get; set; }
        public bool IsOnline { get; set; } = false;
        public bool IsIndividual { get; set; }
        public IList<CustomerAttributeProModel> CustomerAttributes { get; set; }

    }
}
