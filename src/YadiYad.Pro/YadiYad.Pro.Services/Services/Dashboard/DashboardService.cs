using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Individual;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Services.Services.Dashboard;
using Nop.Core.Domain.Customers;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Services.DTO.Dashboard;

namespace YadiYad.Pro.Services.Individual
{
    public class DashboardService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<IndividualProfile> _IndividualProfileRepository;
        private readonly IRepository<JobApplication> _JobApplicationRepository;
        private readonly IRepository<JobProfile> _JobProfileRepository;
        private readonly IRepository<ProOrderItem> _ProOrderItemRepository;
        private readonly IRepository<ProOrder> _ProOrderRepository;
        private readonly IRepository<ConsultationInvitation> _ConsultationInvitationRepository;
        private readonly IRepository<ConsultationProfile> _ConsultationProfileRepository;
        private readonly IRepository<DepositRequest> _DepositRequestRepository;
        private readonly IRepository<OrganizationProfile> _OrganizationProfileRepository;
        private readonly IRepository<RefundRequest> _RefundRequestRepository;
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<PayoutRequest> _PayoutRequestRepository;
        private readonly IRepository<BusinessSegment> _BusinessSegmentRepository;
        private readonly BillingAddressService _billingAddressService;


        #endregion

        #region Ctor

        public DashboardService
            (IMapper mapper,
            IRepository<IndividualProfile> IndividualProfileRepository,
            IRepository<JobApplication> JobApplicationRepository,
            IRepository<JobProfile> JobProfileRepository,
            IRepository<ProOrderItem> ProOrderItemRepository,
            IRepository<ProOrder> ProOrderRepository,
            IRepository<ConsultationInvitation> ConsultationInvitationRepository,
            IRepository<ConsultationProfile> ConsultationProfileRepository,
            IRepository<DepositRequest> DepositRequestRepository,
            IRepository<OrganizationProfile> OrganizationProfileRepository,
            IRepository<RefundRequest> RefundRequestRepository,
            IRepository<Customer> CustomerRepository,
            IRepository<PayoutRequest> PayoutRequestRepository,
            IRepository<BusinessSegment> BusinessSegmentRepository,
        BillingAddressService billingAddressService)
        {
            _mapper = mapper;
            _IndividualProfileRepository = IndividualProfileRepository;
            _JobApplicationRepository = JobApplicationRepository;
            _billingAddressService = billingAddressService;
            _JobProfileRepository = JobProfileRepository;
            _ProOrderItemRepository = ProOrderItemRepository;
            _ProOrderRepository = ProOrderRepository;
            _ConsultationInvitationRepository = ConsultationInvitationRepository;
            _ConsultationProfileRepository = ConsultationProfileRepository;
            _DepositRequestRepository = DepositRequestRepository;
            _OrganizationProfileRepository = OrganizationProfileRepository;
            _RefundRequestRepository = RefundRequestRepository;
            _CustomerRepository = CustomerRepository;
            _PayoutRequestRepository = PayoutRequestRepository;
            _BusinessSegmentRepository = BusinessSegmentRepository;
        }

        #endregion


        #region Methods

        public int GetNoOfJobPendingRematch(int id)
        {
            var _jobAppId = new int[] { 12, 13 };
            
            var record = from ja in _JobApplicationRepository.Table
                         join jp in _JobProfileRepository.Table on ja.JobProfileId equals jp.Id
                         join poi in _ProOrderItemRepository.Table on ja.Id equals poi.RefId
                         join po in _ProOrderRepository.Table on poi.OrderId equals po.Id
                         where jp.Deleted == false
                         && poi.Deleted == false
                         && po.Deleted == false
                         && jp.CustomerId == id
                         && _jobAppId.Contains(ja.JobApplicationStatus)
                         && po.OrderStatusId == 30
                         && poi.Status == 1
                         && poi.ProductTypeId == 5
                         select ja.Id;

            var recordCount = record.Count();
            return recordCount;
        }

