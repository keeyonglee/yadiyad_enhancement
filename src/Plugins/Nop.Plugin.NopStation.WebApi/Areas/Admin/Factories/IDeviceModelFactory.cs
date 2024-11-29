using Nop.Core;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public interface IDeviceModelFactory
    {
        DeviceSearchModel PrepareDeviceSearchModel(DeviceSearchModel searchModel);

        DeviceListModel PrepareDeviceListModel(DeviceSearchModel searchModel);

        DeviceModel PrepareDeviceModel(DeviceModel model, ApiDevice device);
    }
}