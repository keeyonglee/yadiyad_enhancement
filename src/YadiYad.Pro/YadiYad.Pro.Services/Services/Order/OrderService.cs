using AutoMapper;
using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Consultation;
using YadiYad.Pro.Services.DTO.Order;
using YadiYad.Pro.Services.DTO.Service;
using YadiYad.Pro.Services.Services.Base;

using Nop.Core.Domain.Orders;
using Nop.Services.Orders;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using YadiYad.Pro.Core.Domain.Deposit;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Services.DTO.Job;
using YadiYad.Pro.Core.Domain.Refund;
using Nop.Core.Domain.Payments;

namespace YadiYad.Pro.Services.Order
{
    public class OrderService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<ProOrder> _proOrderRepository;
        private readonly IRepository<ProOrderItem> _proOrderItemRepository;
        private readonly IRepository<ConsultationInvitation> _consultationInvitationRepository;
        private readonly IRepository<ServiceApplication> _serviceApplicationRepository;
        private readonly IRepository<JobApplication> _jobApplicationRepository;
        private readonly IRepository<JobProfile> _jobProfileRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<DepositRequest> _depositRequestRepository;
        private readonly IRepository<RefundRequest> _refundRequestRepository;
        #endregion

        #region Ctor

        public OrderService
            (IMapper mapper,
            IRepository<ProOrder> proOrderRepository,
            IRepository<ProOrderItem> proOrderItemRepository,
            IRepository<ProInvoice> proInvoiceRepository,
            IRepository<JobApplication> jobApplicationRepository,
            IRepository<ConsultationInvitation> consultationInvitationRepository,
            IRepository<ConsultationProfile> consultationProfileRepository,
            IRepository<JobProfile> jobProfileRepository,
            IRepository<RefundRequest> refundRequestRepository,
            IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IRepository<Customer> customerRepository,
            IRepository<DepositRequest> depositRequestRepository,
            IRepository<ServiceApplication> serviceApplicationRepository)
        {
            _mapper = mapper;
            _refundRequestRepository = refundRequestRepository;
            _proOrderRepository = proOrderRepository;
            _proOrderItemRepository = proOrderItemRepository;
            _consultationInvitationRepository = consultationInvitationRepository;
            _serviceApplicationRepository = serviceApplicationRepository;
            _customerRepository = customerRepository;
            _depositRequestRepository = depositRequestRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _jobProfileRepository = jobProfileRepository;
        }

        #endregion

        #region Methods

        public string GenerateOrderCustomNumber()
        {
            var currentDate = DateTime.Now;
            var countByProductType = _proOrderRepository.Table.Count();
            var customNumber =
                "PRO-YYMMDD-COUNT"
                .Replace("YY", currentDate.ToString("yy"))
                .Replace("MM", currentDate.ToString("MM"))
                .Replace("DD", currentDate.ToString("dd"))
                .Replace("COUNT", countByProductType.ToString())
                .Trim();

            return customNumber;
        }

        public OrderDTO CreateOrder(int actorId, int customerId, OrderDTO dto)
        {
            var order = dto.ToModel(_mapper, customerId);

            var customer = _customerRepository.Table.Where(x => x.Id == customerId).FirstOrDefault();

            CreateAudit(order, actorId);

            var paymentMethodSystemName = "Payments.IPay88";

            order.PaymentMethodSystemName = paymentMethodSystemName;

            _proOrderRepository.Insert(order);

            foreach (var orderItem in order.OrderItems)
            {
                orderItem.OrderId = order.Id;

                CreateAudit(orderItem, actorId);
            }

            _proOrderItemRepository.Insert(order.OrderItems);

            dto.Id = order.Id;

            foreach (var orderItem in order.OrderItems)
            {
                var orderItemDTO = dto.OrderItems
                    .Where(x => x.RefId == orderItem.RefId
                    && x.ProductTypeId == orderItem.ProductTypeId)
                    .First();
                orderItemDTO.Id = orderItem.Id;
            }

            return dto;
        }

        public OrderDTO UpdateOrder(int actorId, int customerId, OrderDTO dto)
        {
            #region update order

            var order = _proOrderRepository.Table
                .Where(x => x.Id == dto.Id
                && x.CustomerId == customerId)
                .FirstOrDefault();

            var updatingOrder = dto.ToModel(_mapper, customerId);

            UpdateAudit(order, updatingOrder, actorId);

            _proOrderRepository.Update(updatingOrder);

            #endregion

            return dto;
        }