        public int GetNoOfConsultantJobPendingRematch(int id)
        {
            var _consultationAppId = new int[] { 6, 7 };

            var record = from ci in _ConsultationInvitationRepository.Table
                         join cp in _ConsultationProfileRepository.Table on ci.ConsultationProfileId equals cp.Id
                         join poi in _ProOrderItemRepository.Table on ci.Id equals poi.RefId
                         join po in _ProOrderRepository.Table on poi.OrderId equals po.Id
                         where ci.Deleted == false
                         && cp.Deleted == false
                         && poi.Deleted == false
                         && po.Deleted == false
                         && _consultationAppId.Contains(ci.ConsultationApplicationStatus)
                         && cp.OrganizationProfileId == id
                         && po.OrderStatusId == 1
                         && poi.Status == 1
                         && poi.ProductTypeId == 31
                         select ci.Id;

            var recordCount = record.Count();
            return recordCount;
        }

        public decimal GetDepositReserve(int id)
        {
            var record = from poi in _ProOrderItemRepository.Table
                         join po in _ProOrderRepository.Table on poi.OrderId equals po.Id
                         where po.CustomerId == id
                         && po.OrderStatusId == 30
                         && poi.Status == 1
                         select poi.Price;

            var recordSum = record.Sum();

            return recordSum;
        }

        public decimal GetDepositDue(int id)
        {
            var drStatus = new int[] { 0, 2, 3 };

            var record = from dr in _DepositRequestRepository.Table
                         where dr.DepositFrom == id
                         && drStatus.Contains(dr.Status)
                         select dr.Amount;

            var recordSum = record.Sum();

            return recordSum;
        }

        public int GetNewApplicant(int id)
        {
            var jobApplicants = from ja in _JobApplicationRepository.Table
                      join jp in _JobProfileRepository.Table on ja.JobProfileId equals jp.Id
                      where jp.Status == 1
                      select new
                      {
                          ja.Id,
                          BuyerCustomerId = jp.CustomerId,
                          IsApplicant =
                          (
                            ja.JobApplicationStatus == 4 ? true : false
                          )
                      };

            var consultantApplicants = from ci in _ConsultationInvitationRepository.Table
                                       join cp in _ConsultationProfileRepository.Table on ci.ConsultationProfileId equals cp.Id
                                       join op in _OrganizationProfileRepository.Table on cp.OrganizationProfileId equals op.Id
                                       select new
                                       {
                                           ci.Id,
                                           BuyerCustomerId = op.CustomerId,
                                           IsApplicant =
                                           (
                                             ci.ConsultationApplicationStatus == 2 && ci.IsApproved == true ? true : false
                                           )
                                       };

            var allApplicant = jobApplicants.Concat(consultantApplicants);

            var newApplicant = from e in allApplicant
                               where e.BuyerCustomerId == id
                               && e.IsApplicant == true
                               select e.Id;

            return newApplicant.Count();
        }

