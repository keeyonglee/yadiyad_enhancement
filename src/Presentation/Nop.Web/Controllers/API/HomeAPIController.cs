using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Order;
using YadiYad.Pro.Web.DTO.Base;
using YadiYad.Pro.Web.Filters;
using YadiYad.Pro.Web.Infrastructure;

namespace Nop.Web.Controllers
{
    [CamelCaseResponseFormatter]
    [Route("api/shuq/[controller]")]
    public partial class HomeAPIController : BasePublicController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IReturnRequestModelFactory _returnRequestModelFactory;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IShipmentService _shipmentService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly OrderSettings _orderSettings;
        private readonly IProductService _productService;
        private readonly IProductModelFactory _productModelFactory;

        #endregion

        #region Ctor

        public HomeAPIController(ICustomerService customerService,
            ICustomNumberFormatter customNumberFormatter,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IReturnRequestModelFactory returnRequestModelFactory,
            IReturnRequestService returnRequestService,
            IShipmentService shipmentService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            OrderSettings orderSettings,
            IProductService productService,
            IProductModelFactory productModelFactory)
        {
            _customerService = customerService;
            _customNumberFormatter = customNumberFormatter;
            _downloadService = downloadService;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _returnRequestModelFactory = returnRequestModelFactory;
            _returnRequestService = returnRequestService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _orderSettings = orderSettings;
            _productService = productService;
            _productModelFactory = productModelFactory;
        }

        #endregion

        #region Methods


        [HttpPost("featured-eats")]
        public virtual IActionResult HomeEatsProduct([FromBody] ListFilterDTO<ProductOverviewModel> searchDTO)
        {
            var response = new ResponseDTO();

            var data = _productService.GetAllEatsProductDisplayedOnHomePage(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize);

            if (data == null)
                return NotFound();

            var model = _productModelFactory.PrepareProductOverviewModels(data);

            response.SetResponse(model);

            return Ok(response);
        }

        [HttpPost("featured-mart")]
        public virtual IActionResult HomeMartProduct([FromBody] ListFilterDTO<ProductOverviewModel> searchDTO)
        {
            var response = new ResponseDTO();

            var data = _productService.GetAllMartProductDisplayedOnHomePage(
                searchDTO.Offset / searchDTO.RecordSize,
                searchDTO.RecordSize);

            if (data == null)
                return NotFound();

            var model = _productModelFactory.PrepareProductOverviewModels(data);

            response.SetResponse(model);

            return Ok(response);
        }

        #endregion
    }
}