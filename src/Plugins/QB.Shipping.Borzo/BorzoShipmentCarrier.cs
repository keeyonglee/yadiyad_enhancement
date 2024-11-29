using System;
using System.Collections.Generic;
using System.Threading;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Services.Shipping.Tracking;
using Nop.Services.ShippingShuq;
using QB.Shipping.Borzo.Models;

namespace QB.Shipping.Borzo
{
    public class BorzoShipmentCarrier: IShippingCarrier
    {
        #region Fields

        private readonly ShippingBorzoSettings _shippingBorzoSettings;
        private readonly BorzoService _borzoService;

        #endregion

        #region Ctor

        public BorzoShipmentCarrier(ShippingBorzoSettings shippingBorzoSettings,
            BorzoService borzoService)
        {
            _shippingBorzoSettings = shippingBorzoSettings;
            _borzoService = borzoService;
        }

        #endregion

        #region Utilities

        private string BuildAddressString(string address1, string address2, string city, string postcode, string state)
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
            if (!String.IsNullOrEmpty(postcode))
            {
                addressString += ", " + postcode;
            }
            if (!String.IsNullOrEmpty(state))
            {
                addressString += ", " + state;
            }
            
            return addressString;
        }

        private PlaceOrderRequestModel MapShipping(ShipmentCarrierDTO shipment)
        {
            var model = new PlaceOrderRequestModel
            {
                Points = new List<Point>(),
                Matter =  $"{_shippingBorzoSettings.Matter} - Order Id: {shipment.OrderId.ToString()}",
                VehicleTypeId = shipment.DeliveryModeId == (int)DeliveryMode.Car ? _shippingBorzoSettings.Car : _shippingBorzoSettings.Motorcycle,
                TotalWeightKg = Decimal.ToInt32(shipment.TotalWeight),
                IsClientNotificationEnabled = _shippingBorzoSettings.IsClientNotificationEnabled,
                IsContactPersonNotificationEnabled = _shippingBorzoSettings.IsContactPersonNotificationEnabled
            };
            var start = new Point
            {
                ContactPerson = new ContactPerson
                {
                    Phone = shipment.SenderPhone,
                    Name = shipment.SenderName
                },
                Address = BuildAddressString(shipment.SenderAddress1,
                    shipment.SenderAddress2,
                    shipment.SenderCity,
                    shipment.SenderZip,
                    shipment.SenderState),
                RequiredStartDatetime = !String.IsNullOrEmpty(shipment.ScheduleAt) ? shipment.ScheduleAt : DateTime.UtcNow.ToString("o")
            };
            var end = new Point
            {
                ContactPerson = new ContactPerson
                {
                    Phone = shipment.ReceiverPhone,
                    Name = shipment.ReceiverName
                },
                Address = BuildAddressString(shipment.ReceiverAddress1,
                    shipment.ReceiverAddress2,
                    shipment.ReceiverCity,
                    shipment.ReceiverZip,
                    shipment.ReceiverState)
            };
            model.Points.Add(start);
            model.Points.Add(end);
            return model;
        }

        #endregion

        #region Methods

        public const string CarrierName = "Borzo";
        public string Name => CarrierName;
        public string CompanyName => "Borzo";
        public bool RequireTrackingNumberBarCode => false;
        public bool RequireVendorSideTracking => false;
        public bool RequireVendorClientSideTracking => true;
        public bool RequireCheckoutDeliveryDateAndTimeslot => true;
        public bool SetPreparingCreateShipment => true;
        public bool RequireCoverageChecking => true;
        
        
        
