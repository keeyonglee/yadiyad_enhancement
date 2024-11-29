using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using YadiYad.Pro.Core.Domain.Payout;

namespace Nop.Web.Models.Order
{
    public partial class VendorSummaryModel : BaseNopModel
    {
        public VendorSummaryModel()
        {
            Orders = new List<OrderModel>();
            Payouts = new List<PayoutModel>();
        }

        public int OrdersAwaitingShip { get; set; }
        public int ReturnOrdersAwatingProcess { get; set; }
        public string PayoutThisCycle { get; set; }
        public IList<OrderModel> Orders { get; set; }
        public IList<PayoutModel> Payouts { get; set; }


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
            public ShippingStatus ShippingStatusEnum { get; set; }
            public DateTime CreatedOn { get; set; }
            public string ReturnRequestApprovalStatus { get; set; }
            public string ShippingMethod { get; set; }
            public string DeliveryDateText { get; set; }
            public string DeliveryTimeText { get; set; }
            public DateTime? DeliveryDateTime { get; set; }
            public DateTime? ShipBeforeDate { get; set; }
        }

        public partial class PayoutModel : BaseNopEntityModel
        {
            public DateTime Date { get; set; }
            public int StatusId { get; set; }
            public string Amount { get; set; }
            public int NumberOfOrders { get; set; }
            public string Remarks { get; set; }
            public string BankName { get; set; }
            public string BankAccount { get; set; }
            public int OrderId { get; set; }
            public int PayoutGroupId { get; set; }
            public string DateText
            {
                get
                {
                    return Date.ToShortDateString();
                }
            }
            public string StatusText
            {
                get
                {
                    var payoutEnum = (PayoutGroupStatus)StatusId;
                    return payoutEnum.ToString();
                }
            }
        }

        #endregion
    }
}
