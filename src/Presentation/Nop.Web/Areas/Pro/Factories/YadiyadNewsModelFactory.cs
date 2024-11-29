using Nop.Core.Domain.News;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Pro.Models.YadiyadNews;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.Enums;

namespace Nop.Web.Areas.Pro.Factories
{
    public class YadiyadNewsModelFactory
    {
        #region Fields

        private readonly NewsProService _newsProService;


        #endregion

        #region Ctor
        public YadiyadNewsModelFactory(NewsProService newsProService)
        {
            _newsProService = newsProService;
        }
        #endregion
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual YadiyadNewsSearchModel PrepareNewsSearchModel(YadiyadNewsSearchModel searchModel)
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
        public virtual YadiyadNewsListModel PrepareNewsListModel(YadiyadNewsSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var news = _newsProService.SearchYadiyadNews(searchModel.SearchTitle, searchModel.NewsTypeId, searchModel.StartDate, 
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new YadiyadNewsListModel().PrepareToGrid(searchModel, news, () =>
            {
                return news.Select(category =>
                {
                    //fill in model values from the entity
                    var newsModel = category.ToModel<YadiyadNewsModel>();
                    //newsModel.NewsTypeString = Enum.GetName(typeof(NewsType), value: newsModel.NewsTypeId).GetDescription();
                    //newsModel.NewsTypeString = (newsModel.NewsTypeId as NewsType).GetDescription();
                    newsModel.NewsTypeString = ((NewsType)newsModel.NewsTypeId).GetDescription() != "0" ? ((NewsType)newsModel.NewsTypeId).GetDescription() : "-" ;

                    return newsModel;
                });
            });

            return model;
        }

        public virtual YadiyadNewsModel PrepareNewsModel(YadiyadNewsModel model, NewsItem news, bool excludeProperties = false)
        {
            if (news != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = news.ToModel<YadiyadNewsModel>();
                }
            }
            return model;
        }

     
    }
}
