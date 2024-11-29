using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Documents;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Documents;
using Nop.Services.Helpers;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.DepositRequest;
using YadiYad.Pro.Services.DTO.Engagement;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Payout;
using YadiYad.Pro.Services.Services.Base;
using YadiYad.Pro.Services.Services.Messages;

namespace YadiYad.Pro.Services.Deposit
{
    public class DepositRequestService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IWorkContext _workContext;
        private readonly IRepository<DepositRequest> _depositRequestRepo;
        private readonly IRepository<ProOrderItem> _proOrderItemRepo;
        private readonly IRepository<ProOrder> _proOrderRepo;
        private readonly IRepository<ServiceApplication> _serviceApplicationRepo;
        private readonly IRepository<JobApplication> _jobApplicationRepo;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepo;
        private readonly IRepository<ServiceProfile> _serviceProfileRepo;
        private readonly IRepository<Bank> _bankRepo;
        private readonly ICustomerService _customerService;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepo;
        private readonly IRepository<ConsultationProfile> _consultationProfileRepo;
        private readonly IRepository<OrganizationProfile> _organizationProfile;
        private readonly IRepository<ProInvoice> _proInvoiceRepo;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly ProWorkflowMessageService _proWorkflowMessageService;
        private readonly JobApplicationService _jobApplicationService;
        private readonly FeeCalculationService _feeCalculationService;
        private readonly DocumentNumberService _documentNumberService;

        private readonly IRepository<JobProfile> _jobProfileRepo;

        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public DepositRequestService
            (IMapper mapper,
            IWorkContext workContext,
            IRepository<DepositRequest> depositRequestRepository,
            IRepository<ProOrderItem> proOrderItemRepository,
            IRepository<ProOrder> proOrderRepository,
            IRepository<ServiceApplication> serviceApplicationRepository,
            IRepository<JobApplication> jobApplicationRepository,
            IRepository<JobSeekerProfile> jobSeekerProfileRepository,
            IRepository<ServiceProfile> serviceProfileRepository,
            IRepository<JobProfile> jobProfileRepo,
            IRepository<ProInvoice> proInvoiceRepo,
            ICustomerService customerService,
            OrganizationProfileService organizationProfileService,
            IndividualProfileService individualProfileService,
            ProWorkflowMessageService workflowMessageService,
            JobApplicationService jobApplicationService,
            DocumentNumberService documentNumberService,
            IRepository<Bank> bankRepo,
            IRepository<ConsultationInvitation> consultationInvitationRepo,
            IRepository<ConsultationProfile> consultationProfileRepo,
            IRepository<OrganizationProfile> organizationProfile,
            IDateTimeHelper dateTimeHelper,
            FeeCalculationService feeCalculationService,
            ILogger logger)
        {
            _mapper = mapper;
            _workContext = workContext;
            _proInvoiceRepo = proInvoiceRepo;
            _documentNumberService = documentNumberService;
            _depositRequestRepo = depositRequestRepository;
            _proOrderItemRepo = proOrderItemRepository;
            _proOrderRepo = proOrderRepository;
            _serviceApplicationRepo = serviceApplicationRepository;
            _jobApplicationRepo = jobApplicationRepository;
            _jobSeekerProfileRepo = jobSeekerProfileRepository;
            _serviceProfileRepo = serviceProfileRepository;
            _customerService = customerService;
            _organizationProfileService = organizationProfileService;
            _individualProfileService = individualProfileService;
            _jobProfileRepo = jobProfileRepo;
            _proWorkflowMessageService = workflowMessageService;
            _jobApplicationService = jobApplicationService;
            _bankRepo = bankRepo;
            _consultationInvitationRepo = consultationInvitationRepo;
            _consultationProfileRepo = consultationProfileRepo;
            _organizationProfile = organizationProfile;
            _dateTimeHelper = dateTimeHelper;
            _feeCalculationService = feeCalculationService;
            _logger = logger;
        }

        #endregion

        #region Methods

        #region Deposit Request

        public void CreateDepositRequest(int actorId, DateTime? customTodayLocalDate = null)
        {
            //get deposit request raise date 7 or 21 of month
            DateTime todayLocalDate = customTodayLocalDate ?? _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc).Date;
            var requestDate = todayLocalDate;

