using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Common
{
    public class VendorNotificationSettings : ISettings
    {
        public int ArrangeDriverRequestAdvanceMinutes { get; set; }
        public int ArrangeDriverRequestAdvanceHours { get; set; }
    }
}
