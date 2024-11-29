using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Order
{
   
    public enum OrderCancellationReason
    {
        [Display(Name = "Change address")]
        [Description("Need to change delivery address")]
        Address = 1,
        [Display(Name = "Unresponsive to inquiries")]
        [Description("Seller is not responsive to my inquiries")]
        Inquiries = 2,
        [Display(Name = "Modify order")]
        [Description("Modify existing order")]
        Modify = 3,
        [Display(Name = "Others")]
        [Description("Others / Change of mind")]
        Others = 4,
    }
}
