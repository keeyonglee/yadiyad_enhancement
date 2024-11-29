using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq
{
    public class ShippingLalamoveSettings : ISettings
    {
        public string TrackingUrl { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string BaseUrl { get; set; }
        public string TotalFeeCurrency { get; set; }
        public string GetQuotation { get; set; }
        public string PlaceOrder { get; set; }
        public string Market { get; set; }
        public string Motorcycle { get; set; }
        public string Car { get; set; }
        public string CoverageLimit { get; set; }
        public string CancelOrder { get; set; }
        public decimal MarkUpShippingFeePercentage { get; set; }
        public decimal CarCoverageLimit { get; set; }
        public decimal BikeCoverageLimit { get; set; }
        public List<string> AvailableStates01 { get; set; }
        public List<string> AvailableStates02 { get; set; }
        public List<string> AvailableStates03 { get; set; }
        public string MarketKL { get; set; }
        public string MarketJohorBahru { get; set; }
        public string MarketPenang { get; set; }
        public decimal MaxWeightBike { get; set; }
        public decimal MaxWeightCar { get; set; }
    }
}
