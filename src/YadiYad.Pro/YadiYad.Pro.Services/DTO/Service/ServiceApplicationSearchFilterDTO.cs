using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceApplicationSearchFilterDTO
    {
        public int RequesterCustomerId { get; set; }
        public int ProviderCustomerId { get; set; }
        public List<int> Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
