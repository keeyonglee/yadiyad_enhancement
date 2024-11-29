using Newtonsoft.Json;

namespace QB.Shipping.JnT.Models
{
    public class OrderRequest
    {
        [JsonProperty("detail")]
        public OrderRequestDetails Details { get; set; } = new OrderRequestDetails();
    }
    public class OrderRequestDetails
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
        [JsonProperty("orderid")]
        public string OrderId { get; set; }
        [JsonProperty("shipper_name")]
        public string ShipperName { get; set; }
        [JsonProperty("shipper_addr")]
        public string ShipperAddress { get; set; }
        [JsonProperty("shipper_contact")]
        public string ShipperContact { get; set; }
        [JsonProperty("shipper_phone")]
        public string ShipperPhone { get; set; }
        [JsonProperty("sender_name")]
        public string SenderName { get; set; }
        [JsonProperty("sender_zip")]
        public string SenderZip { get; set; }
        [JsonProperty("receiver_name")]
        public string ReceiverName { get; set; }
        [JsonProperty("receiver_addr")]
        public string ReceiverAddress { get; set; }
        [JsonProperty("receiver_zip")]
        public string ReceiverZip { get; set; }
        [JsonProperty("receiver_phone")]
        public string ReceiverPhone { get; set; }
        [JsonProperty("qty")]
        public string Quantity { get; set; }
        [JsonProperty("weight")]
        public string Weight { get; set; }
        [JsonProperty("servicetype")]
        public string ServiceType { get; set; }
        [JsonProperty("item_name")]
        public string ItemName { get; set; }
        [JsonProperty("goodsdesc")]
        public string GoodsDesc { get; set; }
        [JsonProperty("goodsvalue")]
        public decimal GoodsValue { get; set; }
        [JsonProperty("goodsType")]
        public string GoodsType { get; set; }
        [JsonProperty("payType")]
        public string PayType { get; set; }
        [JsonProperty("offerFeeFlag")]
        public int OfferFeeFlag { get; set; }
        [JsonProperty("cuscode")]
        public string CustomerCode { get; set; }
        [JsonProperty("expresstype")]
        public string ExpressType { get; set; }
        [JsonProperty("height")]
        public string Height { get; set; }
        [JsonProperty("length")]
        public string Length { get; set; }
        [JsonProperty("width")]
        public string Width { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }

}