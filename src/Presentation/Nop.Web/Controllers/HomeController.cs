using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System;
using System.Linq;
using YadiYad.Pro.Core.Domain.Home;
using YadiYad.Pro.Web.DTO.Base;

namespace Nop.Web.Controllers
{
    public partial class HomeController : BasePublicController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICatalogModelFactory _catalogModelFactory;

        #endregion

        #region Ctor

        public HomeController(IProductService productService,
            IProductModelFactory productModelFactory,
            ICatalogModelFactory catalogModelFactory)
        {
            _productService = productService;
            _productModelFactory = productModelFactory;
            _catalogModelFactory = catalogModelFactory;
        }

        #endregion

        public virtual IActionResult Index()
        {
            var model = _catalogModelFactory.PrepareHomeCategoryModel();
            return View(model);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }


    }
}