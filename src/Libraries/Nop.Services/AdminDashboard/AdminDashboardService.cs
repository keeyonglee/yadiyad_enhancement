using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Payout;
using Nop.Data;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;

namespace Nop.Services.AdminDashboard
{
    public partial class AdminDashboardService : IAdminDashboardService
    {
        #region Fields

        private readonly IRepository<JobProfile> _JobProfileRepository;
        private readonly IRepository<JobApplication> _JobApplicationRepository;
        private readonly IRepository<ServiceApplication> _ServiceApplicationRepository;
        private readonly IRepository<Order> _OrderRepository;
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<ProOrder> _ProOrderRepository;
        private readonly IRepository<CustomerRole> _CustomerRoleRepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _CustomerCustomerRoleMappingRepository;
        private readonly IRepository<ProOrderItem> _ProOrderItemRepository;
        private readonly IRepository<PayoutRequest> _PayoutRequestRepository;
        private readonly IRepository<RefundRequest> _RefundRequestRepository;
        private readonly IRepository<JobSeekerProfile> _JobSeekerProfileRepository;
        private readonly IRepository<JobSeekerCategory> _JobSeekerCategoryRepository;
        private readonly IRepository<JobServiceCategory> _JobServiceCategoryRepository;
        private readonly IRepository<ServiceProfile> _ServiceProfileRepository;
        private readonly IRepository<ServiceExpertise> _ServiceExpertiseRepository;
        private readonly IRepository<Expertise> _ExpertiseRepository;
        private readonly IRepository<OrganizationProfile> _OrganizationProfileRepository;
        private readonly IRepository<IndividualProfile> _IndividualProfileRepository;
        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public AdminDashboardService(
            IRepository<JobProfile> JobProfileRepository,
            IRepository<JobApplication> JobApplicationRepository,
            IRepository<ServiceApplication> ServiceApplicationRepository,
            IRepository<Order> OrderRepository,
            IRepository<Customer> CustomerRepository,
            IRepository<ProOrder> ProOrderRepository,
            IRepository<CustomerRole> CustomerRoleRepository,
            IRepository<CustomerCustomerRoleMapping> CustomerCustomerRoleMappingRepository,
            IRepository<ProOrderItem> ProOrderItemRepository,
            IRepository<PayoutRequest> PayoutRequestRepository,
            IRepository<RefundRequest> RefundRequestRepository,
            IRepository<JobSeekerProfile> JobSeekerProfileRepository,
            IRepository<JobSeekerCategory> JobSeekerCategoryRepository,
            IRepository<JobServiceCategory> JobServiceCategoryRepository,
            IRepository<ServiceProfile> ServiceProfileRepository,
            IRepository<ServiceExpertise> ServiceExpertiseRepository,
            IRepository<Expertise> ExpertiseRepository,
            IRepository<OrganizationProfile> OrganizationProfileRepository,
            IRepository<IndividualProfile> IndividualProfileRepository,
            IOrderService orderService)

        {
            _JobProfileRepository = JobProfileRepository;
            _JobApplicationRepository = JobApplicationRepository;
            _ServiceApplicationRepository = ServiceApplicationRepository;
            _OrderRepository = OrderRepository;
            _CustomerRepository = CustomerRepository;
            _ProOrderRepository = ProOrderRepository;
            _CustomerRoleRepository = CustomerRoleRepository;
            _CustomerCustomerRoleMappingRepository = CustomerCustomerRoleMappingRepository;
            _ProOrderItemRepository = ProOrderItemRepository;
            _PayoutRequestRepository = PayoutRequestRepository;
            _RefundRequestRepository = RefundRequestRepository;
            _JobSeekerProfileRepository = JobSeekerProfileRepository;
            _JobSeekerCategoryRepository = JobSeekerCategoryRepository;
            _JobServiceCategoryRepository = JobServiceCategoryRepository;
            _ServiceProfileRepository = ServiceProfileRepository;
            _ServiceExpertiseRepository = ServiceExpertiseRepository;
            _ExpertiseRepository = ExpertiseRepository;
            _OrganizationProfileRepository = OrganizationProfileRepository;
            _IndividualProfileRepository = IndividualProfileRepository;
            _orderService = orderService;
        }

