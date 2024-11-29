using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YadiYad.Pro.Web.Models.Common;

namespace YadiYad.Pro.Web.Models.Expertise
{
    public class ExpertiseModel : BaseNopEntityModel
    {
        public ExpertiseModel()
        {
            JobServiceCategoryList = new List<SelectListModel>();
        }
        public string Name { get; set; }
        public int JobServiceCategoryId { get; set; }
        public bool IsOthers { get; set; }
        public List<SelectListModel> JobServiceCategoryList { get; set; }
    }
}
