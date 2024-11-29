using Nop.Core;
using Nop.Core.Domain.News;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YadiYad.Pro.Services.Services.Common
{
    public class NewsProService
    {
        #region Fields

        private readonly IRepository<NewsItem> _NewsRepository;

        #endregion

        #region Ctor

        public NewsProService(IRepository<NewsItem> NewsRepository)
        {
            _NewsRepository = NewsRepository;
        }

        #endregion

        #region Methods

        public virtual IPagedList<NewsItem> GetAll(
        int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _NewsRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Title.ToLower().Contains(keyword.ToLower()));

            query = query.OrderBy(n => n.Title);

            var data = new PagedList<NewsItem>(query, pageIndex, pageSize);

            return data;
        }

        public virtual IPagedList<NewsItem> SearchYadiyadNews(string title, int newsTypeId, DateTime? startDate,
      int pageIndex = 0, int pageSize = int.MaxValue, string keyword = null)
        {
            var query = _NewsRepository.Table;

            if (!string.IsNullOrEmpty(title))
                query = query.Where(n => n.Title.ToLower().Contains(title.ToLower()));

            if (newsTypeId > 0)
                query = query.Where(n => n.NewsTypeId == newsTypeId);

            if (startDate.HasValue)
                query = query.Where(n => startDate.Value >= n.StartDateUtc);

            query = query.OrderBy(n => n.Title);

            var data = new PagedList<NewsItem>(query, pageIndex, pageSize);

            return data;
        }

        /// <summary>
        /// Gets a category
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <returns>Category</returns>
        public virtual NewsItem GetById(int id)
        {
            if (id == 0)
                return null;

            return _NewsRepository.GetById(id);
        }

        public virtual List<NewsItem> GetManyByIds(int[] ids)
        {
            if (ids == null)
                return new List<NewsItem>();

            var query = from p in _NewsRepository.Table
                        where ids.Contains(p.Id)
                        select p;

            return query.ToList();
        }

        public virtual void Update(NewsItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _NewsRepository.Update(item);
        }

        public virtual void Insert(NewsItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _NewsRepository.Insert(item);
        }

        public virtual void DeleteMany(IList<NewsItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var i in items)
            {
                _NewsRepository.Delete(i);
            }
        }

        public virtual void Delete(NewsItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _NewsRepository.Delete(item);
        }

        #endregion

    }
}
