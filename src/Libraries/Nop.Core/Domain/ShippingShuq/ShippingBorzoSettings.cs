using Nop.Core.Configuration;

namespace Nop.Core.Domain.ShippingShuq
{
    public class ShippingBorzoSettings : ISettings
    {
        public string BaseUrl { get; set; }
        public string GetQuotation { get; set; }
        public string PlaceOrder { get; set; }
        public string GetOrders { get; set; }
        public decimal MarkUpShippingFeePercentage { get; set; }
        public string ApiSecret { get; set; }
        public string GetClient { get; set; }
        public int Car { get; set; }
        public int Motorcycle { get; set; }
        public int MaxWeightCar { get; set; }
        public int MaxWeightMotorcycle { get; set; }
        public decimal MaxDeliveryFeeCar { get; set; }
        public decimal MaxDeliveryFeeMotorcycle { get; set; }
        public string Matter { get; set; }
        public string VendorUrl { get; set; }
        public string CustomerUrl { get; set; }
        public bool IsClientNotificationEnabled { get; set; }
        public bool IsContactPersonNotificationEnabled { get; set; }
        public string CallbackSecret { get; set; }
    }
}