using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.YadiyadReporting.DTO;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.JobSeeker;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Organization;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Service;

namespace Nop.Services.YadiyadReporting
{
    public class YadiyadReportingContactsService : CustomerReportService, IYadiyadReportingContactsService
    {
        #region Fields

        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<OrganizationProfile> _organizationProfileRepository;
        private readonly IRepository<BusinessSegment> _businessSegmentRepository;
        private readonly IRepository<IndividualProfile> _individualProfilerepository;
        private readonly IRepository<ServiceProfile> _serviceProfilerepository;
        private readonly IRepository<JobSeekerProfile> _jobSeekerProfilerepository;
        private readonly IRepository<CustomerCustomerRoleMapping> _customerCustomerRoleMappingrepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<ProOrderItem> _orderItemrepository;
        private readonly IRepository<PayoutRequest> _payoutRequestrepository;
        private readonly IRepository<OrderPayoutRequest> _orderPayoutRequestrepository;
        private readonly IRepository<ProInvoice> _proInvoicerepository;
        private readonly IRepository<Invoice> _invoicerepository;

        #endregion

        #region Ctor

        public YadiyadReportingContactsService(ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IRepository<Customer> customerRepository,
            IRepository<Order> orderRepository,
            IRepository<OrganizationProfile> organizationProfileRepository,
            IRepository<BusinessSegment> businessSegmentRepository,
            IRepository<IndividualProfile> individualProfilerepository,
            IRepository<ServiceProfile> serviceProfilerepository,
            IRepository<JobSeekerProfile> jobSeekerProfilerepository,
            IRepository<CustomerCustomerRoleMapping> customerCustomerRoleMappingrepository,
            IStaticCacheManager staticCacheManager,
            IRepository<ProOrderItem> orderItemrepository,
            IRepository<PayoutRequest> payoutRequestrepository,
            IRepository<OrderPayoutRequest> orderPayoutRequestrepository,
            IRepository<ProInvoice> proInvoicerepository,
            IRepository<Invoice> invoicerepository) :
            base(customerService, dateTimeHelper, customerRepository, orderRepository)
        {
            _customerRepository = customerRepository;
            _organizationProfileRepository = organizationProfileRepository;
            _businessSegmentRepository = businessSegmentRepository;
            _individualProfilerepository = individualProfilerepository;
            _serviceProfilerepository = serviceProfilerepository;
            _jobSeekerProfilerepository = jobSeekerProfilerepository;
            _customerCustomerRoleMappingrepository = customerCustomerRoleMappingrepository;
            _staticCacheManager = staticCacheManager;
            _orderItemrepository = orderItemrepository;
            _payoutRequestrepository = payoutRequestrepository;
            _orderPayoutRequestrepository = orderPayoutRequestrepository;
            _proInvoicerepository = proInvoicerepository;
            _invoicerepository = invoicerepository;

        }

        #endregion

        #region Methods

        #region Utilities

        private IQueryable<ReportingContactsOrganizationDTO> GetAllOrganizations(DateTime? createdFrom, DateTime? createdTo)
        {
            var query = _organizationProfileRepository.Table
                .Join(_businessSegmentRepository.Table,
                x => x.SegmentId,
                y => y.Id,
                (x, y) => new { x, y }
                )
                .Select(x => new ReportingContactsOrganizationDTO
                {
                    Name = x.x.Name,
                    EstablishedDate = x.x.DateEstablished,
                    Address = x.x.Address,
                    ContactPersonName = x.x.ContactPersonName,
                    ContactPersonPosition = x.x.ContactPersonPosition,
                    ContactPersonEmail = x.x.ContactPersonEmail,
                    ContactPersonContact = x.x.ContactPersonContact,
                    OrganizationCreatedOnUtc = x.x.CreatedOnUTC,
                    BusinessSegment = x.y.Name,
                    CompanyRegistrationNo = x.x.RegistrationNo
                });

            if (createdFrom != null)
            {
                query = query.Where(x => x.OrganizationCreatedOnUtc >= createdFrom);
            }
            if (createdTo != null)
            {
                query = query.Where(x => x.OrganizationCreatedOnUtc < createdTo);
            }

            return query.OrderByDescending(x => x.OrganizationCreatedOnUtc);
        }

