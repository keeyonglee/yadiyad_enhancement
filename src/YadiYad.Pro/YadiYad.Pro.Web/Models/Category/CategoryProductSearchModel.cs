using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YadiYad.Pro.Web.Models.Category
{
    public partial class CategoryProductSearchModel : BaseSearchModel
    {
        #region Properties

        public int CategoryId { get; set; }

        #endregion
    }
}
