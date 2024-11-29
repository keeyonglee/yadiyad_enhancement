using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class ProEngagementSettings : ISettings
    {
        public int DaysAbleToCancel { get; set; }
        public int DaysMinActivePayToViewInviteOnCancellation { get; set; }
    }
}