        public ICustomOrderEntity GetCustomOrder(int id)
        {
            var model = _proOrderRepository.Table.Where(x => x.Id == id && x.Deleted == false)
                .FirstOrDefault();

            return model;
        }

        public bool HasMatchedOrder(int id, int paymentStatusId, int orderStatusId)
        {
            var any = _proOrderRepository.Table.Any(x =>
            x.Id == id
            && x.PaymentStatusId == paymentStatusId
            && x.OrderStatusId == orderStatusId
            && x.Deleted == false);

            return any;
        }

        public OrderDTO GetOrderById(int id)
        {
            if (id == 0)
                return null;

            var model = _proOrderRepository.Table.Where(x => x.Id == id && x.Deleted == false)
                .FirstOrDefault();

            var dto = _mapper.Map<OrderDTO>(model);

            if (dto != null)
            {
                var orderItemsModels = _proOrderItemRepository.Table.Where(x => x.OrderId == id && x.Deleted == false)
                    .ToList();

                var orderItemsDTOs = orderItemsModels.Select(x => _mapper.Map<OrderItemDTO>(x)).ToList();

                var consultationInvitationIds = orderItemsDTOs.Where(x => x.ProductTypeId == (int)ProductType.ConsultationEngagementMatchingFee).Select(x => x.RefId);
                var consultationInvitations = _consultationInvitationRepository.Table.Where(x => consultationInvitationIds.Contains(x.Id)).Select(x => _mapper.Map<ConsultationInvitationDTO>(x)).ToList();

                var serviceApplicationIds = orderItemsDTOs.Where(x => x.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee).Select(x => x.RefId);
                var serviceApplications = _serviceApplicationRepository.Table.Where(x => serviceApplicationIds.Contains(x.Id)).Select(x => _mapper.Map<ServiceApplicationDTO>(x)).ToList();

                orderItemsDTOs = orderItemsDTOs.Select(x =>
                {
                    if (x.ProductTypeId == (int)ProductType.ConsultationEngagementMatchingFee)
                    {
                        x.ConsultationInvitation = consultationInvitations.Where(y => y.Id == x.RefId).FirstOrDefault();
                    }
                    else if (x.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee)
                    {
                        x.ServiceApplication = serviceApplications.Where(y => y.Id == x.RefId).FirstOrDefault();
                    }
                    return x;
                }).ToList();

                dto.OrderItems = orderItemsDTOs;
            }

            return dto;
        }

        public PagedListDTO<OrderDTO> SearchOrders(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            OrderSearchFilterDTO filterDTO = null)
        {
            var query = _proOrderRepository.Table;
            query = query.Where(x => !x.Deleted);
            if (filterDTO.CustomerIds != null && filterDTO.CustomerIds.Count != 0)
            {
                query = query.Where(x => filterDTO.CustomerIds.Contains(x.CustomerId));
            }
            if (filterDTO.OrderStatusIds != null && filterDTO.OrderStatusIds.Count != 0)
            {
                query = query.Where(x => filterDTO.OrderStatusIds.Contains(x.OrderStatusId));
            }
            if (filterDTO.PaymentStatusIds != null && filterDTO.PaymentStatusIds.Count != 0)
            {
                query = query.Where(x => filterDTO.PaymentStatusIds.Contains(x.PaymentStatusId));
            }
            if (filterDTO.ProductTypeIds != null && filterDTO.ProductTypeIds.Count != 0
                && filterDTO.RefIds != null && filterDTO.RefIds.Count != 0)
            {
                var orderIds = _proOrderItemRepository.Table
                    .Where(x => !x.Deleted
                        && filterDTO.ProductTypeIds.Contains(x.ProductTypeId)
                        && filterDTO.RefIds.Contains(x.RefId)
                    )
                    .Select(x => x.OrderId)
                    .ToList();
                query = query.Where(x => orderIds.Contains(x.Id));
            }

            List<OrderDTO> result = new List<OrderDTO>();
            var record = query.ToList();

            if (record.Count != 0)
            {
                var orderIds = record.Select(x => x.Id).ToList();
                var orderItemsModels = _proOrderItemRepository.Table.Where(x => orderIds.Contains(x.OrderId) && x.Deleted == false)
                    .ToList();

                var orderItemsDTOs = orderItemsModels.Select(x => _mapper.Map<OrderItemDTO>(x)).ToList();

                var consultationInvitationIds = orderItemsDTOs.Where(x => x.ProductTypeId == (int)ProductType.ConsultationEngagementMatchingFee).Select(x => x.RefId).ToList();
                var consultationInvitations = _consultationInvitationRepository.Table.Where(x => consultationInvitationIds.Contains(x.Id)).Select(x => _mapper.Map<ConsultationInvitationDTO>(x)).ToList();

                var serviceApplicationIds = orderItemsDTOs.Where(x => x.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee || x.ProductTypeId == (int)ProductType.ServiceEnagegementFee).Select(x => x.RefId).ToList();
                var serviceApplications = _serviceApplicationRepository.Table.Where(x => serviceApplicationIds.Contains(x.Id)).Select(x => _mapper.Map<ServiceApplicationDTO>(x)).ToList();

                orderItemsDTOs = orderItemsDTOs.Select(x =>
                {
                    if (x.ProductTypeId == (int)ProductType.ConsultationEngagementMatchingFee)
                    {
                        x.ConsultationInvitation = consultationInvitations.Where(y => y.Id == x.RefId).FirstOrDefault();
                    }
                    else if (x.ProductTypeId == (int)ProductType.ServiceEnagegementMatchingFee || x.ProductTypeId == (int)ProductType.ServiceEnagegementFee)
                    {
                        x.ServiceApplication = serviceApplications.Where(y => y.Id == x.RefId).FirstOrDefault();
                    }
                    return x;
                }).ToList();

                result = record.Select(x => _mapper.Map<OrderDTO>(x)).ToList().Select(x =>
                {
                    x.OrderItems = orderItemsDTOs.Where(y => y.OrderId == x.Id).ToList();
                    return x;
                }).ToList();
            }

            var totalCount = result.Count();
            var records = result.ToList();

            var response = new PagedListDTO<OrderDTO>(records, pageIndex, pageSize, totalCount);

            return response;
        }

