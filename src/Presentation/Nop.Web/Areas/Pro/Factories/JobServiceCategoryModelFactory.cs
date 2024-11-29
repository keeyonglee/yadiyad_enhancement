using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Pro.Models.JobServiceCategory;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;

namespace Nop.Web.Areas.Pro.Factories
{
    public partial class JobServiceCategoryModelFactory
    {
        #region Fields

        private readonly JobServiceCategoryService _jobServiceCategoryService;

        #endregion

        #region Ctor

        public JobServiceCategoryModelFactory(JobServiceCategoryService jobServiceCategoryService)
        {
            _jobServiceCategoryService = jobServiceCategoryService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual CategorySearchModel PrepareCategorySearchModel(CategorySearchModel searchModel)
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
        public virtual JobServiceCategoryListModel PrepareCategoryListModel(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var categories = _jobServiceCategoryService.SearchJobServiceCategories(searchModel.SearchCategoryName, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new JobServiceCategoryListModel().PrepareToGrid(searchModel, categories, () =>
            {
                return categories.Select(category =>
                {
                    //fill in model values from the entity
                    var categoryModel = category.ToModel<JobServiceCategoryModel>();
                    if (categoryModel.Published == true)
                    {
                        categoryModel.PublishedString = "Yes";
                    }
                    else
                    {
                        categoryModel.PublishedString = "No";

                    }

                    return categoryModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">Category model</param>
        /// <param name="category">Category</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Category model</returns>
        public virtual JobServiceCategoryModel PrepareCategoryModel(JobServiceCategoryModel model, JobServiceCategory category, bool excludeProperties = false)
        {
            Action<CategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (category != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = category.ToModel<JobServiceCategoryModel>();
                }
            }
            return model;
        }

        #endregion


    }


}
