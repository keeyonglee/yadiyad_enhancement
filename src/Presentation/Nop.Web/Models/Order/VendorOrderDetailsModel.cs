using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Common;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Order
{
    public partial class VendorOrderDetailsModel
    {
        public OrderModel Order { get; set; }
        public GroupReturnRequestModel GroupReturnRequest { get; set; }

        public partial class OrderModel
        {
            public string CustomOrderNumber { get; set; }
            public string OrderTotal { get; set; }
            public bool IsReturnRequestAllowed { get; set; }
            public bool IsCancelOrderAllowed { get; set; }
            public bool IsOrderReceiveAllowed { get; set; }
            public OrderStatus OrderStatusEnum { get; set; }
            public string OrderStatus { get; set; }
            public string PaymentStatus { get; set; }
            public string ShippingStatus { get; set; }
            public string ShippingMethod { get; set; }
            public DateTime CreatedOn { get; set; }
            public OrderDetailsModel OrderDetails { get; set; }
            public IList<VendorOrderModel> Vendors { get; set; }
        }

        public partial class OrderDetailsModel : BaseNopEntityModel
        {
            public OrderDetailsModel()
            {
                TaxRates = new List<TaxRate>();
                GiftCards = new List<GiftCard>();
                OrderNotes = new List<OrderNote>();
                Shipments = new List<ShipmentBriefModel>();

                BillingAddress = new AddressModel();
                ShippingAddress = new AddressModel();
                PickupAddress = new AddressModel();

                CustomValues = new Dictionary<string, object>();
            }

            public bool PrintMode { get; set; }
            public bool PdfInvoiceDisabled { get; set; }

            public string CustomOrderNumber { get; set; }

            public DateTime CreatedOn { get; set; }

            public string OrderStatus { get; set; }

            public bool IsReOrderAllowed { get; set; }

            public bool IsReturnRequestAllowed { get; set; }
            public bool IsCancelOrderAllowed { get; set; }
            public bool IsReadyToReceive { get; set; }

            public bool NeedReturn { get; set; }

            public bool IsShippable { get; set; }
            public bool PickupInStore { get; set; }
            public AddressModel PickupAddress { get; set; }
            public string ReturnRequestApprovalStatus { get; set; }
            public string ShippingStatus { get; set; }
            public AddressModel ShippingAddress { get; set; }
            public string ShippingMethod { get; set; }
            public IList<ShipmentBriefModel> Shipments { get; set; }

            public AddressModel BillingAddress { get; set; }
            public int ReturnShipmentId { get; set; }
            public string VatNumber { get; set; }

            public string PaymentMethod { get; set; }
            public string PaymentMethodStatus { get; set; }
            public bool CanRePostProcessPayment { get; set; }
            public Dictionary<string, object> CustomValues { get; set; }

            public string OrderSubtotal { get; set; }
            public string OrderSubTotalDiscount { get; set; }
            public string OrderShipping { get; set; }
            public string PaymentMethodAdditionalFee { get; set; }
            public string DeliveryDateText { get; set; }
            public string DeliveryTimeText { get; set; }
            public DateTime? DeliveryDateTime { get; set; }
            public string CheckoutAttributeInfo { get; set; }

            public bool PricesIncludeTax { get; set; }
            public bool DisplayTaxShippingInfo { get; set; }
            public string Tax { get; set; }
            public IList<TaxRate> TaxRates { get; set; }
            public bool DisplayTax { get; set; }
            public bool DisplayTaxRates { get; set; }

            public string OrderTotalDiscount { get; set; }
            public int RedeemedRewardPoints { get; set; }
            public string RedeemedRewardPointsAmount { get; set; }
            public string OrderTotal { get; set; }

            public IList<GiftCard> GiftCards { get; set; }

            public bool ShowSku { get; set; }

            public IList<OrderNote> OrderNotes { get; set; }

            public bool ShowVendorName { get; set; }
            public string TrackingNumberUrl { get; set; }
            public bool RequireBarCode { get; set; }
            public DateTime? ShipBeforeDate { get; set; }

            #region Nested Classes

            public partial class TaxRate : BaseNopModel
            {
                public string Rate { get; set; }
                public string Value { get; set; }
            }

            public partial class GiftCard : BaseNopModel
            {
                public string CouponCode { get; set; }
                public string Amount { get; set; }
            }

            public partial class OrderNote : BaseNopEntityModel
            {
                public bool HasDownload { get; set; }
                public string Note { get; set; }
                public DateTime CreatedOn { get; set; }
            }

            public partial class ShipmentBriefModel : BaseNopEntityModel
            {
                public string TrackingNumber { get; set; }
                public string TrackingUrl { get; set; }
                public DateTime? ShippedDate { get; set; }
                public DateTime? DeliveryDate { get; set; }
                public string BarCode { get; set; }
            }

            #endregion
        }

        public partial class VendorOrderModel
        {
            public int VendorId { get; set; }
            public string VendorName { get; set; }
            public PictureModel PictureModel { get; set; }
            public IList<OrderItemModel> OrderItems { get; set; }
        }

        public partial class OrderItemModel : BaseNopEntityModel
        {
            public PictureModel ImageModel { get; set; }
            public Guid OrderItemGuid { get; set; }
            public string Sku { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string ProductSeName { get; set; }
            public string UnitPrice { get; set; }
            public string SubTotal { get; set; }
            public int Quantity { get; set; }
            public string AttributeInfo { get; set; }
            public string RentalInfo { get; set; }
            public bool HasReview { get; set; }
            public string VendorName { get; set; }

            //downloadable product properties
            public int DownloadId { get; set; }
            public int LicenseId { get; set; }
            public int VendorId { get; set; }
            public bool AllowCustomerReviews { get; set; }
        }
    }
}