        private IQueryable<ReportingContactsOrganizationDTO> GetAllRegistrationOnly(DateTime? createdFrom, DateTime? createdTo)
        {
            var query = from c in _customerRepository.Table
                        join ip in _individualProfilerepository.Table on c.Id equals ip.CustomerId into cip

                        from ip in cip.DefaultIfEmpty()
                        where ip == null
                        && c.Username != null
                        select new ReportingContactsOrganizationDTO
                        {
                            IndividualEmail = c.Email,
                            IndividualEmailVerified = c.Active,
                            CreatedOnUtc = c.CreatedOnUtc
                        };

            if (createdFrom != null)
            {
                query = query.Where(x => x.CreatedOnUtc >= createdFrom);
            }
            if (createdTo != null)
            {
                query = query.Where(x => x.CreatedOnUtc < createdTo);
            }

            return query.OrderByDescending(x => x.CreatedOnUtc);
        }

        private IQueryable<ReportingContactsOrganizationDTO> GetAllRegistrationProfile(DateTime? createdFrom, DateTime? createdTo)
        {
            var query = from c in _customerRepository.Table
                        join ip in _individualProfilerepository.Table on c.Id equals ip.CustomerId into cip

                        from ip in cip.DefaultIfEmpty()
                        where ip != null
                        && c.Username != null
                        select new ReportingContactsOrganizationDTO
                        {
                            IndividualEmail = c.Email,
                            IndividualFullName = ip.FullName,
                            IndividualContactNumber = ip.ContactNo,
                            IndividualDOB = ip.DateOfBirth,
                            CreatedOnUtc = c.CreatedOnUtc
                        };

            if (createdFrom != null)
            {
                query = query.Where(x => x.CreatedOnUtc >= createdFrom);
            }
            if (createdTo != null)
            {
                query = query.Where(x => x.CreatedOnUtc < createdTo);
            }


            return query.OrderByDescending(x => x.CreatedOnUtc);
        }

        private IQueryable<ReportingContactsOrganizationDTO> GetAllIndividualServiceJob(DateTime? createdFrom, DateTime? createdTo)
        {
            var query = from c in _customerRepository.Table
                        where c.Username != null

                        join ccrm in _customerCustomerRoleMappingrepository.Table on c.Id equals ccrm.CustomerId
                        join ip in _individualProfilerepository.Table on ccrm.CustomerId equals ip.CustomerId
                        join sp in _serviceProfilerepository.Table on c.Id equals sp.CustomerId into csp

                        from sp in csp.DefaultIfEmpty()
                        join jsp in _jobSeekerProfilerepository.Table on c.Id equals jsp.CustomerId into cjsp

                        from jsp in cjsp.DefaultIfEmpty()

                        group new { CustomerId = c.Id, ServiceProfile = sp } by new { c.Username, ip.FullName, ip.DateOfBirth, ip.ContactNo, c1 = c.CreatedOnUtc, jsp1 = jsp.CreatedOnUTC }
                        into indGroup
                        select new ReportingContactsOrganizationDTO
                        {
                            IndividualUsername = indGroup.Key.Username,
                            IndividualFullName = indGroup.Key.FullName,
                            IndividualContactNumber = indGroup.Key.ContactNo,
                            IndividualDOB = indGroup.Key.DateOfBirth,
                            CreatedOnUtc = indGroup.Key.c1,
                            LatestServiceProfileCreatedOnUTC = indGroup.Max(x => x.ServiceProfile.CreatedOnUTC),
                            JobCVCreatedOnUTC = indGroup.Key.jsp1,
                            ServiceProfileCount = indGroup.Sum(x => x.ServiceProfile.Id != default ? 1 : 0),
                        };

            if (createdFrom != null)
            {
                query = query.Where(x => x.CreatedOnUtc >= createdFrom);
            }
            if (createdTo != null)
            {
                query = query.Where(x => x.CreatedOnUtc < createdTo);
            }

            return query.OrderByDescending(x => x.CreatedOnUtc);
        }

