using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QB.Shipping.LalaMove.Models
{
    public class GetQuotationResponse
    {
        [JsonProperty("totalFee")]
        public decimal TotalFee { get; set; }
        [JsonProperty("totalFeeCurrency")]
        public string TotalFeeCurrency { get; set; }
    }
}
