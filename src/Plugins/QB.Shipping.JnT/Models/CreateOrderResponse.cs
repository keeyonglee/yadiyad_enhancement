using Newtonsoft.Json;

namespace QB.Shipping.JnT.Models
{
    public class CreateOrderResponse
    {
        [JsonProperty("awb_no")]
        public string AwbNo { get; set; }
        [JsonProperty("orderid")]
        public string OrderId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("msg")]
        public string Message { get; set; }
        [JsonProperty("data")]
        public CreateOrderResponseData Data { get; set; } = new CreateOrderResponseData();
    }
    public class CreateOrderResponseData
    {
        [JsonProperty("price")]
        public double? Price { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("businessCode")]
        public string BusinessCode { get; set; }
        [JsonProperty("insuranceFee")]
        public decimal? InsuranceFee{ get; set; }
    }
}