using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class TimeZone : BaseEntity
    {
        public string Name { get; set; }
        public int Offset { get; set; }

    }
}
