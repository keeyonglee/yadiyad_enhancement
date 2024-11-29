using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class HomepageMartProductsViewComponent : NopViewComponent
    {
        private readonly IAclService _aclService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ShuqHomepageSettings _shuqHomepageSettings;

        public HomepageMartProductsViewComponent(IAclService aclService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStoreMappingService storeMappingService,
            ShuqHomepageSettings shuqHomepageSettings)
        {
            _aclService = aclService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeMappingService = storeMappingService;
            _shuqHomepageSettings = shuqHomepageSettings;
        }

        public IViewComponentResult Invoke(int? productThumbPictureSize)
        {
            Random rng = new Random();

            var products = _productService.GetInitialMartProductsDisplayedOnHomepage();
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) 
                && _storeMappingService.Authorize(p) 
                && _productService.ProductIsAvailable(p)
                && p.VisibleIndividually).ToList();
            //availability dates
            //products = products.Where(p => _productService.ProductIsAvailable(p)).ToList();

            //products = products.Where(p => p.VisibleIndividually).ToList();

            if (!products.Any())
                return Content("");

            var model = _productModelFactory.PrepareProductOverviewModels(products, true, true, productThumbPictureSize);

            model = model.OrderBy(a => rng.Next()).Take(_shuqHomepageSettings.MartMaxFeaturedProducts).ToList();

            return View(model);
        }
    }
}