using AutoMapper;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Payout;
using Nop.Core.Domain.Tax;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.DepositRequest;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Payout;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Individual;
using YadiYad.Pro.Services.Organization;
using YadiYad.Pro.Services.Payout;

namespace YadiYad.Pro.Services.Services.Order
{
    public class StatementService
    {
        #region Fields
        private readonly TaxSettings _taxSettings;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IAddressService _addressService;
        private readonly IRepository<ProInvoice> _proInvoiceRepository;
        private readonly IRepository<ProOrderItem> _proOrderItemRepository;
        private readonly IRepository<RefundRequest> _refundRequestRepository;
        private readonly IRepository<DepositRequest> _depositRequestRepository;
        private readonly IRepository<ProOrder> _proOrderRepository;
        private readonly ICustomerService _customerService;
        private readonly OrganizationProfileService _organizationProfileService;
        private readonly IndividualProfileService _individualProfileService;
        private readonly PayoutRequestService _payoutRequestService;

        #endregion

        #region Ctor

        public StatementService
            (
            TaxSettings taxSettings,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IAddressService addressService,
            IRepository<ProInvoice> proInvoiceRepository,
            IRepository<ProOrderItem> proOrderItemRepository,
            IRepository<RefundRequest> refundRequestRepository,
            IRepository<DepositRequest> depositRequestRepository,
            IRepository<ProOrder> proOrderRepository,
            ICustomerService customerService,
            OrganizationProfileService organizationProfileService,
            IndividualProfileService individualProfileService,
            PayoutRequestService payoutRequestService)
        {
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxSettings = taxSettings;
            _addressService = addressService;
            _refundRequestRepository = refundRequestRepository;
            _depositRequestRepository = depositRequestRepository;
            _proInvoiceRepository = proInvoiceRepository;
            _proOrderItemRepository = proOrderItemRepository;
            _customerService = customerService;
            _organizationProfileService = organizationProfileService;
            _individualProfileService = individualProfileService;
            _payoutRequestService = payoutRequestService;
            _proOrderRepository = proOrderRepository;
        }

        #endregion

