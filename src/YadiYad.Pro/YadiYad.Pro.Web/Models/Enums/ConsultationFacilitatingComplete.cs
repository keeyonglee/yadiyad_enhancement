using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Enums
{
    public enum ConsultationFacilitatingComplete
    {
        [Display(Name = "Paid")]
        [Description("Paid")]
        Paid = 4,
        [Display(Name = "Completed")]
        [Description("Completed")]
        Completed = 5,
        [Display(Name = "Cancelled By Organization")]
        [Description("Cancelled By Organization")]
        CancelledByOrganization = 6,
        [Display(Name = "Cancelled By Individual")]
        [Description("Cancelled By Individual")]
        CancelledByIndividual = 7
    }
}
