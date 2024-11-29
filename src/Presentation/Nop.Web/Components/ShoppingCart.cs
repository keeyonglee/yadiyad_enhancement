using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components
{
    public class ShoppingCartViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public ShoppingCartViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareShoppingCartModel();
            return View(model);
        }
    }
}
