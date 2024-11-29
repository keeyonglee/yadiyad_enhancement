using Nop.Web.Framework.Menu;

namespace Nop.Plugin.NopStation.Core.Services
{
    public interface INopStationCoreService
    {
        void ManageSiteMap(SiteMapNode rootNode, SiteMapNode childNode, NopStationMenuType menuType = NopStationMenuType.Root);
    }
}