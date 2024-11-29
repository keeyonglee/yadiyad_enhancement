using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nop.Core.Domain.YadiyadReporting
{
    public enum ReportingContactsType
    {
        [Description("Organization")]
        [Display(Name = "Organization")]
        Organization = 1,
        [Description("Registration Only")]
        [Display(Name = "Registration Only")]
        RegistrationOnly = 2,
        [Description("Registration And Profile")]
        [Display(Name = "Registration And Profile")]
        RegistrationAndProfile = 3,
        [Description("Individual Service And Job")]
        [Display(Name = "Individual Service And Job")]
        IndividualServiceAndJob = 4,
    }
}
