using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;

namespace Nop.Web.Areas.Pro.Models.Expertise
{
    public class ExpertiseSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Search Title")]
        public string SearchTitle { get; set; }
        [NopResourceDisplayName("Search Category")]
        public int SearchCategoryId { get; set; }
        public List<SelectListModel> JobServiceCategoryList { get; set; } = new List<SelectListModel>();


    }
}