        private IQueryable<ReportingProRevenueListDTO> GetAllProRevenueList(DateTime? createdFrom, DateTime? createdTo)
        {
            var _productTypeOrderItem = new int[]
            {
                (int)ProductType.ApplyJobSubscription,
                (int)ProductType.ViewJobCandidateFullProfileSubscription,
            };

            var _productTypePayoutRequest = new int[]
            {
                (int)ProductType.ShuqOrderPayoutServiceFee,
                (int)ProductType.ConsultationEngagementFee,
                (int)ProductType.ConsultationEngagementMatchingFee,
                (int)ProductType.ServiceEnagegementFee,
                (int)ProductType.ServiceEnagegementMatchingFee,
                (int)ProductType.JobEnagegementFee,
                (int)ProductType.ConsultationEscrowFee,
                (int)ProductType.JobEscrowFee,
                (int)ProductType.ServiceEscrowFee,
            };

            var query1 = from oi in _orderItemrepository.Table where _productTypeOrderItem.Contains(oi.ProductTypeId) 
                        join pi in _proInvoicerepository.Table on oi.InvoiceId equals pi.Id
                        join c in _customerRepository.Table on pi.InvoiceTo equals c.Id
                        join ip in _individualProfilerepository.Table on c.Id equals ip.CustomerId
                        select new ReportingProRevenueListDTO
                        {
                            CreatedDate = oi.CreatedOnUTC,
                            ProductTypeId = oi.ProductTypeId,
                            InvoiceNo = pi.InvoiceNumber,
                            InvoiceTo = ip.FullName,
                            InvoiceAmount = oi.Price,
                            Id = oi.Id
                            //ProductType = ((ProductType)oi.ProductTypeId).GetDescription()
        };

            var query2 = from pr in _payoutRequestrepository.Table where _productTypePayoutRequest.Contains(pr.ProductTypeId)
                         join pi in _proInvoicerepository.Table on pr.InvoiceId equals pi.Id
                         join c in _customerRepository.Table on pi.InvoiceTo equals c.Id
                         join ip in _individualProfilerepository.Table on c.Id equals ip.CustomerId
                         select new ReportingProRevenueListDTO
                         {
                             CreatedDate = pr.CreatedOnUTC,
                             ProductTypeId = pr.ProductTypeId,
                             InvoiceNo = pi.InvoiceNumber,
                             InvoiceTo = ip.FullName,
                             InvoiceAmount = pr.Fee,
                             Id = pr.Id
                             //ProductType = ((ProductType)pr.ProductTypeId).GetDescription()
                         };

            var query3 = query1.Union(query2);

            if (createdFrom != null)
            {
                query3 = query3.Where(x => x.CreatedDate >= createdFrom);
            }
            if (createdTo != null)
            {
                query3 = query3.Where(x => x.CreatedDate < createdTo);
            }

            return query3.OrderByDescending(x => x.CreatedDate);
        }

        private IQueryable<ReportingProRevenueListDTO> GetAllProExpenseList(DateTime? createdFrom, DateTime? createdTo)
        {
            var _productTypePayoutRequest = new int[]
            {
                (int)ProductType.ModeratorFacilitateConsultationFee
            };

            var query = from pr in _payoutRequestrepository.Table where _productTypePayoutRequest.Contains(pr.ProductTypeId)
                         join pi in _proInvoicerepository.Table on pr.InvoiceId equals pi.Id into prpi
                         from pi in prpi.DefaultIfEmpty()
                         join c in _customerRepository.Table on pr.PayoutTo equals c.Id
                         join ip in _individualProfilerepository.Table on c.Id equals ip.CustomerId
                         select new ReportingProRevenueListDTO
                         {
                             CreatedDate = pr.CreatedOnUTC,
                             ProductTypeId = pr.ProductTypeId,
                             InvoiceNo = pi.InvoiceNumber,
                             InvoiceTo = ip.FullName,
                             InvoiceAmount = pr.Fee,
                             Id = pr.Id,
                             ProductType = ((ProductType)pr.ProductTypeId).GetDescription()
                         };

            if (createdFrom != null)
            {
                query = query.Where(x => x.CreatedDate >= createdFrom);
            }
            if (createdTo != null)
            {
                query = query.Where(x => x.CreatedDate < createdTo);
            }

            return query.OrderByDescending(x => x.Id);
        }

        private IQueryable<ReportingProRevenueListDTO> GetAllShuqRevenueList(DateTime? createdFrom, DateTime? createdTo)
        {
                var query = from opr in _orderPayoutRequestrepository.Table
                        join i in _invoicerepository.Table on opr.InvoiceId equals i.Id into opri
                        from i in opri.DefaultIfEmpty()
                        join c in _customerRepository.Table on opr.CustomerId equals c.Id
                        join ip in _individualProfilerepository.Table on c.Id equals ip.CustomerId
                        select new ReportingProRevenueListDTO
                        {
                            CreatedDate = opr.CreatedOnUTC,
                            InvoiceNo = i.InvoiceNumber,
                            InvoiceTo = ip.FullName,
                            InvoiceAmount = opr.ServiceCharge != null ? opr.ServiceCharge.Value : 0,
                            Id = opr.Id,
                        };

            if (createdFrom != null)
            {
                query = query.Where(x => x.CreatedDate >= createdFrom);
            }
            if (createdTo != null)
            {
                query = query.Where(x => x.CreatedDate < createdTo);
            }

            return query.OrderByDescending(x => x.Id);
        }

