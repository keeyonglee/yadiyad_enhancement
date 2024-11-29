using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Models
{
    public class DeviceSearchModel : BaseSearchModel
    {
        public DeviceSearchModel()
        {
            SelectedDeviceTypes = new List<int>();
            AvailableDeviceTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.NopStation.WebApi.Devices.List.SelectedDeviceTypes")]
        public IList<int> SelectedDeviceTypes { get; set; }

        public IList<SelectListItem> AvailableDeviceTypes { get; set; }
    }
}
