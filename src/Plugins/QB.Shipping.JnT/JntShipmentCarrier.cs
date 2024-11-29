using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using QB.Shipping.JnT.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Nop.Services.ShippingShuq;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.Logging;
using StackExchange.Profiling.Internal;

namespace QB.Shipping.JnT
{
    public class JntShipmentCarrier : IShippingCarrier
    {
        #region Fields

        private readonly JntService _jntService;
        private readonly ShippingJntSettings _shippingJntSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public JntShipmentCarrier(
         JntService jntService,
         ShippingJntSettings shippingJntSettings,
         ILogger logger)
        {
            _jntService = jntService;
            _shippingJntSettings = shippingJntSettings;
            _logger = logger;
        }

        #endregion

        #region Utilities

        private string BuildAddressString(string address1, string address2, string city, string state)
        {
            var addressString = address1;
            if (!String.IsNullOrEmpty(address2))
            {
                addressString += ", " + address2;
            }
            if (!String.IsNullOrEmpty(city))
            {
                addressString += ", " + city;
            }
            if (!String.IsNullOrEmpty(state))
            {
                addressString += ", " + state;
            }

            return addressString;
        }

        private OrderRequestDetails MapShipping(ShipmentCarrierDTO shipment)
        {
            var shippingDetails = new OrderRequestDetails
            {
                ShipperName = shipment.SenderName,
                ShipperAddress = BuildAddressString(shipment.SenderAddress1, shipment.SenderAddress2, shipment.SenderCity, shipment.SenderState),
                ShipperContact = shipment.SenderName,
                ShipperPhone = shipment.SenderPhone,
                SenderZip = shipment.SenderZip,
                ReceiverName = shipment.ReceiverName,
                ReceiverAddress = BuildAddressString(shipment.ReceiverAddress1, shipment.ReceiverAddress2, shipment.ReceiverCity, shipment.ReceiverState),
                ReceiverZip = shipment.ReceiverZip,
                ReceiverPhone = shipment.ReceiverPhone,
                Weight = shipment.TotalWeight.ToString(),
                ItemName = shipment.ProductName,
                GoodsDesc = shipment.Remarks,
                GoodsValue = shipment.TotalValue,
                OfferFeeFlag = shipment.RequireInsurance ? 1 : 0,
                Quantity = shipment.Quantity.ToString(),
                ServiceType = _shippingJntSettings.ServiceType,
                GoodsType = _shippingJntSettings.GoodsType,
                PayType = _shippingJntSettings.PayType,
                CustomerCode = _shippingJntSettings.CustomerCode,
                Username = _shippingJntSettings.Username,
                ApiKey = _shippingJntSettings.ApiKey,
                Password = _shippingJntSettings.Password
            };
            return shippingDetails;
        }

        #endregion

        #region Methods

        public const string CarrierName = "Jnt";

        public string Name => CarrierName;
        public string CompanyName => "J&T Express";
        public bool RequireTrackingNumberBarCode => true;
        public bool RequireVendorSideTracking => false;
        public bool RequireVendorClientSideTracking => false;
        public bool RequireCheckoutDeliveryDateAndTimeslot => false;
        public bool SetPreparingCreateShipment => false;
        public bool RequireCoverageChecking => false;

        public byte[] GetConsignmentNote(string AwbNumber)
        {
            return new byte[0];
        }

        public ShipmentDetailDTO GetShipmentDetail(string trackingNumber, string marketCode = "")
        {
            try
            {
                var tracking = new Tracking
                {
                    Language = "2",
                    QueryType = 1,
                    QueryNumber = new string[] { trackingNumber }
                };
                // var tracking = new Tracking {QueryNumber = new[] {trackingNumber}};
                var response = _jntService
                    .TrackingAsync<TrackingValidResponse<TrackingResponse>>(tracking, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();

                return response?.GetShipmentDetail(trackingNumber);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error Processing Request for Jnt Shipment {trackingNumber}", ex);
                return default;
            }
        }

        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            return new List<ShipmentStatusEvent>();
        }

        public string GetUrl(string trackingNumber, string code = "")
        {
            return _shippingJntSettings.TrackingUrl + trackingNumber;
        }

        public bool IsMatch(string trackingNumber)
        {
            return true;
        }

        public ShipmentCarrierEstimateCostDTO GetQuotation(ShipmentCarrierDTO shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (shipment.TotalWeight > _shippingJntSettings.MaxWeight)
            {
                return new ShipmentCarrierEstimateCostDTO
                {
                    IsSuccess = false,
                    IsInsideCoverage = true,
                    IsOverWeight = true,
                    IsOverWeightMessage = $"Total product's weight is over the limit of {_shippingJntSettings.MaxWeight}kg. Please reduce product quantity."
                };
            }

            var shippingDetails = MapShipping(shipment);
            shippingDetails.OrderId = $"YD-{DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss")}-{new Random().Next(1, 100000)}";

            var response = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shippingDetails, new CancellationToken())
                .GetAwaiter()
                .GetResult();


            if (response.Details[0].Data == null)
            {
                return new ShipmentCarrierEstimateCostDTO
                {
                    IsSuccess = false,
                    IsInsideCoverage = true,
                };
            }
            else if (response.Details[0].Data.Price > 0)
            {
                return new ShipmentCarrierEstimateCostDTO
                {
                    EstimatePrice = (decimal)response.Details[0].Data.Price,
                    IsSuccess = true,
                    MarkUpPercentage = _shippingJntSettings.MarkUpShippingFeePercentage,
                    IsInsideCoverage = true
                };
            }
            else
            {
                return new ShipmentCarrierEstimateCostDTO
                {
                    IsSuccess = false,
                    IsInsideCoverage = true,
                };
            }
        }

        public ShipmentCarrierReceiptDTO Ship(ShipmentCarrierDTO shipment)
        {
            var shippingDetails = MapShipping(shipment);
            shippingDetails.OrderId = $"{shipment.OrderId} - {new Random().Next(1, 100000)}";

            var response = _jntService
                .CreateOrderAsync<ValidResponse<CreateOrderResponse>>(shippingDetails, new CancellationToken())
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            if (response.Details[0].Data.Price > 0)
            {
                return new ShipmentCarrierReceiptDTO
                {
                    TrackingNumber = response.Details[0].AwbNo,
                    InsuranceFee = response.Details[0].Data.InsuranceFee != null ? response.Details[0].Data.InsuranceFee.Value : 0,
                    Price = (decimal)response.Details[0].Data.Price,
                    ShippingMethod = Name
                };
            }
            else
            {
                return null;
            }
        }

        byte[] IShippingCarrier.GetConsignmentNote(string AwbNumber)
        {
            return _jntService.CreateConsignmentPdfAsync(AwbNumber, new CancellationToken())
              .GetAwaiter()
              .GetResult();
        }

        public string GetUrl(string trackingNumber)
        {
            return "";
        }

        #endregion
    }
}
