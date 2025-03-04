using AutoMapper;
using ClosedXML.Excel;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Documents;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Documents;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YadiYad.Pro.Core.Domain.Campaign;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Organization;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Services.Campaign;
using YadiYad.Pro.Services.Services.Payout;

namespace YadiYad.Pro.Services.Payout
{
    public class PayoutBatchService
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRepository<OrderPayoutRequest> _orderPayoutRequestRepo;
        private readonly IRepository<OrderRefundRequest> _orderRefundRequestRepo;
        private readonly IRepository<PayoutRequest> _payoutRequestRepo;
        private readonly IRepository<PayoutAndGroup> _payoutAndGroupRepo;
        private readonly IRepository<PayoutGroup> _payoutGroupRepo;
        private readonly IRepository<PayoutBatch> _payoutBatchRepo;
        private readonly IRepository<RefundRequest> _refundRequestRepo;
        private readonly IRepository<IndividualProfile> _individualProfileRepo;
        private readonly IRepository<OrganizationProfile> _organizationProfileRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<BankAccount> _bankAccountRepo;
        private readonly IRepository<Bank> _bankRepo;
        private readonly IRepository<Nop.Core.Domain.Orders.Order> _shuqOrderRepo;
        private readonly IRepository<ProOrder> _proOrderRepo;
        private readonly IRepository<ProOrderItem> _proOrderItemRepo;
        private readonly IRepository<Download> _downloadRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISettingService _settingService;
        private readonly DocumentNumberService _documentNumberService;
        private readonly CampaignProcessingService _campaignProcessingService;
        private readonly PayoutBatchSettings _payoutBatchSettings;
        private readonly BankAccountService _bankAccountServie;
        private readonly ChargeService _chargeService;
        private readonly PayoutRequestSettings _payoutRequestSettings;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public PayoutBatchService
            (IMapper mapper,
            ILogger logger,
            IRepository<OrderPayoutRequest> orderPayoutRequestRepo,
            IRepository<OrderRefundRequest> orderRefundRequestRepo,
            IRepository<PayoutRequest> payoutRequestRepo,
            IRepository<PayoutAndGroup> payoutAndGroupRepo,
            IRepository<PayoutGroup> payoutGroupRepo,
            IRepository<PayoutBatch> payoutBatchRepo,
            IRepository<RefundRequest> refundRequestRepo,
            IRepository<IndividualProfile> individualProfileRepo,
            IRepository<OrganizationProfile> organizationProfileRepo,
            IRepository<Customer> customerRepo,
            IRepository<BankAccount> bankAccountRepo,
            IRepository<Bank> bankRepo,
            IRepository<Nop.Core.Domain.Orders.Order> shuqOrderRepo,
            IRepository<ProOrder> proOrderRepo,
            IRepository<ProOrderItem> proOrderItemRepo,
            IRepository<Download> downloadRepo,
            IRepository<Invoice> invoiceRepo,
            IDateTimeHelper dateTimeHelper,
            BankAccountService bankAccountServie,
            PayoutRequestSettings payoutRequestSettings,
            ChargeService chargeService,
            ISettingService settingService,
            DocumentNumberService documentNumberService,
            CampaignProcessingService campaignProcessingService,
            PayoutBatchSettings payoutBatchSettings,
            INotificationService notificationService)
        {
            _mapper = mapper;
            _logger = logger;
            _dateTimeHelper = dateTimeHelper;
            _orderPayoutRequestRepo = orderPayoutRequestRepo;
            _orderRefundRequestRepo = orderRefundRequestRepo;
            _payoutRequestRepo = payoutRequestRepo;
            _payoutAndGroupRepo = payoutAndGroupRepo;
            _payoutGroupRepo = payoutGroupRepo;
            _payoutBatchRepo = payoutBatchRepo;
            _refundRequestRepo = refundRequestRepo;
            _individualProfileRepo = individualProfileRepo;
            _organizationProfileRepo = organizationProfileRepo;
            _customerRepo = customerRepo;
            _bankAccountRepo = bankAccountRepo;
            _bankRepo = bankRepo;
            _shuqOrderRepo = shuqOrderRepo;
            _proOrderRepo = proOrderRepo;
            _proOrderItemRepo = proOrderItemRepo;
            _downloadRepo = downloadRepo;
            _bankAccountServie = bankAccountServie;
            _payoutRequestSettings = payoutRequestSettings;
            _settingService = settingService;
            _chargeService = chargeService;
            _documentNumberService = documentNumberService;
            _campaignProcessingService = campaignProcessingService;
            _payoutBatchSettings = payoutBatchSettings;
            _invoiceRepo = invoiceRepo;
        }

        #endregion

