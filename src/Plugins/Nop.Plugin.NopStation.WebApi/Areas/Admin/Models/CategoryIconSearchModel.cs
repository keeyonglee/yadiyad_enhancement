using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Models
{
    public class CategoryIconSearchModel : BaseSearchModel
    {
        public CategoryIconSearchModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.List.SearchCategoryName")]
        public string SearchCategoryName { get; set; }

        [NopResourceDisplayName("Admin.NopStation.WebApi.CategoryIcons.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public bool HideStoresList { get; set; }
    }
}
