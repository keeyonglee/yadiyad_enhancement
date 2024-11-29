using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Documents
{
    public class RunningNumber: BaseEntity
    {
        public string Name { get; set; }
        public int LastId { get; set; }
        public int LastYear { get; set; }
        public int RunningNumberTypeId { get; set; }
        public int? CustomerId { get; set; }
        public int? VendorId { get; set; }

        public RunningNumberType RunningNumberType
        {
            get => (RunningNumberType)RunningNumberTypeId;
            set => RunningNumberTypeId = (int)value;
        }
    }
}
