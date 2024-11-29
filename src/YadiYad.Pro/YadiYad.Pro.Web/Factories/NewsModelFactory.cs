using Nop.Core.Domain.News;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.Category;
using YadiYad.Pro.Web.Models.News;

namespace YadiYad.Pro.Web.FactoriesPro
{
    public class NewsModelFactory
    {
        #region Fields

        private readonly NewsProService _newsProService;

        #endregion

        #region Ctor

        public NewsModelFactory(NewsProService newsProService)
        {
            _newsProService = newsProService;
        }

        #endregion


        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual CategorySearchModel PrepareNewsSearchModel(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

               /// <summary>
        /// Prepare paged category list model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category list model</returns>
        public virtual NewsListModel PrepareNewsListModel(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var news = _newsProService.GetAll();

            //prepare grid model
            var model = new NewsListModel().PrepareToGrid(searchModel, news, () =>
            {
                return news.Select(category =>
                {
                    //fill in model values from the entity
                    var newsModel = category.ToModel<NewsModel>();


                    return newsModel;
                });
            });

            return model;
        }

        public virtual NewsModel PrepareNewsModel(NewsModel model, NewsItem expertise, bool excludeProperties = false)
        {
            Action<CategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (expertise != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = expertise.ToModel<NewsModel>();
                }
            }
            return model;
        }
    }
}
