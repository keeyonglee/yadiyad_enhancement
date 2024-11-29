using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using YadiYad.Pro.Web.Factories;

namespace YadiYad.Pro.Web.Components
{
    public class FooterProViewComponent : NopViewComponent
    {
        private readonly CommonModelFactory _commonModelFactory;

        public FooterProViewComponent(CommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareFooterModel();
            return View(model);
        }
    }
}