        #endregion

        #region Utilities

        private class Grouper
        {
            public int Id { get; set; }
            public int Counter { get; set; }
            public decimal Amounter { get; set; }
        }

        private DateTime GetCurrentWeekStartDate(DateTime date)
        {
            DayOfWeek currentDay = date.DayOfWeek;
            int daysTillCurrentDay = currentDay - DayOfWeek.Monday;
            return DateTime.Now.AddDays(-daysTillCurrentDay);
        }

        #endregion

        #region Methods

        #region Main
        public int GetProJobHired(DateTime date)
        {
            var _jobAppStatusId = new int[]
            {
                (int)JobApplicationStatus.Hired,
                (int)JobApplicationStatus.PendingPaymentVerification,
                (int)JobApplicationStatus.RevisePaymentRequired,
                (int)JobApplicationStatus.Completed
            };

            var record = from ja in _JobApplicationRepository.Table
                         where !ja.Deleted
                         && _jobAppStatusId.Contains(ja.JobApplicationStatus)
                         && ja.CreatedOnUTC.Month == date.Month
                         && ja.CreatedOnUTC.Year == date.Year
                         select ja.Id;

            return record.Count();
        }

        public int GetProJobPost(DateTime date)
        {
            var record = from jp in _JobProfileRepository.Table
                         where !jp.Deleted
                         && jp.Status != (int)JobProfileStatus.Draft
                         && jp.CreatedOnUTC.Month == date.Month
                         && jp.CreatedOnUTC.Year == date.Year
                         select jp.Id;

            return record.Count();
        }

        public int GetProServiceHired(DateTime date)
        {
            var _serviceAppStatusId = new int[]
            {
                (int)ServiceApplicationStatus.Paid,
                (int)ServiceApplicationStatus.Completed
            };

            var record = from sa in _ServiceApplicationRepository.Table
                         where !sa.Deleted
                         && _serviceAppStatusId.Contains(sa.Status)
                         && sa.CreatedOnUTC.Month == date.Month
                         && sa.CreatedOnUTC.Year == date.Year
                         select sa.Id;

            return record.Count();
        }

        public int GetShuqOrders(DateTime date)
        {
            var record = from o in _OrderRepository.Table
                         where !o.Deleted
                         && o.OrderStatusId != (int)OrderStatus.Cancelled
                         && o.CreatedOnUtc.Month == date.Month
                         && o.CreatedOnUtc.Year == date.Year
                         select o.Id;

            return record.Count();
        }

        public int GetMemberSignUpMonthly(DateTime date)
        {
            var record = from c in _CustomerRepository.Table
                         where !c.Deleted
                         && c.Active
                         && c.Email != null
                         && c.CreatedOnUtc.Month == date.Month
                         && c.CreatedOnUtc.Year == date.Year
                         select c.Id;

            return record.Count();
        }

        public int GetMemberSignUpWeekly(DateTime date)
        {
            var startDate = GetCurrentWeekStartDate(date);
            var endDate = startDate.AddDays(7);

            var record = from c in _CustomerRepository.Table
                         where !c.Deleted
                         && c.Active
                         && c.Email != null
                         && c.CreatedOnUtc >= startDate
                         && c.CreatedOnUtc < endDate
                         select c.Id;

            return record.Count();
        }

        public int GetProOrganizationSignUpMonthly(DateTime date)
        {
            var record = from c in _CustomerRepository.Table
                         join crm in _CustomerCustomerRoleMappingRepository.Table on c.Id equals crm.CustomerId
                         join cr in _CustomerRoleRepository.Table on crm.CustomerRoleId equals cr.Id
                         where c.Active
                         && !c.Deleted
                         && cr.Active
                         && cr.Id == 6
                         && c.CreatedOnUtc.Month == date.Month
                         && c.CreatedOnUtc.Year == date.Year
                         select c.Id;

            return record.Count();

        }

