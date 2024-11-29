using AutoMapper;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Core.Caching;
using Nop.Services.Caching;

using System;
using System.Collections.Generic;
using System.Linq;

using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Individual;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Order;
using YadiYad.Pro.Services.Services.Base;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Core.Infrastructure.Cache;
using YadiYad.Pro.Services.Engagement;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Refund;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.Services.Attentions;

namespace YadiYad.Pro.Services.Service
{
    public class ServiceApplicationService : BaseService, IEngagementService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ServiceApplication> _serviceApplicationRepository;
        private readonly IRepository<ServiceProfile> _serviceProfileRepository;
        private readonly IRepository<ServiceExpertise> _serviceExpertiseRepository;
        private readonly IRepository<Expertise> _expertiseRepository;
        private readonly IRepository<JobServiceCategory> _jobServiceCategoryRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
        private readonly IRepository<IndividualProfile> _individualeRepository;
        private readonly OrderService _orderService;
        private readonly ExpertiseService _expertiseService;
        private readonly ProEngagementSettings _proEngagementSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<ProOrderItem> _proOrderItemRepository;
        private readonly IRepository<RefundRequest> _refundRequestRepository;
        private readonly IndividualProfileService _individualProfileService;
        private readonly ServiceProfileService _serviceProfileService;
        private readonly IRepository<ProOrder> _proOrderRepository;
        private readonly IndividualAttentionService _individualAttentionService;

        private static string subKeyRequestedOrders = "RequestedOrders";
        private static string subKeyConfirmedOrders = "ConfirmedOrders";
        private static string subKeyReceivedOrders = "ReceivedOrders";
        private static string subKeyHiringOrders = "HiringOrders";

        public EngagementType EngagementType => EngagementType.Service;
        public EngagementPartyTypeInfo EngagementPartyTypeInfo => new EngagementPartyTypeInfo
        {
            Buyer = "Buyer",
            Seller = "Seller",
            Moderator = "Moderator"
        };

        #endregion

        #region Ctor

        public ServiceApplicationService
            (IMapper mapper,
            IRepository<ServiceApplication> serviceApplicationRepository,
            IRepository<ServiceProfile> serviceProfileRepository,
            IRepository<ServiceExpertise> serviceExpertiseRepository,
            IRepository<Expertise> expertiseRepository,
            IRepository<JobServiceCategory> jobServiceCategoryRepository,
            IRepository<City> cityRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            IRepository<IndividualProfile> individualeRepository,
            OrderService orderService,
            ExpertiseService expertiseService,
            ProEngagementSettings proEngagementSettings,
            ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IRepository<ProOrderItem> proOrderItemRepository,
            IRepository<RefundRequest> refundRequestRepository,
            IndividualProfileService individualProfileService,
            ServiceProfileService serviceProfileService,
            IRepository<ProOrder> proOrderRepository,
            IndividualAttentionService individualAttentionService)
        {
            _mapper = mapper;
            _serviceApplicationRepository = serviceApplicationRepository;
            _serviceProfileRepository = serviceProfileRepository;
            _serviceExpertiseRepository = serviceExpertiseRepository;
            _expertiseRepository = expertiseRepository;
            _jobServiceCategoryRepository = jobServiceCategoryRepository;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
            _individualeRepository = individualeRepository;
            _orderService = orderService;
            _expertiseService = expertiseService;
            _proEngagementSettings = proEngagementSettings;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _proOrderItemRepository = proOrderItemRepository;
            _refundRequestRepository = refundRequestRepository;
            _individualProfileService = individualProfileService;
            _serviceProfileService = serviceProfileService;
            _proOrderRepository = proOrderRepository;
            _individualAttentionService = individualAttentionService;
        }

        #endregion

        #region Methods

        public virtual void CreateServiceApplication(int actorId, int customerId, ServiceApplicationDTO dto)
        {
            var profile = _serviceProfileRepository.Table.Where(x => x.Id == dto.ServiceProfileId && !x.Deleted).FirstOrDefault();
            dto.ServiceProfileServiceTypeId = profile.ServiceTypeId;
            dto.ServiceProfileServiceModelId = profile.ServiceModelId;
            dto.ServiceProfileServiceFee = profile.ServiceFee;
            dto.ServiceProfileOnsiteFee = profile.OnsiteFee??0;

            var request = _mapper.Map<ServiceApplication>(dto);
            request.Status = (int)ServiceApplicationStatus.New;
            request.CustomerId = customerId;
            request.CreatedById = actorId;
            request.CreatedOnUTC = DateTime.UtcNow;

            _serviceApplicationRepository.Insert(request);
        }

