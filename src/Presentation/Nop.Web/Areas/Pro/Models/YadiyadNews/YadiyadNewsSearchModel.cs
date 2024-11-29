using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;

namespace Nop.Web.Areas.Pro.Models.YadiyadNews
{
    public class YadiyadNewsSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Search Title")]
        public string SearchTitle { get; set; }
        public int NewsTypeId { get; set; }
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }
        public List<SelectListModel> NewsTypeList { get; set; } = new List<SelectListModel>();
    }

}
