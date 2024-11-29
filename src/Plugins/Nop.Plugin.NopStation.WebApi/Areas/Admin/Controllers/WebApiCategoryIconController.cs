using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.NopStation.Core.Infrastructure;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Factories;
using Nop.Plugin.NopStation.WebApi.Areas.Admin.Models;
using Nop.Plugin.NopStation.WebApi.Domains;
using Nop.Plugin.NopStation.WebApi.Extensions;
using Nop.Plugin.NopStation.WebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc.Filters;
using System;

namespace Nop.Plugin.NopStation.WebApi.Areas.Admin.Controllers
{
    [NopStationLicense]
    public class WebApiCategoryIconController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ICategoryIconModelFactory _categoryIconModelFactory;
        private readonly ICategoryIconService _categoryIconService;
        private readonly ICategoryService _categoryService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public WebApiCategoryIconController(ILocalizationService localizationService,
            INotificationService notificationService,
            ICategoryIconModelFactory categoryIconModelFactory,
            ICategoryIconService categoryIconService,
            ICategoryService categoryService,
            IPermissionService permissionService)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _categoryIconModelFactory = categoryIconModelFactory;
            _categoryIconService = categoryIconService;
            _categoryService = categoryService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods        

        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var searchModel = _categoryIconModelFactory.PrepareCategoryIconSearchModel(new CategoryIconSearchModel());
            return View(searchModel);
        }

        public virtual IActionResult GetList(CategoryIconSearchModel searchModel)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var model = _categoryIconModelFactory.PrepareCategoryIconListModel(searchModel);
            return Json(model);
        }

        public virtual IActionResult Create(int categoryId)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            var oldCategoryIcon = _categoryIconService.GetCategoryIconByCategoryId(categoryId);
            if (oldCategoryIcon != null)
                return RedirectToAction("Edit", new { categoryId = categoryId });

            var model = _categoryIconModelFactory.PrepareCategoryIconModel(null, category);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(CategoryIconModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(model.CategoryId);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            var oldCategoryIcon = _categoryIconService.GetCategoryIconByCategoryId(model.CategoryId);
            if (oldCategoryIcon != null)
                return RedirectToAction("Edit", new { categoryId = model.CategoryId });

            if (ModelState.IsValid)
            {
                var categoryIcon = model.ToEntity<ApiCategoryIcon>();

                _categoryIconService.InsertCategoryIcon(categoryIcon);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.CategoryIcons.Created"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { categoryId = categoryIcon.CategoryId });
            }

            model = _categoryIconModelFactory.PrepareCategoryIconModel(model, category);

            return View(model);
        }

        public virtual IActionResult Edit(int categoryId)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(categoryId);
            if (categoryIcon == null)
                return RedirectToAction("Create", new { categoryId = categoryId });

            var model = _categoryIconModelFactory.PrepareCategoryIconModel(null, category);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(CategoryIconModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(model.CategoryId);
            if (category == null || category.Deleted)
                return RedirectToAction("List");

            var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(model.CategoryId);
            if (categoryIcon == null)
                return RedirectToAction("Create", new { categoryId = model.CategoryId });

            if (ModelState.IsValid)
            {
                categoryIcon = model.ToEntity(categoryIcon);
                _categoryIconService.UpdateCategoryIcon(categoryIcon);
                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.CategoryIcons.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { categoryId = categoryIcon.CategoryId });
            }
            model = _categoryIconModelFactory.PrepareCategoryIconModel(model, category);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var categoryIcon = _categoryIconService.GetCategoryIconById(id);
            if (categoryIcon == null)
                throw new ArgumentNullException(nameof(categoryIcon));

            _categoryIconService.DeleteCategoryIcon(categoryIcon);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.NopStation.WebApi.CategoryIcons.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult CreateCategoryIcon(CategoryIconModel model)
        {
            if (!_permissionService.Authorize(WebApiPermissionProvider.ManageCategoryIcon))
                return AccessDeniedView();

            var category = _categoryService.GetCategoryById(model.CategoryId);
            if (category == null || category.Deleted)
                return Json(new { Result = false });

            var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(model.CategoryId);

            if (ModelState.IsValid)
            {
                if (categoryIcon != null)
                {
                    categoryIcon.CategoryBannerId = model.CategoryBannerId;
                    categoryIcon.PictureId = model.IconId;
                    _categoryIconService.UpdateCategoryIcon(categoryIcon);
                }
                else
                {
                    categoryIcon = model.ToEntity<ApiCategoryIcon>();
                    _categoryIconService.InsertCategoryIcon(categoryIcon);
                }
                return Json(new { Result = true });
            }

            return Json(new { Result = false });
        }

        #endregion
    }
}
