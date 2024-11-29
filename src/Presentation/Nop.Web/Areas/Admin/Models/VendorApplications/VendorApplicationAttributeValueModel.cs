using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor attribute value model
    /// </summary>
    public partial class VendorApplicationAttributeValueModel : BaseNopEntityModel, ILocalizedModel<VendorApplicationAttributeValueLocalizedModel>
    {
        #region Ctor

        public VendorApplicationAttributeValueModel()
        {
            Locales = new List<VendorApplicationAttributeValueLocalizedModel>();
        }

        #endregion

        #region Properties

        public int VendorApplicationAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<VendorApplicationAttributeValueLocalizedModel> Locales { get; set; }

        #endregion
    }

    public partial class VendorApplicationAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}