        public virtual IPagedList<ProOrder> GetAll(
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _proOrderRepository.Table;

            query = query.OrderBy(n => n.Id);

            var data = new PagedList<ProOrder>(query, pageIndex, pageSize);

            return data;
        }

        public virtual ProOrder GetById(int id)
        {
            if (id == 0)
                return null;

            return _proOrderRepository.GetById(id);
        }

        public IQueryable<PaymentDetailsDTO> GetPaymentDetailsQuery()
        {
            var query = from po in _proOrderRepository.Table
                        where po.Deleted == false
                        select new PaymentDetailsDTO
                        {
                            Id = po.Id,
                            OrderNumber = po.CustomOrderNumber,
                            CustomerId = po.CustomerId,
                            PaymentStatusId = po.PaymentStatusId,
                            OrderTotal = po.OrderTotal,
                            OrderDate = po.CreatedOnUTC
                        };

            return query;
        }

        public IPagedList<PaymentDetailsDTO> GetPaymentDetailsById(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            string keyword = null,
            PaymentDetailsSearchFilterDTO filterDTO = null)
        {
            var record = GetPaymentDetailsQuery();
            if (filterDTO != null)
            {
                if (filterDTO.OrderDate != null)
                {
                    record = record.Where(x => x.OrderDate <= filterDTO.OrderDate);
                }
                if (filterDTO.CustomerId != 0)
                {
                    record = record.Where(x => x.CustomerId == filterDTO.CustomerId);
                }
                if (filterDTO.PaymentStatusId != 0)
                {
                    record = record.Where(x => x.PaymentStatusId <= filterDTO.PaymentStatusId);
                }
            }

            var response = new PagedList<PaymentDetailsDTO>(record, pageIndex, pageSize);

            return response;
        }

        public void UpdateOrderItem(int actorId, int customerId, List<int> orderItemIds, int invoiceId)
        {
            #region update orderItem
            var orderItems = _proOrderItemRepository.Table
                .Where(x => !x.Deleted && orderItemIds.Contains(x.Id))
                .ToList()
                .Select(x =>
                {
                    x.InvoiceId = invoiceId;
                    UpdateAudit(x, x, actorId);
                    return x;
                })
                .ToList();
            if (orderItems.Count > 0)
            {
                _proOrderItemRepository.Update(orderItems);
            }

            #endregion
        }

        public void UpdateOrderItem(int actorId, int orderItemId, ProOrderItemStatus status)
        {
            var orderItems = _proOrderItemRepository.Table
                .Where(x => !x.Deleted
                && x.Id == orderItemId)
                .FirstOrDefault();

            if (orderItems == null)
            {
                throw new KeyNotFoundException("Pro order item not found.");
            }

            orderItems.Status = (int)status;
            orderItems.UpdateAudit(actorId);
            _proOrderItemRepository.Update(orderItems);
        }

