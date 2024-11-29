using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.WebApi.Models.ShoppingCart
{
    public class CheckoutAttributeChangeModel
    {
        public CheckoutAttributeChangeModel()
        {
            EnabledAttributeIds = new List<int>();
            DisabledAttributeIds = new List<int>();
            OrderTotals = new OrderTotalsModel();
        }

        public ShoppingCartModel Cart { get; set; }

        public OrderTotalsModel OrderTotals { get; set; }

        public string SelectedCheckoutAttributess { get; set; }

        public IList<int> EnabledAttributeIds { get; set; }

        public IList<int> DisabledAttributeIds { get; set; }
    }
}
