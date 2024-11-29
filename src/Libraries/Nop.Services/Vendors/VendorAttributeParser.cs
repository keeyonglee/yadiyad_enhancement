using System.Collections.Generic;
using Nop.Core.Domain.Vendors;
using Nop.Services.Localization;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute parser implementation
    /// </summary>
    public partial class VendorAttributeParser : VendorAttributeValueParser, IVendorAttributeParser
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IVendorAttributeService _vendorAttributeService;

        #endregion

        #region Ctor

        public VendorAttributeParser(ILocalizationService localizationService,
            IVendorAttributeService vendorAttributeService) : base(vendorAttributeService)
        {
            _localizationService = localizationService;
            _vendorAttributeService = vendorAttributeService;
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Validates vendor attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        public virtual IList<string> GetAttributeWarnings(string attributesXml, IList<VendorAttribute> vendorAttributes = null)
        {
            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = ParseVendorAttributes(attributesXml);

            //validate required vendor attributes (whether they're chosen/selected/entered)

            if(vendorAttributes == null
                || vendorAttributes.Count == 0)
            {
                vendorAttributes = _vendorAttributeService.GetAllVendorAttributes();
            }

            var attributes2 = vendorAttributes;
            foreach (var a2 in attributes2)
            {
                if (!a2.IsRequired) 
                    continue;

                var found = false;
                //selected vendor attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id) 
                        continue;

                    var valuesStr = ParseValues(attributesXml, a1.Id);
                    foreach (var str1 in valuesStr)
                    {
                        if (string.IsNullOrEmpty(str1.Trim())) 
                            continue;

                        found = true;
                        break;
                    }
                }
                
                if (found) 
                    continue;

                //if not found
                var notFoundWarning = string.Format(_localizationService.GetResource("ShoppingCart.SelectAttribute"), _localizationService.GetLocalized(a2, a => a.Name));

                warnings.Add(notFoundWarning);
            }

            return warnings;
        }

        #endregion
    }
}