        public int GetProOrganizationSignUpWeekly(DateTime date)
        {
            var startDate = GetCurrentWeekStartDate(date);
            var endDate = startDate.AddDays(7);

            var record = from c in _CustomerRepository.Table
                         join crm in _CustomerCustomerRoleMappingRepository.Table on c.Id equals crm.CustomerId
                         join cr in _CustomerRoleRepository.Table on crm.CustomerRoleId equals cr.Id
                         where c.Active
                         && !c.Deleted
                         && cr.Active
                         && cr.Id == 6
                         && c.CreatedOnUtc >= startDate
                         && c.CreatedOnUtc < endDate
                         select c.Id;

            return record.Count();

        }

        public int GetShuqVendorSignUpMonthly(DateTime date)
        {
            var record = from c in _CustomerRepository.Table
                         join crm in _CustomerCustomerRoleMappingRepository.Table on c.Id equals crm.CustomerId
                         join cr in _CustomerRoleRepository.Table on crm.CustomerRoleId equals cr.Id
                         where c.Active
                         && !c.Deleted
                         && cr.Active
                         && cr.Id == 5
                         && c.CreatedOnUtc.Month == date.Month
                         && c.CreatedOnUtc.Year == date.Year
                         select c.Id;

            return record.Count();
        }

        public int GetShuqVendorSignUpWeekly(DateTime date)
        {
            var startDate = GetCurrentWeekStartDate(date);
            var endDate = startDate.AddDays(7);

            var record = from c in _CustomerRepository.Table
                         join crm in _CustomerCustomerRoleMappingRepository.Table on c.Id equals crm.CustomerId
                         join cr in _CustomerRoleRepository.Table on crm.CustomerRoleId equals cr.Id
                         where c.Active
                         && !c.Deleted
                         && cr.Active
                         && cr.Id == 5
                         && c.CreatedOnUtc >= startDate
                         && c.CreatedOnUtc < endDate
                         select c.Id;

            return record.Count();
        }

        public decimal GetTransactionValueProMonthly(DateTime date)
        {
            var record1 = from poi in _ProOrderItemRepository.Table
                          join po in _ProOrderRepository.Table on poi.OrderId equals po.Id
                          where !poi.Deleted
                          && !po.Deleted
                          && po.OrderStatusId == (int)OrderStatus.Complete
                          && po.PaymentStatusId == (int)PaymentStatus.Paid
                          && poi.CreatedOnUTC.Month == date.Month
                          && poi.CreatedOnUTC.Year == date.Year
                          select poi.Price;

            var record2 = from por in _PayoutRequestRepository.Table
                          where !por.Deleted
                          && por.Status == (int)PayoutRequestStatus.Paid
                          && por.CreatedOnUTC.Month == date.Month
                          && por.CreatedOnUTC.Year == date.Year
                          select por.Fee;

            var record3 = from rr in _RefundRequestRepository.Table
                          where !rr.Deleted
                          && rr.Status == (int)RefundStatus.Paid
                          && rr.CreatedOnUTC.Month == date.Month
                          && rr.CreatedOnUTC.Year == date.Year
                          select rr.Amount;

            return record1.Sum() + record2.Sum() - record3.Sum();
        }

        public decimal GetTransactionValueProWeekly(DateTime date)
        {
            var startDate = GetCurrentWeekStartDate(date);
            var endDate = startDate.AddDays(7);

            var record1 = from poi in _ProOrderItemRepository.Table
                          join po in _ProOrderRepository.Table on poi.OrderId equals po.Id
                          where !poi.Deleted
                          && !po.Deleted
                          && po.OrderStatusId == (int)OrderStatus.Complete
                          && po.PaymentStatusId == (int)PaymentStatus.Paid
                          && po.CreatedOnUTC >= startDate
                          && po.CreatedOnUTC < endDate
                          select poi.Price;

            var record2 = from por in _PayoutRequestRepository.Table
                          where !por.Deleted
                          && por.Status == (int)PayoutRequestStatus.Paid
                          && por.CreatedOnUTC >= startDate
                          && por.CreatedOnUTC < endDate
                          select por.Fee;

            var record3 = from rr in _RefundRequestRepository.Table
                          where !rr.Deleted
                          && rr.Status == (int)RefundStatus.Paid
                          && rr.CreatedOnUTC >= startDate
                          && rr.CreatedOnUTC < endDate
                          select rr.Amount;

            return record1.Sum() + record2.Sum() - record3.Sum();
        }

