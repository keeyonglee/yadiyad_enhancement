using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog
{
    public partial class HomeCategoryModel : BaseNopEntityModel
    {
        public HomeCategoryModel()
        {
            EatsCategoryList = new List<CategoryModel>();
            MartCategoryList = new List<CategoryModel>();
        }

        public IList<CategoryModel> EatsCategoryList { get; set; }
        public IList<CategoryModel> MartCategoryList { get; set; }


		
    }
}