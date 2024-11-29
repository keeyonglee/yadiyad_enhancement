using System;
using System.Collections.Generic;
using System.Text;
using static Nop.Plugin.Widgets.qBoSlider.Models.Public.WidgetZoneModel;

namespace Nop.Plugin.Widgets.qBoSlider.Models.Public
{
    public class CustomSliderSettingModel
    {
        public IList<SlideModel> Slides { get; set; }

        public bool IsVerticalView { get; set; }
    }
}