            if(todayLocalDate.Day < 7)
            {
                requestDate = requestDate.AddDays(requestDate.Day * -1).AddMonths(-1).AddDays(21);
            }
            else if (todayLocalDate.Day < 21)
            {
                requestDate = requestDate.AddDays(requestDate.Day * -1).AddDays(7);
            }
            else
            {
                requestDate = requestDate.AddDays(requestDate.Day * -1).AddDays(21);
            }

            //get due date 14 or 28 of month
            var dueDate = requestDate.AddDays(7);
            var minCycleStartDate = new DateTime();
            var maxCycleStartDate = new DateTime();
            var maxCycleEndDate = new DateTime();

            if (dueDate.Day == 14)
            {
                maxCycleStartDate = dueDate.AddDays(dueDate.Day * -1);
                minCycleStartDate = new DateTime(maxCycleStartDate.Year, maxCycleStartDate.Month, 16);
            }
            else if (dueDate.Day == 28)
            {
                maxCycleStartDate = new DateTime(dueDate.Year, dueDate.Month, 15);
                minCycleStartDate = new DateTime(dueDate.Year, dueDate.Month, 1);
            }

            maxCycleEndDate = maxCycleStartDate.AddMonths(1).AddDays(-1);

            if (minCycleStartDate == new DateTime()
            || maxCycleStartDate == new DateTime()
            || maxCycleEndDate == new DateTime())
            {
                throw new InvalidOperationException("Fail to determine deposit request engagment start date range.");
            }

            _logger.Information($"Generate deposit request with due date, {dueDate.ToString("yyyy-MM-dd")} and request date, {requestDate.ToString("yyyy-MM-dd")}.");

            var newDepositRequests = new List<DepositRequest>();

            var serviceDepositRequests = GetRequiredServiceApplicationDepositRequest(
                requestDate,
                minCycleStartDate,
                maxCycleStartDate,
                maxCycleEndDate
            );

            var jobDepositRequest = GetRequiredJobApplicationDepositRequest(
                requestDate,
                minCycleStartDate,
                maxCycleStartDate,
                maxCycleEndDate
            );


            var docNumbers = _documentNumberService
                .GetDocumentNumbers(RunningNumberType.DepositRequest, serviceDepositRequests.Count);
            var docNumberIndex = 0;
            foreach (var engagement in serviceDepositRequests)
            {
                engagement.DepositNumber = docNumbers[docNumberIndex];
                engagement.DueDate = dueDate;
                engagement.RequestDate = requestDate;
                engagement.CreateAudit(actorId);

                newDepositRequests.Add(engagement);
                docNumberIndex++;
            }

            docNumbers = _documentNumberService
                .GetDocumentNumbers(RunningNumberType.DepositRequest, jobDepositRequest.Count);
            docNumberIndex = 0;
            foreach (var engagement in jobDepositRequest)
            {
                engagement.DepositNumber = docNumbers[docNumberIndex];
                engagement.DueDate = dueDate;
                engagement.RequestDate = requestDate;

                engagement.CreateAudit(actorId);

                newDepositRequests.Add(engagement);

                docNumberIndex++;
            }

            _depositRequestRepo.Insert(newDepositRequests);

