using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Models.Charge;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Order;
using YadiYad.Pro.Services.Order;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class ChargeController : BaseAdminController
    {
        #region Fields

        private readonly ChargeModelFactory _chargeModelFactory;
        private readonly ChargeService _chargeService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ChargeController(
            ChargeModelFactory chargeModelFactory,
            ChargeService chargeService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            IWorkContext workContext)
        {
            _chargeModelFactory = chargeModelFactory;
            _chargeService = chargeService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _workContext = workContext;

        }

        #endregion

        #region Methods

        #region List

        public IActionResult Index()
        {
            return RedirectToAction("List");

        }

        public virtual IActionResult List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {

            //prepare model
            var model = _chargeModelFactory.PrepareChargeSearchModel(new ChargeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ChargeList(ChargeSearchModel searchModel)
        {

            //prepare model
            var model = _chargeModelFactory.PrepareChargeListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create

        public virtual IActionResult ChargeCreate()
        {
            //prepare model
            var model = _chargeModelFactory.PrepareChargeModel(new ChargeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ChargeCreate(ChargeModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var chargeItem = model.ToEntity<Charge>();
                chargeItem.CreatedOnUTC = DateTime.UtcNow;
                chargeItem.CreatedById = _workContext.CurrentCustomer.Id;
                chargeItem.IsActive = true;
                switch (model.ProductTypeId)
                {
                    case 2:
                        chargeItem.SubProductTypeId = model.SubProductTypeJobEnumId;
                        break;
                    case 4:
                        chargeItem.SubProductTypeId = model.SubProductTypeServiceEnumId;

                        break;
                    default:
                        break;
                }

                _chargeService.Insert(chargeItem);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.Charge.Setting.Added"));


                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("ChargeCreate", new { id = chargeItem.Id });
            }

            //prepare model
            model = _chargeModelFactory.PrepareChargeModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult ChargeEdit(int id)
        {
            //try to get a news item with the specified id
            var charge = _chargeService.GetItemById(id);
            if (charge == null)
                return RedirectToAction("List");

            //prepare model
            var model = _chargeModelFactory.PrepareChargeModel(null, charge);
            switch (model.ProductTypeId)
            {
                case 2:
                    model.SubProductTypeJobEnumId = model.SubProductTypeId;
                    break;
                case 4:
                    model.SubProductTypeServiceEnumId = model.SubProductTypeId;

                    break;
                default:
                    break;
            }
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult ChargeEdit(ChargeModel model, bool continueEditing)
        {
            //try to get a news item with the specified id
            var charge = _chargeService.GetItemById(model.Id);
            if (charge == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                charge = model.ToEntity(charge);
                charge.IsActive = true;
                switch (model.ProductTypeId)
                {
                    case 2:
                        charge.SubProductTypeId = model.SubProductTypeJobEnumId;
                        break;
                    case 4:
                        charge.SubProductTypeId = model.SubProductTypeServiceEnumId;
                        break;
                    default:
                        break;
                }
                if (model.EndDateNull == true)
                {
                    charge.EndDate = null;
                }
                _chargeService.Update(charge);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("ChargeEdit", new { id = charge.Id });
            }

            //prepare model
            model = _chargeModelFactory.PrepareChargeModel(model, charge, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #endregion

    }
}
