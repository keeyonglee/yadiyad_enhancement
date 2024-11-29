using System;
using System.Collections.Generic;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order
{
    public partial class CustomerReturnRequestsModel : BaseNopModel
    {
        public CustomerReturnRequestsModel()
        {
            Items = new List<GroupReturnRequestModel>();
        }

        public IList<GroupReturnRequestModel> Items { get; set; }

        #region Nested classes

        public partial class GroupReturnRequestModel : BaseNopEntityModel
        {
            public IList<ReturnRequestModel> ReturnList { get; set; }
            public int ApproveStatusId { get; set; }
            public string ApproveStatusStr { get; set; }
            public int ReturnRequestStatusId { get; set; }
            public string ReturnRequestStatusStr { get; set; }
            public int CustomerId { get; set; }
            public string CustomerInfo { get; set; }
            public int OrderId { get; set; }
            public string CustomOrderNumber { get; set; }
            public DateTime createdOnUtc { get; set; }
            public DateTime updatedOnUtc { get; set; }
            public int RefundStatusId { get; set; }
            public string RefundStatusStr { get; set; }
            public decimal RefundAmount { get; set; }
            public bool IsInsuranceClaim { get; set; }
            public bool HasInsuranceCover { get; set; }
            public decimal? InsuranceClaimAmt { get; set; }
            public int InsuranceRef { get; set; }
            public DateTime? InsurancePayoutDate { get; set; }
            public bool IsOrderShipped { get; set; }
            public bool IsCategoryValid { get; set; }
            public bool NeedReturnShipping { get; set; }
            public int ReturnConditionId { get; set; }
            public int? OrderRefundRequestId { get; set; }
        }

        #endregion
    }
}