        public virtual void UpdateServiceApplicationStatus(int id, int actorId, ServiceApplicationStatus status)
        {
            var record = _serviceApplicationRepository.Table.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                throw new ArgumentNullException(nameof(ServiceProfile));

            if (record.CustomerId != actorId)
            {
                record.RequesterIsRead = false;
                record.ProviderIsRead = true;

                _individualAttentionService.ClearIndividualAttentionCache(record.CustomerId);
            }
            else
            {
                record.RequesterIsRead = true;
                record.ProviderIsRead = false;

                var providerCustomerId = _serviceProfileRepository.Table
                    .Where(x => x.Id == record.ServiceProfileId)
                    .Select(x => x.CustomerId)
                    .FirstOrDefault();

                _individualAttentionService.ClearIndividualAttentionCache(providerCustomerId);
            }

            if (status == ServiceApplicationStatus.Accepted)
            {
                var ServiceFeeType = "Percentage";
                var ServiceFeeAmount = (decimal)0.25;
                var monthPayment = (decimal)0;

                //check type
                if (record.ServiceProfileServiceTypeId == (int)ServiceType.Freelancing)
                {
                    //by week * 4 = month
                    monthPayment = record.ServiceProfileServiceFee * record.Required.Value * 4;
                }
                else if (record.ServiceProfileServiceTypeId == (int)ServiceType.PartTime)
                {
                    //by month
                    monthPayment = record.ServiceProfileServiceFee * record.Required.Value;
                }
                else if (record.ServiceProfileServiceTypeId == (int)ServiceType.Consultation || record.ServiceProfileServiceTypeId == (int)ServiceType.ProjectBased)
                {
                    //by session or by project
                    monthPayment = record.ServiceProfileServiceFee;
                }

                //check onsite
                if (record.ServiceProfileServiceModelId == (int)ServiceModel.Onsite || record.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite)
                {
                    monthPayment += record.ServiceProfileOnsiteFee;
                }

                //check escrow
                if (record.IsEscrow)
                {
                    record.CommissionFeeAmount = monthPayment;
                }

                //check charge type 
                if (ServiceFeeType == "Percentage")
                {
                    record.ServiceFeeAmount = monthPayment * ServiceFeeAmount;
                }
                else if (ServiceFeeType == "Amount")
                {
                    record.ServiceFeeAmount = ServiceFeeAmount;
                }

            }
            record.Status = (int)status;
            record.UpdatedById = actorId;
            record.UpdatedOnUTC = DateTime.UtcNow;

            _serviceApplicationRepository.Update(record);
        }

        private IList<RefundRequestDTO> GetRefundRequestsByRefId(int refId)
        {
            var data = new RefundRequestDTO();
            var queryRR = _proOrderItemRepository.Table
                .Where(x => !x.Deleted && x.RefId == refId
                && x.ProductTypeId == (int)ProductType.ServiceEnagegementFee
                || x.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee)
                .Join(
                    _proOrderRepository.Table.Where(x => !x.Deleted),
                    x => x.OrderId,
                    y => y.Id,
                    (x, y) => x
                )
                .Join(
                    _refundRequestRepository.Table.Where(x => !x.Deleted),
                    x => x.Id,
                    y => y.OrderItemId,
                    (x, y) => y
                );

            var totalCount = queryRR.Count();

            var records = queryRR
                .ToList()
                .Select(x => _mapper.Map<RefundRequestDTO>(x))
                .ToList();


            return records;
        }

        private bool CheckIfHasRefund(ServiceApplicationDTO data)
        {
            var refundRequest = GetRefundRequestsByRefId(data.Id).FirstOrDefault();

            if (refundRequest != null)
                return true;
            else
                return false;
        }

