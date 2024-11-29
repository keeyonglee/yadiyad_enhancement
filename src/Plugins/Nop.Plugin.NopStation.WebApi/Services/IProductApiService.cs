using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Catalog;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface IProductApiService
    {
        Product GetProductByGtin(string gtin);
    }
}
