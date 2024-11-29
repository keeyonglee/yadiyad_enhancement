using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;

namespace Nop.Web.Areas.Pro.Models.Expertise
{
    public class ExpertiseModel : BaseNopEntityModel
    {
        public ExpertiseModel()
        {
            JobServiceCategoryList = new List<SelectListModel>();
        }
        public string Name { get; set; }
        public int JobServiceCategoryId { get; set; }
        public string JobServiceCategoryString{ get; set; }
        public bool IsOthers { get; set; }
        public bool Published { get; set; }
        public string PublishedString { get; set; }
        [NopResourceDisplayName("Category List")]
        public List<SelectListModel> JobServiceCategoryList { get; set; }

    }
}
