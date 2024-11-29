using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using Nop.Plugin.NopStation.WebApi.Models.Sliders;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Media;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Factories
{
    public class SliderModelFactory : ISliderModelFactory
    {
        private readonly IApiSliderService _sliderService;
        private readonly WebApiSettings _webApiSettings;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _workContext;

        public SliderModelFactory(IApiSliderService sliderService,
            WebApiSettings webApiSettings,
            IPictureService pictureService,
            IStaticCacheManager cacheManager,
            IWorkContext workContext)
        {
            _sliderService = sliderService;
            _webApiSettings = webApiSettings;
            _pictureService = pictureService;
            _cacheManager = cacheManager;
            _workContext = workContext;
        }

        public virtual HomePageSliderModel PrepareHomePageSliderModel()
        {
            var maxSliders = _webApiSettings.MaximumNumberOfHomePageSliders > 0 ? _webApiSettings.MaximumNumberOfHomePageSliders : int.MaxValue;

            var key = string.Format(ApiModelCacheDefaults.SliderModelKey, _workContext.CurrentCustomer.Id, maxSliders, _workContext.WorkingLanguage.Id, _webApiSettings.ShowHomepageSlider);
            var cacheKey = new CacheKey(key, ApiModelCacheDefaults.SliderPrefixCacheKey);
            var cachedModel = _cacheManager.Get(cacheKey, () =>
            {
                var model = new HomePageSliderModel();
                if (!_webApiSettings.ShowHomepageSlider)
                    return model;

                var sliders = _sliderService.GetActiveApiSliders(maxSliders);

                if (!sliders.Any())
                    return model;

                model.IsEnabled = true;
                foreach (var slider in sliders)
                {
                    model.Sliders.Add(new HomePageSliderModel.SliderModel()
                    {
                        EntityId = slider.EntityId,
                        Id = slider.Id,
                        ImageUrl = _pictureService.GetPictureUrl(slider.PictureId),
                        SliderType = (int)slider.SliderType
                    });
                }

                return model;
            });

            return cachedModel;
        }
    }
}
