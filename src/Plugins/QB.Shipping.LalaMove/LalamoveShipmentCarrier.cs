using Nop.Core.Domain.Shipping;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;
using QB.Shipping.LalaMove.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Nop.Services.ShippingShuq;
using Nop.Core.Domain.ShippingShuq;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Core.Domain.Catalog;

namespace QB.Shipping.LalaMove
{
    public class LalamoveShipmentCarrier : IShippingCarrier
    {
        #region Fields

        private readonly ShippingLalamoveSettings _shippingLalamoveSettings;
        private readonly LalaMoveService _lalaMoveService;

        #endregion

        #region Ctor

        public LalamoveShipmentCarrier(
            ShippingLalamoveSettings shippingLalamoveSettings,
            LalaMoveService lalaMoveService)
        {
            _shippingLalamoveSettings = shippingLalamoveSettings;
            _lalaMoveService = lalaMoveService;
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

        private PlaceOrderModel MapShipping(ShipmentCarrierDTO shipment, string marketCode)
        {
            var model = new PlaceOrderModel();
            var pickUp = new Waypoint();
            var dropOff = new Waypoint();
            var dropOffContact = new DeliveryInfo();

            pickUp.Address.CountryCode.Market = marketCode;
            pickUp.Address.CountryCode.DisplayString = BuildAddressString(shipment.SenderAddress1,
                shipment.SenderAddress2,
                shipment.SenderCity,
                shipment.SenderZip,
                shipment.SenderState);

            dropOff.Address.CountryCode.Market = marketCode;
            dropOff.Address.CountryCode.DisplayString = BuildAddressString(shipment.ReceiverAddress1,
                shipment.ReceiverAddress2,
                shipment.ReceiverCity,
                shipment.ReceiverZip,
                shipment.ReceiverState);

            model.Market = marketCode;
            model.RequesterContact.Name = shipment.SenderName;
            model.RequesterContact.Phone = shipment.SenderPhone;
            if (!String.IsNullOrEmpty(shipment.ScheduleAt))
            {
                model.ScheduleAT = shipment.ScheduleAt;
            }
            else
            {
                model.ScheduleAT = DateTime.UtcNow.ToString("o");
            }

            dropOffContact.ToStop = 1;
            dropOffContact.ToContact.Name = shipment.ReceiverName;
            dropOffContact.ToContact.Phone = shipment.ReceiverPhone;
            dropOffContact.Remarks = shipment.Remarks != null ? shipment.Remarks : "";

            model.Stops.Add(pickUp);
            model.Stops.Add(dropOff);
            model.Deliveries.Add(dropOffContact);

            if (shipment.DeliveryModeId == (int)DeliveryMode.Car)
            {
                model.ServiceType = _shippingLalamoveSettings.Car;
            }
            else
            {
                model.ServiceType = _shippingLalamoveSettings.Motorcycle;
            }

            return model;
        }

        private (List<string>, string) CheckVendorFromActiveStates(string vendorState)
        {
            var states1 = _shippingLalamoveSettings.AvailableStates01;
            var states2 = _shippingLalamoveSettings.AvailableStates02;
            var states3 = _shippingLalamoveSettings.AvailableStates03;

            if (states1.Contains(vendorState))
            {
                return (states1, _shippingLalamoveSettings.MarketKL);
            }
            else if (states2.Contains(vendorState))
            {
                return (states2, _shippingLalamoveSettings.MarketJohorBahru);
            }
            else if (states3.Contains(vendorState))
            {
                return (states3, _shippingLalamoveSettings.MarketPenang);
            }
            else
            {
                return (null, null);
            }
        }

        private bool CheckBuyerFromVendorStates(string buyerState, List<string> vendorStates)
        {
            var isTrue = false;
            if (vendorStates.Contains(buyerState))
            {
                isTrue = true;
            }
            return isTrue;
        }

        #endregion

        #region Methods

        public const string CarrierName = "Lalamove";
        public string Name => CarrierName;
        public string CompanyName => "Lalamove";
        public bool RequireTrackingNumberBarCode => false;
        public bool RequireVendorSideTracking => true;
        public bool RequireVendorClientSideTracking => false;
        public bool RequireCheckoutDeliveryDateAndTimeslot => true;
        public bool SetPreparingCreateShipment => true;
        public bool RequireCoverageChecking => true;
        public byte[] GetConsignmentNote(string AwbNumber)
        {
            return new byte[0];
        }

        public ShipmentDetailDTO GetShipmentDetail(string trackingNumber, string vendorMarket = "")
        {
            var response = _lalaMoveService.GetOrderDetailsAsync<ValidResponse<PlaceOrderModel>>(trackingNumber, vendorMarket, new CancellationToken())
                .GetAwaiter()
                .GetResult();
            return new ShipmentDetailDTO
            {
                CurrentStatus = response.GetCarrierShippingStatus,
                ShippingCost = response.Price.Amount
            };
        }

        public IList<ShipmentStatusEvent> GetShipmentEvents(string trackingNumber)
        {
            return new List<ShipmentStatusEvent>();
        }

        public string GetUrl(string trackingNumber, string code = "")
        {
            var result = _lalaMoveService.GetOrderDetailsAsync<ValidResponse<PlaceOrderModel>>(trackingNumber, code, new CancellationToken())
                .GetAwaiter()
                .GetResult();

            return result.ShareLink;
        }

        public bool IsMatch(string trackingNumber)
        {
            return true;
        }

        public ShipmentCarrierEstimateCostDTO GetQuotation(ShipmentCarrierDTO shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException($"{nameof(shipment)} cannot be null.");

            if (String.IsNullOrEmpty(shipment.SenderState))
                throw new ArgumentNullException($"{nameof(shipment.SenderState)} cannot be null.");

            if (String.IsNullOrEmpty(shipment.ReceiverState))
                throw new ArgumentNullException($"{nameof(shipment.ReceiverState)} cannot be null.");

            var result = new ShipmentCarrierEstimateCostDTO();

            //weight checking
            if (shipment.DeliveryModeId == (int)DeliveryMode.Car)
            {
                if (shipment.TotalWeight > _shippingLalamoveSettings.MaxWeightCar)
                {
                    result.IsSuccess = false;
                    result.IsInsideCoverage = false;
                    result.IsOverWeight = true;
                    result.IsOverWeightMessage = $"Total product's weight is over the limit of {_shippingLalamoveSettings.MaxWeightCar}kg. Please reduce product quantity.";
                    return result;
                }
            }
            else
            {
                if (shipment.TotalWeight > _shippingLalamoveSettings.MaxWeightBike)
                {
                    result.IsSuccess = false;
                    result.IsInsideCoverage = false;
                    result.IsOverWeight = true;
                    result.IsOverWeightMessage = $"Total product's weight is over the limit of {_shippingLalamoveSettings.MaxWeightBike}kg. Please reduce product quantity.";
                    return result;
                }
            }

            //check vendor from lalamove cities
            (var checkVendorActiveStates, var marketCode) = CheckVendorFromActiveStates(shipment.SenderState);
            if (checkVendorActiveStates == null || marketCode == null)
            {
                result.IsSuccess = false;
                result.CityNotAvailable = true;
            }

            var model = MapShipping(shipment, marketCode);

            var getQuotation = _lalaMoveService
            .GetQuotationAsync<ValidResponse<PlaceOrderModel>>(model, new CancellationToken())
            .GetAwaiter()
            .GetResult();

            if (getQuotation.TotalFee > 0)
            {
                decimal coverageLimitPrice = 0;

                if (model.ServiceType == _shippingLalamoveSettings.Motorcycle)
                {
                    coverageLimitPrice = _shippingLalamoveSettings.BikeCoverageLimit;
                }
                else if (model.ServiceType == _shippingLalamoveSettings.Car)
                {
                    coverageLimitPrice = _shippingLalamoveSettings.CarCoverageLimit;
                }

                if (getQuotation.TotalFee <= coverageLimitPrice)
                {
                    result.IsSuccess = true;
                    result.IsInsideCoverage = true;
                    result.EstimatePrice = getQuotation.TotalFee;
                    result.MarkUpPercentage = _shippingLalamoveSettings.MarkUpShippingFeePercentage;
                }
                else
                {
                    result.IsSuccess = false;
                    result.IsInsideCoverage = false;
                    result.EstimatePrice = getQuotation.TotalFee;
                }
            }

            return result;
        }

        public ShipmentCarrierReceiptDTO Ship(ShipmentCarrierDTO shipment)
        {
            (var checkVendorActiveStates, var marketCode) = CheckVendorFromActiveStates(shipment.SenderState);

            var model = MapShipping(shipment, marketCode);

            var getQuotation = _lalaMoveService
            .GetQuotationAsync<ValidResponse<PlaceOrderModel>>(model, new CancellationToken())
            .GetAwaiter()
            .GetResult();

            if (getQuotation.TotalFee > 0)
            {
                model.QuotedTotalFee.Amount = getQuotation.TotalFee;
                model.QuotedTotalFee.Currency = _shippingLalamoveSettings.TotalFeeCurrency;

                var placeOrder = _lalaMoveService
                    .PlaceOrderAsync<ValidResponse<PlaceOrderModel>>(model, new CancellationToken())
                    .GetAwaiter()
                    .GetResult();

                if (placeOrder.OrderRef != "")
                {
                    return new ShipmentCarrierReceiptDTO
                    {
                        TrackingNumber = placeOrder.OrderRef,
                        Price = placeOrder.TotalFee,
                        ShippingMethod = Name,
                        MarketCode = marketCode
                    };
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public string GetUrl(string trackingNumber)
        {
            return "";
        }

        #endregion
    }
}
