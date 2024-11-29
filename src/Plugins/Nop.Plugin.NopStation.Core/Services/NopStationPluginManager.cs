using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.NopStation.Core.Services;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public class NopStationPluginManager : PluginManager<INopStationPlugin>, INopStationPluginManager
    {
        #region Fields

        private readonly NopStationCoreSettings _nopStationCoreSettings;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public NopStationPluginManager(NopStationCoreSettings nopStationCoreSettings,
            IPluginService pluginService,
            ILocalizationService localizationService,
            ICustomerService customerService) : base(customerService, pluginService)
        {
            _nopStationCoreSettings = nopStationCoreSettings;
            _localizationService = localizationService;
        }

        #endregion

        public virtual IList<INopStationPlugin> LoadNopStationPlugins(Customer customer = null, string pluginSystemName = "", 
            int storeId = 0, string widgetZone = null)
        {
            var systemNames = !string.IsNullOrWhiteSpace(pluginSystemName) ? new List<string> { pluginSystemName } :
                _nopStationCoreSettings.ActiveNopStationSystemNames;

            var nopStationPlugins = LoadActivePlugins(systemNames, customer, storeId);

            return nopStationPlugins;
        }

        public virtual IPagedList<KeyValuePair<string, string>> LoadPluginStringResources(string pluginSystemName = "", 
            string keyword = "", int languageId = 0, int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var nopStationPlugins = LoadNopStationPlugins(pluginSystemName: pluginSystemName, storeId: storeId);
            var originnalResources = new List<KeyValuePair<string, string>>();
            foreach (var plugin in nopStationPlugins)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                    originnalResources.AddRange(plugin.PluginResouces().OrderBy(x => x.Key).ToList());
                else
                    originnalResources.AddRange(plugin.PluginResouces()
                        .Where(x => x.Key.ToLower().Contains(keyword.ToLower())).OrderBy(x => x.Key).ToList());
            }

            var total = originnalResources.Count();
            originnalResources = originnalResources.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            var pagedResources = new List<KeyValuePair<string, string>>();
            foreach (var item in originnalResources)
            {
                var resource = _localizationService.GetResource(item.Key, languageId, false, "", true);
                if (string.IsNullOrWhiteSpace(resource))
                {
                    _localizationService.AddOrUpdatePluginLocaleResource(item.Key, item.Value);
                    resource = item.Value;
                }
                pagedResources.Add(new KeyValuePair<string, string>(item.Key, resource));
            }

            return new PagedList<KeyValuePair<string, string>>(pagedResources, pageIndex, pageSize, total);
        }
    }
}
