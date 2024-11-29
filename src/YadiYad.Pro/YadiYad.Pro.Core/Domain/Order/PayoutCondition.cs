using Nop.Core;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Core.Domain.Order
{

    public enum PayoutCondition
    {
        None = 0,
        LessThan24Hours = 1,
        MoreThen24HoursLessThan72Hours = 2,
        MoreThan72Hours = 3,
    }
}