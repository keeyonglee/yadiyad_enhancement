using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.IPay88.Components
{
    [ViewComponent(Name = "PaymentIPay88")]
    public class PaymentIPay88ViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.IPay88/Views/PaymentInfo.cshtml");
        }
    }
}
