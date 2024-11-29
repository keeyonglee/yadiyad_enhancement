using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq.DTO;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.ShuqOrders;
using Nop.Services.Vendors;
using StackExchange.Profiling.Internal;
using StackExchange.Redis;

namespace Nop.Services.ShippingShuq
{
    public class ShipmentProcessor : IShipmentProcessor
    {
        private readonly ShipmentCarrierResolver _shipmentResolver;
        private readonly IVendorService _vendorService;
        private readonly IShuqOrderService _orderService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IAddressService _addressService;
        private readonly IProductService _productService;
        private readonly ShippingMethodService _shippingMethodService;
        private readonly IShipmentService _shipmentService;
        private readonly IStateProvinceService _stateProvinceService;

        public ShipmentProcessor(ShipmentCarrierResolver shipmentResolver, 
            IVendorService vendorService,
            IShuqOrderService orderService, 
            IReturnRequestService returnRequestService,
            IAddressService addressService, 
            IProductService productService,
            ShippingMethodService shippingMethodService, 
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService)
        {
            _shipmentResolver = shipmentResolver;
            _vendorService = vendorService;
            _orderService = orderService;
            _returnRequestService = returnRequestService;
            _addressService = addressService;
            _productService = productService;
            _shippingMethodService = shippingMethodService;
            _shipmentService = shipmentService;
            _stateProvinceService = stateProvinceService;
        }
        
        public void Ship(Shipment shipment)
        {
            if(shipment == null)
                throw new ArgumentNullException(nameof(shipment));

            if (shipment.Id == 0)
                throw new ArgumentException("Shipment Id cannot be zero");

            if(shipment.ShipmentType == ShipmentType.Fulfillment)
                ShipOrder(shipment);
            else 
                ShipReturnOrder(shipment);
        }

        public ShipmentCarrierCoverageChecking CheckProductCoverage(Vendor vendor, Customer customer, Address customerAddress)
        {
            if (vendor == null)
                throw new ArgumentNullException(nameof(vendor));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (customerAddress == null)
                throw new ArgumentNullException(nameof(customerAddress));

            return CheckProductCustomerAddressCoverage(vendor, customer, customerAddress);
        }

