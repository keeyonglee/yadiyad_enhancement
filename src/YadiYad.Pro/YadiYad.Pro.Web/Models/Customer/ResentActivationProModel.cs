using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Customer
{
    public class ResentActivationProModel : BaseNopModel
    {
        public string Email { get; set; }
        public string Result { get; set; }
        public bool DisplayCaptcha { get; set; }

    }
}