        public ServiceApplicationDTO ProcessCancellationCount(ServiceApplicationDTO data)
        {
            var buyer = _individualProfileService.GetIndividualProfileByCustomerId(data.CustomerId);
            var lastServiceApplication = GetLastServiceApplicationByCustomerId(buyer.CustomerId);

            if (lastServiceApplication == null)
                return data;

            if (lastServiceApplication.Status == (int)ServiceApplicationStatus.CancelledBySeller
                && lastServiceApplication.CancelRehire == false
                && lastServiceApplication.RehiredServiceApplicationId == 0)
            {
                data.IsRehire = true;
            }

            return data;
        }

        public bool IsCancelledServiceRefunded(int id)
        {
            var serviceApplication = _serviceApplicationRepository.Table
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (serviceApplication != null)
            {
                var proOrderItemId = _proOrderItemRepository.Table
                    .Where(x => x.ProductTypeId == (int)ProductType.JobEnagegementFee
                        && x.RefId == id
                        && x.Status == (int)ProOrderItemStatus.Paid)
                    .Select(x => x.Id)
                    .FirstOrDefault();

                var raisedRefundRequest = _refundRequestRepository.Table
                                          .Where(x => x.OrderItemId == proOrderItemId)
                                          .FirstOrDefault();

                if (raisedRefundRequest != null)
                    return true;
            }
            return false;
        }

        public void CancelRehire(int serviceApplicationId)
        {
            var query = (from s in _serviceApplicationRepository.Table
                         where s.Id == serviceApplicationId
                         select s).FirstOrDefault();

            query.CancelRehire = true;

            _serviceApplicationRepository.Update(query);
        }
        public void HasCancelledTwice(int serviceApplicationId, int actorId)
        {
            var serviceApplication = _serviceApplicationRepository.Table
               .Where(x => x.Deleted == false
               && x.Id == serviceApplicationId)
               .FirstOrDefault();

            serviceApplication.HasCancelledTwice = true;

            serviceApplication.UpdateAudit(actorId);

            _serviceApplicationRepository.Update(serviceApplication);
        }
        public void SetHiredTime(int serviceApplicationId)
        {
            var query = (from s in _serviceApplicationRepository.Table
                        where s.Id == serviceApplicationId
                        select s).FirstOrDefault();

            query.HiredTime = DateTime.UtcNow;

            _serviceApplicationRepository.Update(query);
        }

        public void RehireServiceApplication(int serviceApplicationId, int customerId)
        {
            var currentServiceApplication = (from s in _serviceApplicationRepository.Table
                                             where s.Id == serviceApplicationId
                                             select s).FirstOrDefault();

            var lastServiceApplication = GetLastServiceApplicationByCustomerId(customerId);

            if (lastServiceApplication == null)
                return;

            if (lastServiceApplication.Status == (int)ServiceApplicationStatus.CancelledBySeller
                && lastServiceApplication.HiredTime != null
                && lastServiceApplication.CancelRehire == false)
            {
                lastServiceApplication.RehiredServiceApplicationId = currentServiceApplication.Id;
                _serviceApplicationRepository.Update(lastServiceApplication);
            }
            else
            {
                var customerProfile = _individualProfileService.GetIndividualProfileByCustomerId(customerId);
                _individualProfileService.ResetNumberOfCancellation(customerId, customerProfile);
            }
        }
        public ServiceApplication GetLastServiceApplicationByCustomerId(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = from s in _serviceApplicationRepository.Table
                        where s.CustomerId == customerId
                        select s;

            var record = query.OrderByDescending(x => x.HiredTime).FirstOrDefault();

            return record;
        }
        public ServiceApplicationDTO GetServiceApplicationById(int id)
        {
            if (id == 0)
                return null;

            var query = from sa in _serviceApplicationRepository.Table
                        .Where(x => x.Id == id && !x.Deleted)
                        from sp in _serviceProfileRepository.Table
                        .Where(sp=> sp.Deleted == false
                        && sp.Id == sa.ServiceProfileId)
                        select new
                        {
                            ServiceApplication = sa,
                            ServiceProfile = sp
                        };

            var record = query.FirstOrDefault();

            if (record is null)
                return null;

            var response = _mapper.Map<ServiceApplicationDTO>(record.ServiceApplication);

            response.ServiceProfile = _mapper.Map<ServiceProfileDTO>(record.ServiceProfile);

            return response;
        }

