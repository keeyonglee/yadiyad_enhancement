using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.NopStation.WebApi.Models.Checkout;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.NopStation.WebApi.Factories
{
    public class CartCheckoutApiModelFactory
    {
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ICheckoutModelFactory _checkoutModelFactory;

        public CartCheckoutApiModelFactory(IShoppingCartModelFactory shoppingCartModelFactory,
            ICheckoutModelFactory checkoutModelFactory)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _checkoutModelFactory = checkoutModelFactory;
        }
        
        public virtual CheckoutConfirmOrderModel PrepareCheckoutConfirmOrderModel(IList<ShoppingCartItem> cart)
        {
            return new CheckoutConfirmOrderModel()
            {
                Cart = _shoppingCartModelFactory.PrepareShoppingCartModel(new ShoppingCartModel(), cart, false, false, true),
                Confirm = _checkoutModelFactory.PrepareConfirmOrderModel(cart),
                OrderTotals = _shoppingCartModelFactory.PrepareOrderTotalsModel(cart, false),
                SelectedCheckoutAttributes = _shoppingCartModelFactory.FormatSelectedCheckoutAttributes(),
                EstimateShipping = _shoppingCartModelFactory.PrepareEstimateShippingModel(cart)
            };
        }
        
    }
}