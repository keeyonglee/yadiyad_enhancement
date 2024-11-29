using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class CommunicateLanguage : BaseEntity
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public bool Published { get; set; }
    }
}
