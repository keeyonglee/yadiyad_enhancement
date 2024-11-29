using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Components
{
    public class HomepageBlogViewComponent : NopViewComponent
    {
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly BlogSettings _blogSettings;

        public HomepageBlogViewComponent(
            IBlogModelFactory blogModelFactory,
            BlogSettings blogSettings)
        {
            _blogModelFactory = blogModelFactory;
            _blogSettings = blogSettings;
        }

        public IViewComponentResult Invoke()
        {
            if (!_blogSettings.Enabled)
                return Content("");

            var model = _blogModelFactory.PrepareHomepageBlogPostModel();
            return View(model);
        }
    }
}
