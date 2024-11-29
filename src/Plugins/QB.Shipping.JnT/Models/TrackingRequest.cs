using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QB.Shipping.JnT.Models
{
    public class TrackingRequest
    {
        public string Signature { get; set; }
        public string MessageType { get; set; }
        public string ECompanyId { get; set; }
        public LogisticsInterface Details { get; set; } = new LogisticsInterface();
    }
    public class LogisticsInterface
    {
        [JsonProperty("queryType")]
        public int QueryType { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("queryCodes")]
        public string[] QueryNumber { get; set; }

    }
}
