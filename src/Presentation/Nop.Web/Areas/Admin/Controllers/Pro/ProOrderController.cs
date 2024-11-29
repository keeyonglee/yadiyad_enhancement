using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.ProOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Order;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class ProOrderController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ProOrderModelFactory _proOrderModelFactory;
        private readonly OrderService _orderService;

        #endregion

        #region Ctor

        public ProOrderController(IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ProOrderModelFactory proOrderModelFactory,
             OrderService orderService)
        {
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _proOrderModelFactory = proOrderModelFactory;
            _orderService = orderService;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {

            //prepare model
            var model = _proOrderModelFactory.PrepareOrderSearchModel(new ProOrderSearchModel
            {
            });

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OrderList(ProOrderSearchModel searchModel)
        {

            //prepare model
            var model = _proOrderModelFactory.PrepareOrderListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Details

        public virtual IActionResult Edit(int id)
        {

            //try to get an order with the specified id
            var order = _orderService.GetById(id);
            if (order == null)
                return RedirectToAction("List");


            //prepare model
            var model = _proOrderModelFactory.PrepareOrderModel(null, order);

            return View(model);
        }

        #endregion

    }
}
