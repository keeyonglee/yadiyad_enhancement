using AutoMapper;
using Newtonsoft.Json;
using Nop.Core.Domain.Documents;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Engagement;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.Services.Engagement;

namespace YadiYad.Pro.Services.Payout
{
    public class PayoutRequestService
    {
        #region Fields

        private readonly IMapper _mapper;
        private readonly IRepository<PayoutRequest> _payoutRequestRepo;

        private readonly IRepository<ProOrder> _proOrderRepo;
        private readonly IRepository<ProOrderItem> _proOrderItemRepo;

        private readonly IRepository<ServiceProfile> _serviceProfileRepo;
        private readonly IRepository<JobProfile> _jobProfileRepo;
        private readonly IRepository<ConsultationProfile> _consultationProfileRepo;

        private readonly IRepository<JobSeekerProfile> _jobSeekerProfileRepo;

        private readonly IRepository<OrganizationProfile> _organizationProfileRepo;

        private readonly IRepository<ServiceApplication> _serviceApplicationRepo;
        private readonly IRepository<JobApplication> _jobApplicationRepo;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepo;

        private readonly IRepository<JobApplicationMilestone> _jobApplicationMilestoneRepo;

        private readonly IRepository<DepositRequest> _depositRequestRepo;

        private readonly PayoutRequestSettings _payoutRequestSettings;

        private readonly RefundRequestService _refundRequestService;

        private readonly InvoiceService _invoiceService;

        private readonly EngagementService _engagementService;

        private readonly ILocalizationService _localizationService;

        private readonly IDateTimeHelper _dateTimeHelper;

        private readonly OrderService _orderService;

        #endregion

        #region Ctor

