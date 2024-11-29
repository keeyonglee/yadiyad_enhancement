using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Linq;
using Nop.Core.Domain.ShippingShuq.DTO;

namespace QB.Shipping.JnT.Models
{
    public class BaseResponse : ErrorResponse
    {
        [JsonProperty("logisticproviderid")]
        public string LogisticsProviderId { get; set; }
        [JsonProperty("responseitems")]
        public List<BaseResponseItem> Items { get; set; } = new List<BaseResponseItem>();
    }

    public class BaseResponseItem
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("mailno")]
        public string MailNo { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("txlogisticid")]
        public int LogisticId { get; set; }
        [JsonProperty("logisticproviderid")]
        public int LogisticProviderId { get; set; }
    }

    public class ValidResponse<T> : ValidResponse
    {
        [JsonProperty("details")]
        public List<T> Details { get; set; } = new List<T>();
    }

    public class ValidResponse : BaseResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
    }

    public class TrackingValidResponse : BaseResponse
    {
        [JsonProperty("logisticproviderid")]
        public string Logisticproviderid { get; set; }
    }

    public class TrackingValidResponse<T> : TrackingValidResponse
        where T: TrackingResponse
    {
        [JsonProperty("responseitems")]
        public T ResponseItems { get; set; }

        public ShipmentDetailDTO GetShipmentDetail(string awbNumber)
        {
            var orderItem = ResponseItems?
                .TrackingData?
                .FirstOrDefault(s => s.BillCode == awbNumber);

            if (orderItem == null)  
                return null;

            return orderItem.GetShipmentDetail();
        }

        public Dictionary<string, ShipmentDetailDTO> GetShipmentDetails(string[] awbNumbers)
        {
            var shipmentDetails = new Dictionary<string, ShipmentDetailDTO>();
            foreach (var awbNumber in awbNumbers)
            {
                var orderItem = ResponseItems?
                    .TrackingData?
                    .FirstOrDefault(s => s.BillCode == awbNumber);

                if (orderItem == null)
                    continue;

                shipmentDetails.Add(awbNumber, orderItem.GetShipmentDetail());
            }
            return shipmentDetails;
        }
    }

    public class ErrorResponse
    {
        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}