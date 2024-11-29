using YadiYad.Pro.Core.Domain.Order;

namespace YadiYad.Pro.Services.DTO.Payment
{
    /// <summary>
    /// Represents a PostProcessPaymentRequest
    /// </summary>
    public partial class PostProcessPaymentRequestDTO
    {
        /// <summary>
        /// Gets or sets an order. Used when order is already saved (payment gateways that redirect a customer to a third-party URL)
        /// </summary>
        public ProOrder Order { get; set; }
    }
}