        public decimal GetTransactionValueShuqMonthly(DateTime date)
        {
            var record = from o in _OrderRepository.Table
                         where !o.Deleted
                         && o.OrderStatusId == (int)OrderStatus.Complete
                         && o.CreatedOnUtc.Month == date.Month
                         && o.CreatedOnUtc.Year == date.Year
                         select o.OrderTotal;

            return record.Sum();
        }

        public decimal GetTransactionValueShuqWeekly(DateTime date)
        {
            var startDate = GetCurrentWeekStartDate(date);
            var endDate = startDate.AddDays(7);

            var record = from o in _OrderRepository.Table
                         where !o.Deleted
                         && o.OrderStatusId == (int)OrderStatus.Complete
                         && o.CreatedOnUtc >= startDate
                         && o.CreatedOnUtc < endDate
                         select o.OrderTotal;

            return record.Sum();
        }

        public decimal GetServiceChargeProMonthly(DateTime date)
        {
            var _productTypeId = new int[]
            {
                (int)ProductType.ApplyJobSubscription,
                (int)ProductType.ViewJobCandidateFullProfileSubscription,
                (int)ProductType.ServiceEnagegementMatchingFee,
                (int)ProductType.ConsultationEngagementMatchingFee,

            };

            var _productTypeIdEscrow = new int[]
            {
                (int)ProductType.ConsultationEscrowFee,
                (int)ProductType.JobEscrowFee,
                (int)ProductType.ServiceEscrowFee,
            };

            var record1 = from poi in _ProOrderItemRepository.Table
                          join po in _ProOrderRepository.Table
                          on poi.OrderId equals po.Id
                          where !poi.Deleted
                          && _productTypeId.Contains(poi.ProductTypeId)
                          && po.PaymentStatusId == (int)PaymentStatus.Paid
                          && poi.CreatedOnUTC.Month == date.Month
                          && poi.CreatedOnUTC.Year == date.Year
                          select poi.Price;

            var record2 = from poi in _ProOrderItemRepository.Table
                          join rr in _RefundRequestRepository.Table on poi.Id equals rr.OrderItemId
                          where !poi.Deleted
                          && !rr.Deleted
                          && _productTypeId.Contains(poi.ProductTypeId)
                          && poi.CreatedOnUTC.Month == date.Month
                          && poi.CreatedOnUTC.Year == date.Year
                          select rr.ServiceCharge;

            var record3 = from pr in _PayoutRequestRepository.Table
                          where !pr.Deleted
                          && _productTypeIdEscrow.Contains(pr.ProductTypeId)
                          && pr.CreatedOnUTC.Month == date.Month
                          && pr.CreatedOnUTC.Year == date.Year
                          select pr.ServiceCharge;

            return record1.Sum() - record3.Sum() - record2.Sum();
        }

        public decimal GetServiceChargeProWeekly(DateTime date)
        {
            var startDate = GetCurrentWeekStartDate(date);
            var endDate = startDate.AddDays(7);

            var _productTypeId = new int[]
           {
                (int)ProductType.ApplyJobSubscription,
                (int)ProductType.ViewJobCandidateFullProfileSubscription,
                (int)ProductType.ServiceEnagegementMatchingFee,
                (int)ProductType.ConsultationEngagementMatchingFee,

           };

            var _productTypeIdEscrow = new int[]
            {
                (int)ProductType.ConsultationEscrowFee,
                (int)ProductType.JobEscrowFee,
                (int)ProductType.ServiceEscrowFee,
            };

            var record1 = from poi in _ProOrderItemRepository.Table
                          join po in _ProOrderRepository.Table
                          on poi.OrderId equals po.Id
                          where !poi.Deleted
                          && _productTypeId.Contains(poi.ProductTypeId)
                          && po.PaymentStatusId == (int)PaymentStatus.Paid
                          && poi.CreatedOnUTC >= startDate
                          && poi.CreatedOnUTC < endDate
                          select poi.Price;

            var record2 = from poi in _ProOrderItemRepository.Table
                          join rr in _RefundRequestRepository.Table on poi.Id equals rr.OrderItemId
                          where !poi.Deleted
                          && !rr.Deleted
                          && _productTypeId.Contains(poi.ProductTypeId)
                          && poi.CreatedOnUTC >= startDate
                          && poi.CreatedOnUTC < endDate
                          select rr.ServiceCharge;

            var record3 = from pr in _PayoutRequestRepository.Table
                          where !pr.Deleted
                          && _productTypeIdEscrow.Contains(pr.ProductTypeId)
                          && pr.CreatedOnUTC >= startDate
                          && pr.CreatedOnUTC < endDate
                          select pr.ServiceCharge;

            return record1.Sum() - record3.Sum() - record2.Sum();
        }

