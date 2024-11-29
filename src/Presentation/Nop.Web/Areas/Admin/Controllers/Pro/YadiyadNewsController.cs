using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.News;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Pro.Factories;
using Nop.Web.Areas.Pro.Models.YadiyadNews;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Services.Services.Common;
using YadiYad.Pro.Web.Enums;
using YadiYad.Pro.Web.FactoriesPro;
using YadiYad.Pro.Web.Models.News;

namespace Nop.Web.Areas.Admin.Controllers.Pro
{
    

    public class YadiyadNewsController : BaseAdminController
    {
        #region Fields

        private readonly NewsProService _yadiyadNewsService;
        private readonly YadiyadNewsModelFactory _yadiyadNewsFactory;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public YadiyadNewsController(NewsProService yadiyadNewsService,
            YadiyadNewsModelFactory yadiyadNewsFactory,
            IPermissionService permissionService,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _yadiyadNewsService = yadiyadNewsService;
            _yadiyadNewsFactory = yadiyadNewsFactory;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _localizationService = localizationService;
    }

        #endregion

        #region List

        public virtual IActionResult Index()
        {
            return RedirectToAction("YadiyadNews");
        }

        public virtual IActionResult YadiyadNews(int? filterByNewsItemId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _yadiyadNewsFactory.PrepareNewsSearchModel(new YadiyadNewsSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(YadiyadNewsSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _yadiyadNewsFactory.PrepareNewsListModel(searchModel);

           
            return Json(model);
        }

        #endregion

        #region Create

        public virtual IActionResult YadiyadNewsCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //prepare model
            var model = _yadiyadNewsFactory.PrepareNewsModel(new YadiyadNewsModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult YadiyadNewsCreate(YadiyadNewsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var newsItem = model.ToEntity<NewsItem>();
                newsItem.CreatedOnUtc = DateTime.UtcNow;
                _yadiyadNewsService.Insert(newsItem);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Added"));


                if (!continueEditing)
                    return RedirectToAction("YadiyadNews");

                return RedirectToAction("YadiyadNewsEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = _yadiyadNewsFactory.PrepareNewsModel(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Edit

        public virtual IActionResult YadiyadNewsEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _yadiyadNewsService.GetById(id);
            if (newsItem == null)
                return RedirectToAction("YadiyadNews");

            //prepare model
            var model = _yadiyadNewsFactory.PrepareNewsModel(null, newsItem);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult YadiyadNewsEdit(YadiyadNewsModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _yadiyadNewsService.GetById(model.Id);
            if (newsItem == null)
                return RedirectToAction("YadiyadNews");

            if (ModelState.IsValid)
            {
                newsItem = model.ToEntity(newsItem);
                _yadiyadNewsService.Update(newsItem);

                if (!continueEditing)
                    return RedirectToAction("YadiyadNews");

                return RedirectToAction("YadiyadNewsEdit", new { id = newsItem.Id });
            }

            //prepare model
            model = _yadiyadNewsFactory.PrepareNewsModel(model, newsItem, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region Delete


        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageNews))
                return AccessDeniedView();

            //try to get a news item with the specified id
            var newsItem = _yadiyadNewsService.GetById(id);
            if (newsItem == null)
                return RedirectToAction("YadiyadNews");

            _yadiyadNewsService.Delete(newsItem);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.News.NewsItems.Deleted"));

            return RedirectToAction("YadiyadNews");
        }

        #endregion

    }
}
