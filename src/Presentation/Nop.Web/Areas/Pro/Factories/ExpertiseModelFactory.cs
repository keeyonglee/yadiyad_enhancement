using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Pro.Models.Expertise;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;

namespace Nop.Web.Areas.Pro.Factories
{
    public class ExpertiseModelFactory
    {
        #region Fields

        private readonly ExpertiseService _expertiseService;
        private readonly JobServiceCategoryService _jobServiceCategoryService;

        #endregion

        #region Ctor

        public ExpertiseModelFactory(ExpertiseService expertiseService,
            JobServiceCategoryService jobServiceCategoryService)
        {
            _expertiseService = expertiseService;
            _jobServiceCategoryService = jobServiceCategoryService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual ExpertiseSearchModel PrepareExpertiseSearchModel(ExpertiseSearchModel searchModel)
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
        public virtual ExpertiseListModel PrepareExpertiseListModel(ExpertiseSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var categories = _expertiseService.SearchExpertise(searchModel.SearchTitle, searchModel.SearchCategoryId, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare grid model
            var model = new ExpertiseListModel().PrepareToGrid(searchModel, categories, () =>
            {
                var categoryList = _jobServiceCategoryService.GetAllJobServiceCategoris().ToList();

                return categories.Select(category =>
                {
                    //fill in model values from the entity
                    var expertiseModel = category.ToModel<ExpertiseModel>();
                    foreach (var c in categoryList)
                    {
                        if (c.Id == expertiseModel.JobServiceCategoryId)
                        {
                            expertiseModel.JobServiceCategoryString = c.Name;
                            break;
                        }
                    }
                    if (expertiseModel.Published == true)
                    {
                        expertiseModel.PublishedString = "Yes";
                    }
                    else
                    {
                        expertiseModel.PublishedString = "No";

                    }

                    return expertiseModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare category model
        /// </summary>
        /// <param name="model">expertise model</param>
        /// <param name="expertise">expertise</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>expertise model</returns>
        public virtual ExpertiseModel PrepareExpertiseModel(ExpertiseModel model, Expertise expertise, bool excludeProperties = false)
        {
            Action<CategoryLocalizedModel, int> localizedModelConfiguration = null;

            if (expertise != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = expertise.ToModel<ExpertiseModel>();
                }
            }
            return model;
        }

        #endregion
    }
}