        public ShipmentCarrierReceiptDTO Ship(ShipmentCarrierDTO shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException($"{shipment} cannot be null");
            
            var model = MapShipping(shipment);
            
            var placeOrder
                = _borzoService
                    .GetAsync<ValidResponse<PlaceOrderResponseModel>>( _shippingBorzoSettings.PlaceOrder, model, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();

            if (placeOrder.IsSuccessful)
            {
                return new ShipmentCarrierReceiptDTO
                {
                    TrackingNumber = placeOrder.Order.OrderId.ToString(),
                    Price = placeOrder.Order.DeliveryFeeAmount,
                    ShippingMethod = Name
                };
            }

            return null;
        }

        public ShipmentCarrierEstimateCostDTO GetQuotation(ShipmentCarrierDTO shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException(nameof(shipment));
            
            var result = new ShipmentCarrierEstimateCostDTO();

            //weight checking
            if (shipment.DeliveryModeId == (int)DeliveryMode.Car && shipment.TotalWeight > _shippingBorzoSettings.MaxWeightCar)
            {
                result.IsSuccess = false;
                result.IsInsideCoverage = false;
                result.IsOverWeight = true;
                result.IsOverWeightMessage = $"Total product's weight is over the limit of {_shippingBorzoSettings.MaxWeightCar}kg. Please reduce product quantity.";
                return result;
            }
            else if (shipment.DeliveryModeId == (int)DeliveryMode.Bike && shipment.TotalWeight > _shippingBorzoSettings.MaxWeightMotorcycle)
            {
                result.IsSuccess = false;
                result.IsInsideCoverage = false;
                result.IsOverWeight = true;
                result.IsOverWeightMessage = $"Total product's weight is over the limit of {_shippingBorzoSettings.MaxWeightMotorcycle}kg. Please reduce product quantity.";
                return result;
            }

            var model = MapShipping(shipment);
            
            var getQuotation
                = _borzoService
                    .GetAsync<ValidResponse<PlaceOrderResponseModel>>( _shippingBorzoSettings.GetQuotation, model, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();
            
            if (getQuotation.IsSuccessful)
            {
                decimal coverageLimitPrice = 0;

                if (getQuotation.Order.VehicleTypeId == _shippingBorzoSettings.Motorcycle)
                {
                    coverageLimitPrice = _shippingBorzoSettings.MaxDeliveryFeeMotorcycle;
                }
                else
                {
                    coverageLimitPrice = _shippingBorzoSettings.MaxDeliveryFeeCar;
                }

                if (getQuotation.Order.DeliveryFeeAmount <= coverageLimitPrice)
                {
                    result.IsSuccess = true;
                    result.IsInsideCoverage = true;
                    result.EstimatePrice = getQuotation.Order.DeliveryFeeAmount;
                    result.MarkUpPercentage = _shippingBorzoSettings.MarkUpShippingFeePercentage;
                }
                else
                {
                    result.IsSuccess = false;
                    result.IsInsideCoverage = false;
                    result.EstimatePrice = getQuotation.Order.DeliveryFeeAmount;
                }
            }

            return result;
        }
        
        public string GetUrl(string trackingNumber, string code = "")
        {
            if (!Int32.TryParse(trackingNumber, out int orderId))
            {
                throw new ArgumentOutOfRangeException($"{trackingNumber} cannot convert to integer");
            }
            var result
                = _borzoService
                    .GetOrdersAsync<GetOrdersValidResponse<PlaceOrderResponseModel>>( _shippingBorzoSettings.GetOrders, orderId, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();
            if (code == _shippingBorzoSettings.VendorUrl)
            {
                return result.Orders[0].Points[0].TrackingUrl;
            }
            return result.Orders[0].Points[1].TrackingUrl;
        }

        public byte[] GetConsignmentNote(string AwbNumber)
        {
            return new byte[1];
        }

        public ShipmentDetailDTO GetShipmentDetail(string trackingNumber, string marketCode = "")
        {
            return new ShipmentDetailDTO();
        }

        public bool IsMatch(string trackingNumber)
        {
            return true;
        }

        public string GetUrl(string trackingNumber)
        {
            return string.Empty;
        }

        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            return new List<ShipmentStatusEvent>();
        }

        #endregion
        
    }
}