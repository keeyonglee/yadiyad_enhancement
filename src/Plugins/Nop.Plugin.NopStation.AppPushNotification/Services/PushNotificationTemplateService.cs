using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Plugin.NopStation.AppPushNotification.Extensions;
using Nop.Services.Events;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public class AppPushNotificationTemplateService : IPushNotificationTemplateService
    {
        #region Fields

        private readonly IStaticCacheManager _cacheManager;
        private readonly IRepository<AppPushNotificationTemplate> _pushNotificationTemplateRepository;
        private readonly CatalogSettings _catalogSettings;
        private readonly IRepository<StoreMapping> _storeMappingRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreContext _storeContext;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public AppPushNotificationTemplateService(IStaticCacheManager cacheManager,
            IRepository<AppPushNotificationTemplate> pushNotificationTemplateRepository,
            CatalogSettings catalogSettings,
            IRepository<StoreMapping> storeMappingRepository,
            IStoreMappingService storeMappingService,
            IStoreContext storeContext,
            IEventPublisher eventPublisher)
        {
            _pushNotificationTemplateRepository = pushNotificationTemplateRepository;
            _cacheManager = cacheManager;
            _catalogSettings = catalogSettings;
            _storeMappingRepository = storeMappingRepository;
            _storeMappingService = storeMappingService;
            _storeContext = storeContext;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods

        public void DeletePushNotificationTemplate(AppPushNotificationTemplate pushNotificationTemplate)
        {
            if (pushNotificationTemplate == null)
                throw new ArgumentNullException(nameof(pushNotificationTemplate));

            _pushNotificationTemplateRepository.Delete(pushNotificationTemplate);

            _cacheManager.RemoveByPrefix(AppPushNotificationDefaults.MessageTemplatesPrefixCacheKey);

            _eventPublisher.EntityDeleted(pushNotificationTemplate);
        }

        public void InsertPushNotificationTemplate(AppPushNotificationTemplate pushNotificationTemplate)
        {
            if (pushNotificationTemplate == null)
                throw new ArgumentNullException(nameof(pushNotificationTemplate));

            _pushNotificationTemplateRepository.Insert(pushNotificationTemplate);

            _cacheManager.RemoveByPrefix(AppPushNotificationDefaults.MessageTemplatesPrefixCacheKey);

            _eventPublisher.EntityInserted(pushNotificationTemplate);
        }

        public void UpdatePushNotificationTemplate(AppPushNotificationTemplate pushNotificationTemplate)
        {
            if (pushNotificationTemplate == null)
                throw new ArgumentNullException(nameof(pushNotificationTemplate));

            _pushNotificationTemplateRepository.Update(pushNotificationTemplate);

            _cacheManager.RemoveByPrefix(AppPushNotificationDefaults.MessageTemplatesPrefixCacheKey);

            _eventPublisher.EntityUpdated(pushNotificationTemplate);
        }

        public AppPushNotificationTemplate GetPushNotificationTemplateById(int pushNotificationTemplateId)
        {
            if (pushNotificationTemplateId == 0)
                return null;

            return _pushNotificationTemplateRepository.GetById(pushNotificationTemplateId);
        }

        public IPagedList<AppPushNotificationTemplate> GetAllPushNotificationTemplates(string keyword = null, bool? active = null, 
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var key = string.Format(AppPushNotificationDefaults.MessageTemplatesAllCacheKey, keyword, active, storeId);
            var templates = _cacheManager.Get(new CacheKey(key, AppPushNotificationDefaults.MessageTemplatesPrefixCacheKey), () =>
            {
                var query = _pushNotificationTemplateRepository.Table;

                if (!string.IsNullOrWhiteSpace(keyword))
                    query = query.Where(x => x.Name.Contains(keyword) || x.Title.Contains(keyword) || x.Body.Contains(keyword));

                if (active.HasValue)
                    query = query.Where(x => x.Active == active.Value);

                if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
                {
                    var storeMappings = _storeMappingRepository.Table
                        .Where(x => x.EntityName == nameof(AppPushNotificationTemplate) && x.StoreId == storeId)
                        .ToList();

                    query = query.Where(x => !x.LimitedToStores || storeMappings.Any(y => y.EntityId == x.Id));
                }

                return query.OrderBy(t => t.Name).ToList();
            });
            return new PagedList<AppPushNotificationTemplate>(templates, pageIndex, pageSize);
        }

        public IList<AppPushNotificationTemplate> GetPushNotificationTemplatesByName(string messageTemplateName, int storeId = 0)
        {
            if (string.IsNullOrWhiteSpace(messageTemplateName))
                return new List<AppPushNotificationTemplate>();

            var key = string.Format(AppPushNotificationDefaults.MessageTemplatesByNameCacheKey, messageTemplateName, storeId);
            return _cacheManager.Get(new CacheKey(key, AppPushNotificationDefaults.MessageTemplatesPrefixCacheKey), () =>
            {
                var templates = _pushNotificationTemplateRepository.Table
                    .Where(messageTemplate => messageTemplate.Name.Equals(messageTemplateName))
                    .OrderBy(messageTemplate => messageTemplate.Id).ToList();

                if (storeId > 0)
                    templates = templates.Where(messageTemplate => _storeMappingService.Authorize(messageTemplate, storeId)).ToList();

                return templates;
            });
        }
       
        public virtual IList<AppPushNotificationTemplate> GetTemplatesByIds(int[] templateIds)
        {
            if (templateIds == null || templateIds.Length == 0)
                return new List<AppPushNotificationTemplate>();

            var query = from o in _pushNotificationTemplateRepository.Table
                        where templateIds.Contains(o.Id) 
                        select o;
            var templates = query.ToList();
            //sort by passed identifiers
            var sortedTemplates = new List<AppPushNotificationTemplate>();
            foreach (var id in templateIds)
            {
                var template = templates.Find(x => x.Id == id);
                if (template != null)
                    sortedTemplates.Add(template);
            }

            return sortedTemplates;
        }

        #endregion
    }
}
