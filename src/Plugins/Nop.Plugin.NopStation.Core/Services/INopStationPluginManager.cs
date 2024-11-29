using System.Collections.Generic;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.Core.Services;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public interface INopStationPluginManager
    {
        IList<INopStationPlugin> LoadNopStationPlugins(Customer customer = null, string pluginSystemName = "", 
            int storeId = 0, string widgetZone = null);

        IPagedList<KeyValuePair<string, string>> LoadPluginStringResources(string pluginSystemName = "", 
            string keyword = "", int languageId = 0, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);
    }
}