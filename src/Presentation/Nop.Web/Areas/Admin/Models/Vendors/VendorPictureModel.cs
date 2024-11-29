using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Vendors;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor picture model
    /// </summary>
    public partial class VendorPictureModel : BaseNopEntityModel
    {
        #region Properties

        public int VendorId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.Picture")]
        public string PictureUrl { get; set; }

        [Required]
        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.OverrideAltAttribute")]
        public string OverrideAltAttribute { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.OverrideTitleAttribute")]
        public string OverrideTitleAttribute { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.VendorPictureType")]
        public int VendorPictureType { get; set; }

        [NopResourceDisplayName("Admin.Vendors.Pictures.Fields.VendorPictureType")]
        public string VendorPictureTypeName
        {
            get
            {
                var strValue = ((VendorPictureType?)VendorPictureType)?.GetDescription();

                return strValue;
            }
        }

        #endregion
    }
}