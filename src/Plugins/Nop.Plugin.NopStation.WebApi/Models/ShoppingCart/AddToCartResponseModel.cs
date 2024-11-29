using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Models.ShoppingCart
{
    public class AddToCartResponseModel
    {
        public int TotalShoppingCartProductsQuantity { get; set; }
        public int TotalShoppingCartProducts { get; set; }

        public int TotalWishListProducts { get; set; }

        public bool RedirectToDetailsPage { get; set; }

        public bool RedirectToWishListPage { get; set; }

        public bool RedirectToShoppingCartPage { get; set; }
    }
}
