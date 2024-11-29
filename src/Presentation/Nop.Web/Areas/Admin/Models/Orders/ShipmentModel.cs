using System;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Shipping;

namespace Nop.Web.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment model
    /// </summary>
    public partial class ShipmentModel : BaseNopEntityModel
    {
        #region Ctor

        public ShipmentModel()
        {
            ShipmentStatusEvents = new List<ShipmentStatusEventModel>();
            Items = new List<ShipmentItemModel>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Orders.Shipments.ID")]
        public override int Id { get; set; }

        public int OrderId { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.TotalWeight")]
        public string TotalWeight { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.TrackingNumber")]
        public string TrackingNumber { get; set; }
        public bool RequireBarCode { get; set; }
        public bool RequireVendorSideTracking { get; set; }
        public int ShippingStatusId { get; set; }

        public string TrackingNumberUrl { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.ShippedDate")]
        public string ShippedDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CanShip")]
        public bool CanShip { get; set; }

        public DateTime? ShippedDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.DeliveryDate")]
        public string DeliveryDate { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.CanDeliver")]
        public bool CanDeliver { get; set; }

        public DateTime? DeliveryDateUtc { get; set; }

        [NopResourceDisplayName("Admin.Orders.Shipments.AdminComment")]
        public string AdminComment { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.RequireInsurance")]
        public bool RequireInsurance { get; set; }
        [NopResourceDisplayName("Admin.Orders.Shipments.GoodsValue")]
        public decimal GoodsValue { get; set; }
        public string Remarks { get; set; }
        public List<ShipmentItemModel> Items { get; set; }

        public IList<ShipmentStatusEventModel> ShipmentStatusEvents { get; set; }
        public string VendorBusinessNature { get; set; }

        public string ShippingStatusText
        {
            get
            {
                return ((ShippingStatus)ShippingStatusId).ToString();
            }
        }

        #endregion
    }
}