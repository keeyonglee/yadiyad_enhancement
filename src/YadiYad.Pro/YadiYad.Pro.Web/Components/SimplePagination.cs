using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Components
{
    public class SimplePagination : NopViewComponent
    {
        #region Ctor

        public SimplePagination()
        {
        }

        #endregion

        #region Methods

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
