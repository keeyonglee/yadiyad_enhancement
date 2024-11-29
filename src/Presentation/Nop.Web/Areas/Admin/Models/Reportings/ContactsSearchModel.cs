using Nop.Web.Framework.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Reportings
{
    public class ContactsSearchModel : BaseSearchModel
    {
        [Display(Name = "Created From")]
        [UIHint("DateNullable")]
        public DateTime? CreatedFrom { get; set; }
        [Display(Name = "Created To")]
        [UIHint("DateNullable")]
        public DateTime? CreatedTo { get; set; }
    }
}
