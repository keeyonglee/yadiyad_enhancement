using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq.DTO
{
    public class ShipmentCarrierEstimateCostDTO
    {
        public bool IsSuccess { get; set; }
        public decimal EstimatePrice { get; set; }
        public bool IsInsideCoverage { get; set; }
        public bool IsOverWeight { get; set; }
        public string IsOverWeightMessage { get; set; }
        public decimal  MarkUpPercentage { get; set; }
        public bool CityNotAvailable { get; set; }
        public decimal EstimatedPricePlusMarkUp 
        {
            get
            {
                return decimal.Round(EstimatePrice + EstimatePrice * MarkUpPercentage, 1) ;
            }
        }
    }
}
