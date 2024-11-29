using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.JobServiceCategory;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class JobServiceCategoryController : BaseAdminController
    {
        #region Fields

        private readonly IAclService _aclService;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly JobServiceCategoryModelFactory _categoryModelFactory;
        private readonly JobServiceCategoryService _categoryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IExportManager _exportManager;
        private readonly IImportManager _importManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;


        #endregion

        #region Ctor

        public JobServiceCategoryController(IAclService aclService,
            ICacheKeyService cacheKeyService,
            JobServiceCategoryModelFactory categoryModelFactory,
            JobServiceCategoryService categoryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _aclService = aclService;
            _cacheKeyService = cacheKeyService;
            _categoryModelFactory = categoryModelFactory;
            _categoryService = categoryService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _discountService = discountService;
            _exportManager = exportManager;
            _importManager = importManager;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        //protected virtual void UpdateLocales(JobServiceCategory category, CategoryModel model)
        //{
        //    foreach (var localized in model.Locales)
        //    {
        //        _localizedEntityService.SaveLocalizedValue(category,
        //            x => x.Name,
        //            localized.Name,
        //            localized.LanguageId);

        //        //search engine name
        //        var seName = _urlRecordService.ValidateSeName(category, localized.SeName, localized.Name, false);
        //        _urlRecordService.SaveSlug(category, seName, localized.LanguageId);
        //    }
        //}

        //protected virtual void UpdatePictureSeoNames(JobServiceCategory category)
        //{
        //    var picture = _pictureService.GetPictureById(category.PictureId);
        //    if (picture != null)
        //        _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(category.Name));
        //}

        //protected virtual void SaveCategoryAcl(JobServiceCategory category, CategoryModel model)
        //{
        //    category.SubjectToAcl = model.SelectedCustomerRoleIds.Any();
        //    _categoryService.UpdateCategory(category);

        //    var existingAclRecords = _aclService.GetAclRecords(category);
        //    var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
        //    foreach (var customerRole in allCustomerRoles)
        //    {
        //        if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
        //        {
        //            //new role
        //            if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
        //                _aclService.InsertAclRecord(category, customerRole.Id);
        //        }
        //        else
        //        {
        //            //remove role
        //            var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
        //            if (aclRecordToDelete != null)
        //                _aclService.DeleteAclRecord(aclRecordToDelete);
        //        }
        //    }
        //}

        //protected virtual void SaveStoreMappings(Category category, CategoryModel model)
        //{
        //    category.LimitedToStores = model.SelectedStoreIds.Any();
        //    _categoryService.UpdateCategory(category);

        //    var existingStoreMappings = _storeMappingService.GetStoreMappings(category);
        //    var allStores = _storeService.GetAllStores();
        //    foreach (var store in allStores)
        //    {
        //        if (model.SelectedStoreIds.Contains(store.Id))
        //        {
        //            //new store
        //            if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
        //                _storeMappingService.InsertStoreMapping(category, store.Id);
        //        }
        //        else
        //        {
        //            //remove store
        //            var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
        //            if (storeMappingToDelete != null)
        //                _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
        //        }
        //    }
        //}

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {


            //prepare model
            var model = _categoryModelFactory.PrepareCategorySearchModel(new CategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CategorySearchModel searchModel)
        {
            //prepare model
            var model = _categoryModelFactory.PrepareCategoryListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            //prepare model
            var model = _categoryModelFactory.PrepareCategoryModel(new JobServiceCategoryModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(JobServiceCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<JobServiceCategory>();

                _categoryService.InsertCategory(category);

                //activity log
                _customerActivityService.InsertActivity("AddNewJobServiceCategory",
                    string.Format(_localizationService.GetResource("ActivityLog.AddNewJobServiceCategory"), category.Name), category);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.JobServiceCategory.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _categoryModelFactory.PrepareCategoryModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            //try to get a category with the specified id
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                return RedirectToAction("List");

            //prepare model
            var model = _categoryModelFactory.PrepareCategoryModel(null, category);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(JobServiceCategoryModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            //try to get a category with the specified id
            var category = _categoryService.GetCategoryById(model.Id);
            if (category == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                category = model.ToEntity(category);
                _categoryService.UpdateCategory(category);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _categoryModelFactory.PrepareCategoryModel(model, category, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            //try to get a category with the specified id
            var category = _categoryService.GetCategoryById(id);
            if (category == null)
                return RedirectToAction("List");

            _categoryService.DeleteCategory(category);

            //activity log
            _customerActivityService.InsertActivity("DeleteCategory",
                string.Format(_localizationService.GetResource("ActivityLog.DeleteCategory"), category.Name), category);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Catalog.Categories.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            if (selectedIds != null)
            {
                _categoryService.DeleteCategories(_categoryService.GetCategoriesByIds(selectedIds.ToArray()).Where(p => _workContext.CurrentVendor == null).ToList());
            }

            return Json(new { Result = true });
        }

        #endregion
    }
}
