using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Refund
{
    public enum DashboardRefundStatus
    {
        [Description("Refunding")]
        Refunding = 0,
        [Description("Refunded")]
        Refunded = 1,
        [Description("Reprocessing Refunded")]
        ReprocessingRefunded = -1
    }
}
