using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public class ApiSliderService : IApiSliderService
    {
        #region Fields

        private readonly IRepository<ApiSlider> _sliderRepository;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ApiSliderService(IRepository<ApiSlider> sliderRepository,
            IStaticCacheManager cacheManager)
        {
            _sliderRepository = sliderRepository;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public void DeleteApiSlider(ApiSlider slider)
        {
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            _sliderRepository.Delete(slider);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SliderPrefixCacheKey);
        }

        public void InsertApiSlider(ApiSlider slider)
        {
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));
            
            _sliderRepository.Insert(slider);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SliderPrefixCacheKey);
        }

        public void InsertApiSlider(List<ApiSlider> sliders)
        {
            if (sliders == null)
                throw new ArgumentNullException(nameof(sliders));

            _sliderRepository.Insert(sliders);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SliderPrefixCacheKey);
        }

        public void UpdateApiSlider(ApiSlider slider)
        {
            if (slider == null)
                throw new ArgumentNullException(nameof(slider));

            _sliderRepository.Update(slider);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SliderPrefixCacheKey);
        }

        public ApiSlider GetApiSliderById(int sliderId)
        {
            if (sliderId == 0)
                return null;

            return _sliderRepository.GetById(sliderId);
        }

        public IPagedList<ApiSlider> GetAllApiSliders(List<int> stids = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var sliders = _sliderRepository.Table;

            if (stids != null && stids.Any())
                sliders = sliders.Where(x => stids.Contains(x.SliderTypeId));

            sliders = sliders.OrderBy(e => e.DisplayOrder);

            return new PagedList<ApiSlider>(sliders, pageIndex, pageSize);
        }

        public IList<ApiSlider> GetActiveApiSliders(int maximumItems = 0)
        {
            var sliders = _sliderRepository.Table
                .Where(x => (!x.ActiveStartDateUtc.HasValue || x.ActiveStartDateUtc <= DateTime.UtcNow) &&
                (!x.ActiveEndDateUtc.HasValue || x.ActiveEndDateUtc >= DateTime.UtcNow));

            sliders = sliders.OrderBy(e => e.DisplayOrder);
            return new PagedList<ApiSlider>(sliders, 0, maximumItems).ToList();
        }

        public IList<ApiSlider> GetApiSliderByIds(int[] ids)
        {
            if(ids == null || ids.Length == 0)
                return new List<ApiSlider>();


            var query = from s in _sliderRepository.Table
                        where ids.Contains(s.Id)
                        select s;
            var logItems = query.ToList();

            //sort by passed identifiers
            var sortedLogItems = new List<ApiSlider>();
            foreach (var id in ids)
            {
                var slider = logItems.Find(x => x.Id == id);
                if (slider != null)
                    sortedLogItems.Add(slider);
            }
            return sortedLogItems;
        }

        public void DeleteApiSliders(IList<ApiSlider> sliders)
        {
            if (sliders == null)
                throw new ArgumentNullException(nameof(sliders));

            _sliderRepository.Delete(sliders);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.SliderPrefixCacheKey);
        }

        #endregion
    }
}
