using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Components
{
    public class VendorStatisticsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICommonModelFactory _commonModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public VendorStatisticsViewComponent(ICommonModelFactory commonModelFactory,
            IPermissionService permissionService,
            IWorkContext workContext)
        {
            _commonModelFactory = commonModelFactory;
            _permissionService = permissionService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke()
        {
            //only vendor have access to this report
            if (_workContext.CurrentVendor == null)
                return Content(string.Empty);

            //prepare model
            var model = _commonModelFactory.PrepareVendorStatisticsModel();

            return View(model);
        }

        #endregion
    }
}
