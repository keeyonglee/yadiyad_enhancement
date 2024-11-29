using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Category
{
    public class CategorySearchModel : BaseSearchModel
    {
        #region Ctor

        public CategorySearchModel()
        {
            AvailableStores = new List<SelectListItem>();
            AvailablePublishedOptions = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchPublished")]
        public int SearchPublishedId { get; set; }

        public IList<SelectListItem> AvailablePublishedOptions { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Categories.List.SearchStore")]
        public int SearchStoreId { get; set; }

        public IList<SelectListItem> AvailableStores { get; set; }

        public bool HideStoresList { get; set; }

        #endregion
    }
}
