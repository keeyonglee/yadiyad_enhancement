using AutoMapper;
using Nop.Core.Domain.Documents;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payout;
using Nop.Data;
using Nop.Services.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using YadiYad.Pro.Core.Domain.Consultation;
using YadiYad.Pro.Core.Domain.Job;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Core.Domain.Refund;
using YadiYad.Pro.Core.Domain.Service;
using YadiYad.Pro.Services.Consultation;
using YadiYad.Pro.Services.DTO.Common;
using YadiYad.Pro.Services.DTO.Payout;
using YadiYad.Pro.Services.DTO.Refund;
using YadiYad.Pro.Services.Job;
using YadiYad.Pro.Services.Service;
using YadiYad.Pro.Services.Services.Base;

namespace YadiYad.Pro.Services.Refund
{
    public class RefundRequestService : BaseService
    {
        #region Fields
        private readonly IMapper _mapper;
        private readonly IRepository<RefundRequest> _refundRequestRepository;
        private readonly IRepository<ProOrder> _proOrderRepository;
        private readonly IRepository<ProOrderItem> _proOrderItemRepository;

        private readonly JobApplicationService _jobApplicationService;
        private readonly ServiceApplicationService _serviceApplicationService;
        private readonly ConsultationInvitationService _consultationInvitationService;
        private readonly DocumentNumberService _documentNumberService;

        #endregion

        #region Ctor

        public RefundRequestService
            (IMapper mapper,
            IRepository<RefundRequest> refundRequestRepository,
            IRepository<ProOrder> proOrderRepository,
            IRepository<ProOrderItem> proOrderItemRepository,
            JobApplicationService jobApplicationService,
            ServiceApplicationService serviceApplicationService,
            ConsultationInvitationService consultationInvitationService,
            DocumentNumberService documentNumberService)
        {
            _mapper = mapper;
            _refundRequestRepository = refundRequestRepository;
            _proOrderRepository = proOrderRepository;
            _proOrderItemRepository = proOrderItemRepository;
            _jobApplicationService = jobApplicationService;
            _serviceApplicationService = serviceApplicationService;
            _consultationInvitationService = consultationInvitationService;
            _documentNumberService = documentNumberService;
        }

        #endregion

        #region Methods

        public virtual RefundRequestDTO CreateRefundRequest(int actorId, int orderItemId, decimal amount, decimal serviceCharges = 0m, bool isCN = false)
        {
            var order =
                    (from poi in _proOrderItemRepository.Table
                    .Where(poi => poi.Deleted == false
                    && poi.Id == orderItemId)
                     from po in _proOrderRepository.Table
                     .Where(po => po.Deleted == false
                     && po.Id == poi.OrderId
                     && po.OrderStatusId == (int)OrderStatus.Complete)
                     from rr in _refundRequestRepository.Table
                     .Where(rr=>rr.Deleted == false
                     && rr.OrderItemId == poi.Id)
                     .DefaultIfEmpty()
                     select new
                     {
                         CustomerId = po.CustomerId,
                         RefundRequestId = rr != null?(int?)rr.Id:null
                     })
                     .Where(x=>x.RefundRequestId == null)
                    .FirstOrDefault();
                    
            if (order != null)
            {
                var docNumbers = _documentNumberService
                    .GetDocumentNumbers(isCN ? RunningNumberType.CreditNote : RunningNumberType.Refund, 1);

                var model = new RefundRequest
                {
                    OrderItemId = orderItemId,
                    RefundNumber = docNumbers.First(),
                    Amount = amount,
                    ServiceCharge = serviceCharges,
                    RefundTo = order.CustomerId
                };
                model.CreateAudit(actorId);

                _refundRequestRepository.Insert(model);

                return _mapper.Map<RefundRequestDTO>(model);
            }

            return null;
        }

        public RefundRequestDTO GetByOrderItemId(int orderItemId)
        {
            var entity = _refundRequestRepository.Table
                .Where(x => x.Deleted == false
                && x.OrderItemId == orderItemId)
                .FirstOrDefault();

            var dto = _mapper.Map<RefundRequestDTO>(entity);

            return dto;
        }

        public RefundRequestDTO RefundProOrderItem(int actorId, ProductType productType, int refId, decimal adminCharges = 0.00m, decimal deduction = 0.00m)
        {
            //get all order item by product type and refid
            var orderItemQuery =
                from oi in _proOrderItemRepository.Table
                   .Where(oi => oi.Deleted == false
                   && oi.Status == (int)ProOrderItemStatus.Paid
                   && oi.ProductTypeId == (int)productType
                   && oi.RefId == refId)
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

            //create refund request
            var engagementFeeOrderItem = orderItems.Where(x => x.Price > 0).First();

            var refundRequestDTO = CreateRefundRequest(
                actorId,
                engagementFeeOrderItem.Id,
                engagementFeeOrderItem.Price - adminCharges - deduction,
                adminCharges);

            return refundRequestDTO;
        }

        public PagedListDTO<RefundRequestDTO> GetRefundRequestByRefId(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            RefundRequestFilterDTO filterDTO = null)
        {
            var responseDTO = new PagedListDTO<RefundRequestDTO>(new List<RefundRequestDTO>(), pageIndex, pageSize, 0);
            var queryRR = _proOrderItemRepository.Table
                .Where(x => !x.Deleted && filterDTO.ProductTypeIDs.Contains(x.ProductTypeId)  && x.RefId == filterDTO.RefId)
                .Join(
                    _proOrderRepository.Table.Where(x => !x.Deleted),
                    x => x.OrderId,
                    y => y.Id,
                    (x, y) => x
                )
                .Join(
                    _refundRequestRepository.Table.Where(x => !x.Deleted && x.RefundTo == filterDTO.CustomerId),
                    x => x.Id,
                    y => y.OrderItemId,
                    (x, y) => y
                );

            var totalCount = queryRR.Count();

            var query = queryRR
                        .OrderByDescending(x => x.CreatedOnUTC)
                        .Take(pageSize)
                        .Skip(pageSize * pageIndex);

            var records = query
                .ToList()
                .Select(x => _mapper.Map<RefundRequestDTO>(x))
                .ToList();

            responseDTO = new PagedListDTO<RefundRequestDTO>(records, pageIndex, pageSize, totalCount);

            return responseDTO;
        }

        public RefundRequestDTO UpdateRefundRequestStatus(int actorId, int refundRequestId, RefundStatus status)
        {
            //verify updating status flow
            var expectedCurrentStatus = new List<int>();

            switch (status)
            {
                case RefundStatus.Paid:
                    {
                        expectedCurrentStatus.Add((int)RefundStatus.New);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid status.");
            }

            //query refund request
            var refundRequest =
                _refundRequestRepository.Table
                .Where(x => x.Deleted == false
                && x.Id == refundRequestId
                && expectedCurrentStatus.Contains(x.Status))
                .FirstOrDefault();

            if(refundRequest == null)
            {
                throw new KeyNotFoundException("No valid refund request found.");
            }

            //update refund request status
            refundRequest.Status = (int)status;
            refundRequest.UpdateAudit(actorId);

            _refundRequestRepository.Update(refundRequest);

            var refundRequestDTO = _mapper.Map<RefundRequestDTO>(refundRequest);

            return refundRequestDTO;
        }

        #endregion
    }
}