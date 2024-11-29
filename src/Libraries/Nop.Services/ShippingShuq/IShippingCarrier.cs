using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Shipping;
using Nop.Services.Shipping.Tracking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nop.Core.Domain.ShippingShuq.DTO;

namespace Nop.Services.ShippingShuq
{
    public interface IShippingCarrier : IShipmentTracker
    {
        public string Name { get; }
        public string CompanyName { get; }
        public bool RequireTrackingNumberBarCode { get; }
        public bool RequireVendorSideTracking { get; }
        public bool RequireVendorClientSideTracking { get; }
        public bool RequireCheckoutDeliveryDateAndTimeslot { get; }
        public bool SetPreparingCreateShipment { get; }
        public bool RequireCoverageChecking { get; }
        public ShipmentCarrierReceiptDTO Ship(ShipmentCarrierDTO shipment);
        public ShipmentCarrierEstimateCostDTO GetQuotation(ShipmentCarrierDTO shipment);
        public byte[] GetConsignmentNote(string AwbNumber);
        ShipmentDetailDTO GetShipmentDetail(string trackingNumber, string marketCode = "");
        public string GetUrl(string trackingNumber, string code = "");
    }
}
