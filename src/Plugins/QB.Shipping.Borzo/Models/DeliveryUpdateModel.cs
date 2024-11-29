using System;
using Newtonsoft.Json;
using Nop.Core.Domain.ShippingShuq.DTO;

namespace QB.Shipping.Borzo.Models
{
    public class DeliveryUpdateModel
    {
        [JsonProperty("event_datetime")]
        public DateTime EventDatetime { get; set; }
        [JsonProperty("event_type")]
        public string EventType { get; set; }
        [JsonProperty("delivery")]
        public Delivery Delivery { get; set; }
        [JsonProperty("order")]
        public Delivery Order { get; set; }
    }

    public class Delivery
    {
        [JsonProperty("order_id")]
        public int BorzoOrderId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("delivery_price_amount")]
        public decimal DeliveryPriceAmount { get; set; }
        public CarrierShippingStatus GetCarrierShippingStatus => Status?.ToUpper() switch
        {
            "ACTIVE" => CarrierShippingStatus.PickedUp,
            "FINISHED" => CarrierShippingStatus.Delivered,
            "FAILED" => CarrierShippingStatus.Aborted,
            "CANCELED" => CarrierShippingStatus.Aborted,
            _ => CarrierShippingStatus.InProcess,
        };
    }

    public class Order
    {
        [JsonProperty("order_id")]
        public int BorzoOrderId { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}