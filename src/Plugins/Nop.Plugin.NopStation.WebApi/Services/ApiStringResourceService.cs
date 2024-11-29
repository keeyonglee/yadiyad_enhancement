using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Infrastructure.Cache;
using Nop.Services.Localization;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public class ApiStringResourceService : IApiStringResourceService
    {
        #region Fields

        private readonly IRepository<ApiStringResource> _apiStringResourceRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Ctor

        public ApiStringResourceService(IRepository<ApiStringResource> apiStringResourceRepository,
            ILocalizationService localizationService,
            IStaticCacheManager staticCacheManager)
        {
            _apiStringResourceRepository = apiStringResourceRepository;
            _localizationService = localizationService;
            _cacheManager = staticCacheManager;
        }

        #endregion

        #region Utilities

        private static Dictionary<string, KeyValuePair<int, string>> ResourceValuesToDictionary(IEnumerable<ApiStringResource> locales)
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceName));
            }

            return dictionary;
        }

        #endregion

        #region Methods

        public void DeleteApiStringResource(ApiStringResource apiStringResource)
        {
            if (apiStringResource == null)
                throw new ArgumentNullException(nameof(apiStringResource));

            _apiStringResourceRepository.Delete(apiStringResource);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
        }

        public void InsertApiStringResource(ApiStringResource apiStringResource)
        {
            if (apiStringResource == null)
                throw new ArgumentNullException(nameof(apiStringResource));

            _apiStringResourceRepository.Insert(apiStringResource);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
        }

        public void InsertApiStringResource(List<ApiStringResource> apiStringResources)
        {
            if (apiStringResources == null)
                throw new ArgumentNullException(nameof(apiStringResources));

            _apiStringResourceRepository.Insert(apiStringResources);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
        }

        public void UpdateApiStringResource(ApiStringResource apiStringResource)
        {
            if (apiStringResource == null)
                throw new ArgumentNullException(nameof(apiStringResource));

            _apiStringResourceRepository.Update(apiStringResource);

            _cacheManager.RemoveByPrefix(ApiModelCacheDefaults.StringResourcePrefixCacheKey);
        }

        public ApiStringResource GetApiStringResourceById(int apiStringResourceId)
        {
            if (apiStringResourceId == 0)
                return null;

            return _apiStringResourceRepository.GetById(apiStringResourceId);
        }

        public ApiStringResource GetApiStringResourceByName(string resourceKey)
        {
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();

            var query = from l in _apiStringResourceRepository.Table
                        orderby l.ResourceName
                        where l.ResourceName.ToLower() == resourceKey
                        select l;
         
            return query.FirstOrDefault();
        }

        public Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId)
        {
            var localeResources = _localizationService.GetAllResourceValues(languageId, loadPublicLocales: null);

            var key = string.Format(ApiModelCacheDefaults.StringResourceKey, languageId);
            return _cacheManager.Get(new CacheKey(key, ApiModelCacheDefaults.StringResourcePrefixCacheKey), () =>
            {
                var query = from l in _apiStringResourceRepository.Table
                            orderby l.ResourceName
                            select l;
                var appResources = ResourceValuesToDictionary(query);

                var resources = new Dictionary<string, KeyValuePair<int, string>>();
                foreach (var item in appResources)
                {
                    if (localeResources.TryGetValue(item.Key, out KeyValuePair<int, string> value))
                        resources.Add(item.Key, value);
                }

                return resources;
            });
        }

        #endregion
    }
}
