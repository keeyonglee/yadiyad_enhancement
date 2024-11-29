using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.ShoppingCart
{
    public partial class WishlistModel : BaseNopModel
    {
        public WishlistModel()
        {
            Items = new List<ShoppingCartItemModel>();
            Warnings = new List<string>();
        }

        public Guid CustomerGuid { get; set; }
        public string CustomerFullname { get; set; }

        public bool EmailWishlistEnabled { get; set; }

        public bool ShowSku { get; set; }

        public bool ShowProductImages { get; set; }

        public bool IsEditable { get; set; }

        public bool DisplayAddToCart { get; set; }

        public bool DisplayTaxShippingInfo { get; set; }

        public IList<ShoppingCartItemModel> Items { get; set; }

        public IList<string> Warnings { get; set; }


        #region Nested Classes

        public partial class ShoppingCartItemModel : BaseNopEntityModel
        {
            public ShoppingCartItemModel()
            {
                Picture = new PictureModel();
                AllowedQuantities = new List<SelectListItem>();
                Warnings = new List<string>();
                ProductReviewOverview = new ProductReviewOverviewModel();
                Address = new ProductAddressModel();
            }

            public string Sku { get; set; }

            public PictureModel Picture { get; set; }

            public int ProductId { get; set; }

            public string ProductName { get; set; }

            public string ProductSeName { get; set; }

            public string UnitPrice { get; set; }

            public string SubTotal { get; set; }

            public string Discount { get; set; }
            public int? MaximumDiscountedQty { get; set; }

            public int Quantity { get; set; }
            public List<SelectListItem> AllowedQuantities { get; set; }

            public string AttributeInfo { get; set; }

            public string RecurringInfo { get; set; }

            public string RentalInfo { get; set; }

            public bool AllowItemEditing { get; set; }

            public IList<string> Warnings { get; set; }
            public ProductReviewOverviewModel ProductReviewOverview { get; set; }
            public ProductAddressModel Address { get; set; }
        }

        public partial class ProductAddressModel
        {
            public Address Address { get; set; }
            public string State { get; set; }
        }
        #endregion
    }
}