        public OrderItemDTO GetOrderItem(int depositRequestId)
        {
            var orderItemQuery =
                from dr in _depositRequestRepository.Table
                    .Where(dr => dr.Deleted == false
                    && dr.Id == depositRequestId)
                from oi in _proOrderItemRepository.Table
                   .Where(oi => oi.Deleted == false
                   && oi.Id == dr.OrderItemId)
                select oi;

            var orderitem = orderItemQuery.FirstOrDefault();

            var orderItemDTO = _mapper.Map<OrderItemDTO>(orderitem);

            return orderItemDTO;
        }

        public OrderItemDTO GetOrderItem(int productTypeId, int refId)
        {
            var orderItemQuery =
                from oi in _proOrderItemRepository.Table
                   .Where(oi => oi.Deleted == false
                   && oi.ProductTypeId == productTypeId
                   && oi.RefId == refId)
                from po in _proOrderRepository.Table
                    .Where(po=>po.Deleted == false
                    && po.Id == oi.OrderId
                    && po.OrderStatusId == (int)OrderStatus.Complete)
                select oi;

            var orderitem = orderItemQuery.FirstOrDefault();

            if(orderitem == null && productTypeId == (int)ProductType.ConsultationEngagementMatchingFee)
            {
                //find consultation engagement fee
                var consultationEngagementFeeQuery =
                    from oi in _proOrderItemRepository.Table
                       .Where(oi => oi.Deleted == false
                       && oi.ProductTypeId == (int)ProductType.ConsultationEngagementFee
                       && oi.RefId == refId
                       && oi.Price < 0)
                    from po in _proOrderRepository.Table
                        .Where(po => po.Deleted == false
                        && po.Id == oi.OrderId
                        && po.OrderStatusId == (int)OrderStatus.Complete)
                    select oi;

                var consultationEngagementFee = consultationEngagementFeeQuery
                    .FirstOrDefault();

                if (consultationEngagementFee != null
                    && consultationEngagementFee.OffsetProOrderItemId != null
                    && consultationEngagementFee.OffsetProOrderItemId > 0)
                {
                    //get offsetted consultation engagement fee
                    var offsetConsultationEngagementMatchingFeeQuery =
                        from oi in _proOrderItemRepository.Table
                           .Where(oi => oi.Deleted == false
                           && oi.ProductTypeId == (int)ProductType.ConsultationEngagementFee
                           && oi.Id == consultationEngagementFee.OffsetProOrderItemId)
                        from po in _proOrderRepository.Table
                            .Where(po => po.Deleted == false
                            && po.Id == oi.OrderId
                            && po.OrderStatusId == (int)OrderStatus.Complete)
                        select oi;

                    var offsetConsultationEngagementMatchingFee =
                        offsetConsultationEngagementMatchingFeeQuery.FirstOrDefault();

                    if (offsetConsultationEngagementMatchingFee != null)
                    {
                        //get carry forward consultation engagement matching fee
                        var offsetConsultationEngagementFeeQuery =
                            from oi in _proOrderItemRepository.Table
                               .Where(oi => oi.Deleted == false
                               && oi.ProductTypeId == (int)ProductType.ConsultationEngagementMatchingFee
                               && oi.RefId == offsetConsultationEngagementMatchingFee.RefId)
                            from po in _proOrderRepository.Table
                                .Where(po => po.Deleted == false
                                && po.Id == oi.OrderId
                                && po.OrderStatusId == (int)OrderStatus.Complete)
                            select oi;

                        var offsetConsultationEngagementFee =
                            offsetConsultationEngagementFeeQuery.FirstOrDefault();

                        if(offsetConsultationEngagementFee != null)
                        {
                            orderitem = offsetConsultationEngagementFee;
                        }
                    }
                }
            }

            var orderItemDTO = _mapper.Map<OrderItemDTO>(orderitem);

            return orderItemDTO;
        }

