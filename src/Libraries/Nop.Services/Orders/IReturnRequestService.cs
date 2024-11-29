using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Services.Orders
{
    /// <summary>
    /// Return request service interface
    /// </summary>
    public partial interface IReturnRequestService
    {
        /// <summary>
        /// Updates a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        void UpdateReturnRequest(ReturnRequest returnRequest);

        void UpdateGroupReturnRequest(GroupReturnRequest groupReturnRequest);
        /// <summary>
        /// Deletes a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        void DeleteReturnRequest(ReturnRequest returnRequest);
        IList<ReturnOrderItem> GetReturnOrderItemByReturnOrderId(int returnOrderId);
        /// <summary>
        /// Gets a return request
        /// </summary>
        /// <param name="returnRequestId">Return request identifier</param>
        /// <returns>Return request</returns>
        ReturnRequest GetReturnRequestById(int returnRequestId);
        Dispute GetDisputeById(int id);
        void ResetDisputeAction(Dispute dispute);
        GroupReturnRequest GetGroupReturnRequestById(int groupId);
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
        IPagedList<ReturnRequest> SearchReturnRequests(int groupId = 0, int storeId = 0, int customerId = 0,
            int orderItemId = 0, string customNumber = "", DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        IPagedList<ReturnOrder> SearchReturnOrder(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        IPagedList<GroupReturnRequest> SearchCustomerGroupReturnRequests(int storeId = 0, int customerId = 0, DateTime? createdFromUtc = null,
            ApproveStatusEnum? ap = null, DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, int vendorId = 0);

        IPagedList<GroupReturnRequest> SearchGroupReturnRequests(DateTime? createdFromUtc = null, int customerId = 0,
            ApproveStatusEnum? ap = null, DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, int vendorId = 0);
        IList<GroupReturnRequest> GetGroupReturnRequestByOrderId(int orderId);
        IPagedList<Dispute> SearchDispute(DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int orderId = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        IList<ReturnRequest> SearchReturnRequestList(int storeId = 0, int customerId = 0,
            int orderItemId = 0, string customNumber = "", DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);
        /// <summary>
        /// Delete a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        void DeleteReturnRequestAction(ReturnRequestAction returnRequestAction);
        void InsertReturnOrderItem(ReturnOrderItem returnOrderItem);
        void InsertReturnOrder(ReturnOrder returnOrder);
        IList<ReturnOrder> GetReturnOrderByGroupReturnRequestId(int groupId);
        ReturnOrder GetReturnOrderById(int id);
        ReturnOrderItem GetReturnOrderItemById(int id);
        /// <summary>
        /// Gets all return request actions
        /// </summary>
        /// <returns>Return request actions</returns>
        IList<ReturnRequestAction> GetAllReturnRequestActions();
        IList<ReturnRequestImage> GetReturnRequestImageByGroupReturnRequestId(int groupReturnRequestId);
        IList<ReturnRequest> GetReturnRequestByGroupReturnRequestId(int groupId);
        /// <summary>
        /// Gets a return request action
        /// </summary>
        /// <param name="returnRequestActionId">Return request action identifier</param>
        /// <returns>Return request action</returns>
        ReturnRequestAction GetReturnRequestActionById(int returnRequestActionId);

        /// <summary>
        /// Inserts a return request
        /// </summary>
        /// <param name="returnRequest">Return request</param>
        void InsertReturnRequest(ReturnRequest returnRequest);
        void InsertSellerDisputePicture(SellerDisputePicture disputePicture);
        IList<SellerDisputePicture> GetSellerDisputePictureByGroupReturnRequestId(int disputeId);
        IList<Dispute> GetDisputeByGroupReturnRequestId(int groupReturnRequestId);
        SellerDisputePicture GetSellerDisputePictureById(int id);
        void DeleteSellerDisputePicture(SellerDisputePicture picture);
        void InsertDispute(Dispute dispute);
        void UpdateDispute(Dispute dispute);
        void UpdateReturnOrder(ReturnOrder returnOrder);
        void InsertGroupReturnRequest(GroupReturnRequest groupReturnRequest);
        /// <summary>
        /// Inserts a return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        void InsertReturnRequestAction(ReturnRequestAction returnRequestAction);

        /// <summary>
        /// Updates the  return request action
        /// </summary>
        /// <param name="returnRequestAction">Return request action</param>
        void UpdateReturnRequestAction(ReturnRequestAction returnRequestAction);

        /// <summary>
        /// Delete a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        void DeleteReturnRequestReason(ReturnRequestReason returnRequestReason);

        /// <summary>
        /// Gets all return request reasons
        /// </summary>
        /// <returns>Return request reasons</returns>
        IList<ReturnRequestReason> GetAllReturnRequestReasons();

        /// <summary>
        /// Gets a return request reason
        /// </summary>
        /// <param name="returnRequestReasonId">Return request reason identifier</param>
        /// <returns>Return request reason</returns>
        ReturnRequestReason GetReturnRequestReasonById(int returnRequestReasonId);

        /// <summary>
        /// Inserts a return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        void InsertReturnRequestReason(ReturnRequestReason returnRequestReason);

        /// <summary>
        /// Updates the  return request reason
        /// </summary>
        /// <param name="returnRequestReason">Return request reason</param>
        void UpdateReturnRequestReason(ReturnRequestReason returnRequestReason);

        void InsertReturnRequestImage(ReturnRequestImage rrImage);
        string GetReturnRequestStatus(GroupReturnRequest returnRequest);


        IList<GroupReturnRequest> GetNotProcessedReturnRequests(DateTime olderThan);
        bool Approve(GroupReturnRequest groupReturnRequest);
        void InsertReturnOrderItems(IEnumerable<ReturnOrderItem> rOrderItems);
        void ProcessRefund(GroupReturnRequest groupReturnRequest);
        void UpdateApproveStatusToDispute(int groupReturnRequestId);
        void RaiseDisputeOnDefect(int groupReturnRequstId, int orderId, DisputeTypeEnum disputeType);
        IList<Product> GetProducts(GroupReturnRequest groupReturnRequest);
        IList<Category> GetProductCategories(GroupReturnRequest groupReturnRequest);
        IList<GroupReturnRequest> GetPendingInspectionReturns(DateTime cutOffTime);
    }
}
