using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Organization
{
    public enum CompanySize
    {
        [Description("Less than 50 employees")]
        Fifty = 1,
        [Description("51 - 100 employees")]
        Hundred = 2,
        [Description("101 - 500 employees")]
        FiveHundred = 3,
        [Description("More Than 500 employees")]
        Above = 4
    }
}
