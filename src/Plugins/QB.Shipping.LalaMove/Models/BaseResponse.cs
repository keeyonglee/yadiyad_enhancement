using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.ShippingShuq.DTO;

namespace QB.Shipping.LalaMove.Models
{
    public class BaseResponse : ErrorResponse
    {
        [JsonProperty("orderRef")]
        public string OrderRef { get; set; }
        
    }

    public class ErrorResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class ValidResponse : BaseResponse
    {
        [JsonProperty("totalFee")]
        public string StrTotalFee 
        {
            get => TotalFee.ToString();
            set => TotalFee = decimal.TryParse(value, out decimal totalFee) ? totalFee : 0;
        }
        [JsonProperty("totalFeeCurrency")]
        public string TotalFeeCurrency { get; set; }
        
        [JsonIgnore]
        public decimal TotalFee { get; set; }
    }

    public class ValidResponse<T> : ValidResponse
    {
        [JsonProperty("driverId")]
        public string DriverId { get; set; }
        [JsonProperty("shareLink")]
        public string ShareLink { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("pod")]
        public bool? Pod { get; set; }
        [JsonProperty("price")]
        public Fee Price { get; set; }

        public CarrierShippingStatus GetCarrierShippingStatus
            => Status?.ToUpper() switch
            {
                "PICKED_UP" => CarrierShippingStatus.PickedUp,
                "COMPLETED" => CarrierShippingStatus.Delivered,
                "EXPIRED" => CarrierShippingStatus.Aborted,
                "REJECTED" => CarrierShippingStatus.Aborted,
                _ => CarrierShippingStatus.InProcess,
            };
    }
}
