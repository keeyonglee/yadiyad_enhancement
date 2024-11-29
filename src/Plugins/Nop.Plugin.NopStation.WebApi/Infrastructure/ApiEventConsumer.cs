using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Events;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Services.Customers;
using Microsoft.Extensions.Primitives;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Services.Localization;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure
{
    public class ApiEventConsumer : IConsumer<CustomerLoggedOutEvent>,
        IConsumer<CustomerLoggedinEvent>,
        IConsumer<OrderPlacedEvent>
    {
        private readonly IApiDeviceService _deviceService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly ICustomerApiService _customerApiService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;

        public ApiEventConsumer(IApiDeviceService apiDeviceService,
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            ICustomerApiService customerApiService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IOrderService orderService)
        {
            _deviceService = apiDeviceService;
            _httpContextAccessor = httpContextAccessor;
            _customerService = customerService;
            _customerApiService = customerApiService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _orderService = orderService;
        }

        public void HandleEvent(CustomerLoggedOutEvent eventMessage)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers
                .TryGetValue(WebApiCustomerDefaults.DeviceId, out StringValues headerValues))
            {
                var deviceId = headerValues.FirstOrDefault();
                var device = _deviceService.GetApiDeviceByDeviceId(deviceId, _storeContext.CurrentStore.Id);
                if (device != null)
                {
                    device.CustomerId = eventMessage.Customer.Id;
                    device.IsRegistered = false;
                    _deviceService.UpdateApiDevice(device);
                }
            }
        }

        public void HandleEvent(CustomerLoggedinEvent eventMessage)
        {
            if (_httpContextAccessor.HttpContext.Request.Headers
                .TryGetValue(WebApiCustomerDefaults.DeviceId, out StringValues headerValues))
            {
                var deviceId = headerValues.FirstOrDefault();
                var device = _deviceService.GetApiDeviceByDeviceId(deviceId, _storeContext.CurrentStore.Id);
                if (device != null)
                {
                    device.CustomerId = eventMessage.Customer.Id;
                    device.IsRegistered = true;
                    _deviceService.UpdateApiDevice(device);
                }

                var customerGuid = HelperExtension.GetGuid(deviceId);
                var customer = _customerService.GetCustomerByGuid(customerGuid);
                if (customer != null)
                {
                    customer.CustomerGuid = Guid.NewGuid();
                    _customerService.UpdateCustomer(customer);
                }
            }
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            var order = eventMessage.Order;

            if (_httpContextAccessor.HttpContext.Request.Headers
                .TryGetValue(WebApiCustomerDefaults.DeviceId, out StringValues headerValues))
            {
                var deviceId = headerValues.FirstOrDefault();
                var device = _deviceService.GetApiDeviceByDeviceId(deviceId, _storeContext.CurrentStore.Id);

                if (device == null)
                    return;

                if (device.DeviceType == Domains.DeviceType.Android)
                {
                    var orderNote = new OrderNote()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DisplayToCustomer = false,
                        OrderId = order.Id,
                        Note = _localizationService.GetResource("NopStation.WebApi.Order.PlacedFromAndroid")
                    };
                    _orderService.InsertOrderNote(orderNote);
                }
                else if (device.DeviceType == Domains.DeviceType.IPhone)
                {
                    var orderNote = new OrderNote()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        DisplayToCustomer = false,
                        Note = _localizationService.GetResource("NopStation.WebApi.Order.PlacedFromIPhone")
                    };
                    _orderService.InsertOrderNote(orderNote);
                }
            }
            else
            {
                var orderNote = new OrderNote()
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    DisplayToCustomer = false,
                    Note = _localizationService.GetResource("NopStation.WebApi.Order.PlacedFromWeb")
                };
                _orderService.InsertOrderNote(orderNote);
            }
        }
    }
}
