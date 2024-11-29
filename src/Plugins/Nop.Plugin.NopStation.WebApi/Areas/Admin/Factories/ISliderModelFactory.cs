using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public interface ISliderModelFactory
    {
        SliderSearchModel PrepareSliderSearchModel(SliderSearchModel searchModel);

        SliderListModel PrepareSliderListModel(SliderSearchModel searchModel);

        SliderModel PrepareSliderModel(SliderModel model, ApiSlider slider, bool excludeProperties = false);
    }
}