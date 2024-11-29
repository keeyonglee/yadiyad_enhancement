using System;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor note model
    /// </summary>
    public partial class VendorApplicationNoteModel : BaseNopEntityModel
    {
        #region Properties

        public int VendorApplicationId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorNotes.Fields.Note")]
        public string Note { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorNotes.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #endregion
    }
}