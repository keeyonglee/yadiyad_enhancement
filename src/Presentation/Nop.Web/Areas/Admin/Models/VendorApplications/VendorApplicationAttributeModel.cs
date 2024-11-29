using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.VendorApplications
{
    /// <summary>
    /// Represents a vendor attribute model
    /// </summary>
    public partial class VendorApplicationAttributeModel : BaseNopEntityModel, ILocalizedModel<VendorApplicationAttributeLocalizedModel>
    {
        #region Ctor

        public VendorApplicationAttributeModel()
        {
            Locales = new List<VendorApplicationAttributeLocalizedModel>();
            VendorApplicationAttributeValueSearchModel = new VendorApplicationAttributeValueSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.AttributeControlType")]
        public string AttributeControlTypeName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<VendorApplicationAttributeLocalizedModel> Locales { get; set; }

        public VendorApplicationAttributeValueSearchModel VendorApplicationAttributeValueSearchModel { get; set; }

        #endregion
    }

    public partial class VendorApplicationAttributeLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Fields.Name")]
        public string Name { get; set; }
    }
}