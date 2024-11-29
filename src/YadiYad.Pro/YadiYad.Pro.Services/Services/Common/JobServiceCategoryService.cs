using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;

namespace YadiYad.Pro.Services.Common
{
    public class JobServiceCategoryService
    {
        #region Fields
        private readonly IRepository<JobServiceCategory> _JobServiceCategoryRepository;
        private readonly IRepository<Expertise> _ExpertiseRepository;

        #endregion

        #region Ctor

        public JobServiceCategoryService
            (IRepository<JobServiceCategory> JobServiceCategoryRepository,
            IRepository<Expertise> ExpertiseRepository)
        {
            _JobServiceCategoryRepository = JobServiceCategoryRepository;
            _ExpertiseRepository = ExpertiseRepository;

        }

        #endregion


        #region Methods

        public virtual IPagedList<JobServiceCategory> GetAllJobServiceCategoris(int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _JobServiceCategoryRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()) && n.Published == true);

            query = query.OrderBy(n => n.Name);

            var news = new PagedList<JobServiceCategory>(query, pageIndex, pageSize);

            return news;
        }

        public virtual IPagedList<JobServiceCategory> SearchJobServiceCategories(string categoryName,
           int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _JobServiceCategoryRepository.Table;

            if (!string.IsNullOrEmpty(categoryName))
                query = query.Where(n => n.Name.ToLower().Contains(categoryName.ToLower()));

            query = query.OrderBy(n => n.Name);

            var news = new PagedList<JobServiceCategory>(query, pageIndex, pageSize);

            return news;
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual JobServiceCategory GetCategoryById(int categoryId)
        {
            if (categoryId == 0)
                return null;

            return _JobServiceCategoryRepository.GetById(categoryId);
        }

        public virtual List<JobServiceCategory> GetCategoriesByIds(int[] categoryIds)
        {
            if (categoryIds == null || categoryIds.Length == 0)
                return new List<JobServiceCategory>();

            var query = from p in _JobServiceCategoryRepository.Table
                        where categoryIds.Contains(p.Id)
                        select p;

            return query.ToList();
        }

        //Update
        public virtual void UpdateCategory(JobServiceCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _JobServiceCategoryRepository.Update(category);
        }

        public virtual void InsertCategory(JobServiceCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _JobServiceCategoryRepository.Insert(category);
        }

        public virtual void DeleteCategories(IList<JobServiceCategory> categories)
        {
            if (categories == null)
                throw new ArgumentNullException(nameof(categories));

            foreach (var category in categories)
            {
                DeleteCategory(category);
            }
        }

        public virtual void DeleteCategory(JobServiceCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            _JobServiceCategoryRepository.Delete(category);
        }

        public virtual List<JobServiceCategoryExpertise> GetAllJobServiceCategorisExpertise()
        {
            var categories = (from p in _JobServiceCategoryRepository.Table
                              select new JobServiceCategoryExpertise
                              {
                                  Id = p.Id,
                                  Name = p.Name,
                                  ImageUrl = p.ImageUrl
                              }).ToList();
            var expertiseQuery = _ExpertiseRepository.Table.Where(x => x.Published == true).ToList();

            foreach (var cat in categories)
            {
                cat.Expertises = expertiseQuery.Where(x => x.JobServiceCategoryId == cat.Id).ToList();
            };
            return categories;
        }

        #endregion
    }
}