        public virtual void ReproposeServiceApplication(int id, int actorId, DateTime startDate, int? duration)
        {
            var record = _serviceApplicationRepository.Table.Where(x => x.Id == id && !x.Deleted).FirstOrDefault();

            if (record is null)
                throw new ArgumentNullException(nameof(ServiceProfile));

            record.RequesterIsRead = false;
            record.ProviderIsRead = true;

            record.StartDate = startDate;
            record.Duration = duration;
            record.Status = (int)ServiceApplicationStatus.Reproposed;
            record.UpdatedById = actorId;
            record.UpdatedOnUTC = DateTime.UtcNow;

            _serviceApplicationRepository.Update(record);

            _individualAttentionService.ClearIndividualAttentionCache(record.CustomerId);
        }

        public IPagedList<ServiceApplicationDTO> SearchServiceApplications(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            ServiceApplicationSearchFilterDTO filterDTO = null)
        {
            var record = from a in _serviceApplicationRepository.Table
                         .Where(a => !a.Deleted)
                         .OrderByDescending(a => a.CreatedOnUTC)
                         from i in _individualeRepository.Table
                         .Where(i => i.CustomerId == a.CustomerId
                         && i.Deleted == false)
                         from p in _serviceProfileRepository.Table
                         .Where(p => p.Id == a.ServiceProfileId
                         && p.Deleted == false)
                         from si in _individualeRepository.Table
                        .Where(si => si.CustomerId == p.CustomerId
                         && si.Deleted == false)
                         from c in _cityRepository.Table
                        .Where(c => c.Id == a.CityId)
                        .DefaultIfEmpty()
                         select new ServiceApplicationDTO
                         {
                             Id = a.Id,
                             CustomerId = a.CustomerId,
                             ServiceProfileId = a.ServiceProfileId,
                             StartDate = a.StartDate,
                             DaysAbleToCancel = _proEngagementSettings.DaysAbleToCancel,
                             CancellationDateTime = a.CancellationDateTime,
                             CanCancel = DateTime.UtcNow <= (DateTime?)a.StartDate.AddDays(_proEngagementSettings.DaysAbleToCancel) && a.Status != 7 && a.Status != 8 ? true : false,
                             EndDate = a.EndDate,
                             Duration = a.Duration,
                             Location = a.Location,
                             Required = a.Required,
                             RehiredServiceApplicationId = a.RehiredServiceApplicationId,
                             HiredTime = a.HiredTime,
                             HasCancelledTwice = a.HasCancelledTwice,
                             CancelRehire = a.CancelRehire,
                             Status = a.Status,
                             ServiceFeeAmount = a.ServiceFeeAmount,
                             CommissionFeeAmount = a.CommissionFeeAmount,
                             TotalAmount = a.ServiceFeeAmount + a.CommissionFeeAmount,
                             IsEscrow = a.IsEscrow,
                             ProviderIsRead = a.ProviderIsRead,
                             RequesterIsRead = a.RequesterIsRead,
                             CreatedOnUTC = a.CreatedOnUTC,
                             UpdatedOnUTC = a.UpdatedOnUTC,
                             ServiceProfileServiceTypeId = a.ServiceProfileServiceTypeId,
                             ServiceProfileServiceModelId = a.ServiceProfileServiceModelId,
                             ServiceProfileServiceFee = a.ServiceProfileServiceFee,
                             ServiceProfileOnsiteFee = a.ServiceProfileOnsiteFee,
                             BuyerName = i.FullName,
                             BuyerContactNo = i.ContactNo,
                             BuyerEmail = i.Email,
                             SellerName = si.FullName,
                             SellerContactNo = si.ContactNo,
                             SellerEmail = si.Email,
                             ServiceProfileServiceTypeName =
                                              a.ServiceProfileServiceTypeId == (int)ServiceType.Freelancing
                                              ? ServiceType.Freelancing.GetDescription()
                                              : a.ServiceProfileServiceTypeId == (int)ServiceType.PartTime
                                              ? ServiceType.PartTime.GetDescription()
                                              : a.ServiceProfileServiceTypeId == (int)ServiceType.Consultation
                                              ? ServiceType.Consultation.GetDescription()
                                              : a.ServiceProfileServiceTypeId == (int)ServiceType.ProjectBased
                                              ? ServiceType.Consultation.GetDescription()
                                              : null,
                             ServiceProfileServiceModelName =
                                              a.ServiceProfileServiceModelId == (int)ServiceModel.Onsite
                                              ? ServiceModel.Onsite.GetDescription()
                                              : a.ServiceProfileServiceModelId == (int)ServiceModel.PartialOnsite
                                              ? ServiceModel.PartialOnsite.GetDescription()
                                              : a.ServiceProfileServiceModelId == (int)ServiceModel.Remote
                                              ? ServiceModel.Remote.GetDescription()
                                              : null,
                             ServiceProfile = new ServiceProfileDTO
                             {
                                 Id = p.Id,
                                 CustomerId = p.CustomerId,
                                 CityId = p.CityId,
                                 StateProvinceId = p.StateProvinceId,
                                 CountryId = p.CountryId,
                                 ExperienceYearName = ((ExperienceYear)p.YearExperience).GetDescription()
                             },
                             CityId = a.CityId,
                             CityName = c != null ? c.Name : null,
                             ZipPostalCode = a.ZipPostalCode,
                             CancellationEndRemarks = a.CancellationRemarks
                         };

            //if (string.IsNullOrWhiteSpace(keyword) == false)
            //{
            //    record = record
            //        .Where(x => x.CategoryName.ToLower().Contains(keyword)
            //}

            if (filterDTO != null)
            {
                if (filterDTO.RequesterCustomerId != 0)
                {
                    record = record.Where(x => x.CustomerId == filterDTO.RequesterCustomerId);

                }
                if (filterDTO.ProviderCustomerId != 0)
                {
                    record = record.Where(x => x.ServiceProfile.CustomerId == filterDTO.ProviderCustomerId);
                }
                if (filterDTO.Status.Count > 0)
                {
                    record = record.Where(x => filterDTO.Status.Contains(x.Status));
                }
                if (filterDTO.StartDate.HasValue)
                {
                    record = record.Where(x => x.CreatedOnUTC >= filterDTO.StartDate);
                }

                if (filterDTO.EndDate.HasValue)
                {
                    record = record.Where(x => x.CreatedOnUTC <= filterDTO.EndDate);
                }
            }

            var response = new PagedList<ServiceApplicationDTO>(record, pageIndex, pageSize);

            var serviceProfileIds = response.Select(x => x.ServiceProfileId).ToList();

            //get category
            var serviceProfileCategories =
                (from se in _serviceExpertiseRepository.Table
                .Where(se => se.Deleted == false
                && serviceProfileIds.Contains(se.ServiceProfileId))
                 from e in _expertiseRepository.Table
                 .Where(e => e.Id == se.ExpertiseId)
                 from jsc in _jobServiceCategoryRepository.Table
                 .Where(jsc => jsc.Id == e.JobServiceCategoryId)
                 select new
                 {
                     CategoryId = jsc.Id,
                     CategoryName = jsc.Name,
                     se.ServiceProfileId
                 }).GroupBy(x => new
                 {
                     x.CategoryId,
                     x.CategoryName,
                     x.ServiceProfileId
                 })
                .Select(x => new
                {
                    x.Key.CategoryId,
                    x.Key.CategoryName,
                    x.Key.ServiceProfileId
                })
                .ToList();

            //get refundable orderItem
            var cancelledServiceEngagementIds = response
                .Where(x => x.Status == (int)ServiceApplicationStatus.CancelledBySeller)
                .Select(x => x.Id)
                .ToList();

            var refundableOrderItems = _orderService.GetRefundableOrderItems(ProductType.ServiceEnagegementFee, cancelledServiceEngagementIds);

            foreach (var sa in response)
            {
                var category = serviceProfileCategories.Where(x => x.ServiceProfileId == sa.ServiceProfileId).First();

                sa.ServiceProfile.CategoryId = category.CategoryId;
                sa.ServiceProfile.CategoryName = category.CategoryName;

                sa.ServiceProfile.ServiceExpertises = _expertiseService.GetExpertisesByServiceProfileId(sa.ServiceProfileId);

                if (sa.ServiceProfile.CountryId != null)
                {
                    var countryRecord = _countryRepository.Table.Where(x => x.Id == sa.ServiceProfile.CountryId).FirstOrDefault();
                    sa.ServiceProfile.CountryName = countryRecord.Name;

                }

                if (sa.ServiceProfile.StateProvinceId != null)
                {
                    var stateProvinceRecord = _stateProvinceRepository.Table.Where(x => x.Id == sa.ServiceProfile.StateProvinceId).FirstOrDefault();
                    sa.ServiceProfile.StateProvinceName = stateProvinceRecord.Name;

                }

                if (sa.ServiceProfile.CityId != null)
                {
                    var cityRecord = _cityRepository.Table.Where(x => x.Id == sa.ServiceProfile.CityId).FirstOrDefault();
                    sa.ServiceProfile.CityName = cityRecord.Name;

                }

                sa.CanRefund = refundableOrderItems.Any(x => x.RefId == sa.Id);
            }

            return response;
        }


