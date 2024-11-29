using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Domains;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.WebApi.Services
{
    public interface IApiDeviceService
    {
        void DeleteApiDevice(ApiDevice device);

        void InsertApiDevice(ApiDevice device);

        void UpdateApiDevice(ApiDevice device);

        ApiDevice GetApiDeviceById(int deviceId);

        ApiDevice GetApiDeviceByDeviceId(string deviceToken, int storeId);

        IPagedList<ApiDevice> SearchApiDevices(int customerId = 0, int appTypeId = 0, IList<int> dtids = null, 
            int pageIndex = 0, int pageSize = int.MaxValue);

        IList<ApiDevice> GetApiDeviceByIds(int[] deviceByIds);

        void DeleteApiDevices(IList<ApiDevice> devices);
    }
}