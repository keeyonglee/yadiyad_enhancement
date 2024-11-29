using Nop.Core.Domain.Shipping;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.ShippingShuq
{
    public class ShippingMethodService
    {
        private readonly IRepository<ShippingMethod> _shippingMethodrepository;

        public ShippingMethodService(
            IRepository<ShippingMethod> shippingMethodrepository)
        {
            _shippingMethodrepository = shippingMethodrepository;
        }

        public virtual ShippingMethod GetShippingMethodByName(string shippingMethodName)
        {
            if (string.IsNullOrEmpty(shippingMethodName))
                return null;

            return _shippingMethodrepository.Table.Where(x => x.Name == shippingMethodName).FirstOrDefault();
        }
    }
}