        public PayoutRequestService
            (IMapper mapper,
            PayoutRequestSettings payoutRequestSettings,
            IDateTimeHelper dateTimeHelper,
            IRepository<PayoutRequest> payoutRequestRepo,
            IRepository<ProOrder> proOrderRepo,
            IRepository<ProOrderItem> proOrderItemRepo,
            IRepository<ServiceProfile> serviceProfileRepo,
            IRepository<JobSeekerProfile> jobSeekerProfileRepo,
            IRepository<ConsultationProfile> consultationProfileRepo,
            IRepository<JobProfile> jobProfileRepo,
            IRepository<OrganizationProfile> organizationProfileRepo,
            IRepository<ServiceApplication> serviceApplicationRepo,
            IRepository<JobApplication> jobApplicationRepo,
            IRepository<ConsultationInvitation> consultationInvitationRepo,
            IRepository<JobApplicationMilestone> jobMilestoneApplicationRepo,
            IRepository<DepositRequest> depositRequestRepo,
            ILocalizationService localizationService,
            InvoiceService invoiceService,
            RefundRequestService refundRequestService,
            EngagementService engagementService,
            OrderService orderService)
        {
            _mapper = mapper;
            _localizationService = localizationService;
            _payoutRequestSettings = payoutRequestSettings;
            _dateTimeHelper = dateTimeHelper;
            _payoutRequestRepo = payoutRequestRepo;

            _proOrderRepo = proOrderRepo;
            _proOrderItemRepo = proOrderItemRepo;

            _serviceProfileRepo = serviceProfileRepo;
            _jobSeekerProfileRepo = jobSeekerProfileRepo;
            _organizationProfileRepo = organizationProfileRepo;

            _consultationProfileRepo = consultationProfileRepo;
            _jobProfileRepo = jobProfileRepo;

            _serviceApplicationRepo = serviceApplicationRepo;
            _jobApplicationRepo = jobApplicationRepo;
            _consultationInvitationRepo = consultationInvitationRepo;

            _jobApplicationMilestoneRepo = jobMilestoneApplicationRepo;
            _depositRequestRepo = depositRequestRepo;

            _invoiceService = invoiceService;
            _engagementService = engagementService;
            _refundRequestService = refundRequestService;
            _orderService = orderService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// get payout request list
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filterDTO"></param>
        /// <returns></returns>
        public PagedListDTO<PayoutRequestDTO> GetPayoutRequests(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            PayoutRequestFilterDTO filterDTO = null
            )
        {

            var responseDTO = new PagedListDTO<PayoutRequestDTO>(new List<PayoutRequestDTO>(), pageIndex, pageSize, 0);

            var allowAccess = false;
            var allowedRaisePayout = false;
            var engagementTypeId = 0;
            DateTime? engagementStartDate = null;
            DateTime? engagementEndDate = null;
            int? endJobMilestoneId = null;

            //filter by customer id either is organization, service buyer, service provider, job seeker, consultant
            switch((ProductType)filterDTO.ProductTypeID)
            {
                case ProductType.ServiceEnagegementFee:
                    {
                        var engagement = (
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
                            .Select(x => new
                            {
                                x.sa.Id,
                                x.sa.ServiceProfileServiceTypeId,
                                x.sa.StartDate,
                                x.sa.EndDate
                            })
                            .FirstOrDefault();

                        if (engagement != null)
                        {
                            engagementTypeId = engagement.ServiceProfileServiceTypeId;
                            engagementStartDate = engagement.StartDate;
                            engagementEndDate = engagement.EndDate;
                            allowAccess = true;
                        }
                    }
                    break;
                case ProductType.JobEnagegementFee:
                    {
                        var engagement = (
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
                            .Select(x => new
                            {
                                x.ja.Id,
                                x.ja.StartDate,
                                x.ja.EndDate,
                                x.ja.JobType,
                                x.ja.EndMilestoneId
                            })
                            .FirstOrDefault();

                        if (engagement != null)
                        {
                            engagementTypeId = engagement.JobType;
                            engagementStartDate = engagement.StartDate;
                            engagementEndDate = engagement.EndDate;
                            allowAccess = true;
                            endJobMilestoneId = engagement.EndMilestoneId;
                        }
                    }
                    break;
                case ProductType.ConsultationEngagementFee:
                    {
                        var engagement = (
                            from ci in _consultationInvitationRepo.Table
                            .Where(ci => ci.Deleted == false
                            && ci.Id == filterDTO.RefId)

                            from sp in _serviceProfileRepo.Table
                            .Where(sp => sp.Deleted == false
                            && sp.Id == ci.ServiceProfileId)

                            from cp in _consultationProfileRepo.Table
                            .Where(cp => cp.Deleted == false
                            && cp.Id == ci.ConsultationProfileId)

                            from op in _organizationProfileRepo.Table
                            .Where(op => op.Deleted == false
                            && op.Id == cp.OrganizationProfileId)
                            select new
                            {
                                ci,
                                sp,
                                cp,
                                op
                            })
                            .Where(x =>
                                x.op.CustomerId == filterDTO.CustomerId
                                || x.sp.CustomerId == filterDTO.CustomerId)
                            .Select(x => new
                            {
                                x.ci.Id
                            })
                            .FirstOrDefault();

                        if(engagement != null)
                        {
                            allowAccess = true;
                        }
                    }
                    break;
            }

            //if no refId return empty list
            if (allowAccess == false)
            {
                throw new KeyNotFoundException("Engagement not found.");
            }

            //query payout request record
            var queryPR = (from pr in _payoutRequestRepo.Table
                        .Where(pr => pr.Deleted == false
                           && pr.RefId == filterDTO.RefId
                           && pr.ProductTypeId == filterDTO.ProductTypeID)

                           from jm in _jobApplicationMilestoneRepo.Table
                           .Where(jm => jm.Deleted == false
                           && jm.Id == pr.JobApplicationMilestoneId)
                           .DefaultIfEmpty()

                           select new PayoutRequestDTO
                           {
                               Id = pr.Id,
                               OrderItemId = pr.OrderItemId,
                               Fee = pr.Fee,
                               ServiceCharge = pr.ServiceCharge,
                               ServiceChargeRate = pr.ServiceChargeRate,
                               ServiceChargeType = pr.ServiceChargeType,
                               PayoutTo = pr.PayoutTo,
                               Status = pr.Status,
                               InvoiceId = pr.InvoiceId,
                               ServiceChargeInvoiceId = pr.ServiceChargeInvoiceId,
                               TimeSheetJson = pr.TimeSheetJson,
                               WorkDesc = pr.WorkDesc,
                               OnsiteDuration = pr.OnsiteDuration,
                               AttachmentDownloadId = pr.AttachmentDownloadId,
                               Remark = pr.Remark,
                               StartDate = pr.StartDate,
                               EndDate = pr.EndDate,
                               CreatedOnUTC = pr.CreatedOnUTC,
                               JobMilestoneId = pr.JobApplicationMilestoneId,
                               JobMilestoneName = jm != null ? jm.Description : null,
                               JobMilestonePhase = jm != null ? (int?)(jm.Sequence + 1) : null
                           });

            var totalCount = queryPR.Count();

            var query = queryPR
                        .OrderByDescending(x => x.CreatedOnUTC)
                        .Take(pageSize)
                        .Skip(pageSize * pageIndex);



            var records = query.ToList();

            allowedRaisePayout = CheckIfAllowRaisePayout(
                query,
                (ProductType)filterDTO.ProductTypeID,
                (JobType)engagementTypeId,
                filterDTO.RefId,
                engagementStartDate,
                engagementEndDate,
                endJobMilestoneId
            );

            responseDTO = new PagedListDTO<PayoutRequestDTO>(records, pageIndex, pageSize, totalCount);
            responseDTO.AdditionalData = new
            {
                AllowedRaisePayout = allowedRaisePayout
            };

            return responseDTO;
        }

        /// <summary>
        /// check if engagement allow to raise new payout request
        /// </summary>
        /// <param name="payoutRequestQuery"></param>
        /// <param name="productType"></param>
        /// <param name="jobType"></param>
        /// <param name="refId"></param>
        /// <param name="engagementStartDate"></param>
        /// <param name="engagementEndDate"></param>
        /// <returns></returns>
        public bool CheckIfAllowRaisePayout(
            IQueryable<PayoutRequestDTO> payoutRequestQuery,
            ProductType productType,
            JobType jobType,
            int refId,
            DateTime? engagementStartDate,
            DateTime? engagementEndDate,
            int? endJobMilestoneId
            )
        {
            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;
            var today = DateTime.UtcNow.AddHours(hoursDiff).Date;
            bool allowedRaisePayout = false;

            //check if allow create new payout request
            if (productType == ProductType.JobEnagegementFee
                && jobType == JobType.ProjectBased)
            {
                var lastJobApplicationMileStoneId = _jobApplicationMilestoneRepo.Table
                    .Where(x => x.Deleted == false
                    && x.JobApplicationId == refId
                    && (endJobMilestoneId.HasValue == false 
                        || x.Id == endJobMilestoneId.Value))
                    .OrderByDescending(x => x.Sequence)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                if (lastJobApplicationMileStoneId == 0)
                {
                    throw new KeyNotFoundException("Engagement not found.");
                }
                else if (engagementStartDate != null
                    && today < engagementStartDate)
                {
                    allowedRaisePayout = false;
                }
                else
                {
                    allowedRaisePayout = payoutRequestQuery.Where(x =>
                        x.JobMilestoneId == lastJobApplicationMileStoneId)
                            .Any() == false;
                }
            }
            else
            {
                if (engagementEndDate != null)
                {
                    //if engagement have end date and payout request not created, allowed user to create.
                    allowedRaisePayout = payoutRequestQuery.Where(x =>
                        x.StartDate <= engagementEndDate.Value
                        && engagementEndDate.Value <= x.EndDate)
                            .Any() == false;
                }
                else if (engagementStartDate != null
                    && today >= engagementStartDate)
                {
                    allowedRaisePayout = true;
                }

                if (allowedRaisePayout)
                {
                    var maxAllowedPayoutCreationDate =
                        today.Day >= 16
                        ? new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1)
                        : new DateTime(today.Year, today.Month, 15);

                    allowedRaisePayout =
                        payoutRequestQuery.Where(x => x.EndDate == maxAllowedPayoutCreationDate)
                        .Any() == false;
                }
            }

            return allowedRaisePayout;
        }

        /// <summary>
        /// get single payout request
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="id"></param>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public PayoutRequestDTO GetPayoutRequest(int? customerId, int? id, int? invoiceId = null)
        {
            var queryPR = (from pr in _payoutRequestRepo.Table
                        .Where(pr => pr.Deleted == false
                        && (id == null
                            || pr.Id == id)
                        && (invoiceId == null
                            || pr.InvoiceId == invoiceId
                            || pr.ServiceChargeInvoiceId == invoiceId))
                           from oi in _proOrderItemRepo.Table
                           .Where(oi => oi.Deleted == false
                           && oi.Id == pr.OrderItemId)

                           from jm in _jobApplicationMilestoneRepo.Table
                           .Where(jm => jm.Deleted == false
                           && jm.Id == pr.JobApplicationMilestoneId)
                           .DefaultIfEmpty()
                           select new PayoutRequestDTO
                           {
                               Id = pr.Id,
                               RefId = oi.RefId,
                               ProductTypeId = oi.ProductTypeId,
                               OrderItemId = pr.OrderItemId,
                               Fee = pr.Fee,
                               ServiceCharge = pr.ServiceCharge,
                               ServiceChargeRate = pr.ServiceChargeRate,
                               ServiceChargeType = pr.ServiceChargeType,
                               PayoutTo = pr.PayoutTo,
                               Status = pr.Status,
                               InvoiceId = pr.InvoiceId,
                               ServiceChargeInvoiceId = pr.ServiceChargeInvoiceId,
                               TimeSheetJson = pr.TimeSheetJson,
                               WorkDesc = pr.WorkDesc,
                               OnsiteDuration = pr.OnsiteDuration,
                               AttachmentDownloadId = pr.AttachmentDownloadId,
                               Remark = pr.Remark,
                               StartDate = pr.StartDate,
                               EndDate = pr.EndDate,
                               CreatedOnUTC = pr.CreatedOnUTC,
                               ProratedWorkDuration = pr.ProratedWorkDuration,
                               JobMilestoneId = pr.JobApplicationMilestoneId,
                               JobMilestoneName = jm != null ? jm.Description : null,
                               JobMilestonePhase = jm != null ? (int?)(jm.Sequence + 1) : null
                           });

            queryPR = queryPR
                .OrderByDescending(x => x.CreatedOnUTC);

            var record = queryPR.FirstOrDefault();
            var referId = record.RefId;
            var productTypeId = record.ProductTypeId;

            switch (productTypeId)
            {
                case (int)ProductType.ServiceEnagegementFee:
                    referId = (
                        from sa in _serviceApplicationRepo.Table
                        .Where(sa => sa.Deleted == false
                        && sa.Id == referId)

                        from sp in _serviceProfileRepo.Table
                        .Where(sp => sp.Deleted == false
                        && sp.Id == sa.ServiceProfileId)
                        select new
                        {
                            sa,
                            sp
                        })
                        .Where(x =>
                            customerId == null
                            || x.sa.CustomerId == customerId
                            || x.sp.CustomerId == customerId)
                        .Select(x => x.sa.Id)
                        .FirstOrDefault();
                    break;
                case (int)ProductType.JobEnagegementFee:
                    referId = (
                        from ja in _jobApplicationRepo.Table
                        .Where(ja => ja.Deleted == false
                        && ja.Id == referId)

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
                            customerId == null
                            || x.jsp.CustomerId == customerId
                            || x.jp.CustomerId == customerId)
                        .Select(x => x.ja.Id)
                        .FirstOrDefault();
                    break;
                case (int)ProductType.ConsultationEngagementMatchingFee:
                    referId = (
                        from ci in _consultationInvitationRepo.Table
                        .Where(ci => ci.Deleted == false
                        && ci.Id == referId)

                        from sp in _serviceProfileRepo.Table
                        .Where(sp => sp.Deleted == false
                        && sp.Id == ci.ServiceProfileId)

                        from cp in _consultationProfileRepo.Table
                        .Where(cp => cp.Deleted == false
                        && cp.Id == ci.ConsultationProfileId)

                        from op in _organizationProfileRepo.Table
                        .Where(op => op.Deleted == false
                        && op.Id == cp.OrganizationProfileId)
                        select new
                        {
                            ci,
                            sp,
                            cp,
                            op
                        })
                        .Where(x =>
                            customerId == null
                            || x.op.CustomerId == customerId
                            || x.sp.CustomerId == customerId)
                        .Select(x => x.ci.Id)
                        .FirstOrDefault();
                    break;
            }

            //if no refId return null
            if (referId == 0)
            {
                record = null;
            }

            return record;

        }

        /// <summary>
        /// get payout request which available to create new
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="productTypeId"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public PayoutRequestDTO GetNewPayoutRequest(int customerId, int productTypeId, int refId, bool disabledCheckOwnship = true)
        {
            int orderItemId = 0;
            DateTime? engagementStartDate = null;
            DateTime? engagementEndDate = null;
            int? endJobMilestoneId = null;

            var isProjectBased = false;
            int jobProfileId = 0;
            var payoutTo = 0;

            switch (productTypeId)
            {
                case (int)ProductType.ServiceEnagegementFee:
                    {
                        var serviceEngagement = (
                        from sa in _serviceApplicationRepo.Table
                        .Where(sa => sa.Deleted == false
                        && sa.Id == refId)

                        from sp in _serviceProfileRepo.Table
                        .Where(sp => sp.Deleted == false
                        && sp.Id == sa.ServiceProfileId)

                        from oi in _proOrderItemRepo.Table
                        .Where(oi => oi.Deleted == false
                        && oi.RefId == sa.Id
                        && oi.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
                        select new
                        {
                            sa,
                            sp,
                            oi
                        })
                        .Where(x => disabledCheckOwnship == true
                         || x.sp.CustomerId == customerId)
                        .Select(x => new
                        {
                            x.oi.Id,
                            x.sa.StartDate,
                            x.sa.EndDate,
                            SellerId = x.sp.CustomerId
                        })
                        .FirstOrDefault();
                        if (serviceEngagement != null)
                        {
                            orderItemId = serviceEngagement.Id;
                            engagementStartDate = serviceEngagement.StartDate;
                            engagementEndDate = serviceEngagement.EndDate;
                            payoutTo = serviceEngagement.SellerId;
                        }
                    }
                    break;
                case (int)ProductType.JobEnagegementFee:
                    {
                        var engagement = (
                        from ja in _jobApplicationRepo.Table
                        .Where(ja => ja.Deleted == false
                        && ja.IsEscrow == true
                        && ja.Id == refId)

                        from jsp in _jobSeekerProfileRepo.Table
                        .Where(jsp => jsp.Deleted == false
                        && jsp.Id == ja.JobSeekerProfileId)

                        from jp in _jobProfileRepo.Table
                        .Where(jp => jp.Deleted == false
                        && jp.Id == ja.JobProfileId)

                        from oi in _proOrderItemRepo.Table
                        .Where(oi => oi.Deleted == false
                        && oi.RefId == ja.Id
                        && oi.ProductTypeId == productTypeId)
                        select new
                        {
                            ja,
                            jsp,
                            jp,
                            oi
                        })
                        .Where(x =>
                            disabledCheckOwnship == true
                            || x.jsp.CustomerId == customerId)
                        .Select(x => new
                        {
                            x.oi.Id,
                            x.ja.StartDate,
                            x.ja.EndDate,
                            x.jp.JobType,
                            JobProfileId = x.jp.Id,
                            x.ja.EndMilestoneId,
                            SellerId = x.jsp.CustomerId
                        })
                        .FirstOrDefault();

                        if (engagement != null)
                        {
                            orderItemId = engagement.Id;
                            engagementStartDate = engagement.StartDate;
                            engagementEndDate = engagement.EndDate;
                            isProjectBased = engagement.JobType == (int)JobType.ProjectBased;
                            jobProfileId = engagement.JobProfileId;
                            endJobMilestoneId = engagement.EndMilestoneId;
                            payoutTo = engagement.SellerId;
                        }
                    }
                    break;
                case (int)ProductType.ModeratorFacilitateConsultationFee:
                case (int)ProductType.ConsultationEngagementFee:
                    {
                        var orderItemProductTypeId = (int)ProductType.ConsultationEngagementFee;

                        if((int)ProductType.ModeratorFacilitateConsultationFee == productTypeId)
                        {
                            orderItemProductTypeId = (int)ProductType.ConsultationEngagementMatchingFee;
                        }

                        var engagement = (
                        from ci in _consultationInvitationRepo.Table
                        .Where(ci => ci.Deleted == false
                        && ci.Id == refId)

                        from sp in _serviceProfileRepo.Table
                        .Where(sa => sa.Deleted == false
                        && sa.Id == ci.ServiceProfileId)

                        from cp in _consultationProfileRepo.Table
                        .Where(cp => cp.Deleted == false
                        && cp.Id == ci.ConsultationProfileId)

                        from op in _organizationProfileRepo.Table
                        .Where(op => op.Deleted == false
                        && op.Id == cp.OrganizationProfileId)

                        from oi in _proOrderItemRepo.Table
                        .Where(oi => oi.Deleted == false
                        && oi.RefId == ci.Id
                        && oi.ProductTypeId == orderItemProductTypeId)
                        select new
                        {
                            ci,
                            sp,
                            cp,
                            op,
                            oi
                        })
                        .Where(x =>
                            disabledCheckOwnship == true
                            || x.sp.CustomerId == customerId
                            || x.ci.ModeratorCustomerId == customerId
                            || x.op.CustomerId == customerId)
                        .Select(x => new
                        {
                            OrderItemId = x.oi.Id,
                            ModeratorId = x.ci.ModeratorCustomerId,
                            SellerId = x.sp.CustomerId
                        })
                        .FirstOrDefault();

                        if(engagement != null)
                        {
                            orderItemId = engagement.OrderItemId;

                            if(productTypeId == (int)ProductType.ModeratorFacilitateConsultationFee)
                            {
                                payoutTo = engagement.ModeratorId.Value;
                            }

                            if (productTypeId == (int)ProductType.ConsultationEngagementFee)
                            {
                                payoutTo = engagement.SellerId;
                            }
                        }
                        else if (productTypeId == (int)ProductType.ModeratorFacilitateConsultationFee)
                        {
                            var consultationEngagement = _engagementService.GetEngagement((int)ProductType.ModeratorFacilitateConsultationFee, refId);

                            var orderItem = _orderService.GetOrderItem((int)ProductType.ConsultationEngagementMatchingFee, refId);

                            payoutTo = consultationEngagement.ModeratorCustomerId.Value;

                            orderItemId = orderItem.Id;
                        }
                    }
                    break;
            }

            if (orderItemId == 0)
            {
                return null;
            }

            DateTime? payoutRequestStartDate = null;
            DateTime? payoutRequestEndDate = null;
            int? jobMilestoneId = null;
            string jobMilestoneName = null;
            int? jobMilestonePhase = 0;

            if (isProjectBased)
            {
                int? endMilestoneSequenceNo = null;
                
                if(endJobMilestoneId.HasValue)
                {
                    var endJobMilestone =
                        _jobApplicationMilestoneRepo.Table
                        .Where(jm => jm.Deleted == false
                        && jm.JobApplicationId == refId
                        && jm.Id == endJobMilestoneId.Value)
                        .FirstOrDefault();

                    if(endJobMilestone != null)
                    {
                        endMilestoneSequenceNo = endJobMilestone.Sequence;
                    }
                }

                var firstPayableJobMilestone =
                    (from jm in _jobApplicationMilestoneRepo.Table
                    .Where(jm => jm.Deleted == false
                    && jm.JobApplicationId == refId
                    && (endMilestoneSequenceNo.HasValue == false
                        || jm.Sequence <= endMilestoneSequenceNo.Value))
                     from pr in _payoutRequestRepo.Table
                     .Where(pr => pr.Deleted == false
                         && pr.OrderItemId == orderItemId
                         && jm.Id == pr.JobApplicationMilestoneId)
                     .DefaultIfEmpty()
                     select new
                     {
                         jm,
                         pr
                     })
                    .Where(x => x.pr == null)
                    .OrderBy(x => x.jm.Sequence)
                    .Select(x => new
                    {
                        x.jm.Id,
                        x.jm.Sequence,
                        x.jm.Description
                    })
                    .FirstOrDefault();

                if (firstPayableJobMilestone != null)
                {
                    jobMilestoneId = firstPayableJobMilestone.Id;
                    jobMilestoneName = firstPayableJobMilestone.Description;
                    jobMilestonePhase = firstPayableJobMilestone.Sequence + 1;
                }
            }
            else if (productTypeId == (int)ProductType.ModeratorFacilitateConsultationFee)
            {

            }
            else
            {
                var lastPayoutRequestDate = _payoutRequestRepo.Table
                    .Where(x => x.Deleted == false
                    && x.OrderItemId == orderItemId)
                    .Select(x => (DateTime?)x.EndDate)
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                if (lastPayoutRequestDate != null)
                {
                    payoutRequestStartDate = lastPayoutRequestDate.Value.AddDays(1);
                }
                else if (engagementStartDate.HasValue)
                {
                    payoutRequestStartDate = engagementStartDate.Value;
                }

                if (payoutRequestStartDate.HasValue)
                {
                    //get engagement end date
                    if (payoutRequestStartDate.Value.Day <= 15)
                    {
                        payoutRequestEndDate = payoutRequestStartDate.Value
                            .AddDays(-1 * payoutRequestStartDate.Value.Day)
                            .AddDays(15);
                    }
                    else if (payoutRequestStartDate.Value.Day > 15)
                    {
                        payoutRequestEndDate = payoutRequestStartDate.Value
                            .AddDays(-1 * payoutRequestStartDate.Value.Day)
                            .AddDays(1)
                            .AddMonths(1)
                            .AddDays(-1);
                    }
                }

                //ensure payout date not more than engagement end date
                if (payoutRequestEndDate.HasValue && engagementEndDate.HasValue)
                {
                    if (payoutRequestEndDate.Value > engagementEndDate.Value)
                    {
                        payoutRequestEndDate = engagementEndDate;
                    }
                }
            }

            if(payoutTo==0)
            {
                throw new Exception("Fail to determine payout to.");
            }

            var newRecord = new PayoutRequestDTO
            {
                Id = 0,
                RefId = refId,
                ProductTypeId = productTypeId,
                OrderItemId = orderItemId,
                PayoutTo = payoutTo,
                Status = (int)PayoutRequestStatus.New,
                StartDate = payoutRequestStartDate,
                EndDate = payoutRequestEndDate,
                JobMilestoneId = jobMilestoneId,
                JobMilestoneName = jobMilestoneName,
                JobMilestonePhase = jobMilestonePhase
            };

            return newRecord;
        }

        /// <summary>
        /// create payout request
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PayoutRequestDTO CreatePayoutRequest(int actorId, string actorName, PayoutRequestDTO dto, bool disabledCheckOwnship = true)
        {
            var newModel = GetNewPayoutRequest(actorId, dto.ProductTypeId, dto.RefId, disabledCheckOwnship);
            var model = _mapper.Map<PayoutRequest>(dto);
            model.OrderItemId = newModel.OrderItemId;

            //payout to determine in GetNewPayoutRequest
            model.PayoutTo = newModel.PayoutTo;
            model.Status = newModel.Status;
            model.StartDate = newModel.StartDate;
            model.EndDate = newModel.EndDate;
            model.JobApplicationMilestoneId = newModel.JobMilestoneId;
            model.ProductTypeId = dto.ProductTypeId;
            model.RefId = dto.RefId;

            if(dto.ProductTypeId == (int)ProductType.ModeratorFacilitateConsultationFee
                || dto.ProductTypeId == (int)ProductType.ConsultationEngagementFee)
            {
                model.WorkDesc = "Auto generated by system.";
                model.TimeSheetJson = "[]";
                model.Status = (int)PayoutRequestStatus.Approved;
                //working hours or days
                model.OnsiteDuration = dto.OnsiteDuration;
            }

            var remarkDTOs = new List<RemarksDTO>();

            if (string.IsNullOrWhiteSpace(dto.EnteredRemark) == false)
            {
                remarkDTOs.Add(new RemarksDTO
                {
                    Remark = dto.EnteredRemark,
                    RemarkDate = DateTime.UtcNow,
                    ActorName = actorName
                });

                model.Remark = JsonConvert.SerializeObject(remarkDTOs);
            }

            model.CreateAudit(actorId);

            _payoutRequestRepo.Insert(model);

            var payoutRequestDTO = _mapper.Map<PayoutRequestDTO>(model);
            return payoutRequestDTO;
        }

        /// <summary>
        /// update payout request detail by buyer
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public PayoutRequestDTO UpdatePayoutRequestDetail(int actorId, string actorName, PayoutRequestDTO dto)
        {
            var model = _payoutRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == dto.Id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            if ((model.Status != (int)PayoutRequestStatus.RequiredMoreInfo)
                || dto.Status != (int)PayoutRequestStatus.New)
            {
                throw new InvalidOperationException();
            }

            if (model.PayoutTo == actorId)
            {
                model.WorkDesc = dto.WorkDesc;
                model.OnsiteDuration = dto.OnsiteDuration;
                model.Fee = dto.Fee;
                model.ServiceCharge = dto.ServiceCharge;
                model.ServiceChargeRate = dto.ServiceChargeRate;
                model.ServiceChargeType = dto.ServiceChargeType;
                model.TimeSheetJson = dto.TimeSheetJson;
                model.AttachmentDownloadId = dto.AttachmentDownloadId;
                model.Status = dto.Status;

                dto.Remark = model.Remark;
                var remarkDTOs = dto.Remarks;

                if (string.IsNullOrWhiteSpace(dto.EnteredRemark) == false)
                {
                    remarkDTOs.Add(new RemarksDTO
                    {
                        Remark = dto.EnteredRemark,
                        RemarkDate = DateTime.UtcNow,
                        ActorName = actorName
                    });
                }
                model.Remark = JsonConvert.SerializeObject(remarkDTOs);
            }

            model.UpdateAudit(actorId);
            _payoutRequestRepo.Update(model);

            var payoutRequestDTO = _mapper.Map<PayoutRequestDTO>(model);
            return payoutRequestDTO;
        }

        /// <summary>
        /// update payout request to required more info and approved
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="enteredRemark"></param>
        /// <returns></returns>
        public PayoutRequestDTO UpdatePayoutRequestStatus(int actorId, string actorName, int id, int status, string enteredRemark)
        {
            var model = _payoutRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == id)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            var engagement = _engagementService.GetEngagement(model.ProductTypeId, model.RefId);

            if(engagement == null)
            {
                throw new KeyNotFoundException("Engagement not found.");
            }

            if (!(model.Status == (int)PayoutRequestStatus.RequiredMoreInfo && engagement.BuyerCustomerId != actorId)
                && !(model.Status == (int)PayoutRequestStatus.Approved && engagement.BuyerCustomerId != actorId)
                && !(model.Status == (int)PayoutRequestStatus.New
                || model.Status == (int)PayoutRequestStatus.Paid))
            {
                throw new InvalidOperationException();
            }

            var dto = _mapper.Map<PayoutRequestDTO>(model);

            var remarkDTOs = dto.Remarks;

            if(string.IsNullOrWhiteSpace(enteredRemark) == false)
            {
                remarkDTOs.Add(new RemarksDTO
                {
                    Remark = enteredRemark,
                    RemarkDate = DateTime.UtcNow,
                    ActorName = actorName
                });
            }

            model.Status = status;
            model.Remark = JsonConvert.SerializeObject(remarkDTOs);
            model.UpdateAudit(actorId);
            _payoutRequestRepo.Update(model);

            var payoutRequestDTO = _mapper.Map<PayoutRequestDTO>(model);
            return payoutRequestDTO;
        }

        
        /// <summary>
        /// update payout request to paid and perform necessary update
        /// </summary>
        /// <param name="actorId"></param>
        /// <param name="payoutRequestId"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public PayoutRequestDTO UpdatePayoutRequestPaidStatus(int actorId, int payoutRequestId, string remark = null)
        {
            var model = _payoutRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Id == payoutRequestId)
                .FirstOrDefault();

            if (model == null)
            {
                throw new KeyNotFoundException();
            }

            var engagement = _engagementService.GetEngagement(model.ProductTypeId, model.RefId);

            //create invoice
            var newInvoice = new InvoiceDTO
            {
                InvoiceFrom = engagement.SellerCustomerId,
                InvoiceTo = engagement.BuyerCustomerId,
                RefType = (int)InvoiceRefType.Payout
            };

            var createdInvoice = _invoiceService.CreateInvoice(actorId, newInvoice,
                model.ProductTypeId == (int)ProductType.ModeratorFacilitateConsultationFee
                ? RunningNumberType.ModeratorInvoice
                : RunningNumberType.SellerProInvoice,
                engagement.SellerCustomerId) ;

            if(model.ServiceCharge > 0)
            {

                var newServiceChargeInvoice = new InvoiceDTO
                {
                    InvoiceFrom = null,
                    InvoiceTo = engagement.SellerCustomerId,
                    RefType = (int)InvoiceRefType.Payout
                };

                var createdServiceChargeInvoice = _invoiceService.CreateInvoice(actorId, newServiceChargeInvoice, RunningNumberType.ProInvoice);
                model.ServiceChargeInvoiceId = createdServiceChargeInvoice.Id;
            }

            //update payout request
            model.Status = (int)PayoutRequestStatus.Paid;
            model.Remark = remark;
            model.InvoiceId = createdInvoice.Id;
            model.UpdateAudit(actorId);
            _payoutRequestRepo.Update(model);

            var payoutRequestDTO = _mapper.Map<PayoutRequestDTO>(model);

            //check if it final 
            var isLastPayoutRequest = IsLastPayoutRequest(payoutRequestDTO, engagement);

            if(isLastPayoutRequest)
            {
                //update engagement to complete
                _engagementService.CompleteEngagement(actorId, model.ProductTypeId, model.RefId);

                //get left over deposit
                var depositBalance =
                    (from dr in _depositRequestRepo.Table
                .Where(dr => dr.Deleted == false
                && dr.Status == (int)DepositRequestStatus.Paid
                && dr.ProductTypeId == model.ProductTypeId
                && dr.RefId == model.RefId)
                     select new
                     {
                         OrderItemId = dr.OrderItemId,
                         Amount = dr.Amount
                     })
                .Concat(
                    from pr in _payoutRequestRepo.Table
                    .Where(pr => pr.Deleted == false
                    && pr.Status == (int)DepositRequestStatus.Paid
                    && pr.ProductTypeId == model.ProductTypeId
                    && pr.RefId == model.RefId)
                    select new
                    {
                        OrderItemId = (int?)null,
                        Amount = -1 * (pr.ServiceCharge + pr.Fee)
                    }
                )
                .GroupBy(x => 1)
                .Select(x => new
                {
                    LastDepositOrderItemId = x.Max(y => y.OrderItemId),
                    LeftOverDepositAmount = x.Sum(y => y.Amount)
                })
                .FirstOrDefault();

                //initial refund
                if(depositBalance != null
                    && depositBalance.LeftOverDepositAmount > 0
                    && depositBalance.LastDepositOrderItemId.HasValue)
                {
                    _refundRequestService.CreateRefundRequest(
                        actorId,
                        depositBalance.LastDepositOrderItemId.Value,
                        depositBalance.LeftOverDepositAmount);
                }
            }

            return payoutRequestDTO;
        }

        //public decimal GetLeftOverDepositOverPayout()
        //{

        //}

        public bool IsLastPayoutRequest(
            PayoutRequestDTO payoutRequestDTO,
            EngagementDTO engagementDTO)
        {
            var isLast = false;

            switch(engagementDTO.ProductType)
            {
                case ProductType.JobEnagegementFee:
                    {
                        if(engagementDTO.IsProjectPayout)
                        {
                            if(engagementDTO.EndMilestoneId == payoutRequestDTO.JobMilestoneId)
                            {
                                isLast = true;
                            }
                        }
                        else
                        {
                            if (engagementDTO.EndDate.HasValue
                                && payoutRequestDTO.EndDate.HasValue
                                && engagementDTO.EndDate.Value.Date == payoutRequestDTO.EndDate.Value.Date)
                            {
                                isLast = true;
                            }
                        }
                    }
                    break;
                case ProductType.ServiceEnagegementFee:
                    {
                        if (engagementDTO.EndDate.HasValue
                            && payoutRequestDTO.EndDate.HasValue
                            && engagementDTO.EndDate.Value.Date == payoutRequestDTO.EndDate.Value.Date)
                        {
                            isLast = true;
                        }
                    }
                    break;
                case ProductType.ConsultationEngagementFee:
                    {
                        isLast = true;
                    }
                    break;
            }


            return isLast;
        }

        /// <summary>
        /// get payout request summary
        /// </summary>
        /// <param name="refId"></param>
        /// <param name="productTypeId"></param>
        /// <returns></returns>
        public PayoutDetailDTO GetPayoutRequestSummary(int refId, int productTypeId)
        {
            var dto = new PayoutDetailDTO();

            var engagement = (
                from oi in _proOrderItemRepo.Table
                .Where(oi => oi.Deleted == false
                && oi.RefId == refId
                && oi.ProductTypeId == productTypeId)

                from pr in _payoutRequestRepo.Table
                .Where(pr => pr.Deleted == false
                && pr.OrderItemId == oi.Id
                && oi.ProductTypeId == productTypeId)
                select new
                {
                    oi,
                    pr
                }).ToList();
            if (engagement != null)
            {
                var totalFee = engagement.Where(x => x.pr.Status == (int)PayoutRequestStatus.Paid).Sum(x => x.pr.Fee);
                var totalServiceCharge = engagement.Where(x => x.pr.Status == (int)PayoutRequestStatus.Paid).Sum(x => x.pr.ServiceCharge);
                dto.TotalAmount = totalFee + totalServiceCharge;
            }
            return dto;
        }

        /// <summary>
        /// auto approve payout request after 3 days inactivity
        /// </summary>
        /// <param name="actorId"></param>
        /// <returns></returns>
        public List<PayoutRequestDTO> AutoApprovedPayoutRequest(int actorId = 1)
        {
            var dtos = new List<PayoutRequestDTO>();
            var timezone = _dateTimeHelper.DefaultStoreTimeZone;
            var hoursDiff = timezone.BaseUtcOffset.TotalHours;

            //get list of payout request to be approve
            var payoutRequests = 
                (from pr in _payoutRequestRepo.Table
                .Where(x => x.Deleted == false
                && x.Status == (int)PayoutRequestStatus.New
                && ((x.UpdatedOnUTC != null
                    && x.UpdatedOnUTC.Value.AddHours(hoursDiff).Date.AddDays(_payoutRequestSettings.AutoApprovalDays) <= DateTime.UtcNow.AddHours(hoursDiff).Date)
                    || (x.CreatedOnUTC.AddHours(hoursDiff).Date.AddDays(_payoutRequestSettings.AutoApprovalDays) <= DateTime.UtcNow.AddHours(hoursDiff).Date))
                    )
                from oi in _proOrderItemRepo.Table
                .Where(x=>x.Deleted == false
                && x.Id == pr.OrderItemId)
                select new
                {
                    PayoutRequet = pr,
                    ProductTypeId = oi.ProductTypeId,
                    RefId = oi.RefId
                }).ToList();

            var strPayoutRequestAutoApproveRemarks = _localizationService.GetResource("PayoutRequest.AutoApproveRemarks");
            var strPayoutRequestAutoApproveActorName = _localizationService.GetResource("PayoutRequest.AutoApproveActorName");

            payoutRequests.ForEach(x =>
            {
                x.PayoutRequet.Status = (int)PayoutRequestStatus.Approved;
                x.PayoutRequet.UpdateAudit(actorId);

                var dto = _mapper.Map<PayoutRequestDTO>(x.PayoutRequet);
                dto.ProductTypeId = x.ProductTypeId;
                dto.RefId = x.RefId;

                var remarkDTOs = dto.Remarks;

                remarkDTOs.Add(new RemarksDTO
                {
                    ActorName = strPayoutRequestAutoApproveActorName,
                    Remark = strPayoutRequestAutoApproveRemarks,
                    RemarkDate = DateTime.Now
                });
                x.PayoutRequet.Remark = JsonConvert.SerializeObject(remarkDTOs);
                dto.Remark = JsonConvert.SerializeObject(remarkDTOs);
                dtos.Add(dto);
            });

            _payoutRequestRepo.Update(payoutRequests.Select(x=>x.PayoutRequet).ToList());

            return dtos;
        }

        #endregion

    }
}
