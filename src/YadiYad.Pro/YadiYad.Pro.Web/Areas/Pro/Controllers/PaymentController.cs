using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class PaymentController : BaseController
    {
        [HttpGet("pro/[controller]/response")]
        public IActionResult PaymentResponse(
            [FromQuery] int orderId,
            [FromQuery] int status = 0)
        {
            return View("PaymentFail", orderId);
        }
    }
}
