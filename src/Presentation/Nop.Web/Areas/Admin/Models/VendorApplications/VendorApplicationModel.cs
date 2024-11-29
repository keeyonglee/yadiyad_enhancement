using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor model
    /// </summary>
    public partial class VendorApplicationModel : BaseNopEntityModel
    {
        #region Ctor

        public VendorApplicationModel()
        {
            SampleProductPictureIds = new List<VendorApplicationSampleProductPicture>();
            AvailableEatCategories = new List<SelectListItem>();
            AvailableMartCategories = new List<SelectListItem>();

        }


        #endregion

        #region Properties
        public int CustomerId { get; set; }
        public string StoreName { get; set; }
        public int BusinessNatureCategoryId { get; set; }
        public string BusinessNatureCategory { get; set; }
        public int CategoryId { get; set; }
        public int EatCategoryId { get; set; }
        public int MartCategoryId { get; set; }

        public string Category { get; set; }
        public string ProposedCategory { get; set; }
        public string AdminComment { get; set; }
        public bool? IsApproved { get; set; }
        public string Status { get; set; }
        public bool Deleted { get; set; }
        public int CreatedById { get; set; }
        public int? UpdatedById { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public IList<VendorApplicationSampleProductPicture> SampleProductPictureIds { get; set; }
        public IList<SelectListItem> AvailableEatCategories { get; set; }
        public IList<SelectListItem> AvailableMartCategories { get; set; }

        #endregion
    }
}