        public ServiceBuyerItemCounterDTO GetServiceBuyerItemCounter(int customerId)
        {
            var dto = new ServiceBuyerItemCounterDTO();
            //dto.NoRequestedOrders =
            //    (from sa in _serviceApplicationRepository.Table
            //    .Where(sa => sa.Deleted == false
            //    && sa.CustomerId == customerId
            //    && (sa.Status == (int)ServiceApplicationStatus.New
            //        || (sa.Status == (int)ServiceApplicationStatus.Reproposed
            //            || sa.Status == (int)ServiceApplicationStatus.Accepted
            //            || (sa.Status == (int)ServiceApplicationStatus.Rejected
            //                && sa.RequesterIsRead == false)))
            //    )
            //     select sa.Id)
            //    .Count();

            var qNoRequestedOrders = (from sa in _serviceApplicationRepository.Table
                                        .Where(sa => sa.Deleted == false
                                        && sa.CustomerId == customerId
                                        && (sa.Status == (int)ServiceApplicationStatus.New
                                            || (sa.Status == (int)ServiceApplicationStatus.Reproposed
                                                || sa.Status == (int)ServiceApplicationStatus.Accepted
                                                || sa.Status == (int)ServiceApplicationStatus.Rejected
                                                ))
                                        )
                                        select sa.Id);

            var keyNoRequestedOrders = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.IndividualServiceBuyerCounter, customerId, subKeyRequestedOrders);

