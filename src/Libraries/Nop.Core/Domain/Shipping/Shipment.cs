using System;
using System.Linq;
using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment
    /// </summary>
    public partial class Shipment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }
        
        /// <summary>
        /// Gets or sets the tracking number of this shipment
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// Gets or sets the total weight of this shipment
        /// It's nullable for compatibility with the previous version of nopCommerce where was no such property
        /// </summary>
        public decimal? TotalWeight { get; set; }

        /// <summary>
        /// Gets or sets the shipped date and time
        /// </summary>
        public DateTime? ShippedDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the delivery date and time
        /// </summary>
        public DateTime? DeliveryDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the entity creation date
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        public decimal Insurance { get; set; }
        public int Type { get; set; }
        public int? ReturnOrderId { get; set; }
        public decimal ShippingTotal { get; set; }
        public int? ShippingMethodId { get; set; }
        public int DeliveryModeId { get; set; } = (int)DeliveryMode.Bike;
        public bool RequireInsurance { get; set; }
        public int RetryCount { get; set; } = 0;
        public string MarketCode { get; set; }
        public string ScheduleAt { get; set; }
        public DeliveryMode DeliveryMode
        {
            get
            {
                var acceptableValues = (int[]) Enum.GetValues(typeof(DeliveryMode));
                return acceptableValues.Contains(DeliveryModeId) ? (DeliveryMode) DeliveryModeId : DeliveryMode.Bike;
            }
            set => DeliveryModeId = (int) value;
        }

        #region Custom Properties

        public ShipmentType ShipmentType 
        {   get => (ShipmentType)Type;

            set => Type = (int)value;
        }

        #endregion
    }
}