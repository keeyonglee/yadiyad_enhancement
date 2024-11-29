using System;
using System.Collections.Generic;
using System.Text;

namespace YadiYad.Pro.Services.DTO.Service
{
    public class ServiceProfileSearchFilterDTO
    {
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> ExpertiseIds { get; set; } = new List<int>();
        public int YearExperience { get; set; }
        public int ServiceTypeId { get; set; }
        public int ServiceModelId { get; set; }
        public int StateProvinceId { get; set; }
        public int ConsultationProfileId { get; set; }
        public int CustomerId { get; set; }
        public int BuyerCustomerId { get; set; }
        public int ServiceSearchSortById { get; set; }
        public string ServiceSearchSortByName { get; set; }
        public List<int> ExcludeServiceTypeIds { get; set; } = new List<int>();

    }
}
