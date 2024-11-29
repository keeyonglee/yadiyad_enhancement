using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Domains;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface IApiSliderService
    {
        void DeleteApiSlider(ApiSlider slider);

        void DeleteApiSliders(IList<ApiSlider> sliders);

        void InsertApiSlider(ApiSlider slider);

        void InsertApiSlider(List<ApiSlider> sliders);

        void UpdateApiSlider(ApiSlider slider);

        ApiSlider GetApiSliderById(int sliderId);

        IPagedList<ApiSlider> GetAllApiSliders(List<int> stids = null, int pageIndex = 0, int pageSize = int.MaxValue);

        IList<ApiSlider> GetActiveApiSliders(int maximumItems = 0);

        IList<ApiSlider> GetApiSliderByIds(int[] ids);
    }
}