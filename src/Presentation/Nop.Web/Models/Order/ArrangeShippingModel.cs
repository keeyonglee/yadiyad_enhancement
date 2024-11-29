using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Order
{
    public partial class ArrangeShippingModel : BaseNopModel
    {
        public int OrderId { get; set; }
        public bool RequireInsurance { get; set; }
    }
}
