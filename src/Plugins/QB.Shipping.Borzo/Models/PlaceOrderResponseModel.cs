using System.Collections.Generic;
using Newtonsoft.Json;

namespace QB.Shipping.Borzo.Models
{
    public class PlaceOrderResponseModel
    {
        [JsonProperty("order_id")]
        public int? OrderId { get; set; }
        [JsonProperty("delivery_fee_amount")]
        public decimal DeliveryFeeAmount { get; set; }
        [JsonProperty("vehicle_type_id")]
        public decimal VehicleTypeId { get; set; }
        [JsonProperty("points")]
        public List<PointResponse> Points { get; set; }
    }

    public class PointResponse
    {
        [JsonProperty("tracking_url")]
        public string TrackingUrl { get; set; }
    }
}