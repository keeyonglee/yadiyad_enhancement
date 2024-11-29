using System;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Services.Customers;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public class CustomerApiService : ICustomerApiService
    {
        private readonly ICustomerService _customerService;

        public CustomerApiService(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public virtual Customer InsertDeviceGuestCustomer(string deviceId)
        {
            var customerGuid = HelperExtension.GetGuid(deviceId);
            var customer = new Customer
            {
                CustomerGuid = customerGuid,
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow
            };

            //add to 'Guests' role
            var guestRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.GuestRoleName);
            if (guestRole == null)
                throw new NopException("'Guests' role could not be loaded");

            _customerService.InsertCustomer(customer);

            _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping
            {
                CustomerId = customer.Id,
                CustomerRoleId = guestRole.Id
            });

            return customer;
        }
    }
}