        public decimal GetServiceChargeShuqMonthly(DateTime selectedDate)
        {
            return 0;
        }

        public decimal GetServiceChargeShuqWeekly(DateTime selectedDate)
        {
            return 0;
        }

        #endregion

        #region Pro

        public List<TopSegmentsChartModel> GetTopJobCVSegments(List<string> colorCodes)
        {
            var record2 = from jsc in _JobServiceCategoryRepository.Table
                          join jsc2 in _JobSeekerCategoryRepository.Table on jsc.Id equals jsc2.CategoryId into jsc3

                          from jsc2 in jsc3.DefaultIfEmpty()
                          join jsp in _JobSeekerProfileRepository.Table on jsc2.JobSeekerProfileId equals jsp.Id into jsc2jsp

                          from jsp in jsc2jsp.DefaultIfEmpty()
                          group new { CategoryId = jsc.Id, JobSeekerProfileId = jsp.Id } by new { jsc.Name, jsc.Id }

                          into jscGroup
                          select new TopSegmentsChartModel
                          {
                              Name = jscGroup.Key.Name,
                              Id = jscGroup.Key.Id,
                              Quantity = jscGroup.Sum(x => x.JobSeekerProfileId != default ? 1 : 0)
                          };

            var listRecord = record2.OrderBy(x => x.Id).ToList();

            for (int i = 0; i < listRecord.Count(); i++)
            {
                listRecord[i].ColorCode = colorCodes[i];
            }

            return  listRecord.OrderByDescending(x => x.Quantity).ToList();
        }

        public List<TopSegmentsChartModel> GetTopServiceSegments(List<string> colorCodes)
        {
            var record2 = from jsc in _JobServiceCategoryRepository.Table
                          join e in _ExpertiseRepository.Table on jsc.Id equals e.JobServiceCategoryId into jsce

                          from e in jsce.DefaultIfEmpty()
                          join se in _ServiceExpertiseRepository.Table on e.Id equals se.ExpertiseId into ese

                          from se in ese.DefaultIfEmpty()
                          join sp in _ServiceProfileRepository.Table on se.ServiceProfileId equals sp.Id into sesp

                          from sp in sesp.DefaultIfEmpty()
                          group new {CategoryId = jsc.Id, ServiceProfileId = sp.Id} by new {jsc.Id, jsc.Name}
                          into spGroup
                          select new TopSegmentsChartModel
                          {
                              Name = spGroup.Key.Name,
                              Id = spGroup.Key.Id,
                              Quantity = spGroup.Sum(x => x.ServiceProfileId != default ? 1 : 0)
                          };

            //var record = from sp in _ServiceProfileRepository.Table
            //             join se in _ServiceExpertiseRepository.Table on sp.Id equals se.ServiceProfileId
            //             join e in _ExpertiseRepository.Table on se.ExpertiseId equals e.Id
            //             join jsc in _JobServiceCategoryRepository.Table on e.JobServiceCategoryId equals jsc.Id
            //             where e.Published 
            //             group jsc.Id by new { jsc.Name }
            //             into eGroup
            //             select new TopSegmentsChartModel
            //             {
            //                 Name = eGroup.Key.Name,
            //                 Quantity = eGroup.Count()
            //             };

            var listRecord = record2.OrderBy(x => x.Id).ToList();

            for (int i = 0; i < listRecord.Count(); i++)
            {
                listRecord[i].ColorCode = colorCodes[i];
            }

            return listRecord.OrderByDescending(x => x.Quantity).ToList();
        }

