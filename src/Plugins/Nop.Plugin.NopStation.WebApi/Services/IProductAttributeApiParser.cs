using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface IProductAttributeApiParser
    {
        decimal ParseCustomerEnteredPrice(Product product, NameValueCollection form);

        int ParseEnteredQuantity(Product product, NameValueCollection form);

        string ParseProductAttributes(Product product, NameValueCollection form, List<string> errors);

        void ParseRentalDates(Product product, NameValueCollection form, out DateTime? startDate, out DateTime? endDate);
    }
}