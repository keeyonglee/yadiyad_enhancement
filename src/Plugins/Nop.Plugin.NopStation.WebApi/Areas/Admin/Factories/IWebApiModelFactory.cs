using Nop.Core.Domain.Localization;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Web.Areas.Admin.Models.Localization;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories
{
    public interface IWebApiModelFactory
    {
        ConfigurationModel PrepareConfigurationModel();

        LocaleResourceListModel PrepareLocaleResourceListModel(LocaleResourceSearchModel searchModel, Language language);
    }
}