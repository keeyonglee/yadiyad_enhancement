using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Common
{
    /// <summary>
    /// Represents slide model
    /// </summary>
    public class GenericSliderModel : BaseNopEntityModel
    {
        /// <summary>
        /// Gets or sets picture url
        /// </summary>
        public string PictureUrl { get; set; }

        /// <summary>
        /// Gets or sets slider localized hyperlink
        /// </summary>
        public string Hyperlink { get; set; }

        /// <summary>
        /// Gets or sets slider localized description HTML
        /// </summary>
        public string Description { get; set; }
    }
}
