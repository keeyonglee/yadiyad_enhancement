namespace Nop.Core.Domain.ShippingShuq.DTO
{
    public class ShipmentDetailDTO
    {
        public CarrierShippingStatus CurrentStatus { get; set; }
        public decimal ShippingCost { get; set; }
    }
}