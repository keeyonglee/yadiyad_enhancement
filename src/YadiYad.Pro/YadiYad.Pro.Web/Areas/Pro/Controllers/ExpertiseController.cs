using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Common;
using YadiYad.Pro.Web.FactoriesPro;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.Category;
using YadiYad.Pro.Web.Models.Common;
using YadiYad.Pro.Web.Models.Expertise;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class ExpertiseController : BaseController
    {
        #region Fields

        private readonly ExpertiseModelFactory _expertiseModelFactory;
        private readonly ExpertiseService _expertiseService;
        private readonly JobServiceCategoryService _jobServiceCategoryService;

        #endregion

        #region Ctor

        public ExpertiseController(ExpertiseModelFactory expertiseModelFactory,
            ExpertiseService expertiseService,
            JobServiceCategoryService jobServiceCategoryService)
        {
            _expertiseModelFactory = expertiseModelFactory;
            _expertiseService = expertiseService;
            _jobServiceCategoryService = jobServiceCategoryService;
        }

        #endregion

        #region List

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            var model = _expertiseModelFactory.PrepareExpertiseSearchModel(new CategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CategorySearchModel searchModel)
        {
            var model = _expertiseModelFactory.PrepareExpertiseListModel(searchModel);

            return Json(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult Edit(int id)
        {
            //try to get a category with the specified id
            var expertise = _expertiseService.GetExpertById(id);
            if (expertise == null)
                return RedirectToAction("List");

            //prepare model
            var model = _expertiseModelFactory.PrepareExpertModel(null, expertise);

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
        public virtual IActionResult Edit(ExpertiseModel model, bool continueEditing)
        {
            //try to get a category with the specified id
            var expertise = _expertiseService.GetExpertById(model.Id);
            if (expertise == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {

                expertise = model.ToEntity(expertise);
                _expertiseService.UpdateExpertise(expertise);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = expertise.Id });
            }

            //prepare model
            model = _expertiseModelFactory.PrepareExpertModel(model, expertise, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Create

        public virtual IActionResult Create()
        {
            //prepare model
            var model = _expertiseModelFactory.PrepareExpertModel(new ExpertiseModel(), null);
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
        public virtual IActionResult Create(ExpertiseModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var expertise = model.ToEntity<Expertise>();
                _expertiseService.InsertExpertise(expertise);

                return RedirectToAction("Edit", new { id = expertise.Id });
            }

            //prepare model
            model = _expertiseModelFactory.PrepareExpertModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            //try to get a category with the specified id
            var expertise = _expertiseService.GetExpertById(id);
            if (expertise == null)
                return RedirectToAction("List");

            _expertiseService.Delete(expertise);

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                _expertiseService.DeleteMany(_expertiseService.GetExpertisesByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }

        #endregion
    }
}
