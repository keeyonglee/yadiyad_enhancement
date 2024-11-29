using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public class SliderModelFactory : ISliderModelFactory
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IApiSliderService _sliderService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public SliderModelFactory(CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IDateTimeHelper dateTimeHelper,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IApiSliderService sliderService,
            IPictureService pictureService)
        {
            _catalogSettings = catalogSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _languageService = languageService;
            _localizationService = localizationService;
            _sliderService = sliderService;
            _pictureService = pictureService;
        }

        #endregion

        #region Utilities

        protected void PrepareSliderTypes(IList<SelectListItem> items, bool excludeDefaultItem = false, string label = "")
        {
            var selectList = SliderType.Product.ToSelectList(false);
            foreach (var item in selectList)
                items.Add(item);

            if (!excludeDefaultItem)
            {
                label = string.IsNullOrWhiteSpace(label) ? _localizationService.GetResource("Admin.Common.All") : label;
                items.Insert(0, new SelectListItem()
                {
                    Text = label,
                    Value = "0"
                });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare slider search model
        /// </summary>
        /// <param name="searchModel">Slider search model</param>
        /// <returns>Slider search model</returns>
        public virtual SliderSearchModel PrepareSliderSearchModel(SliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            PrepareSliderTypes(searchModel.AvailableSliderTypes);
            searchModel.SelectedSliderTypes = new List<int> { 0 };
            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged slider list model
        /// </summary>
        /// <param name="searchModel">Slider search model</param>
        /// <returns>Slider list model</returns>
        public virtual SliderListModel PrepareSliderListModel(SliderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var selectedTypes = (searchModel.SelectedSliderTypes?.Contains(0) ?? true) ? null : searchModel.SelectedSliderTypes.ToList();

            //get sliders
            var sliders = _sliderService.GetAllApiSliders(selectedTypes, searchModel.Page - 1, searchModel.PageSize);

            //prepare list model
            var model = new SliderListModel().PrepareToGrid(searchModel, sliders, () =>
            {
                return sliders.Select(slider =>
                {
                    return PrepareSliderModel(null, slider);
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare slider model
        /// </summary>
        /// <param name="model">Slider model</param>
        /// <param name="slider">Slider</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Slider model</returns>
        public virtual SliderModel PrepareSliderModel(SliderModel model, ApiSlider slider, bool excludeProperties = false)
        {
            if (slider != null)
            {
                if (model == null)
                {
                    model = slider.ToModel<SliderModel>();
                    if (slider.ActiveStartDateUtc.HasValue)
                        model.ActiveStartDate = _dateTimeHelper.ConvertToUserTime(slider.ActiveStartDateUtc.Value, DateTimeKind.Utc);
                    if (slider.ActiveEndDateUtc.HasValue)
                        model.ActiveEndDate = _dateTimeHelper.ConvertToUserTime(slider.ActiveEndDateUtc.Value, DateTimeKind.Utc);

                    model.PictureUrl = _pictureService.GetPictureUrl(slider.PictureId, 120);
                    model.SliderTypeStr = _localizationService.GetLocalizedEnum(slider.SliderType);
                }

                model.CreatedOn = _dateTimeHelper.ConvertToUserTime(slider.CreatedOnUtc, DateTimeKind.Utc);
            }

            if (!excludeProperties)
            {
                PrepareSliderTypes(model.AvailableSliderTypes, true);
            }

            return model;
        }

        #endregion
    }
}
