using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.Category;
using YadiYad.Pro.Web.Models.Expertise;

namespace YadiYad.Pro.Web.FactoriesPro
{
    public class ExpertiseModelFactory
    {
        #region Fields

        private readonly ExpertiseService _expertiseService;
        private readonly CatalogSettings _catalogSettings;
        private readonly ILocalizationService _localizationService;


        #endregion

        #region Ctor

        public ExpertiseModelFactory(ExpertiseService expertiseService,
            CatalogSettings catalogSettings,
            ILocalizationService localizationService)
        {
            _expertiseService = expertiseService;
            _catalogSettings = catalogSettings;
            _localizationService = localizationService;
        }

        #endregion
        /// <summary>
        /// Prepare category search model
        /// </summary>
        /// <param name="searchModel">Category search model</param>
        /// <returns>Category search model</returns>
        public virtual CategorySearchModel PrepareExpertiseSearchModel(CategorySearchModel searchModel)
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
        public virtual ExpertiseListModel PrepareExpertiseListModel(CategorySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            //get categories
            var expertises = _expertiseService.GetAllExpertise();

            //prepare grid model
            var model = new ExpertiseListModel().PrepareToGrid(searchModel, expertises, () =>
            {
                return expertises.Select(expert =>
                {
                    //fill in model values from the entity
                    var expertModel = expert.ToModel<ExpertiseModel>();


                    return expertModel;
                });
            });

            return model;
        }

        public virtual ExpertiseModel PrepareExpertModel(ExpertiseModel model, Expertise expertise, bool excludeProperties = false)
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
    }
}
