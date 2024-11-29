using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YadiYad.Pro.Core.Domain.Payout;

namespace YadiYad.Pro.Services.DTO.Payout
{
    public class PayoutBatchDTO
    {
        public int Id { get; set; }
        public DateTime GeneratedDateTime { get; set; }
        public DateTime? DownloadDateTime { get; set; }
        public DateTime? ReconDateTime { get; set; }
        public int Status { get; set; }
        public string StatusRemarks { get; set; }
        public int? ReconFileDownloadId { get; set; }
        public Guid? ReconFileDownloadGuid { get; set; }
        public int PayoutGroupCount { get; set; }
        public decimal Amount { get; set; }
        public string PayoutBatchNumber { get; set; }
        public int PlatformId { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime? UpdatedOnUTC { get; set; }
        public Platform Platform { 
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
