using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace Nop.Web.Models.Vendors
{
    public partial class ApplyVendorModel
    {

        public string Name { get; set; }
        public int BusinessNatureCategoryId { get; set; }
        public string ProposedCategory { get; set; }
        public List<int> SampleProductPictureIds { get; set; }
        public int? CategoryId { get; set; }
    }
}