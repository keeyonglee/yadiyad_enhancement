using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Pro.Models.JobServiceCategory
{
    public class JobServiceCategoryModel : BaseNopEntityModel
    {
        #region Properties


        [NopResourceDisplayName("Admin.Catalog.JobServiceCategory.Fields.Name")]
        public string Name { get; set; }
        public bool Published { get; set; }
        public string PublishedString { get; set; }

        #endregion
    }
}
