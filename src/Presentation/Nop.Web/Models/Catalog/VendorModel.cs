using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class VendorModel : BaseNopEntityModel
    {
        public VendorModel()
        {
            PictureModel = new PictureModel();
            Products = new List<ProductOverviewModel>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            VendorAttributes = new List<VendorAttributeModel>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        public bool AllowCustomersToContactVendors { get; set; }

        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }

        public string AboutUs { get; set; }
        public string CategoryName { get; set; }
        public PictureModel AboutUsPictureModel { get; set; }
        public List<PictureModel> SliderPictureModel { get; set; }
        public VendorBreadcrumbModel Breadcrumb { get; set; }
        public List<VendorAttributeModel> VendorAttributes { get; set; }

        public DateTime JoinedDate { get; set; }
        public ProductReviewOverviewModel VendorReviewOverview { get; set; }
        public List<ProductReviewModel> VendorProductReviews { get; set; }

        #region Nested classes
        public partial class VendorBreadcrumbModel : BaseNopModel
        {
            public VendorBreadcrumbModel()
            {
                CategoryBreadcrumb = new List<CategorySimpleModel>();
            }

            public bool Enabled { get; set; }
            public int VendorId { get; set; }
            public string VendorName { get; set; }
            public string VendorSeName { get; set; }
            public IList<CategorySimpleModel> CategoryBreadcrumb { get; set; }
        }

        public partial class VendorAttributeModel : BaseNopEntityModel
        {
            public VendorAttributeModel()
            {
                Values = new List<VendorAttributeValueModel>();
            }

            public string Name { get; set; }

            public bool IsRequired { get; set; }

            /// <summary>
            /// Default value for textboxes
            /// </summary>
            public string DefaultValue { get; set; }

            public AttributeControlType AttributeControlType { get; set; }

            public IList<VendorAttributeValueModel> Values { get; set; }
        }

        public partial class VendorAttributeValueModel : BaseNopEntityModel
        {
            public string Name { get; set; }

            public bool IsPreSelected { get; set; }
        }
        #endregion
    }
}