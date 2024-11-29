using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.ShippingShuq.DTO
{
    public class ShipmentCarrierReceiptDTO
    {
        public string TrackingNumber { get; set; }
        public decimal Price { get; set; }
        public decimal InsuranceFee { get; set; }
        public string ShippingMethod { get; set; }
        public string MarketCode { get; set; }
        public decimal PriceRoundUp 
        { 
            get 
            {
                return decimal.Round(Price);
            }
        }
        public decimal InsuranceFeeRoundUp
        {
            get
            {
                return decimal.Round(InsuranceFee);
            }
        }
    }
}