        public PagedListDTO<DashboardJobEngagementDTO> GetJobEngagementList(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            DashboardJobEngagementRequestFilter filterDTO = null)
        {
            var prStatus = new int[] { 1, 2 };
            var jobApplicationStatus = new int[] { 1, 2, 3, 4, 6, 12, 13, 14, 15, 17, 18, 19 };

            var joinQuery3 = from dr in _DepositRequestRepository.Table
                             where dr.ProductTypeId == 5
                             && dr.Status == 1
                             group dr by new { dr.RefId, dr.ProductTypeId }
                             into drGroup
                             select new
                             {
                                 Fee = drGroup.Sum(x => x.Amount),
                                 engagementId = drGroup.Key.RefId
                             };

            var joinQuery2 = from pr in _PayoutRequestRepository.Table
                             join poi in _ProOrderItemRepository.Table on pr.OrderItemId equals poi.Id
                             where poi.ProductTypeId == 5
                             && prStatus.Contains(pr.Status)
                             group pr by new { poi.RefId, poi.ProductTypeId }
                             into prGroup
                             select new
                             {
                                 //fee = (prGroup.Any(y => y.Fee == 0) ? 0 : prGroup.Sum(y => y.Fee)) +
                                 //       (prGroup.Any(y => y.OnSiteFee == 0) ? 0 : prGroup.Sum(y => y.OnSiteFee)) +
                                 //        (prGroup.Any(y => y.ServiceCharge == 0) ? 0 : prGroup.Sum(y => y.ServiceCharge)),
                                 fee = prGroup.Sum(y => y.Fee) + prGroup.Sum(y => y.ServiceCharge),
                                 engagementId = prGroup.Key.RefId
                             };

            var joinQuery = from ja in _JobApplicationRepository.Table
                            join jp in _JobProfileRepository.Table on ja.JobProfileId equals jp.Id
                            join co in _CustomerRepository.Table on jp.CustomerId equals co.Id
                            join poi in _ProOrderItemRepository.Table on new { p1 = ja.Id, p2 = 5, p3 = false } equals new { p1 = poi.RefId, p2 = poi.ProductTypeId, p3 = poi.Deleted }
                            join po in _ProOrderRepository.Table on new { p1 = poi.OrderId, p2 = 30, p3 = false } equals new { p1 = po.Id, p2 = po.OrderStatusId, p3 = po.Deleted }
                            join poi2 in _ProOrderItemRepository.Table on new { p1 = poi.Id, /*p2 = (decimal)0,*/ p3 = false } equals new { p1 = (int)poi2.OffsetProOrderItemId, /*p2 = poi2.Price, */p3 = poi2.Deleted } into pois
                            from poi2t in pois.DefaultIfEmpty()
                            join po2 in _ProOrderRepository.Table on new { p1 = poi2t.OrderId, p2 = false, p3 = 30 } equals new { p1 = po2.Id, p2 = po2.Deleted, p3 = po2.OrderStatusId} into pos
                            from po2t in pos.DefaultIfEmpty()
                            join rr in _RefundRequestRepository.Table on new { p1 = poi.Id, p2 = false } equals new { p1 = rr.OrderItemId, p2 = rr.Deleted } into refundR
                            from rrl in refundR.DefaultIfEmpty()
                            join pr in joinQuery2 on ja.Id equals pr.engagementId into jq2
                            from prt in jq2.DefaultIfEmpty()
                            join dr in joinQuery3 on ja.Id equals dr.engagementId into jq3
                            from drt in jq3.DefaultIfEmpty()
                            where ja.Deleted == false
                            && jp.Deleted == false
                            && !(poi.Id != 0 && po.Id == 0)
                            //&& !(poi2t.Id != 0 && po2t.Id == 0)
                            && jobApplicationStatus.Contains(ja.JobApplicationStatus)
                            && jp.CustomerId == filterDTO.CustomerId
                            select new DashboardJobEngagementDTO
                            {
                                EngagementId = ja.Id,
                                EngagementDate = ja.CreatedOnUTC,
                                EngagementTitle = jp.JobTitle,
                                JobApplicationStatus = ja.JobApplicationStatus,
                                StartDate = ja.StartDate,
                                IsEscrow = ja.IsEscrow,
                                DepositStatusCondition = poi.Status,
                                OffsettedAmount = poi2t.Price,
                                OffsetEngagementId = 
                                (
                                    poi2t.Id == 0 ? 0 : poi2t.RefId
                                ),
                                RefundAmount = 
                                (
                                    rrl.Id == 0 ? 0 : rrl.Amount
                                ),
                                RefundStatus = 
                                (
                                    rrl.Id == 0 ? 0 : rrl.Status
                                ),
                                TotalDepositAmount = drt.Fee,
                                TotalPayoutAmount = prt.fee

                            };


            var count = joinQuery.Count();

            var query = joinQuery
                .OrderBy(x => x.EngagementId)
                .Take(pageSize)
                .Skip(pageSize * pageIndex);

            var records = query
                .ToList()
                .Select(x => _mapper.Map<DashboardJobEngagementDTO>(x))
                .ToList();

            var responseDTO = new PagedListDTO<DashboardJobEngagementDTO>(records, pageIndex, pageSize, count);

            return responseDTO;
        }