        public PagedList<TopOrganizationsTableModel> GetTopOrganizations()
        {
            var pageIndex = 0;
            var pageSize = 10;
            var _jobAppStatusId = new int[]
           {
                (int)JobApplicationStatus.Hired,
                (int)JobApplicationStatus.Completed
           };

            var query = from op in _OrganizationProfileRepository.Table
                        join c in _CustomerRepository.Table on op.CustomerId equals c.Id
                        join ja in _JobApplicationRepository.Table on op.Id equals ja.OrganizationProfileId
                        join jp in _JobProfileRepository.Table on ja.JobProfileId equals jp.Id
                        where !ja.Deleted
                        group new { ja.JobApplicationStatus, jp.Id } by new { op.Name, op.ContactPersonName, op.ContactPersonEmail, op.ContactPersonContact }
                        into opGroup
                        select new TopOrganizationsTableModel
                        {
                            Name = opGroup.Key.Name,
                            ContactPerson = opGroup.Key.ContactPersonName,
                            ContactPersonEmail = opGroup.Key.ContactPersonEmail,
                            ContactPersonPhone = opGroup.Key.ContactPersonContact,
                            TotalCandidateHired = opGroup.Sum(s => _jobAppStatusId.Contains(s.JobApplicationStatus) ? 1 : 0),
                            JobPostCount = opGroup.Select(s => s.Id).Count()
                        };

            query = query.OrderByDescending(x => x.TotalCandidateHired);
            return new PagedList<TopOrganizationsTableModel>(query, pageIndex, pageSize);
        }

        public PagedList<TopOrganizationsTableModel> GetTopIndividuals()
        {
            var pageIndex = 0;
            var pageSize = 10;

            //var query = from ip in _IndividualProfileRepository.Table
            //            join c in _CustomerRepository.Table on ip.CustomerId equals c.Id
            //            join pr in _PayoutRequestRepository.Table on c.Id equals pr.PayoutTo
            //            where !ip.Deleted
            //            && !c.Deleted
            //            && !pr.Deleted
            //            group pr by new { ip.FullName, ip.Email, ip.ContactNo}
            //            into ipGroup
            //            select new TopOrganizationsTableModel
            //            {
            //                ContactPerson = ipGroup.Key.FullName,
            //                ContactPersonEmail = ipGroup.Key.Email,
            //                ContactPersonPhone = ipGroup.Key.ContactNo,
            //                JobEngagementAmount = ipGroup.Sum(s => s.Fee)
            //            };

            var query2 = from pr in _PayoutRequestRepository.Table
                         where !pr.Deleted
                         group pr by new { pr.PayoutTo }
                         into prGroup
                         select new Grouper
                         {
                             Id = prGroup.Key.PayoutTo,
                             Amounter = prGroup.Sum(s => s.Fee)
                         };

            var query3 = from g in query2
                         join c in _CustomerRepository.Table on g.Id equals c.Id
                         join ip in _IndividualProfileRepository.Table on c.Id equals ip.CustomerId
                         select new TopOrganizationsTableModel
                         {
                             ContactPerson = ip.FullName,
                             ContactPersonEmail = ip.Email,
                             JobEngagementAmount = g.Amounter,
                             ContactPersonPhone = ip.ContactNo
                         };


            query3 = query3.OrderByDescending(x => x.JobEngagementAmount);
            return new PagedList<TopOrganizationsTableModel>(query3, pageIndex, pageSize);
        }

        public List<TopSegmentsChartModel> GetTopJobCVSegmentsCharge(List<string> colorCodes)
        {
            var _productTypeJob = new int[]
            {
                (int)ProductType.JobEnagegementFee,
                (int)ProductType.JobEscrowFee,
                (int)ProductType.JobBuyerCancellationAdminCharges,
            };

            var query = from jsc in _JobServiceCategoryRepository.Table
                          join jp in _JobProfileRepository.Table on jsc.Id equals jp.CategoryId into jpjsc
                          from jp in jpjsc.DefaultIfEmpty()
                          join ja in _JobApplicationRepository.Table on jp.Id equals ja.JobProfileId into jajp
                          from ja in jajp.DefaultIfEmpty()
                          join pr in _PayoutRequestRepository.Table.Where(x => _productTypeJob.Contains(x.ProductTypeId) && !x.Deleted)
                          on ja.Id equals pr.RefId into japr
                          from pr in japr.DefaultIfEmpty()
                          group new { CategoryId = jsc.Id, PayoutRequestId = pr.Id } by new { jsc.Name, jsc.Id }
                          into jscGroup
                          select new TopSegmentsChartModel
                          {
                              Name = jscGroup.Key.Name,
                              Quantity = jscGroup.Sum(x => x.PayoutRequestId != default ? 1 : 0),
                              Id = jscGroup.Key.Id
                          };

            var listRecord = query.OrderBy(x => x.Id).ToList();

            for (int i = 0; i < listRecord.Count(); i++)
            {
                listRecord[i].ColorCode = colorCodes[i];
            }

            return listRecord.OrderByDescending(x => x.Quantity).ToList();
        }

