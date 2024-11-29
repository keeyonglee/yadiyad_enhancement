using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Nop.Web.Models.Vendors
{
    public partial class VendorApplicationModel : BaseNopModel
    {

        public string Name { get; set; }
        public int StoreType { get; set; }
        public int EatCategoryId { get; set; }
        public int MartCategoryId { get; set; }
        public string NewCategory { get; set; }
        public List<int> DownloadIds { get; set; }
    }
}