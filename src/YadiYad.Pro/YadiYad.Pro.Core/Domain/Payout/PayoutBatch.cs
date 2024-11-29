using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Core.Domain.Payout
{
    public class PayoutBatch : BaseEntityExtension
    {
        public DateTime GeneratedDateTime { get; set; }
        public DateTime? DownloadDateTime { get; set; }
        public DateTime? ReconDateTime { get; set; }
        public int Status { get; set; }
        public string StatusRemarks { get; set; }
        public int? ReconFileDownloadId { get; set; }

        public int PlatformId { get; set; }

        public Platform Platform
        {
            get
            {
                return (Platform)PlatformId;
            }

            set
            {
                PlatformId = (int)value;
            }
        }
    }
}