        /// <summary>
        /// get order item which available for offset by product type and ref id
        /// </summary>
        /// <param name="productType"></param>
        /// <param name="refId"></param>
        /// <returns></returns>
        public OrderItemDTO GetOrderItemBForOffsetByProductTypeRefId(int buyerId, ProductType productType, int refId)
        {
            ProOrderItem proOrderitem = null;
            string engagementCode = null;

            switch (productType)
            {
                case ProductType.JobEnagegementFee:
                    {
                        //get order item with open status and same job profile with current job application
                        var orderItemQuery =
                               (from ja in _jobApplicationRepository.Table
                                .Where(ja => ja.Deleted == false
                                && ja.Id == refId)

                                from eja in _jobApplicationRepository.Table
                                .Where(eja => eja.Deleted == false
                                && eja.JobProfileId == ja.JobProfileId)

                                from jp  in _jobProfileRepository.Table
                                .Where(jp=>jp.Deleted == false
                                && jp.CustomerId == buyerId)

                                from oi in _proOrderItemRepository.Table
                                   .Where(oi => oi.Deleted == false
                                   && oi.Status == (int)ProOrderItemStatus.OpenForRematch
                                   && oi.ProductTypeId == (int)productType
                                   && oi.RefId == eja.Id)

                                from o in _proOrderRepository.Table
                                .Where(o => o.Deleted == false
                                && o.Id == oi.OrderId)
                                select new
                                {
                                    ProOrderItem = oi,
                                    JobApplication = eja
                                })
                               .OrderBy(x => x.ProOrderItem.CreatedOnUTC);

                        var orderItem = orderItemQuery.FirstOrDefault();

                        if(orderItem != null)
                        {
                            proOrderitem = orderItem.ProOrderItem;
                            var engagementDTO = _mapper.Map<JobApplicationDTO>(orderItem.JobApplication);
                            engagementCode = engagementDTO.Code;
                        }
                    }
                    break;
                case ProductType.ConsultationEngagementFee:
                    {
                        //get order item with open status and same consultation profile with current consultation invitation
                        var orderItemQuery =
                               (from ci in _consultationInvitationRepository.Table
                                .Where(ci => ci.Deleted == false
                                && ci.Id == refId)

                                from eci in _consultationInvitationRepository.Table
                                .Where(eci => eci.Deleted == false
                                && eci.ConsultationProfileId == ci.ConsultationProfileId)

                                from oi in _proOrderItemRepository.Table
                                   .Where(oi => oi.Deleted == false
                                   && oi.Status == (int)ProOrderItemStatus.OpenForRematch
                                   && oi.ProductTypeId == (int)productType
                                   && oi.RefId == eci.Id)

                                from o in _proOrderRepository.Table
                                .Where(o => o.Deleted == false
                                && o.CustomerId == buyerId
                                && o.Id == oi.OrderId)
                                select new
                                {
                                    ProOrderItem = oi,
                                    ConsultationInvitation = eci
                                })
                               .OrderBy(x => x.ProOrderItem.CreatedOnUTC);

                        var orderItem = orderItemQuery.FirstOrDefault();

                        if (orderItem != null)
                        {
                            proOrderitem = orderItem.ProOrderItem;
                            var engagementDTO = _mapper.Map<ConsultationInvitationDTO>(orderItem.ConsultationInvitation);
                            engagementCode = engagementDTO.Code;
                        }

                    }
                    break;
                case ProductType.ServiceEnagegementFee:
                    {
                        //get order item with open status on any service application
                        var orderItemQuery =
                            (from oi in _proOrderItemRepository.Table
                               .Where(oi => oi.Deleted == false
                               && oi.Status == (int)ProOrderItemStatus.OpenForRematch
                                   && oi.ProductTypeId == (int)productType)

                             from esa in _serviceApplicationRepository.Table
                             .Where(esa => esa.Deleted == false
                             && esa.Id == oi.RefId)

                             from o in _proOrderRepository.Table
                             .Where(o => o.Deleted == false
                             && o.CustomerId == buyerId
                            && o.Id == oi.OrderId)
                             select new
                             {
                                 ProOrderItem = oi,
                                 ServiceApplication = esa
                             })
                            .OrderBy(x => x.ProOrderItem.CreatedOnUTC);

                        var orderItem = orderItemQuery.FirstOrDefault();

                        if (orderItem != null)
                        {
                            proOrderitem = orderItem.ProOrderItem;
                            var engagementDTO = _mapper.Map<ServiceApplicationDTO>(orderItem.ServiceApplication);
                            engagementCode = engagementDTO.Code;
                        }
                    }
                    break;
            }

            OrderItemDTO orderItemDTO = null;

            if(proOrderitem != null)
            {
                orderItemDTO = _mapper.Map<OrderItemDTO>(proOrderitem);
                orderItemDTO.EngagementCode = engagementCode;
            }

            return orderItemDTO;
        }

