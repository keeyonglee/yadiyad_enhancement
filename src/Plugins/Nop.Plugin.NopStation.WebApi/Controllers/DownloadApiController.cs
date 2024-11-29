using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.WebApi.Models.Common;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;

namespace Nop.Plugin.NopStation.WebApi.Controllers
{
    [Route("api/download")]
    public partial class DownloadApiController : BaseApiController
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public DownloadApiController(CustomerSettings customerSettings,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IOrderService orderService,
            IProductService productService,
            IWorkContext workContext)
        {
            _customerSettings = customerSettings;
            _downloadService = downloadService;
            _localizationService = localizationService;
            _orderService = orderService;
            _productService = productService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpGet("sample/{productId}")]
        public virtual IActionResult Sample(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return NotFound();

            if (!product.HasSampleDownload)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.NoSampleDownload"));

            var download = _downloadService.GetDownloadById(product.SampleDownloadId);
            if (download == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.SampleDownloadNotAvailable"));

            if (download.UseDownloadUrl)
            {
                var response = new GenericResponseModel<DownloadModel>();
                response.Data.Redirect = true;
                response.Data.DownloadUrl = download.DownloadUrl;
                return Ok(response);
            }

            if (download.DownloadBinary == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadDataNotAvailable"));

            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        [HttpGet("getdownload/{orderItemId}/{agree?}")]
        public virtual IActionResult GetDownload(Guid orderItemId, bool agree = false)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return NotFound();

            var order = _orderService.GetOrderById(orderItem.OrderId);
            var product = _productService.GetProductById(orderItem.ProductId);
            if (!_orderService.IsDownloadAllowed(orderItem))
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.NotAllowed"));

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (_workContext.CurrentCustomer == null)
                    return Unauthorized();

                if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.NotYourOrder"));
            }

            var download = _downloadService.GetDownloadById(product.DownloadId);
            if (download == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadNotAvailable"));

            if (product.HasUserAgreement && !agree)
            {
                var response = new GenericResponseModel<DownloadModel>();
                response.Data.HasUserAgreement = true;
                response.Data.OrderItemId = orderItemId;
                return Ok(response);
            }

            if (!product.UnlimitedDownloads && orderItem.DownloadCount >= product.MaxNumberOfDownloads)
                return BadRequest(string.Format(_localizationService.GetResource("DownloadableProducts.ReachedMaximumNumber"), product.MaxNumberOfDownloads));

            if (download.UseDownloadUrl)
            {
                //increase download
                orderItem.DownloadCount++;
                _orderService.UpdateOrder(order);

                var response = new GenericResponseModel<DownloadModel>();
                response.Data.Redirect = true;
                response.Data.DownloadUrl = download.DownloadUrl;
                return Ok(response);
            }

            //binary download
            if (download.DownloadBinary == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadDataNotAvailable"));

            //increase download
            orderItem.DownloadCount++;
            _orderService.UpdateOrder(order);

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        [HttpGet("getlicense/{orderItemId}")]
        public virtual IActionResult GetLicense(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return NotFound();

            var order = _orderService.GetOrderById(orderItem.OrderId);
            var product = _productService.GetProductById(orderItem.ProductId);
            if (!_orderService.IsLicenseDownloadAllowed(orderItem))
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.NotAllowed"));

            if (_customerSettings.DownloadableProductsValidateUser)
            {
                if (_workContext.CurrentCustomer == null || order.CustomerId != _workContext.CurrentCustomer.Id)
                    return Unauthorized();
            }

            var download = _downloadService.GetDownloadById(orderItem.LicenseDownloadId.HasValue ? orderItem.LicenseDownloadId.Value : 0);
            if (download == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadNotAvailable"));

            if (download.UseDownloadUrl)
            {
                var response = new GenericResponseModel<DownloadModel>();
                response.Data.Redirect = true;
                response.Data.DownloadUrl = download.DownloadUrl;
                return Ok(response);
            }

            //binary download
            if (download.DownloadBinary == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadDataNotAvailable"));

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : product.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        [HttpGet("getfileupload/{downloadId}")]
        public virtual IActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadNotAvailable"));

            if (download.UseDownloadUrl)
            {
                var response = new GenericResponseModel<DownloadModel>();
                response.Data.Redirect = true;
                response.Data.DownloadUrl = download.DownloadUrl;
                return Ok(response);
            }

            //binary download
            if (download.DownloadBinary == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadDataNotAvailable"));

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        [HttpGet("ordernotefile/{orderNoteId}")]
        public virtual IActionResult GetOrderNoteFile(int orderNoteId)
        {
            var orderNote = _orderService.GetOrderNoteById(orderNoteId);
            if (orderNote == null)
                return NotFound();

            var order = _orderService.GetOrderById(orderNote.OrderId);

            if (_workContext.CurrentCustomer == null || order.CustomerId != _workContext.CurrentCustomer.Id)
                return Unauthorized();

            var download = _downloadService.GetDownloadById(orderNote.DownloadId);
            if (download == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadNotAvailable"));

            if (download.UseDownloadUrl)
            {
                var response = new GenericResponseModel<DownloadModel>();
                response.Data.Redirect = true;
                response.Data.DownloadUrl = download.DownloadUrl;
                return Ok(response);
            }

            //binary download
            if (download.DownloadBinary == null)
                return BadRequest(_localizationService.GetResource("NopStation.WebApi.Download.DownloadDataNotAvailable"));

            //return result
            var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : orderNote.Id.ToString();
            var contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }

        #endregion
    }
}