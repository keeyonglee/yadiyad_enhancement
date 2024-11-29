using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Domains;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface ICategoryIconService
    {
        void DeleteCategoryIcon(ApiCategoryIcon categoryIcon);

        void InsertCategoryIcon(ApiCategoryIcon categoryIcon);

        void UpdateCategoryIcon(ApiCategoryIcon categoryIcon);

        ApiCategoryIcon GetCategoryIconById(int categoryIconId);

        IList<ApiCategoryIcon> GetCategoryIconByIds(int[] categoryIconIds);

        ApiCategoryIcon GetCategoryIconByCategoryId(int categoryId);

        IPagedList<ApiCategoryIcon> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue);

        void DeleteCategoryIcons(List<ApiCategoryIcon> categoryIcons);
    }
}