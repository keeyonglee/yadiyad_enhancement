using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.DTO.Common;

namespace YadiYad.Pro.Services.Common
{
    public class CityService
    {
        #region 
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;

        #endregion

        #region Ctor

        public CityService
            (ICacheKeyService cacheKeyService,
            IStaticCacheManager staticCacheManager,
            IRepository<City> cityRepository,
            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository)
        {
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _cityRepository = cityRepository;
            _countryRepository = countryRepository;
            _stateProvinceRepository = stateProvinceRepository;
        }

        #endregion


        #region Methods

        public virtual IPagedList<City> GetAllCity(
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            string keyword = null,
            int countryId = 0,
            int stateProvinceId = 0)
        {
            var query = _cityRepository.Table;

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.Name.ToLower().Contains(keyword.ToLower()));

            if(countryId != 0)
            {
                query = query.Where(x => x.CountryId == countryId);
            }

            if(stateProvinceId != 0)
            {
                query = query.Where(x => x.StateProvinceId == stateProvinceId);
            }

            query = query.OrderBy(n => n.Name);

            var data = new PagedList<City>(query, pageIndex, pageSize);

            return data;
        }

        public virtual IPagedList<LocationDTO> GetAllLocation(
            int pageIndex = 0, 
            int pageSize = int.MaxValue, 
            string keyword = null,
            int countryId = 0,
            int stateProvinceId = 0)
        {
            var query = _cityRepository.Table
                .Join(_stateProvinceRepository.Table,
                x => x.StateProvinceId,
                y => y.Id,
                (x, y) => new
                {
                    City = x.Name,
                    CityId = x.Id,
                    StateProvince = y.Name,
                    StateProvinceId = y.Id,
                    CountryId = y.CountryId
                })
                .Join(_countryRepository.Table, 
                x=>x.CountryId, 
                y=>y.Id, 
                (x, y) => new LocationDTO
                {
                    City = x.City,
                    CityId = x.CityId,
                    StateProvince = x.StateProvince,
                    StateProvinceId = x.StateProvinceId,
                    CountryId = x.CountryId,
                    Country = y.Name,
                    DisplayName = x.City + ", "+x.StateProvince+", "+y.Name
                });

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(n => n.DisplayName.ToLower().Contains(keyword.ToLower()));

            if(countryId != 0)
            {
                query = query.Where(x => x.CountryId == countryId);
            }

            if(stateProvinceId != 0)
            {
                query = query.Where(x => x.StateProvinceId == stateProvinceId);
            }

            query = query.OrderBy(n => n.Country).ThenBy(x=>x.StateProvince).ThenBy(x=>x.Country);

            var data = new PagedList<LocationDTO>(query, pageIndex, pageSize);

            return data;
        }

        public virtual IList<City> GetCitiesByStateId(int stateId, int languageId = 0, bool showHidden = false)
        {
            var key = _cacheKeyService.PrepareKeyForDefaultCache(NopDirectoryDefaults.CitiesByStateCacheKey, stateId, languageId, showHidden);

            return _staticCacheManager.Get(key, () =>
            {
                var query = from sp in _cityRepository.Table
                            orderby sp.DisplayOrder, sp.Name
                            where sp.StateProvinceId == stateId &&
                            (showHidden || sp.Published)
                            select sp;
                var cities = query.ToList();

                if (languageId > 0)
                {
                    cities = cities
                        .OrderBy(c => c.Name)
                        .ToList();
                }

                return cities;
            });
        }

        #endregion
    }
}
