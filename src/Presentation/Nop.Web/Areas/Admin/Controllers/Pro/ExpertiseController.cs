using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.Expertise;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Web.Models.Common;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    public class ExpertiseController : BaseAdminController
    {
        #region Fields

        private readonly ExpertiseModelFactory _expertiseModelFactory;
        private readonly ExpertiseService _expertiseService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly JobServiceCategoryService _jobServiceCategoryService;


        #endregion

        #region Ctor

        public ExpertiseController(ExpertiseModelFactory expertiseModelFactory,
            ExpertiseService expertiseService,
            IPermissionService permissionService,
            IWorkContext workContext,
            JobServiceCategoryService jobServiceCategoryService)
        {
            _expertiseModelFactory = expertiseModelFactory;
            _expertiseService = expertiseService;
            _permissionService = permissionService;
            _workContext = workContext;
            _jobServiceCategoryService = jobServiceCategoryService;

        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();

            //prepare model
            var model = _expertiseModelFactory.PrepareExpertiseSearchModel(new ExpertiseSearchModel());
            var categoryList = _jobServiceCategoryService.GetAllJobServiceCategoris().ToList();

            foreach (var cat in categoryList)
            {
                var item = new SelectListModel();
                item.Id = cat.Id;
                item.Name = cat.Name;
                model.JobServiceCategoryList.Add(item);
            }
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(ExpertiseSearchModel searchModel)
        {
            //prepare model
            var model = _expertiseModelFactory.PrepareExpertiseListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Create

        public virtual IActionResult Create()
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
            //    return AccessDeniedView();

            //prepare model
            var model = _expertiseModelFactory.PrepareExpertiseModel(new ExpertiseModel(), null);

            var categoryList = _jobServiceCategoryService.GetAllJobServiceCategoris().ToList();

            foreach (var cat in categoryList)
            {
                var item = new SelectListModel();
                item.Id = cat.Id;
                item.Name = cat.Name;
                model.JobServiceCategoryList.Add(item);
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ExpertiseModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var category = model.ToEntity<Expertise>();

                _expertiseService.InsertExpertise(category);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _expertiseModelFactory.PrepareExpertiseModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult Edit(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            //try to get a category with the specified id
            var category = _expertiseService.GetExpertById(id);
            if (category == null)
                return RedirectToAction("List");

            //prepare model
            var model = _expertiseModelFactory.PrepareExpertiseModel(null, category);

            var categoryList = _jobServiceCategoryService.GetAllJobServiceCategoris().ToList();

            foreach (var cat in categoryList)
            {
                var item = new SelectListModel();
                item.Id = cat.Id;
                item.Name = cat.Name;
                model.JobServiceCategoryList.Add(item);
            }

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ExpertiseModel model, bool continueEditing)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            //try to get a category with the specified id
            var category = _expertiseService.GetExpertById(model.Id);
            if (category == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                category = model.ToEntity(category);
                _expertiseService.UpdateExpertise(category);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = category.Id });
            }

            //prepare model
            model = _expertiseModelFactory.PrepareExpertiseModel(model, category, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            //try to get a category with the specified id
            var category = _expertiseService.GetExpertById(id);
            if (category == null)
                return RedirectToAction("List");

            _expertiseService.Delete(category);

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageExpertise))
            //    return AccessDeniedView();
            if (selectedIds != null)
            {
                _expertiseService.DeleteMany(_expertiseService.GetExpertisesByIds(selectedIds.ToArray()).Where(p => _workContext.CurrentVendor == null).ToList());
            }

            return Json(new { Result = true });
        }

        #endregion
    }
}
