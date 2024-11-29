using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public interface ICategoryIconModelFactory
    {
        CategoryIconSearchModel PrepareCategoryIconSearchModel(CategoryIconSearchModel searchModel);

        CategoryIconListModel PrepareCategoryIconListModel(CategoryIconSearchModel searchModel);

        CategoryIconModel PrepareCategoryIconModel(CategoryIconModel model, Category category);
    }
}