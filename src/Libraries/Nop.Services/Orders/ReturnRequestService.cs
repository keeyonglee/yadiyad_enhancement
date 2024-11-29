using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Events;
using Nop.Services.Media;
using Nop.Services.ShuqReturnRequest;
using Org.BouncyCastle.Bcpg;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Return request service
    /// </summary>
    public partial class ReturnRequestService : IReturnRequestService
    {
        #region Fields

        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<ReturnRequest> _returnRequestRepository;
        private readonly IRepository<GroupReturnRequest> _groupReturnRequestRepository;
        private readonly IRepository<ReturnRequestAction> _returnRequestActionRepository;
        private readonly IRepository<ReturnRequestReason> _returnRequestReasonRepository;
        private readonly IRepository<ReturnRequestImage> _returnRequestImageRepository;
        private readonly IRepository<Dispute> _disputeRepository;
        private readonly IRepository<SellerDisputePicture> _sellerDisputePictureRepository;
        private readonly IRepository<ReturnOrder> _returnOrderRepository;
        private readonly IRepository<ReturnOrderItem> _returnOrderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Shipment> _shipmentRepository;

        #endregion

        #region Ctor

        public ReturnRequestService(ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IRepository<ReturnRequest> returnRequestRepository,
            IRepository<GroupReturnRequest> groupReturnRequestRepository,
            IRepository<ReturnRequestAction> returnRequestActionRepository,
            IRepository<ReturnRequestImage> returnRequestImageRepository,
            IRepository<Dispute> disputeRepository,
            IRepository<ReturnRequestReason> returnRequestReasonRepository,
            IRepository<SellerDisputePicture> sellerDisputePictureRepository,
            IRepository<ReturnOrder> returnOrderRepository,
            IRepository<ReturnOrderItem> returnOrderItemRepository,
            IRepository<Product> productRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Order> orderRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository,
            IWorkContext workContext,
            IRepository<Shipment> shipmentRepository)
        {
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;
            _returnRequestRepository = returnRequestRepository;
            _groupReturnRequestRepository = groupReturnRequestRepository;
            _returnRequestActionRepository = returnRequestActionRepository;
            _returnRequestImageRepository = returnRequestImageRepository;
            _disputeRepository = disputeRepository;
            _returnRequestReasonRepository = returnRequestReasonRepository;
            _sellerDisputePictureRepository = sellerDisputePictureRepository;
            _returnOrderRepository = returnOrderRepository;
            _returnOrderItemRepository = returnOrderItemRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productCategoryRepository = productCategoryRepository;
            _categoryRepository = categoryRepository;
            _workContext = workContext;
            _shipmentRepository = shipmentRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual void DeleteReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            _returnRequestRepository.Delete(returnRequest);

            //event notification
            _eventPublisher.EntityDeleted(returnRequest);
        }

        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        public virtual ReturnRequest GetReturnRequestById(int returnRequestId)
        {
            if (returnRequestId == 0)
                return null;

            return _returnRequestRepository.GetById(returnRequestId);
        }

        public virtual GroupReturnRequest GetGroupReturnRequestById(int groupId)
        {
            if (groupId == 0)
                return null;

            return _groupReturnRequestRepository.GetById(groupId);
        }

        public virtual IList<ReturnRequestImage> GetReturnRequestImageByGroupReturnRequestId(int groupReturnRequestId)
        {
            var query = from pp in _returnRequestImageRepository.Table
                        where pp.GroupReturnRequestId == groupReturnRequestId
                        orderby pp.Id
                        select pp;

            var pictures = query.ToList();

            return pictures;
        }
        public virtual IList<ReturnRequest> GetReturnRequestByGroupReturnRequestId(int groupId)
        {
            if (groupId == 0)
                return null;

            var query = from o in _returnRequestRepository.Table
                        where o.GroupReturnRequestId == groupId
                        select o;

            return query.ToList();
        }

        public virtual IList<ReturnOrderItem> GetReturnOrderItemByReturnOrderId(int returnOrderId)
        {
            if (returnOrderId == 0)
                return null;
            

            var query = from ro in _returnOrderItemRepository.Table
                        where ro.ReturnOrderId == returnOrderId
                        select ro;

            return query.ToList();
        }

        public virtual IList<Product> GetProducts(GroupReturnRequest groupReturnRequest)
        {
            if (groupReturnRequest == null)
                throw new ArgumentNullException(nameof(groupReturnRequest));

            var query = from r in _returnRequestRepository.Table
                        join oi in _orderItemRepository.Table on r.OrderItemId equals oi.Id
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        where r.GroupReturnRequestId == groupReturnRequest.Id
                        select p;

            return query.ToList();
        }
        
        public virtual IList<Category> GetProductCategories(GroupReturnRequest groupReturnRequest)
        {
            if (groupReturnRequest == null)
                throw new ArgumentNullException(nameof(groupReturnRequest));

            var query = from r in _returnRequestRepository.Table
                        join oi in _orderItemRepository.Table on r.OrderItemId equals oi.Id
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        where r.GroupReturnRequestId == groupReturnRequest.Id
                        select c;

            return query.ToList();
        }

        public IList<GroupReturnRequest> GetPendingInspectionReturns(DateTime cutOffTime)
        {
            var query = from s in _shipmentRepository.Table
                join r in _returnOrderRepository.Table on s.ReturnOrderId equals r.Id
                join g in _groupReturnRequestRepository.Table on r.GroupReturnRequestId equals g.Id
                where s.DeliveryDateUtc < cutOffTime
                      && g.ReturnConditionId == (int) ReturnConditionEnum.Pending
                select g;

            return query.ToList();
        }


        /// <summary>
        /// Search return requests
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all entries</param>
        /// <param name="customerId">Customer identifier; 0 to load all entries</param>
        /// <param name="orderItemId">Order item identifier; 0 to load all entries</param>
        /// <param name="customNumber">Custom number; null or empty to load all entries</param>
        /// <param name="rs">Return request status; null to load all entries</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>Return requests</returns>
        public IPagedList<ReturnRequest> SearchReturnRequests(int groupId = 0, int storeId = 0, int customerId = 0,
            int orderItemId = 0, string customNumber = "", DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _returnRequestRepository.Table;
            if (storeId > 0)
                query = query.Where(rr => storeId == rr.StoreId);
            if (customerId > 0)
                query = query.Where(rr => customerId == rr.CustomerId);


            if (orderItemId > 0)
                query = query.Where(rr => rr.OrderItemId == orderItemId);

            if (!string.IsNullOrEmpty(customNumber))
                query = query.Where(rr => rr.CustomNumber == customNumber);

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            if (groupId > 0)
                query = query.Where(rr => rr.GroupReturnRequestId == groupId);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = new PagedList<ReturnRequest>(query, pageIndex, pageSize, getOnlyTotalCount);

            return returnRequests;
        }

        public IPagedList<GroupReturnRequest> SearchGroupReturnRequests(DateTime? createdFromUtc = null, int customerId = 0,
            ApproveStatusEnum? ap = null, DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, int vendorId = 0)
        {
            //var query = _groupReturnRequestRepository.Table;

            var query = (from gr in _groupReturnRequestRepository.Table
                         join rr in _returnRequestRepository.Table on gr.Id equals rr.GroupReturnRequestId
                         join oi in _orderItemRepository.Table on rr.OrderItemId equals oi.Id
                         join pr in _productRepository.Table on oi.ProductId equals pr.Id
                         where pr.VendorId == vendorId
                         select gr).Distinct();

            var rec = query.ToList();

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            if (customerId != 0)
                query = query.Where(rr => customerId == rr.CustomerId);

            if (ap.HasValue)
            {
                var approveStatusId = (int)ap.Value;
                query = query.Where(rr => rr.ApproveStatusId == approveStatusId);
            }

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = new PagedList<GroupReturnRequest>(query, pageIndex, pageSize, getOnlyTotalCount);

            return returnRequests;
        }

        public IPagedList<GroupReturnRequest> SearchCustomerGroupReturnRequests(int storeId = 0, int customerId = 0, DateTime? createdFromUtc = null,
            ApproveStatusEnum? ap = null, DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, int vendorId = 0)
        {
            //var query = _groupReturnRequestRepository.Table;

            var query = (from gr in _groupReturnRequestRepository.Table
                         join rr in _returnRequestRepository.Table on gr.Id equals rr.GroupReturnRequestId
                         join oi in _orderItemRepository.Table on rr.OrderItemId equals oi.Id
                         join pr in _productRepository.Table on oi.ProductId equals pr.Id
                         where rr.CustomerId == customerId
                         select gr).Distinct();

            var rec = query.ToList();

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            if (ap.HasValue)
            {
                var approveStatusId = (int)ap.Value;
                query = query.Where(rr => rr.ApproveStatusId == approveStatusId);
            }

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = new PagedList<GroupReturnRequest>(query, pageIndex, pageSize, getOnlyTotalCount);

            return returnRequests;
        }

        public IPagedList<ReturnOrder> SearchReturnOrder(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _returnOrderRepository.Table;

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnOrders = new PagedList<ReturnOrder>(query, pageIndex, pageSize, getOnlyTotalCount);

            return returnOrders;
        }

        public IPagedList<Dispute> SearchDispute(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int orderId = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _disputeRepository.Table;

            if (orderId > 0)
                query = query.Where(rr => orderId == rr.OrderId);

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var disputeList = new PagedList<Dispute>(query, pageIndex, pageSize, getOnlyTotalCount);

            return disputeList;
        }

        public virtual IList<ReturnRequest> SearchReturnRequestList(int storeId = 0, int customerId = 0,
    int orderItemId = 0, string customNumber = "", DateTime? createdFromUtc = null,
    DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _returnRequestRepository.Table;
            if (storeId > 0)
                query = query.Where(rr => storeId == rr.StoreId);
            if (customerId > 0)
                query = query.Where(rr => customerId == rr.CustomerId);

            if (orderItemId > 0)
                query = query.Where(rr => rr.OrderItemId == orderItemId);

            if (!string.IsNullOrEmpty(customNumber))
                query = query.Where(rr => rr.CustomNumber == customNumber);

            if (createdFromUtc.HasValue)
                query = query.Where(rr => createdFromUtc.Value <= rr.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(rr => createdToUtc.Value >= rr.CreatedOnUtc);

            query = query.OrderByDescending(rr => rr.CreatedOnUtc).ThenByDescending(rr => rr.Id);

            var returnRequests = new PagedList<ReturnRequest>(query, pageIndex, pageSize, getOnlyTotalCount);

            return returnRequests;
        }

        /// <summary>
        /// Delete a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public virtual void DeleteReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException(nameof(returnRequestAction));

            _returnRequestActionRepository.Delete(returnRequestAction);

            //event notification
            _eventPublisher.EntityDeleted(returnRequestAction);
        }

        /// <summary>
        /// Gets all return request actions
        /// </summary>
        /// <returns>Return request actions</returns>
        public virtual IList<ReturnRequestAction> GetAllReturnRequestActions()
        {
            var query = from rra in _returnRequestActionRepository.Table
                        orderby rra.DisplayOrder, rra.Id
                        select rra;

            return query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopOrderDefaults.ReturnRequestActionAllCacheKey));
        }

        /// <summary>
        /// Gets a return request action
        /// </summary>
        /// <param name="returnRequestActionId">Return request action identifier</param>
        /// <returns>Return request action</returns>
        public virtual ReturnRequestAction GetReturnRequestActionById(int returnRequestActionId)
        {
            if (returnRequestActionId == 0)
                return null;

            return _returnRequestActionRepository.ToCachedGetById(returnRequestActionId);
        }

        /// <summary>
        /// Inserts a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual void InsertReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            _returnRequestRepository.Insert(returnRequest);

            //event notification
            _eventPublisher.EntityInserted(returnRequest);
        }

        public virtual ReturnOrderItem GetReturnOrderItemById(int id)
        {
            if (id == 0)
                return null;

            return _returnOrderItemRepository.ToCachedGetById(id);
        }

        public virtual ReturnOrder GetReturnOrderById(int id)
        {
            if (id == 0)
                return null;

            return _returnOrderRepository.ToCachedGetById(id);
        }
        public virtual IList<ReturnOrder> GetReturnOrderByGroupReturnRequestId(int groupId)
        {
            if (groupId == 0)
                return new List<ReturnOrder>();

            var query = from pp in _returnOrderRepository.Table
                        where pp.GroupReturnRequestId == groupId
                        orderby pp.Id
                        select pp;

            var returnOrders = query.ToList();

            return returnOrders;
        }

        public virtual void InsertReturnOrder(ReturnOrder returnOrder)
        {
            if (returnOrder == null)
                throw new ArgumentNullException(nameof(returnOrder));

            returnOrder.CreatedOnUtc = DateTime.UtcNow;
            _returnOrderRepository.Insert(returnOrder);

            //event notification
            _eventPublisher.EntityInserted(returnOrder);
        }

        public virtual void InsertReturnOrderItem(ReturnOrderItem returnOrderItem)
        {
            if (returnOrderItem == null)
                throw new ArgumentNullException(nameof(returnOrderItem));

            _returnOrderItemRepository.Insert(returnOrderItem);

            //event notification
            _eventPublisher.EntityInserted(returnOrderItem);
        }

        

        public virtual void InsertSellerDisputePicture(SellerDisputePicture disputePicture)
        {
            if (disputePicture == null)
                throw new ArgumentNullException(nameof(disputePicture));

            _sellerDisputePictureRepository.Insert(disputePicture);

            //event notification
            _eventPublisher.EntityInserted(disputePicture);
        }

        public virtual IList<SellerDisputePicture> GetSellerDisputePictureByGroupReturnRequestId(int groupId)
        {
            var query = from pp in _sellerDisputePictureRepository.Table
                        where pp.GroupReturnRequestId == groupId
                        orderby pp.Id
                        select pp;

            var disputePictures = query.ToList();

            return disputePictures;
        }

        public virtual SellerDisputePicture GetSellerDisputePictureById(int id)
        {
            if (id == 0)
                return null;

            return _sellerDisputePictureRepository.ToCachedGetById(id);
        }

        public virtual IList<Dispute> GetDisputeByGroupReturnRequestId(int groupReturnRequestId)
        {
            if (groupReturnRequestId == 0)
                return null;
            
            var query = from dp in _disputeRepository.Table
                        where dp.GroupReturnRequestId == groupReturnRequestId
                        select dp;

            return query.ToList();
        }
        
        public virtual Dispute GetDisputeById(int id)
        {
            if (id == 0)
                return null;

            return _disputeRepository.GetById(id);
        }
        /// <summary>
        /// Inserts a return request
        /// </summary>
        /// <param name="Dispute">Return request</param>
        public virtual void InsertDispute(Dispute dispute)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            _disputeRepository.Insert(dispute);

            //event notification
            _eventPublisher.EntityInserted(dispute);
        }
        public virtual void UpdateDispute(Dispute dispute)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            _disputeRepository.Update(dispute);

            //event notification
            _eventPublisher.EntityUpdated(dispute);
        }

        public virtual void ResetDisputeAction(Dispute dispute)
        {
            if (dispute == null)
                throw new ArgumentNullException(nameof(dispute));

            dispute.DisputeAction = (int)DisputeActionEnum.Pending;
            dispute.PartialAmount = 0;
            dispute.RaiseClaim = false;

            UpdateDispute(dispute);
        }

        public virtual void InsertGroupReturnRequest(GroupReturnRequest groupReturnRequest)
        {
            if (groupReturnRequest == null)
                throw new ArgumentNullException(nameof(groupReturnRequest));

            _groupReturnRequestRepository.Insert(groupReturnRequest);

            //event notification
            _eventPublisher.EntityInserted(groupReturnRequest);
        }
        /// <summary>
        /// Inserts a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public virtual void InsertReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException(nameof(returnRequestAction));

            _returnRequestActionRepository.Insert(returnRequestAction);

            //event notification
            _eventPublisher.EntityInserted(returnRequestAction);
        }

        /// <summary>
        /// Updates the return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        public virtual void UpdateReturnRequest(ReturnRequest returnRequest)
        {
            if (returnRequest == null)
                throw new ArgumentNullException(nameof(returnRequest));

            returnRequest.UpdatedOnUtc =DateTime.UtcNow;
            _returnRequestRepository.Update(returnRequest);

            //event notification
            _eventPublisher.EntityUpdated(returnRequest);
        }
        public virtual void UpdateReturnOrder(ReturnOrder returnOrder)
        {
            if (returnOrder == null)
                throw new ArgumentNullException(nameof(returnOrder));

            _returnOrderRepository.Update(returnOrder);

            //event notification
            _eventPublisher.EntityUpdated(returnOrder);
        }

        public virtual void UpdateGroupReturnRequest(GroupReturnRequest groupReturnRequest)
        {
            if (groupReturnRequest == null)
                throw new ArgumentNullException(nameof(groupReturnRequest));
            
            groupReturnRequest.UpdatedOnUtc = DateTime.UtcNow;
            groupReturnRequest.UpdatedById = _workContext.CurrentCustomer.Id;

            _groupReturnRequestRepository.Update(groupReturnRequest);

            //event notification
            _eventPublisher.EntityUpdated(groupReturnRequest);
        }

        /// <summary>
        /// Updates the return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        public virtual void UpdateReturnRequestAction(ReturnRequestAction returnRequestAction)
        {
            if (returnRequestAction == null)
                throw new ArgumentNullException(nameof(returnRequestAction));

            _returnRequestActionRepository.Update(returnRequestAction);

            //event notification
            _eventPublisher.EntityUpdated(returnRequestAction);
        }

        /// <summary>
        /// Delete a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public virtual void DeleteReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException(nameof(returnRequestReason));

            _returnRequestReasonRepository.Delete(returnRequestReason);

            //event notification
            _eventPublisher.EntityDeleted(returnRequestReason);
        }

        public virtual void DeleteSellerDisputePicture(SellerDisputePicture picture)
        {
            if (picture == null)
                throw new ArgumentNullException(nameof(picture));

            _sellerDisputePictureRepository.Delete(picture);

            //event notification
            _eventPublisher.EntityDeleted(picture);
        }

        /// <summary>
        /// Gets all return request reasons
        /// </summary>
        /// <returns>Return request reasons</returns>
        public virtual IList<ReturnRequestReason> GetAllReturnRequestReasons()
        {
            var query = from rra in _returnRequestReasonRepository.Table
                        orderby rra.DisplayOrder, rra.Id
                        select rra;

            return query.ToCachedList(_cacheKeyService.PrepareKeyForDefaultCache(NopOrderDefaults.ReturnRequestReasonAllCacheKey));
        }

        /// <summary>
        /// Gets a return request reason
        /// </summary>
        /// <param name="returnRequestReasonId">Return request reason identifier</param>
        /// <returns>Return request reason</returns>
        public virtual ReturnRequestReason GetReturnRequestReasonById(int returnRequestReasonId)
        {
            if (returnRequestReasonId == 0)
                return null;

            return _returnRequestReasonRepository.ToCachedGetById(returnRequestReasonId);
        }

        /// <summary>
        /// Inserts a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public virtual void InsertReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException(nameof(returnRequestReason));

            _returnRequestReasonRepository.Insert(returnRequestReason);

            //event notification
            _eventPublisher.EntityInserted(returnRequestReason);
        }

        /// <summary>
        /// Updates the  return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        public virtual void UpdateReturnRequestReason(ReturnRequestReason returnRequestReason)
        {
            if (returnRequestReason == null)
                throw new ArgumentNullException(nameof(returnRequestReason));

            _returnRequestReasonRepository.Update(returnRequestReason);

            //event notification
            _eventPublisher.EntityUpdated(returnRequestReason);
        }

        /// <summary>
        /// Inserts a return rquest picture
        /// </summary>
        /// <param name="returnRequestImage">va sample product picture</param>
        public virtual void InsertReturnRequestImage(ReturnRequestImage rrImage)
        {
            if (rrImage == null)
                throw new ArgumentNullException(nameof(rrImage));

            _returnRequestImageRepository.Insert(rrImage);

            //event notification
            _eventPublisher.EntityInserted(rrImage);
        }
        
        public IList<GroupReturnRequest> GetNotProcessedReturnRequests(DateTime olderThan)
        {
            var returnRequests = from r in _returnRequestRepository.Table
                join g in _groupReturnRequestRepository.Table on r.GroupReturnRequestId equals g.Id
                join dr in _disputeRepository.Table on g.Id equals dr.GroupReturnRequestId into dre
                from d in dre.DefaultIfEmpty()
                where g.ApproveStatusId == (int)ApproveStatusEnum.Pending
                      && d.Id == null
                      && g.CreatedOnUtc < olderThan
                select g;

            return returnRequests.ToList();
        }

        public IList<GroupReturnRequest> GetGroupReturnRequestByOrderId(int orderId)
        {
            var returnRequests = (from g in _groupReturnRequestRepository.Table
                                  join r in _returnRequestRepository.Table on g.Id equals r.GroupReturnRequestId
                                  join oi in _orderItemRepository.Table on r.OrderItemId equals oi.Id
                                  join o in _orderRepository.Table on oi.OrderId equals o.Id
                                  where o.Id == orderId
                                  select g).Distinct();

            return returnRequests.ToList();
        }


        public virtual bool Approve(GroupReturnRequest groupReturnRequest)
        {
            if (groupReturnRequest.ApproveStatus != ApproveStatusEnum.Pending && groupReturnRequest.ApproveStatus != ApproveStatusEnum.InDispute)
                return false;

            if (groupReturnRequest.ApproveStatus == ApproveStatusEnum.Approved)
                return false;
            
            groupReturnRequest.ApproveStatus = ApproveStatusEnum.Approved;
            groupReturnRequest.ApprovalDateUtc = DateTime.UtcNow;
            UpdateGroupReturnRequest(groupReturnRequest);

            return true;
        }

        public void InsertReturnOrderItems(IEnumerable<ReturnOrderItem> rOrderItems)
        {
            _returnOrderItemRepository.Insert(rOrderItems);
        }

        public virtual void Decline(GroupReturnRequest groupReturnRequest, Order order)
        {
            groupReturnRequest.ApproveStatus = ApproveStatusEnum.NotApproved;
            UpdateGroupReturnRequest(groupReturnRequest);
            
            _eventPublisher.Publish(new ReturnRequestDeclinedEvent(groupReturnRequest));
        }

        public virtual void ProcessRefund(GroupReturnRequest groupReturnRequest)
        {
            groupReturnRequest.ReturnConditionId = (int)ReturnConditionEnum.Mint;
            _eventPublisher.Publish(new ReturnRequestApprovedEvent(groupReturnRequest));

            UpdateGroupReturnRequest(groupReturnRequest);
        }

        public virtual void UpdateApproveStatusToDispute(int groupReturnRequestId)
        {
            var groupReturn = GetGroupReturnRequestById(groupReturnRequestId);

            groupReturn.ApproveStatusId = (int)ApproveStatusEnum.InDispute;
            UpdateGroupReturnRequest(groupReturn);
        }
        public virtual void RaiseDisputeOnDefect(int groupReturnRequstId, int orderId, DisputeTypeEnum disputeType)
        {
            var dispute = new Dispute();
            var disputeList = GetDisputeByGroupReturnRequestId(groupReturnRequstId);

            if (disputeType == DisputeTypeEnum.RejectRefund)
            {
                UpdateApproveStatusToDispute(groupReturnRequstId);
                dispute.DisputeReasonId = (int)DisputeReasonEnum.InvalidReasonFromBuyer;
            }
            else if (disputeType == DisputeTypeEnum.DefectReturn)
            {
                UpdateApproveStatusToDispute(groupReturnRequstId);
                dispute.IsReturnDispute = true;
                dispute.DisputeReasonId = (int)DisputeReasonEnum.IncompleteDamagedReturn;
            }
                

            dispute.DisputeAction = (int)DisputeActionEnum.Pending;
            dispute.OrderId = orderId;
            dispute.VendorId = _workContext.CurrentVendor.Id;
            dispute.UpdatedOnUtc = DateTime.UtcNow;
            dispute.GroupReturnRequestId = groupReturnRequstId;

            if (disputeList.Count > 0)
            {
                disputeList[0].IsReturnDispute = dispute.IsReturnDispute;
                disputeList[0].DisputeReasonId = dispute.DisputeReasonId;
                UpdateDispute(disputeList[0]);
                ResetDisputeAction(disputeList[0]);
            }
            else
            {
                dispute.CreatedOnUtc = DateTime.UtcNow;
                InsertDispute(dispute);
            }

                
        }

        public string GetReturnRequestStatus(GroupReturnRequest returnRequest)
        {
            var approveStatus = "";

            if (returnRequest.ApproveStatus == ApproveStatusEnum.Pending)
                approveStatus = ShuqReturnRequestDefaults.Pending;
            else if (returnRequest.ApproveStatus == ApproveStatusEnum.InDispute)
                approveStatus = ShuqReturnRequestDefaults.InDispute;
            else if (returnRequest.NeedReturnShipping == true && returnRequest.ReturnConditionId == (int)ReturnConditionEnum.Pending)
                approveStatus = ShuqReturnRequestDefaults.Return;
            else if (returnRequest.ApproveStatus == ApproveStatusEnum.Approved)
                approveStatus = ShuqReturnRequestDefaults.Approved;
            else if (returnRequest.ApproveStatus == ApproveStatusEnum.NotApproved)
                approveStatus = ShuqReturnRequestDefaults.Rejected;

            return approveStatus;
        }
        #endregion

        #region Private Methods



        #endregion
    }
}