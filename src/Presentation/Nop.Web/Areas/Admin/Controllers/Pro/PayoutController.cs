using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Payout;
using Nop.Web.Framework.Mvc.Filters;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.Payout;
using System.Text;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Services.Messages;
using Nop.Services.Localization;
using Nop.Core.Domain.Payout;
using Nop.Services.Payout;
using Nop.Services.Helpers;
using Nop.Services.Events;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class PayoutController : BaseAdminController
    {
        #region Fields
        private readonly IDownloadService _downloadService;
        private readonly PayoutBatchService _payoutBatchService;
        private readonly OrderPayoutService _orderPayoutService;
        private readonly PayoutRequestService _payoutRequestService;
        private readonly RefundRequestService _refundRequestService;
        private readonly BankAccountModelFactory _bankAccountFactory;
        private readonly BankAccountService _bankAccountserviceService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;

        private readonly PayoutModelFactory _payoutFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly INopFileProvider _fileProvider;
        private readonly ILogger _logger;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public PayoutController(IDownloadService downloadService,
            PayoutBatchService payoutBatchService,
            OrderPayoutService orderPayoutService,
            PayoutRequestService payoutRequestService,
            RefundRequestService refundRequestService,
            BankAccountModelFactory bankAccountFactory,
            BankAccountService bankAccountserviceService,
            ProWorkflowMessageService proWorkflowMessageService,
            PayoutModelFactory payoutFactory,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IWorkContext workContext,
            INopFileProvider fileProvider,
            ILogger logger,
            IDateTimeHelper dateTimeHelper,
            IEventPublisher eventPublisher)
        {
            _localizationService = localizationService;
            _downloadService = downloadService;
            _payoutBatchService = payoutBatchService;
            _orderPayoutService = orderPayoutService;
            _payoutRequestService = payoutRequestService;
            _refundRequestService = refundRequestService;
            _bankAccountFactory = bankAccountFactory;
            _bankAccountserviceService = bankAccountserviceService;
            _proWorkflowMessageService = proWorkflowMessageService;
            _payoutFactory = payoutFactory;
            _permissionService = permissionService;
            _workContext = workContext;
            _fileProvider = fileProvider;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Batch

        public virtual IActionResult Index()
        {
            return RedirectToAction("Batch");
        }

        public virtual IActionResult Batch(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _payoutFactory.PreparePayoutBatchSearchModel(new PayoutBatchSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BatchList(PayoutBatchSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _payoutFactory.PreparePayoutBatchListModel(searchModel);


            return Json(model);
        }

        #endregion

        #region Group

        public virtual IActionResult Group(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var payoutBatch = _payoutBatchService.GetPayoutBatchDetails(id);
            if (payoutBatch == null)
                return RedirectToAction("Batch");

            //prepare model
            var model = _payoutFactory.PreparePayoutGroupSearchModel(id, new PayoutGroupSearchModel());
            model.BatchId = id;
            model.BatchNumber = payoutBatch.PayoutBatchNumber;

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult GroupList(PayoutGroupSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _payoutFactory.PreparePayoutGroupListModel(searchModel);


            return Json(model);
        }

        #endregion

        #region PayoutRequest

        public virtual IActionResult PayoutRequest(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var payoutGroup = _payoutBatchService.GetPayoutGroupsDetails(id);
            if (payoutGroup == null)
                return RedirectToAction("Batch");
            var payoutBatch = _payoutBatchService.GetPayoutBatchDetails(payoutGroup.PayoutBatchId);

            //prepare model
            var model = _payoutFactory.PreparePayoutRequestSearchModel(new PayoutRequestSearchModel());
            model.GroupId = id;
            model.BatchId = payoutBatch.Id;
            model.BatchNumber = payoutBatch.PayoutBatchNumber;
            if (payoutGroup.IndividualProfile != null)
            {
                model.CustomerName = payoutGroup.IndividualProfile.FullName;
            }
            else if (payoutGroup.OrganizationProfile != null)
            {
                model.CustomerName = payoutGroup.OrganizationProfile.Name;

            }
            else if (payoutGroup.Customer != null)
            {
                model.CustomerName = payoutGroup.Customer.Email;
            }

            if (payoutBatch.Platform == Platform.Pro)
            {
                ViewData["RequestTypeTitle"] = "Listed Professional Fee";
            }
            else if (payoutBatch.Platform == Platform.Shuq)
            {
                ViewData["RequestTypeTitle"] = "Product Price";
            }
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RequestList(PayoutRequestSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _payoutFactory.PreparePayoutRequestListModel(searchModel);


            return Json(model);
        }

        #endregion

        #region Recon

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var payoutBatch = _payoutBatchService.GetPayoutBatchDetails(id);
            if (payoutBatch == null)
                return RedirectToAction("Batch");

            //prepare model
            var model = _payoutFactory.PreparePayoutBatchModel(null, payoutBatch);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditAsync(PayoutBatchModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            var actorId = _workContext.CurrentCustomer.Id;

            //try to get a news item with the specified id
            var payoutBatch = _payoutBatchService.GetPayoutBatchDetails(model.Id);
            if (payoutBatch == null)
                return RedirectToAction("Batch");

            if (ModelState.IsValid)
            {
                var reconFile = _downloadService.GetDownloadById(model.ReconFileDownloadId.Value);

                if (reconFile != null)
                {
                    var reconFileAsset = _downloadService.GetDownloadByGuid(reconFile.DownloadGuid);
                    int reconStatus = (int)PayoutBatchStatus.Fail;
                    string statusRemarks = "";
                    var fileBinary = ProcessReconFileAsync(model.Id, reconFileAsset.DownloadBinary, out reconStatus, out statusRemarks);

                    var localDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow);

                    reconFile.DownloadBinary = fileBinary;
                    reconFile.DownloadGuid = Guid.NewGuid();
                    reconFile.IsNew = true;
                    reconFile.IsPrivateContent = true;
                    reconFile.Filename = $"PB{model.Id}_"+ localDate.ToString("yyyyMMdd");
                    _downloadService.UpdateDownload(reconFile);

                    payoutBatch.Status = reconStatus;
                    payoutBatch.ReconFileDownloadId = model.ReconFileDownloadId;
                    payoutBatch.ReconDateTime = DateTime.UtcNow;
                    payoutBatch.StatusRemarks = statusRemarks;
                    _payoutBatchService.UpdatePayoutBatchStatus(actorId, payoutBatch);
                    _payoutBatchService.AllocateInvoiceToOrderPayoutRequest(actorId);

                    if (!continueEditing)
                        return RedirectToAction("Batch");

                    return RedirectToAction("Edit", new { id = payoutBatch.Id });
                }
                else
                {
                    ModelState.AddModelError(
                        nameof(PayoutBatchModel.ReconFileDownloadId), 
                        _localizationService.GetResource("Account.PayoutBatch.ReconFileDownloadId.Required")
                        );
                }
            }

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult AsyncUploadAsync()
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded"
                });
            }

            if (!Path.GetExtension(httpPostedFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("File extension is not supported");
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

            var download = new Download
            {
                DownloadGuid = Guid.NewGuid(),
                UseDownloadUrl = false,
                DownloadUrl = string.Empty,
                DownloadBinary = fileBinary,
                ContentType = contentType,
                //we store filename without extension for downloads
                Filename = _fileProvider.GetFileNameWithoutExtension(fileName),
                Extension = fileExtension,
                IsNew = true,
                IsPrivateContent = true
            };

            try
            {
                _downloadService.InsertDownload(download);

                //when returning JSON the mime-type must be set to text/plain
                //otherwise some browsers will pop-up a "Save As" dialog.
                return Json(new
                {
                    success = true,
                    downloadId = download.Id,
                    downloadUrl = Url.Action("DownloadFile", "Download", new { downloadGuid = download.DownloadGuid })
                });
            }
            catch (Exception exc)
            {
                _logger.Error(exc.Message, exc, _workContext.CurrentCustomer);

                return Json(new
                {
                    success = false,
                    message = "File cannot be saved"
                });
            }
        }

        public virtual byte[] ProcessReconFileAsync(int batchId, byte[] processFile, out int reconStatus, out string statusRemarks)
        {
            var actorId = _workContext.CurrentCustomer.Id;
            reconStatus = (int)PayoutBatchStatus.Fail;
            statusRemarks = "";
            string fileName = null,
             coId = null,
             originatorId = null,
             originatorName = null,
             originatorAccount = null,
             creditingDate = null,
             hashingTotal = null,
             paymentDesc = null,
             paymentReference = null;

            var payoutBatch = _payoutBatchService.GetPayoutBatchDetails(batchId);
            var payoutGroups = _payoutBatchService.GetPayoutGroupsByBatchId(batchId);

            var payoutRecordStartRowNo = 3;

            try
            {
                using (var stream = new MemoryStream(processFile))
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var workbook = package.Workbook;
                        var worksheet = workbook.Worksheets.First();
                        int lastRowNo = worksheet.Dimension.Rows;
                        int columns = worksheet.Dimension.Columns;

                        int noProcessedPayoutGroup = 0;
                        int noSuccessPayoutGroup = 0;
                        int noFailPayoutGroup = 0;

                        var isHeaderValidationPass = false;


                        #region read header info
                        try
                        {
                            string header = worksheet.Cells[1, 1].Value?.ToString();

                            // var fileNameStart = "Details Report for The Processed File from Maybank";
                            // var fileNameEnd = "Co Id";
                            // fileName = ExtractContentValue(header, fileNameStart, fileNameEnd);

                            var coStart = "Co Id";
                            var coEnd = "Originator ID";
                            coId = ExtractContentValue(header, coStart, coEnd);

                            var originatorIdStart = "Originator ID";
                            var originatorIdEnd = "Originator Name";
                            originatorId = ExtractContentValue(header, originatorIdStart, originatorIdEnd);

                            var originatorNameStart = "Originator Name";
                            var originatorNameEnd = "Originator Account";
                            originatorName = ExtractContentValue(header, originatorNameStart, originatorNameEnd);

                            var originatorAccountStart = "Originator Account";
                            var originatorAccountEnd = "Crediting Date";
                            originatorAccount = ExtractContentValue(header, originatorAccountStart, originatorAccountEnd);

                            var creditingDateStart = "Crediting Date";
                            var creditingDateEnd = "Hashing Total";
                            creditingDate = ExtractContentValue(header, creditingDateStart, creditingDateEnd);

                            var hashingTotalStart = "Hashing Total";
                            var hashingTotalEnd = "Payment Reference";
                            hashingTotal = ExtractContentValue(header, hashingTotalStart, hashingTotalEnd);

                            var paymentReferenceStart = "Payment Reference";
                            var paymentReferenceEnd = "Payment Desc.";
                            paymentReference = ExtractContentValue(header, paymentReferenceStart, paymentReferenceEnd);

                            var paymentDescStart = "Payment Desc.";
                            var paymentDescEnd = "";
                            paymentDesc = ExtractContentValue(header, paymentDescStart, paymentDescEnd);

                            isHeaderValidationPass = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex.Message, ex);
                            statusRemarks = "Recon file header is not present in expected format.";
                            reconStatus = (int)PayoutBatchStatus.Fail;
                        }
                        #endregion

                        if(isHeaderValidationPass == false)
                        {
                            //do nothing
                        }
                        else if (payoutBatch.PayoutBatchNumber
                            .Equals(paymentReference, StringComparison.InvariantCultureIgnoreCase) == false)
                        {
                            statusRemarks = "Payment reference did not match with payout batch number.";
                            reconStatus = (int)PayoutBatchStatus.Fail;
                        }
                        else
                        {
                            int noRecordToBeProcess = 0;

                            for (int rowNo = payoutRecordStartRowNo; rowNo <= lastRowNo; rowNo++)
                            {
                                var value1stCol = worksheet.Cells[rowNo, 1].Value;

                                if (int.TryParse((value1stCol?.ToString()??""), out var sn) == false)
                                {
                                    break;
                                }

                                noRecordToBeProcess++;
                            }

                            if (noRecordToBeProcess == 0)
                            {
                                statusRemarks = $"No data.";
                                reconStatus = (int)PayoutBatchStatus.Fail;
                            }
                            else if (noRecordToBeProcess != payoutGroups.Count)
                            {
                                statusRemarks = $"Batch record count vs the record count inside recon file do not match.";
                                reconStatus = (int)PayoutBatchStatus.Fail;
                            }
                            else
                            {
                                worksheet.Cells[2, 15].Value = "Upload Status";
                                worksheet.Cells[2, 17].Value = "Upload Error";

                                for (int rowNo = payoutRecordStartRowNo; rowNo < payoutRecordStartRowNo + noRecordToBeProcess; rowNo++)
                                {
                                    bool isValid = true;
                                    string no = worksheet.Cells[rowNo, 1].Value?.ToString();
                                    string payeeName = worksheet.Cells[rowNo, 2].Value?.ToString();
                                    string idType = worksheet.Cells[rowNo, 3].Value?.ToString();
                                    string idNumber = worksheet.Cells[rowNo, 4].Value?.ToString();
                                    string paymentMode = worksheet.Cells[rowNo, 6].Value?.ToString();
                                    string creditBank = worksheet.Cells[rowNo, 7].Value?.ToString();
                                    string idValidation = worksheet.Cells[rowNo, 8].Value?.ToString();
                                    string accountNo = worksheet.Cells[rowNo, 9].Value?.ToString();
                                    string amount = worksheet.Cells[rowNo, 10].Value?.ToString();
                                    string waive = worksheet.Cells[rowNo, 11].Value?.ToString();
                                    string status = worksheet.Cells[rowNo, 12].Value?.ToString();
                                    string reason = worksheet.Cells[rowNo, 13].Value?.ToString();
                                    decimal amountValue = 0;

                                    //validate
                                    if (!string.IsNullOrEmpty(no) && int.TryParse(no, out int recordNo))
                                    {
                                        if (idType != "NIC" && idType != "PAS")
                                        {
                                            //invalid id type
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid Id Type";
                                            isValid = false;
                                        }

                                        if (paymentMode != "1" &&
                                            paymentMode != "2" &&
                                            paymentMode != "11" &&
                                            paymentMode != "12" &&
                                            paymentMode != "13" &&
                                            paymentMode != "14" &&
                                            paymentMode != "15")
                                        {
                                            //invalid payment mode
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid Payment Mode";
                                            isValid = false;
                                        }

                                        if (creditBank != "1" &&
                                            creditBank != "2" &&
                                            creditBank != "3" &&
                                            creditBank != "4" &&
                                            creditBank != "5" &&
                                            creditBank != "6" &&
                                            creditBank != "7" &&
                                            creditBank != "8" &&
                                            creditBank != "9" &&
                                            creditBank != "10" &&
                                            creditBank != "11" &&
                                            creditBank != "12" &&
                                            creditBank != "13" &&
                                            creditBank != "14" &&
                                            creditBank != "15" &&
                                            creditBank != "16" &&
                                            creditBank != "17" &&
                                            creditBank != "18" &&
                                            creditBank != "19" &&
                                            creditBank != "20" &&
                                            creditBank != "21")
                                        {
                                            //invalid credit bank
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid Credit Bank";
                                            isValid = false;
                                        }

                                        if (idValidation != "Y" && idType != "N")
                                        {
                                            //invalid id validation
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid ID Validation";
                                            isValid = false;
                                        }

                                        if (!decimal.TryParse(amount, out amountValue))
                                        {
                                            //invalid amount
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid Amount";
                                            isValid = false;
                                        }

                                        if (waive != "Y" && waive != "N")
                                        {
                                            //invalid waive
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid Waive";
                                            isValid = false;
                                        }

                                        if (status != "Success" && status != "Fail")
                                        {
                                            //invalid status
                                            worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Invalid Status";
                                            isValid = false;
                                        }

                                        var payoutGroup = payoutGroups
                                            .Where(x => x.AccountNumber == accountNo
                                            && x.Amount == amountValue)
                                            .FirstOrDefault();

                                        if (isValid)
                                        {
                                            //update
                                            if (payoutGroup is null)
                                            {
                                                //invalid payout group
                                                worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "No payout group match in payout batch.";
                                                isValid = false;
                                            }
                                            else
                                            {
                                                noProcessedPayoutGroup++;

                                                if (payoutGroup.Amount != amountValue)
                                                {
                                                    noFailPayoutGroup++;
                                                    isValid = false;

                                                    payoutGroup.Status = (int)PayoutGroupStatus.Error;
                                                    payoutGroup.Remarks = "Amount not tally with recon file.";
                                                    _payoutBatchService.UpdatePayoutGroupStatus(actorId, payoutGroup);

                                                    worksheet.Cells[rowNo, 17].Value += (!string.IsNullOrEmpty(worksheet.Cells[rowNo, 17].Value?.ToString()) ? ", " : "") + "Amount not tally with system.";
                                                }
                                                else if (status?.ToLower() != "success")
                                                {
                                                    noFailPayoutGroup++;
                                                    isValid = false;

                                                    payoutGroup.Status = (int)PayoutGroupStatus.Error;
                                                    payoutGroup.Remarks = string.IsNullOrWhiteSpace(reason) ? $"Recon status presented as \"{status}\"" : reason;
                                                    _payoutBatchService.UpdatePayoutGroupStatus(actorId, payoutGroup);

                                                    //update bank account to invalid
                                                    var bankAccount = _bankAccountserviceService.GetBankAccountByCustomerId(payoutGroup.PayoutTo);
                                                    bankAccount.Status = "Rejected";
                                                    bankAccount.IsVerified = false;
                                                    _bankAccountserviceService.UpdateBankAccountById(actorId, bankAccount);
                                                    _proWorkflowMessageService.SendPayouRequestFailInvalidBankAccountMessage(bankAccount, _workContext.WorkingLanguage.Id);
                                                }
                                                else if (payoutGroup.Status != (int)PayoutGroupStatus.Success)
                                                {
                                                    noSuccessPayoutGroup++;
                                                    isValid = true;

                                                    var payoutAndGroup = _payoutBatchService.GetRequestByGroupId(payoutGroup.Id);

                                                    foreach (var payout in payoutAndGroup)
                                                    {
                                                        switch ((PayoutAndGroupRefType)payout.RefTypeId)
                                                        {
                                                            case PayoutAndGroupRefType.PayoutRequest:
                                                                {
                                                                    //update payout
                                                                    if (payout.PayoutRequest.Status != (int)PayoutRequestStatus.Paid)
                                                                    {
                                                                        _payoutRequestService.UpdatePayoutRequestPaidStatus(actorId, payout.PayoutRequest.Id);
                                                                    }
                                                                }
                                                                break;
                                                            case PayoutAndGroupRefType.RefundRequest:
                                                                {
                                                                    //update refund
                                                                    if (payout.RefundRequest.Status != (int)RefundStatus.Paid)
                                                                    {
                                                                        _refundRequestService.UpdateRefundRequestStatus(actorId, payout.RefundRequest.Id, RefundStatus.Paid);
                                                                    }
                                                                }
                                                                break;
                                                            case PayoutAndGroupRefType.OrderPayoutRequest:
                                                                {
                                                                    //update payout
                                                                    if (payout.OrderPayoutRequest.OrderPayoutStatusId != (int)OrderPayoutStatus.Paid)
                                                                    {
                                                                        var orderPayoutRequest = _orderPayoutService.UpdatePayoutRequestPaidStatus(actorId, payout.OrderPayoutRequest.Id);
                                                                        var png = new PayoutAndGroupShuqDTO { PayoutGroupId = payout.PayoutGroupId };
                                                                        _eventPublisher.Publish(new OrderPayoutRequestEvent(orderPayoutRequest, png));
                                                                    }
                                                                }
                                                                break;
                                                            case PayoutAndGroupRefType.OrderRefundRequest:
                                                                {
                                                                    //update refund
                                                                    if (payout.OrderRefundRequest.RefundStatusId != (int)RefundStatus.Paid)
                                                                    {
                                                                        _orderPayoutService.UpdateRefundRequestPaidStatus(actorId, payout.OrderRefundRequest.Id);
                                                                    }
                                                                }
                                                                break;
                                                        }
                                                    }

                                                    //validate is all payout request close
                                                    payoutAndGroup = _payoutBatchService.GetRequestByGroupId(payoutGroup.Id);
                                                    var notSuccess = payoutAndGroup.Where(x => (x.RefTypeId == (int)PayoutAndGroupRefType.PayoutRequest && x.PayoutRequest.Status != (int)PayoutRequestStatus.Paid) ||
                                                                                               (x.RefTypeId == (int)PayoutAndGroupRefType.RefundRequest && x.RefundRequest.Status != (int)RefundStatus.Paid) ||
                                                                                               (x.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest && x.OrderPayoutRequest.OrderPayoutStatusId != (int)OrderPayoutStatus.Paid) ||
                                                                                               (x.RefTypeId == (int)PayoutAndGroupRefType.OrderRefundRequest && x.OrderRefundRequest.RefundStatusId != (int)RefundStatus.Paid)
                                                                                               );

                                                    //update payoutGroup
                                                    payoutGroup.Status = notSuccess.Count() == 0 ? (int)PayoutGroupStatus.Success : (int)PayoutGroupStatus.Error;
                                                    payoutGroup.Remarks = reason;
                                                    payoutGroup.Remarks = (payoutGroup.Remarks ?? "").Substring(0, Math.Min((payoutGroup.Remarks?.Length ?? 0), 255));
                                                    _payoutBatchService.UpdatePayoutGroupStatus(actorId, payoutGroup);

                                                }
                                            }

                                            worksheet.Cells[rowNo, 15].Value = isValid ? "Success" : "Fail";
                                            worksheet.Cells[rowNo, 17].Value = isValid ? "" : worksheet.Cells[rowNo, 17].Value;
                                        }
                                        else if (payoutGroup != null)
                                        {
                                            //update payoutGroup
                                            payoutGroup.Status = (int)PayoutGroupStatus.Error;
                                            payoutGroup.Remarks = $"{worksheet.Cells[rowNo, 17].Value?.ToString()}"+((" Reason: "+(string.IsNullOrWhiteSpace(reason)?null: reason))??"");
                                            payoutGroup.Remarks = (payoutGroup.Remarks ?? "").Substring(0, Math.Min((payoutGroup.Remarks?.Length ?? 0), 255));
                                            _payoutBatchService.UpdatePayoutGroupStatus(actorId, payoutGroup);
                                        }
                                    }
                                }

                                //update payout batch status
                                if (string.IsNullOrWhiteSpace(statusRemarks))
                                {
                                    payoutGroups = _payoutBatchService.GetPayoutGroupsByBatchId(batchId);
                                    var noErrorRecord = payoutGroups.Where(x => x.Status == (int)PayoutGroupStatus.Error).Count();
                                    var noProcessedRecord = payoutGroups.Where(x => x.Status == (int)PayoutGroupStatus.New).Count();
                                    var noSucceedRecord = payoutGroups.Where(x => x.Status == (int)PayoutGroupStatus.Success).Count();

                                    if (noSucceedRecord == 0 && noErrorRecord == 0)
                                    {
                                        reconStatus = (int)PayoutBatchStatus.Fail;
                                        statusRemarks = "all records failed";
                                    }
                                    else
                                    {
                                        reconStatus = noErrorRecord == 0 ? (int)PayoutBatchStatus.Success : (int)PayoutBatchStatus.Error;
                                        statusRemarks = $"Total {noSucceedRecord + noErrorRecord} processed payout group, {noSucceedRecord} succeed, {noErrorRecord} failed.";
                                    }
                                }
                            }
                        }

                        package.Save();
                        return package.GetAsByteArray();
                    }
                }
            }
            catch (Exception ex)
            {
                statusRemarks = "Error occur when process recon file.";
                _logger.Error(ex.Message, ex);
                reconStatus = (int)PayoutBatchStatus.Fail;

                return processFile;
            }
        }


        public string ExtractContentValue(string content, string startStr, string endStr)
        {
            int fileNameStart = content.LastIndexOf(startStr) + startStr.Length;
            int fileNameEnd = content.IndexOf(endStr, fileNameStart);

            string result;
            if (endStr == "")
            {
                result = content.Substring(fileNameStart);
            }
            else
            {
                result = content.Substring(fileNameStart, fileNameEnd - fileNameStart);
            }
            return result.Replace(":", "").Replace("\n", "").TrimStart().TrimEnd();
        }

        #endregion

        #region Export
        public virtual IActionResult Download(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var payoutDetail = _payoutBatchService.GetPayoutBatchDetails(id);
            var payoutGroups = _payoutBatchService.GetPayoutGroupsByBatchId(id);

            if (payoutDetail != null && payoutGroups.Count > 0)
            {
                var now = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
                var payoutBatchGeneratedUserTime =
                    _dateTimeHelper.ConvertToUserTime(payoutDetail.GeneratedDateTime, DateTimeKind.Utc);
                var payoutDate = _payoutBatchService.GetPayoutDay(payoutBatchGeneratedUserTime);
                var fileName = $"payout_{payoutDetail.PayoutBatchNumber}_{now:yyyyMMddHHmmss}.csv";

                if (payoutDetail.Status != (int)PayoutBatchStatus.Success
                    && payoutDetail.Status != (int)PayoutBatchStatus.Error)
                {
                    payoutDetail.Status = (int)PayoutBatchStatus.Downloaded;
                    payoutDetail.DownloadDateTime = DateTime.UtcNow;
                    _payoutBatchService.UpdatePayoutBatch(payoutDetail);
                }

                var result = _payoutBatchService.GeneratePayoutBatchCSVContent(
                    payoutDetail,
                    payoutGroups,
                    payoutDate
                    );

                return File(Encoding.UTF8.GetBytes(result), MimeTypes.TextCsv, fileName);
            }
            else
            {
                throw new ArgumentException("Export failed.");
            }
        }
        #endregion

        #region Vendor

        [HttpPost]
        public virtual IActionResult PayoutVendorList(PayoutVendorSearchModel searchModel)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
            //    return AccessDeniedDataTablesJson();

            var model = _payoutFactory.PreparePayoutVendorListModel(searchModel, _workContext.CurrentVendor.Id);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult PayoutVendorOrderList(PayoutVendorSearchModel searchModel)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
            //    return AccessDeniedDataTablesJson();

            var model = _payoutFactory.PreparePayoutVendorOrderListModel(searchModel);

            return Json(model);
        }

        #endregion
    }
}
