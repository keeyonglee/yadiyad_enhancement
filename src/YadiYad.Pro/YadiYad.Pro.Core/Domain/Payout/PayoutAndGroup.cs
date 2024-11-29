using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutAndGroup : BaseEntityExtension
    {
        public int PayoutGroupId { get; set; }
        public int RefTypeId { get; set; }
        public int RefId { get; set; }
    }
}