        public PagedListDTO<DashboardConsultationEngagementDTO> GetConsultationEngagementList(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            DashboardJobEngagementRequestFilter filterDTO = null)
        {
            var prStatus = new int[] { 1, 2 };
            var jobApplicationStatus = new int[] { 4, 5, 6, 7 };

            var joinQuery3 = from dr in _DepositRequestRepository.Table
                             where dr.ProductTypeId == 5
                             && dr.Status == 1
                             group dr by new { dr.RefId, dr.ProductTypeId }
                             into drGroup
                             select new
                             {
                                 Fee = drGroup.Sum(x => x.Amount),
                                 engagementId = drGroup.Key.RefId
                             };

            var joinQuery2 = from pr in _PayoutRequestRepository.Table
                             join poi in _ProOrderItemRepository.Table on pr.OrderItemId equals poi.Id
                             where poi.ProductTypeId == 5
                             && prStatus.Contains(pr.Status)
                             group pr by new { poi.RefId, poi.ProductTypeId }
                             into prGroup
                             select new
                             {
                                 //fee = (prGroup.Any(y => y.Fee == 0) ? 0 : prGroup.Sum(y => y.Fee)) +
                                 //       (prGroup.Any(y => y.OnSiteFee == 0) ? 0 : prGroup.Sum(y => y.OnSiteFee)) +
                                 //        (prGroup.Any(y => y.ServiceCharge == 0) ? 0 : prGroup.Sum(y => y.ServiceCharge)),
                                 fee = prGroup.Sum(y => y.Fee) + prGroup.Sum(y => y.ServiceCharge),
                                 engagementId = prGroup.Key.RefId
                             };

            var joinQuery = from ci in _ConsultationInvitationRepository.Table
                            join cp in _ConsultationProfileRepository.Table on ci.ConsultationProfileId equals cp.Id
                            join bs in _BusinessSegmentRepository.Table on cp.SegmentId equals bs.Id
                            join op in _OrganizationProfileRepository.Table on cp.OrganizationProfileId equals op.Id
                            join co in _CustomerRepository.Table on op.CustomerId equals co.Id
                            join poi in _ProOrderItemRepository.Table on new { p1 = ci.Id, p2 = 5, p3 = false } equals new { p1 = poi.RefId, p2 = poi.ProductTypeId, p3 = poi.Deleted }
                            join po in _ProOrderRepository.Table on new { p1 = poi.OrderId, p2 = 30, p3 = false } equals new { p1 = po.Id, p2 = po.OrderStatusId, p3 = po.Deleted }
                            join poi2 in _ProOrderItemRepository.Table on new { p1 = poi.Id, /*p2 = (decimal)0,*/ p3 = false } equals new { p1 = (int)poi2.OffsetProOrderItemId, /*p2 = poi2.Price, */p3 = poi2.Deleted } into pois
                            from poi2t in pois.DefaultIfEmpty()
                            join po2 in _ProOrderRepository.Table on new { p1 = poi2t.OrderId, p2 = false, p3 = 30 } equals new { p1 = po2.Id, p2 = po2.Deleted, p3 = po2.OrderStatusId } into pos
                            from po2t in pos.DefaultIfEmpty()
                            join rr in _RefundRequestRepository.Table on new { p1 = poi.Id, p2 = false } equals new { p1 = rr.OrderItemId, p2 = rr.Deleted } into refundR
                            from rrl in refundR.DefaultIfEmpty()
                            join pr in joinQuery2 on ci.Id equals pr.engagementId into jq2
                            from prt in jq2.DefaultIfEmpty()
                            join dr in joinQuery3 on ci.Id equals dr.engagementId into jq3
                            from drt in jq3.DefaultIfEmpty()
                            where ci.Deleted == false
                            && cp.Deleted == false
                            && !(poi.Id != 0 && po.Id == 0)
                            //&& !(poi2t.Id != 0 && po2t.Id == 0)
                            && jobApplicationStatus.Contains(ci.ConsultationApplicationStatus)
                            && op.CustomerId == filterDTO.CustomerId
                            select new DashboardConsultationEngagementDTO
                            {
                                EngagementId = ci.Id,
                                EngagementDate = ci.CreatedOnUTC,
                                Segment = bs.Name,
                                ConsultationRequest = cp.Id,
                                ConsultationApplicationStatus = ci.ConsultationApplicationStatus,
                                AppointmentStartDate = ci.AppointmentStartDate,
                                DepositStatusCondition = poi.Status,
                                OffsettedAmount = poi2t.Price,
                                OffsetEngagementId =
                                (
                                    poi2t.Id == 0 ? 0 : poi2t.RefId
                                ),
                                RefundAmount =
                                (
                                    rrl.Id == 0 ? 0 : rrl.Amount
                                ),
                                RefundId = rrl.Id,
                                RefundStatus = rrl.Status,
                                TotalDepositAmount = drt.Fee,
                                TotalPayoutAmount = prt.fee
                            };


            var count = joinQuery.Count();

            var query = joinQuery
                .OrderBy(x => x.EngagementId)
                .Take(pageSize)
                .Skip(pageSize * pageIndex);

            var records = query
                .ToList()
                .Select(x => _mapper.Map<DashboardConsultationEngagementDTO>(x))
                .ToList();

            var responseDTO = new PagedListDTO<DashboardConsultationEngagementDTO>(records, pageIndex, pageSize, count);

            return responseDTO;
        }
        #endregion
    }
}
