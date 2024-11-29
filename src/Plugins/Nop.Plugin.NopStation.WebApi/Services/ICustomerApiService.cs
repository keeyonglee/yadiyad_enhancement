using Nop.Core.Domain.Customers;
using System;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface ICustomerApiService
    {
        Customer InsertDeviceGuestCustomer(string deviceId);
    }
}