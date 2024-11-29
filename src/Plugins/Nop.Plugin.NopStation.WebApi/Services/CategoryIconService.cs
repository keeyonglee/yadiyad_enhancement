using Nop.Core;
using Nop.Data;
using Nop.Plugin.NopStation.WebApi.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public class CategoryIconService : ICategoryIconService
    {
        #region Fields

        private readonly IRepository<ApiCategoryIcon> _categoryIconRepository;

        #endregion

        #region Ctor

        public CategoryIconService(IRepository<ApiCategoryIcon> categoryIconRepository)
        {
            _categoryIconRepository = categoryIconRepository;
        }

        #endregion

        #region Methods

        public void DeleteCategoryIcon(ApiCategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconRepository.Delete(categoryIcon);
        }

        public void InsertCategoryIcon(ApiCategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconRepository.Insert(categoryIcon);
        }

        public void UpdateCategoryIcon(ApiCategoryIcon categoryIcon)
        {
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconRepository.Update(categoryIcon);
        }

        public ApiCategoryIcon GetCategoryIconById(int categoryIconId)
        {
            if (categoryIconId == 0)
                return null;

            return _categoryIconRepository.GetById(categoryIconId);
        }

        public ApiCategoryIcon GetCategoryIconByCategoryId(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return _categoryIconRepository.Table.FirstOrDefault(x => x.CategoryId == categoryId);
        }

        public IPagedList<ApiCategoryIcon> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var categoryIcons = _categoryIconRepository.Table;

            categoryIcons = categoryIcons.OrderByDescending(e => e.Id);

            return new PagedList<ApiCategoryIcon>(categoryIcons, pageIndex, pageSize);
        }

        public IList<ApiCategoryIcon> GetCategoryIconByIds(int[] categoryIconIds)
        {
            if (categoryIconIds == null || categoryIconIds.Length == 0)
                return new List<ApiCategoryIcon>();

            var query = _categoryIconRepository.Table.Where(x => categoryIconIds.Contains(x.Id));

            return query.ToList();
        }

        public void DeleteCategoryIcons(List<ApiCategoryIcon> categoryIcons)
        {
            if (categoryIcons == null)
                throw new ArgumentNullException(nameof(categoryIcons));

            _categoryIconRepository.Delete(categoryIcons);
        }

        #endregion
    }
}
