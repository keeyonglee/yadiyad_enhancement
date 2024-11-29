using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Plugin.NopStation.AppPushNotification.Areas.Admin.Models
{
    public class AppPushNotificationTemplateSearchModel : BaseSearchModel
    {
        public AppPushNotificationTemplateSearchModel()
        {
            AvailableActiveTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchKeyword")]
        public string SearchKeyword { get; set; }

        [NopResourceDisplayName("Admin.NopStation.AppPushNotification.PushNotificationTemplates.List.SearchActiveId")]
        public int SearchActiveId { get; set; }

        public IList<SelectListItem> AvailableActiveTypes { get; set; }
    }
}
