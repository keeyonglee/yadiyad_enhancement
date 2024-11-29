using Nop.Services.Orders;
using Nop.Core.Domain.Orders;

namespace Nop.Services.Payments
{
    public partial class PostProcessPaymentRequest
    {
        public ICustomOrderEntity CustomOrder { get; set; }
    }
}
