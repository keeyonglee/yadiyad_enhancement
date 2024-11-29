using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public class ProductApiService : IProductApiService
    {
        private readonly IRepository<Product> _productRepository;
        public ProductApiService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public Product GetProductByGtin(string gtin)
        {
            var query = from p in _productRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Gtin == gtin
                        select p;

            var product = query.FirstOrDefault();
            return product;
        }
    }
}