        public List<TopSegmentsChartModel> GetTopServiceSegmentsCharge(List<string> colorCodes)
        {
            var _productTypeService = new int[]
            {
                (int)ProductType.ServiceEnagegementMatchingFee,
                (int)ProductType.ServiceEnagegementFee,
                (int)ProductType.ServiceEscrowFee,
                (int)ProductType.ServiceBuyerCancellationAdminCharges,
            };

            var query3 = from jsc in _JobServiceCategoryRepository.Table
                         join e in _ExpertiseRepository.Table on jsc.Id equals e.JobServiceCategoryId into jsce

                         from e in jsce.DefaultIfEmpty()
                         join se in _ServiceExpertiseRepository.Table on e.Id equals se.ExpertiseId into ese

                         from se in ese.DefaultIfEmpty()
                         join sp in _ServiceProfileRepository.Table on se.ServiceProfileId equals sp.Id into sesp

                         from sp in sesp.DefaultIfEmpty()
                         join sa in _ServiceApplicationRepository.Table on sp.Id equals sa.ServiceProfileId into spsa

                         from sa in spsa.DefaultIfEmpty()
                         join pr in _PayoutRequestRepository.Table.Where(x => _productTypeService.Contains(x.ProductTypeId) && !x.Deleted) 
                         on sa.Id equals pr.RefId into sapr

                         from pr in sapr.DefaultIfEmpty()
                         group new { CategoryId = jsc.Id, PayoutRequestId = pr.Id} by new { jsc.Name, jsc.Id }
                         into prGroup
                         select new TopSegmentsChartModel
                         {
                             Name = prGroup.Key.Name,
                             Quantity = prGroup.Sum(x => x.PayoutRequestId != default ? 1 : 0),
                             Id = prGroup.Key.Id
                         };

            var listRecord = query3.OrderBy(x => x.Id).ToList();

            for (int i = 0; i < listRecord.Count(); i++)
            {
                listRecord[i].ColorCode = colorCodes[i];
            }

            return listRecord.OrderByDescending(x => x.Quantity).ToList();

            //var query2 = from pr in _PayoutRequestRepository.Table
            //             join sa in _ServiceApplicationRepository.Table on pr.RefId equals sa.Id
            //             join sp in _ServiceProfileRepository.Table on sa.ServiceProfileId equals sp.Id
            //             join se in _ServiceExpertiseRepository.Table on sp.Id equals se.ServiceProfileId
            //             join e in _ExpertiseRepository.Table on se.ExpertiseId equals e.Id
            //             join jsc in _JobServiceCategoryRepository.Table on e.JobServiceCategoryId equals jsc.Id
            //             where !pr.Deleted
            //             && _productTypeService.Contains(pr.ProductTypeId)
            //             group jsc.Name by new { jsc.Name }
            //             into jscGroup
            //             select new TopSegmentsChartModel
            //             {
            //                 Name = jscGroup.Key.Name,
            //                 Quantity = jscGroup.Count()
            //             };
            //return query2.OrderByDescending(x => x.Quantity).ToList();
        }

        #endregion

        #region Shuq

        public int GetNumberOfOrdersByVendorId(int vendorId, DateTime from, DateTime to)
        {
            var orders = _orderService.SearchOrders(
                vendorId: vendorId,
                createdFromUtc: from,
                createdToUtc: to);

            return orders.TotalCount;
        }

        #endregion

        #endregion
    }
}