        #region Methods
        public PayoutBatchGenerationStatus CreatePayoutBatch(int actorId)
        {
            //check if there are running process
            var payoutBatchLockerSetting = _settingService.LoadSetting<PayoutBatchLockerSetting>();

            if (payoutBatchLockerSetting.LastGeneratingUTCDateTime.HasValue
                && payoutBatchLockerSetting.LastGeneratedUTCDateTime != payoutBatchLockerSetting.LastGeneratingUTCDateTime)
            {
                _logger.Information("CreatePayoutBatch: " + JsonConvert.SerializeObject(payoutBatchLockerSetting));

                //check if running process overtime to cater stop halfway issue
                var diffInTicks = (DateTime.UtcNow
                    - payoutBatchLockerSetting.LastGeneratingUTCDateTime.Value.AddSeconds(_payoutRequestSettings.MaxPayoutGenerationSeconds)).Ticks;
                if (diffInTicks < 0)
                {
                    return PayoutBatchGenerationStatus.InProcess;
                }
            }

            //begin lock with generating utc date time
            payoutBatchLockerSetting.LastGeneratingUTCDateTime = DateTime.UtcNow;
            _settingService.SaveSetting(payoutBatchLockerSetting, x => x.LastGeneratingUTCDateTime, clearCache: true);

            try
            {
                //get local time now
                var timezone = _dateTimeHelper.DefaultStoreTimeZone;
                var hoursDiff = timezone.BaseUtcOffset.TotalHours;
                var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
                var scheduledExecuteTime = localDateTime.Date;
                var payoutDate = localDateTime.Date;

                //cut off for 14 and 28 of the month
                if (scheduledExecuteTime.Day >= 28)
                {
                    payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 28);
                }
                else if (scheduledExecuteTime.Day >= 14)
                {
                    payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 14);
                }
                else
                {
                    payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 28).AddMonths(-1);
                }

                var payoutUTCDate = payoutDate.AddHours(hoursDiff * -1);

                //get approved payout request list
                var payoutRequestsQuery =
                    (
                    from pr in _payoutRequestRepo.Table
                    .Where(pr =>
                        pr.Deleted == false
                        && pr.CreatedOnUTC < payoutUTCDate
                        && pr.Status == (int)PayoutRequestStatus.Approved
                    )
                    from pg in
                        (from png in _payoutAndGroupRepo.Table
                        .Where(png =>
                            png.Deleted == false
                            && png.RefTypeId == (int)PayoutAndGroupRefType.PayoutRequest
                            && png.RefId == pr.Id)
                         from pg in _payoutGroupRepo.Table
                         .Where(pg =>
                             pg.Deleted == false
                             && pg.Id == png.PayoutGroupId
                             && pg.Status != (int)PayoutGroupStatus.Error)
                         select new
                         {
                             pg.Id,
                             png.RefId
                         })
                        .Where(p => p.RefId == pr.Id)
                    .DefaultIfEmpty()
                    select new
                    {
                        pr,
                        pg
                    })
                    .Where(x => x.pg == null)
                    .Select(x => x.pr);

                var payoutRequests = payoutRequestsQuery.ToList();

                //get refund request list
                var refundRequests =
                    (
                    from rr in _refundRequestRepo.Table
                    .Where(rr =>
                        rr.Deleted == false
                        && rr.CreatedOnUTC < payoutUTCDate
                        && rr.Status == (int)RefundStatus.New
                    )
                    from pg in
                        (from png in _payoutAndGroupRepo.Table
                        .Where(png =>
                            png.Deleted == false
                            && png.RefTypeId == (int)PayoutAndGroupRefType.RefundRequest
                            && png.RefId == rr.Id)
                         from pg in _payoutGroupRepo.Table
                         .Where(pg =>
                             pg.Deleted == false
                             && pg.Id == png.PayoutGroupId
                             && pg.Status != (int)PayoutGroupStatus.Error)
                         select new
                         {
                             pg.Id,
                             png.RefId
                         })
                        .Where(p => p.RefId == rr.Id)
                    .DefaultIfEmpty()
                    select new
                    {
                        rr,
                        pg
                    })
                    .Where(x => x.pg == null)
                    .Select(x => x.rr)
                    .ToList();

                //if there is at least 1 payout request or refund request
                if (payoutRequests.Count > 0 || refundRequests.Count > 0)
                {
                    //generate payout and refund list
                    var payoutAndRefundRequests = payoutRequests.Select(x => new
                    {
                        Id = x.Id,
                        Amount = x.Fee,
                        RefTypeId = (int)PayoutAndGroupRefType.PayoutRequest,
                        BeneficialId = x.PayoutTo
                    })
                    .Cast<dynamic>()
                    .Concat(refundRequests.Select(x => new
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        RefTypeId = (int)PayoutAndGroupRefType.RefundRequest,
                        BeneficialId = x.RefundTo
                    })
                    .Cast<dynamic>())
                    .ToList();

                    //group payout by payee
                    var newPayoutGroupsBeforeSplit = payoutAndRefundRequests
                        .GroupBy(
                        x => x.BeneficialId,
                        (beneficialId, reqs) => new PayoutGroup
                        {
                            Amount = reqs.Sum(y => (decimal)y.Amount),
                            Status = (int)PayoutRequestStatus.New,
                            PayoutTo = beneficialId
                        }
                        )
                        .ToList()
                        .Select(x =>
                        {
                            x.CreateAudit(actorId);
                            return x;
                        })
                        .ToList();

                    // filter customer without valid bankAccount
                    var payoutTos = newPayoutGroupsBeforeSplit.Select(x => x.PayoutTo).ToList();
                    var customerWithValidBankAcc = _bankAccountRepo.Table
                        .Where(x => !x.Deleted && x.IsVerified != null && x.IsVerified.Value && payoutTos.Contains(x.CustomerId))
                        .Join(_bankRepo.Table, ba => ba.BankId, b => b.Id, (ba, b) => new
                        {
                            ba.CustomerId,
                            ba.AccountNumber,
                            ba.AccountHolderName,
                            BankShortName = b.ShortName
                        })
                        .ToList()
                        .Cast<dynamic>()
                        .ToList();

                    newPayoutGroupsBeforeSplit = newPayoutGroupsBeforeSplit
                        .Where(x => customerWithValidBankAcc.Any(y => y.CustomerId == x.PayoutTo))
                        .ToList();

                    //if no payout group skip
                    if (newPayoutGroupsBeforeSplit.Count <= 0)
                    {
                        return PayoutBatchGenerationStatus.NoItemToProcess;
                    }

                    // Process payout based on payout cycle
                    foreach (var newPayoutGroup in newPayoutGroupsBeforeSplit)
                    {
                        // Split payout group into batches of up to 99 records or not more than RM150,000
                        var currentBatch = new List<PayoutGroup>();
                        var currentBatchAmount = 0m;
                        foreach (var group in newPayoutGroupsBeforeSplit)
                        {
                            if (currentBatch.Count >= 99 || (currentBatchAmount + group.Amount) > 150000)
                            {
                                ProcessPayoutBatch(currentBatch, actorId, customerWithValidBankAcc);
                                currentBatch = new List<PayoutGroup>();
                                currentBatchAmount = 0m;
                            }
                            currentBatch.Add(group);
                            currentBatchAmount += group.Amount;
                        }
                        if (currentBatch.Count > 0)
                        {
                            ProcessPayoutBatch(currentBatch, actorId, customerWithValidBankAcc);
                        }
                    }
                }

                return PayoutBatchGenerationStatus.Success;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //end process
                payoutBatchLockerSetting.LastGeneratedUTCDateTime = payoutBatchLockerSetting.LastGeneratingUTCDateTime;

                _settingService.SaveSetting(payoutBatchLockerSetting, x => x.LastGeneratedUTCDateTime, clearCache: true);
            }
        }

        private void ProcessPayoutBatch(List<PayoutGroup> newPayoutGroups, int actorId, List<dynamic> customerWithValidBankAcc)
        {
            // create payout batch
            var newPayoutBatch = new PayoutBatch
            {
                GeneratedDateTime = DateTime.UtcNow,
                Status = (int)PayoutRequestStatus.New,
                Platform = Platform.Pro
            };
            newPayoutBatch.CreateAudit(actorId);
            _payoutBatchRepo.Insert(newPayoutBatch);

            //create payout group
            var updatedNewPayoutGroups = newPayoutGroups
                .Select(x =>
                {
                    x.PayoutBatchId = newPayoutBatch.Id;
                    var bankAccount = customerWithValidBankAcc.First(y => y.CustomerId == x.PayoutTo);

                    x.AccountNumber = bankAccount.AccountNumber;
                    x.AccountHolderName = bankAccount.AccountHolderName;
                    x.BankName = bankAccount.BankShortName;

                    return x;
                })
                .ToList();
            _payoutGroupRepo.Insert(updatedNewPayoutGroups);

            //create payout and group
            var payoutAndGroups = updatedNewPayoutGroups
                .Select(x => new PayoutAndGroup
                {
                    PayoutGroupId = x.Id,
                    RefTypeId = (int)PayoutAndGroupRefType.PayoutRequest,
                    RefId = x.PayoutTo, // Use appropriate field to identify the reference ID
                })
                .Select(x =>
                {
                    x.CreateAudit(actorId);
                    return x;
                })
                .ToList();

            _payoutAndGroupRepo.Insert(payoutAndGroups);
        }

        // Export Payout Recon & Notify If any error
        public async Task<PayoutReconciliationStatus> ProcessPayoutReconciliation(int actorId)
        {
            try
            {
                // Create payout batches
                var createPayoutBatchStatus = CreatePayoutBatch(actorId);

                if (createPayoutBatchStatus == PayoutBatchGenerationStatus.NoItemToProcess)
                {
                    _logger.Information("No payout batches to reconcile.");
                    return PayoutReconciliationStatus.NoItemToProcess;
                }

                // Load payout batches that need reconciliation
                var payoutBatches = _payoutBatchRepo.Table
                    .Where(batch => batch.Status == (int)PayoutRequestStatus.New)
                    .ToList();

                if (payoutBatches.Count == 0)
                {
                    _logger.Information("No payout batches to reconcile.");
                    return PayoutReconciliationStatus.NoItemToProcess;
                }

                var allPayoutGroups = _payoutGroupRepo.Table
                    .Where(group => payoutBatches.Select(batch => batch.Id).Contains(group.PayoutBatchId))
                    .ToList();

                // Generate payout report in Excel
                var reportPath = Path.Combine(Path.GetTempPath(), "PayoutReconciliationReport.xlsx");
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Payout Reconciliation");

                    // Add headers
                    worksheet.Cell("A1").Value = "Payout Batch ID";
                    worksheet.Cell("B1").Value = "Payout Group ID";
                    worksheet.Cell("C1").Value = "Beneficial ID";
                    worksheet.Cell("D1").Value = "Account Number";
                    worksheet.Cell("E1").Value = "Account Holder Name";
                    worksheet.Cell("F1").Value = "Bank Name";
                    worksheet.Cell("G1").Value = "Amount";
                    worksheet.Cell("H1").Value = "Status";
                    worksheet.Cell("I1").Value = "Remarks";

                    // Add data
                    var row = 2;
                    foreach (var batch in payoutBatches)
                    {
                        var groups = allPayoutGroups.Where(g => g.PayoutBatchId == batch.Id).ToList();
                        foreach (var group in groups)
                        {
                            var bankAccount = _bankAccountRepo.Table
                                .Where(ba => ba.CustomerId == group.PayoutTo && !ba.Deleted && ba.IsVerified == true)
                                .Join(_bankRepo.Table, ba => ba.BankId, b => b.Id, (ba, b) => new
                                {
                                    ba.CustomerId,
                                    ba.AccountNumber,
                                    ba.AccountHolderName,
                                    BankShortName = b.ShortName
                                })
                                .FirstOrDefault();

                            worksheet.Cell(row, 1).Value = batch.Id;
                            worksheet.Cell(row, 2).Value = group.Id;
                            worksheet.Cell(row, 3).Value = group.PayoutTo;
                            worksheet.Cell(row, 4).Value = bankAccount?.AccountNumber ?? "N/A";
                            worksheet.Cell(row, 5).Value = bankAccount?.AccountHolderName ?? "N/A";
                            worksheet.Cell(row, 6).Value = bankAccount?.BankShortName ?? "N/A";
                            worksheet.Cell(row, 7).Value = group.Amount;
                            worksheet.Cell(row, 8).Value = group.Status == (int)PayoutRequestStatus.Approved ? "Approved" : "Failed";
                            worksheet.Cell(row, 9).Value = group.Status == (int)PayoutRequestStatus.Approved ? "" : "Update Bank Information";

                            row++;
                        }
                    }

                    // Save the report
                    workbook.SaveAs(reportPath);
                }

                _logger.Information($"Payout reconciliation report generated at {reportPath}");

                // Notify payees with failed payouts
                var failedPayoutGroups = allPayoutGroups
                    .Where(group => group.Status != (int)PayoutRequestStatus.Approved)
                    .ToList();

                var failedPayees = failedPayoutGroups.Select(group => group.PayoutTo).Distinct();
                foreach (var payeeId in failedPayees)
                {
                    var email = _customerRepo.Table
                        .Where(c => c.Id == payeeId)
                        .Select(c => c.Email)
                        .FirstOrDefault();

                    if (!string.IsNullOrEmpty(email))
                    {
                        // Replace with actual notification logic
                        await _notificationService.SendNotificationAsync(
                            email,
                            "Payout Failed",
                            "Your recent payout attempt was unsuccessful. Please update your bank information."
                        );
                        _logger.Information($"Notification sent to payee {payeeId} ({email}) regarding payout failure.");
                    }
                }

                return PayoutReconciliationStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.Error($"Payout reconciliation failed: {ex.Message}");
                return PayoutReconciliationStatus.Failure;
            }
        }

        public PayoutBatchGenerationStatus CreatePayoutBatchShuq(int actorId)
        {
            //check if there are running process
            var payoutBatchLockerSetting = _settingService.LoadSetting<PayoutBatchShuqLockerSetting>();

            if (payoutBatchLockerSetting.LastGeneratingUTCDateTime.HasValue
                && payoutBatchLockerSetting.LastGeneratedUTCDateTime != payoutBatchLockerSetting.LastGeneratingUTCDateTime)
            {
                _logger.Information("CreatePayoutBatchShuq: " + JsonConvert.SerializeObject(payoutBatchLockerSetting));

                //check if running process overtime to cater stop halfway issue
                var diffInTicks = (DateTime.UtcNow
                    - payoutBatchLockerSetting.LastGeneratingUTCDateTime.Value.AddSeconds(_payoutRequestSettings.MaxPayoutGenerationSeconds)).Ticks;
                if (diffInTicks < 0)
                {
                    return PayoutBatchGenerationStatus.InProcess;
                }
            }

            //begin lock with generating utc date time
            payoutBatchLockerSetting.LastGeneratingUTCDateTime = DateTime.UtcNow;
            _settingService.SaveSetting(payoutBatchLockerSetting, x => x.LastGeneratingUTCDateTime, clearCache: true);

            try
            {
                //get local time now
                var timezone = _dateTimeHelper.DefaultStoreTimeZone;
                var hoursDiff = timezone.BaseUtcOffset.TotalHours;
                var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
                var scheduledExecuteTime = localDateTime.Date;
                var payoutDate = localDateTime.Date;
                var endDateCutoff = _payoutBatchSettings.EndDateCutoff >= 0 && _payoutBatchSettings.EndDateCutoff < 6 ? _payoutBatchSettings.EndDateCutoff : 2;

                //cut off for 7, 14, 21 and 28 of the month
                if (scheduledExecuteTime.Day >= (28-endDateCutoff))
                {
                    payoutDate = (new DateTime(payoutDate.Year, payoutDate.Month, 28)).AddDays(-endDateCutoff);
                }
                else if (scheduledExecuteTime.Day >= (21-endDateCutoff))
                {
                    payoutDate = (new DateTime(payoutDate.Year, payoutDate.Month, 21)).AddDays(-endDateCutoff);
                }
                else if (scheduledExecuteTime.Day >= (14-endDateCutoff))
                {
                    payoutDate = (new DateTime(payoutDate.Year, payoutDate.Month, 14)).AddDays(-endDateCutoff);
                }
                else if (scheduledExecuteTime.Day >= (7-endDateCutoff))
                {
                    payoutDate = (new DateTime(payoutDate.Year, payoutDate.Month, 7)).AddDays(-endDateCutoff);
                }
                else
                {
                    payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 28).AddMonths(-1);
                }

                var payoutUTCDate = payoutDate.AddHours(hoursDiff * -1);
                //get approved payout request list
                var payoutRequests =
                    (
                    from pr in _orderPayoutRequestRepo.Table
                    .Where(pr =>
                        pr.Deleted == false
                        && pr.TransactionDate < payoutUTCDate
                        && pr.OrderPayoutStatusId == (int)OrderPayoutStatus.Pending
                    )
                    from pg in
                        (from png in _payoutAndGroupRepo.Table
                        .Where(png =>
                            png.Deleted == false
                            && png.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest
                            && png.RefId == pr.Id)
                         from pg in _payoutGroupRepo.Table
                         .Where(pg =>
                             pg.Deleted == false
                             && pg.Id == png.PayoutGroupId
                             && pg.Status != (int)PayoutGroupStatus.Error)
                         select new
                         {
                             pg.Id,
                             png.RefId
                         })
                        .Where(p => p.RefId == pr.Id)
                    .DefaultIfEmpty()
                    select new
                    {
                        pr,
                        pg
                    })
                    .Where(x => x.pg == null)
                    .Select(x => x.pr)
                    .ToList();

                //get refund request list
                var refundRequests =
                    (
                    from rr in _orderRefundRequestRepo.Table
                    .Where(rr =>
                        rr.Deleted == false &&
                        rr.TransactionDate < payoutUTCDate &&
                        rr.RefundStatusId == (int)RefundStatus.New
                    )
                    from pg in
                        (from png in _payoutAndGroupRepo.Table
                        .Where(png =>
                            png.Deleted == false
                            && png.RefTypeId == (int)PayoutAndGroupRefType.OrderRefundRequest
                            && png.RefId == rr.Id)
                         from pg in _payoutGroupRepo.Table
                         .Where(pg =>
                             pg.Deleted == false
                             && pg.Id == png.PayoutGroupId
                             && pg.Status != (int)PayoutGroupStatus.Error)
                         select new
                         {
                             pg.Id,
                             png.RefId
                         })
                        .Where(p => p.RefId == rr.Id)
                    .DefaultIfEmpty()
                    select new
                    {
                        rr,
                        pg
                    })
                    .Where(x => x.pg == null)
                    .Select(x => x.rr)
                    .ToList();

                //if there is at least 1 payout request or refund request
                if (payoutRequests.Count > 0 || refundRequests.Count > 0)
                {
                    var sstSettings = _settingService.LoadSetting<SSTSettings>();
                    // add payout service charges into payoutRequests
                    payoutRequests = payoutRequests
                        .Where(x=>x.ServiceChargesUTC == null)
                        .GroupBy(
                        x => x.VendorCustomerId,
                        (vendorCustomerId, reqs) => new
                        {
                            VendorCustomerId = vendorCustomerId,
                            OrderCount = reqs.Sum(y => y.ChargablePayoutAmount() > 0 ? 1 : 0),
                            allReqs = reqs
                        }
                        )
                        .ToList()
                        .Select(x => {
                            var charge = _chargeService.GetLatestCharge((int)ProductType.ShuqOrderPayoutServiceFee, 0, x.OrderCount);

                            return new
                            {
                                VendorCustomerId = x.VendorCustomerId,
                                OrderCount = x.OrderCount,
                                allReqs = x.allReqs,
                                PayoutChargesRate = charge
                            };
                        })
                        .ToList()
                        .SelectMany(x => {
                            return x.allReqs.Select(y =>
                            {
                                y.ServiceChargeRate = x.PayoutChargesRate.ValueType == (int)ChargeValueType.Rate
                                    ? (decimal?)x.PayoutChargesRate.Value 
                                    : null;

                                y.ServiceCharge =
                                    (y.ProductPriceExclTax - y.RefundAmount) >  0
                                    ? Math.Round(
                                    x.PayoutChargesRate.ValueType == (int)ChargeValueType.Rate 
                                        ? (y.ChargablePayoutAmount()) * y.ServiceChargeRate.Value
                                        : (y.ChargablePayoutAmount()) > x.PayoutChargesRate.Value
                                            ? x.PayoutChargesRate.Value
                                            : (y.ChargablePayoutAmount())
                                    , 2)
                                    : 0;

                                y.ServiceChargesUTC = DateTime.UtcNow;

                                _campaignProcessingService.Apply(y.VendorCustomerId, y);
                                if(y.ServiceCharge < 0)
                                {
                                    y.ServiceCharge = 0;
                                }

                                if (y.ServiceCharge > 0)
                                {
                                    y.ServiceChargeSST = Math.Round(
                                    sstSettings.HasSST
                                        ? y.ServiceCharge.Value * sstSettings.SSTRate
                                        : 0
                                    , 2);
                                }
                                else
                                {
                                    y.ServiceChargeSST = 0;
                                }


                                return y;
                            })
                            .ToList();
                        })
                       .ToList();

                    _orderPayoutRequestRepo.Update(payoutRequests);

                    //generate payout and refund list
                    var payoutAndRefundRequests = new List<dynamic>()
                        .Concat(payoutRequests.Select(x => new
                        {
                            Id = x.Id,
                            Amount = x.GrossPayoutAmount() - x.ServiceCharge.Value - x.ServiceChargeSST.Value,
                            RefTypeId = (int)PayoutAndGroupRefType.OrderPayoutRequest,
                            BeneficialId = x.VendorCustomerId
                        })
                            .ToList()
                        )
                        .Concat(refundRequests.Select(x => new
                        {
                            Id = x.Id,
                            Amount = x.Amount,
                            RefTypeId = (int)PayoutAndGroupRefType.OrderRefundRequest,
                            BeneficialId = x.CustomerId
                        })
                            .ToList()
                        )
                        .ToList();

                    //group payout by payee
                    var newPayoutGroupsBeforeSplit = payoutAndRefundRequests
                        .GroupBy(
                        x => x.BeneficialId,
                        (beneficialId, reqs) => new PayoutGroup
                        {
                            Amount = reqs.Sum(y => (decimal)y.Amount),
                            Status = (int)PayoutRequestStatus.New,
                            PayoutTo = beneficialId
                        }
                        )
                        .ToList()
                        .Select(x =>
                        {
                            x.CreateAudit(actorId);
                            return x;
                        })
                        .ToList();

                    //filter payout with position total amount
                    newPayoutGroupsBeforeSplit = newPayoutGroupsBeforeSplit
                        .Where(x => x.Amount > 0)
                        .ToList();

                    // filter customer without valid bankAccount
                    var payoutTos = newPayoutGroupsBeforeSplit.Select(x => x.PayoutTo).ToList();
                    var customerWithValidBankAcc =
                        (from ba in _bankAccountRepo.Table
                        .Where(x => !x.Deleted
                        && x.IsVerified != null
                        && x.IsVerified.Value
                        && payoutTos.Contains(x.CustomerId))
                         from b in _bankRepo.Table
                         .Where(b => b.Id == ba.BankId)
                         select new
                         {
                             ba,
                             b
                         }
                        )
                        .Select(x => new
                        {
                            x.ba.CustomerId,
                            x.ba.AccountNumber,
                            x.ba.AccountHolderName,
                            //BankName = x.b.Name,
                            BankShortName = x.b.ShortName
                        })
                        .ToList();

                    newPayoutGroupsBeforeSplit = newPayoutGroupsBeforeSplit
                        .Where(x => customerWithValidBankAcc.Any(y => y.CustomerId == x.PayoutTo))
                        .ToList();

                    //if no payout group skip
                    if (newPayoutGroupsBeforeSplit.Count <= 0)
                    {
                        return PayoutBatchGenerationStatus.NoItemToProcess;
                    }

                    //split payout file in batches according to max amount and max record
                    var currentIdx = 0;
                    var splittedNewPayoutGroupsArr = new List<List<PayoutGroup>> { new List<PayoutGroup>() };
                    foreach (var item in newPayoutGroupsBeforeSplit)
                    {
                        if ((splittedNewPayoutGroupsArr.ElementAt(currentIdx).Sum(x => x.Amount) + item.Amount) > _payoutRequestSettings.CsvMaxTotalAmount
                            || splittedNewPayoutGroupsArr.ElementAt(currentIdx).Count + 1 > _payoutRequestSettings.CsvMaxRecord)
                        {
                            currentIdx += 1;
                            splittedNewPayoutGroupsArr.Add(new List<PayoutGroup>());
                        }
                        splittedNewPayoutGroupsArr.ElementAt(currentIdx).Add(item);
                    }

                    //create 
                    foreach (var newPayoutGroups in splittedNewPayoutGroupsArr)
                    {
                        // create payout batch
                        var newPayoutBatch = new PayoutBatch
                        {
                            GeneratedDateTime = DateTime.UtcNow,
                            Status = (int)PayoutRequestStatus.New,
                            Platform = Platform.Shuq
                        };
                        newPayoutBatch.CreateAudit(actorId);
                        _payoutBatchRepo.Insert(newPayoutBatch);

                        //create payout group
                        var updatedNewPayoutGroups = newPayoutGroups
                            .Select(x =>
                            {
                                x.PayoutBatchId = newPayoutBatch.Id;
                                var bankAccount = customerWithValidBankAcc.Where(y => y.CustomerId == x.PayoutTo).First();

                                x.AccountNumber = bankAccount.AccountNumber;
                                x.AccountHolderName = bankAccount.AccountHolderName;
                                x.BankName = bankAccount.BankShortName;

                                return x;
                            }
                            )
                            .ToList();

                        foreach(var payoutGroup in updatedNewPayoutGroups)
                        {
                            _payoutGroupRepo.Insert(payoutGroup);
                        }

                        //create payout and group
                        var validPayoutRequests = payoutAndRefundRequests
                            .Where(x => updatedNewPayoutGroups.Any(y => y.PayoutTo == x.BeneficialId))
                            .ToList();

                        var payoutAndGroups = validPayoutRequests
                            .Select(x => new PayoutAndGroup
                            {
                                PayoutGroupId = updatedNewPayoutGroups
                                    .Where(y => y.PayoutTo == x.BeneficialId)
                                    .Select(x => x.Id)
                                    .First(),
                                RefTypeId = x.RefTypeId,
                                RefId = x.Id,
                            })
                            .Select(x =>
                            {
                                x.CreateAudit(actorId);
                                return x;
                            })
                            .ToList();

                        _payoutAndGroupRepo.Insert(payoutAndGroups);
                    }

                    // update charges for orderPayoutRequest
                    var payoutRequestToUpdate = payoutRequests
                        .Where(x => customerWithValidBankAcc.Any(y => y.CustomerId == x.CustomerId))
                        .ToList();
                    _orderPayoutRequestRepo.Update(payoutRequestToUpdate);
                }

                return PayoutBatchGenerationStatus.Success;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                //end process
                payoutBatchLockerSetting.LastGeneratedUTCDateTime = payoutBatchLockerSetting.LastGeneratingUTCDateTime;

                _settingService.SaveSetting(payoutBatchLockerSetting, x => x.LastGeneratedUTCDateTime, clearCache: true);
            }
        }

        public virtual PayoutBatchDTO GetPayoutBatchDetails(int batchId)
        {
            var batchDTO = (from b in _payoutBatchRepo.Table
                            where !b.Deleted && b.Id == batchId
                            orderby b.CreatedOnUTC descending
                            select new PayoutBatchDTO
                            {
                                Id = b.Id,
                                GeneratedDateTime = b.GeneratedDateTime,
                                DownloadDateTime = b.DownloadDateTime,
                                ReconDateTime = b.ReconDateTime,
                                Status = b.Status,
                                StatusRemarks = b.StatusRemarks,
                                ReconFileDownloadId = b.ReconFileDownloadId,
                                CreatedOnUTC = b.CreatedOnUTC,
                                UpdatedOnUTC = b.UpdatedOnUTC,
                                CreatedById = b.CreatedById,
                                UpdatedById = b.UpdatedById,
                                PayoutBatchNumber = $"PB{b.Id}",
                                PlatformId = b.PlatformId
                            }).FirstOrDefault();

            if (batchDTO != null)
            {
                var groups = _payoutGroupRepo.Table.Where(x => !x.Deleted && batchDTO.Id == x.PayoutBatchId).ToList();

                var batchGroups = groups.Where(y => y.PayoutBatchId == batchDTO.Id).ToList();
                batchDTO.PayoutGroupCount = batchGroups.Count();
                batchDTO.Amount = batchGroups.Sum(y => y.Amount);
            }

            return batchDTO;
        }

        public virtual PagedList<PayoutBatchDTO> GetPayoutBatchs(int pageIndex = 0, int pageSize = int.MaxValue, PayoutBatchFilterDTO filterDTO = null)
        {
            var query = from b in _payoutBatchRepo.Table
                        where !b.Deleted
                        from d in _downloadRepo.Table
                        .Where(d => d.Id == b.ReconFileDownloadId)
                        .DefaultIfEmpty()
                        orderby b.CreatedOnUTC descending
                        select new PayoutBatchDTO
                        {
                            Id = b.Id,
                            GeneratedDateTime = b.GeneratedDateTime,
                            DownloadDateTime = b.DownloadDateTime,
                            ReconDateTime = b.ReconDateTime,
                            Status = b.Status,
                            PlatformId = b.PlatformId,
                            StatusRemarks = b.StatusRemarks,
                            ReconFileDownloadId = b.ReconFileDownloadId,
                            CreatedOnUTC = b.CreatedOnUTC,
                            UpdatedOnUTC = b.UpdatedOnUTC,
                            CreatedById = b.CreatedById,
                            UpdatedById = b.UpdatedById,
                            PayoutBatchNumber = $"PB{b.Id}",
                            ReconFileDownloadGuid = d != null ? (Guid?)d.DownloadGuid : null
                        };

            if (filterDTO != null)
            {
                if (filterDTO.GeneratedDate != null) { query = query.Where(x => x.CreatedOnUTC.Date == filterDTO.GeneratedDate.Value.Date); }
                if (filterDTO.Status != null && filterDTO.Status != 0) { query = query.Where(x => x.Status == filterDTO.Status); }
            }

            var totalCount = query.Count();

            var finalQuery = query
                        .Skip(pageSize * pageIndex)
                        .Take(pageSize);

            var records = finalQuery.ToList();

            if (records.Count > 0)
            {
                var batchIds = records.Select(x => x.Id).ToList();
                var groups = _payoutGroupRepo.Table.Where(x => !x.Deleted && batchIds.Contains(x.PayoutBatchId)).ToList();
                foreach (var batchDTO in records)
                {
                    var batchGroups = groups.Where(y => y.PayoutBatchId == batchDTO.Id).ToList();
                    batchDTO.PayoutGroupCount = batchGroups.Count();
                    batchDTO.Amount = batchGroups.Sum(y => y.Amount);
                }
            }

            var data = new PagedList<PayoutBatchDTO>(records, pageIndex, pageSize, totalCount);


            return data;
        }

        public virtual PayoutGroupDTO GetPayoutGroupsDetails(int groupId)
        {
            var groupDTO = (from g in _payoutGroupRepo.Table
                            where !g.Deleted && g.Id == groupId
                            orderby g.CreatedOnUTC descending
                            select new PayoutGroupDTO
                            {
                                PayoutGroupId = g.Id,
                                Amount = g.Amount,
                                AccountNumber = g.AccountNumber,
                                AccountHolderName = g.AccountHolderName,
                                BankName = g.BankName,
                                Status = g.Status,
                                PayoutTo = g.PayoutTo,
                                Remarks = g.Remarks,
                                PayoutBatchId = g.PayoutBatchId,
                                CreatedOnUTC = g.CreatedOnUTC,
                                UpdatedOnUTC = g.UpdatedOnUTC,
                                CreatedById = g.CreatedById,
                                UpdatedById = g.UpdatedById,
                                CreatedOn = _dateTimeHelper.ConvertToUserTime(g.CreatedOnUTC, DateTimeKind.Utc),
                                UpdatedOn = g.UpdatedOnUTC != null ? _dateTimeHelper.ConvertToUserTime(g.UpdatedOnUTC.Value, DateTimeKind.Utc) : g.UpdatedOnUTC
                            }).FirstOrDefault();


            if (groupDTO != null)
            {
                var payoutAndGroups = _payoutAndGroupRepo.Table.Where(x => !x.Deleted && x.PayoutGroupId == groupDTO.PayoutGroupId).ToList();
                var bankAccounts = (from ba in _bankAccountRepo.Table
                                    join b in _bankRepo.Table on ba.BankId equals b.Id
                                    where !ba.Deleted && ba.CustomerId == groupDTO.PayoutTo
                                    select new BankAccountDTO
                                    {
                                        BankId = ba.BankId,
                                        BankName = b.Name,
                                        AccountNumber = ba.AccountNumber,
                                    })
                                   .ToList();
                var batchGroups = payoutAndGroups.Where(x => x.PayoutGroupId == groupDTO.PayoutGroupId).ToList();
                groupDTO.RequestCount = batchGroups.Count();
                groupDTO.BankAccount = bankAccounts.Where(x => x.CustomerId == groupDTO.PayoutTo).FirstOrDefault();

                groupDTO.IndividualProfile = _individualProfileRepo.Table
                    .Where(x => !x.Deleted && x.CustomerId == groupDTO.PayoutTo)
                    .Select(x => new IndividualProfileDTO
                    {
                        CustomerId = x.CustomerId,
                        FullName = x.FullName,
                    })
                    .FirstOrDefault();
                if (groupDTO.IndividualProfile == null)
                {
                    groupDTO.OrganizationProfile = _organizationProfileRepo.Table
                        .Where(x => !x.Deleted && x.CustomerId == groupDTO.PayoutTo)
                        .Select(x => new OrganizationProfileDTO
                        {
                            CustomerId = x.CustomerId,
                            Name = x.Name,
                        })
                        .FirstOrDefault();
                    if (groupDTO.OrganizationProfile == null)
                    {
                        var customers = _customerRepo.Table
                            .Where(x => !x.Deleted && x.Id == groupDTO.PayoutTo)
                            .Select(x => new Customer
                            {
                                Id = x.Id,
                                Email = x.Email,
                            })
                            .FirstOrDefault();
                    }
                }
            }

            return groupDTO;
        }

        public virtual PagedList<PayoutGroupDTO> GetPayoutGroupsByBatchId(int batchId, int pageIndex = 0, int pageSize = int.MaxValue, PayoutGroupFilterDTO filterDTO = null)
        {
            var query = from g in _payoutGroupRepo.Table
                        where !g.Deleted && g.PayoutBatchId == batchId
                        orderby g.CreatedOnUTC descending
                        select new PayoutGroupDTO
                        {
                            Id = g.Id,
                            Amount = g.Amount,
                            AccountNumber = g.AccountNumber,
                            AccountHolderName = g.AccountHolderName,
                            BankName = g.BankName,
                            Status = g.Status,
                            PayoutTo = g.PayoutTo,
                            Remarks = g.Remarks,
                            CreatedOnUTC = g.CreatedOnUTC,
                            UpdatedOnUTC = g.UpdatedOnUTC,
                            CreatedById = g.CreatedById,
                            UpdatedById = g.UpdatedById,
                            PayoutBatchId = g.PayoutBatchId
                        };

            if (filterDTO != null)
            {
                if (filterDTO.CustomerId != null && filterDTO.CustomerId != 0) { query = query.Where(x => filterDTO.CustomerId == x.PayoutTo); }
                if (filterDTO.Status != null && filterDTO.Status != 0) { query = query.Where(x => x.Status == filterDTO.Status); }
            }

            var totalCount = query.Count();

            var finalQuery = query
                        .Take(pageSize)
                        .Skip(pageSize * pageIndex);

            var records = finalQuery.ToList();

            if (records.Count > 0)
            {
                var groupIds = records.Select(x => x.Id).ToList();
                var payoutAndGroups = _payoutAndGroupRepo.Table.Where(x => !x.Deleted && groupIds.Contains(x.PayoutGroupId)).ToList();
                var customerIds = records.Select(x => x.PayoutTo).ToList();
                var individualProfiles = _individualProfileRepo.Table
                    .Where(x => !x.Deleted && customerIds.Contains(x.CustomerId))
                    .Select(x => new IndividualProfileDTO
                    {
                        CustomerId = x.CustomerId,
                        FullName = x.FullName,
                    })
                    .ToList();
                var organizationProfiles = _organizationProfileRepo.Table
                    .Where(x => !x.Deleted && customerIds.Where(x => !individualProfiles.Select(y => y.Id).ToList().Contains(x)).ToList().Contains(x.CustomerId))
                    .Select(x => new OrganizationProfileDTO
                    {
                        CustomerId = x.CustomerId,
                        Name = x.Name,
                    })
                    .ToList();
                var customers = _customerRepo.Table
                    .Where(x => !x.Deleted && customerIds.Where(x =>
                        !individualProfiles.Select(y => y.Id).ToList().Contains(x)
                        && !organizationProfiles.Select(y => y.Id).ToList().Contains(x)
                     ).ToList().Contains(x.Id)
                     )
                    .Select(x => new Customer
                    {
                        Id = x.Id,
                        Email = x.Email,
                    })
                    .ToList();
                var bankAccounts = (from ba in _bankAccountRepo.Table
                                    join b in _bankRepo.Table on ba.BankId equals b.Id
                                    where !ba.Deleted && customerIds.Contains(ba.CustomerId)
                                    select new BankAccountDTO
                                    {
                                        CustomerId = ba.CustomerId,
                                        BankId = ba.BankId,
                                        BankName = b.Name,
                                        AccountNumber = ba.AccountNumber,
                                    })
                                   .ToList();
                foreach (var groupDTO in records)
                {
                    var batchGroups = payoutAndGroups.Where(x => x.PayoutGroupId == groupDTO.Id).ToList();
                    groupDTO.RequestCount = batchGroups.Count();
                    groupDTO.BankAccount = _bankAccountServie.GetBankAccountByCustomerId(groupDTO.PayoutTo);
                    groupDTO.IndividualProfile = individualProfiles.Where(x => x.CustomerId == groupDTO.PayoutTo).FirstOrDefault();
                    groupDTO.OrganizationProfile = organizationProfiles.Where(x => x.CustomerId == groupDTO.PayoutTo).FirstOrDefault();
                    groupDTO.Customer = customers.Where(x => x.Id == groupDTO.PayoutTo).FirstOrDefault();
                }
            }

            var data = new PagedList<PayoutGroupDTO>(records, pageIndex, pageSize, totalCount);

            return data;
        }

        public virtual PagedList<PayoutAndGroupDTO> GetRequestByGroupId(int groupId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from pg in _payoutAndGroupRepo.Table
                        where !pg.Deleted && pg.PayoutGroupId == groupId
                        orderby pg.CreatedOnUTC descending
                        select new PayoutAndGroupDTO
                        {
                            Id = pg.Id,
                            PayoutGroupId = pg.PayoutGroupId,
                            RefTypeId = pg.RefTypeId,
                            RefId = pg.RefId,
                            CreatedOnUTC = pg.CreatedOnUTC,
                            UpdatedOnUTC = pg.UpdatedOnUTC,
                            CreatedById = pg.CreatedById,
                            UpdatedById = pg.UpdatedById,
                        };


            var totalCount = query.Count();

            var finalQuery = query
                        .Take(pageSize)
                        .Skip(pageSize * pageIndex);

            var records = finalQuery.ToList();

            if (records.Count > 0)
            {
                var payoutReqIds = records.Where(x => x.RefTypeId == (int)PayoutAndGroupRefType.PayoutRequest).Select(x => x.RefId).ToList();
                var refundReqIds = records.Where(x => x.RefTypeId == (int)PayoutAndGroupRefType.RefundRequest).Select(x => x.RefId).ToList();
                var orderPayoutReqIds = records.Where(x => x.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest).Select(x => x.RefId).ToList();
                var orderRefundReqIds = records.Where(x => x.RefTypeId == (int)PayoutAndGroupRefType.OrderRefundRequest).Select(x => x.RefId).ToList();

                var payoutReqs = _payoutRequestRepo.Table.Where(x => !x.Deleted && payoutReqIds.Contains(x.Id)).ToList().Select(x => _mapper.Map<PayoutRequestDTO>(x)).ToList();
                var refundReqs = _refundRequestRepo.Table.Where(x => !x.Deleted && refundReqIds.Contains(x.Id)).ToList().Select(x => _mapper.Map<RefundRequestDTO>(x)).ToList();
                var orderPayoutReqs = _orderPayoutRequestRepo.Table.Where(x => !x.Deleted && orderPayoutReqIds.Contains(x.Id)).ToList()/*.Select(x => _mapper.Map<OrderPayoutRequestDTO>(x))*/.ToList();
                var orderRefundReqs = _orderRefundRequestRepo.Table.Where(x => !x.Deleted && orderRefundReqIds.Contains(x.Id)).ToList()/*.Select(x => _mapper.Map<OrderRefundRequestDTO>(x))*/.ToList();

                var orderItemIds = new List<int>()
                    .Concat(payoutReqs.Select(x => x.OrderItemId).ToList())
                    .Concat(refundReqs.Select(x => x.OrderItemId).ToList());
                var orderItems = from oi in _proOrderItemRepo.Table
                                 join o in _proOrderRepo.Table.Where(x => !x.Deleted) on oi.OrderId equals o.Id
                                 where !oi.Deleted && orderItemIds.Contains(oi.Id)
                                 select new
                                 {
                                     OrderItemId = oi.Id,
                                     CustomOrderNumber = o.CustomOrderNumber
                                 };

                var shuqOrderItems = new List<int>()
                    .Concat(orderPayoutReqs.Select(x => x.OrderId).ToList())
                    .Concat(orderRefundReqs.Select(x => x.OrderId).ToList());
                var shuqOrders = from oi in _shuqOrderRepo.Table
                                 where !oi.Deleted && shuqOrderItems.Contains(oi.Id)
                                 select new
                                 {
                                     OrderId = oi.Id,
                                     CustomOrderNumber = oi.CustomOrderNumber
                                 };

                foreach (var pg in records)
                {
                    if (pg.RefTypeId == (int)PayoutAndGroupRefType.PayoutRequest)
                    {
                        pg.PayoutRequest = payoutReqs.Where(x => x.Id == pg.RefId).FirstOrDefault();
                        pg.CustomOrderNumber = orderItems.Where(x => x.OrderItemId == pg.PayoutRequest.OrderItemId).Select(x => x.CustomOrderNumber).FirstOrDefault();
                    }
                    else if (pg.RefTypeId == (int)PayoutAndGroupRefType.RefundRequest)
                    {
                        pg.RefundRequest = refundReqs.Where(x => x.Id == pg.RefId).FirstOrDefault();
                        pg.CustomOrderNumber = orderItems.Where(x => x.OrderItemId == pg.RefundRequest.OrderItemId).Select(x => x.CustomOrderNumber).FirstOrDefault();
                    }
                    else if (pg.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest)
                    {
                        pg.OrderPayoutRequest = orderPayoutReqs.Where(x => x.Id == pg.RefId).FirstOrDefault();
                        pg.CustomOrderNumber = shuqOrders.Where(x => x.OrderId == pg.OrderPayoutRequest.OrderId).Select(x => x.CustomOrderNumber).FirstOrDefault();
                    }
                    else if (pg.RefTypeId == (int)PayoutAndGroupRefType.OrderRefundRequest)
                    {
                        pg.OrderRefundRequest = orderRefundReqs.Where(x => x.Id == pg.RefId).FirstOrDefault();
                        pg.CustomOrderNumber = shuqOrders.Where(x => x.OrderId == pg.OrderRefundRequest.OrderId).Select(x => x.CustomOrderNumber).FirstOrDefault();
                    }
                }
            }

            var data = new PagedList<PayoutAndGroupDTO>(records, pageIndex, pageSize, totalCount);

            return data;
        }

        public virtual void UpdatePayoutBatch(PayoutBatchDTO dto)
        {
            var model = _payoutBatchRepo.Table
                .Where(x => x.Id == dto.Id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException("Payout batch not found.");
            }

            model.Status = dto.Status;
            model.DownloadDateTime = dto.DownloadDateTime;

            _payoutBatchRepo.Update(model);
        }

        public virtual void UpdatePayoutBatchStatus(int actorId, PayoutBatchDTO dto)
        {
            var model = _payoutBatchRepo.Table
                .Where(x => x.Id == dto.Id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException("Payout batch not found.");
            }

            if (model.Status == (int)PayoutBatchStatus.Success
                || model.Status == (int)PayoutBatchStatus.Error)
            {
                throw new InvalidOperationException("The payout batch no longer allow to update.");
            }

            model.Status = dto.Status;

            if (model.Status == (int)PayoutBatchStatus.Success
                || model.Status == (int)PayoutBatchStatus.Error
                || model.Status == (int)PayoutBatchStatus.Fail)
            {
                model.ReconFileDownloadId = dto.ReconFileDownloadId;
                model.ReconDateTime = DateTime.UtcNow;
                model.StatusRemarks = dto.StatusRemarks;
            }

            model.UpdateAudit(actorId);

            _payoutBatchRepo.Update(model);
        }

        public virtual void UpdatePayoutGroup(PayoutGroupDTO dto)
        {
            var request = _mapper.Map<PayoutGroup>(dto);
            _payoutGroupRepo.Update(request);
        }

        public virtual void UpdatePayoutGroupStatus(int actorId, PayoutGroupDTO dto)
        {
            var model = _payoutGroupRepo.Table
                .Where(x => x.Id == dto.Id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException("Payout group not found.");
            }

            model.Status = dto.Status;
            model.Remarks = dto.Remarks;
            model.UpdateAudit(actorId);

            _payoutGroupRepo.Update(model);
        }

        public virtual void AllocateInvoiceToOrderPayoutRequest(int actorId)
        {
            var paidPayoutRequests = _orderPayoutRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.OrderPayoutStatusId == (int)OrderPayoutStatus.Paid
                && x.InvoiceId == null)
                .ToList();

            var paidPayoutRequestIds = paidPayoutRequests.Select(x => x.Id)
                .ToList();

            var payoutGroups =
                (from pag in _payoutAndGroupRepo.Table
                .Where(pag => pag.Deleted == false
                && pag.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest
                && paidPayoutRequestIds.Contains(pag.RefId))
                 from pg in _payoutGroupRepo.Table
                 .Where(pg => pg.Deleted == false
                 && pg.Id == pag.PayoutGroupId
                 && pg.Status == (int)PayoutGroupStatus.Success)
                 select new
                 {
                     pag.PayoutGroupId,
                     OrderPayoutRequestId = pag.RefId,
                     pg.PayoutTo
                 })
                .ToList();

            var payoutGroupPayouts = payoutGroups.Select(x => new
            {
                x.PayoutGroupId,
                x.PayoutTo
            }).Distinct().ToList();

            var invoiceNumbers = _documentNumberService.GetDocumentNumbers(
                RunningNumberType.ShuqInvoice,
                payoutGroupPayouts.Count);

            var invoices = invoiceNumbers.Select((x, i) => 
                new Invoice
                {
                    InvoiceNumber = x,
                    InvoiceTo = payoutGroupPayouts[i].PayoutTo
                })
                .ToList();

            invoices
                .ForEach(x =>
                {
                    x.CreateAudit(actorId);
                    _invoiceRepo.Insert(x);
                });

            for(int i = 0; i< payoutGroupPayouts.Count; i++)
            {
                var payoutGroup = payoutGroupPayouts[i];
                var invoice = invoices[i];

                var orderPayoutRequestIds = payoutGroups
                    .Where(x => x.PayoutGroupId == payoutGroup.PayoutGroupId)
                    .ToList();

                paidPayoutRequests
                    .Where(x => orderPayoutRequestIds.Any(y => y.OrderPayoutRequestId == x.Id))
                    .ToList()
                    .ForEach(x =>
                    {
                        x.InvoiceId = invoice.Id;
                        x.UpdateAudit(actorId);
                    });
            }

            //update payout request
            _orderPayoutRequestRepo.Update(paidPayoutRequests);
        }

        public virtual PagedList<PayoutVendorDTO> GetPayoutVendorList(int vendorId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from pg in _payoutGroupRepo.Table
                        join c in _customerRepo.Table on pg.PayoutTo equals c.Id
                        join pag in _payoutAndGroupRepo.Table on pg.Id equals pag.PayoutGroupId
                        join opr in _orderPayoutRequestRepo.Table on pag.RefId equals opr.Id
                        where c.VendorId == vendorId &&
                        pag.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest &&
                        !pg.Deleted
                        orderby pg.Id descending
                        group new {OrderPayoutRequestId =  opr.Id} by new {pg.CreatedOnUTC, pg.Status, pg.Amount, pg.Id}
                        into payoutVendor
                        select new PayoutVendorDTO
                        {
                            Date = payoutVendor.Key.CreatedOnUTC,
                            StatusId = payoutVendor.Key.Status,
                            Amount = payoutVendor.Key.Amount,
                            PayoutGroupId = payoutVendor.Key.Id,
                            NumberOfOrders = payoutVendor.Sum(x => x.OrderPayoutRequestId != default ? 1 : 0)
                        };

            return new PagedList<PayoutVendorDTO>(query, pageIndex, pageSize);
        }

        public virtual PagedList<PayoutVendorDTO> GetPayoutVendorOrderList(int payoutGroupId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query1 = from pg in _payoutGroupRepo.Table
                        join c in _customerRepo.Table on pg.PayoutTo equals c.Id
                        join pag in _payoutAndGroupRepo.Table on pg.Id equals pag.PayoutGroupId
                        join opr in _orderPayoutRequestRepo.Table on pag.RefId equals opr.Id
                        where pg.Id == payoutGroupId &&
                        pag.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest
                        select opr.OrderId;

            var orderList = query1.ToList();

            var query2 = from o in _shuqOrderRepo.Table
                         where orderList.Contains(o.Id)
                         select new PayoutVendorDTO
                         {
                             OrderId = o.Id,
                             Date = o.CreatedOnUtc,
                             Amount = o.OrderTotal
                         };

            return new PagedList<PayoutVendorDTO>(query2, pageIndex, pageSize);
        }

        public virtual IPagedList<PayoutVendorDTO> GetPayoutVendors(int vendorId, PayoutGroupStatus? status, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from pg in _payoutGroupRepo.Table
                        join c in _customerRepo.Table on pg.PayoutTo equals c.Id
                        join pag in _payoutAndGroupRepo.Table on pg.Id equals pag.PayoutGroupId
                        join opr in _orderPayoutRequestRepo.Table on pag.RefId equals opr.Id
                        join pb in _payoutBatchRepo.Table on pg.PayoutBatchId equals pb.Id
                        where c.VendorId == vendorId &&
                        pag.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest &&
                        !pg.Deleted
                        
                        orderby pg.Id descending
                        group new { OrderPayoutRequestId = opr.Id } by new { pg.CreatedOnUTC, pg.Status, pg.Amount, pg.Id, PayoutBatchId = pb.Id}
                        into payoutVendor
                        select new PayoutVendorDTO
                        {
                            Date = payoutVendor.Key.CreatedOnUTC,
                            StatusId = payoutVendor.Key.Status,
                            Amount = payoutVendor.Key.Amount,
                            PayoutGroupId = payoutVendor.Key.Id,
                            NumberOfOrders = payoutVendor.Sum(x => x.OrderPayoutRequestId != default ? 1 : 0),
                            PayoutBatchId = payoutVendor.Key.PayoutBatchId
                        };

            if (status != null)
            {
                query = query.Where(x => x.StatusId == (int)status);
            }

            return new PagedList<PayoutVendorDTO>(query, pageIndex, pageSize);
        }

        public virtual IPagedList<PayoutVendorDTO> GetPayoutVendorOrders(int payoutGroupId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query1 = from pg in _payoutGroupRepo.Table
                         join c in _customerRepo.Table on pg.PayoutTo equals c.Id
                         join pag in _payoutAndGroupRepo.Table on pg.Id equals pag.PayoutGroupId
                         join opr in _orderPayoutRequestRepo.Table on pag.RefId equals opr.Id
                         where pg.Id == payoutGroupId &&
                         pag.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest
                         select opr.OrderId;

            var orderList = query1.ToList();

            var query2 = from o in _shuqOrderRepo.Table
                         where orderList.Contains(o.Id)
                         select new PayoutVendorDTO
                         {
                             OrderId = o.Id,
                             Date = o.CreatedOnUtc,
                             Amount = o.OrderTotal
                         };

            return new PagedList<PayoutVendorDTO>(query2, pageIndex, pageSize);
        }

        public virtual decimal GetPayoutVendorsTotal(int vendorId, PayoutGroupStatus? status)
        {
            var query = from pg in _payoutGroupRepo.Table
                        join c in _customerRepo.Table on pg.PayoutTo equals c.Id
                        join pag in _payoutAndGroupRepo.Table on pg.Id equals pag.PayoutGroupId
                        where c.VendorId == vendorId &&
                        pag.RefTypeId == (int)PayoutAndGroupRefType.OrderPayoutRequest &&
                        !pg.Deleted
                        group new { payoutGroupAmount = pg.Id } by new { pg.Amount, pg.Status }
                        into payoutVendor
                        select new PayoutVendorDTO
                        {
                            Amount = payoutVendor.Key.Amount,
                            StatusId = payoutVendor.Key.Status
                        };

            if (status != null)
            {
                query = query.Where(x => x.StatusId == (int)status);
            }

            return query.Sum(x => x.Amount);
        }

        public virtual decimal GetPayoutLastCycleVendorsTotal(int vendorId)
        {
            var lastCycleDate = GetPayoutLastCycleDate();
            var query = from pb in _payoutBatchRepo.Table
                        join pg in _payoutGroupRepo.Table on pb.Id equals pg.PayoutBatchId
                        join c in _customerRepo.Table on pg.PayoutTo equals c.Id
                        where c.VendorId == vendorId &&
                        pb.GeneratedDateTime >= lastCycleDate
                        group new { payoutGroupAmount = pg.Id } by new { pg.Amount }
                        into payoutVendor
                        select new PayoutVendorDTO
                        {
                            Amount = payoutVendor.Key.Amount
                        };


            return query.Sum(x => x.Amount);
        }

        public virtual string GeneratePayoutBatchCSVContent(
            PayoutBatchDTO payoutBatchDTO,s
            PagedList<PayoutGroupDTO> payoutGroupDTOs,
            DateTime generationDateTime
            )
        {

            const char separator = ',';
            var sb = new StringBuilder();
            // employer info
            sb.Append("Employer Info :");
            sb.Append(new String(separator, 8));
            sb.Append(Environment.NewLine);

            var strNow = generationDateTime.ToString("dd MM yyyy").Replace(" ", "/"); 

            sb.Append("Crediting Date (eg. dd/MM/yyyy)");
            sb.Append(separator);
            sb.Append(strNow);
            sb.Append(separator);
            sb.Append(separator);
            sb.Append("Please save this template to .csv (comma delimited) file before uploading the file via M2U Biz");
            sb.Append(new String(separator, 5));
            sb.Append(Environment.NewLine);

            sb.Append("Payment Reference");
            sb.Append(separator);
            sb.Append(payoutBatchDTO.PayoutBatchNumber);
            sb.Append(new String(separator, 7));
            sb.Append(Environment.NewLine);

            sb.Append("Payment Description");
            sb.Append(separator);
            sb.Append(payoutBatchDTO.PayoutBatchNumber);
            sb.Append(new String(separator, 7));
            sb.Append(Environment.NewLine);

            sb.Append("Bulk Payment Type");
            sb.Append(separator);
            sb.Append("Other Bulk Payment");
            sb.Append(new String(separator, 7));
            sb.Append(Environment.NewLine);

            sb.Append(new String(separator, 8));
            sb.Append(Environment.NewLine);

            //table header
            sb.Append("Beneficiary Name");
            sb.Append(separator);
            sb.Append("Beneficiary Bank");
            sb.Append(separator);
            sb.Append("Beneficiary Account No");
            sb.Append(separator);
            sb.Append("ID Type");
            sb.Append(separator);
            sb.Append("ID Number");
            sb.Append(separator);
            sb.Append("Payment Amount");
            sb.Append(separator);
            sb.Append("Payment Reference");
            sb.Append(separator);
            sb.Append("Payment Description");
            sb.Append(separator);
            sb.Append(Environment.NewLine);

            //table body
            for (int idx = 0; idx <= 100; idx++)
            {
                if (idx < payoutGroupDTOs.Count)
                {
                    var payoutGroup = payoutGroupDTOs.ElementAt(idx);
                    sb.Append(payoutGroup.AccountHolderName);
                    sb.Append(separator);
                    sb.Append(payoutGroup.BankName);
                    sb.Append(separator);
                    sb.Append(payoutGroup.AccountNumber);
                    sb.Append(separator);
                    sb.Append(payoutGroup.BankAccount.IdentityType == 2 ? "Passport" : "NRIC");
                    sb.Append(separator);
                    sb.Append(payoutGroup.BankAccount.Identity);
                    sb.Append(separator);
                    sb.Append(payoutGroup.Amount.ToString("0.00"));
                    sb.Append(separator);
                    sb.Append(payoutBatchDTO.PayoutBatchNumber);
                    sb.Append(separator);
                    sb.Append($"{payoutBatchDTO.PayoutBatchNumber} {idx + 1}");
                    sb.Append(separator);
                }
                else
                {
                    sb.Append(new String(separator, 8));
                }
                if (idx != 100)
                {
                    sb.Append("#REF!");
                }
                sb.Append(Environment.NewLine);
            }

            var result = sb.ToString();

            return result;
        }

        private DateTime GetPayoutLastCycleDate()
        {
            //get local time now
            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;
            var localDateTime = DateTime.UtcNow.AddHours(hoursDiff);
            var scheduledExecuteTime = localDateTime.Date;
            var payoutDate = localDateTime.Date;

            //cut off for 7, 14, 21 and 28 of the month
            if (scheduledExecuteTime.Day >= 28)
            {
                payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 28);
            }
            else if (scheduledExecuteTime.Day >= 21)
            {
                payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 21);
            }
            else if (scheduledExecuteTime.Day >= 14)
            {
                payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 14);
            }
            else if (scheduledExecuteTime.Day >= 7)
            {
                payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 7);
            }
            else
            {
                payoutDate = new DateTime(payoutDate.Year, payoutDate.Month, 28).AddMonths(-1);
            }

            return payoutDate.AddHours(hoursDiff * -1);
        }

        public DateTime GetPayoutDay(DateTime payoutBatchDate)
        {
            // parameter payoutBatch datetime
            var dayOfMonth = payoutBatchDate.Day;
            var payoutDaTe = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc);
            if (dayOfMonth >= 5 && dayOfMonth <= 7)
            {
                payoutDaTe = (new DateTime(payoutBatchDate.Year, payoutBatchDate.Month, 7));
            }
            else if (dayOfMonth >= 12 && dayOfMonth <= 14)
            {
                payoutDaTe = (new DateTime(payoutBatchDate.Year, payoutBatchDate.Month, 14));
            }
            else if (dayOfMonth >= 19 && dayOfMonth <= 21)
            {
                payoutDaTe = (new DateTime(payoutBatchDate.Year, payoutBatchDate.Month, 21));
            }
            else if (dayOfMonth >= 26 && dayOfMonth <= 28)
            {
                payoutDaTe = (new DateTime(payoutBatchDate.Year, payoutBatchDate.Month, 28));
            }

            return payoutDaTe;
        }
        #endregion

    }
}
