using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ShippingShuq;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Shipping;
using Nop.Services.Vendors;
using OfficeOpenXml.FormulaParsing.Excel.Functions;

namespace Nop.Services.ShippingShuq
{
    public class ShipmentCarrierResolver
    {
        #region Fields

        private readonly IEnumerable<IShippingCarrier> _shippingCarriers;
        private readonly IRepository<VendorAttributeValue_ShippingMethod_Mapping> _mappingRepository;
        private readonly IRepository<ShippingMethod> _shippingMethodRepository;
        private readonly CourierSettings _courierSettings;
        private readonly ICategoryService _categoryService;
        private readonly IShippingService _shippingService;

        #endregion

        #region Ctor

        public ShipmentCarrierResolver(
            IEnumerable<IShippingCarrier>  shippingCarriers,
            IRepository<VendorAttributeValue_ShippingMethod_Mapping> mappingRepository,
            IRepository<ShippingMethod> shippingMethodRepository,
            CourierSettings courierSettings,
            ICategoryService categoryService,
            IShippingService shippingService)
        {
            _shippingCarriers = shippingCarriers;
            _mappingRepository = mappingRepository;
            _shippingMethodRepository = shippingMethodRepository;
            _courierSettings = courierSettings;
            _categoryService = categoryService;
            _shippingService = shippingService;
        }

        #endregion

        #region Methods

        public IShippingCarrier ResolveByVendorBusinessNature(int businessNatureId)
        {
            var shippingMethodId = _mappingRepository.Table
                .Where(s => s.VendorAttributeValueId == businessNatureId)
                .Select(s => s.ShippingMethodId)
                .FirstOrDefault();
            return ResolveByShippingMethod(shippingMethodId);
        }

        public IShippingCarrier ResolveByShippingMethod(int shippingMethodId)
        {
            var shippingCarrierName = _shippingMethodRepository.Table
                .Where(x => x.Id == shippingMethodId)
                .Select(x => x.Name)
                .FirstOrDefault();
            return ResolveByCarrierName(shippingCarrierName);
        }

        public IShippingCarrier ResolveByCarrierName(string name)
        {
            return _shippingCarriers.FirstOrDefault(x => x.Name == name);
        }

        public IShippingCarrier ResolveByCourierSetting(Vendor vendor)
        {
            if (vendor.CourierId != 0)
            {
                var courier = _shippingService.GetShippingMethodById(vendor.CourierId);
                return ResolveByCarrierName(courier.Name);
            }
            else if (vendor.CategoryId != null)
            {
                var vendorCategory = _categoryService.GetCategoryById(vendor.CategoryId.Value);
                if (vendorCategory.Name == NopVendorDefaults.VendorCategoryEats)
                {
                    var eatsCourier = _shippingService.GetShippingMethodById(_courierSettings.Eats);
                    return ResolveByCarrierName(eatsCourier.Name);
                }
                else
                {
                    var martCourier = _shippingService.GetShippingMethodById(_courierSettings.Mart);
                    return ResolveByCarrierName(martCourier.Name);
                }
            }
            else
            {
                throw new NullReferenceException("Vendor's categoryId cannot be null");
            }
        }

        #endregion

    }
}
