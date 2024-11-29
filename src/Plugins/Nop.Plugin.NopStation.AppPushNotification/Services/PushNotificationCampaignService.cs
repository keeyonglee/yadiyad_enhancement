using System;
using System.Linq.Dynamic.Core;
using System.Linq;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.NopStation.AppPushNotification.Domains;
using Nop.Services.Events;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Stores;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Data.Extensions;
using Nop.Plugin.NopStation.WebApi.Data;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.AppPushNotification.Services
{
    public class AppPushNotificationCampaignService : IPushNotificationCampaignService
    {
        #region Fields

        private readonly IRepository<AppPushNotificationCampaign> _notificationCampaignRepository;
        private readonly IRepository<ApiDevice> _apiDeviceRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly CatalogSettings _catalogSettings;
        private readonly INopDataProvider _dataProvider;

        #endregion

        #region Ctor

        public AppPushNotificationCampaignService(IEventPublisher eventPublisher,
            IRepository<AppPushNotificationCampaign> notificationCampaignRepository,
            IRepository<ApiDevice> apiDeviceRepository,
            CatalogSettings catalogSettings,
            INopDataProvider dataProvider)
        {
            _notificationCampaignRepository = notificationCampaignRepository;
            _apiDeviceRepository = apiDeviceRepository;
            _eventPublisher = eventPublisher;
            _catalogSettings = catalogSettings;
            _dataProvider = dataProvider;
        }

        #endregion

        #region Methods

        public virtual IPagedList<AppPushNotificationCampaign> GetAllPushNotificationCampaigns(string keyword = "",
            DateTime? searchFrom = null, DateTime? searchTo = null, bool? addedToQueueStatus = null, 
            int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _notificationCampaignRepository.Table.Where(x => !x.Deleted);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Name.Contains(keyword) || x.Title.Contains(keyword) || x.Body.Contains(keyword));
            if (searchFrom.HasValue)
                query = query.Where(x => x.SendingWillStartOnUtc >= searchFrom.Value);
            if (searchTo.HasValue)
                query = query.Where(x => x.SendingWillStartOnUtc <= searchTo.Value);
            if (addedToQueueStatus.HasValue)
                query = query.Where(x => x.AddedToQueueOnUtc.HasValue == addedToQueueStatus.Value);

            if (storeId > 0 && !_catalogSettings.IgnoreStoreLimitations)
            {
                query = query.Where(x => x.LimitedToStoreId == storeId);
            }

            query = query.OrderByDescending(x => x.Id);

            return new PagedList<AppPushNotificationCampaign>(query, pageIndex, pageSize);
        }

        public virtual AppPushNotificationCampaign InsertPushNotificationCampaign(AppPushNotificationCampaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            _notificationCampaignRepository.Insert(campaign);
            //event notification
            _eventPublisher.EntityInserted(campaign);

            return campaign;
        }
       
        public AppPushNotificationCampaign GetPushNotificationCampaignById(int id)
        {
            var query = _notificationCampaignRepository;
            return query.GetById(id);
        }

        public void UpdatePushNotificationCampaign(AppPushNotificationCampaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            _notificationCampaignRepository.Update(campaign);
            //event notification
            _eventPublisher.EntityUpdated(campaign);
        }

        public void UpdatePushNotificationCampaigns(IList<AppPushNotificationCampaign> campaigns)
        {
            _notificationCampaignRepository.Update(campaigns);
        }

        public void DeletePushNotificationCampaign(AppPushNotificationCampaign campaign)
        {
            if (campaign == null)
                throw new ArgumentNullException("campaign");

            campaign.Deleted = true;
            _notificationCampaignRepository.Update(campaign);
            _eventPublisher.EntityDeleted(campaign);
        }

        public IPagedList<ApiDevice> GetCampaignDevices(AppPushNotificationCampaign campaign, int pageIndex = 0)
        {
            var pStoreId = SqlParameterHelper.GetInt32Parameter("StoreId", campaign.LimitedToStoreId);
            var pDeviceTypeIds = SqlParameterHelper.GetStringParameter("DeviceTypeIds", campaign.DeviceTypes);
            var pCustomerRoleIds = SqlParameterHelper.GetStringParameter("CustomerRoleIds", campaign.CustomerRoles);
            var pPageIndex = SqlParameterHelper.GetInt32Parameter("PageIndex", pageIndex);
            var pPageSize = SqlParameterHelper.GetInt32Parameter("PageSize", 100);
            var pTotalRecords = SqlParameterHelper.GetOutputInt32Parameter("TotalRecords");

            //invoke stored procedure
            var devices = _apiDeviceRepository.EntityFromSql("NS_AppNotificationDeviceLoadAllPaged",
                pStoreId,
                pDeviceTypeIds,
                pCustomerRoleIds,
                pPageIndex,
                pPageSize,
                pTotalRecords).ToList();

            var totalRecords = pTotalRecords.Value != DBNull.Value ? Convert.ToInt32(pTotalRecords.Value) : 0;
            return new PagedList<ApiDevice>(devices, pageIndex, 100, totalRecords);
        }

        #endregion
    }
}
