using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class CustomerOnlineStatusMapping : BaseEntityExtension
    {
        public int CustomerId { get; set; }
        public bool IsOnline { get; set; }
    }
}
