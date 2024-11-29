using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Models
{
    public class SliderSearchModel : BaseSearchModel
    {
        public SliderSearchModel()
        {
            AvailableSliderTypes = new List<SelectListItem>();
            SelectedSliderTypes = new List<int>();
        }

        [NopResourceDisplayName("Admin.NopStation.WebApi.Sliders.List.SelectedSliderTypes")]
        public IList<int> SelectedSliderTypes { get; set; }

        public IList<SelectListItem> AvailableSliderTypes { get; set; }
    }
}
