using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;
using Nop.Web.Models.Vendors;
using static Nop.Web.Models.Order.VendorOrderDetailsModel;

namespace Nop.Web.Models.Order
{
    public partial class CustomerOrderListModel : BaseNopModel
    {
        public CustomerOrderListModel()
        {
            Orders = new List<OrderModel>();
            RecurringOrders = new List<RecurringOrderModel>();
            RecurringPaymentErrors = new List<string>();
            GroupReturnRequests = new List<GroupReturnRequestModel>();
        }

        public PagerModel PagerModel { get; set; }
        public IList<OrderModel> Orders { get; set; }
        public IList<RecurringOrderModel> RecurringOrders { get; set; }
        public IList<string> RecurringPaymentErrors { get; set; }
        public IList<GroupReturnRequestModel> GroupReturnRequests { get; set; }


        #region Nested classes

        public partial class OrderModel : BaseNopEntityModel
        {
            public string CustomOrderNumber { get; set; }
            public string OrderTotal { get; set; }
            public bool IsReturnRequestAllowed { get; set; }
            public bool IsCancelOrderAllowed { get; set; }
            public OrderStatus OrderStatusEnum { get; set; }
            public string OrderStatus { get; set; }
            public string PaymentStatus { get; set; }
            public string ShippingStatus { get; set; }
            public DateTime CreatedOn { get; set; }
            public string ReturnRequestApprovalStatus { get; set; }
            public string ReturnRequestStatusStr { get; set; }
            public string ShippingMethod { get; set; }
            public string DeliveryDateText { get; set; }
            public string DeliveryTimeText { get; set; }
            public DateTime? DeliveryDateTime { get; set; }
            public bool IsReadyToReceive { get; set; }
            public DateTime? ShipBeforeDate { get; set; }
            public PictureModel ImageModel { get; set; }
            public VendorModel Vendor { get; set; }
            public VendorOrderModel VendorOrder { get; set; }
        }

        public partial class RecurringOrderModel : BaseNopEntityModel
        {
            public string StartDate { get; set; }
            public string CycleInfo { get; set; }
            public string NextPayment { get; set; }
            public int TotalCycles { get; set; }
            public int CyclesRemaining { get; set; }
            public int InitialOrderId { get; set; }
            public bool CanRetryLastPayment { get; set; }
            public string InitialOrderNumber { get; set; }
            public bool CanCancel { get; set; }
        }

        #endregion
    }
}