using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QB.Shipping.JnT.Models
{
    public class ConsignmentNoteRequest
    {
        [JsonProperty("data_digest")]
        public string Signature { get; set; }
        [JsonProperty("msg_type")]
        public string MessageType { get; set; }
        [JsonProperty("logistics_interface")]
        public ConsignmentNoteInterface Data { get; set; } = new ConsignmentNoteInterface();
    }
    public class ConsignmentNoteInterface
    {
        [JsonProperty("account")]
        public string CustomerAccount { get; set; }
        [JsonProperty("password")]
        public string CustomerPassword { get; set; }
        [JsonProperty("customercode")]
        public string CustomerId { get; set; }
        [JsonProperty("billcode")]
        public string Airwaybill { get; set; }
    }
}
