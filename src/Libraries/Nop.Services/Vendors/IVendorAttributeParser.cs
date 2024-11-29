using System.Collections.Generic;
using Nop.Core.Domain.Vendors;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute parser
    /// </summary>
    public partial interface IVendorAttributeParser : IVendorAttributeValueParser
    {
        /// <summary>
        /// Validates vendor attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Warnings</returns>
        IList<string> GetAttributeWarnings(string attributesXml, IList<VendorAttribute> vendorAttributes = null);
    }
}