using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq
{
   public class ShippingJntSettings : ISettings
    {
        public string TrackingUrl { get; set; }
        public string OrderKey { get; set; } 
        public string TrackingKey { get; set; } 
        public string BaseUrl { get; set; } 
        public string Username { get; set; }
        public string ApiKey { get; set; } 
        public string CustomerCode { get; set; } 
        public string MessageType { get; set; } 
        public string ECompanyId { get; set; }
        public string CreateOrder { get; set; } 
        public string Tracking { get; set; } 
        public string ConsignmentNote { get; set; } 
        public string ServiceType { get; set; } 
        public string GoodsType { get; set; } 
        public string PayType { get; set; } 
        public string Password { get; set; }
        public decimal MarkUpShippingFeePercentage { get; set; }
        public decimal MaxWeight { get; set; }
        public int ShipBeforeDateAdvanceDay { get; set; }
    }
}