            //dto.NoRequestedOrders = _staticCacheManager.Get(keyNoRequestedOrders, () => qNoRequestedOrders.Count());
            dto.NoRequestedOrders = qNoRequestedOrders.Count();


            //dto.NoComfirmedOrders =
            //    (from sa in _serviceApplicationRepository.Table
            //    .Where(sa => sa.Deleted == false
            //    && sa.CustomerId == customerId
            //    && (sa.Status == (int)ServiceApplicationStatus.Paid))
            //     select sa.Id)
            //    .Count();

            var qNoConfirmedOrders = (from sa in _serviceApplicationRepository.Table
                                    .Where(sa => sa.Deleted == false
                                    && sa.CustomerId == customerId
                                    && (sa.Status == (int)ServiceApplicationStatus.Paid
                                        || sa.Status == (int)ServiceApplicationStatus.Completed
                                        || sa.Status == (int)ServiceApplicationStatus.CancelledByBuyer
                                        || sa.Status == (int)ServiceApplicationStatus.CancelledBySeller))
                                      select sa.Id);

            var keyNoConfirmedOrders = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.IndividualServiceBuyerCounter, customerId, subKeyConfirmedOrders);

            //dto.NoComfirmedOrders = _staticCacheManager.Get(keyNoConfirmedOrders, () => qNoConfirmedOrders.Count());
            dto.NoComfirmedOrders = qNoConfirmedOrders.Count();

            return dto;
        }

        public ServiceSellerItemCounterDTO GetServiceSellerItemCounter(int customerId)
        {
            var dto = new ServiceSellerItemCounterDTO();
            //dto.NoReceivedRequests =
            //    (from sa in _serviceApplicationRepository.Table
            //    .Where(sa => sa.Deleted == false
            //    && (sa.Status == (int)ServiceApplicationStatus.New))
            //     from sp in _serviceProfileRepository.Table
            //     .Where(sp => sp.Deleted == false
            //         && sp.CustomerId == customerId
            //         && sp.Id == sa.ServiceProfileId)
            //     select sa.Id)
            //    .Count();

            var qNoReceivedRequests = (from sa in _serviceApplicationRepository.Table
                                        .Where(sa => sa.Deleted == false
                                        && (sa.Status == (int)ServiceApplicationStatus.New
                                            || sa.Status == (int)ServiceApplicationStatus.Reproposed
                                            || sa.Status == (int)ServiceApplicationStatus.Accepted
                                            || sa.Status == (int)ServiceApplicationStatus.Rejected
                                            ))
                                       from sp in _serviceProfileRepository.Table
                                       .Where(sp => sp.Deleted == false
                                           && sp.CustomerId == customerId
                                           && sp.Id == sa.ServiceProfileId)
                                       select sa.Id);

            var keyNoReceivedRequests = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.IndividualServiceSellerCounter, customerId, subKeyReceivedOrders);

            //dto.NoReceivedRequests = _staticCacheManager.Get(keyNoReceivedRequests, () => qNoReceivedRequests.Count());
            dto.NoReceivedRequests = qNoReceivedRequests.Count();

            //dto.NoHiringRequests =
            //    (from sa in _serviceApplicationRepository.Table
            //    .Where(sa => sa.Deleted == false
            //    && (sa.Status == (int)ServiceApplicationStatus.Paid))
            //     from sp in _serviceProfileRepository.Table
            //     .Where(sp => sp.Deleted == false
            //         && sp.CustomerId == customerId
            //        && sp.Id == sa.ServiceProfileId)
            //     select sa.Id)
            //    .Count();

            var qNoHiringRequests = (from sa in _serviceApplicationRepository.Table
                                    .Where(sa => sa.Deleted == false
                                        && (sa.Status == (int)ServiceApplicationStatus.Paid
                                            || sa.Status == (int)ServiceApplicationStatus.Completed
                                            || sa.Status == (int)ServiceApplicationStatus.CancelledByBuyer
                                            || sa.Status == (int)ServiceApplicationStatus.CancelledBySeller
                                        ))
                                     from sp in _serviceProfileRepository.Table
                                     .Where(sp => sp.Deleted == false
                                         && sp.CustomerId == customerId
                                        && sp.Id == sa.ServiceProfileId)
                                     select sa.Id);

            var keyNoHiringRequests = _cacheKeyService.PrepareKeyForShortTermCache(NopModelCacheDefaults.IndividualServiceSellerCounter, customerId, subKeyHiringOrders);

            //dto.NoHiringRequests = _staticCacheManager.Get(keyNoHiringRequests, () => qNoHiringRequests.Count());
            dto.NoHiringRequests = qNoHiringRequests.Count();

            return dto;
        }

        public void UpdateServiceBuyerRead(int serviceApplicationId, int actorId)
        {
            var serviceApplication = _serviceApplicationRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == serviceApplicationId
                && x.CustomerId == actorId)
                .FirstOrDefault();

            if (serviceApplication != null)
            {
                _individualAttentionService.ClearIndividualAttentionCache(actorId);

                serviceApplication.RequesterIsRead = true;

                //serviceApplication.UpdateAudit(actorId);

                _serviceApplicationRepository.Update(serviceApplication);
            }
        }

        public void UpdateServiceSellerRead(int serviceApplicationId, int actorId)
        {
            var serviceApplication =
                (from sa in _serviceApplicationRepository.Table
                .Where(sa => sa.Deleted == false
                && sa.Id == serviceApplicationId)
                 from sp in _serviceProfileRepository.Table
                 .Where(sp => sp.Deleted == false
                 && sp.CustomerId == actorId
                 && sp.Id == sa.ServiceProfileId)
                 select sa)
                .FirstOrDefault();

            if (serviceApplication != null)
            {
                _individualAttentionService.ClearIndividualAttentionCache(actorId);

                serviceApplication.ProviderIsRead = true;

                //serviceApplication.UpdateAudit(actorId);

                _serviceApplicationRepository.Update(serviceApplication);
            }
        }
        public bool UpdateServiceApplicationEndDate(
            int actorId,
            int id,
            UpdateServiceApplicationEndDateDTO request)
        {
            var result = false;

            var serviceApplication =
                    (from sa in _serviceApplicationRepository.Table
                        .Where(sa => sa.Deleted == false
                        && sa.Id == id
                        && (sa.Status == (int)ServiceApplicationStatus.Accepted
                            || sa.Status == (int)ServiceApplicationStatus.Paid)
                        && sa.CustomerId == actorId
                            )
                     select sa)
                    .FirstOrDefault();

            if (serviceApplication == null)
            {
                throw new InvalidOperationException("Service application not found.");
            }

            serviceApplication.EndDate = request.EndDate;
            serviceApplication.CancellationRemarks = request.Remarks;
            serviceApplication.UpdateAudit(actorId);

            _serviceApplicationRepository.Update(serviceApplication);

            result = true;

            return result;
        }

        public EngagementPartyInfo GetEngagingParties(int serviceApplicationId)
        {
            return QueryableEngagementParties().Where(s => s.EngagementId == serviceApplicationId).FirstOrDefault();
        }

        public bool Cancel(int serviceApplicationId, EngagementParty cancellingParty, int actorId)
        {
            var serviceApplication = _serviceApplicationRepository.Table
                                        .Where(q => q.Id == serviceApplicationId)
                                        .FirstOrDefault();
            //validate before cancel
            if(serviceApplication.Status != (int)ServiceApplicationStatus.Paid)
                return false;

            if (cancellingParty == EngagementParty.Buyer)
                serviceApplication.Status = (int)ServiceApplicationStatus.CancelledByBuyer;
            else
                serviceApplication.Status = (int)ServiceApplicationStatus.CancelledBySeller;

            serviceApplication.UpdateAudit(actorId);

            _serviceApplicationRepository.Update(serviceApplication);

            return true;
        }


        public void UpdateCancel(
            int serviceApplicationId, 
            DateTime submissionTime, 
            string userRemarks, 
            int reasonId, 
            int? attachmentId,
            EngagementParty cancellationParty)
        {
            var serviceApplication = _serviceApplicationRepository.Table
                                        .Where(q => q.Id == serviceApplicationId)
                                        .FirstOrDefault();

            if (cancellationParty != EngagementParty.Buyer)
            {
                serviceApplication.RequesterIsRead = false;
            }
            if (cancellationParty != EngagementParty.Seller)
            {
                serviceApplication.ProviderIsRead = false;
            }

            serviceApplication.CancellationReasonId = reasonId;
            serviceApplication.CancellationRemarks = userRemarks;
            serviceApplication.CancellationDownloadId = attachmentId;
            serviceApplication.CancellationDateTime = submissionTime;

            _serviceApplicationRepository.Update(serviceApplication);
        }

        public IQueryable<EngagementPartyInfo> QueryableEngagementParties()
        {
            return from sa in _serviceApplicationRepository.Table
                   join sp in _serviceProfileRepository.Table on sa.ServiceProfileId equals sp.Id
                   join cb in _individualeRepository.Table on sa.CustomerId equals cb.CustomerId
                   join cs in _individualeRepository.Table on sp.CustomerId equals cs.CustomerId
                   select new EngagementPartyInfo
                   {
                       EngagementId = sa.Id,
                       EngagementType = EngagementType,
                       IsEscrow = sa.IsEscrow,
                       BuyerId = cb.CustomerId,
                       BuyerName = cb.FullName,
                       SellerId = cs.CustomerId,
                       SellerName = cs.FullName,
                   };
        }

        public DateTime? GetStartDate(int serviceApplicationId)
        {
            return (from sa in _serviceApplicationRepository.Table
                    join sp in _serviceProfileRepository.Table on sa.ServiceProfileId equals sp.Id
                    select sp.TenureStart).FirstOrDefault();
        }

        #endregion
    }
}