        public OrderItemDTO SetOrderItemToOpenForRematch(ProductType productType, int refId)
        {
            //get all order item by product type and refid
            var orderItemQuery =
                from oi in _proOrderItemRepository.Table
                   .Where(oi => oi.Deleted == false
                   && oi.ProductTypeId == (int)productType
                   && oi.RefId == refId)
                from o in _proOrderRepository.Table
                    .Where(o=>o.Deleted == false
                    && o.OrderStatusId == (int)OrderStatus.Complete
                    && o.Id == oi.OrderId)
                select oi;

            var orderItems = orderItemQuery.ToList();

            //check if order item exists
            if (orderItems == null
                || orderItems.Count <= 0)
            {
                throw new InvalidOperationException("Pro order item not found for the engagment.");
            }

            //skip if order item being offset
            if (orderItems.Any(x=>x.OffsetProOrderItemId.HasValue == true && x.OffsetProOrderItemId >= 0))
            {
                return null;
            }

            //set order item to "Open For Rematch"
            var engagementFeeOrderItem = orderItems.Where(x => x.Price > 0).First();

            engagementFeeOrderItem.Status = (int)ProOrderItemStatus.OpenForRematch;

            _proOrderItemRepository.Update(engagementFeeOrderItem);

            OrderItemDTO orderItemDTO = _mapper.Map<OrderItemDTO>(engagementFeeOrderItem);

            return orderItemDTO;
        }

        public OrderItemDTO SetOpenForRematchedOrderItemToPaid(int actorId, int proOrderItemId)
        {
            //get all order item by product type and refid
            var orderItemQuery =
                from oi in _proOrderItemRepository.Table
                   .Where(oi => oi.Deleted == false
                   && oi.Status == (int)ProOrderItemStatus.OpenForRematch
                   && oi.Id == proOrderItemId)
                from o in _proOrderRepository.Table
                    .Where(o => o.Deleted == false
                    && o.OrderStatusId == (int)OrderStatus.Complete
                    && o.Id == oi.OrderId)
                select oi;

            var orderItems = orderItemQuery.ToList();

            //check if order item exists
            if (orderItems == null
                || orderItems.Count <= 0)
            {
                throw new InvalidOperationException("Pro order item not found for the engagment.");
            }

            //set order item from "Open For Rematch" to "Paid" after raised refund
            var engagementFeeOrderItem = orderItems.First();

            engagementFeeOrderItem.Status = (int)ProOrderItemStatus.Paid;
            engagementFeeOrderItem.UpdateAudit(actorId);

            _proOrderItemRepository.Update(engagementFeeOrderItem);

            OrderItemDTO orderItemDTO = _mapper.Map<OrderItemDTO>(engagementFeeOrderItem);

            return orderItemDTO;
        }

        public List<OrderItemDTO> GetRefundableOrderItems(ProductType productType, List<int> refIds)
        {
            //get order items which haven't refund and haven't offsetted
            var orderItemsQuery =
                (from poi in _proOrderItemRepository.Table
                .Where(poi => poi.Deleted == false
                && poi.ProductTypeId == (int)productType
                && poi.Status == (int)ProOrderItemStatus.OpenForRematch
                && refIds.Contains(poi.RefId))
                 from po in _proOrderRepository.Table
                .Where(po => po.Deleted == false
                && po.Id == poi.OrderId
                && po.OrderStatusId == (int)OrderStatus.Complete
                && po.PaymentStatusId == (int)PaymentStatus.Paid

                && (from ofpoi in _proOrderItemRepository.Table
                    .Where(ofpoi => poi.Deleted == false
                    && ofpoi.OffsetProOrderItemId == poi.Id)
                    from ofpo in _proOrderRepository.Table
                    .Where(ofpo => ofpo.Deleted == false
                    && ofpo.OrderStatusId == (int)OrderStatus.Complete
                    && ofpo.PaymentStatusId == (int)PaymentStatus.Paid
                    && ofpo.Id == ofpoi.OrderId)
                    select ofpo)
                    .Any() == false

                && (from rr in _refundRequestRepository.Table
                    .Where(rr => rr.Deleted == false
                    && rr.OrderItemId == poi.Id)
                    select rr)
                    .Any() == false)
                 select new
                 {
                     poi
                 })
                .Select(x => x.poi);

            var orderItems = orderItemsQuery
                .ToList();

            var orderItemDTOs = _mapper.Map<List<OrderItemDTO>>(orderItems);

            return orderItemDTOs;
        }

        #endregion
    }
}
