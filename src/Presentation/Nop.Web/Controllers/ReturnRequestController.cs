﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Order;
using Nop.Services.ShippingShuq;
using YadiYad.Pro.Services.DTO.Order;
using System.Threading.Tasks;
using Wkhtmltopdf.NetCore;
using Nop.Services.Payout;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Core.Domain.Tax;
using System.Collections.Generic;
using Nop.Services.Events;
using Nop.Services.Vendors;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class ReturnRequestController : BasePublicController
    {
        #region Fields

        private readonly IGeneratePdf _generatePdf;
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

        private readonly OrderRefundRequestService _orderRefundRequestService;
        private readonly IAddressService _addressService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ICountryService _countryService;
        private readonly TaxSettings _taxSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor

        public ReturnRequestController(
            IGeneratePdf generatePdf, 
            ICustomerService customerService,
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
            OrderRefundRequestService orderRefundRequestService,
            IAddressService addressService,
            IStateProvinceService stateProvinceService,
            ICountryService countryService,
            TaxSettings taxSettings,
            IEventPublisher eventPublisher,
            IVendorService vendorService)
        {
            _generatePdf = generatePdf;
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

            _orderRefundRequestService = orderRefundRequestService;
            _addressService = addressService;
            _stateProvinceService = stateProvinceService;
            _countryService = countryService;
            _taxSettings = taxSettings;
            _eventPublisher = eventPublisher;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        [HttpsRequirement]
        public virtual IActionResult CustomerReturnRequests()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = _returnRequestModelFactory.PrepareCustomerReturnRequestsModel();

            return View(model);
        }

        public virtual IActionResult ReturnRequestDetail(int orderId)
        {
            var model = _returnRequestModelFactory.PrepareGroupReturnRequestModel(orderId);
            return View(model);
        }
        [HttpsRequirement]
        public virtual IActionResult ReturnRequest_Customer(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("Homepage");

            var model = new SubmitReturnRequestModel();
            model = _returnRequestModelFactory.PrepareSubmitReturnRequestModel(model, order);
            return View(model);
        }

        [HttpsRequirement]
        public virtual IActionResult ReturnRequest(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("Homepage");

            var model = new SubmitReturnRequestModel();
            model = _returnRequestModelFactory.PrepareSubmitReturnRequestModel(model, order);
            return View(model);
        }

        [HttpPost, ActionName("ReturnRequest")]
        public virtual IActionResult ReturnRequestSubmit(int orderId, SubmitReturnRequestModel model, IFormCollection form)
        {
            var customer = _workContext.CurrentCustomer;
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return Challenge();

            var vendor = _vendorService.GetVendorByOrderId(order.Id);

            if (!_orderProcessingService.IsReturnRequestAllowed(order))
                return RedirectToRoute("Homepage");

            var count = 0;

            var downloadId = 0;
            if (_orderSettings.ReturnRequestsAllowFiles)
            {
                var download = _downloadService.GetDownloadByGuid(model.UploadedFileGuid);
                if (download != null)
                    downloadId = download.Id;
            }

            var orderShipment = _shipmentService.GetShipmentsByOrderId(orderId);
            bool hasInsurance;
            var insuranceAmt = 0.0m;

            foreach (var shipItem in orderShipment)
            {
                insuranceAmt += shipItem.Insurance;
            }

            if (orderShipment[0].Insurance > 0)
                hasInsurance = true;
            else
                hasInsurance = false;

            var groupReturn = new GroupReturnRequest
            {
                HasInsuranceCover = hasInsurance,
                InsuranceClaimAmt = insuranceAmt,
                ApproveStatusId = (int)ApproveStatusEnum.Pending,
                CreatedById = _workContext.CurrentCustomer.Id,
                UpdatedById = _workContext.CurrentCustomer.Id,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow,
                CustomerId = customer.Id
            };

            _returnRequestService.InsertGroupReturnRequest(groupReturn);

            //returnable products
            var orderItems = _orderService.GetOrderItems(order.Id, isNotReturnable: false);
            foreach (var orderItem in orderItems)
            {
                var quantity = 0; //parse quantity
                foreach (var formKey in form.Keys)
                    if (formKey.Equals($"quantity{orderItem.Id}", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int.TryParse(form[formKey], out quantity);
                        break;
                    }
                if (quantity > 0)
                {
                    var rrr = _returnRequestService.GetReturnRequestReasonById(model.ReturnRequestReasonId);
                    var rra = _returnRequestService.GetReturnRequestActionById(model.ReturnRequestActionId);

                    var rr = new ReturnRequest
                    {
                        GroupReturnRequestId = groupReturn.Id,
                        CustomNumber = "",
                        StoreId = _storeContext.CurrentStore.Id,
                        OrderItemId = orderItem.Id,
                        Quantity = quantity,
                        CustomerId = _workContext.CurrentCustomer.Id,
                        ReasonForReturn = rrr != null ? _localizationService.GetLocalized(rrr, x => x.Name) : "not available",
                        RequestedAction = rra != null ? _localizationService.GetLocalized(rra, x => x.Name) : "not available",
                        CustomerComments = model.Comments,
                        UploadedFileId = downloadId,
                        StaffNotes = string.Empty,
                        CreatedOnUtc = DateTime.UtcNow,
                        UpdatedOnUtc = DateTime.UtcNow
                    };

                    _returnRequestService.InsertReturnRequest(rr);

                    order.OrderStatusId = (int)OrderStatus.ReturnRefundProcessing;
                    _orderService.UpdateOrder(order);

                    //set return request custom number
                    rr.CustomNumber = _customNumberFormatter.GenerateReturnRequestCustomNumber(rr);
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                    _returnRequestService.UpdateReturnRequest(rr);

                    //notify store owner
                    // _workflowMessageService.SendNewReturnRequestStoreOwnerNotification(rr, orderItem, order, _localizationSettings.DefaultAdminLanguageId);
                    //notify customer
                    _workflowMessageService.SendNewReturnRequestCustomerNotification(rr, orderItem, order);
                    //notify vendor
                    _workflowMessageService.SendOrderReturnRefundVendorNotification(order, vendor, order.CustomerLanguageId);
                    //notify vendor app
                    _eventPublisher.Publish(new ReturnRequestNewEvent(order));

                    count++;
                }
            }

            model = _returnRequestModelFactory.PrepareSubmitReturnRequestModel(model, order);
            if (count > 0)
                model.Result = _localizationService.GetResource("ReturnRequests.Submitted");
            else
                model.Result = _localizationService.GetResource("ReturnRequests.NoItemsSubmitted");

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult UploadFileReturnRequest()
        {
            if (!_orderSettings.ReturnRequestsEnabled || !_orderSettings.ReturnRequestsAllowFiles)
            {
                return Json(new
                {
                    success = false,
                    downloadGuid = Guid.Empty,
                });
            }

            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty,
                });
            }

            var fileBinary = _downloadService.GetDownloadBits(httpPostedFile);

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = _fileProvider.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = _fileProvider.GetFileExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            var validationFileMaximumSize = _orderSettings.ReturnRequestsFileMaximumSize;
            if (validationFileMaximumSize > 0)
            {
                //compare in bytes
                var maxFileSizeBytes = validationFileMaximumSize * 1024;
                if (fileBinary.Length > maxFileSizeBytes)
                {
                    return Json(new
                    {
                        success = false,
                        message = string.Format(_localizationService.GetResource("ShoppingCart.MaximumUploadedFileSize"), validationFileMaximumSize),
                        downloadGuid = Guid.Empty,
                    });
                }
            }

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = "",
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true
            };
            _downloadService.InsertDownload(download);

            //when returning JSON the mime-type must be set to text/plain
            //otherwise some browsers will pop-up a "Save As" dialog.
            return Json(new
            {
                success = true,
                message = _localizationService.GetResource("ShoppingCart.FileUploaded"),
                downloadUrl = Url.Action("GetFileUpload", "Download", new { downloadId = download.DownloadGuid }),
                downloadGuid = download.DownloadGuid,
            });
        }

        [HttpGet]
        public async Task<IActionResult> Pdf(int orderRefundRequestId)
        {
            var dto = new StatementDTO();
            string view = $"Views/ReturnRequest/Pdf.cshtml";

            var orderRefundRequest = await _orderRefundRequestService.GetOrderRefundRequestByIdAsync(orderRefundRequestId);

            if(orderRefundRequest == null)
            {
                return NotFound();
            }

            var order = _orderService.GetOrderById(orderRefundRequest.OrderId);

            if(order.CustomerId != _workContext.CurrentCustomer.Id)
            {
                return NotFound();
            }

            dto.StatementType = 3;
            dto.StatementNumber = orderRefundRequest.DocumentNumber;
            dto.CreatedOnUTC = orderRefundRequest.TransactionDate;

            //billing address
            var billingAddress = _addressService.GetAddressById(order.BillingAddressId);
            var billingState = _stateProvinceService.GetStateProvinceByAddress(billingAddress);
            var billingCountry = _countryService.GetCountryByAddress(billingAddress);

            dto.StatementToName = billingAddress.FirstName + " " + billingAddress.LastName;
            dto.StatementToAddress1 = billingAddress.Address1;
            dto.StatementToAddress2 = billingAddress.Address2;
            dto.StatementToCity = billingAddress.City;
            dto.StatementToZipPostalCode = billingAddress.ZipPostalCode;
            dto.StatementToState = billingState?.Name;
            dto.StatementToCountry = billingCountry?.Name;

            //store address
            var storeAddress = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);

            if (storeAddress != null)
            {
                if (storeAddress.CountryId.HasValue)
                {
                    var storeCountry = _countryService.GetCountryById(storeAddress.CountryId.Value);
                    dto.StatementFromCountry = storeCountry.Name;
                }
                if (storeAddress.StateProvinceId.HasValue)
                {
                    var storeState = _stateProvinceService.GetStateProvinceById(storeAddress.StateProvinceId.Value);
                    dto.StatementFromState = storeState.Name;
                }

                dto.StatementFromAddress1 = storeAddress.Address1;
                dto.StatementFromAddress2 = storeAddress.Address2;
                dto.StatementFromZipPostalCode = storeAddress.ZipPostalCode;
                dto.StatementFromCity = storeAddress.City;
            }
            else
            {
                dto.StatementFromAddress1 = "Address 1";
                dto.StatementFromState = "State 1";
                dto.StatementFromCountry = "country 1";
            }

            dto.StatementFromName = _storeContext.CurrentStore.Name;
            dto.StatementFromBusinessName = _storeContext.CurrentStore.Name;

            //item
            dto.StatementItems = new List<StatementItemDTO>();

            var refundedOrder = new StatementItemDTO
            {
                Quantity = 1,
                Price = orderRefundRequest.Amount,
                UnitPrice = orderRefundRequest.Amount,
                ItemNames = new List<string>
                {
                    $"Return request refund",
                    $"Order no.: #{order.Id}"
                }
            };

            dto.StatementItems.Add(refundedOrder);

            foreach(var item in dto.StatementItems)
            {
                dto.SubTotal += item.Price;
            }

            dto.GrandTotal = dto.SubTotal;

            return await _generatePdf.GetPdf<StatementDTO>(view, dto);
        }

        #endregion
    }
}