        protected virtual void ShipOrder(Shipment shipment)
        {
            //vendor shipment already created
            if (!shipment.TrackingNumber.IsNullOrWhiteSpace())
                return;
            
            // gather info for shipment
            var order = _orderService.GetOrderById(shipment.OrderId);
            
            if(order?.ShippingAddressId.HasValue == false)
                throw new ArgumentException("Order shipping address is not set");
            
            var shippingAddress = _addressService.GetAddressById(order.ShippingAddressId.Value);
            var shippingState = "";
            if (shippingAddress != null && shippingAddress.StateProvinceId != null)
            {
                shippingState = _stateProvinceService.GetStateProvinceById(shippingAddress.StateProvinceId.Value).Name;
            }
            var orderItems = _orderService.GetOrderItems(order.Id, isShipEnabled: true);
            
            var shippingProduct = _productService.GetProductById(orderItems[0].ProductId);
            var vendor = _vendorService.GetVendorById(shippingProduct.VendorId);
            
            var shipmentCarrier = GetShippingCarrier(vendor);
            if (shipmentCarrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            
            var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
            var vendorState = "";
            if (vendorAddress != null && vendorAddress.StateProvinceId != null)
            {
                vendorState = _stateProvinceService.GetStateProvinceById(vendorAddress.StateProvinceId.Value).Name;
            }

            var shipmentdto = new ShipmentCarrierDTO
            {
                OrderId = order.Id,
                SenderName = vendor.Name,
                SenderAddress = $"{vendorAddress.Address1} {vendorAddress.Address2}",
                SenderPhone = vendorAddress.PhoneNumber,
                SenderCompanyName = vendorAddress.Company ?? vendor.Name,
                SenderZip = vendorAddress.ZipPostalCode,
                SenderAddress1 = vendorAddress.Address1,
                SenderAddress2 = vendorAddress.Address2,
                SenderCity = vendorAddress.City,
                SenderState = vendorState,
                ReceiverName = $"{shippingAddress.FirstName} {shippingAddress.LastName}",
                ReceiverAddress = $"{shippingAddress.Address1} {shippingAddress.Address2}",
                ReceiverZip = shippingAddress.ZipPostalCode,
                ReceiverAddress1 = shippingAddress.Address1,
                ReceiverAddress2 = shippingAddress.Address2,
                ReceiverCity = shippingAddress.City,
                ReceiverState = shippingState,
                ReceiverPhone = shippingAddress.PhoneNumber,
                TotalWeight = orderItems.Sum(q => q.ItemWeight) ?? 0.00M,
                ProductName = shippingProduct.Name,
                Remarks = string.Empty,
                TotalValue = orderItems.Sum(q => q.Quantity * q.PriceInclTax),
                RequireInsurance = shipment.RequireInsurance,
                DeliveryModeId = (int)shipment.DeliveryMode,
                Quantity = orderItems.Sum(q => q.Quantity),
                ScheduleAt = shipment.ScheduleAt
            };

            var shippingCarrierReceipt = shipmentCarrier.Ship(shipmentdto);

            if (shippingCarrierReceipt == null)
                return;
            
            shipment.TrackingNumber = shippingCarrierReceipt.TrackingNumber;
            shipment.Insurance = shippingCarrierReceipt.InsuranceFeeRoundUp;
            shipment.ShippingTotal = shippingCarrierReceipt.PriceRoundUp;
            shipment.ShippingMethodId = _shippingMethodService.GetShippingMethodByName(shipmentCarrier.Name).Id;
            shipment.MarketCode = shippingCarrierReceipt.MarketCode;
            shipment.TotalWeight = orderItems.Sum(q => q.ItemWeight) ?? 0.00M;
            _shipmentService.UpdateShipment(shipment);

            AddShippingItems(orderItems, shipment.Id);
        }
        
        protected virtual void ShipReturnOrder(Shipment shipment)
        {
            //vendor shipment already created
            if (!shipment.TrackingNumber.IsNullOrWhiteSpace())
                return;
            
            //gather info for shipment
            if (shipment.ReturnOrderId == null)
                return;
            
            var rOrder = _returnRequestService.GetReturnOrderById(shipment.ReturnOrderId ?? 0);
            var originalShipment = _shipmentService.GetShipmentsByOrderId(shipment.OrderId).FirstOrDefault();
            var rOrderItemsIds = _returnRequestService
                .GetReturnRequestByGroupReturnRequestId(rOrder.GroupReturnRequestId)
                .Select(q => q.OrderItemId);

            var order = _orderService.GetOrderById(shipment.OrderId);
            
            if(order?.ShippingAddressId.HasValue == false)
                throw new ArgumentException("Order shipping address is not set");
            
            var customerAddress = _addressService.GetAddressById(order.ShippingAddressId.Value);
            var customerState = "";
            if (customerAddress != null && customerAddress.StateProvinceId != null)
            {
                customerState = _stateProvinceService.GetStateProvinceById(customerAddress.StateProvinceId.Value).Name;
            }
            var orderItems = _orderService.GetOrderItems(order.Id, isShipEnabled: true)
                .Where(q => rOrderItemsIds.Contains(q.Id))
                .ToList();
            
            var shippingProduct = _productService.GetProductById(orderItems[0].ProductId);
            var vendor = _vendorService.GetVendorById(shippingProduct.VendorId);
            
            var shipmentCarrier = GetShippingCarrier(vendor);
            if (shipmentCarrier == null)
                throw new ArgumentNullException("Cannot resolved any Shipping Carrier");
            
            var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
            var vendorState = "";
            if (vendorAddress != null && vendorAddress.StateProvinceId != null)
            {
                vendorState = _stateProvinceService.GetStateProvinceById(vendorAddress.StateProvinceId.Value).Name;
            }

            var shipmentdto = new ShipmentCarrierDTO
            {
                OrderId = shipment.ReturnOrderId ?? 0,
                SenderName = $"{customerAddress.FirstName} {customerAddress.LastName}",
                SenderAddress = $"{customerAddress.Address1} {customerAddress.Address2}",
                SenderPhone = customerAddress.PhoneNumber,
                SenderCompanyName = string.Empty,
                SenderZip = customerAddress.ZipPostalCode,
                SenderAddress1 = customerAddress.Address1,
                SenderAddress2 = customerAddress.Address2,
                SenderCity = customerAddress.City,
                SenderState = customerState,
                ReceiverName = vendor.Name,
                ReceiverAddress = $"{vendorAddress.Address1} {vendorAddress.Address2}",
                ReceiverZip = vendorAddress.ZipPostalCode,
                ReceiverAddress1 = vendorAddress.Address1,
                ReceiverAddress2 = vendorAddress.Address2,
                ReceiverCity = vendorAddress.City,
                ReceiverState = vendorState,
                ReceiverPhone = vendorAddress.PhoneNumber,
                TotalWeight = shipment.TotalWeight ?? 0.00M,
                ProductName = shippingProduct.Name,
                Remarks = $"Return {shipment.OrderId}",
                TotalValue = orderItems.Sum(q => q.Quantity * q.PriceInclTax),
                RequireInsurance = false, // insurance not require for Returns
                DeliveryModeId = (int)shipment.DeliveryMode,
                Quantity = orderItems.Sum(q => q.Quantity),
                ScheduleAt = shipment.ScheduleAt
            };

            var shippingCarrierReceipt = shipmentCarrier.Ship(shipmentdto);

            if (shippingCarrierReceipt == null)
                return;
            
            shipment.TrackingNumber = shippingCarrierReceipt.TrackingNumber;
            shipment.Insurance = shippingCarrierReceipt.InsuranceFeeRoundUp;
            shipment.ShippingTotal = shippingCarrierReceipt.PriceRoundUp;
            shipment.ShippingMethodId = _shippingMethodService.GetShippingMethodByName(shipmentCarrier.Name).Id;
            shipment.MarketCode = shippingCarrierReceipt.MarketCode;
            shipment.TotalWeight = orderItems.Sum(q => q.ItemWeight) ?? 0.00M;
            _shipmentService.UpdateShipment(shipment);

            AddShippingItems(orderItems, shipment.Id);
        }

        protected virtual ShipmentCarrierCoverageChecking CheckProductCustomerAddressCoverage(Vendor vendor, Customer customer, Address customerAddress)
        {
            var checking = new ShipmentCarrierCoverageChecking();
            var carrier = GetShippingCarrier(vendor);
            if (!carrier.RequireCoverageChecking)
            {
                checking.InsideCoverage = true;
                return checking;
            }
            else
            {
                var vendorAddress = _addressService.GetAddressById(vendor.AddressId);
                var vendorState = "";
                if (vendorAddress != null && vendorAddress.StateProvinceId != null)
                {
                    vendorState = _stateProvinceService.GetStateProvinceById(vendorAddress.StateProvinceId.Value).Name;
                }

                var customerState = "";
                if (customerAddress != null && customerAddress.StateProvinceId != null)
                {
                    customerState = _stateProvinceService.GetStateProvinceById(customerAddress.StateProvinceId.Value).Name;
                }

                var shipmentdto = new ShipmentCarrierDTO
                {
                    OrderId = 0,
                    SenderName = vendor.Name,
                    SenderAddress = $"{vendorAddress.Address1} {vendorAddress.Address2}",
                    SenderPhone = vendorAddress.PhoneNumber,
                    SenderCompanyName = vendorAddress.Company ?? vendor.Name,
                    SenderZip = vendorAddress.ZipPostalCode,
                    SenderAddress1 = vendorAddress.Address1,
                    SenderAddress2 = vendorAddress.Address2,
                    SenderCity = vendorAddress.City,
                    SenderState = vendorState,
                    ReceiverName = $"{customerAddress.FirstName} {customerAddress.LastName}",
                    ReceiverAddress = $"{customerAddress.Address1} {customerAddress.Address2}",
                    ReceiverZip = customerAddress.ZipPostalCode,
                    ReceiverAddress1 = customerAddress.Address1,
                    ReceiverAddress2 = customerAddress.Address2,
                    ReceiverCity = customerAddress.City,
                    ReceiverState = customerState,
                    ReceiverPhone = customerAddress.PhoneNumber,
                    TotalWeight = 0.00M,
                    ProductName = string.Empty,
                    Remarks = string.Empty,
                    DeliveryModeId = (int)DeliveryMode.Bike,
                    Quantity = 1
                };

                var getQuotation = carrier.GetQuotation(shipmentdto);

                if (getQuotation.IsInsideCoverage)
                {
                    checking.InsideCoverage = true;
                    return checking;
                }
                else
                {
                    checking.InsideCoverage = false;
                    checking.VendorId = vendor.Id;
                    return checking;
                }
            }
        }

        private IShippingCarrier GetShippingCarrier(Vendor vendor)
        {
            return _shipmentResolver.ResolveByCourierSetting(vendor);
        }

        private void AddShippingItems(IList<OrderItem> orderItems, int shipmentId)
        {
            var items = _shipmentService.GetShipmentItemsByShipmentId(shipmentId);
            if (!items.Any())
            {
                foreach (var item in orderItems)
                {
                    var shipmentItem = new ShipmentItem
                    {
                        ShipmentId = shipmentId,
                        OrderItemId = item.Id,
                        Quantity = item.Quantity
                    };
                    _shipmentService.InsertShipmentItem(shipmentItem);
                }
            }
        }
    }
}