            //send email
            newDepositRequests
                .Select(x =>
                {
                    var dto = _mapper.Map<DepositRequestDTO>(x);
                    _proWorkflowMessageService.SendDepositRequestNotification(_workContext.WorkingLanguage.Id, dto);
                    return x;
                })
                .ToList();
        }

        public List<DepositRequest> GetRequiredServiceApplicationDepositRequest(
            DateTime raisingDate,
            DateTime minCycleStartDate,
            DateTime maxCycleStartDate,
            DateTime maxCycleEndDate
            )
        {
            //get eligible service application to raise deposit request
            //only paid service application can create deposit
            //if there are unpaid deposit, no new deposit will be created
            //if due date is 14, last cycle start date must be between 16 to end of month
            //if due date is 28, last cycle start date must be between 1 to 15
            var serviceApplications =
                (from sa in _serviceApplicationRepo.Table
                .Where(x => x.Deleted == false
                && x.IsEscrow == true
                && x.Status == (int)ServiceApplicationStatus.Paid
                && x.StartDate != null
                && x.StartDate <= raisingDate
                && (x.EndDate == null
                || x.EndDate <= maxCycleEndDate))

                 from sp in _serviceProfileRepo.Table
                 .Where(sp => sp.Deleted == false
                 && sp.Id == sa.ServiceProfileId)
                 from dr in _depositRequestRepo.Table

                 .Where(dr => dr.Deleted == false
                 && dr.RefId == sa.Id
                 && dr.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
                 group new
                 {
                     sa,
                     dr
                 }
                 by new
                 {
                     EngagementId = sa.Id,
                     BuyerCustomerId = sa.CustomerId,
                     SellerCustomerId = sp.CustomerId,
                     StartDate = sa.StartDate,
                     EndDate = sa.EndDate,
                     ServiceProfileServiceFee = sa.ServiceProfileServiceFee,
                     ServiceProfileServiceModelId = sa.ServiceProfileServiceModelId,
                     ServiceProfileServiceTypeId = sa.ServiceProfileServiceTypeId,
                     Required = sa.Required
                 }
                into g1
                 select new
                 {
                     g1.Key.EngagementId,
                     g1.Key.BuyerCustomerId,
                     g1.Key.SellerCustomerId,
                     g1.Key.StartDate,
                     g1.Key.EndDate,
                     g1.Key.ServiceProfileServiceFee,
                     g1.Key.ServiceProfileServiceModelId,
                     g1.Key.ServiceProfileServiceTypeId,
                     g1.Key.Required,
                     NoUnpaidDeposit = g1.Sum(x => x.dr.Status != (int)DepositRequestStatus.Paid ? 1 : 0),
                     LastCycleStartDate = g1.Max(x => x.dr.CycleStart),
                     LastCycleEndDate = g1.Max(x => x.dr.CycleEnd),
                 })
                 .Where(x => x.NoUnpaidDeposit <= 0
                 && x.LastCycleStartDate >= minCycleStartDate
                 && x.LastCycleStartDate <= maxCycleStartDate)
                 .Select(x => new
                 {
                     EngagementId = x.EngagementId,
                     BuyerCustomerId = x.BuyerCustomerId,
                     SellerCustomerId = x.SellerCustomerId,
                     Engagement = new ServiceApplicationDTO
                     {
                         ServiceProfileServiceFee = x.ServiceProfileServiceFee,
                         ServiceProfileServiceModelId = x.ServiceProfileServiceModelId,
                         ServiceProfileServiceTypeId = x.ServiceProfileServiceTypeId,
                         Required = x.Required
                     },
                     LastCycleEndDate = x.LastCycleEndDate
                 })
                 .ToList()
                 .Select(x => new DepositRequest
                 {
                     RefId = x.EngagementId,
                     DepositFrom = x.BuyerCustomerId,
                     DepositTo = x.SellerCustomerId,
                     ProductTypeId = (int)ProductType.ServiceEnagegementFee,
                     Amount = x.Engagement.FeePerDepositExclSST,
                     CycleStart = x.LastCycleEndDate.Value.AddDays(1),
                     CycleEnd = x.LastCycleEndDate.Value.AddMonths(1),
                     Status = (int)DepositRequestStatus.New
                 })
                 .ToList();

            return serviceApplications;
        }

        public List<DepositRequest> GetRequiredJobApplicationDepositRequest(
            DateTime raisingDate,
            DateTime minCycleStartDate,
            DateTime maxCycleStartDate,
            DateTime maxCycleEndDate
            )
        {
            //get eligible job application to raise deposit request
            //only paid job application can create deposit
            //only job time freelancing daily or hourly required deposit request
            //if there are unpaid deposit, no new deposit will be created
            //if due date is 14, last cycle start date must be between 16 to end of month
            //if due date is 28, last cycle start date must be between 1 to 15
            var jobApplications =
                (from ja in _jobApplicationRepo.Table
                .Where(ja => ja.Deleted == false
                && ja.JobApplicationStatus == (int)JobApplicationStatus.Hired
                && (ja.JobType == (int)JobType.Freelancing
                    || ja.JobType == (int)JobType.PartTime)
                && ja.IsEscrow == true
                && ja.StartDate != null
                && ja.StartDate <= raisingDate
                && (ja.EndDate == null
                || ja.EndDate <= maxCycleEndDate))

                 from jsp in _jobSeekerProfileRepo.Table
                 .Where(jsp => jsp.Deleted == false
                 && jsp.Id == ja.JobSeekerProfileId)

                 from op in _organizationProfile.Table
                 .Where(op => op.Deleted == false
                 && op.Id == ja.OrganizationProfileId)

                 from dr in _depositRequestRepo.Table
                 .Where(dr => dr.Deleted == false
                 && dr.RefId == ja.Id
                 && dr.ProductTypeId == (int)ProductType.JobEnagegementFee)

                 group new
                 {
                     ja,
                     dr
                 }
                 by new
                 {
                     EngagementId = ja.Id,
                     BuyerCustomerId = op.CustomerId,
                     SellerCustomerId = jsp.CustomerId,
                     StartDate = ja.StartDate,
                     EndDate = ja.EndDate,
                     JobType = ja.JobType,
                     JobRequired = ja.JobRequired,
                     PayAmount = ja.PayAmount
                 }
                into g1
                 select new
                 {
                     g1.Key.EngagementId,
                     g1.Key.BuyerCustomerId,
                     g1.Key.SellerCustomerId,
                     g1.Key.StartDate,
                     g1.Key.EndDate,
                     g1.Key.JobType,
                     g1.Key.JobRequired,
                     g1.Key.PayAmount,
                     NoUnpaidDeposit = g1.Sum(x => x.dr.Status != (int)DepositRequestStatus.Paid ? 1 : 0),
                     LastCycleStartDate = g1.Max(x => x.dr.CycleStart),
                     LastCycleEndDate = g1.Max(x => x.dr.CycleEnd)
                 })
                 .Where(x => x.NoUnpaidDeposit <= 0
                 && x.LastCycleStartDate >= minCycleStartDate
                 && x.LastCycleStartDate <= maxCycleStartDate)
                 .Select(x => new
                 {
                     EngagementId = x.EngagementId,
                     BuyerCustomerId = x.BuyerCustomerId,
                     SellerCustomerId = x.SellerCustomerId,
                     Engagement = new JobApplicationDTO
                     {
                         JobType = x.JobType,
                         JobRequired = x.JobRequired,
                         PayAmount = x.PayAmount
                     },
                     LastCycleEndDate = x.LastCycleEndDate
                 })
                 .ToList()
                 .Select(x => new DepositRequest
                 {
                     RefId = x.EngagementId,
                     DepositFrom = x.BuyerCustomerId,
                     DepositTo = x.SellerCustomerId,
                     ProductTypeId = (int)ProductType.ServiceEnagegementFee,
                     Amount = x.Engagement.FeePerDepositExclSST,
                     CycleStart = x.LastCycleEndDate.Value.AddDays(1),
                     CycleEnd = x.LastCycleEndDate.Value.AddMonths(1),
                     Status = (int)DepositRequestStatus.New

                 })
                 .ToList();

            return jobApplications;
        }

        public void SendDepositRequestReminder(int actorId)
        {
            var todayDay = DateTime.UtcNow.Day;
            var allowedDays = new List<int> { 10, 12, 13, 24, 26, 27 };
            if (allowedDays.Contains(todayDay))
            {
                var expectedReminderCount = 0;
                if (todayDay == 12 || todayDay == 26)
                {
                    expectedReminderCount = 1;
                }
                else if (todayDay == 13 || todayDay == 27)
                {
                    expectedReminderCount = 2;
                }

                var dueRequests = (from dr in _depositRequestRepo.Table.Where(x => !x.Deleted && x.Status != (int)DepositRequestStatus.Paid && x.DueDate <= DateTime.UtcNow.Date && x.ReminderCount == expectedReminderCount)
                                   join poi in _proOrderItemRepo.Table.Where(x => !x.Deleted) on new { dr.ProductTypeId, dr.RefId } equals new { poi.ProductTypeId, poi.RefId }
                                   join po in _proOrderRepo.Table.Where(x => !x.Deleted && x.OrderStatusId == (int)OrderStatus.Complete) on poi.OrderId equals po.Id
                                   select new
                                   {
                                       dr,
                                       poi,
                                       po
                                   })
                               .ToList()
                               .GroupBy(x => x.dr.Id, (key, vals) => vals.First())
                               .ToList();

                if (dueRequests.Count > 0)
                {
                    var updatedDueRequest = dueRequests
                        .Select(x =>
                        {
                            x.dr.ReminderCount += 1;
                            x.dr.Status = (int)DepositRequestStatus.Reminded;
                            x.dr.UpdateAudit(actorId);

                            var dto = _mapper.Map<DepositRequestDTO>(x.dr);
                            dto.ItemName = x.poi.ItemName;
                            dto.CustomOrderNumber = x.po.CustomOrderNumber;

                            _proWorkflowMessageService.SendDepositRequestNotification(_workContext.WorkingLanguage.Id, dto);
                            return x;
                        })
                        .ToList();

                    var updatedNewDepositRequest = updatedDueRequest.Select(x => x.dr).ToList();
                    _depositRequestRepo.Update(updatedNewDepositRequest);
                }
            }
        }

        public DepositRequestDTO CreateUpdateProjectBasedDepositRequest(int actorId, int jobApplicationId, DepositRequestDTO dto)
        {
            JobApplicationDTO jobApplication = this._jobApplicationService.GetJobApplicationById(jobApplicationId);
            if (jobApplication.JobProfile.CustomerId != actorId)
            {
                throw new KeyNotFoundException();
            }
            dto.PaymentChannel = PaymentChannel.BankIn;
            dto.RefId = jobApplicationId;
            dto.ProductTypeId = (int)ProductType.JobEnagegementFee;
            var depositRequest = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.RefId == dto.RefId
                && x.ProductTypeId == dto.ProductTypeId)
                .FirstOrDefault();

            if (depositRequest != null)
            {
                depositRequest.BankId = dto.BankId;
                depositRequest.BankInDate = dto.BankInDate;
                depositRequest.BankInReference = dto.BankInReference;
                depositRequest.BankInSlipDownloadId = dto.BankInSlipDownloadId;
                depositRequest.Status = 0;
                depositRequest.UpdateAudit(actorId);
                this._depositRequestRepo.Update(depositRequest);
            }
            else
            {
                var docNumbers = _documentNumberService
                    .GetDocumentNumbers(RunningNumberType.DepositRequest, 1);

                depositRequest = this._mapper.Map<DepositRequest>(dto);
                depositRequest.Amount = jobApplication.PayAmount;
                depositRequest.Status = 0;
                depositRequest.RequestDate = DateTime.Today;
                depositRequest.DueDate = DateTime.Today;
                depositRequest.DepositNumber = docNumbers.First();
                depositRequest.DepositFrom = jobApplication.JobProfile.CustomerId;
                depositRequest.DepositTo = jobApplication.JobSeekerProfile.CustomerId;
                depositRequest.CreateAudit(actorId);
                this._depositRequestRepo.Insert(depositRequest);
            }

            _jobApplicationService.UpdateJobApplicationStatus(actorId, jobApplicationId, JobApplicationStatus.PendingPaymentVerification);

            return dto;
        }

        public void TerminateApplicationAfterDue(int actorId, DateTime? customTodayLocalDate = null)
        {
            //get deposit request raise date 7 or 21 of month
            DateTime todayLocalDate = customTodayLocalDate ?? _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc).Date;

            var dueRequestRaw = (from dr in _depositRequestRepo.Table
                              .Where(x =>
                                    !x.Deleted &&
                                    x.Status != (int)DepositRequestStatus.Paid &&
                                    x.DueDate < todayLocalDate &&
                                    x.CycleEnd > todayLocalDate &&
                                    (x.ProductTypeId == (int)ProductType.ServiceEnagegementFee || x.ProductTypeId == (int)ProductType.JobEnagegementFee)
                              )
                                 join poi in _proOrderItemRepo.Table.Where(x => !x.Deleted) on new { dr.ProductTypeId, dr.RefId } equals new { poi.ProductTypeId, poi.RefId }
                                 join po in _proOrderRepo.Table.Where(x => !x.Deleted && x.OrderStatusId == (int)OrderStatus.Complete) on poi.OrderId equals po.Id
                                 select new
                                 {
                                     dr,
                                     poi,
                                     po
                                 })
                               .ToList()
                               .GroupBy(x => new { x.dr.ProductTypeId, x.dr.RefId }, (key, vals) => vals.First())
                               .ToList();
            var dueRequest = (new List<DepositRequestDTO>[dueRequestRaw.Count])
                .Select((x, idx) =>
                {
                    var model = dueRequestRaw.ElementAt(idx);
                    var dto = _mapper.Map<DepositRequestDTO>(model.dr);
                    dto.ItemName = model.poi.ItemName;
                    dto.CustomOrderNumber = model.po.CustomOrderNumber;

                    return dto;
                })
                .ToList();

            if (dueRequest.Count > 0)
            {
                DateTime dueDate = _dateTimeHelper.ConvertToUserTime(DateTime.UtcNow, DateTimeKind.Utc).Date;
                var jobDueReq = dueRequest.Where(x => x.ProductTypeId == (int)ProductType.JobEnagegementFee).ToList();
                if (jobDueReq.Count > 0)
                {
                    var dueJobApplication = _jobApplicationRepo.Table
                        .Where(x =>
                            !x.Deleted &&
                            x.JobApplicationStatus == (int)JobApplicationStatus.Hired 
                            && x.IsEscrow == true
                            && jobDueReq.Select(x => x.RefId).Contains(x.Id)
                        )
                        .ToList()
                        .Select(x =>
                        {
                            x.EndDate = jobDueReq.Where(y=>y.RefId == x.Id).First().DueDate;
                            x.UpdateAudit(actorId);
                            return x;
                        })
                        .ToList();
                    if (dueJobApplication.Count > 0)
                    {
                        _jobApplicationRepo.Update(dueJobApplication);
                    }
                }
                var serviceDueReq = dueRequest.Where(x => x.ProductTypeId == (int)ProductType.ServiceEnagegementFee).ToList();
                if (serviceDueReq.Count > 0)
                {
                    var dueServiceApplications = _serviceApplicationRepo.Table
                        .Where(x =>
                            !x.Deleted &&
                            x.Status == (int)ServiceApplicationStatus.Paid &&
                            serviceDueReq.Select(x => x.RefId).Contains(x.Id)
                        )
                        .ToList()
                        .Select(x =>
                        {
                            x.EndDate = serviceDueReq.Where(y => y.RefId == x.Id).First().DueDate;
                            x.UpdateAudit(actorId);
                            return x;
                        })
                        .ToList();
                    if (dueServiceApplications.Count > 0)
                    {
                        _serviceApplicationRepo.Update(dueServiceApplications);
                    }
                }

                // update deposit request status
                var depositRequestsForUpdate = dueRequestRaw.Select(x => x.dr)
                    .Select(x =>
                    {
                        x.Status = (int)DepositRequestStatus.Overdue;
                        x.UpdateAudit(actorId);
                        return x;
                    })
                    .ToList();
                _depositRequestRepo.Update(depositRequestsForUpdate);

                //send email
                dueRequest.ForEach(x =>
                {
                    _proWorkflowMessageService.SendTerminatingApplicationNotificationBuyer(_workContext.WorkingLanguage.Id, x, dueDate);
                    _proWorkflowMessageService.SendTerminatingApplicationNotificationSeller(_workContext.WorkingLanguage.Id, x, dueDate);
                });
            }
        }

        public void CreatePaidDepositRequest(int actorId, DepositRequest depositRequest)
        {
            var docNumbers = _documentNumberService
                .GetDocumentNumbers(RunningNumberType.DepositRequest, 1);

            depositRequest.RequestDate = DateTime.Today;
            depositRequest.DueDate = DateTime.Today;
            depositRequest.DepositNumber = docNumbers.First();
            depositRequest.Status = (int)DepositRequestStatus.Paid;

            depositRequest.CreateAudit(actorId);

            _depositRequestRepo.Insert(depositRequest);
        }

        public PagedListDTO<DepositRequestDTO> GetDepositRequests(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            DepositRequestFilterDTO filterDTO = null
            )
        {
            var responseDTO = new PagedListDTO<DepositRequestDTO>(new List<DepositRequestDTO>(), pageIndex, pageSize, 0);
            var referId = 0;
            switch (filterDTO.ProductTypeId)
            {
                case (int)ProductType.ServiceEnagegementFee:
                    referId = (
                        from sa in _serviceApplicationRepo.Table
                        .Where(sa => sa.Deleted == false
                        && sa.Id == filterDTO.RefId)

                        from sp in _serviceProfileRepo.Table
                        .Where(sp => sp.Deleted == false
                        && sp.Id == sa.ServiceProfileId)
                        select new
                        {
                            sa,
                            sp
                        })
                        .Where(x =>
                            x.sa.CustomerId == filterDTO.CustomerId
                            || x.sp.CustomerId == filterDTO.CustomerId)
                        .Select(x => x.sa.Id)
                        .FirstOrDefault();
                    break;
                case (int)ProductType.JobEnagegementFee:
                    referId = (
                        from ja in _jobApplicationRepo.Table
                        .Where(ja => ja.Deleted == false
                        && ja.Id == filterDTO.RefId)

                        from jsp in _jobSeekerProfileRepo.Table
                        .Where(jsp => jsp.Deleted == false
                        && jsp.Id == ja.JobSeekerProfileId)

                        from jp in _jobProfileRepo.Table
                        .Where(jp => jp.Deleted == false
                        && jp.Id == ja.JobProfileId)
                        select new
                        {
                            ja,
                            jsp,
                            jp
                        })
                        .Where(x =>
                            x.jsp.CustomerId == filterDTO.CustomerId
                            || x.jp.CustomerId == filterDTO.CustomerId)
                        .Select(x => x.ja.Id)
                        .FirstOrDefault();
                    break;
                case (int)ProductType.ConsultationEngagementFee:
                    referId = (
                        from ci in _consultationInvitationRepo.Table
                        .Where(ci=>ci.Deleted == false
                        && ci.Id == filterDTO.RefId)

                        from cp in _consultationProfileRepo.Table
                        .Where(cp=>cp.Deleted == false
                        && cp.Id == ci.ConsultationProfileId)

                        from sp in _serviceProfileRepo.Table
                        .Where(sp=>sp.Deleted == false
                        && sp.Id == ci.ServiceProfileId)

                        from og in _organizationProfile.Table
                        .Where(og => og.Deleted == false
                        && og.Id == cp.OrganizationProfileId)

                        select new
                        {
                            ci,
                            sp,
                            og
                        })
                        .Where(x =>
                            x.sp.CustomerId == filterDTO.CustomerId
                            || x.og.CustomerId == filterDTO.CustomerId)
                        .Select(x => x.ci.Id)
                        .FirstOrDefault();
                    break;
            }


            //if no refId return empty list
            if (referId == 0)
            {
                return responseDTO;
            }

            //query payout request record
            var queryPR = (from dr in _depositRequestRepo.Table
                        .Where(dr => dr.Deleted == false
                           && dr.RefId == referId
                           && dr.ProductTypeId == filterDTO.ProductTypeId)

                           select dr);

            var totalCount = queryPR.Count();

            var query = queryPR
                        .OrderByDescending(x => x.UpdatedOnUTC)
                        .Take(pageSize)
                        .Skip(pageSize * pageIndex);

            var records = query.ToList();
            var recordDTOs = new List<DepositRequestDTO>();

            //get invoice id
            var proOrderItemIds = records.Select(x => x.OrderItemId).Distinct().ToList();
            var matchingIds = new List<int>
            {
                (int)ProductType.ConsultationEngagementMatchingFee,
                (int)ProductType.ServiceEnagegementMatchingFee
            };
           

            var porOrderItems =
                (from poi in _proOrderItemRepo.Table
                 where poi.Deleted == false
                 && proOrderItemIds.Contains(poi.Id)
                 from svpoi in _proOrderItemRepo.Table
                 where svpoi.Deleted == false
                 && svpoi.Id != poi.Id
                 && svpoi.OrderId == poi.OrderId
                 && matchingIds.Contains(svpoi.ProductTypeId)
                 select new
                 {
                     ProOrderItemId = poi.Id,
                     ProInvoiceId = svpoi.InvoiceId
                 })
                .ToList();

            foreach (var record in records)
            {
                var proInvoiceId = porOrderItems.Where(x => x.ProOrderItemId == record.OrderItemId).Select(x=>x.ProInvoiceId).FirstOrDefault();
                var depositRequestDTO = _mapper.Map<DepositRequestDTO>(record);
                depositRequestDTO.ServiceChargeInvoiceId = proInvoiceId;

                recordDTOs.Add(depositRequestDTO);
            }
            

            responseDTO = new PagedListDTO<DepositRequestDTO>(recordDTOs, pageIndex, pageSize, totalCount);

            return responseDTO;
        }

        public void UpdateDepositRequest(int actorId, int depositRequestId, int orderItemId, int status)
        {
            var model = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == depositRequestId)
                .FirstOrDefault();

            if (model != null)
            {
                if (orderItemId != 0)
                {
                    model.OrderItemId = orderItemId;
                }
                model.Status = status;
                model.UpdateAudit(actorId);
                _depositRequestRepo.Update(model);
            }
        }

        public void SetDepositRequestOrderItem(int actorId, int depositRequestId, int orderItemId)
        {
            var model = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == depositRequestId)
                .FirstOrDefault();

            if (model != null)
            {
                model.OrderItemId = orderItemId;
                model.UpdateAudit(actorId);
                _depositRequestRepo.Update(model);
            }
        }

        public DepositRequestDTO GetDepositRequestByOrderItemId(int orderItemId)
        {

            var model = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.OrderItemId == orderItemId)
                .FirstOrDefault();

            var dto = _mapper.Map<DepositRequestDTO>(model);

            return dto;
        }

        public DepositRequestDTO GetDepositRequestById(int id)
        {

            var model = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefault();

            var dto = _mapper.Map<DepositRequestDTO>(model);

            return dto;
        }

        public DepositRequestDTO GetDepositRequestByProductTypeRefId(int actorId, int refId, int productTypeId)
        {
            DepositRequestDTO dto = null;

            var model =
                (from dr in _depositRequestRepo.Table.Where(x =>
                x.Deleted == false
                && (actorId == 0
                    || x.DepositFrom == actorId)
                && x.RefId == refId
                && x.ProductTypeId == productTypeId)
                 from bk in _bankRepo.Table
                 .Where(x => x.Id == dr.BankId)
                 .DefaultIfEmpty()
                 select new
                 {
                     DepositRequest = dr,
                     Bank = bk
                 })
                .FirstOrDefault();

            if(model != null)
            {
                dto = _mapper.Map<DepositRequestDTO>(model.DepositRequest);

                if (dto != null && model.Bank != null)
                {
                    dto.bankName = model.Bank.Name;
                }
            }

            return dto;
        }

        public DepositDetailDTO GetDepositDetails(int refId, int productTypeId)
        {
            var dto = new DepositDetailDTO();
            var depositRequests = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.RefId == refId
                && x.ProductTypeId == productTypeId)
                .ToList();

            var latest = depositRequests.OrderByDescending(x => x.RequestDate).FirstOrDefault();

            dto.TotalAmount = depositRequests.Where(x => x.Status == (int)DepositRequestStatus.Paid).Sum(x => x.Amount);
            if (latest != null && latest.Status != (int)DepositRequestStatus.Paid)
            {
                dto.NextDueDate = latest.DueDate;
                dto.NextReminderCount = latest.ReminderCount;
            }


            return dto;
        }

        #endregion

        #region Approve Deposit Request

        public virtual DepositRequest GetApprovedDepositRequestById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return _depositRequestRepo.GetById(categoryId);
        }

        public virtual void UpdateApprovedDepositRequest(DepositRequest deposit)
        {
            if (deposit == null)
                throw new ArgumentNullException(nameof(deposit));

            _depositRequestRepo.Update(deposit);

            if(deposit.Status == (int)DepositRequestStatus.Invalid)
            {
                _jobApplicationService.UpdateJobApplicationStatus(deposit.UpdatedById.Value, deposit.RefId, JobApplicationStatus.RevisePaymentRequired);
            }
        }

        public virtual IPagedList<DepositRequest> SearchApproveDepositRequestTable(
            DateTime? from, DateTime? until, int? statusId,
          int pageIndex = 0,
          int pageSize = int.MaxValue)
        {
            var query = _depositRequestRepo.Table.Where(x => x.Deleted == false && x.PaymentChannelId == (int)PaymentChannel.BankIn);

            if (from != null)
            {
                query = query.Where(n => n.BankInDate >= from);
            }
            if (until != null)
            {
                query = query.Where(n => n.BankInDate < until);
            }
            if (statusId != 0 && statusId != null)
            {
                query = query.Where(n => n.Status == statusId);
            }

            query = query.OrderByDescending(n => n.Id);


            var data = new PagedList<DepositRequest>(query, pageIndex, pageSize);

            return data;
        }

        public virtual void AutoApprovedDepositRequest(int actorId, int depositRequestId, string engagementCode)
        {
            var depositRequest = _depositRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == depositRequestId)
                .FirstOrDefault();

            if(depositRequest == null)
            {
                throw new KeyNotFoundException("Deposit request not found.");
            }

            depositRequest.Status = (int)DepositRequestStatus.Paid;
            depositRequest.ApproveRemarks = $"Offset by job application {engagementCode}";

            _depositRequestRepo.Update(depositRequest);
        }

        #endregion


        #endregion
    }
}
