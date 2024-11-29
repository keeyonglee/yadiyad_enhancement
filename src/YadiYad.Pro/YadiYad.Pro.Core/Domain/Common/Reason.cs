using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Common
{
    public class Reason : BaseEntity
    {
        public string Name { get; set; }
        public int EngagementType { get; set; }
        public int Party { get; set; }
        public bool Published { get; set; }
        public bool OptionalRehireOrRefund { get; set; }
        public bool AllowedAfterStart { get; set; }
        public bool BlameSeller { get; set; }

        public EngagementParty EngagementParty
        {
            get => (EngagementParty)Party;
            set => Party = (int)value;
        }
    }
}
