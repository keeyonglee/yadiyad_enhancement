using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Services.News;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Core.Domain.Common;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.FactoriesPro;
using YadiYad.Pro.Web.Infrastructure;
using YadiYad.Pro.Web.Models.Category;
using YadiYad.Pro.Web.Models.Common;
using YadiYad.Pro.Web.Models.News;

namespace YadiYad.Pro.Web.Areas.Pro.Controllers
{
    [Area(AreaNames.Pro)]
    [AutoValidateAntiforgeryToken]
    public class NewsController : Controller
    {
        #region Fields

            private readonly NewsModelFactory _newsModelFactory;
            private readonly NewsProService _newsService;

        #endregion

        #region Ctor

        public NewsController(NewsModelFactory newsModelFactory,
            NewsProService newsService)
        {
            _newsModelFactory = newsModelFactory;
            _newsService = newsService;
        }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            var model = _newsModelFactory.PrepareNewsSearchModel(new CategorySearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(CategorySearchModel searchModel)
        {
            var model = _newsModelFactory.PrepareNewsListModel(searchModel);

            return Json(model);
        }



        #endregion

        #region Edit

        public virtual IActionResult Edit(int id)
        {
            //try to get a category with the specified id
            var expertise = _newsService.GetById(id);
            if (expertise == null)
                return RedirectToAction("List");

            //prepare model
            var model = _newsModelFactory.PrepareNewsModel(null, expertise);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Edit(NewsModel model, bool continueEditing)
        {
            //try to get a category with the specified id
            var news = _newsService.GetById(model.Id);
            if (news == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                news = model.ToEntity(news);
                news.LanguageId = 1;
                _newsService.Update(news);

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = news.Id });
            }

            //prepare model
            model = _newsModelFactory.PrepareNewsModel(model, news, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Create

        public virtual IActionResult Create()
        {
            //prepare model
            var model = _newsModelFactory.PrepareNewsModel(new NewsModel(), null);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Create(NewsModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var news = model.ToEntity<NewsItem>();
                news.LanguageId = 1;

                _newsService.Insert(news);

                return RedirectToAction("Edit", new { id = news.Id });
            }

            //prepare model
            model = _newsModelFactory.PrepareNewsModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            //try to get a category with the specified id
            var expertise = _newsService.GetById(id);
            if (expertise == null)
                return RedirectToAction("List");

            _newsService.Delete(expertise);

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                _newsService.DeleteMany(_newsService.GetManyByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }

        #endregion
    }
}
