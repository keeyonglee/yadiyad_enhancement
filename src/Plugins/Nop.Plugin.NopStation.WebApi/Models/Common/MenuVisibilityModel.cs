using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Models.Common
{
    public class MenuVisibilityModel
    {
        public bool HasReturnRequests { get; set; }
        public bool HideDownloadableProducts { get; set; }
    }
}
