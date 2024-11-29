using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.NopStation.WebApi.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new string[] { 
                    $"/Plugins/NopStation.WebApi/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.WebApi/Areas/Admin/Views/Shared/{{0}}.cshtml"
                }
                .Concat(viewLocations);
            }
            else
            {
                viewLocations = new string[] { 
                    $"/Plugins/NopStation.WebApi/Views/{{1}}/{{0}}.cshtml",
                    $"/Plugins/NopStation.WebApi/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }
            return viewLocations;
        }
    }
}
