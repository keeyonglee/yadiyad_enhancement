using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nop.Core.Domain.ShippingShuq.DTO;

namespace QB.Shipping.JnT.Models
{
    public class TrackingResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("data")]
        public List<Data> TrackingData { get; set; } = new List<Data>();
    }
    public class Data
    {
        [JsonProperty("billcode")]
        public string BillCode { get; set; }
        [JsonProperty("details")]
        public List<Details> TrackingDetails { get; set; } = new List<Details>();
        [JsonProperty("orderDetail")]
        public OrderDetail OrderDetail { get; set; }

        public ShipmentDetailDTO GetShipmentDetail()
        {
            var carrierShippingStatus= TrackingDetails
                .OrderByDescending(s => s.AcceptTime)
                .First()
                .GetCarrierShippingStatus;
            
            return new ShipmentDetailDTO
            {
                CurrentStatus = carrierShippingStatus,
                ShippingCost = OrderDetail?.ShippingFee ?? 0
            };
        }
    }
    public class Details
    {
        [JsonProperty("remark")]
        public string Remark { get; set; }
        [JsonProperty("acceptTime")]
        public string AcceptTime { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("scanstatus")]
        public string ScanStatus { get; set; }
        [JsonProperty("entrySiteCode")]
        public string EntrySiteCode { get; set; }
        [JsonProperty("longitude")]
        public string Longitude { get; set; }
        [JsonProperty("latitude")]
        public string Latitude { get; set; }
        [JsonProperty("weight")]
        public string Weight { get; set; }
        
        public CarrierShippingStatus GetCarrierShippingStatus
            => ScanStatus?.ToUpper() switch
            {
                "PICKED UP" => CarrierShippingStatus.PickedUp,
                "DELIVERED" => CarrierShippingStatus.Delivered,
                "RETURNED" => CarrierShippingStatus.Returned,
                _ => CarrierShippingStatus.InProcess,
            };
    }
    
    public class OrderDetail
    {
        [JsonProperty("shippingfee")]
        public decimal ShippingFee { get; set; }
    }
}
