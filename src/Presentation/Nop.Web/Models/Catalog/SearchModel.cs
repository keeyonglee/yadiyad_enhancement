using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Core.Domain.Catalog;

namespace Nop.Web.Models.Catalog
{
    public partial class SearchModel : BaseNopModel
    {
        public SearchModel()
        {
            PagingFilteringContext = new CatalogPagingFilteringModel();
            Products = new List<ProductOverviewModel>();

            AvailableCategories = new List<SelectListItem>();
            AvailableManufacturers = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
        }

        public string Warning { get; set; }

        public bool NoResults { get; set; }

        /// <summary>
        /// Min Rating
        /// </summary>
        [NopResourceDisplayName("Search.MinRating")]
        public string rf { get; set; }

        /// <summary>
        /// Max Rating
        /// </summary>
        [NopResourceDisplayName("Search.MaxRating")]
        public string rt { get; set; }

        /// <summary>
        /// Query string
        /// </summary>
        [NopResourceDisplayName("Search.SearchTerm")]
        public string q { get; set; }

        /// <summary>
        /// Category ID
        /// </summary>
        [NopResourceDisplayName("Search.Category")]
        public int cid { get; set; }

        /// <summary>
        /// Category IDs
        /// </summary>
        [NopResourceDisplayName("Search.Categorys")]
        public List<int> cids { get; set; }

        [NopResourceDisplayName("Search.IncludeSubCategories")]
        public bool isc { get; set; }

        /// <summary>
        /// Manufacturer ID
        /// </summary>
        [NopResourceDisplayName("Search.Manufacturer")]
        public int mid { get; set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        [NopResourceDisplayName("Search.Vendor")]
        public int vid { get; set; }

        /// <summary>
        /// Price - From 
        /// </summary>
        public string pf { get; set; }

        /// <summary>
        /// Price - To 
        /// </summary>
        public string pt { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public string lat { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public string lng { get; set; }

        /// <summary>
        /// A value indicating whether to search in descriptions
        /// </summary>
        [NopResourceDisplayName("Search.SearchInDescriptions")]
        public bool sid { get; set; }

        /// <summary>
        /// A value indicating whether "advanced search" is enabled
        /// </summary>
        [NopResourceDisplayName("Search.AdvancedSearch")]
        public bool adv { get; set; }

        /// <summary>
        /// A value indicating whether "allow search by vendor" is enabled
        /// </summary>
        public bool asv { get; set; }

        /// <summary>
        /// show vendor state
        /// </summary>
        public bool ShowVendorState { get; set; }

        public decimal? CoverageDistance { get; set; }
        public ShuqBusinessNatureEnum Category { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }
        public IList<SelectListItem> AvailableManufacturers { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        public string TopSliderWidgetZone { get; set; }
        public string SideSliderWidgetZone { get; set; }


        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }

        #region Nested classes

        public class CategoryModel : BaseNopEntityModel
        {
            public string Breadcrumb { get; set; }
        }

        #endregion
    }
}