        public StatementDTO GetInvoiceStatement(int invoiceId)
        {
            if (invoiceId == 0)
                return null;
            
            var dto = _proInvoiceRepository.Table.Where(x => x.Id == invoiceId && x.Deleted == false)
                    .Select(x => new StatementDTO
                    {
                        Id = x.Id,
                        StatementNumber = x.InvoiceNumber,
                        StatementType = (int)StatementType.Invoice,
                        RefType = x.RefType,
                        StatementTo = x.InvoiceTo,
                        StatementFrom = x.InvoiceFrom,
                        CreatedOnUTC = x.CreatedOnUTC,
                        StatementItems = new List<StatementItemDTO>()
                    })
                    .FirstOrDefault();

            if (dto != null)
            {
                var toCustomer = _customerService.GetCustomerById(dto.StatementTo);
                var toRoles = _customerService.GetCustomerRoles(toCustomer);
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                {
                    var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(toCustomer.Id);
                    if (individualProfile != null)
                    {
                        dto.StatementToName = individualProfile.FullName;
                        dto.StatementToAddress1 = individualProfile.Address;
                        dto.StatementToState = individualProfile.StateProvinceName;
                        dto.StatementToCountry = individualProfile.CountryName;
                    }
                }
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                {
                    var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(toCustomer.Id);
                    if (organizationProfile != null)
                    {
                        dto.StatementToName = organizationProfile.ContactPersonName;
                        dto.StatementToBusinessName = organizationProfile.Name;
                        dto.StatementToAddress1 = organizationProfile.Address;
                        dto.StatementToState = organizationProfile.StateProvinceName;
                        dto.StatementToCountry = organizationProfile.CountryName;
                    }
                }

                if (dto.StatementFrom != null)
                {
                    var fromCustomer = _customerService.GetCustomerById(dto.StatementFrom.Value);
                    var fromRoles = _customerService.GetCustomerRoles(fromCustomer);
                    if (fromRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                    {
                        var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(fromCustomer.Id);
                        if (individualProfile != null)
                        {
                            dto.StatementFromName = individualProfile.FullName;
                            dto.StatementFromAddress1 = individualProfile.Address;
                            dto.StatementFromState = individualProfile.StateProvinceName;
                            dto.StatementFromCountry = individualProfile.CountryName;
                            dto.SSTRegNo = individualProfile.SSTRegNo;
                        }
                    }
                    if (fromRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                    {
                        var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(fromCustomer.Id);
                        if (organizationProfile != null)
                        {
                            dto.StatementFromName = organizationProfile.ContactPersonName;
                            dto.StatementFromBusinessName = organizationProfile.Name;
                            dto.StatementFromAddress1 = organizationProfile.Address;
                            dto.StatementFromState = organizationProfile.StateProvinceName;
                            dto.StatementFromCountry = organizationProfile.CountryName;
                        }
                    }
                }
                else
                {
                    SetDefaultAddress(dto);
                }

                if (dto.RefType == (int)InvoiceRefType.Order)
                {
                    var orderItems = 
                        (from poi in _proOrderItemRepository.Table
                        .Where(x => x.InvoiceId == dto.Id && x.Deleted == false)
                        from po in _proOrderRepository.Table
                        .Where(po => po.Deleted == false
                        && po.Id == poi.OrderId
                        && po.OrderStatusId == (int)OrderStatus.Complete
                        && po.PaymentStatusId == (int)PaymentStatus.Paid)
                        select new
                        {
                            poi,
                            po
                        })
                        .ToList()
                        .Select(x => {

                            var serviceChargeDTO = new ServiceChargeDTO
                            {
                                Value = 0,
                                ChargeValueType = (int)ChargeValueType.Amount
                            };

                            var engagementFee = x.poi.Price;

                            if (string.IsNullOrWhiteSpace(x.po.MoreInfo) == false
                            && (x.poi.ProductTypeId == (int)ProductType.ViewJobCandidateFullProfileSubscription
                                || x.poi.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee
                                || x.poi.ProductTypeId == (int)ProductType.ConsultationEngagementMatchingFee))
                            {
                                serviceChargeDTO = JsonConvert.DeserializeObject<ServiceChargeDTO>(x.po.MoreInfo);

                                if(serviceChargeDTO.ChargeValueType == (int)ChargeValueType.Rate)
                                {
                                    engagementFee = engagementFee / serviceChargeDTO.Value;
                                }
                            }

                            return new StatementItemDTO
                            {
                                ItemName = GetItemDesc(
                                     (ProductType)x.poi.ProductTypeId,
                                     (ProductType)x.poi.ProductTypeId,
                                     x.poi.RefId,
                                     null,
                                     dto.StatementFromName,
                                     engagementFee,
                                     (ChargeValueType)serviceChargeDTO.ChargeValueType,
                                     serviceChargeDTO.Value),
                                Quantity = x.poi.Quantity,
                                UnitPrice = x.poi.UnitPrice,
                                Tax = x.poi.Tax,
                                Price = x.poi.Price
                            };
                        }).ToList();

                    if (orderItems != null)
                    {
                        dto.StatementItems = orderItems;
                    }
                }
                else if (dto.RefType == (int)InvoiceRefType.Payout)
                {
                    var payoutRequest = _payoutRequestService.GetPayoutRequest(null, null, dto.Id);

                    if (payoutRequest.Status == (int)PayoutRequestStatus.Paid)
                    {
                        var engagementFee = payoutRequest.Fee + payoutRequest.ServiceCharge;

                        var statementItem = new StatementItemDTO
                        {
                            ItemName = GetItemDesc(
                                (ProductType)payoutRequest.ProductTypeId,
                                (ProductType)payoutRequest.ProductTypeId,
                                payoutRequest.RefId,
                                null,
                                dto.StatementToName,
                                engagementFee,
                                (ChargeValueType)payoutRequest.ServiceChargeType,
                                payoutRequest.ServiceChargeRate),
                            Quantity = 1,
                            UnitPrice = engagementFee,
                            Tax = 0,
                            Price = engagementFee
                        };

                        if (statementItem != null)
                        {
                            dto.StatementItems.Add(statementItem);
                        }
                    }
                }

                dto.SubTotal = dto.StatementItems.Sum(x => x.Price);
                dto.Tax = dto.StatementItems.Sum(x => x.Tax);
                dto.GrandTotal = dto.SubTotal + dto.Tax;
            }

            if(dto.StatementItems == null
                || dto.StatementItems.Count == 0)
            {
                return null;
            }

            return dto;
        }

        public StatementDTO GetEscrowProcessingInvoiceStatement(int invoiceId)
        {
            if (invoiceId == 0)
                return null;

            var dto = _proInvoiceRepository.Table.Where(x => x.Id == invoiceId && x.Deleted == false)
                    .Select(x => new StatementDTO
                    {
                        Id = x.Id,
                        StatementNumber = x.InvoiceNumber,
                        StatementType = (int)StatementType.Invoice,
                        RefType = x.RefType,
                        StatementTo = x.InvoiceTo,
                        StatementFrom = x.InvoiceFrom,
                        CreatedOnUTC = x.CreatedOnUTC,
                        StatementItems = new List<StatementItemDTO>()
                    })
                    .FirstOrDefault();

            if(dto.StatementFrom != null)
            {
                return null;
            }

            if (dto != null)
            {
                //set from address
                SetDefaultAddress(dto);

                var toCustomer = _customerService.GetCustomerById(dto.StatementTo);
                var toRoles = _customerService.GetCustomerRoles(toCustomer);
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                {
                    var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(toCustomer.Id);
                    if (individualProfile != null)
                    {
                        dto.StatementToName = individualProfile.FullName;
                        dto.StatementToAddress1 = individualProfile.Address;
                        dto.StatementToState = individualProfile.StateProvinceName;
                        dto.StatementToCountry = individualProfile.CountryName;
                    }
                }
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                {
                    var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(toCustomer.Id);
                    if (organizationProfile != null)
                    {
                        dto.StatementToName = organizationProfile.ContactPersonName;
                        dto.StatementToBusinessName = organizationProfile.Name;
                        dto.StatementToAddress1 = organizationProfile.Address;
                        dto.StatementToState = organizationProfile.StateProvinceName;
                        dto.StatementToCountry = organizationProfile.CountryName;
                    }
                }

                if (dto.RefType == (int)InvoiceRefType.Order)
                {
                    var orderItems =
                        (from poi in _proOrderItemRepository.Table
                        .Where(x => x.InvoiceId == dto.Id && x.Deleted == false)
                         from po in _proOrderRepository.Table
                         .Where(po => po.Deleted == false
                         && po.Id == poi.OrderId
                         && po.OrderStatusId == (int)OrderStatus.Complete
                         && po.PaymentStatusId == (int)PaymentStatus.Paid)
                         select new
                         {
                             poi,
                             po
                         })
                        .Select(x => new StatementItemDTO
                        {
                            ItemName = x.poi.ItemName,
                            Quantity = x.poi.Quantity,
                            UnitPrice = x.poi.UnitPrice,
                            Tax = x.poi.Tax,
                            Price = x.poi.Price
                        }).ToList();

                    if (orderItems != null)
                    {
                        dto.StatementItems = orderItems;
                    }
                }
                else if (dto.RefType == (int)InvoiceRefType.Payout)
                {
                    var payoutRequest = _payoutRequestService.GetPayoutRequest(null, null, dto.Id);

                    if (payoutRequest.Status == (int)PayoutRequestStatus.Paid)
                    {
                        var engagementFee = payoutRequest.Fee + payoutRequest.ServiceCharge;

                        var statementItem = new StatementItemDTO
                        {
                            ItemName = GetItemDesc(
                                    (ProductType)payoutRequest.ProductTypeId == ProductType.ConsultationEngagementFee
                                    ? ProductType.ConsultationEscrowFee
                                    : (ProductType)payoutRequest.ProductTypeId == ProductType.JobEnagegementFee
                                    ? ProductType.JobEscrowFee
                                    : (ProductType)payoutRequest.ProductTypeId == ProductType.ServiceEnagegementFee
                                    ? ProductType.ServiceEscrowFee
                                    : 0,
                                (ProductType)payoutRequest.ProductTypeId,
                                payoutRequest.RefId,
                                null,
                                dto.StatementToName,
                                engagementFee,
                                (ChargeValueType)payoutRequest.ServiceChargeType,
                                payoutRequest.ServiceChargeRate),
                            Quantity = 1,
                            UnitPrice = payoutRequest.ServiceCharge,
                            Tax = 0,
                            Price = payoutRequest.ServiceCharge
                        };

                        if (statementItem != null)
                        {
                            dto.StatementItems.Add(statementItem);
                        }
                    }
                }

                dto.SubTotal = dto.StatementItems.Sum(x => x.Price);
                dto.Tax = dto.StatementItems.Sum(x => x.Tax);
                dto.GrandTotal = dto.SubTotal + dto.Tax;
            }

            if (dto.StatementItems == null
                || dto.StatementItems.Count == 0)
            {
                return null;
            }

            return dto;
        }

        public StatementDTO GetRefundStatement(int refundRequestId)
        {
            if (refundRequestId == 0)
                return null;

            var dto = _refundRequestRepository.Table
                .Where(x => x.Id == refundRequestId && x.Deleted == false
                && x.Status == (int)RefundStatus.Paid)
                    .Select(x => new StatementDTO
                    {
                        Id = x.Id,
                        OrderItemId = x.OrderItemId,
                        StatementNumber = x.RefundNumber,
                        StatementType = (int)StatementType.Refund,
                        StatementTo = x.RefundTo,
                        CreatedOnUTC = x.CreatedOnUTC,
                        SubTotal = x.Amount,
                        Tax = 0,
                        GrandTotal = x.Amount,

                    })
                    .FirstOrDefault();

            if (dto != null)
            {
                var toCustomer = _customerService.GetCustomerById(dto.StatementTo);
                var toRoles = _customerService.GetCustomerRoles(toCustomer);
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                {
                    var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(toCustomer.Id);
                    if (individualProfile != null)
                    {
                        dto.StatementToName = individualProfile.FullName;
                        dto.StatementToAddress1 = individualProfile.Address;
                        dto.StatementToState = individualProfile.StateProvinceName;
                        dto.StatementToCountry = individualProfile.CountryName;
                    }
                }
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                {
                    var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(toCustomer.Id);
                    if (organizationProfile != null)
                    {
                        dto.StatementToName = organizationProfile.ContactPersonName;
                        dto.StatementToBusinessName = organizationProfile.Name;
                        dto.StatementToAddress1 = organizationProfile.Address;
                        dto.StatementToState = organizationProfile.StateProvinceName;
                        dto.StatementToCountry = organizationProfile.CountryName;
                    }
                }

                if (dto.StatementFrom != null)
                {
                    var fromCustomer = _customerService.GetCustomerById(dto.StatementFrom.Value);
                    var fromRoles = _customerService.GetCustomerRoles(fromCustomer);
                    if (fromRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                    {
                        var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(fromCustomer.Id);
                        if (individualProfile != null)
                        {
                            dto.StatementFromName = individualProfile.FullName;
                            dto.StatementFromAddress1 = individualProfile.Address;
                            dto.StatementFromState = individualProfile.StateProvinceName;
                            dto.StatementFromCountry = individualProfile.CountryName;
                            dto.SSTRegNo = individualProfile.SSTRegNo;
                        }
                    }
                    if (fromRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                    {
                        var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(fromCustomer.Id);
                        if (organizationProfile != null)
                        {
                            dto.StatementFromName = organizationProfile.ContactPersonName;
                            dto.StatementFromBusinessName = organizationProfile.Name;
                            dto.StatementFromAddress1 = organizationProfile.Address;
                            dto.StatementFromState = organizationProfile.StateProvinceName;
                            dto.StatementFromCountry = organizationProfile.CountryName;
                        }
                    }
                }
                else
                {
                    SetDefaultAddress(dto);
                }

                var orderItems = _proOrderItemRepository.Table
                    .Where(x => x.Id == dto.OrderItemId 
                    && x.Deleted == false)
                        .ToList();
                var productType = orderItems.FirstOrDefault().ProductTypeId;
                if (productType == (int)ProductType.ApplyJobSubscription || productType == (int)ProductType.ViewJobCandidateFullProfileSubscription)
                {
                    dto.StatementType = (int)StatementType.CreditNote;
                }
                var statementItems = orderItems
                    .Select(x => new StatementItemDTO
                    {
                        ItemName = GetItemDesc(
                            ProductType.RefundProfessionalFeesDeposit,
                            (ProductType)x.ProductTypeId,
                            x.RefId),
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Tax = x.Tax,
                        Price = x.Price
                    }).ToList();

                if (statementItems != null)
                {
                    dto.StatementItems = statementItems;
                }
            }

            return dto;
        }

        public StatementDTO GetDepositRequestStatement(int depositRequestId)
        {
            if (depositRequestId == 0)
                return null;

            var depositRequest = _depositRequestRepository.Table
                    .Where(x => x.Id == depositRequestId && x.Deleted == false
                    && x.Status == (int)DepositRequestStatus.Paid)
                    .FirstOrDefault();

            var dto = new StatementDTO();

            if (depositRequest != null)
            {
                dto = new StatementDTO
                {
                    Id = depositRequest.Id,
                    OrderItemId = depositRequest.OrderItemId,
                    StatementNumber = depositRequest.DepositNumber,
                    StatementType = (int)StatementType.Deposit,
                    StatementTo = depositRequest.DepositTo,
                    StatementFrom = depositRequest.DepositFrom,
                    CreatedOnUTC = depositRequest.CreatedOnUTC,
                    SubTotal = depositRequest.Amount,
                    Tax = 0,
                    GrandTotal = depositRequest.Amount
                };

                var toCustomer = _customerService.GetCustomerById(dto.StatementTo);
                var toRoles = _customerService.GetCustomerRoles(toCustomer);
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                {
                    var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(toCustomer.Id);
                    if (individualProfile != null)
                    {
                        dto.StatementToName = individualProfile.FullName;
                        dto.StatementToAddress1 = individualProfile.Address;
                        dto.StatementToState = individualProfile.StateProvinceName;
                        dto.StatementToCountry = individualProfile.CountryName;
                    }
                }
                if (toRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                {
                    var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(toCustomer.Id);
                    if (organizationProfile != null)
                    {
                        dto.StatementToName = organizationProfile.ContactPersonName;
                        dto.StatementToBusinessName = organizationProfile.Name;
                        dto.StatementToAddress1 = organizationProfile.Address;
                        dto.StatementToState = organizationProfile.StateProvinceName;
                        dto.StatementToCountry = organizationProfile.CountryName;
                    }
                }

                if (dto.StatementFrom != null)
                {
                    var fromCustomer = _customerService.GetCustomerById(dto.StatementFrom.Value);
                    var fromRoles = _customerService.GetCustomerRoles(fromCustomer);
                    if (fromRoles.Any(x => x.SystemName == NopCustomerDefaults.IndividualRoleName))
                    {
                        var individualProfile = _individualProfileService.GetIndividualProfileByCustomerId(fromCustomer.Id);
                        if (individualProfile != null)
                        {
                            dto.StatementFromName = individualProfile.FullName;
                            dto.StatementFromAddress1 = individualProfile.Address;
                            dto.StatementFromState = individualProfile.StateProvinceName;
                            dto.StatementFromCountry = individualProfile.CountryName;
                            dto.SSTRegNo = individualProfile.SSTRegNo;
                        }
                    }
                    if (fromRoles.Any(x => x.SystemName == NopCustomerDefaults.OrganizationRoleName))
                    {
                        var organizationProfile = _organizationProfileService.GetOrganizationProfileByCustomerId(fromCustomer.Id);
                        if (organizationProfile != null)
                        {
                            dto.StatementFromName = organizationProfile.ContactPersonName;
                            dto.StatementFromBusinessName = organizationProfile.Name;
                            dto.StatementFromAddress1 = organizationProfile.Address;
                            dto.StatementFromState = organizationProfile.StateProvinceName;
                            dto.StatementFromCountry = organizationProfile.CountryName;
                        }
                    }
                }
                else
                {
                    SetDefaultAddress(dto);
                }

                var orderItems = _proOrderItemRepository.Table.Where(x => x.Id == dto.OrderItemId && x.Deleted == false)
                    .ToList()
                    .Select(x => new StatementItemDTO
                    {
                        ItemName = GetItemDesc(
                            ProductType.DepositRequest,  
                            (ProductType)depositRequest.ProductTypeId,
                            depositRequest.RefId),
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Tax = x.Tax,
                        Price = x.Price
                    }).ToList();

                if (orderItems != null)
                {
                    dto.StatementItems = orderItems;
                }
            }

            return dto;
        }

        public string GetItemDesc(
            ProductType productType,
            ProductType? engagementType = null,
            int? engagamentId = null,
            string buyerName = null, 
            string sellerName = null, 
            decimal? engagementFee = null,
            ChargeValueType? chargeValueType = null,
            decimal? serviceChargeRate = null)
        {
            var desc = "";
            string engagementCode = null;

            if(engagamentId.HasValue && engagementType != null)
            {
                switch(engagementType.Value)
                {
                    case ProductType.JobEnagegementFee:
                    case ProductType.JobEscrowFee:
                    case ProductType.ViewJobCandidateFullProfileSubscription:
                        {
                            var engagement = new JobApplicationDTO();
                            engagement.Id = engagamentId.Value;

                            engagementCode = engagement.Code;
                        }
                        break;
                    case ProductType.ServiceEnagegementFee:
                    case ProductType.ServiceEscrowFee:
                    case ProductType.ServiceEnagegementMatchingFee:
                        {
                            var engagement = new ServiceApplicationDTO();
                            engagement.Id = engagamentId.Value;

                            engagementCode = engagement.Code;
                        }
                        break;
                    case ProductType.ConsultationEngagementFee:
                    case ProductType.ConsultationEscrowFee:
                    case ProductType.ConsultationEngagementMatchingFee:
                        {
                            var engagement = new ConsultationInvitationDTO();
                            engagement.Id = engagamentId.Value;

                            engagementCode = engagement.Code;
                        }
                        break;
                }
            }

            switch (productType)
            {
                case ProductType.ApplyJobSubscription:
                    desc = $"Pay-to-Apply Jobs (PAJ)";
                    break;
                case ProductType.ViewJobCandidateFullProfileSubscription:
                    desc = $"Pay-to-View-and-Invite (PVI)"
                        + (chargeValueType.Value == ChargeValueType.Rate
                            ? $"\r\nAmount: RM {engagementFee?.ToString("#,##0.00")} x {serviceChargeRate?.ToString("P")} = RM {(engagementFee * serviceChargeRate)?.ToString("#,##0.00")}"
                            : $"\r\nAmount: RM {(serviceChargeRate)?.ToString("#,##0.00")}");
                    break;
                case ProductType.ConsultationEngagementMatchingFee:
                case ProductType.ServiceEnagegementMatchingFee:
                    desc = $"Service Charge"
                        + $"\r\nJob Id: {engagementCode}"
                        + $"\r\nFreelancer: {sellerName}"
                        + (chargeValueType.Value == ChargeValueType.Rate
                            ? $"\r\nAmount: RM {engagementFee?.ToString("#,##0.00")} x {serviceChargeRate?.ToString("P")} = RM {(engagementFee * serviceChargeRate)?.ToString("#,##0.00")}"
                            : $"\r\nAmount: RM {(serviceChargeRate)?.ToString("#,##0.00")}");
                    break;
                case ProductType.ConsultationEscrowFee:
                case ProductType.JobEscrowFee:
                case ProductType.ServiceEscrowFee:
                    desc = $"Escrow Processing Charge"
                        + $"\r\nJob Id: {engagementCode}"
                        + $"\r\nFreelancer: {sellerName}"
                        + (chargeValueType.Value == ChargeValueType.Rate
                            ? $"\r\nAmount: RM {engagementFee?.ToString("#,##0.00")} x {serviceChargeRate?.ToString("P")} = RM {(engagementFee * serviceChargeRate)?.ToString("#,##0.00")}"
                            : $"\r\nAmount: RM {(serviceChargeRate)?.ToString("#,##0.00")}");
                    break;
                case ProductType.JobEnagegementFee:
                case ProductType.ConsultationEngagementFee:
                case ProductType.ServiceEnagegementFee:
                    desc = $"Professional Fees"
                        + $"\r\nJob Id: {engagementCode}";
                    break;
                case ProductType.DepositRequest:
                    desc = $"Refundable deposit for professional fees"
                        + $"\r\nJob Id: {engagementCode}";
                    break;
                case ProductType.RefundProfessionalFeesDeposit:
                    desc = $"Refund professional fees deposit"
                        + $"\r\nJob Id: {engagementCode}";
                    break;
            }


            return desc;
        }

        public void SetDefaultAddress(StatementDTO statementDTO)
        {
            var defaultAddress = _addressService.GetAddressById(_taxSettings.DefaultTaxAddressId);
            if (defaultAddress != null)
            {
                if (defaultAddress.CountryId.HasValue)
                {
                    var country = _countryService.GetCountryById(defaultAddress.CountryId.Value);
                    statementDTO.StatementFromCountry = country.Name;
                }
                if (defaultAddress.StateProvinceId.HasValue)
                {
                    var state = _stateProvinceService.GetStateProvinceById(defaultAddress.StateProvinceId.Value);
                    statementDTO.StatementFromState = state.Name;
                }

                statementDTO.StatementFromAddress1 = defaultAddress.Address1;
                statementDTO.StatementFromAddress2 = defaultAddress.Address2;
                statementDTO.StatementFromZipPostalCode = defaultAddress.ZipPostalCode;
                statementDTO.StatementFromCity = defaultAddress.City;
            }
            else
            {
                statementDTO.StatementFromAddress1 = "Address 1";
                statementDTO.StatementFromState = "State 1";
                statementDTO.StatementFromCountry = "country 1";
            }

            statementDTO.StatementFromName = _storeContext.CurrentStore.Name;
            statementDTO.StatementFromBusinessName = _storeContext.CurrentStore.Name;
        }
    }
}