        #endregion

        public IPagedList<ReportingContactsOrganizationDTO> SearchContactsOrganization(DateTime? createdFrom = null, DateTime? createdTo = null,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllOrganizations(createdFrom, createdTo);
            return new PagedList<ReportingContactsOrganizationDTO>(query, pageIndex, pageSize);
        }

        public IPagedList<ReportingContactsOrganizationDTO> SearchContactsRegistrationOnly(DateTime? createdFrom, DateTime? createdTo,
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllRegistrationOnly(createdFrom, createdTo);
            return new PagedList<ReportingContactsOrganizationDTO>(query, pageIndex, pageSize);
        }

        public IPagedList<ReportingContactsOrganizationDTO> SearchContactsRegistrationProfile(DateTime? createdFrom, DateTime? createdTo, 
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllRegistrationProfile(createdFrom, createdTo);
            return new PagedList<ReportingContactsOrganizationDTO>(query, pageIndex, pageSize);
        }

        public IPagedList<ReportingContactsOrganizationDTO> SearchContactsIndividualServiceJob(DateTime? createdFrom, DateTime? createdTo, 
            int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllIndividualServiceJob(createdFrom, createdTo);
            return new PagedList<ReportingContactsOrganizationDTO>(query, pageIndex, pageSize);
        }

        public IPagedList<ReportingProRevenueListDTO> SearchProRevenueList(DateTime? createdFrom = null, DateTime? createdTo = null, int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllProRevenueList(createdFrom, createdTo);
            return new PagedList<ReportingProRevenueListDTO>(query, pageIndex, pageSize);
        }

        public IPagedList<ReportingProRevenueListDTO> SearchProExpenseList(DateTime? createdFrom = null, DateTime? createdTo = null, int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllProExpenseList(createdFrom, createdTo);
            return new PagedList<ReportingProRevenueListDTO>(query, pageIndex, pageSize);
        }
        public IPagedList<ReportingProRevenueListDTO> SearchShuqRevenueList(DateTime? createdFrom = null, DateTime? createdTo = null, int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = GetAllShuqRevenueList(createdFrom, createdTo);
            return new PagedList<ReportingProRevenueListDTO>(query, pageIndex, pageSize);
        }

        public IList<ReportingContactsOrganizationDTO> GetAllOrganizationsExport(DateTime? createdFrom, DateTime? createdTo)
        {
            return GetAllOrganizations(createdFrom, createdTo).ToList();
        }

        public IList<ReportingContactsOrganizationDTO> GetAllRegistrationOnlyExport(DateTime? createdFrom, DateTime? createdTo)
        {
            return GetAllRegistrationOnly(createdFrom, createdTo).ToList();
        }

        public IList<ReportingContactsOrganizationDTO> GetAllRegistrationProfileExport(DateTime? createdFrom, DateTime? createdTo)
        {
            return GetAllRegistrationProfile(createdFrom, createdTo).ToList();
        }

        public IList<ReportingContactsOrganizationDTO> GetAllIndividualServiceJobExport(DateTime? createdFrom, DateTime? createdTo)
        {
            return GetAllIndividualServiceJob(createdFrom, createdTo).ToList();
        }

        public IList<ReportingProRevenueListDTO> GetAllProRevenueExport(DateTime? createdFrom, DateTime? createdTo)
        {
            var exportList =  GetAllProRevenueList(createdFrom, createdTo).ToList();
            foreach (var item in exportList)
            {
                item.ProductType = ((ProductType)item.ProductTypeId).GetDescription();
            }
            return exportList;
        }

        public IList<ReportingProRevenueListDTO> GetAllProExpensesExport(DateTime? createdFrom, DateTime? createdTo)
        {
            return GetAllProExpenseList(createdFrom, createdTo).ToList();
        }

        public IList<ReportingProRevenueListDTO> GetAllShuqRevenueExport(DateTime? createdFrom, DateTime? createdTo)
        {
            return GetAllShuqRevenueList(createdFrom, createdTo).ToList();
        }


        #endregion
    }
}
