using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class ProductOverviewModel : BaseNopEntityModel
    {
        public ProductOverviewModel()
        {
            ProductPrice = new ProductPriceModel();
            Address = new ProductAddressModel();
            DefaultPictureModel = new PictureModel();
            SpecificationAttributeModels = new List<ProductSpecificationModel>();
            ReviewOverviewModel = new ProductReviewOverviewModel();
            ProductSpecifications = new List<ProductSpecificationModel>();
        }

        public string Name { get; set; }
        public string VendorName { get; set; }
        public string VendorStateName { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string SeName { get; set; }

        public string Sku { get; set; }

        public ProductType ProductType { get; set; }

        public bool MarkAsNew { get; set; }
        public bool IsEats { get; set; }
        public bool IsMart { get; set; }
        public bool VendorOnline { get; set; }
        public bool PastSeasonal { get; set; }

        //price
        public ProductPriceModel ProductPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //specification attributes
        public IList<ProductSpecificationModel> SpecificationAttributeModels { get; set; }
        //price
        public ProductReviewOverviewModel ReviewOverviewModel { get; set; }
        public string ProductBusinessNature { get; set; }
        public IList<ProductSpecificationModel> ProductSpecifications { get; set; }
        public ProductAddressModel Address { get; set; }

        #region Nested Classes

        public partial class ProductPriceModel : BaseNopModel
        {
            public string OldPrice { get; set; }
            public string Price { get; set; }
            public decimal PriceValue { get; set; }
            /// <summary>
            /// PAngV baseprice (used in Germany)
            /// </summary>
            public string BasePricePAngV { get; set; }

            public bool DisableBuyButton { get; set; }
            public bool DisableWishlistButton { get; set; }
            public bool DisableAddToCompareListButton { get; set; }

            public bool AvailableForPreOrder { get; set; }
            public DateTime? PreOrderAvailabilityStartDateTimeUtc { get; set; }

            public bool IsRental { get; set; }

            public bool ForceRedirectionAfterAddingToCart { get; set; }

            /// <summary>
            /// A value indicating whether we should display tax/shipping info (used in Germany)
            /// </summary>
            public bool DisplayTaxShippingInfo { get; set; }
        }

        public partial class ProductAddressModel
        {
            public Address Address { get; set; }
            public string State { get; set; }
        }
        #endregion
    }
}