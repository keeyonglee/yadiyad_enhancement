using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.NopStation.WebApi.Models.Catalog
{
    public class HomepageCategoryModel : BaseNopEntityModel
    {
        public HomepageCategoryModel()
        {
            SubCategories = new List<HomepageCategoryModel>();
            Products = new List<ProductOverviewModel>();
        }

        public string Name { get; set; }

        public string SeName { get; set; }

        public IList<HomepageCategoryModel> SubCategories { get; set; }

        public IList<ProductOverviewModel> Products { get; set; }
    }
}
