using Nop.Core;
using Nop.Data;
using Nop.Plugin.NopStation.WebApi.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public class ApiDeviceService : IApiDeviceService
    {
        #region Fields

        private readonly IRepository<ApiDevice> _deviceRepository;

        #endregion

        #region Ctor

        public ApiDeviceService(IRepository<ApiDevice> deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        #endregion

        #region Methods

        public void DeleteApiDevice(ApiDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            _deviceRepository.Delete(device);
        }

        public void InsertApiDevice(ApiDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            _deviceRepository.Insert(device);
        }

        public void UpdateApiDevice(ApiDevice device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            _deviceRepository.Update(device);
        }

        public ApiDevice GetApiDeviceById(int deviceId)
        {
            if (deviceId == 0)
                return null;

            return _deviceRepository.GetById(deviceId);
        }

        public ApiDevice GetApiDeviceByDeviceId(string deviceToken, int storeId)
        {
            return _deviceRepository.Table.FirstOrDefault(x => x.DeviceToken == deviceToken && x.StoreId == storeId);
        }

        public IPagedList<ApiDevice> SearchApiDevices(int customerId = 0, int appTypeId = 0, IList<int> dtids = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _deviceRepository.Table;
            if (dtids != null && dtids.Any())
                query = query.Where(x => dtids.Contains(x.DeviceTypeId));

            if (customerId > 0)
                query = query.Where(x => x.CustomerId == customerId);

            if (appTypeId > 0)
                query = query.Where(x => x.AppTypeId == appTypeId);

            query = query.OrderByDescending(e => e.Id);

            return new PagedList<ApiDevice>(query, pageIndex, pageSize);
        }

        public IList<ApiDevice> GetApiDeviceByIds(int[] deviceIds)
        {
            if (deviceIds == null || deviceIds.Length == 0)
                return new List<ApiDevice>();

            var devices = _deviceRepository.Table.Where(x => deviceIds.Contains(x.Id)).ToList();

            var sortedDevices = new List<ApiDevice>();
            foreach (var id in deviceIds)
            {
                var device = devices.Find(x => x.Id == id);
                if (device != null)
                    sortedDevices.Add(device);
            }
            return sortedDevices;
        }

        public void DeleteApiDevices(IList<ApiDevice> devices)
        {
            if (devices == null)
                throw new ArgumentNullException(nameof(devices));

            _deviceRepository.Delete(devices);
        }

        #endregion
    }
}
