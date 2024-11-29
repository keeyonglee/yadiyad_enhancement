using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Individual
{
    public enum ProfileImageViewMode
    {
        [Display(Name = "Organization")]
        [Description("Organization")]
        Organization = 0,
        [Display(Name = "Public")]
        [Description("Public")]
        Public = 1
    }
}
